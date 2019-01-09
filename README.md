# docker-workshop

Het doel van deze hackathon is het spelen met Docker. We gebruiken hiervoor een testproject in .NET Core dat bestaat uit een website en een service. De website en de webservice zullen allebei in een eigen docker container draaien waarbij de website bevragingen doet bij de webservice.

In het begin zal elke stap beschreven en uitgelegd worden. Aan het einde wordt de hoeveelheid uitleg weinig tot niet en kan er lekker gespeeld worden.

- Open een Developer Command Prompt for VS 2017
- Maak ergens een map aan met de naam code, bijvoorbeeld c:\code
- Zet de huidige directory naar die map
- Voer het volgende commando uit om de source code van het project op te halen : 
    ```
    git clone https://github.com/double-bee/docker-workshop --branch step1 --single-branch
    ```


- Open de solution docker-workshop.sln

- webservice los starten en testen. Daarna website starten en testen lokaal
- dockerfile webservice maken en testen

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
