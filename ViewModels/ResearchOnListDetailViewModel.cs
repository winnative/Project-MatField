using CommunityToolkit.Mvvm.ComponentModel;
using Project_MatField.Models;

namespace Project_MatField.ViewModels
{
    public class ResearchOnListDetailViewModel : ObservableObject
    {
        public Research _research;
        public ResearchOnListDetailViewModel(Research research)
        {
            _research = research;
            OnListLabel = research.Subject;
            ResourceOnTooltip = research.Resources;
            SummaryResearchOnTooltip = research.Text;
        }

        public string OnListLabel { get; set; }
        public string ResourceOnTooltip { get; set; }
        public string SummaryResearchOnTooltip { get; set; }
        public string ResourceHeaderOnTooltip { get { return "منابع"; } }
        public string SummaryResearchHeaderOnTooltip { get { return "بخشی از متن"; } }
    }
}
