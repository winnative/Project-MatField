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
using System.Text.RegularExpressions;

namespace Project_MatField.ViewModels
{
    public class LibraryViewModel : ObservableObject
    {
        public enum SearchBasedOnMode
        {
            Code,
            Name,
            Writer,
            Translator,
            Publisher,
            PublishYear,
            Volume,
            Column,
            Row,
            Point
        }

        // DbContext
        private Realm _dbContext = null!;

        // Essential Vars
        private BookGroupOnListDetailViewModel? _selectedGroup = null;
        private BookInDetailViewModel? _selectedBookInDetailViewModel = null;
        private string _addingGroupName = null!;
        private string _groupNameAddingErrorText = null!;
        private string _searchText = null!;
        private SearchBasedOnMode _selectedSearchFilterBasedOnMode;
        private string _searchBaseText = null!;


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

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                OnSearchingCommand.NotifyCanExecuteChanged();
            }
        }

        public string SearchBaseText
        {
            get => _searchBaseText;
            set
            {
                SetProperty(ref _searchBaseText, value);
            }
        }

        public SearchBasedOnMode SelectedSearchFilterBasedOnMode
        {
            get => _selectedSearchFilterBasedOnMode;
            set => SetProperty(ref _selectedSearchFilterBasedOnMode, value);
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
        public IRelayCommand OnSearchingCommand { get; set; } = null!;
        public IRelayCommand OnSearchFilterBaseChangingCommand { get; set; } = null!;
        

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
            _dbContext = (Application.Current as App)?._serviceProvider
                .GetService<Realm>()!;
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
            OnSearchingCommand = new RelayCommand(OnSearching);
            OnSearchFilterBaseChangingCommand = new RelayCommand(OnFilterBaseChanged);
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

            foreach (var book in bookInfo!) { BookGroupsOnList.Add(book); }
        }

        public void FilterBooksByGroupId(string groupId)
        {
            BookesInDetail
                .Clear();

            var bookData = _dbContext
                .All<Book>()
                .ToList();

            var bookInfo = bookData!
                .Where(x => x.ParentGroupId == groupId)
                .Select(x => new BookInDetailViewModel(x))
                .ToList();

            foreach (var book in bookInfo!) { BookesInDetail.Add(book); }
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

            foreach (var book in bookInfo!) { BookesInDetail.Add(book); }
        }

        public void FilterBooksBySearch(SearchBasedOnMode searchBase)
        {
            BookesInDetail.Clear();

            var bookData = _dbContext
                .All<Book>()
                .ToList();
            List<BookInDetailViewModel>? bookInfo = [];

            switch (searchBase)
            {
                case SearchBasedOnMode.Code:
                    bookInfo = bookData!
                        .Where(x => x.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
                case SearchBasedOnMode.Name:
                    bookInfo = bookData!
                        .Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
                case SearchBasedOnMode.Writer:
                    bookInfo = bookData!
                        .Where(x => x.Writer.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
                case SearchBasedOnMode.Translator:
                    bookInfo = bookData!
                        .Where(x => x.Translator.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
                case SearchBasedOnMode.Publisher:
                    bookInfo = bookData!
                        .Where(x => x.Publisher.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
                case SearchBasedOnMode.PublishYear:
                    bookInfo = bookData!
                        .Where(x => x.PublishYear.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
                case SearchBasedOnMode.Volume:
                    bookInfo = bookData!
                        .Where(x => x.Volume.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
                case SearchBasedOnMode.Column:
                    bookInfo = bookData!
                        .Where(x => x.Column.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
                case SearchBasedOnMode.Row:
                    bookInfo = bookData!
                        .Where(x => x.Row.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new BookInDetailViewModel(x))
                        .ToList();
                    break;
            }
            foreach (var book in bookInfo!) { BookesInDetail.Add(book); }
        }

        // Command execute methods

        public void OnFilterBaseChanged()
        {
            SelectedSearchFilterBasedOnMode = SearchBaseText switch
            {
                "شماره" => SearchBasedOnMode.Code,
                "نام کتاب" => SearchBasedOnMode.Name,
                "نویسنده" => SearchBasedOnMode.Writer,
                "مترجم" => SearchBasedOnMode.Translator,
                "ناشر" => SearchBasedOnMode.Publisher,
                "سال نشر" => SearchBasedOnMode.PublishYear,
                "جلد" => SearchBasedOnMode.Volume,
                "شماره قفسه" => SearchBasedOnMode.Column,
                "شماره ردیف" => SearchBasedOnMode.Row,
                _ => SearchBasedOnMode.Name
            };
        }

        public void OnSearching()
        {
            if (SearchText == string.Empty)
            {
                if (SelectedGroup is not null)
                {
                    FilterBooksByGroupId(SelectedGroup.GroupId);
                }
                else
                {
                    ShowAllBooks();
                }
            }
            else
            {
                FilterBooksBySearch(SelectedSearchFilterBasedOnMode);
            }
        }

        public void OnGroupSelected()
        {
            if (SelectedGroup != null)
            {
                FilterBooksByGroupId(SelectedGroup!.GroupId);
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
            _dbContext.Write(() =>
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

                _dbContext.Remove(bookGroup!);
                BookGroupsOnList.Remove(SelectedGroup!);
                SelectedGroup = null;
            });
        }

        public void OnAddingBook()
        {
            var newRandomCode = _dbContext
                .All<Book>()
                .Count() + 1;

            while (BookesInDetail.Any(x => x.Code == newRandomCode.ToString()))
            {
                newRandomCode += 1;
            }

            Book bookToAdd = new()
            {
                ParentGroupId = _selectedGroup!.GroupId,
                Code = newRandomCode
                .ToString(),
                Name = "ندارد",
                Writer = "ندارد",
                Translator = "ندارد",
                Publisher = "ندارد",
                Subject = "ندارد"
            };

            _dbContext.Write(() =>
                {
                    _dbContext
                    .Add(bookToAdd);
                });

            BookesInDetail.Add(new(bookToAdd));
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

            BookesInDetail.Remove(SelectedBookInDetailViewModel!);
            SelectedBookInDetailViewModel = null;
        }

        public void OnCellUpdating()
        {
            _dbContext.Write(() =>
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
                    NormalGroupNameDetected?.Invoke(this, null!);

                    return true;
                }

                GroupNameAddingErrorText = "پوشه‌ای با این نام قبلا ایجاد شده!";
                DuplicateGroupNameDetected?.Invoke(this, null!);

                return false;
            }
            GroupNameAddingErrorText = "نام پوشه نمی تواند خالی باشد!";
            DuplicateGroupNameDetected?.Invoke(this, null!);

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
                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.Code)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.Code)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "عنوان کتاب")
            {
                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.Name)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.Name)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if(e.Column.Tag is "نویسنده")
            {

                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.Writer)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.Writer)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "مترجم")
            {
                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.Translator)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.Translator)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "ناشر")
            {
                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.Publisher)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.Publisher)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "سال نشر")
            {
                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.PublishYear)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.PublishYear)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "جلد")
            {
                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.Volume)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.Volume)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "شماره قفسه")
            {
                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.Column)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.Column)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag is "شماره طبقه")
            {
                if (e.Column.SortDirection == null ||
                    e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    tempList = BookesInDetail
                        .OrderBy(x => x.Row)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    tempList = BookesInDetail
                        .OrderByDescending(x => x.Row)
                        .ToList();

                    BookesInDetail.Clear();
                    tempList.ForEach(BookesInDetail.Add);
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
