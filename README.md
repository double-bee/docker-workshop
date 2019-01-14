# docker-workshop

Het doel van deze hackathon is het spelen met Docker. We gebruiken hiervoor een testproject in .NET Core dat bestaat uit een website en een service. De website en de webservice zullen allebei in een eigen docker container draaien waarbij de website bevragingen doet bij de webservice.

In het begin zal elke stap beschreven en uitgelegd worden. Aan het einde wordt de hoeveelheid uitleg weinig tot niet en kan er lekker gespeeld worden.

1. Open een Developer Command Prompt for VS 2017
2. Maak een map aan met de naam `code` in de root van de C-schijf, dus `c:\code`
3. Zet de huidige directory naar die map
4. Voer het volgende commando uit om de source code van het project op te halen : 
    ```
    git clone https://github.com/double-bee/docker-workshop --branch step1 --single-branch
    ```
5. Open de solution docker-workshop.sln met Visual Studio 2017.
6. Configureer het start-up project
   - door rechts te klikken op de solution in de solution-explorer en te kiezen voor de menu-optie `Set Startup Projects..`. 
   - Kies daar de optie `Multiple startup projects` 
   - Zet de action van beide projecten op `Start`. 
   - Sluit het dialoog door op `Ok` te klikken. 
7. Start beide projecten door in Visual Studio op `F5` te drukken.
   Als het goed is krijg je een browser met 2 tabbladen. Op het ene tabblad zie je de url `http://localhost:55803/api/Groceries`, hierin zie je het resultaat van een aanroep naar de webservice. In het tweede tabblad zie je de url `http://localhost:53142`, hierin zie je de (erg simpele) website die zijn producten heeft opgehaald bij de webservice.
8. Sluit de browser tabbladen.
9. Publish de webservice door:
   - Klik met de rechter muisknop in de Solution Explorere op het project `Groceries.Service`. 
   - Kies `Publish...`
   - Kies in de linker lijst voor `Folder`
   - Type in `Choose a folder` de tekst `publish`
   - Klik op de knop `Publish`
Het gebouwde project staat nu in de map `Publish` (`C:\code\docker-workshop\Groceries.Service\Publish`). Kijk er maar even in. Hier staat alles dat nodig om de webservice te draaien, behalve .NET Core zelf.

We gaan nu een Docker image maken waarin alle benodigde bestanden van de webservice worden gezet. Dit doen we door een dockerfile te maken. Dit is het script dat beschrijft wat er in de image komt te staan. 

10. Dockerfile webservice maken:
   - Klik met de rechter muisknop in de Solution Explorere op het project `Groceries.Service`. 
   - Kies voor `Add` en dan `New Item...`. 
   - Type rechtsboven in het zoekvenster `Text` en type dan onderaan als `Name` de tekst `dockerfile`
   - Hernoem de net toegevoegde file `dockerfile.txt` naar `dockerfile`
   - Plak als inhoud voor deze dockerfile de volgende tekst:
   ```
   FROM microsoft/dotnet:2.1-aspnetcore-runtime
   WORKDIR /app
   COPY publish .
   ENTRYPOINT ["dotnet", "Groceries.Service.dll"]
   ```
   - Druk op SAVE

Elke docker image heeft een andere image als basis. Met de `FROM` regel kiezen wij een image waarin Microsoft alle benodigdheden voor een .NET Core applicatie heeft gezet. De image is zelf weer afgeleid van een Linux-gebaseerde image.
Met het commando `WORKDIR /app` geven we de huidige directory in de Docker image aan, net als de huidige directory in een command-prompt.
Daarna kopieren we de inhoud van de `publish` directory met daarin de inhoud van de webservice naar de Docker image.
Als laatste geven we aan welk programma gestart moet worden na het starten van de Docker container (`dotnet` met als argument `Groceries.Service.dll`).

11. Ga in de Visual Studio Command Prompt (die je nog open hebt of een nieuwe) naar `C:\code\docker-workshop\Groceries.Service`.
12. Voer het volgende commando uit om een Docker image te maken (let op, geen hoofdletters!):
```
docker build -t groceries.service .
```
Je hebt nu je eerste Docker image gemaakt. Gefeliciteerd, je leven is verrijkt!
Deze image staat niet in de project-directory, maar ergens centraal op het systeem.
 
13. Bekijk de aanwezige images op dit systeem met het volgende commando:
```
docker images
```

Je ziet twee images staan. Een image van Microsoft die we als basis hebben gebruikt en de image die we net zelf hebben gemaakt. Onthoud de grootte van groceries.service.

14. Start nu een container van de net gemaakte image:
'''
docker run -d -p 80:80 groceries.service
'''

Na het uitvoeren van het 'run' commando is een container gestart. Het nummer dat op het scherm verschijnt is de id van de container.

15. Open een browser en ga naar het adres 'http://localhost/api/Groceries'. Je ziet dat de webservice draait en resultaten teruggeeft. Een lijst met boodschappen.

16. Typ in de command prompt het volgende commando in: 'docker ps -a'

Je ziet nu een lijst met containers die aanwezig zijn op het systeem. Dit kunnen containers zijn die draaien en die gestopt zijn. Zoek de container id op van de container die gemaakt is van de image groceries.service. Nu is dat er maar 1 maar in een load balancing situatie kunnen er meerdere containers draaien die dezelfde image als basis hebben. Vandaar dat je niet de naam van de image kunt gebruiken om bijvoorbeeld aan te geven welke container er moet stoppen.

17. Stop de container met het volgende commando: 'docker stop CONTAINER_ID'. Als je nu de browser ververst zul je zien dat de service niet meer draait.

18. Controleer de status van de container met het commando 'docker ps -a' en verwijder de container met het commando 'docker rm CONTAINER_ID'.

We hebben net buiten docker de software gebouwd en die software in een image gezet waar al .NET Core in anwezig was. Met deze image kun je vervolgens op elk willekeurig systeem een container starten en er zeker van zijn dat het werkt.
We kunnen echter nog een verbetering maken. We kunnen het bouwen van de software ook doen tijdens het maken van een image. De compiler die je hiervoor nodig hebt kun je ook gewoon als image downloaden. Het voordeel hiervan is dat zelfs de build omgeving altijd gelijk zal zijn. We gaan hiervoor de dockerfile aanpassen.

19. Wijzig de dockerfile als volgt en bouw de image opnieuw (zie stap 12):
'''
FROM microsoft/dotnet:2.1-sdk
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o ../app
ENTRYPOINT ["dotnet", "/app/Groceries.Service.dll"]
'''

De eerste regel haalt nu een andere image binnen. In dit image zit niet alleen de .NET Core runtime zoals in de vorige image maar ook de SDK met de compiler. Vervolgens kopieeren we de sourcecode naar de image en roepen dotnet publish aan. Dotnet publish download benodigde packages, bouwt het project en kopieert het resultaat naar de map '/app'.

20. Typ in de command prompt het volgende commando in: 'docker images'. Kijk naar de grootte van ons gemaakte image.

Deze image is aanzienlijk groter. In deze image zit nu een hele .NET Core SDK, de sourcecode van het project en de uitvoerbare binaries. Dat is natuurlijk geen gewenste situatie. Tijd om dit te verbeteren met behulp van een multi stage dockerfile.

21. Wijzig de dockerfile als volgt en bouw de image opnieuw:
'''
FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o ../app

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "/app/Groceries.Service.dll"]
'''

De eerste FROM haalt de .NET Core SDK image binnen en geeft ook een alias 'build' aan de image die we maken. Daar wordt de sourcecode in gekopieerd.
Het RUN commando zorgt ervoor dat er een container wordt gestart van deze 'build' image en dat het project wordt gebouwd. Het resultaat komt in de map '/app' te staan.

Vervolgens halen we een .NET Core runtime image binnen en kopieeren we uit de 'build' image de inhoud van de map '/app' naar de nieuwe image.
De laatste FROM bepaalt welke image er over blijft, elke dockerfile levert altijd 1 image op, alle tijdelijke containers worden automatisch verwijderd. De 'build' image wordt bewaard. Docker vergelijkt een volgende keer dat de dockerfile aangeroepen wordt de file met de image die nog bewaard was. Als er niets is gewijzigd aan de commando's kan de oude image direct gebruikt worden. 

22. Typ in de command prompt het volgende commando in: 'docker images'. Kijk naar de grootte van ons gemaakte image. Stukken beter.

De webservice is zo geconfigureerd dat hij zijn data opslaat in '/data/groceries.json'. Maar waar staat die data dan? Die staat in de container. Maar dat betekent dat de data weg is als je de container verwijdert. Er zijn meerdere manieren om met persistente data om te gaan in docker. Volumes en bind mounts. Volumes zijn virtuele schijven die je koppelt aan een container. De inhoud hiervan beheert Docker zelf ergens. Je kan ook een map in een docker container laten wijzen naar een map op de host. Alles dat de container dan in die map schrijft komt dan terecht in een aangewezen map op de host. Dat laatste gaan wij nu ook doen.

23. Maak de map 'c:\temp\data' aan.

24. Typ in de command prompt het volgende commando in:
'''
docker run -d -p 80:80 -v c:\temp\data:/data groceries.service
'''

We hebben hier tegen docker gezegd dat de map '/data' in de container gekoppeld moet worden aan de map 'c:\temp\data'. Alles dat de container schrijft in '/data' wordt geschreven in de map 'c:\temp\data'. Overigens zie je aan de parameter '/data' dat het gaat om een linux container en deze draaien we in Windows.

25. Open het bestand 'c:\temp\data\groceries.json' en voeg een boodschap toe. Open daarna de browser en ga naar URL 'http://localhost/api/Groceries' om te controleren of de boodschap inderdaad wordt weergegeven door de service.

Wat we nog niet hebben getest is de parameter '-p 80:80' in het 'docker run' commando. Deze parameter zegt dat poort 80 buiten de container wordt gekoppeld aan poort 80 in de container.

26. Typ in de command prompt het volgende commando in:
'''
docker run -d -p 81:80 groceries.service
'''

27. Typ in de command prompt het volgende commando in: 'docker ps -a'

Nu draaien er twee webservices. Een luistert op poort 80 en een luistert op poort 81. Ze hebben allebei dezelfde image als basis. Open daarna de browser en ga naar URL 'http://localhost:81/api/Groceries' om dit te controleren. Je zult ook zien dat ze allebei een ander resultaat teruggeven. De service op poort 80 heeft als het goed is een boodschap meer. Hoe komt dat?

28. Ga naar Visual Studio 2017 en voeg voor de groceries.web ook een dockerfile toe.

29. Bouw de docker image groceries.web, start er een container van op en test of de website werkt.

30. Verwijder alle containers door ze te stoppen met docker stop 'CONTAINER_ID' en daarna docker rm 'CONTAINER_ID'

31. Ga in de command prompt naar de map "c:\docker-workshop\" en voer het volgende commando uit:
'''
git checkout step2
'''

We gaan nu aan de slag met docker-compose. Docker-compose is een tool waarmee je applicaties die uit meerdere docker containers bestaan kunt draaien. Onze applicatie bestaat uit een website die zijn gegevens ophaalt bij een webservice.



testen



https://docs.microsoft.com/en-us/azure/virtual-machines/windows/using-visual-studio-vm

- installeer [Visual Studio Code](https://code.visualstudio.com/download)
- file -> preferences -> settings -> proxy -> http://proxy04.wgwa.local:8080
- docker -> settings -> proxy -> http://proxy04.wgwa.local:8080
- installeer [GIT voor Windows](https://git-scm.com/download/win)
- clone https://github.com/double-bee/docker-workshop
- installeer visual studio code
- installeer omnisharp plugin voor visual studio code
- maak een map aan met de naam Groceries.Service
- cd naar de map
- dotnet new webapi
- code .
- dotnet run
- ga in een browser naar http://localhost:5000/api/Values om het project te testen
- maak een map aan met de naam Groceries.Web
- cd naar de map
- dotnet new mvc
- code .
- dotnet add package Microsoft.AspNet.WebApi.Client
- cd naar Groceries.Service
- docker build --build-arg HTTP_PROXY=http://proxy04.wgwa.local:8080 -t groceries.service .
- docker run --rm -p 80:80 -v c:\temp\:c:\temp groceries.service
