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

namespace Project_MatField.ViewModels;

public class ResearchesViewModel : ObservableObject
{
    public enum FilterMode
    {
        Subject,
        Resources,
        Comment,
        ResearchText
    }

    private Realm _dbContext = null!;

    private ResearchOnListDetailViewModel? _selectedOnList;
    private ResearchInDetailViewModel? _selectedResearchInDetail;
    private string _addingGroupName = null!;
    private string _searchFilterText = null!;
    private string _searchText = null!;

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
            OnResearchUpdatingCommand.NotifyCanExecuteChanged();
        }
    }

    public string AddingGroupName
    {
        get => _addingGroupName;
        set => SetProperty(ref _addingGroupName, value);
    }

    public string SearchFilterText
    {
        get => _searchFilterText;
        set => SetProperty(ref _searchFilterText, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public ResearchInDetailViewModel? SelectedResearchInDetail
    {
        get => _selectedResearchInDetail;
        set => SetProperty(ref _selectedResearchInDetail, value);
    }     

    public IRelayCommand OnFolderCreatingCommand { get; set; } = null!;
    public IRelayCommand OnEntityDeletingCommand { get; set; } = null!;
    public IRelayCommand OnResearchCreatingCommand { get; set; } = null!;
    public IRelayCommand OnResearchUpdatingCommand { get; set; } = null!;
    public IRelayCommand OnSearchTextChangedCommand { get; set; } = null!;

    private void InitializeViewModel()
    {
        GetDbContextInstance();
        InitializeCommands();
        LoadTreeView();
    }

    private void InitializeCommands()
    {
        OnFolderCreatingCommand = new RelayCommand(OnFolderCreating);
        OnEntityDeletingCommand = new RelayCommand(OnEntityDeleting, CanExecuteOnEntityDeleting);
        OnResearchCreatingCommand = new RelayCommand(OnResearchCreating, CanExecuteOnResearchCreating);
        OnResearchUpdatingCommand = new RelayCommand(OnResearchUpdating, CanExecuteOnSaveChanging);
        OnSearchTextChangedCommand = new RelayCommand(OnSearchTextChanged);
    }

    private void LoadTreeView()
    {
        var researchGroups = _dbContext
            .All<ResearchGroup>()
            .ToList();

        var researches = _dbContext
            .All<Research>()
            .ToList();

        InitializeTreeView(researchGroups, researches);
    }

    private void InitializeTreeView(List<ResearchGroup> groups, List<Research> researches)
    {
        ResearchEntitiesOnList.Clear();
        groups.ForEach(rg =>
        {
            var researchGroupOnList = new ResearchOnListDetailViewModel(rg) 
            { Mode = ResearchOnListDetailViewModel.ResearchMode.ResearchGroup };

            ResearchEntitiesOnList.Add(researchGroupOnList);
            researches.ForEach(r =>
            {
                if (r.ParentGroupId == rg.Id)
                {
                    researchGroupOnList.Children.Add(new(r) 
                    { Mode = ResearchOnListDetailViewModel.ResearchMode.Research });
                }
            });
        });
    }

    public void FilterResearchesBasedOnSearch(string searchText, FilterMode filter)
    {
        var allResearches = _dbContext.All<Research>().ToList();
        var allGroups = _dbContext.All<ResearchGroup>().ToList();

        List<ResearchGroup> groups = [];
        List<Research> researches = [];

        if (filter is FilterMode.Subject)
        {
            researches = allResearches
                .Where(x => x.Subject.Contains(searchText))
                .ToList();
        }
        else if (filter is FilterMode.Resources)
        {
            researches = allResearches
                .Where(x => x.Resources.Contains(searchText))
                .ToList();
        }
        else if (filter is FilterMode.Comment)
        {
            researches = allResearches
                .Where(x => x.Comment.Contains(searchText))
                .ToList();
        }
        else if (filter is FilterMode.ResearchText)
        {
            researches = allResearches
                .Where(x => x.Text.Contains(searchText))
                .ToList();
        }

        researches.ForEach(res =>
        {
            var groupToAdd = allGroups
                .Where(g => res.ParentGroupId == g.Id)
                .First();

            groups.Add(groupToAdd);
        });

        InitializeTreeView(groups, researches);
    }

    private void GetDbContextInstance() => 
        _dbContext = (Application.Current as App)?._serviceProvider
        .GetService<Realm>()!;

    public void OnResearchSelecting() => 
        InitializeSelectedResearchInDetailBasedOnId();

    public void OnSearchTextChanged()
    {
        if (SearchFilterText is not "")
        {
            if (SearchFilterText is "موضوع")
            {
                FilterResearchesBasedOnSearch(SearchText, FilterMode.Subject);
            }
            else if (SearchFilterText is "منابع")
            {
                FilterResearchesBasedOnSearch(SearchText, FilterMode.Resources);
            }
            else if (SearchFilterText is "نظر محقق")
            {
                FilterResearchesBasedOnSearch(SearchText, FilterMode.Comment);
            }
            else if (SearchFilterText is "متن تحقیق")
            {
                FilterResearchesBasedOnSearch(SearchText, FilterMode.ResearchText);
            }
        }
        else
        {
            LoadTreeView();
        }
    }

    public void OnResearchUpdating()
    {
        SelectedOnList!.DisplayName = SelectedResearchInDetail!.Subject;
        OnSaveChanging();
    }

    private void OnSaveChanging()
    {
        _dbContext.Write(() =>
        {
            _dbContext
            .Add(SelectedResearchInDetail!.ToModel(), true);
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
        SelectedResearchInDetail.Subject = "خالی";
        SelectedResearchInDetail.Resources = "خالی";
        SelectedResearchInDetail.Comment = "خالی";
        SelectedResearchInDetail.Text = "";
        SelectedResearchInDetail.Id = Ulid.NewUlid().ToString();
        OnSaveChanging();
        SelectedOnList.Children.Add(new(SelectedResearchInDetail.ToModel()) { Mode = ResearchOnListDetailViewModel.ResearchMode.Research });
    }

    public async void OnEntityDeleting()
    {
        if (SelectedOnList!.Mode is ResearchOnListDetailViewModel.ResearchMode.Research)
        {
            var researchToDelete = _dbContext.Find<Research>(SelectedOnList!.Id);
            _dbContext.Write(() =>
            {
                _dbContext.Remove(researchToDelete!);
            });
            (ResearchEntitiesOnList.Where(x => x.Children.Any(y => y.Id == SelectedOnList!.Id)).First()).Children.Remove(SelectedOnList!);
            SelectedOnList = null;
        }
        else
        {
            ContentDialog contentDialogAskForFolderDeleting = new();
            contentDialogAskForFolderDeleting.Title = "حذف پوشه؟";
            contentDialogAskForFolderDeleting.Content = "با حذف پوشه تمامی تحقیقات مربوط به آن نیز حذف خواهد شد.";
            contentDialogAskForFolderDeleting.CloseButtonText = "انصراف";
            contentDialogAskForFolderDeleting.PrimaryButtonText = "حذف";
            contentDialogAskForFolderDeleting.FlowDirection = FlowDirection.RightToLeft;
            contentDialogAskForFolderDeleting.XamlRoot = ((Application.Current as App)!._serviceProvider.GetService<MainWindow>())!.Content.XamlRoot;
            if (await contentDialogAskForFolderDeleting.ShowAsync() == ContentDialogResult.Primary)
            {
                var researchGroupToDelete = _dbContext
                    .Find<ResearchGroup>(SelectedOnList.Id);

                var researchesToDelete = _dbContext
                    .All<Research>()
                    .Where(x => x.ParentGroupId == SelectedOnList.Id)
                    .ToList();

                _dbContext.Write(() =>
                {
                    _dbContext.Remove(researchGroupToDelete!);
                    researchesToDelete.ForEach(_dbContext.Remove);
                });
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
