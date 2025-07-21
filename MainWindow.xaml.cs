using Microsoft.UI.Xaml;
using Project_MatField.ViewModels;

namespace Project_MatField
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           navViewContent.DataContext = new NavigationItemsViewModel(frameContent, navViewContent);
        }
        public NavigationItemsViewModel ViewModel => (NavigationItemsViewModel)navViewContent.DataContext;
    }
}
