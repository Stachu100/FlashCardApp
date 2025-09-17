using SQLite;
using System.IO;
using System.Threading.Tasks;
using FiszkiApp.EntityClasses.Models;

namespace FiszkiApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LocalCategoryTable>().Wait();
            _database.CreateTableAsync<LocalFlashcardTable>().Wait();
        }

        public Task<int> AddCategoryAsync(LocalCategoryTable category)
        {
            return _database.InsertAsync(category);
        }

        public async Task<int> AddCategoryAndGetIdAsync(LocalCategoryTable category)
        {
            await _database.InsertAsync(category);

            if (category.IdCategory > 0)
            {
                return category.IdCategory;
            }
            else
            {
                throw new Exception("Nie udało się uzyskać ID kategorii.");
            }
        }

        public Task<int> UpdateCategoryAsync(LocalCategoryTable category)
        {
            return _database.UpdateAsync(category);
        }

        public async Task<int> DeleteCategoryAsync(LocalCategoryTable category)
        {
            await DeleteFlashcardsByCategoryId(category.IdCategory);
            return await _database.DeleteAsync(category);
        }

        public Task<List<LocalCategoryTable>> GetCategoriesAsync()
        {
            return _database.Table<LocalCategoryTable>().ToListAsync();
        }

        public Task<List<LocalCategoryTable>> GetCategoriesByUserIdAsync(int userId)
        {
            return _database.Table<LocalCategoryTable>().Where(c => c.UserID == userId).ToListAsync();
        }

        public Task<LocalCategoryTable> GetCategoryByIdAsync(int id)
        {
            return _database.Table<LocalCategoryTable>().Where(i => i.IdCategory == id).FirstOrDefaultAsync();
        }

        public Task<LocalCategoryTable> GetCategoryByApiIdAsync(int apiId)
        {
            return _database.Table<LocalCategoryTable>()
                .Where(c => c.API_ID_Category == apiId)
                .FirstOrDefaultAsync();
        }

        public Task<int> AddFlashcardAsync(LocalFlashcardTable flashcard)
        {
            return _database.InsertAsync(flashcard);
        }

        public Task<List<LocalFlashcardTable>> GetFlashcardsByCategoryIdAsync(int categoryId)
        {
            return _database.Table<LocalFlashcardTable>().Where(f => f.IdCategory == categoryId).ToListAsync();
        }

        public Task<int> DeleteFlashcardsByCategoryId(int categoryId)
        {
            return _database.Table<LocalFlashcardTable>().Where(f => f.IdCategory == categoryId).DeleteAsync();
        }

        public Task<int> DeleteFlashcardAsync(LocalFlashcardTable flashcard)
        {
            return _database.DeleteAsync(flashcard);
        }

        public Task<int> UpdateFlashcardAsync(LocalFlashcardTable flashcard)
        {
            return _database.UpdateAsync(flashcard);
        }

        //public async Task ResetDatabaseAsync()
        //{
        //    await _database.DropTableAsync<LocalFlashcardTable>();
        //    await _database.DropTableAsync<LocalCategoryTable>();

        //    await _database.CreateTableAsync<LocalCategoryTable>();
        //    await _database.CreateTableAsync<LocalFlashcardTable>();
        //}
    }
}