using CommunityToolkit.Mvvm.ComponentModel;
using Project_MatField.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MatField.ViewModels
{
    public class ResearchesViewModel : ObservableObject
    {
        public ResearchesViewModel() { }
        public ResearchesViewModel(params ResearchGroupOnListDetailViewModel[] researchGroupsOnList)
        {
            foreach (var researchGroup in researchGroupsOnList)
            {
                ResearchGroupsOnList.Add(researchGroup);
            }
        }

        public ObservableCollection<ResearchGroupOnListDetailViewModel> ResearchGroupsOnList { get; set; } = [];
    }
}
