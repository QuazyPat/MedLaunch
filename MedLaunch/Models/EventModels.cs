using System.ComponentModel;

namespace MedLaunch.Models
{
    public class GlobalSettingsEventModel : INotifyPropertyChanged
    {
        private bool _netplayEnable;
        public bool NetplayEnable
        {
            get { return _netplayEnable; }
            set
            {
                if (value == _netplayEnable)
                    return;

                _netplayEnable = value;
                OnPropertyChanged("Title");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
