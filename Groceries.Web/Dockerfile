FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Groceries.Web/Groceries.Web.csproj Groceries.Web/
COPY Groceries.Web/nuget.config Groceries.Web/
RUN dotnet restore Groceries.Web/Groceries.Web.csproj
COPY . .
WORKDIR /src/Groceries.Web
RUN dotnet build Groceries.Web.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Groceries.Web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Groceries.Web.dll"]
