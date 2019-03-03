using MahApps.Metro.SimpleChildWindow;
using System.Linq;
using System.Windows;

namespace MedLaunch
{
    /// <summary>
    /// Interaction logic for LaunchStringWindow.xaml
    /// </summary>
    public partial class LaunchStringWindow : ChildWindow
    {
        public MainWindow mw { get; set; }

        public LaunchStringWindow()
        {
            InitializeComponent();

            // get the mainwindow
            mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            string ls = mw.LaunchString;

            // populate textbox
            tbLaunchBox.Text = ls;
        }

        private void CANCEL_Click(object sender, RoutedEventArgs e)
        {
            // set the launchstring as empty
            mw.LaunchString = string.Empty;
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // grab the text from the textbox and set this as the launchstring
            mw.LaunchString = tbLaunchBox.Text;
            mw.LaunchRomHandler(mw.LaunchString, true);
            this.Close();
        }
    }
}
