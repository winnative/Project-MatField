using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Realms;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Configuration;
using Project_MatField.Kernel;
using Microsoft.Extensions.DependencyInjection;

namespace Project_MatField
{
    public partial class App : Application
    {
        private Window? _window;
        public Realm _dbContext = null!;
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
            InitializeMainWindow(out _window);
        }

        private void InitializeMainWindow(out Window window)
        {
            window = _serviceProvider.GetService<MainWindow>()!;
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
            serviceCollection.AddSingleton(Realm.GetInstance(dbFile));
            serviceCollection.AddSingleton<MainWindow>();
        }
    }
}
