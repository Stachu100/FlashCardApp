using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FiszkiApp.dbConnetcion.APIQueries;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FiszkiApp.EntityClasses.Models;
using Microsoft.Maui.Controls;

namespace FiszkiApp.ViewModel
{
    public partial class LookFlashCardPageViewModel : ObservableValidator
    {
        private readonly FlashCardService _flashCardService;
        private readonly int _categoryId;
        private int _currentPage = 1;
        private const int PageSize = 10;


        public LookFlashCardPageViewModel(int categoryId)
        {
            _categoryId = categoryId;
            _flashCardService = new FlashCardService();
            Flashcards = new ObservableCollection<FlashCard>();
            LoadFlashcardsCommand = new AsyncRelayCommand(LoadFlashcardsAsync);
        }

        [ObservableProperty]
        private ObservableCollection<FlashCard> flashcards;

        public IAsyncRelayCommand LoadFlashcardsCommand { get; }

        private async Task LoadFlashcardsAsync()
        {
            var flashcardsFromApi = await _flashCardService.GetFlashCardsByCategoryPagedAsync(_categoryId, _currentPage);


            if (_currentPage == 1) Flashcards.Clear();

            int lpNumber = (_currentPage - 1) * PageSize + 1;
            foreach (var flashcard in flashcardsFromApi)
            {
                flashcard.Lp = lpNumber++;
                Flashcards.Add(flashcard);
            }

            _currentPage++;
        }
    }
}