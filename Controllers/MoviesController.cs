using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using swift_movies.Models;

namespace swift_movies.Controllers;

public class MoviesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IHttpClientFactory httpClientFactory, ILogger<MoviesController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var movies = new List<Movie>();

        try
        {
            movies = await _httpClientFactory
                .CreateClient()
                .GetFromJsonAsync<List<Movie>>("http://localhost:5000/api/movie")
                ?? new List<Movie>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to load movies from external API.");
        }

        return View(movies);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClientFactory
                .CreateClient()
                .DeleteAsync($"http://localhost:5000/api/movie/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to delete movie {MovieId}. StatusCode: {StatusCode}", id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to delete movie {MovieId}.", id);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Add()
    {
        return View();
    }
}
