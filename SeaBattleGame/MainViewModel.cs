using System.ComponentModel;

namespace SeaBattleGame
{
    class MainViewModel : INotifyPropertyChanged
    {
        private object content;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                NotifyPropertyChanged("Content");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
