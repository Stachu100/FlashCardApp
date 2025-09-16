using Microsoft.Extensions.Configuration;
using FiszkiApp.Services;
using FiszkiApp.dbConnetcion.APIQueries;
using System;
using System.IO;

namespace FiszkiApp
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }
        public static CountriesDic CountriesDic { get; private set; }

        public static ProfileDetails ProfileDetails { get; private set; }

        private static DatabaseService _databaseService;

        public App()
        {
            InitializeComponent();

            CountriesDic = new CountriesDic();
            ProfileDetails = new ProfileDetails();

            var builder = new ConfigurationBuilder()
            .SetBasePath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            MainPage = new AppShell();
        }

        public static DatabaseService Database
        {
            get
            {
                if (_databaseService == null)
                {
                    var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiszkiApp.db3");
                    _databaseService = new DatabaseService(dbPath);
                }
                return _databaseService;
            }
        }

        protected override void OnSleep()
        {
            bool rememberMe = Preferences.Default.Get("RememberMe", false);

            if (!rememberMe)
            {
                Preferences.Clear();
            }
        }
    }
}