using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace Project_MatField.ViewModels
{
    public class AttachFileOnListViewModel : ObservableObject
    {
        public enum FileIcon
        {
            Image,
            Text,
            Unknown
        }

        public AttachFileOnListViewModel() { }
        public AttachFileOnListViewModel(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            FileName = fileName;
            Name = fileInfo.Name;
            if (fileInfo.Extension is ".png" or ".jpg" or ".gif")
            {
                FileTypeIcon = FileIcon.Image;
                FileTypeIconGlyph = "\uE91B";
            }
            else if (fileInfo.Extension is ".txt" or ".pdf" or ".docx" or ".doc")
            {
                FileTypeIcon = FileIcon.Text;
                FileTypeIconGlyph = "\uF000";
            }
            else
            {
                FileTypeIcon= FileIcon.Unknown;
                FileTypeIconGlyph = "\uE7C3";
            }
        }

        public string FileName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public FileIcon FileTypeIcon { get; set; } = FileIcon.Unknown;
        public string FileTypeIconGlyph { get; set; } = null!;
    }
}
