using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Project_MatField.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MatField.ViewModels
{
    public class ResearchGroupOnListDetailViewModel : ObservableObject
    {
        private readonly Realm _dbContext;

        private string _name = null!;
        private string _id = null!;
        private IList<string> _researchIds = [];

        public ResearchGroupOnListDetailViewModel(ResearchGroup researchGroup)
        {
            _id = researchGroup.Id;
            _name = researchGroup.Name;
            _researchIds = researchGroup.ResearchIds;
            GroupName = researchGroup.Name;
            if (researchGroup.ResearchIds.Any())
            {
                Count = $"دارای {researchGroup.ResearchIds.Count} تحقیق";
            }
            else
            {
                Count = "پوشه خالی است";
            }
        }

        public string GroupName
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Count { get; }

        public ObservableCollection<ResearchOnListDetailViewModel> RelatedResearches
        {
            get
            {
                ObservableCollection<ResearchOnListDetailViewModel> relatedResearches = [];
                foreach(var researchId in _researchIds)
                {
                    var wantedResearch = GetById(researchId);
                    if (wantedResearch != null)
                    {
                        relatedResearches.Add(new(wantedResearch));
                    }
                }
                return relatedResearches;
            }
            set
            {
                var relatedResearchIds = value
                    .Select(x => x._research.Id)
                    .ToList();
                _researchIds = relatedResearchIds;
            }
        }

        private Research? GetById(string researchId)
        {
            return _dbContext.Find<Research>(researchId);
        }
    }
}
