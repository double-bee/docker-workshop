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

Deze solution bestaat uit twee projecten, een webservice waar boodschappen toegevoegd, verwijderd en opgevraagd kunnen worden en een website die een user interface biedt voor deze acties.
- Open het bestand /Controllers/GroceriesController.cs in het project Groceries.Service.

Boven de klasse GroceriesController zie je de tekst [Route("api/[controller]")] staan. Dit betekent dat als een client de url http://websitenaam/api/Groceries aanroept dat deze klasse wordt aangeroepen. Welke methode er aangeroepen wordt hangt af van de rest van de url en wat voor HTTP opdracht er wordt gegeven (GET, PUT, etc.) We gaan nu de GET actie implementeren. Als dat is gebeurd zal een HTTP GET op http://websitenaam/api/Groceries resulteren in een lijst van boodschappen in het JSON formaat.

- Voeg aan de GroceriesController de volgende code toe :
    ```
    private readonly IGroceryRepository _groceryRepository;

    public GroceriesController(IGroceryRepository groceryRepository)
    {
        _groceryRepository = groceryRepository;
    }

    // GET api/values
    [HttpGet]
    public ActionResult<IEnumerable<Grocery>> Get()
    {
        return Ok(_groceryRepository.GetAll());
    }
    ```
In bovenstaande code wordt de GroceryRepository geinjecteerd in de controller. Vervolgens wordt bij een HttpGet request het resultaat van de GetAll terug gestuurd. .NET converteerd de Grocery objecten automatisch naar JSON. De aanroep van OK zorgt voor een positief resultaat naar de client. (HTTP 200)

Het framework van .NET Core maakt de klasse GroceryController aan als dit nodig is. Aangezien we nu een argument aan de constructor hebben toegevoegd moeten we nog wel aangeven hoe .NET Core daaraan komt.

- Open de file Startup.cs en zorg dat de methode ConfigureServices er als volgt uit ziet:
    ```
            public void ConfigureServices(IServiceCollection services)
            {
                services.Configure<GroceryRepositoryOptions>(_config);
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                services.AddTransient<IGroceryRepository, GroceryRepository>();
            }

    ```

- Ga terug naar de command prompt
- ga naar de directory c:\code\docker-workshop\Groceries.Service
- Voer het volgende commando uit:
    ```
    donet publish -o out
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