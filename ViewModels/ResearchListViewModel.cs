using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MatField.ViewModels
{
    public class ResearchListViewModel : ObservableObject
    {
        public ResearchListViewModel() { }
        public ResearchListViewModel(params ResearchOnListDetailViewModel[] researchesOnList)
        {
            foreach (var research in researchesOnList)
            {
                ResearchesOnList.Add(research);
            }
        }

        public ObservableCollection<ResearchOnListDetailViewModel> ResearchesOnList { get; set; } = [];
    }
}
