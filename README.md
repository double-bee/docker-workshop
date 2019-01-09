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
9. Dockerfile webservice maken:
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

10. Ga in de Visual Studio Command Prompt (die je nog open hebt of een nieuwe) naar `C:\code\docker-workshop\Groceries.Service`.
11. Voer het volgende commando uit om een Docker image te maken (let op, geen hoofdletters!):
```
docker build -t groceries.web .
```
Je hebt nu je eerste Docker image gemaakt, gefeliciteerd je leven is verrijkt!
Deze image staat niet in de project-directory, maar ergens centraal op het systeem.
12. Bekijk de aanwezige images op dit systeem met het volgende commando (onthoudt de grootte van groceries.web):
```
docker images
```

13. docker run webservice

- docker file is groot, daarna multistage build om kleiner te maken
- nu zelfde maar dan met volume mapping
- dockerfile website maken en met elkaar laten praten
- docker compose





Deze solution bestaat uit twee projecten, een webservice waar boodschappen toegevoegd, verwijderd en opgevraagd kunnen worden en een website die een user interface biedt voor deze acties.

- Ga terug naar de command prompt
- ga naar de directory c:\code\docker-workshop\Groceries.Service
- Voer het volgende commando uit:
    ```
    dotnet publish -o out
    ```
Nu is de webservice gebouwd en gepublished in het mapje out. Het resultaat hiervan gaan we in een Docker image stoppen. Hiervoor moeten we een Dockerfile maken. Dat is het recept van de Docker image.
- Maak een bestand aan met de naam DockerFile. Zet het volgende in dat bestand:
    ```
    FROM microsoft/dotnet:2.1-aspnetcore-runtime
    WORKDIR /app
    COPY out .
    ENTRYPOINT ["dotnet", "/app/Groceries.Service.dll"]
    ```


- Voer het volgende commando uit
    ```
    docker build -t groceries.service .
    ```
We hebben nu een image gemaakt met de naam groceries.service. De inhoud van die image is wat in de map out staat. We kunnen nu docker vragen welke images er op het systeem staan.
- Voer het volgende commando uit:
    ```
    docker image
    ```
 
multistage
Service docker build en run

Network opzetten
Beide run met netwerkopties 
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
