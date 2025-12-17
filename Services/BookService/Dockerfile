# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copier les fichiers de projet
COPY ["BookService.csproj", "."]
RUN dotnet restore "BookService.csproj"

# Copier tout le code source
COPY . .
RUN dotnet build "BookService.csproj" -c Release -o /app/build

# Étape 2 : Publication
FROM build AS publish
RUN dotnet publish "BookService.csproj" -c Release -o /app/publish

# Étape 3 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Exposer le port 8081 pour différencier du service principal
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

ENTRYPOINT ["dotnet", "BookService.dll"]
