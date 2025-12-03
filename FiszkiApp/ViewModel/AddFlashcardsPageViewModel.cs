using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FiszkiApp.ViewModel
{
    public partial class AddFlashcardsPageViewModel : ObservableValidator
    {
        private readonly int _categoryId;
        private readonly DatabaseService _databaseService;

        public AddFlashcardsPageViewModel(int categoryId)
        {
            _categoryId = categoryId;
            _databaseService = App.Database;
            Flashcards = new ObservableCollection<LocalFlashcardTable>();

            LoadFlashcardsCommand = new AsyncRelayCommand(LoadFlashcardsAsync);

            AddFlashcardCommand = new AsyncRelayCommand(AddFlashcardAsync);
            SubmitFlashcardsCommand = new AsyncRelayCommand(SubmitFlashcardsAsync);
            DeleteFlashcardCommand = new AsyncRelayCommand<LocalFlashcardTable?>(DeleteFlashcardAsync);
        }

        [ObservableProperty]
        private ObservableCollection<LocalFlashcardTable> flashcards;

        [ObservableProperty]
        private LocalFlashcardTable selectedFlashcard;

        [ObservableProperty]
        private string frontText;

        [ObservableProperty]
        private string backText;

        public IAsyncRelayCommand AddFlashcardCommand { get; }
        public IAsyncRelayCommand SubmitFlashcardsCommand { get; }
        public IAsyncRelayCommand LoadFlashcardsCommand { get; }
        public IAsyncRelayCommand<LocalFlashcardTable> DeleteFlashcardCommand { get; }

        private async Task LoadFlashcardsAsync()
        {
            var flashcardsFromDb = await _databaseService.GetFlashcardsByCategoryIdAsync(_categoryId);

            Flashcards.Clear();
            int lpNumber = 1;
            foreach (var flashcard in flashcardsFromDb)
            {
                flashcard.Lp = lpNumber++;
                Flashcards.Add(flashcard);
            }
        }

        private async Task AddFlashcardAsync()
        {
            if (string.IsNullOrWhiteSpace(FrontText) || string.IsNullOrWhiteSpace(BackText))
            {
                await Shell.Current.DisplayAlert("Błąd", "Wszystkie pola muszą być wypełnione.", "OK");
                return;
            }

            var newFlashcard = new LocalFlashcardTable
            {
                FrontFlashCard = FrontText,
                BackFlashCard = BackText,
                IdCategory = _categoryId
            };

            await _databaseService.AddFlashcardAsync(newFlashcard);

            FrontText = string.Empty;
            BackText = string.Empty;

            await LoadFlashcardsAsync();
        }

        private async Task SubmitFlashcardsAsync()
        {
            if (!string.IsNullOrWhiteSpace(FrontText) || !string.IsNullOrWhiteSpace(BackText))
            {
                var newFlashcard = new LocalFlashcardTable
                {
                    FrontFlashCard = FrontText,
                    BackFlashCard = BackText,
                    IdCategory = _categoryId
                };
                await _databaseService.AddFlashcardAsync(newFlashcard);
            }

            await Shell.Current.GoToAsync("..");
        }

        private async Task DeleteFlashcardAsync(LocalFlashcardTable? flashcard)
        {
            if (flashcard == null)
                return;

            await _databaseService.DeleteFlashcardAsync(flashcard);

            Flashcards.Remove(flashcard);
        }
    }
}