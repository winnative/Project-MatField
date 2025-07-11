using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Project_MatField.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;

namespace Project_MatField.ViewModels
{
    public class ResearchesViewModel : ObservableObject
    {
        private Realm _dbContext;

        private ResearchOnListDetailViewModel? _selectedOnList;
        private string _addingGroupName = null!;

        public ResearchesViewModel() { InitializeViewModel(); }
        public ResearchesViewModel(params ResearchOnListDetailViewModel[] researchGroupsOnList)
        {
            foreach (var researchGroup in researchGroupsOnList)
            {
                ResearchGroupsOnList.Add(researchGroup);
            }
            InitializeViewModel();
        }

        public ObservableCollection<ResearchOnListDetailViewModel> ResearchGroupsOnList { get; set; } = [];

        public ResearchOnListDetailViewModel? SelectedOnList
        {
            get => _selectedOnList;
            set => SetProperty(ref _selectedOnList, value);
        }

        public string AddingGroupName
        {
            get => _addingGroupName;
            set => SetProperty(ref _addingGroupName, value);
        }

        public IRelayCommand OnFolderCreatingCommand { get; set; }
        public IRelayCommand OnFolderDeletingCommand { get; set; }
        public IRelayCommand OnResearchCreatingCommand { get; set; }
        public IRelayCommand OnResearchDeletingCommand { get; set; }

        private void InitializeViewModel()
        {
            GetDbContextInstance();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            OnFolderCreatingCommand = new RelayCommand(OnFolderCreating);
            OnFolderDeletingCommand = new RelayCommand(OnFolderDeleting);
            OnResearchCreatingCommand = new RelayCommand(OnResearchCreating);
            OnResearchDeletingCommand = new RelayCommand(OnResearchDeleting);
        }

        private void GetDbContextInstance() => 
            _dbContext = (Application.Current as App)?._serviceProvider
            .GetService<Realm>()!;

        public void OnFolderCreating()
        {
            var newResearchGroup = new ResearchGroup(AddingGroupName, []);
            var newGroupNode = new ResearchOnListDetailViewModel(newResearchGroup);
            
            ResearchGroupsOnList.Add(newGroupNode);
        }

        public void OnFolderDeleting()
        {

        }

        public void OnResearchCreating()
        {

        }

        public void OnResearchDeleting()
        {

        }
    }
}
