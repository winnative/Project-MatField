using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Project_MatField.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;

namespace Project_MatField.ViewModels
{
    public class ResearchesViewModel : ObservableObject
    {
        private Realm _dbContext = null!;

        private ResearchOnListDetailViewModel? _selectedOnList;
        private string _addingGroupName = null!;

        public ResearchesViewModel() { InitializeViewModel(); }
        public ResearchesViewModel(params ResearchOnListDetailViewModel[] researchGroupsOnList)
        {
            foreach (var researchGroup in researchGroupsOnList)
            {
                ResearchEntitiesOnList.Add(researchGroup);
            }
            InitializeViewModel();
        }

        public ObservableCollection<ResearchOnListDetailViewModel> ResearchEntitiesOnList { get; set; } = [];

        public ResearchOnListDetailViewModel? SelectedOnList
        {
            get => _selectedOnList;
            set
            {
                SetProperty(ref _selectedOnList, value);
                OnResearchCreatingCommand.NotifyCanExecuteChanged();
                OnResearchDeletingCommand.NotifyCanExecuteChanged();
                OnFolderDeletingCommand.NotifyCanExecuteChanged();
            }
        }

        public string AddingGroupName
        {
            get => _addingGroupName;
            set => SetProperty(ref _addingGroupName, value);
        }

        public IRelayCommand OnFolderCreatingCommand { get; set; } = null!;
        public IRelayCommand OnFolderDeletingCommand { get; set; } = null!;
        public IRelayCommand OnResearchCreatingCommand { get; set; } = null!;
        public IRelayCommand OnResearchDeletingCommand { get; set; } = null!;

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

            ResearchEntitiesOnList.Add(newGroupNode);
        }

        public async void OnFolderDeleting()
        {
            if (SelectedOnList!.Mode is ResearchOnListDetailViewModel.ResearchMode.Research)
            {
                var researchToDelete = _dbContext.Find<Research>(SelectedOnList!.Id);
                _dbContext.Remove(researchToDelete!);
                ResearchEntitiesOnList.Remove(SelectedOnList);
                SelectedOnList = null;
            }
            else
            {
                ContentDialog contentDialogAskForFolderDeleting = new();
                contentDialogAskForFolderDeleting.Title = "حذف پوشه؟";
                contentDialogAskForFolderDeleting.Content = "با حذف پوشه تمامی تحقیقات مربوط به آن نیز حذف خواهد شد.";
                contentDialogAskForFolderDeleting.CloseButtonText = "انصراف";
                contentDialogAskForFolderDeleting.PrimaryButtonText = "حذف پوشه";
                contentDialogAskForFolderDeleting.XamlRoot = ((Application.Current as App)!._serviceProvider.GetService<MainWindow>())!.Content.XamlRoot;
                if (await contentDialogAskForFolderDeleting.ShowAsync() == ContentDialogResult.Primary)
                {
                    var researchGroupToDelete = _dbContext
                        .Find<ResearchGroup>(SelectedOnList.Id);

                    var researchesToDelete = _dbContext
                        .All<Research>()
                        .Where(x => x.ParentGroupId == SelectedOnList.Id)
                        .ToList();

                }
            }
        }

        public void OnResearchCreating()
        {

        }

        public void OnResearchDeleting()
        {

        }
    }
}
