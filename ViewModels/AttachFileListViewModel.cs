using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MatField.ViewModels
{
    public class AttachFileListViewModel : ObservableObject
    {
        public AttachFileListViewModel() { }
        public AttachFileListViewModel(params AttachFileOnListViewModel[] attachFiles)
        {
            foreach (var attachFile in attachFiles)
            {
                AttachFiles.Add(attachFile);
            }
        }

        public ObservableCollection<AttachFileOnListViewModel> AttachFiles { get; set; } = [];
    }
}
