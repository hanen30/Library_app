using System.Net.Http.Json;
using System.Text.Json;
using LibraryApp.Models;

namespace LibraryApp.Services
{
    public class BookServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public BookServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            // Configuration de l'URL du service
            var bookServiceUrl = _configuration.GetValue<string>("BookServiceUrl") ?? "http://bookservice-service:8081";
            _httpClient.BaseAddress = new Uri(bookServiceUrl);
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Book>>("/api/books");
                return response ?? new List<Book>();
            }
            catch (Exception ex)
            {
                // En cas d'erreur, retourner une liste vide
                Console.WriteLine($"Erreur lors de la récupération des livres: {ex.Message}");
                return new List<Book>();
            }
        }

        public async Task<Book?> GetBookAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Book>($"/api/books/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du livre {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<Book?> CreateBookAsync(Book book)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/books", book);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Book>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création du livre: {ex.Message}");
                return null;
            }
        }
    }
}
