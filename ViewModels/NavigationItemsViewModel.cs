using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Project_MatField.Pages;
using System.Collections.Generic;
using System.Windows.Input;

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
                AppFrame.Navigate(typeof(PageLibrary));
            }
            else if (_view.SelectedItem == _view.MenuItems[1])
            {
                AppFrame.Navigate(typeof(PageResearches));
            }
            else
            {
                AppFrame.Navigate(typeof(PageSettings));
            }
        }
    }
}
