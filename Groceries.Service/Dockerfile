FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Groceries.Service/Groceries.Service.csproj Groceries.Service/
RUN dotnet restore Groceries.Service/Groceries.Service.csproj
COPY . .
WORKDIR /src/Groceries.Service
RUN dotnet build Groceries.Service.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Groceries.Service.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Groceries.Service.dll"]
