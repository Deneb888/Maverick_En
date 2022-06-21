#define ENGLISH_VER

using DevExpress.Xpf.Charts;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Anitoa.Pages
{
    /// <summary>
    /// ucReportThree.xaml 的交互逻辑
    /// </summary>
    public partial class ucReportThree : UserControl
    {
        public event EventHandler ChooseM;
        public ucReportThree()
        {
            InitializeComponent();
            this.Loaded += ucReportThree_Loaded;
        }

        void ucReportThree_Loaded(object sender, RoutedEventArgs e)
        {
            // UpdateTempCurve();
            radChart.Animate();
            radChart1.Animate();

            if (!CommData.deviceFound)  // disabled for now
            {
                rbStop.IsEnabled = false;
                rbStop.Opacity = 0.3;
            }
            else if (CommData.currCycleState > 0)
            {
                rbStop.IsEnabled = true;
                rbStop.Opacity = 1.0;

                UpdateCurState();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
#if ENGLISH_VER
            if (MessageBox.Show("Confirm Force Stop?", "System Message", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;
#else
            if (MessageBox.Show("你确定要停止吗？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;
#endif

            if (ChooseM != null)
            {
                ChooseM("1", null);
            }
        }

        public void UpdateTempCurve()
        {
            dcXYDiagram2D.Series.Clear();
            dcXYDiagram2D.ActualAxisX.ConstantLinesInFront.Clear();
            dcXYDiagram2D1.Series.Clear();
            dcXYDiagram2D1.ActualAxisX.ConstantLinesInFront.Clear();

            LineSeries2D dxcLs = new LineSeries2D();
            dxcLs.Tag = "PI temp";
            dxcLs.DisplayName = "PI temp";
            dxcLs.MarkerVisible = false;

            int count = CommData.temp_history[0].Count;       // Zhimin modified 5-5-2019. Drop the last data point because it is usually bad
            dxcAxisRange.MaxValueInternal = Convert.ToDouble(count) + 1;
            if (count > 10) dxcAreaX.GridSpacing = Convert.ToDouble(count) * 0.1;

            //dxcLs1 = new LineSeries2D();
            //dxcLs1.Tag = chan + ":" + currks;
            //dxcLs1.DisplayName = chan + ":" + currks;
            //dxcLs1.MarkerVisible = false;
            dxcLs.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            dxcLs.Brush = new SolidColorBrush(Color.FromRgb(155, 9, 243));

            for (int i = 0; i < count; i++)
            {
                SeriesPoint sp = new SeriesPoint();
                double id = Convert.ToDouble(i);
                sp.Argument = id.ToString();

                sp.Value = Convert.ToDouble(CommData.temp_history[0].ElementAt(i));
                dxcLs.Points.Add(sp);
            }

            dcXYDiagram2D.Series.Add(dxcLs);

            //========================

            LineSeries2D dxcLs1 = new LineSeries2D();
            dxcLs1.Tag = "PT temp";
            dxcLs1.DisplayName = "PT temp";
            dxcLs1.MarkerVisible = false;

            int count1 = CommData.temp_history[1].Count;       // Zhimin modified 5-5-2019. Drop the last data point because it is usually bad
            dxcAxisRange1.MaxValueInternal = Convert.ToDouble(count1) + 1;
            if (count1 > 10) dxcAreaX1.GridSpacing = Convert.ToDouble(count1) * 0.1;

            //dxcLs1 = new LineSeries2D();
            //dxcLs1.Tag = chan + ":" + currks;
            //dxcLs1.DisplayName = chan + ":" + currks;
            //dxcLs1.MarkerVisible = false;
            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(243, 9, 143));

            for (int i = 0; i < count1; i++)
            {
                SeriesPoint sp = new SeriesPoint();
                double id = Convert.ToDouble(i);
                sp.Argument = id.ToString();

                sp.Value = Convert.ToDouble(CommData.temp_history[1].ElementAt(i));
                dxcLs1.Points.Add(sp);
            }

            dcXYDiagram2D1.Series.Add(dxcLs1);

            //========================

            radChart.Animate();
            radChart1.Animate();

            UpdateCurState();
        }

        public void UpdateCurState()
        {
#if ENGLISH_VER
            switch (CommData.currCycleState)
            {
                case 0:
                    txtCurState.Text = "Ready";
                    break;
                case 1:
                    txtCurState.Text = "Heating up";
                    break;
                case 2:
                    txtCurState.Text = "In Progress";
                    break;
                case 3:
                    txtCurState.Text = "Holding";
                    break;
                case 4:
                    txtCurState.Text = "Melting Start";
                    break;
                case 5:
                    txtCurState.Text = "Melting";
                    break;
            }
#else
                switch (CommData.currCycleState)
                {
                    case 0:
                        txtCurState.Text = "准备就绪";
                        break;
                    case 1:
                        txtCurState.Text = "开始加热";
                        break;
                    case 2:
                        txtCurState.Text = "运行中";
                        break;
                    case 3:
                        txtCurState.Text = "保持";
                        break;
                    case 4:
                        txtCurState.Text = "熔解预热";
                        break;
                    case 5:
                        txtCurState.Text = "熔解";
                        break;
                }
#endif
        }
    }
}
