using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FiszkiApp.dbConnetcion.APIQueries;
using FiszkiApp.ViewModel;
using FiszkiApp.View;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;
using Microsoft.Maui.Controls;

namespace FiszkiApp.ViewModel
{
    public partial class LookFlashCardPageViewModel : ObservableObject
    {
        private readonly FlashCardService _flashCardService;
        private readonly int _categoryId;
        private int _currentPage = 1;
        private const int PageSize = 10;
        private bool _isLoading;


        public LookFlashCardPageViewModel(int categoryId)
        {
            _categoryId = categoryId;
            _flashCardService = new FlashCardService();
            Flashcards = new ObservableCollection<FlashCard>();
            LoadFlashcardsCommand = new AsyncRelayCommand(LoadFlashcardsAsync);
            LoadFlashcardsCommand.Execute(null);
        }

        [ObservableProperty]
        private ObservableCollection<FlashCard> flashcards;

        [ObservableProperty]
        private FlashCard selectedFlashcard;

        public IAsyncRelayCommand LoadFlashcardsCommand { get; }

        private async Task LoadFlashcardsAsync()
        {
            if (_isLoading) return;
            _isLoading = true;

            var flashcardsFromApi = await _flashCardService.GetFlashCardsByCategoryPagedAsync(_categoryId, _currentPage);
            if (flashcardsFromApi == null || flashcardsFromApi.Count == 0)
            {
                _isLoading = false;
                return;
            }

            if (_currentPage == 1) Flashcards.Clear();

            int lpNumber = (_currentPage - 1) * PageSize + 1;
            foreach (var flashcard in flashcardsFromApi)
            {
                flashcard.Lp = lpNumber++;
                Flashcards.Add(flashcard);
            }

            _currentPage++;
            _isLoading = false;
        }
    }
}