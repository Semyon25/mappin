using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mappin
{
    public class ClassForTable : INotifyPropertyChanged
    {
        private string firstRow;
        public string FirstRow
        {
            get { return firstRow; }
            set { firstRow = value; OnPropertyChanged(); }
        }
        private string secondRow;
        public string SecondRow
        {
            get { return secondRow; }
            set { secondRow = value; OnPropertyChanged(); }
        }
        public ClassForTable(string first, string second)
        {
            FirstRow = first;
            SecondRow = second;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
