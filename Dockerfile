# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copier les fichiers de projet
COPY ["LibraryApp.csproj", "."]
RUN dotnet restore "LibraryApp.csproj"

# Copier tout le code source
COPY . .
RUN dotnet build "LibraryApp.csproj" -c Release -o /app/build

# Étape 2 : Publication
FROM build AS publish
RUN dotnet publish "LibraryApp.csproj" -c Release -o /app/publish

# Étape 3 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Important : Exposer le port 8080 au lieu de 80 pour Kubernetes
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LibraryApp.dll"]
