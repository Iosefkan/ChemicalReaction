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
    /// Interaction logic for DataTableWindow.xaml
    /// </summary>
    public partial class DataTableWindow : Window
    {
        class Row
        {
            public double time { get; set; }
            public double y1 { get; set; }
            public double y2 { get; set; }
            public double y3 { get; set; }
            public double y4 { get; set; }
        }
        public DataTableWindow(double[] time, double[] y1, double[] y2, double[] y3, double[] y4)
        {
            InitializeComponent();
            Row[] rows = time.Select((time, index) => new Row
            {
                time = Math.Round(time, 4),
                y1 = Math.Round(y1[index], 4),
                y2 = Math.Round(y2[index], 4),
                y3 = Math.Round(y3[index], 4),
                y4 = Math.Round(y4[index], 4)
            }).ToArray();
            DataTable.ItemsSource = rows;
        }
    }
}
