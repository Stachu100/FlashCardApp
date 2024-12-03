﻿using System.Text;
using Newtonsoft.Json;
using FiszkiApp.EntityClasses.Models;
using FiszkiApp.Services;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class FlashCardService
    {
        private readonly HttpClient _httpClient;

        public FlashCardService()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<List<FlashCard>> GetFlashCardsByCategoryAsync(int categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"FlashCard/{categoryId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var flashcards = JsonConvert.DeserializeObject<List<FlashCard>>(responseContent);
                    return flashcards ?? new List<FlashCard>();
                }
                else
                {
                    Console.WriteLine($"Błąd API: {response.StatusCode} - {responseContent}");
                    return new List<FlashCard>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return new List<FlashCard>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return new List<FlashCard>();
            }
        }

        public async Task<List<FlashCard>> GetFlashCardsByCategoryPagedAsync(int categoryId, int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync($"FlashCard/display/{categoryId}/{page}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var flashcards = JsonConvert.DeserializeObject<List<FlashCard>>(responseContent);
                    return flashcards ?? new List<FlashCard>();
                }
                else
                {
                    Console.WriteLine($"Błąd API: {response.StatusCode} - {responseContent}");
                    return new List<FlashCard>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return new List<FlashCard>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return new List<FlashCard>();
            }
        }
    }
}