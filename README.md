# APIGrandRepertoire
## Connection à la base de donnée
J'injecte en tant que dépendance un object SqlConnection depuis program.cs.
Cet object récupère la connection string depuis appsettings.json.
<br>
Toutes les requêtes à la base de de données se font depuis les repositories.

## Les services
Ils gèrent la logique. La seule logique dans cette API (dû au fait que ce ne sont que des GET) est la pagination.

## Installer les dépendances de l'application.
Normalement, si l'on démarre l'application avec un dotnet run ou en cliquant sur le bouton vert de Visual Studio, il devrait
télécharger et installer lui-même les dépendances qui sont dans le .csproj. Sinon un dotnet restore les installe.
