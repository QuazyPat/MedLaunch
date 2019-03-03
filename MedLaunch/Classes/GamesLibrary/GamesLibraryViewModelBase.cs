using System.ComponentModel;

namespace MedLaunch.Classes.GamesLibrary
{
    public class GamesLibraryViewModelBase : INotifyPropertyChanged
    {
        public GamesLibraryViewModelBase()
        {

        }

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
