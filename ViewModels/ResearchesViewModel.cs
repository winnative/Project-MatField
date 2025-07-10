using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_MatField.Models;
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
        private ResearchOnListDetailViewModel _selectedOnList;
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
        public ObservableCollection<ResearchInDetailViewModel> ResearchesInDetail { get; set; } = [];
        public ResearchOnListDetailViewModel SelectedOnList
        {
            get => _selectedOnList;
            set => SetProperty(ref _selectedOnList, value);
        }

        public IRelayCommand OnFolderOrResearchCreatingCommand { get; set; }
        public IRelayCommand OnFolderOrResearchDeletingCommand { get; set; }
        public IRelayCommand OnResearchCreatingCommand { get; set; }
        public IRelayCommand OnResearchDeletingCommand { get; set; }

        public void InitializeViewModel()
        {
            InitializeCommands();
        }

        public void InitializeCommands()
        {
            OnFolderOrResearchCreatingCommand = new RelayCommand(OnFolderOrResearchCreating);
            OnFolderOrResearchDeletingCommand = new RelayCommand(OnFolderOrResearchDeleting);
        }

        public void OnFolderOrResearchCreating()
        {
            
        }

        public void OnFolderOrResearchDeleting()
        {

        }
    }
}
