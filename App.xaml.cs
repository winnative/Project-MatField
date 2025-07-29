using System;
using System.IO;
using Microsoft.UI.Xaml;
using Realms;
using System.Configuration;
using Project_MatField.Kernel;
using Microsoft.Extensions.DependencyInjection;

namespace Project_MatField
{
    public partial class App : Application
    {
        public IServiceProvider _serviceProvider;
        public App()
        {
            InitializeComponent();
            CreateDbPathIfNotExist();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            InitializeMainWindow();
        }

        private void InitializeMainWindow()
        {
            var window = _serviceProvider.GetService<MainWindow>()!;
            window.ExtendsContentIntoTitleBar = true;
            window.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
            window.ResizeAndMove(1000, 700);
            window.ApplyMinSize();
            window.Activate();
        }

        private void CreateDbPathIfNotExist()
        {
            if (Directory.Exists(ConfigurationManager.ConnectionStrings["dbDirD"].ConnectionString) is false)
            {
                Directory.CreateDirectory(ConfigurationManager.ConnectionStrings["dbDirD"].ConnectionString);
            }
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            var dbFile = new RealmConfiguration(ConfigurationManager.ConnectionStrings["dbPathD"].ConnectionString);
            dbFile.SchemaVersion = (ulong)Convert.ToInt16(ConfigurationManager.AppSettings["realmVersion"]!.ToString());
            serviceCollection.AddSingleton(Realm.GetInstance(dbFile));
            serviceCollection.AddSingleton<MainWindow>();
        }
    }
}
