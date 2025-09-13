using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FiszkiApp.ViewModel
{
    public partial class FlipCardPageViewModel : ObservableValidator
    {
        private readonly int _categoryId;
        private readonly DatabaseService _databaseService;
        private ObservableCollection<LocalFlashcardTable> _flashcards;
        private int _currentFlashcardIndex;

        public FlipCardPageViewModel(int categoryId)
        {
            _categoryId = categoryId;
            _databaseService = App.Database;
            _flashcards = new ObservableCollection<LocalFlashcardTable>();
            _currentFlashcardIndex = 0;

            LoadFlashcardsCommand = new AsyncRelayCommand(LoadFlashcardsAsync);
            NextFlashcardCommand = new AsyncRelayCommand(NextFlashcardAsync);
            PreviousFlashcardCommand = new AsyncRelayCommand(PreviousFlashcardAsync);

            LoadFlashcardsCommand.Execute(null);
        }

        [ObservableProperty]
        private LocalFlashcardTable currentFlashcard;

        [ObservableProperty]
        private string categoryName;

        [ObservableProperty]
        private bool isFrontVisible = true;

        [ObservableProperty]
        private bool isBackVisible = false;

        public IAsyncRelayCommand LoadFlashcardsCommand { get; }
        public IAsyncRelayCommand FlipCardCommand { get; }
        public IAsyncRelayCommand NextFlashcardCommand { get; }
        public IAsyncRelayCommand PreviousFlashcardCommand { get; }

        public bool CanGoNext => _currentFlashcardIndex < _flashcards.Count - 1;
        public bool CanGoPrevious => _currentFlashcardIndex > 0;

        private async Task LoadFlashcardsAsync()
        {
            var flashcards = await _databaseService.GetFlashcardsByCategoryIdAsync(_categoryId);
            _flashcards = new ObservableCollection<LocalFlashcardTable>(flashcards);

            var category = await _databaseService.GetCategoryByIdAsync(_categoryId);
            CategoryName = category.CategoryName;

            if (_flashcards.Any())
            {
                _currentFlashcardIndex = 0;
                CurrentFlashcard = _flashcards.First();
            }

            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(CanGoPrevious));
        }

        public async Task NextFlashcardAsync()
        {
            if (CanGoNext && _flashcards.Count > 0)
            {
                _currentFlashcardIndex++;
                CurrentFlashcard = _flashcards[_currentFlashcardIndex];

                IsFrontVisible = true;
                IsBackVisible = false;

                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoPrevious));
            }

            await Task.CompletedTask;
        }
        public async Task PreviousFlashcardAsync()
        {
            if (CanGoPrevious)
            {
                _currentFlashcardIndex--;
                CurrentFlashcard = _flashcards[_currentFlashcardIndex];

                IsFrontVisible = true;
                IsBackVisible = false;

                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoPrevious));
            }

            await Task.CompletedTask;
        }
    }
}