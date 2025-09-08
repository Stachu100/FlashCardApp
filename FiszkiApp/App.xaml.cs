using Microsoft.Extensions.Configuration;
using FiszkiApp.Services;
using System;
using System.IO;
using FiszkiApp.View;

namespace FiszkiApp
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }
        private static DatabaseService _databaseService;

        public App()
        {
            InitializeComponent();

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