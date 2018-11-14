# docker-workshop
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