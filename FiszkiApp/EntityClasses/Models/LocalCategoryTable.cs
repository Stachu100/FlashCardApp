using SQLite;

namespace FiszkiApp.EntityClasses.Models
{
    public class LocalCategoryTable
    {
        [PrimaryKey, AutoIncrement]
        public int IdCategory { get; set; }

        public string CategoryName { get; set; }

        public string FrontLanguage { get; set; }

        public string BackLanguage { get; set; }

        public string LanguageLevel { get; set; }
        public int IsSent { get; set; }

        public int UserID { get; set; }

        [Ignore]
        public string FrontFlagUrl { get; set; }
        [Ignore]
        public string BackFlagUrl { get; set; }

        [Ignore]
        public int API_ID_Category { get; set; }

        [Ignore]
        public string UserName { get; set; }
    }
}