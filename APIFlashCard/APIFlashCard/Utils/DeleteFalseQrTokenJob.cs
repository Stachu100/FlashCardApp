using APIFlashCard.Data;
using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using System;

namespace APIFlashCard.Utils
{
    public class DeleteFalseQrTokenJob : IInvocable
    {
        private readonly FlashCardDbContext _db;

        public DeleteFalseQrTokenJob(FlashCardDbContext db)
        {
            _db = db;
        }
        public async Task Invoke()
        {

            var sqlScript = "DELETE from QrLoginToken where IsUsed = 0;";    
            await _db.Database.ExecuteSqlRawAsync(sqlScript);
        }
    }
}
