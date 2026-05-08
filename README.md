# Swift Movies

A minimal ASP.NET Core MVC starter website.

## What it does

- Serves a homepage at `/`
- Serves a movies page at `/movies`
- Loads movie data from an external API at `http://localhost:5000/api/movie`
- Uses `HomeController` and `MoviesController`
- Uses the `Movie` model for API response deserialization

## Project structure

- `Program.cs` - configures MVC and HTTP client support
- `Controllers/HomeController.cs` - homepage route
- `Controllers/MoviesController.cs` - fetches movie data and returns the movies view
- `Models/Movie.cs` - movie data model
- `Views/Home/Index.cshtml` - homepage UI
- `Views/Movies/Index.cshtml` - movies list UI

## Run locally

```powershell
cd c:\dev\swift-movies
dotnet run --project swift-movies.csproj
```

Then open `http://localhost:5000` or the URL printed by the app.

## Notes

- The app does not include a database or authentication.
- The `/movies` page depends on the external API being available at `http://localhost:5000/api/movie`.
- Keep the external API running separately for the movies list to load.
