using Microsoft.Extensions.Configuration;
using FiszkiApp.Services;
using FiszkiApp.dbConnetcion.APIQueries;
using System;
using System.IO;
using FiszkiApp.Resources.Styles.Colors;

#if ANDROID
using Microsoft.Maui.Platform;
#endif

namespace FiszkiApp
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }
        public static CountriesDic CountriesDic { get; private set; }
        public static ProfileDetails ProfileDetails { get; private set; }
        public static UserCountriesService UserCountriesService { get; private set; }

        private static DatabaseService _databaseService;

        public App()
        {
            InitializeComponent();

            var savedTheme = Preferences.Get("AppTheme", "Pink");
            SetTheme(savedTheme);

            CountriesDic = new CountriesDic();
            ProfileDetails = new ProfileDetails();
            UserCountriesService = new UserCountriesService();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            MainPage = new AppShell();
        }

        public void SetTheme(string theme)
        {
            var oldThemes = Resources.MergedDictionaries.Where(d => d.ContainsKey("ThemeName")).ToList();

            foreach (var dict in oldThemes)
                Resources.MergedDictionaries.Remove(dict);

            ResourceDictionary newDict = theme switch
            {
                "Pink" => new Pink(),
                "Blue" => new Blue(),
                "Green" => new Green(),
                _ => new Pink()
            };

            Resources.MergedDictionaries.Add(newDict);

#if ANDROID
            if (newDict.TryGetValue("StatusBarColor", out var statusBarColorObj) && statusBarColorObj is Color statusBarColor)
            {
                var window = Platform.CurrentActivity?.Window;
                if (window != null)
                {
                    window.SetStatusBarColor(statusBarColor.ToPlatform());
                }
            }
#endif
        }

        public static DatabaseService Database
        {
            get
            {
                if (_databaseService == null)
                {
                    var dbPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "FiszkiApp.db3"
                    );
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