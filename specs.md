# Projet : Puissance 4

Vous devez développer une application qui permet de jouer au puissance 4 en ligne.

## Attentes du projet

### Entités :

- Player
    - Liste de joueurs
    - Identification **LOGIN** - **PASSWORD**

- Game
    - **HOST / GUEST**
    - Lister les parties :
        - en attente d'un **guest** - **AWAITING GUEST**
        - en attente d'une **action** - **IN PROGRESS**
        - jouées par le player et finies - **FINISHED**

- Grid :
    - **CELLS / ROWS / COLUMNS**
    - alternance des **TURN**

### Diagrammes récapitulatif :

```mermaid
classDiagram
    class Player {
        +String login
        +String password
        +List<Game> games
    }

    class Game {
        +Player host
        +Player guest
        +Grid grid
        +String status
        +startGame()
        +joinGame(Player guest)
        +playTurn(Player player, int column)
        +checkWinCondition(): boolean
    }

    class Grid {
        +int rows
        +int columns
        +Cell[][] cells
        +dropToken(int column, Token token)
        +isFull(): boolean
    }

    class Cell {
        +int row
        +int column
        +Token token
    }

    class Token {
        +String color
    }

    Player "1" -- "0..*" Game : plays
    Game "1" -- "1" Grid : has
    Grid "1" -- "0..*" Cell : contains
    Cell "1" -- "1" Token : holds
```

```mermaid
sequenceDiagram
    participant Player
    participant Frontend
    participant Backend
    participant Database

    Player->>Frontend: Se connecter
    Frontend->>Backend: Authentification
    Backend->>Database: Vérifier les identifiants
    Database-->>Backend: Résultat de l'authentification
    Backend-->>Frontend: Résultat de l'authentification
    Frontend-->>Player: Afficher la liste des parties

    Player->>Frontend: Créer une partie
    Frontend->>Backend: Demande de création de partie
    Backend->>Database: Enregistrer la nouvelle partie
    Database-->>Backend: Confirmation de l'enregistrement
    Backend-->>Frontend: Confirmation de la création de partie
    Frontend-->>Player: Afficher la partie créée

    Player->>Frontend: Jouer un tour
    Frontend->>Backend: Envoyer le coup joué
    Backend->>Database: Mettre à jour l'état de la grille
    Database-->>Backend: Confirmation de la mise à jour
    Backend-->>Frontend: Mettre à jour l'affichage de la grille
    Frontend-->>Player: Afficher le nouvel état de la grille
```

### Cahier des charges technique :

Application 3-tiers :
- **APPLICATIF** - Logique métier
- **PRESENTATION** - Interface Web
- **ACCES DONNEES** - Database + accès

Technos :
- ASP.NET Core Blazor
- ASP.NET Core pour les API REST (Sérialisation en JSON)
- SQLite

### Livrables et tests

Les livrables devront contenir :
- Le code des différents composants
- Une base de données initialisée avec le schéma complet
- Un fichier README qui explique comment lancer votre application
- Un jeu de tests avec notamment les identifiants des utilisateurs à utiliser pour tester l'application
- Un diagramme récapitulatif des entités métier

## Pratiques à mettre en place

- **MODULARITE** : Respecter les principes **SOLID** (notamment l'injection des dépendances)
- **DOMAIN DRIVEN DEVELOPMENT** : Utilisation UL pour rédiger des scénarii / Rédaction de tests d'acceptation
- **SERIALISATION** : Faire attention à la sécurité - JSON
- **WEB SERVICES** : API RESTful / Contrat d'interface / API niv 2
- **SECURITE API** : Utilisation paire ID-PASSWORD en en-tête
- **ORM** : EF comme ORM / Queries - lecture des données sans modification / Commands - Create/Delete/Update / Repository - CRUD directement sur la DB en utilisant EF / Service - Business Logic utilisant uniquement les Respositories
- **SOAR** : Communication asynchrone
- **TESTS** : Utilisation d'un base "InMemory" / Tests unitaires = Services et Respositories / Tests d'intégrations = Services (CRUD) et +
- **WEB** : Blazor en mode WebAssembly pour avoir une page interactive


## Architecture du projet

- **NTiersP4.Domain** : Domain, permet de stocker les interfaces, services et modèles de l'application
- **NTiersP4.Infrastructure** : Implémente les repositories et gère les accès aux données avec la base de données
- **NTiersP4.API** : L'application principale (back) qui possède les controllers et permet de répondre aux requêtes en utilisant les fonctions définies dans les services
- **NTiersP4.UI** : Front Blazor App (WebAssembly), réalise les requêtes à API et affiche le jeu