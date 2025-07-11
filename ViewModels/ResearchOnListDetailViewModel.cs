using CommunityToolkit.Mvvm.ComponentModel;
using Project_MatField.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Project_MatField.ViewModels
{
    public class ResearchOnListDetailViewModel : ObservableObject
    {
        public enum ResearchMode
        {
            Research,
            ResearchGroup
        }

        private string _id = null!;
        private string _displayName = null!;
        private ResearchMode _mode;

        public ResearchOnListDetailViewModel(Research research)
        {
            _id = research.Id;
            _displayName = research.Subject;
            _mode = ResearchMode.Research;
        }

        public ResearchOnListDetailViewModel(ResearchGroup researchGroup)
        {
            _id = researchGroup.Id;
            _displayName = researchGroup.Name;
            _mode = ResearchMode.ResearchGroup;
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public ResearchMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        public ObservableCollection<ResearchOnListDetailViewModel> Children { get; set; } = null!;
    }
}
