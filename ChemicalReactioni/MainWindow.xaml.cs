using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Collections.Specialized.BitVector32;

namespace ChemicalReactioni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Reaction reaction = new Reaction();
        public MainWindow()
        {
            InitializeComponent();
        }
        private static System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StopModeling.IsEnabled = false;
            PauseModeling.IsEnabled = false;
            ReactionPlot.Configuration.Pan = false;
            ReactionPlot.Configuration.Zoom = false;
            DataTable.IsEnabled = false;

            ReactionPlot.Plot.XLabel("Время, мин");
            ReactionPlot.Plot.YLabel("Концентрация компонентов, моль/л");
            ReactionPlot.Plot.Title("Зависимость концентрации компонентов от времени");
            ReactionPlot.AllowDrop = false;
            var legend = ReactionPlot.Plot.Legend();
            legend.Location = Alignment.UpperLeft;

            reaction = new Reaction();
            
            reaction.E1 = 158520;
            reaction.E2 = 150160;
            reaction.E3 = 143890;
            reaction.k01 = 9.61 * Math.Pow(10, 13);
            reaction.k02 = 6.78 * Math.Pow(10, 12);
            reaction.k03 = 9.23 * Math.Pow(10, 11);
            reaction.V = 300;
            reaction.Cain = 0.4;
            reaction.Q = 15;
            reaction.T = 307;
            ReactionPlot.Plot.SetAxisLimits(0, reaction.theta + 1, -0.01, reaction.Cain);
            ReactionPlot.Refresh();
            this.DataContext = reaction;
        }

        private EventHandler? tick;
        public void LiveData(ScatterPlot y1, ScatterPlot y2, ScatterPlot y3, ScatterPlot y4, int dots, double lastCb)
        {
            StopModeling.IsEnabled = true;
            PauseModeling.IsEnabled = true;
            TimeRelation.IsEnabled = false;
            Charting.IsEnabled = false;
            DataTable.IsEnabled = false;

            E1.IsEnabled = false;
            E2.IsEnabled = false;
            E3.IsEnabled = false;
            K01.IsEnabled = false;
            K02.IsEnabled = false;
            K03.IsEnabled = false;
            V.IsEnabled = false;

            tick = (object? sender, EventArgs e) => dispatcherTimer_Tick(y1, y2, y3, y4, dots);
            
            dispatcherTimer.Tick += tick;
            double second = 1 / _sliderValue;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(second);
            dispatcherTimer.Start();
            
        }
        public static double CainChange;
        public static double TChange;
        public static double QChange;
        private void UpdateChart(int index)
        {
            reaction.Ca = y1[index];
            reaction.Cb = y2[index];
            reaction.Cc = y3[index];
            reaction.Cd = y4[index];
            if (reaction.theta < time[index])
            {
                MessageBox.Show("Измененные данные не подходят для измененных данных");
                RevokeChanges();
                return;
            }
            (var timeTmp,var y1Tmp,var y2Tmp,var y3Tmp,var y4Tmp) = Get.GetConcentrationsMathNet(reaction, time[index], dotsCount - index);
            if (IsNotValidArrayData(timeTmp) || IsNotValidArrayData(y1Tmp) || IsNotValidArrayData(y2Tmp) || IsNotValidArrayData(y3Tmp) || IsNotValidArrayData(y4Tmp))
            {
                MessageBox.Show("Невозможно смоделировать реактор с введенными данными\tВведите другие данные");
                RevokeChanges();
                return;
            }
            ReactionPlot.Plot.SetAxisLimits(0, reaction.theta + 1, -0.01, reaction.Cain);
            Array.Copy(timeTmp, 0, time, index, timeTmp.Length);
            Array.Copy(y1Tmp, 0, y1, index, y1Tmp.Length);
            Array.Copy(y2Tmp, 0, y2, index, y2Tmp.Length);
            Array.Copy(y3Tmp, 0, y3, index, y3Tmp.Length);
            Array.Copy(y4Tmp, 0, y4, index, y3Tmp.Length);
            ReactionPlot.Render();
            QChange = reaction.Q;
            TChange = reaction.T;
            CainChange = reaction.Cain;

        }
        void RevokeChanges()
        {
            T.Text = TChange.ToString(CultureInfo.InvariantCulture);
            Q.Text = QChange.ToString(CultureInfo.InvariantCulture);
            CAIN.Text = CainChange.ToString(CultureInfo.InvariantCulture);
            reaction.T = TChange;
            reaction.Q = QChange;
            reaction.Cain = CainChange;
        }
        private void dispatcherTimer_Tick(ScatterPlot CaScatter, ScatterPlot CbScatter, ScatterPlot CcScatter, ScatterPlot CdScatter, int dots)
        {
            if (QChange != reaction.Q || TChange != reaction.T || CainChange != reaction.Cain)
            {
                try
                {
                    int dotsUpdate = (_index - 1) * ((dots / (int)reaction.theta) + 3) < dots ? (_index - 1) * ((dots / (int)reaction.theta) + 3) : dots - 1;
                    UpdateChart(dotsUpdate);
                }
                catch
                {
                    MessageBox.Show("Невозможно смоделировать реактор с введенными данными\tВведите другие данные");
                    RevokeChanges();
                    return;
                }
            }
            int dotsPerSec;
            try
            {
                dotsPerSec = _index * ((dots / (int)reaction.theta) + 3) < dots ? _index * ((dots / (int)reaction.theta) + 3) : dots - 1;
            }
            catch
            {
                dotsPerSec = dots - 1;
            }
            CaScatter.MaxRenderIndex = dotsPerSec;
            CbScatter.MaxRenderIndex = dotsPerSec;
            CcScatter.MaxRenderIndex = dotsPerSec;
            CdScatter.MaxRenderIndex = dotsPerSec;
            _index++;
            ReactionPlot.Render();
            if (_index >= reaction.theta || _index * ((dots / (int)reaction.theta) + 3) > dots)
            {
                CaScatter.MaxRenderIndex = dotsCount - 1;
                CbScatter.MaxRenderIndex = dotsCount - 1;
                CcScatter.MaxRenderIndex = dotsCount - 1;
                CdScatter.MaxRenderIndex = dotsCount - 1;
                ReactionPlot.Render();
                dispatcherTimer.Stop();
                StopModeling.IsEnabled = false;
                PauseModeling.IsEnabled = false;
                Charting.IsEnabled = true;
                TimeRelation.IsEnabled = true;
                DataTable.IsEnabled = true;
                E1.IsEnabled = true;
                E2.IsEnabled = true;
                E3.IsEnabled = true;
                K01.IsEnabled = true;
                K02.IsEnabled = true;
                K03.IsEnabled = true;
                V.IsEnabled = true;

                dispatcherTimer.Tick -= tick;
                MessageBox.Show("Моделирование завершено");
                return;
            }
        }
        private static double[] time = new double[0];
        private static double[] y1 = new double[0];
        private static double[] y2 = new double[0];
        private static double[] y3 = new double[0];
        private static double[] y4 = new double[0];
        private static int dotsCount = 10000;
        private static int _index = 0;
        private static double _sliderValue = 10;
        private static bool _isInstant = false;
        private void Charting_Click(object sender, RoutedEventArgs e)
        {
            ReactionPlot.Plot.Clear();
            _index = 0;
            reaction.Ca = 0;
            reaction.Cb = 0;
            reaction.Cc = 0;
            reaction.Cd = 0;
            (time, y1, y2, y3, y4) = Get.GetConcentrationsMathNet(reaction, 0, dotsCount);

            ReactionPlot.Plot.SetAxisLimits(0, reaction.theta + 1, -0.01, reaction.Cain);
            //ReactionPlot.Plot.AxisAuto();
            ScottPlot.LineStylePatterns.Custom = new float[] { 1, 1, 2, 1, 3, 1, 4, 1 };
            
            if (IsNotValidArrayData(time) || IsNotValidArrayData(y1) || IsNotValidArrayData(y2) || IsNotValidArrayData(y3) || IsNotValidArrayData(y4))
            {
                MessageBox.Show("Невозможно смоделировать реактор с введенными данными\tВведите другие данные");
                return;
            }
            var scatterCa = ReactionPlot.Plot.AddScatter(time, y1, markerSize: 0, lineStyle: LineStyle.DashDotDot, lineWidth: 2, label: "Компонент A", color: System.Drawing.Color.Blue); ;
            var scatterCb = ReactionPlot.Plot.AddScatter(time, y2, markerSize: 0, lineStyle: LineStyle.Solid, lineWidth: 2, label: "Компонент B", color: System.Drawing.Color.Red);
            var scatterCc = ReactionPlot.Plot.AddScatter(time, y3, markerSize: 0, lineStyle: LineStyle.Dash, lineWidth: 2, label: "Компонент C", color: System.Drawing.Color.Green);
            var scatterCd = ReactionPlot.Plot.AddScatter(time, y4, markerSize: 0, lineStyle: LineStyle.Custom, lineWidth: 2, label: "Компонент D", color: System.Drawing.Color.Orange);
            if (!_isInstant)
            {
                CainChange = reaction.Cain;
                TChange = reaction.T;
                QChange = reaction.Q;
                scatterCa.MaxRenderIndex = _index;
                scatterCb.MaxRenderIndex = _index;
                scatterCc.MaxRenderIndex = _index;
                scatterCd.MaxRenderIndex = _index;
            }

            ReactionPlot.Refresh();
            
            if (!_isInstant)
                LiveData(scatterCa, scatterCb, scatterCc, scatterCd, time.Length, y2.Last());
            else
            {
                DataTable.IsEnabled = true;
            }
        }
        private bool IsNotValidArrayData(double[] arr)
        {
            return (arr.Contains(double.NaN) || arr.Contains(double.PositiveInfinity) || arr.Contains(double.PositiveInfinity));
        }

        private void StopModeling_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= tick;
            TimeRelation.IsEnabled = true;
            Charting.IsEnabled = true;

            E1.IsEnabled = true;
            E2.IsEnabled = true;
            E3.IsEnabled = true;
            K01.IsEnabled = true;
            K02.IsEnabled = true;
            K03.IsEnabled = true;
            V.IsEnabled = true;

            StopModeling.IsEnabled = false;
            PauseModeling.IsEnabled = false;
            ReactionPlot.Plot.Clear();
            ReactionPlot.Plot.SetAxisLimits(0, reaction.theta + 1, -0.01, reaction.Cain);
            ReactionPlot.Refresh();
        }

        private void PauseModeling_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.IsEnabled = !dispatcherTimer.IsEnabled;
        }
        DataTableWindow? dataTable;
        private void DataTable_Click(object sender, RoutedEventArgs e)
        {
            dataTable?.Close();
            dataTable = new DataTableWindow(time, y1, y2, y3, y4);
            dataTable.Show();
        }

        private void TimeRelation_Click(object sender, RoutedEventArgs e)
        {
            var time = new TimeWindow(_sliderValue, _isInstant);
            if (time.ShowDialog() == true)
            {
                _sliderValue = time.ResponseTime;
                _isInstant = time.ResponseIsInstant;
            }
        }
    }
}
