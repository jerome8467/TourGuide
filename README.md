# TourGuide

API ASP.NET Core (.NET 7) de l'application TourGuide : elle localise les utilisateurs,
leur recommande les attractions proches, calcule leurs points de récompense
et leur propose des offres de voyage.

Projet n°8 du parcours Développeur back-end .NET d'OpenClassrooms :
reprise d'un code existant pour corriger les tests, corriger la recommandation
d'attractions et améliorer les performances.

## Travail réalisé

- Correction des tests unitaires qui échouaient
- Recommandation des 5 attractions les plus proches de l'utilisateur, renvoyées
  sous forme de DTO (nom, coordonnées, distance, points de récompense)
- Optimisation des performances pour 100 000 utilisateurs (traitements
  asynchrones et parallèles, sécurisation des accès concurrents)
- Mise en place d'une intégration continue avec GitHub Actions

## Prérequis

- SDK .NET 7

Aucune base de données n'est nécessaire : les utilisateurs sont générés en mémoire.

## Structure de la solution

| Projet | Rôle |
|---|---|
| `Api` | API web TourGuide (contrôleur, services, DTO) |
| `GpsUtil` | Service externe simulé : localisation et liste des attractions |
| `RewardCentral` | Service externe simulé : points de récompense |
| `TripPricer` | Service externe simulé : offres de voyage |
| `TourGuideTest` | Tests unitaires et tests de performance (xUnit) |

## Lancer l'API

    dotnet run --project Api/TourGuide.csproj

Swagger est disponible sur `http://localhost:5200/swagger`.

| Route | Description |
|---|---|
| `GET /TourGuide/getLocation?userName=...` | Dernière position de l'utilisateur |
| `GET /TourGuide/getNearbyAttractions?userName=...` | Les 5 attractions les plus proches |
| `GET /TourGuide/getRewards?userName=...` | Récompenses de l'utilisateur |
| `GET /TourGuide/getTripDeals?userName=...` | Offres de voyage |

## Lancer les tests

    dotnet test TourGuide.sln

Résultats des tests de performance pour 100 000 utilisateurs :

| Test | Objectif | Temps obtenu |
|---|---|---|
| `HighVolumeTrackLocation` | moins de 15 min | environ 19 s |
| `HighVolumeGetRewards` | moins de 20 min | environ 53 s |

## Intégration continue

Le pipeline GitHub Actions (`.github/workflows/ci.yml`) se lance à chaque push
et à chaque pull request sur la branche `DevelopFixed`. Il :

1. compile la solution ;
2. exécute tous les tests ;
3. publie les DLL de l'API et les met à disposition en archive zip
   (artefact `TourGuide-dll`, dans l'onglet Actions).

