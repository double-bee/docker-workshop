# docker-workshop

Het doel van deze hackathon is het spelen met Docker. We gebruiken hiervoor een testproject in .NET Core dat bestaat uit een website en een service. De website en de webservice zullen allebei in een eigen docker container draaien waarbij de website bevragingen doet bij de webservice.

In het begin zal elke stap beschreven en uitgelegd worden. Aan het einde wordt de hoeveelheid uitleg weinig tot niets en kan er lekker gespeeld worden.

1. Open een Developer Command Prompt for VS 2017
2. Maak een map aan met de naam `code` in de root van de C-schijf, dus `c:\code`
3. Zet de huidige directory naar die map
4. Voer het volgende commando uit om de source code van het project op te halen : 
    ```
    git clone https://github.com/double-bee/docker-workshop --branch step1
    ```
5. Open de solution docker-workshop.sln met Visual Studio 2017.
6. Configureer het start-up project
   - door rechts te klikken op de solution in de solution-explorer en te kiezen voor de menu-optie `Set Startup Projects..`. 
   - Kies daar de optie `Multiple startup projects` 
   - Zet de action van beide projecten op `Start`. 
   - Sluit het dialoog door op `Ok` te klikken. 
7. Start beide projecten door in Visual Studio op `F5` te drukken.
   Als het goed is opent je een browser met 2 tabbladen. Op het ene tabblad zie je de url `http://localhost:55803/api/Groceries`, hierin zie je het resultaat van een aanroep naar de webservice. In het tweede tabblad zie je de url `http://localhost:53142`, hierin zie je de (erg simpele) website. Later in de workshop gaat deze website een lijst met producten ophalen bij de service.
8. Sluit de browser tabbladen.

Het publishen van een project houdt in dat je de sourceode omzet naar binaries en alle bestanden die nodig zijn voor het draaien van het project in een map worden gezet.

9. Publish de webservice door:
   - Klik met de rechter muisknop in de Solution Explorere op het project `Groceries.Service`. 
   - Kies `Publish...`
   - Kies in de linker lijst voor `Folder`
   - Type in `Choose a folder` de tekst `publish`
   - Klik op de knop `Publish`
Het gebouwde project staat nu in de map `Publish` (`C:\code\docker-workshop\Groceries.Service\Publish`). Kijk er maar even in. Hier staat alles dat nodig om de webservice te draaien, behalve .NET Core. Dit project is afhankelijk van andere software, namelijk .NET Core. Normaal gesproken zou je dit op de server installeren waar deze service moet gaan draaien.

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
Als laatste geven we aan welk programma gestart moet worden bij het starten van de Docker container (`dotnet` met als argument `Groceries.Service.dll`).

11. Ga in de Visual Studio Command Prompt (die je nog open hebt of een nieuwe) naar de map `C:\code\docker-workshop\Groceries.Service`.
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

Je ziet minstens twee images staan. Een image van Microsoft die we als basis hebben gebruikt en de image die we net zelf hebben gemaakt. Onthoud de grootte van groceries.service.

14. Start nu een container van de net gemaakte image:
```
docker run -d -p 80:80 groceries.service
```

Na het uitvoeren van het `run` commando is een container gestart. Het nummer dat op het scherm verschijnt is de id van de container.

15. Open een browser en ga naar het adres `http://localhost/api/Groceries`. Je ziet dat de webservice draait en resultaten teruggeeft. Een lijst met boodschappen.

16. Typ in de command prompt het volgende commando in: `docker ps -a`

Je ziet nu een lijst met containers die aanwezig zijn op het systeem. Dit kunnen containers zijn die draaien en die gestopt zijn. Zoek de container id op van de container die gemaakt is van de image groceries.service. Nu is dat er maar 1 maar in een load balancing situatie kunnen er meerdere containers draaien die dezelfde image als basis hebben. Vandaar dat je niet de naam van de image kunt gebruiken om bijvoorbeeld aan te geven welke container er moet stoppen.

17. Stop de container met het volgende commando: `docker stop CONTAINER_ID`. Je hoeft niet de hele container id in te vullen, alleen de eerste drie letters is voldoende als deze uniek zijn onder de containers. Als je nu de browser ververst zul je zien dat de service niet meer draait. 

18. Controleer de status van de container met het commando `docker ps -a` en verwijder de container met het commando `docker rm CONTAINER_ID`.

We hebben net buiten docker de software gebouwd en die software in een image gezet waar al .NET Core in aanwezig was. Met deze image kun je vervolgens op elk willekeurig systeem een container starten en er zeker van zijn dat het werkt.
We kunnen echter nog een verbetering maken. We kunnen het bouwen van de software ook doen tijdens het maken van een image. De compiler die je hiervoor nodig hebt kun je ook gewoon als image downloaden. Het voordeel hiervan is dat zelfs de build omgeving altijd gelijk zal zijn. We gaan hiervoor de dockerfile aanpassen.

19. Wijzig de dockerfile als volgt en bouw de image opnieuw (zie stap 12):
```
FROM microsoft/dotnet:2.1-sdk
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o ../app
ENTRYPOINT ["dotnet", "/app/Groceries.Service.dll"]
```

De eerste regel haalt nu een andere image binnen. In dit image zit niet alleen de .NET Core runtime zoals in de vorige image maar ook de SDK (Software Development Kit) met de compiler. Vervolgens kopieeren we de sourcecode naar de image en roepen dotnet publish aan. Dotnet publish download benodigde packages, bouwt het project en kopieert het resultaat naar de map `/app`.

20. Typ in de command prompt het volgende commando in: `docker images`. Kijk naar de grootte van ons gemaakte image.

Deze image is aanzienlijk groter. In deze image zit nu een hele .NET Core SDK, de sourcecode van het project en de uitvoerbare binaries. Dat is natuurlijk geen gewenste situatie. Tijd om dit te verbeteren met behulp van een multi stage dockerfile.

21. Wijzig de dockerfile als volgt en bouw de image opnieuw:
```
FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o ../app

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "/app/Groceries.Service.dll"]
```

De eerste FROM haalt de .NET Core SDK image binnen en geeft ook een alias `build` aan de image die we maken. Daar wordt de sourcecode in gekopieerd.
Het RUN commando zorgt ervoor dat er een container wordt gestart van deze `build` image en dat het project wordt gebouwd. Het resultaat komt in de map `/app` te staan.

Vervolgens halen we een .NET Core runtime image binnen en kopieeren we uit de `build` image de inhoud van de map `/app` naar de nieuwe image.
De laatste FROM bepaalt welke image er over blijft, elke dockerfile levert altijd 1 image op, alle tijdelijke containers worden automatisch verwijderd. De `build` image wordt bewaard. Docker vergelijkt een volgende keer dat de dockerfile aangeroepen wordt de file met de image die nog bewaard was. Als er niets is gewijzigd aan de commando`s kan de oude image direct gebruikt worden. 

22. Typ in de command prompt het volgende commando in: `docker images`. Kijk naar de grootte van ons gemaakte image. Stukken beter.

De webservice is zo geconfigureerd dat hij zijn data opslaat in `/data/groceries.json`. Maar waar staat die data dan? Die staat in de container. Maar dat betekent dat de data weg is als je de container verwijdert. Er zijn meerdere manieren om met persistente data om te gaan in docker. Volumes en bind mounts. Volumes zijn virtuele schijven die je koppelt aan een container. De inhoud hiervan beheert Docker zelf ergens. Je kan ook een map in een docker container laten wijzen naar een map op de host. Alles dat de container in die map schrijft komt dan terecht in een aangewezen map op de host. Dat laatste gaan wij nu ook doen.

23. Maak de map `c:\temp\data` aan.

24. Typ in de command prompt het volgende commando in:
```
docker run -d -p 80:80 -v c:\temp\data:/data groceries.service
```

We hebben hier tegen docker gezegd dat de map `/data` in de container gekoppeld moet worden aan de map `c:\temp\data`. Alles dat de container schrijft in `/data` wordt geschreven in de map `c:\temp\data`. Overigens zie je aan de parameter `/data` dat het gaat om een linux container en deze draaien we in Windows.

25. Ga naar URL `http://localhost/api/Groceries`. Open daarna het bestand `c:\temp\data\groceries.json` en voeg een boodschap toe. Open daarna de browser en ga naar URL `http://localhost/api/Groceries` om te controleren of de boodschap inderdaad wordt weergegeven door de service.

Wat we nog niet hebben getest is de parameter `-p 80:80` in het `docker run` commando. Deze parameter zegt dat poort 80 buiten de container wordt gekoppeld aan poort 80 in de container.

26. Typ in de command prompt het volgende commando in:
```
docker run -d -p 81:80 groceries.service
```

27. Typ in de command prompt het volgende commando in: `docker ps -a`

Nu draaien er twee webservices. Een luistert op poort 80 en een luistert op poort 81. Ze hebben allebei dezelfde image als basis. Open daarna de browser en ga naar URL `http://localhost:81/api/Groceries` om dit te controleren. Je zult ook zien dat ze allebei een ander resultaat teruggeven. De service op poort 80 heeft als het goed is een boodschap meer. Hoe komt dat?

28. Ga naar Visual Studio 2017, voeg voor de groceries.web ook een dockerfile toe en maak deze compleet voor het groceries.web project.

29. Bouw de docker image groceries.web, start er een container van op en test of de website werkt op `http://localhost:GEKOZEN_POORT`.

30. Verwijder alle containers door ze te stoppen en te verwijderen.

31. Sluit de solution in Visual Studio.

32. Ga in de Command prompt naar de map `c:\code\`. Voer hier het volgende commando uit:
```
git clone https://github.com/double-bee/docker-workshop --branch step2 docker-workshop2
```

33. Ga in de command prompt naar deze nieuwe map en open de solution met visual studio.

We gaan nu met docker-compose aan de slag. Hiermee kun je meerdere containers tegelijk starten die eventueel ook samen in een virtueel netwerk met elkaar communiceren.

34. In de solution explorer van Visual Studio, maak onder de solution items een bestand aan met de naam `docker-compose.yml`.

35. Voeg de volgende code toe aan het bestand:
```
version: '3.0'

services:
  groceries.service:
    image: groceries.service
    build:
      context: Groceries.Service
      dockerfile: Dockerfile

  groceries.web:
    image: groceries.web
    build:
      context: Groceries.Web
      dockerfile: Dockerfile
    ports:
      - "85:80"
    environment:
      - GroceryServiceUri=http://groceries.service/api/Groceries
    depends_on:
      - groceries.service
```
Deze file definieert twee services en geeft aan hoe ze gebouwd moeten worden, namelijk met de dockerfile in de beide mappen. Als extra toevoeging geven we nog een environment variabele mee aan de groceries.web website. Deze website moet namelijk weten waar hij zijn data vandaan moet halen, waar de groceries.service op te bereiken is. Hier werken we niet met vaste adressen maar kan verwezen worden naar de naam van de service, de webservice is op deze naam in het virtuele netwerk te bereiken.

36. Voer het volgende commando uit in de map `c:\code\docker-workshop2`
```
docker-compose up --build
```
Door het commando `docker-compose up` te geven worden de twee images eventueel gebouwd en daarna worden er containers van gestart. Je kunt de website testen door de URL http://localhost:85 in de browser te tikken. De website roept de webservice aan, ontvangt een lijst met boodschappen en toont deze op het scherm.

Eerder tijdens de workshop hebben we ervoor gezorgd door de -v parameter aan `docker run` mee te geven dat de webservice zijn data op de host opsloeg. Dat is nu niet meer het geval nu we docker-compose gebruiken aangezien we geen `docker run` aanroepen. Uiteraard is dit wel te configureren in de `docker-compose.yml` file.

37. Pas de `docker-compose.yml` file aan zodat de data van de groceries.service weer in `c:\temp\data` wordt opgeslagen. Je mag zelf uitzoeken hoe. De documentatie is hier te vinden: https://docs.docker.com/compose/compose-file/

Momenteel is de yaml file zo geconfigureerd dat groceries.service alleen bereikbaar is in het opgezette virtuele netwerk. Hierdoor kan de website wel bij de service maar wij niet meer.

38.  Zorg er voor dat de webservice wel door jouw browser bereikbaar is.

Je kan ook zelf commando's uitvoeren in een draaiende container met het commando `docker exec`. Probeer het maar met de groceries.service container.

39. Voer het volgende commando uit en kijk wat rond in het file system van de container. Weet je de groceries.json te vinden? Je kunt groceries.json op je eigen machine wijzigen en die wijziging bekijken in de draaiende container.

```
docker exec -it <container id> bash
```

Op het internet zijn duizenden images van softwarepakketten beschikbaar die je kunt downloaden en draaien op je eigen PC maar bijvoorbeeld ook op een Synology NAS. Dit werkt een stuk fijner dan packages downloaden en installeren met de angst dat de package rotzooi achter laat als je de package probeert te verwijderen.

40. Start Microsoft SQL Server met het volgende commando.
```
docker run -d -p 1433:1433 -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password1." mcr.microsoft.com/mssql/server
```

Zo, nu kun je zeggen dat je vandaag in drie minuten SQL Server hebt geinstalleerd. Bij het starten van dit commando merkt Docker dat de image met de naam `mcr.microsoft.com/mssql/server` die meegegeven werd niet beschikbaar is op het systeem. Hij probeert daarom de image te vinden op Docker hub. Dit is de "marktplaats" voor Docker images. Hier is onder andere de linux image van SQL server te vinden. Kijk maar eens op https://hub.docker.com/_/microsoft-mssql-server.
Je kunt dit image bijvoorbeeld gebruiken voor het draaien van geautomatiseerde testen. Dat gaat als volgt: start je eigen te testen software in een container, start de SQL server container, vul de database, draai de test, stop de containers, opgeruimd staat netjes. Dit alles doe je natuurlijk met docker-compose. Kijk maar naar het volgende voorbeeld:

```
version: "3"

services:

  product-api:
    image: pocsoccontainerregistry.azurecr.io/producten.api:latest
    ports:
      - "8000:80"
    environment:
      - ConnectionStrings__ProductenContext=Server=mssql-deployment,1433;Database=ProductDB;MultipleActiveResultSets=true;User Id=SA;password=Sa!123!password
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - mssql-deployment
 
  mssql-deployment:
    image: mcr.microsoft.com/mssql/server
    ports:
      - "1533:1433"
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Sa!123!password
  
  test:
    depends_on:
      - product-api
    image: postman/newman:alpine
    volumes:
      - ./Postman:/etc/newman
    command: run /etc/newman/Product_API.postman_collection.json -e "/etc/newman/Test.postman_environment.json" --reporters cli,junit --reporter-junit-export "/etc/newman/Output/output.xml"

```

Er worden drie containers gestart. Een met Sql server, een met de te testen service (producten.api) en een container met het programma Postman die testen uitvoert op de service. Na het starten van deze .yaml file staan de resultaten van de testen in de file output.xml.

Wist je dat Kubernetes tegenwoordig ook in Docker voor Windows zit? Onderin de taakbalk zie je het Docker logo. In de settings van Docker kun je Kubernetes aan zetten. Doe dit maar.
De docker-compose.yaml file kun je converteren naar een yaml file die Kubernetes begrijpt. Dit kun je doen met de volgende tool: https://github.com/kubernetes/kompose/releases/download/v1.17.0/kompose-windows-amd64.exe

41. Verwijder alle containers in Docker.

42. Wijzig de docker-compose.yaml naar:
```
version: '3.0'

services:
  groceries.service:
    image: groceries.service
    build:
      context: Groceries.Service
      dockerfile: Dockerfile
    ports:
      - "80:80"

  groceries.web:
    image: groceries.web
    build:
      context: Groceries.Web
      dockerfile: Dockerfile
    ports:
      - "85:80"
    environment:
      - GroceryServiceUri=http://groceries.service/api/Groceries
    depends_on:
      - groceries.service
```

43. Converteer de `docker-compose.yaml` file naar een file die Kubernetes begrijpt.
```
kompose-windows-amd64 convert
```

Er worden vier files gemaakt. Helaas zijn deze niet direct te gebruiken.

44. Voeg in de twee files die eindigen op deployment.yaml de regel `imagePullPolicy: Never` toe onder de regel die begint met `image:`

45. Voeg boven de regel met het woord 'status' in het bestand `groceries-web-service.yaml` de regel 'type: LoadBalancer' toe

46. Wijzig in het bestand `groceries-web-deployment.yaml` de tekst `value: http://groceries.service/api/Groceries` naar `value: http://groceries-service/api/Groceries`

47. Voer de volgende commando's uit
```
kubectl apply -f groceries-service-deployment.yaml
kubectl apply -f groceries-service-service.yaml
kubectl apply -f groceries-web-deployment.yaml
kubectl apply -f groceries-web-service.yaml
```

Als het goed is gegaan kan je nu de website http://localhost:85 openen. Deze website en de service die aangeroepen wordt door de website draaien nu in Kubernetes.

48. Stop de groceries.web container en kijk daarna naar de status van de container. Hij draait weer! Kubernetes detecteerde dat de container niet meer draaide, heeft de oude verwijderd en heeft een nieuwe gestart.

49. Wijzig nu de file 'groceries-service-deployment.yaml' zodanig dat er meerdere instanties, bijvoorbeeld 10, van de service gaan draaien. Voor hulp, kijk in https://kubernetes.io/docs/concepts/workloads/controllers/deployment/ en https://kubernetes.io/docs/reference/generated/kubectl/kubectl-commands#apply

50. Bekijk nu het aantal containers dat er draait in Docker. Het leuke is dat als de website een verzoek doet bij de webservice, deze aanroepen netjes worden verdeeld over de beschikbare services, automatische load-balancing.
