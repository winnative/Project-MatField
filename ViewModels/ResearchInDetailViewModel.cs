using CommunityToolkit.Mvvm.ComponentModel;
using Project_MatField.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Project_MatField.ViewModels
{
    public class ResearchInDetailViewModel : ObservableObject
    {
        private string _id = null!;
        private string _subject = null!;
        private string _resources = null!;
        private string _comment = null!;
        private IList<string> _attachedFileLinks = [];
        private string _parentGroupId = null!;

        public ResearchInDetailViewModel() { }
        public ResearchInDetailViewModel(Research research)
        {
            _id = research.Id;
            _subject = research.Subject;
            _resources = research.Resources;
            _comment = research.Comment;
            _attachedFileLinks = research.AttachedFileLinks;
            _parentGroupId = research.ParentGroupId;
        }

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Resources
        {
            get => _resources;
            set => SetProperty(ref _resources, value);
        }

        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        public ObservableCollection<AttachFileOnListViewModel> AttachFiles
        {
            get
            {
                ObservableCollection<AttachFileOnListViewModel> attaches = [];
                foreach (var item in _attachedFileLinks)
                {
                    attaches.Add(new AttachFileOnListViewModel(item));
                }
                return attaches;
            }
            set
            {
                var attaches = value
                    .Select(x => x.FileName)
                    .ToList();

                SetProperty(ref _attachedFileLinks, attaches);
            }
        }

        public string ParentGroupId
        {
            get => _parentGroupId;
            set => SetProperty(ref _parentGroupId, value);
        }
    }
}
