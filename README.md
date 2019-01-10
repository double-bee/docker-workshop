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
8. Sluit de browser.
9. Publish de webservice door:
   - Klik met de rechter muisknop in de Solution Explorere op het project `Groceries.Service`. 
   - Kies `Publish...`
   - Kies in de linker lijst voor `Folder`
   - Type in `Choose a folder` de tekst `publish`
   - Klik op de knop `Publish`
Het gebouwde project staat nu in de map `Publish` (`C:\code\docker-workshop\Groceries.Service\Publish`)

We gaan nu een Docker image maken waarin alle benodigde bestanden van de webservice worden gezet. Dit doen we door een dockerfile te maken. Dit is het script dat beschrijft wat er in de image komt te staan. 

10. Dockerfile webservice maken:
   - Klik met de rechter muisknop in de Solution Explorere op het project `Groceries.Service`. 
   - Kies voor `Add` en dan `New Item...`. 
   - Type rechtsboven in het zoekvenster `Text` en type dan onderaan als `Name` de tekst `dockerfile`
   - Hernoem de net toegevoegde file `dockerfile.txt` naar `dockerfile`
   - Plak als inhoud voor deze dockerfile de volgende tekst:
   ```
   FROM microsoft/dotnet:2.1-runtime
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
docker build -t groceries.web .
```
Je hebt nu je eerste Docker image gemaakt. Gefeliciteerd, je leven is verrijkt!
Deze image staat niet in de project-directory, maar ergens centraal op het systeem.
 
13. Bekijk de aanwezige images op dit systeem met het volgende commando (onthoud de grootte van groceries.web):
```
docker images
```

Je ziet twee images staan. Een image van Microsoft die we als basis hebben gebruikt en de image die we net zelf hebben gemaakt.

14. Start nu een container van de net gemaakte image:
'''
docker run -d -p 80:80 groceries.web
'''

15. Open een browser en ga naar het adres 'http://localhost/api/Groceries'. Je ziet dat de webservice draait en resultaten teruggeeft.

16. Typ in de command prompt het volgende commando in: 'docker ps -a'

Je ziet nu een lijst met containers die aanwezig zijn op het systeem. Dit kunnen containers zijn die draaien en die gestopt zijn.

17. Stop de container met het volgende commando: 'docker stop groceries.web'. Als je nu de browser ververst zul je zien dat de service niet meer draait.

18. Controleer de status van de container met het commando 'docker ps -a'.

We hebben net buiten docker de software gebouwd en die software, met zijn afhankelijkheden (.NET Core) in een image gezet. Met deze image kun je vervolgens op elk willekeurig systeem een container starten en er zeker van zijn dat het werkt.
We kunnen echter nog een verbetering maken. We kunnen het bouwen van de software ook doen tijdens het maken van een image. De compiler die je hiervoor nodig hebt kun je ook gewoon als image downloaden. Het voordeel hiervan is dat zelfs de build omgeving altijd gelijk zal zijn. We gaan hiervoor de dockerfile aanpassen.

19. Wijzig de dockerfile als volgt:
'''
FROM microsoft/dotnet:2.2-sdk
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o ../app
ENTRYPOINT ["dotnet", "/app/Groceries.Service.dll"]
'''

De eerste regel haalt nu een andere image binnen. In dit image zit niet alleen de .NET Core runtime zoals in de vorige image maar ook de SDK met de compiler. Vervolgens kopieeren we de sourcecode naar de image en roepen dotnet publish aan. Dotnet publish download benodigde packages, bouwt het project en kopieert het resultaat naar de map /app.

20. Typ in de command prompt het volgende commando in: 'docker images'. Kijk naar de grootte van ons gemaakte image.

Deze image is aanzienlijk groter. In deze image zit nu een hele .NET Core SDK, de sourcecode van het project en natuurlijk de uitvoerbare binaries. Dat is natuurlijk geen gewenste situatie. Tijd om dit te verbeteren met behulp van een multi stage dockerfile.

21. Wijzig de dockerfile als volgt:
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
Het RUN commando zorgt ervoor dat er een container wordt gestart van deze image en dat het project wordt gebouwd. Het resultaat komt in de map /app te staan.

Vervolgens halen we een .NET Core runtime image binnen en kopieeren we uit de 'build' image de inhoud van de map /app naar de nieuwe image.
De laatste FROM bepaalt welke image er over blijft, elke dockerfile levert altijd 1 image op, alle andere tijdelijke images en containers worden automatisch verwijderd.

22. Typ in de command prompt het volgende commando in: 'docker images'. Kijk naar de grootte van ons gemaakte image.


- nu zelfde maar dan met volume mapping
- dockerfile website maken en met elkaar laten praten
- docker compose





testen

docker compose



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
