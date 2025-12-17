# LibraryApp - Déploiement Kubernetes et Micro-services

## Architecture cible

### Phase 1 : Déploiement monolithique sur Kubernetes
- LibraryApp (Razor Pages + MVC) déployée comme un seul pod
- Base de données SQLite locale (pour commencer)

### Phase 2 : Introduction des micro-services
- LibraryApp (interface utilisateur)
- BookService (API REST pour la gestion des livres)
- Communication HTTP entre les services

## Fichiers créés

### Dockerisation
- `Dockerfile` : Containerisation de LibraryApp
- `Services/BookService/Dockerfile` : Containerisation du micro-service

### Kubernetes
- `k8s/deployment.yaml` : Déploiement de LibraryApp
- `k8s/service.yaml` : Service externe pour LibraryApp
- `k8s/bookservice-deployment.yaml` : Déploiement du BookService
- `k8s/bookservice-service.yaml` : Service interne pour BookService

### Micro-service
- `Services/BookService/` : API REST complète pour la gestion des livres
- `Services/BookServiceClient.cs` : Client HTTP pour communiquer avec le service
- `Controllers/BooksMicroServiceController.cs` : Contrôleur utilisant le micro-service

## Étapes de déploiement

### 1. Prérequis
```bash
# Installer Docker Desktop ou Docker Engine
# Installer Minikube
minikube start
```

### 2. Build des images Docker
```bash
# Build LibraryApp
docker build -t libraryapp:latest .

# Build BookService
cd Services/BookService
docker build -t bookservice:latest .
cd ../..
```

### 3. Déploiement sur Kubernetes
```bash
# Déployer LibraryApp
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml

# Déployer BookService
kubectl apply -f k8s/bookservice-deployment.yaml
kubectl apply -f k8s/bookservice-service.yaml
```

### 4. Vérification
```bash
# Vérifier les pods
kubectl get pods

# Vérifier les services
kubectl get services

# Accéder à l'application
minikube service libraryapp-service
```

## Communication entre services

### Comment LibraryApp communique avec BookService
1. **BookServiceClient** : Client HTTP configuré dans `Program.cs`
2. **URL du service** : `http://bookservice-service:8081` (nom du service Kubernetes)
3. **Appels API** : GET/POST vers `/api/books`

### Exemple d'appel
```csharp
var books = await _bookServiceClient.GetBooksAsync();
```

## Passage du monolithe aux micro-services

### Avantages
- **Scalabilité** : Chaque service peut être scalé indépendamment
- **Maintenance** : Code plus petit et plus facile à maintenir
- **Technologies** : Chaque service peut utiliser sa propre technologie

### Inconvénients
- **Complexité** : Plus de services à gérer
- **Réseau** : Latence potentielle entre services
- **Débogage** : Plus difficile de tracer les requêtes

### Étapes de migration
1. **Identifier les domaines** : Livres, Membres, Emprunts
2. **Créer les APIs** : Exposer les fonctionnalités comme services
3. **Migrer progressivement** : Commencer par les lectures, puis les écritures
4. **Supprimer l'ancien code** : Une fois la migration validée

## Bonnes pratiques pour débutants

### Erreurs à éviter
1. **Trop de micro-services trop tôt** : Commencer simple
2. **Oublier la gestion des erreurs** : Toujours gérer les timeouts et erreurs réseau
3. **Hardcoder les URLs** : Utiliser la configuration et les noms de services Kubernetes
4. **Ignorer la sécurité** : Sécuriser les communications entre services

### Recommandations
1. **Commencer petit** : Un seul micro-service à la fois
2. **Utiliser des logs** : Tracer les appels entre services
3. **Monitorer** : Surveiller la santé des services
4. **Tests** : Tester les communications réseau

## Prochaines étapes

1. **Ajouter PostgreSQL** : Remplacer SQLite par une base de données partagée
2. **Service Members** : Créer un micro-service pour la gestion des membres
3. **Service Loans** : Créer un micro-service pour les emprunts
4. **API Gateway** : Centraliser l'accès aux micro-services
5. **Monitoring** : Ajouter des outils de monitoring et logging
