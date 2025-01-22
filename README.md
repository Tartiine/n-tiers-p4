Pour faire fonctionner le projet, il faut avoir EF (Entity Framework) d'installé : `dotnet tool install --global dotnet-ef`

Ensuite, sur la branche j'ai nettoyé les migrations et la base de données.
Pour débuter, il faudra les regénérer à l'aide de ces commandes (à lancer à la suite, à la racine du projet):

- `dotnet restore`
- `dotnet build`
- `dotnet ef migrations add InitialCreate --project NTiersP4.Infrastructure --startup-project NTiersP4.API`
- `dotnet ef database update --project NTiersP4.Infrastructure --startup-project NTiersP4.API`


Pour supprimer la base de données et revenir à 0 :
- `dotnet ef database drop --project NTiersP4.Infrastructure --startup-project NTiersP4.API`
- `dotnet ef migrations remove --project NTiersP4.Infrastructure --startup-project NTiersP4.API`


Pour lancer le projet, il suffit juste de lancer le ./run (bash ou PS)