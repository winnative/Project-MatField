using CommunityToolkit.Mvvm.ComponentModel;
using Project_MatField.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;

namespace Project_MatField.ViewModels
{
    public class BookOnListDetailViewModel : ObservableObject
    {
        private Book _book;
        public BookOnListDetailViewModel(Book book)
        {
            _book = book;
            OnListLabel = book.Name;
            OnTooltipWriter = $"نویسنده: {book.Writer}";
            OnTooltipVolume = $"جلد: {book.Volume}";
            OnTooltipPublishYear = $"سال نشر: {book.PublishYear} ){DateTime.Now.Year - book.PublishYear} سال پیش(";
        }
        public Book Book
        {
            get => _book;
            set => SetProperty(ref _book, value);
        }

        public string OnListLabel { get; set; }
        public string OnTooltipWriter { get; set; }
        public string OnTooltipVolume { get; set; }
        public string OnTooltipPublishYear { get; set; }
        public string OnTooltipHeader { get { return "خلاصه اطلاعات"; } }
    }
}
