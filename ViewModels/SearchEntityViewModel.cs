using CommunityToolkit.Mvvm.ComponentModel;
using Project_MatField.Models;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MatField.ViewModels
{
    public class SearchEntityViewModel : ObservableObject
    {
        public enum EntityKind
        {
            Research,
            ResearchGroup,
            Book,
            BookGroup
        }

        private string _id = null!;
        private string _displayContent = null!;
        private EntityKind _entityKind;

        public SearchEntityViewModel(Book book)
        {
            _entityKind = EntityKind.Book;
            _id = book.Id;
            _displayContent = book.Name;
        }

        public SearchEntityViewModel(Research research)
        {
            _entityKind= EntityKind.Research;
            _id = research.Id;
            _displayContent = research.Subject;
        }

        public SearchEntityViewModel(BookGroup bookGroup)
        {
            _entityKind = EntityKind.BookGroup;
            _id = bookGroup.Id;
            _displayContent = bookGroup.Name;
        }

        public SearchEntityViewModel(ResearchGroup researchGroup)
        {
            _entityKind = EntityKind.ResearchGroup;
            _id = researchGroup.Id;
            _displayContent = researchGroup.Name;
        }

        public string Id
        {
            get => _id;
        }

        public string DisplayContent
        {
            get => _displayContent;
        }

        public EntityKind SearchEntityKind
        {
            get => _entityKind;
        }

        public string DisplayKind
        {
            get
            {
                if (_entityKind is EntityKind.Research)
                {
                    return "تحقیق";
                }
                else if (_entityKind is EntityKind.ResearchGroup)
                {
                    return "پوشه تحقیق";
                }
                else if (_entityKind is EntityKind.Book)
                {
                    return "کتاب";
                }
                else
                {
                    return "پوشه کتاب";
                }
            }
        }
    }
}
