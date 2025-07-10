using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Project_MatField.Pages;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.UI.Xaml.Media.Animation;

namespace Project_MatField.ViewModels
{
    public class NavigationItemsViewModel : ObservableObject
    {
        private Frame _frame;
        private NavigationView _view;

        public NavigationItemsViewModel() { }
        public NavigationItemsViewModel(Frame frame, NavigationView navView)
        {
            _frame = frame;
            _view = navView;
            OnTabSelectedCommand = new RelayCommand(OnTabSelected);

        }

        public ICommand OnTabSelectedCommand { get; set; }

        public Frame AppFrame
        {
            get => _frame;
            set => _frame = value;
        }

        public NavigationView View
        {
            get => _view;
            set => _view = value;
        }

        public void OnTabSelected()
        {
            if (_view.SelectedItem == _view.MenuItems[0])
            {
                AppFrame.Navigate(typeof(PageLibrary), null, new SlideNavigationTransitionInfo());
            }
            else if (_view.SelectedItem == _view.MenuItems[1])
            {
                AppFrame.Navigate(typeof(PageResearches), null, new SlideNavigationTransitionInfo());
            }
            else
            {
                AppFrame.Navigate(typeof(PageSettings), null, new SlideNavigationTransitionInfo());
            }
        }
    }
}
