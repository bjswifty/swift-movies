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
        return View(new Movie());
    }

    [HttpPost]
    public async Task<IActionResult> Add(Movie model)
    {
        var yearInput = Request.Form["Year"].ToString().Trim();

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(nameof(model.Title), "Title is required.");
        }

        if (string.IsNullOrWhiteSpace(yearInput))
        {
            ModelState.AddModelError(nameof(model.Year), "Year is required.");
        }
        else if (!int.TryParse(yearInput, out var yearValue) || yearInput.Length != 4)
        {
            ModelState.AddModelError(nameof(model.Year), "Year must be a four-digit number.");
        }
        else
        {
            model.Year = yearValue;
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var payload = new { Title = model.Title.Trim(), Year = model.Year };
            var response = await _httpClientFactory
                .CreateClient()
                .PostAsJsonAsync("http://localhost:5000/api/movie", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var errorText = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, !string.IsNullOrWhiteSpace(errorText)
                ? errorText
                : "Unable to add movie. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to add movie.");
            ModelState.AddModelError(string.Empty, "Unable to add movie. Please try again later.");
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var movie = await _httpClientFactory
                .CreateClient()
                .GetFromJsonAsync<Movie>($"http://localhost:5000/api/movie/{id}");

            if (movie == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(movie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to load movie {MovieId} for edit.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Movie model)
    {
        var yearInput = Request.Form["Year"].ToString().Trim();

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(nameof(model.Title), "Title is required.");
        }

        if (string.IsNullOrWhiteSpace(yearInput))
        {
            ModelState.AddModelError(nameof(model.Year), "Year is required.");
        }
        else if (!int.TryParse(yearInput, out var yearValue) || yearInput.Length != 4)
        {
            ModelState.AddModelError(nameof(model.Year), "Year must be a four-digit number.");
        }
        else
        {
            model.Year = yearValue;
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var payload = new { Title = model.Title.Trim(), Year = model.Year };
            var response = await _httpClientFactory
                .CreateClient()
                .PutAsJsonAsync($"http://localhost:5000/api/movie/{model.Id}", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var errorText = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, !string.IsNullOrWhiteSpace(errorText)
                ? errorText
                : "Unable to update movie. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update movie {MovieId}.", model.Id);
            ModelState.AddModelError(string.Empty, "Unable to save movie changes. Please try again later.");
        }

        return View(model);
    }
}
