using CommunityToolkit.Mvvm.ComponentModel;
using Project_MatField.Models;

namespace Project_MatField.ViewModels
{
    public class BookGroupOnListDetailViewModel : ObservableObject
    {
        private string _count;
        private string _onListLabel = null!;
        private string _name = null!;
        public BookGroupOnListDetailViewModel(BookGroup bookGroup)
        {
            _name = bookGroup.Name;
            _count = $"({bookGroup.BookIds.Count})";
            _onListLabel = bookGroup.Name;
            GroupId = bookGroup.Id;
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string GroupId { get; set; }
        public string OnListLabel
        {
            get => _onListLabel;
            set => SetProperty(ref _onListLabel, value);
        }
        public string Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }
    }
}
