using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCSSTweaks
{
    public class FileItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public FileSize Size { get; set; }
        public string Date { get; set; }
        public string Tag { get { return string.Join(",", Tags); } }
        public List<string> Tags = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        PropertyChangedEventArgs arg = new PropertyChangedEventArgs("Tag");

        public void AddTag(string tag)
        {
            if (!Tags.Contains(tag))
                Tags.Add(tag);
            PropertyChanged?.Invoke(this, arg);
        }
        public void RemoveTag(string tag)
        {
            if (Tags.Contains(tag))
                Tags.Remove(tag);
            PropertyChanged?.Invoke(this, arg);
        }
    }
}
