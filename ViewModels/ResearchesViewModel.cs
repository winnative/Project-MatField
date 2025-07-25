using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Project_MatField.Models;
using Realms;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Project_MatField.ViewModels;

public class ResearchesViewModel : ObservableObject
{
    private Realm _dbContext = null!;

    private ResearchOnListDetailViewModel? _selectedOnList;
    private ResearchInDetailViewModel? _selectedResearchInDetail;
    private string _addingGroupName = null!;

    public ResearchesViewModel() => InitializeViewModel();
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
            OnEntityDeletingCommand.NotifyCanExecuteChanged();
            InitializeSelectedResearchInDetailBasedOnId();
            OnSaveChangingCommand.NotifyCanExecuteChanged();
        }
    }

    public string AddingGroupName
    {
        get => _addingGroupName;
        set => SetProperty(ref _addingGroupName, value);
    }

    public ResearchInDetailViewModel? SelectedResearchInDetail
    {
        get => _selectedResearchInDetail;
        set => SetProperty(ref _selectedResearchInDetail, value);
    }     

    public IRelayCommand OnFolderCreatingCommand { get; set; } = null!;
    public IRelayCommand OnEntityDeletingCommand { get; set; } = null!;
    public IRelayCommand OnResearchCreatingCommand { get; set; } = null!;
    public IRelayCommand OnSaveChangingCommand { get; set; } = null!;

    private void InitializeViewModel()
    {
        GetDbContextInstance();
        InitializeCommands();
        InitializeTreeView();
    }

    private void InitializeCommands()
    {
        OnFolderCreatingCommand = new RelayCommand(OnFolderCreating);
        OnEntityDeletingCommand = new RelayCommand(OnEntityDeleting, CanExecuteOnEntityDeleting);
        OnResearchCreatingCommand = new RelayCommand(OnResearchCreating, CanExecuteOnResearchCreating);
        OnSaveChangingCommand = new RelayCommand(OnSaveChanging, CanExecuteOnSaveChanging);
    }

    private void InitializeTreeView()
    {
        var researchGroups = _dbContext
            .All<ResearchGroup>()
            .ToList();

        var researches = _dbContext
            .All<Research>()
            .ToList();

        researchGroups.ForEach(rg =>
        {
            var researchGroupOnList = new ResearchOnListDetailViewModel(rg) { Mode = ResearchOnListDetailViewModel.ResearchMode.ResearchGroup };
            ResearchEntitiesOnList.Add(researchGroupOnList);
            researches.ForEach(r =>
            {
                if (r.ParentGroupId == rg.Id)
                {
                    researchGroupOnList.Children.Add(new(r) { Mode = ResearchOnListDetailViewModel.ResearchMode.Research });
                }
            });
        });
    }

    private void GetDbContextInstance() => 
        _dbContext = (Application.Current as App)?._serviceProvider
        .GetService<Realm>()!;

    public void OnResearchSelecting() => 
        InitializeSelectedResearchInDetailBasedOnId();

    public void OnSaveChanging()
    {
        SelectedOnList!.DisplayName = SelectedResearchInDetail!.Subject;
        _dbContext.Write(() =>
        {
            _dbContext
            .Add(SelectedResearchInDetail.ToModel(), true);
        });
    }

    public void OnFolderCreating()
    {
        var newResearchGroup = new ResearchGroup(AddingGroupName, []);
        var newGroupNode = new ResearchOnListDetailViewModel(newResearchGroup);
        newGroupNode.Children = new();

        _dbContext.Write(() =>
        {
            _dbContext
            .Add(newResearchGroup, true);
        });
        ResearchEntitiesOnList.Add(newGroupNode);
    }

    public void OnResearchCreating()
    {
        SelectedResearchInDetail = new();
        SelectedResearchInDetail.ParentGroupId = SelectedOnList!.Id;
        OnSaveChanging();
        SelectedOnList.Children.Add(new(SelectedResearchInDetail.ToModel()) { Mode = ResearchOnListDetailViewModel.ResearchMode.Research, DisplayName = "خالی", Id = Ulid.NewUlid().ToString() });
    }

    public async void OnEntityDeleting()
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
            contentDialogAskForFolderDeleting.PrimaryButtonText = "حذف";
            contentDialogAskForFolderDeleting.XamlRoot = ((Application.Current as App)!._serviceProvider.GetService<MainWindow>())!.Content.XamlRoot;
            if (await contentDialogAskForFolderDeleting.ShowAsync() == ContentDialogResult.Primary)
            {
                var researchGroupToDelete = _dbContext
                    .Find<ResearchGroup>(SelectedOnList.Id);

                var researchesToDelete = _dbContext
                    .All<Research>()
                    .Where(x => x.ParentGroupId == SelectedOnList.Id)
                    .ToList();

                _dbContext.Remove(researchGroupToDelete!);
                researchesToDelete.ForEach(_dbContext.Remove);
                ResearchEntitiesOnList.Remove(SelectedOnList);
                SelectedOnList = null;
            }
        }
    }

    public bool CanExecuteOnResearchCreating() =>
        SelectedOnListCondition() && SelectedOnList!.Mode is ResearchOnListDetailViewModel.ResearchMode.ResearchGroup;
    public bool CanExecuteOnEntityDeleting() => 
        SelectedOnListCondition();
    public bool CanExecuteOnSaveChanging() => 
        SelectedResearchInDetail is not null;
    public bool SelectedOnListCondition() => 
        SelectedOnList != null;


    public void InitializeSelectedResearchInDetailBasedOnId()
    {
        if (SelectedOnList?.Mode == ResearchOnListDetailViewModel.ResearchMode.Research)
        {
            var selectedResearch = _dbContext
                    .All<Research>()
                    .Where(x => x.Id == SelectedOnList!.Id)
                    .FirstOrDefault();

            SelectedResearchInDetail = new(selectedResearch!);
        }
        else
        {
            if (SelectedResearchInDetail is not null)
            {
                SelectedResearchInDetail.Subject = "";
                SelectedResearchInDetail.Resources = "";
                SelectedResearchInDetail.Comment = "";
                SelectedResearchInDetail.Text = "";
            }
            SelectedResearchInDetail = null;
        }

        if (SelectedOnList is null)
        {
            if (SelectedResearchInDetail is not null)
            {
                SelectedResearchInDetail.Subject = "";
                SelectedResearchInDetail.Resources = "";
                SelectedResearchInDetail.Comment = "";
                SelectedResearchInDetail.Text = "";
            }
            SelectedResearchInDetail = null;
        }
    }
}
