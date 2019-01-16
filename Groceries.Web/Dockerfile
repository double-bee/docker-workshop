FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o ../app

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "/app/Groceries.Web.dll"]
