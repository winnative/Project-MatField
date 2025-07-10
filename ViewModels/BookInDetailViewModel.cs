using CommunityToolkit.Mvvm.ComponentModel;
using Project_MatField.Models;

namespace Project_MatField.ViewModels
{
    public class BookInDetailViewModel : ObservableObject
    {
        private string _id = null!;
        private string _code = null!;
        private string _name = null!;
        private string _writer = null!;
        private string _translator = null!;
        private string _publisher = null!;
        private int _publishYear;
        private int _vol = 0;
        private int _col = 0;
        private int _row;
        private string _subject = null!;
        private string _parentGroupId = null!;

        public BookInDetailViewModel() { }
        public BookInDetailViewModel(Book book)
        {
            OriginBook = book;
            _id = book.Id;
            _code = book.Code;
            _name = book.Name;
            _writer = book.Writer;
            _translator = book.Translator;
            _publisher = book.Publisher;
            _publishYear = book.PublishYear;
            _vol = book.Volume;
            _col = book.Column;
            _row = book.Row;
            _subject = book.Subject;
            _parentGroupId = book.ParentGroupId;
        }

        public Book OriginBook { get; set; } = null!;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Writer
        {
            get => _writer;
            set => SetProperty(ref _writer, value);
        }

        public string Translator
        {
            get => _translator;
            set => SetProperty(ref _translator, value);
        }

        public string Publisher
        {
            get => _publisher;
            set => SetProperty(ref _publisher, value);
        }

        public int PublishYear
        {
            get => _publishYear;
            set => SetProperty(ref _publishYear, value);
        }

        public int Volume
        {
            get => _vol;
            set => SetProperty(ref _vol, value);
        }

        public int Column
        {
            get => _col;
            set => SetProperty(ref _col, value);
        }

        public int Row
        {
            get => _row;
            set => SetProperty(ref _row, value);
        }

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        public string ParentGroupId
        {
            get => _parentGroupId;
            set => SetProperty(ref _parentGroupId, value);
        }

        public Book ToModel()
        {
            var model = new Book();
            model.Id = Id;
            model.Code = Code;
            model.Name = Name;
            model.Writer = Writer;
            model.Translator = Translator;
            model.Publisher = Publisher;
            model.PublishYear = PublishYear;
            model.Volume = Volume;
            model.Column = Column;
            model.Row = Row;
            model.Subject = Subject;
            model.ParentGroupId = ParentGroupId;

            return model;
        }
    }
}
