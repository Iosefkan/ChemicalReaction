using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChemicalReactioni
{
    /// <summary>
    /// Interaction logic for TimeWindow.xaml
    /// </summary>
    public partial class TimeWindow : Window
    {
        public TimeWindow(double time, bool isInstant)
        {
            InitializeComponent();
            slValue.Value = time;
            checkBox.IsChecked = isInstant;
        }
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            slValue.IsEnabled = true;
            slText.IsEnabled = true;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            slValue.IsEnabled = false;
            slText.IsEnabled = false;
        }

        public bool ResponseIsInstant
        {
            get { return (bool)checkBox.IsChecked!; }
        }
        public double ResponseTime
        {
            get { return slValue.Value; }
        }
        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
