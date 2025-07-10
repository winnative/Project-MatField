using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Project_MatField.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Project_MatField.ViewModels
{
    public class LibraryViewModel : ObservableObject
    {
        // DbContext
        private Realm _dbContext = null!;

        // Essential Vars
        private BookGroupOnListDetailViewModel? _selectedGroup = null;
        private BookInDetailViewModel? _selectedBookInDetailViewModel = null;
        private string _addingGroupName = null!;
        private string _groupNameAddingErrorText = null!;


        // Validation Error Events
        public event EventHandler? DuplicateGroupNameDetected;
        public event EventHandler? NormalGroupNameDetected;


        // Class Constructors
        public LibraryViewModel()
        {
            InitializeModelView();
        }
        public LibraryViewModel(BookGroupOnListDetailViewModel[] bookGroupsOnList,
            BookInDetailViewModel[] bookesInDetail)
        {

            foreach (var bookGroup in bookGroupsOnList)
            {
                BookGroupsOnList.Add(bookGroup);
            }

            foreach (var book in bookesInDetail)
            {
                BookesInDetail.Add(book);
            }

            InitializeModelView();
        }

        public LibraryViewModel(params BookGroupOnListDetailViewModel[] bookGroupsOnList)
        {
            foreach (var bookGroup in bookGroupsOnList)
            {
                BookGroupsOnList.Add(bookGroup);
            }

            InitializeModelView();
        }


        // Observable Properties
        public ObservableCollection<BookGroupOnListDetailViewModel> BookGroupsOnList { get; set; } = [];
        public ObservableCollection<BookInDetailViewModel> BookesInDetail { get; set; } = [];
        public string GroupNameAddingErrorText
        {
            get => _groupNameAddingErrorText;
            set => SetProperty(ref _groupNameAddingErrorText, value);
        }
        public string AddingGroupName
        {
            get => _addingGroupName;
            set
            {
                SetProperty(ref _addingGroupName, value);
                OnAddingGroupCommand.NotifyCanExecuteChanged();
            }
        }
        public BookInDetailViewModel? SelectedBookInDetailViewModel
        {
            get => _selectedBookInDetailViewModel;
            set
            {
                SetProperty(ref _selectedBookInDetailViewModel, value);
                OnDeletingBookCommand.NotifyCanExecuteChanged();
            }
        }
        public BookGroupOnListDetailViewModel? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                SetProperty(ref _selectedGroup, value);
                OnDeletingGroupCommand.NotifyCanExecuteChanged();
                OnAddingBookCommand.NotifyCanExecuteChanged();
            }
        }


        // Relay Commands
        public IRelayCommand OnSelectedCommand { get; set; } = null!;
        public IRelayCommand OnFilterRemovedCommand { get; set; } = null!;
        public IRelayCommand OnAddingGroupCommand { get; set; } = null!;
        public IRelayCommand OnDeletingGroupCommand { get; set; } = null!;
        public IRelayCommand OnAddingBookCommand { get; set; } = null!;
        public IRelayCommand OnDeletingBookCommand { get; set; } = null!;
        public IRelayCommand OnCellUpdatingCommand { get; set; } = null!;
        

        // Initializers
        private void InitializeModelView()
        {
            InitializeDbContext();
            ShowAllBooks();
            ShowAllGroups();
            InitializeCommands();
        }

        private void InitializeDbContext()
        {
            _dbContext = (Application.Current as App)?._serviceProvider.GetService<Realm>()!;
        }

        private void InitializeCommands()
        {
            OnSelectedCommand = new RelayCommand(OnGroupSelected);
            OnFilterRemovedCommand = new RelayCommand(OnFilterRemoved);
            OnAddingGroupCommand = new RelayCommand(OnAddingGroup, CanExecuteAddingGroup);
            OnDeletingGroupCommand = new RelayCommand(OnDeletingGroup, CanExecuteBasedOnGroupSelecting);
            OnAddingBookCommand = new RelayCommand(OnAddingBook, CanExecuteBasedOnGroupSelecting);
            OnDeletingBookCommand = new RelayCommand(OnDeletingBook, CanExecuteDeletingBook);
            OnCellUpdatingCommand = new RelayCommand(OnCellUpdating);
        }


        // Get or filter data
        private void ShowAllGroups()
        {
            BookGroupsOnList.Clear();
            var bookData = _dbContext
                .All<BookGroup>()
                .ToList();

            var bookInfo = bookData!
                .Select(x => new BookGroupOnListDetailViewModel(x))
                .ToList();

            foreach (var book in bookInfo!)
            {
                BookGroupsOnList.Add(book);
            }
        }
        public void FilterByGroupId(string groupId)
        {
            BookesInDetail.Clear();
            var bookData = _dbContext
                .All<Book>()
                .ToList();

            var bookInfo = bookData!
                .Where(x => x.ParentGroupId == groupId)
                .Select(x => new BookInDetailViewModel(x))
                .ToList();

            foreach (var book in bookInfo!)
            {
                BookesInDetail.Add(book);
            }
        }

        public void ShowAllBooks()
        {
            BookesInDetail.Clear();
            var bookData = _dbContext
                .All<Book>()
                .ToList();

            var bookInfo = bookData!
                .Select(x => new BookInDetailViewModel(x))
                .ToList();

            foreach (var book in bookInfo!)
            {
                BookesInDetail.Add(book);
            }
        }


        // Command execute methods
        public void OnGroupSelected()
        {
            if (SelectedGroup != null)
            {
                FilterByGroupId(SelectedGroup!.GroupId);
            }
        }

        public void OnFilterRemoved()
        {
            if (SelectedGroup != null)
            {
                ShowAllBooks();
            }
        }

        public void OnAddingGroup()
        {
            _dbContext
                .Write(() =>
            {
                BookGroup group = new(AddingGroupName, []);
                _dbContext.Add(group);
                var groupForShow = new BookGroupOnListDetailViewModel(group);
                BookGroupsOnList.Add(groupForShow);
            });
        }

        public void OnDeletingGroup()
        {
            _dbContext.Write(() =>
            {
                var bookGroup = _dbContext
                .Find<BookGroup>(SelectedGroup!.GroupId);

                _dbContext
                .Remove(bookGroup!);

                BookGroupsOnList.Remove(SelectedGroup!);
                SelectedGroup = null;
            });
        }
        public void OnAddingBook()
        {
            var newRandomCode = _dbContext.All<Book>().Count() + 1;
            while (BookesInDetail.Any(x => x.Code == newRandomCode.ToString()))
            {
                newRandomCode += 1;
            }

            Book bookToAdd = new()
            {
                ParentGroupId = _selectedGroup!.GroupId,
                Code = newRandomCode.ToString()
            };

            _dbContext
                .Write(() =>
                {
                    _dbContext
                    .Add(bookToAdd);
                });

            BookesInDetail
                .Add(new(bookToAdd));
        }

        public void OnDeletingBook()
        {
            var bookToRemove = _dbContext
                .Find<Book>(SelectedBookInDetailViewModel!.Id);

            _dbContext
                .Write(() =>
                {
                    _dbContext
                    .Remove(bookToRemove!);
                });

            BookesInDetail
                .Remove(SelectedBookInDetailViewModel!);
            SelectedBookInDetailViewModel = null;
        }

        public void OnCellUpdating()
        {
            _dbContext
                .Write(() =>
                {
                    _dbContext
                    .Add(SelectedBookInDetailViewModel!.ToModel(), true);
                });
        }

        // Command can execute methods
        public bool CanExecuteBasedOnGroupSelecting()
        {
            return SelectedGroup != null;
        }

        public bool CanExecuteAddingGroup()
        {
            if (AddingGroupName is not "")
            {
                if (BookGroupsOnList
                    .Where(x => x.Name == AddingGroupName)
                    .Any() is false)
                {
                    NormalGroupNameDetected?
                        .Invoke(this, null!);

                    return true;
                }

                GroupNameAddingErrorText = "پوشه‌ای با این نام قبلا ایجاد شده!";
                DuplicateGroupNameDetected?
                    .Invoke(this, null!);

                return false;
            }
            GroupNameAddingErrorText = "نام پوشه نمی تواند خالی باشد!";
            DuplicateGroupNameDetected?
                .Invoke(this, null!);

            return false;
        }

        public bool CanExecuteDeletingBook()
        {
            return SelectedBookInDetailViewModel != null;
        }

        // DataGrid Sorting Algorithm
        public void DataGridLibrarySorting(object sender, DataGridColumnEventArgs e)
        {
            var tempList = new List<BookInDetailViewModel>();
            if (e.Column.Tag is "شماره")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.Code).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.Code).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "عنوان کتاب")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.Name).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.Name).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if(e.Column.Tag is "نویسنده")
            {

                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.Writer).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.Writer).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "مترجم")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.Translator).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.Translator).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "ناشر")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.Publisher).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.Publisher).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "سال نشر")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.PublishYear).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.PublishYear).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "جلد")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.Volume).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.Volume).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "شماره قفسه")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.Column).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.Column).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "شماره طبقه")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail.OrderBy(x => x.Row).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail.OrderByDescending(x => x.Row).ToList();
                    BookesInDetail.Clear();
                    tempList.ForEach(x =>
                    {
                        BookesInDetail.Add(x);
                    });
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }


            foreach (var dgColumn in ((DataGrid)sender).Columns)
            {
                if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }
        }
    }
}
