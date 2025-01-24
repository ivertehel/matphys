using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Checkbox2_1_Checked(object sender, RoutedEventArgs e)
        {
            Task2_1.Visibility = Visibility.Visible;
            Task2_2.Visibility = Visibility.Hidden;
            Task2_3.Visibility = Visibility.Hidden;
        }

        private void Checkbox2_2_Checked(object sender, RoutedEventArgs e)
        {
            Task2_1.Visibility = Visibility.Hidden;
            Task2_2.Visibility = Visibility.Visible;
            Task2_3.Visibility = Visibility.Hidden;
        }

        private void Checkbox2_3_Checked(object sender, RoutedEventArgs e)
        {
            Task2_1.Visibility = Visibility.Hidden;
            Task2_2.Visibility = Visibility.Hidden;
            Task2_3.Visibility = Visibility.Visible;
        }
    }
}