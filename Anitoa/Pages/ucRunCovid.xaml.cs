#define ENGLISH_VER

#define Lumin_Lite

// #define TwoByFour

#define VARIABLE_CYCLES

using DevExpress.Xpf.Charts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// ucRunCovid.xaml 的交互逻辑
    /// </summary>
    public partial class ucRunCovid : UserControl
    {
        private static int MAX_CHAN = 4;
        private static int MAX_WELL = 16;
        private static int MAX_CYCL = 501;

        public double[,,] m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,] m_bData = new double[MAX_CHAN, MAX_CYCL];
        public double[,,] m_zData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,,] m_zData2 = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,] m_CTValue = new double[MAX_CHAN, MAX_WELL];
        public double[,] m_mean = new double[MAX_CHAN, MAX_WELL];
        public double[,] factorValue = new double[MAX_CHAN, MAX_CYCL];
        public bool[,] m_falsePositive = new bool[MAX_CHAN, MAX_WELL];
        public string[,] m_Confidence = new string[MAX_CHAN, MAX_WELL];

        public bool[,] m_Result = new bool[MAX_CHAN, MAX_WELL];

        public event EventHandler ChooseM;
        // private int index = 0;
        private List<string> ChanList = new List<string>();
        private List<string> KSList = new List<string>();
        private double threshold = 10;
        private bool bTest = false, norm_top = true;

        public ucRunCovid()
        {
            InitializeComponent();
            this.Loaded += ucRunCovid_Loaded;
            this.Unloaded += ucRunCovid_Unloaded;
        }

        void ucRunCovid_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();

            // Zhimin added: default use current running data
            if (!string.IsNullOrEmpty(CommData.F_Path))
            {
                if (File.Exists(CommData.F_Path))
                {
                    ReadFileNew(CommData.F_Path, 0);
                }
                else
                {
                    ParseDicList();
                }

                ReadCCurveShow();
                DrawLineNew();
            }
            else
            {
                if (ParseDicList() > 0)
                {
                    ReadCCurveShow();
                    DrawLineNew();
                }
            }

            /*            else
                        {
                            ReadFileNew(".\\ImgData\\Test.txt", 0);
                            ReadCCurveShow();
                        }
            */
        }

        void ucRunCovid_Unloaded(object sender, RoutedEventArgs e)
        {
            //if (CommData.reportPending)
            //{
            //    CommData.reportPending = false;
            //    ChooseM("1", null);
            //}
        }

        public void InitData()
        {
            if (CommData.experimentModelData != null)
            {
                //txtAA.Text = txtBA.Text = CommData.experimentModelData.chanonedes == null ? "通道1" : CommData.experimentModelData.chanonedes;
                //txtAB.Text = txtBB.Text = CommData.experimentModelData.chantwodes == null ? "通道2" : CommData.experimentModelData.chantwodes;
                //txtAC.Text = txtBC.Text = CommData.experimentModelData.chanthreedes == null ? "通道3" : CommData.experimentModelData.chanthreedes;
                //txtAD.Text = txtBD.Text = CommData.experimentModelData.chanfourdes == null ? "通道4" : CommData.experimentModelData.chanfourdes;
                /*
                DateTime dt1 = Convert.ToDateTime(CommData.experimentModelData.emdatetime);
                DateTime dt2 = Convert.ToDateTime(CommData.experimentModelData.endatetime);
                double TimeCount = dt2.Subtract(dt1).TotalSeconds;
                TimeSpan t = new TimeSpan(0, 0, (int)TimeCount);
                txtHS.Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
                txtWCSJ.Text = CommData.experimentModelData.endatetime.ToString();
                */
            }
            //dcAxisRange.MinValue = 0;
            //dcAxisRange_X.MinValue = 0;
            //dcAxisRange_X.MaxValue = CommData.Cycle;
            //dcAxisRange.MaxValue = 1500;

            // radChart.Animate();

            //if (!string.IsNullOrEmpty(CommData.F_Path))
            //{
            //    ReadFileNew(CommData.F_Path, 0);
            //    ReadCCurveShow();
            //}

            if (CommData.experimentModelData != null)
            {
                gdA1.Content = CommData.experimentModelData.A1des;
                gdA2.Content = CommData.experimentModelData.A2des;
                gdA3.Content = CommData.experimentModelData.A3des;
                gdA4.Content = CommData.experimentModelData.A4des;
                gdA5.Content = CommData.experimentModelData.A5des;
                gdA6.Content = CommData.experimentModelData.A6des;
                gdA7.Content = CommData.experimentModelData.A7des;
                gdA8.Content = CommData.experimentModelData.A8des;
                gdB1.Content = CommData.experimentModelData.B1des;
                gdB2.Content = CommData.experimentModelData.B2des;
                gdB3.Content = CommData.experimentModelData.B3des;
                gdB4.Content = CommData.experimentModelData.B4des;
                gdB5.Content = CommData.experimentModelData.B5des;
                gdB6.Content = CommData.experimentModelData.B6des;
                gdB7.Content = CommData.experimentModelData.B7des;
                gdB8.Content = CommData.experimentModelData.B8des;

                gdA1.ToolTip = CommData.experimentModelData.A1des;
                gdA2.ToolTip = CommData.experimentModelData.A2des;
                gdA3.ToolTip = CommData.experimentModelData.A3des;
                gdA4.ToolTip = CommData.experimentModelData.A4des;
                gdA5.ToolTip = CommData.experimentModelData.A5des;
                gdA6.ToolTip = CommData.experimentModelData.A6des;
                gdA7.ToolTip = CommData.experimentModelData.A7des;
                gdA8.ToolTip = CommData.experimentModelData.A8des;
                gdB1.ToolTip = CommData.experimentModelData.B1des;
                gdB2.ToolTip = CommData.experimentModelData.B2des;
                gdB3.ToolTip = CommData.experimentModelData.B3des;
                gdB4.ToolTip = CommData.experimentModelData.B4des;
                gdB5.ToolTip = CommData.experimentModelData.B5des;
                gdB6.ToolTip = CommData.experimentModelData.B6des;
                gdB7.ToolTip = CommData.experimentModelData.B7des;
                gdB8.ToolTip = CommData.experimentModelData.B8des;

#if ENGLISH_VER

                txtAA.Text = txtBA.Text = (CommData.experimentModelData.chanonedes == null || CommData.experimentModelData.chanonedes == "") ? "Chan 1" : CommData.experimentModelData.chanonedes;
                txtAB.Text = txtBB.Text = (CommData.experimentModelData.chantwodes == null || CommData.experimentModelData.chantwodes == "") ? "Chan 2" : CommData.experimentModelData.chantwodes;
                txtAC.Text = txtBC.Text = (CommData.experimentModelData.chanthreedes == null || CommData.experimentModelData.chanthreedes == "") ? "Chan 3" : CommData.experimentModelData.chanthreedes;
                txtAD.Text = txtBD.Text = (CommData.experimentModelData.chanfourdes == null || CommData.experimentModelData.chanfourdes == "") ? "Chan 4" : CommData.experimentModelData.chanfourdes;

#else

                txtAA.Text = txtBA.Text = (CommData.experimentModelData.chanonedes == null || CommData.experimentModelData.chanonedes == "") ? "通道1" : CommData.experimentModelData.chanonedes;
                txtAB.Text = txtBB.Text = (CommData.experimentModelData.chantwodes == null || CommData.experimentModelData.chantwodes == "") ? "通道2" : CommData.experimentModelData.chantwodes;
                txtAC.Text = txtBC.Text = (CommData.experimentModelData.chanthreedes == null || CommData.experimentModelData.chanthreedes == "") ? "通道3" : CommData.experimentModelData.chanthreedes;
                txtAD.Text = txtBD.Text = (CommData.experimentModelData.chanfourdes == null || CommData.experimentModelData.chanfourdes == "") ? "通道4" : CommData.experimentModelData.chanfourdes;
#endif
            }

            if (CommData.KsIndex == 4)
            {
                gdA5.IsEnabled = false;
                gdA6.IsEnabled = false;
                gdA7.IsEnabled = false;
                gdA8.IsEnabled = false;
                gdB1.IsEnabled = false;
                gdB2.IsEnabled = false;
                gdB3.IsEnabled = false;
                gdB4.IsEnabled = false;
                gdB5.IsEnabled = false;
                gdB6.IsEnabled = false;
                gdB7.IsEnabled = false;
                gdB8.IsEnabled = false;

                gdA5.Opacity = 0.3;
                gdA6.Opacity = 0.3;
                gdA7.Opacity = 0.3;
                gdA8.Opacity = 0.3;

                gdB1.Opacity = 0.3;
                gdB2.Opacity = 0.3;
                gdB3.Opacity = 0.3;
                gdB4.Opacity = 0.3;
                gdB5.Opacity = 0.3;
                gdB6.Opacity = 0.3;
                gdB7.Opacity = 0.3;
                gdB8.Opacity = 0.3;

                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int n = 0; n < 4; n++)                 // Zhimin: 4 wells in the first row.
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + n;
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainA.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                }
            }
            else if (CommData.KsIndex == 8)
            {
#if Lumin_Lite

                gdB1.IsEnabled = false;
                gdB2.IsEnabled = false;
                gdB3.IsEnabled = false;
                gdB4.IsEnabled = false;
                gdB5.IsEnabled = false;
                gdB6.IsEnabled = false;
                gdB7.IsEnabled = false;
                gdB8.IsEnabled = false;

                gdB1.Opacity = 0.3;
                gdB2.Opacity = 0.3;
                gdB3.Opacity = 0.3;
                gdB4.Opacity = 0.3;
                gdB5.Opacity = 0.3;
                gdB6.Opacity = 0.3;
                gdB7.Opacity = 0.3;
                gdB8.Opacity = 0.3;

                BuildGdResultTable();

#elif TwoByFour
                //2019.04.03
                gdA5.IsEnabled = false;
                gdA6.IsEnabled = false;
                gdA7.IsEnabled = false;
                gdA8.IsEnabled = false;
                gdB5.IsEnabled = false;
                gdB6.IsEnabled = false;
                gdB7.IsEnabled = false;
                gdB8.IsEnabled = false;

                gdA5.Opacity = 0.3;
                gdA6.Opacity = 0.3;
                gdA7.Opacity = 0.3;
                gdA8.Opacity = 0.3;

                gdB5.Opacity = 0.3;
                gdB6.Opacity = 0.3;
                gdB7.Opacity = 0.3;
                gdB8.Opacity = 0.3;

                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + n;
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainA.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                    for (int n = 0; n < 4; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + (n + 4);
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainB.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                }
#else
                //2019.04.03

                gdB1.IsEnabled = false;
                gdB2.IsEnabled = false;
                gdB3.IsEnabled = false;
                gdB4.IsEnabled = false;
                gdB5.IsEnabled = false;
                gdB6.IsEnabled = false;
                gdB7.IsEnabled = false;
                gdB8.IsEnabled = false;

                gdB1.Opacity = 0.3;
                gdB2.Opacity = 0.3;
                gdB3.Opacity = 0.3;
                gdB4.Opacity = 0.3;
                gdB5.Opacity = 0.3;
                gdB6.Opacity = 0.3;
                gdB7.Opacity = 0.3;
                gdB8.Opacity = 0.3;

                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int n = 0; n < 8; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + n;
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainA.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                }
#endif
            }
            else if (CommData.KsIndex == 16)
            {
/*                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int n = 0; n < 8; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + n;
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainA.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                    for (int n = 0; n < 8; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + (n + 8);
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainB.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                }
*/

                BuildGdResultTable();

#if Lumin_Lite

                gdA5.IsEnabled = true;
                gdA6.IsEnabled = true;
                gdA7.IsEnabled = true;
                gdA8.IsEnabled = true;
                gdB1.IsEnabled = true;
                gdB2.IsEnabled = true;
                gdB3.IsEnabled = true;
                gdB4.IsEnabled = true;
                gdB5.IsEnabled = true;
                gdB6.IsEnabled = true;
                gdB7.IsEnabled = true;
                gdB8.IsEnabled = true;

                gdA5.Opacity = 1.0;
                gdA6.Opacity = 1.0;
                gdA7.Opacity = 1.0;
                gdA8.Opacity = 1.0;

                gdB1.Opacity = 1.0;
                gdB2.Opacity = 1.0;
                gdB3.Opacity = 1.0;
                gdB4.Opacity = 1.0;
                gdB5.Opacity = 1.0;
                gdB6.Opacity = 1.0;
                gdB7.Opacity = 1.0;
                gdB8.Opacity = 1.0;

                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int n = 0; n < 8; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + n;
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainA.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                    for (int n = 0; n < 8; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + (n + 8);
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainB.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                }

#endif


            }

            //gdA1.IsChecked = true;
            //gdA2.IsChecked = true;
            //gdA3.IsChecked = true;
            //gdA4.IsChecked = true;

            // chan1.IsChecked = true;
            // chan2.IsChecked = true;

            bTest = false;

            cbNorm.IsChecked = norm_top;

            //2019.04.03
            /*            if ((CommData.cboChan3 + CommData.cboChan4) == 0)
                        {
                            chan3.IsEnabled = false;
                            chan4.IsEnabled = false;

                            chan3.Opacity = 0.3;
                            chan4.Opacity = 0.3;
                        }
            */

            /*            if (CommData.cboChan2 > 0)
                        {
                            chan2.IsEnabled = true;
                            chan2.Opacity = 1;
                        }
                        else
                        {
                            chan2.IsEnabled = false;
                            chan2.Opacity = 0.3;
                        }

                        if (CommData.cboChan3 > 0)
                        {
                            chan3.IsEnabled = true;
                            chan3.Opacity = 1;
                        }
                        else
                        {
                            chan3.IsEnabled = false;
                            chan3.Opacity = 0.3;
                        }

                        if (CommData.cboChan4 > 0)
                        {
                            chan4.IsEnabled = true;
                            chan4.Opacity = 1;
                        }
                        else
                        {
                            chan4.IsEnabled = false;
                            chan4.Opacity = 0.3;
                        }
            */

            if (CommData.TdIndex < 3)
            {
                chan3.IsEnabled = false;
                chan3.Opacity = 0.3;
            }
            if (CommData.TdIndex < 4)
            {
                chan4.IsEnabled = false;
                chan4.Opacity = 0.3;
            }

            //            InitCheckStatus();

            threshold = 100 * CommData.experimentModelData.curfitCtTh;
            txtCtThreshold.Text = threshold.ToString("0.0");
        }

        private void BuildGdResultTable()
        {

            const int MAX_ROW = 64;

            for (int n = 0; n < MAX_ROW; n++)
            {
                TextBlock text = new TextBlock();
                text.Tag = "ic" + "," + n;
                text.Visibility = Visibility.Collapsed;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.Text = "0.0";
                gdResult.Children.Add(text);
                Grid.SetRow(text, n + 1);
                Grid.SetColumn(text, 4);
            }


            for (int n = 0; n < MAX_ROW; n++)
            {
                TextBlock text = new TextBlock();
                text.Tag = "n" + "," + n;
                text.Visibility = Visibility.Collapsed;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.Text = "0.0";
                text.TextWrapping = TextWrapping.Wrap;
                text.MaxWidth = 95;
                gdResult.Children.Add(text);
                Grid.SetRow(text, n + 1);
                Grid.SetColumn(text, 1);
            }

            for (int n = 0; n < MAX_ROW; n++)
            {
                TextBlock text = new TextBlock();
                text.Tag = "nc" + "," + n;
                text.Visibility = Visibility.Collapsed;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.Text = "0.0";
                gdResult.Children.Add(text);
                Grid.SetRow(text, n + 1);
                Grid.SetColumn(text, 2);
            }

            for (int n = 0; n < MAX_ROW; n++)
            {
                TextBlock text = new TextBlock();
                text.Tag = "pc" + "," + n;
                text.Visibility = Visibility.Collapsed;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.Text = "0.0";
                gdResult.Children.Add(text);
                Grid.SetRow(text, n + 1);
                Grid.SetColumn(text, 3);
            }

            for (int n = 0; n < MAX_ROW; n++)
            {
                TextBlock text = new TextBlock();
                text.Tag = "r" + "," + n;
                text.Visibility = Visibility.Collapsed;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.Text = "0.0";
                gdResult.Children.Add(text);
                Grid.SetRow(text, n + 1);
                Grid.SetColumn(text, 8);
            }

            for (int n = 0; n < MAX_ROW; n++)
            {
                TextBlock text = new TextBlock();
                text.Tag = "eg" + "," + n;
                text.Visibility = Visibility.Collapsed;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.Text = "0.0";
                gdResult.Children.Add(text);
                Grid.SetRow(text, n + 1);
                Grid.SetColumn(text, 5);
            }

            for (int n = 0; n < MAX_ROW; n++)
            {
                TextBlock text = new TextBlock();
                text.Tag = "ng" + "," + n;
                text.Visibility = Visibility.Collapsed;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.Text = "0.0";
                gdResult.Children.Add(text);
                Grid.SetRow(text, n + 1);
                Grid.SetColumn(text, 6);
            }

            for (int n = 0; n < MAX_ROW; n++)
            {
                TextBlock text = new TextBlock();
                text.Tag = "w" + "," + n;
                text.Visibility = Visibility.Collapsed;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.Text = "0.0";
                gdResult.Children.Add(text);
                Grid.SetRow(text, n + 1);
                Grid.SetColumn(text, 0);
            }
        }

        public void InitCheckStatus()
        {
            if (CommData.experimentModelData.A1des.Count() > 0) gdA1.IsChecked = true;
            else gdA1.IsChecked = false;
            if (CommData.experimentModelData.A2des.Count() > 0) gdA2.IsChecked = true;
            else gdA2.IsChecked = false;
            if (CommData.experimentModelData.A3des.Count() > 0) gdA3.IsChecked = true;
            else gdA3.IsChecked = false;
            if (CommData.experimentModelData.A4des.Count() > 0) gdA4.IsChecked = true;
            else gdA4.IsChecked = false;
            if (CommData.experimentModelData.A5des.Count() > 0) gdA5.IsChecked = true;
            else gdA5.IsChecked = false;
            if (CommData.experimentModelData.A6des.Count() > 0) gdA6.IsChecked = true;
            else gdA6.IsChecked = false;
            if (CommData.experimentModelData.A7des.Count() > 0) gdA7.IsChecked = true;
            else gdA7.IsChecked = false;
            if (CommData.experimentModelData.A8des.Count() > 0) gdA8.IsChecked = true;
            else gdA8.IsChecked = false;

            if (CommData.experimentModelData.B1des.Count() > 0) gdB1.IsChecked = true;
            else gdB1.IsChecked = false;
            if (CommData.experimentModelData.B2des.Count() > 0) gdB2.IsChecked = true;
            else gdB2.IsChecked = false;
            if (CommData.experimentModelData.B3des.Count() > 0) gdB3.IsChecked = true;
            else gdB3.IsChecked = false;
            if (CommData.experimentModelData.B4des.Count() > 0) gdB4.IsChecked = true;
            else gdB4.IsChecked = false;
            if (CommData.experimentModelData.B5des.Count() > 0) gdB5.IsChecked = true;
            else gdB5.IsChecked = false;
            if (CommData.experimentModelData.B6des.Count() > 0) gdB6.IsChecked = true;
            else gdB6.IsChecked = false;
            if (CommData.experimentModelData.B7des.Count() > 0) gdB7.IsChecked = true;
            else gdB7.IsChecked = false;
            if (CommData.experimentModelData.B8des.Count() > 0) gdB8.IsChecked = true;
            else gdB8.IsChecked = false;

            if (CommData.cboChan1 > 0) chan1.IsChecked = true;
            else chan1.IsChecked = false;
            if (CommData.cboChan2 > 0) chan2.IsChecked = true;
            else chan2.IsChecked = false;
            if (CommData.cboChan3 > 0) chan3.IsChecked = true;
            else chan3.IsChecked = false;
            if (CommData.cboChan4 > 0) chan4.IsChecked = true;
            else chan4.IsChecked = false;
        }


        /// <summary>
        /// 读取文件
        /// </summary>
        public void OpenReadFile()
        {
            //            if(CommData.flash_loaded)
            //            {
            //                MessageBox.Show("在设备连接状态不能读外部数据文件");
            //                return;
            //            }

            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
#if ENGLISH_VER
            pOpenFileDialog.Filter = "All files|*.*";//若打开指定类型的文件只需修改Filter，如打开txt文件，改为*.txt即可
            pOpenFileDialog.Title = "Open Data File";
#else
            pOpenFileDialog.Filter = "所有文件|*.*";//若打开指定类型的文件只需修改Filter，如打开txt文件，改为*.txt即可
            pOpenFileDialog.Title = "打开文件";
#endif
            pOpenFileDialog.Multiselect = false;
            if (pOpenFileDialog.ShowDialog() == true)
            {
                string path = pOpenFileDialog.FileName;
                CommData.F_Path = path;
                CommData.experimentModelData.ImgFileName = path;

                ReadFileNew(path, 0);
                ReadCCurveShow();

                txtFilePath.Text = path;
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"> 0: 历史文件数据 1: 正在进行的数据</param>
        public void ReadFileNew(string path, int type)
        {
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);

                if (sr == null)
                    return;

                var line = System.IO.File.ReadAllLines(path);
                string[] ss = line.ToArray();
                sr.Close();

                CommData.diclist = new Dictionary<string, List<string>>();
                string name = "";
                string dpstr = "";
                bool dpheader = false;

                foreach (var item in ss)
                {
                    if (string.IsNullOrEmpty(item)) continue;

                    if (item.Contains("Chipdp"))
                    {
                        dpheader = true;
                    }
                    else if (item.Contains("Chip#"))
                    {
                        name = item;
                        dpheader = false;
                        if (!CommData.diclist.Keys.Contains(name))
                        {
                            CommData.diclist[name] = new List<string>();
                        }
                    }
                    else
                    {
                        if (item.Contains("Chip#"))
                        {
                            Debug.Assert(false);         // should never happen
                            continue;
                        }

                        if (dpheader)
                        {
                            dpstr += item;

                            //if (String.IsNullOrEmpty(dpstr))
                            //    continue;

                            //string[] s2 = dpstr.Split(' ');
                            //int len = s2.Length - 1;

                            //byte[] trim_buff = new byte[len];

                            //for(int i = 0; i < len; i++)
                            //{
                            //    trim_buff[i] = Convert.ToByte(s2[i]);
                            //}

                            //List<int> rlist = new List<int>();      // row index
                            //List<int> clist = new List<int>();      // col index                           

                            //int k = 0;

                            //char version = (char)trim_buff[k]; k++;
                            //int sn1 = trim_buff[k]; k++;
                            //int sn2 = trim_buff[k]; k++;

                            //int num_channels = trim_buff[k]; k++;
                            //int num_wells = trim_buff[k]; k++;
                            //int num_pages = trim_buff[k]; k++;

                            //if (CommData.row_index[0, 0] == null && CommData.col_index[0, 0] == null || true)           // This means dp data not loaded yet from flash
                            //{
                            //    CommData.KsIndex = num_wells;
                            //    // CommData.flash_loaded = true;                                                   // treated the same as flash loaded.
                            //    CommData.TdIndex = num_channels;

                            //    for (int i = 0; i < num_channels; i++)
                            //    {
                            //        for (int j = 0; j < num_wells; j++)
                            //        {
                            //            int n = trim_buff[k]; k++;
                            //            rlist.Clear();
                            //            clist.Clear();
                            //            for (int l = 0; l < n; l++)
                            //            {
                            //                int row = trim_buff[k++]; // k++;
                            //                int col = trim_buff[k]; k++;

                            //                rlist.Add(row);
                            //                clist.Add(col);
                            //            }
                            //            CommData.row_index[i, j] = new List<int>(rlist);
                            //            CommData.col_index[i, j] = new List<int>(clist);
                            //        }
                            //    }
                            //}

                            //CommData.dpinfo_loaded = true;

                            CommData.ParseDpstr(dpstr);

                            Reinit();
                        }
                        else
                        {
                            CommData.diclist[name].Add(item);
                        }
                    }
                }

                CommData.experimentModelData.ampData = CommData.diclist;
                //CommData.experimentModelData.row_index = CommData.row_index;
                //CommData.experimentModelData.col_index = CommData.col_index;

                /*  List<string>[] ci = new List<string>[2];

                  List<string> cii = new List<string> ();
                  cii.Add("1");
                  cii.Add("2");

                  for(int i=0; i<2; i++)
                  {

                          ci[i] = cii;

                  }


                  CommData.experimentModelData.cindex = ci;

                  */

                if (!String.IsNullOrEmpty(CommData.experimentModelData.dpStr) && !String.IsNullOrEmpty(dpstr) && CommData.experimentModelData.dpStr != dpstr)
                {
                    MessageBox.Show("Equipment ID in data file mismatches current equipment ID.", "Maverick Message");
                }

                if (!String.IsNullOrEmpty(dpstr))
                {
                    CommData.experimentModelData.dpStr = dpstr;
                }

                if (type == 0)
                {
                    foreach (var item in CommData.diclist.Keys)
                    {
                        if (CommData.diclist[item].Count == 0) continue;

                        //    CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);

                        //=======================Zhimin: deal with bad cycle number=================

                        int m, n, c;

                        m = Convert.ToInt32(CommData.diclist[item].Count);
                        n = CommData.imgFrame;


                        if (m % n != 0)
                        {
#if DEBUG
                            MessageBox.Show("File corruption detected, missing rows");
#endif
                            c = m / n + 1; // (int)Math.Round(Convert.ToDouble(m / n)) + 1;
                        }
                        else
                        {
                            c = m / n;
                        }

                        CommData.Cycle = c;

                        //==============================

                        int chan = GetChan(item);
                        int skip = 0;
                        for (int i = 1; i <= CommData.Cycle; i++)
                        {
                            //   int index=i-1;
                            int k = (i * 12) - 1 - skip;

                            if (k >= CommData.diclist[item].Count)
                            {
                                factorValue[chan, i] = 1;
#if DEBUG
                                MessageBox.Show("File corruption detected, factor data in wrong row");
#endif
                                continue;
                            }

                            char[] charSeparator = new char[] { ' ' };

                            string[] strs = CommData.diclist[item][k].Split(charSeparator, StringSplitOptions.RemoveEmptyEntries);
                            factorValue[chan, i] = CommData.GetFactor(Convert.ToInt32(strs[11]));

                            int rn = Convert.ToInt32(strs[12]);

                            //Debug.Assert(rn == 11);

                            if (rn != 11 && rn < 12)
                            {
#if DEBUG
                                MessageBox.Show("File corruption detected, factor data not right position");
#endif
                                skip += rn + 1;

                                k = (i * 12) - 1 - skip;
                                strs = CommData.diclist[item][k].Split(charSeparator, StringSplitOptions.RemoveEmptyEntries);
                                factorValue[chan, i] = CommData.GetFactor(Convert.ToInt32(strs[11]));
                            }
                            else if (rn > 12)
                            {
                                factorValue[chan, i] = 1;
                                continue;
                            }

                            if (i == 1)
                                factorValue[chan, 0] = factorValue[chan, i];
                        }
                    }
                    XdcAxisRange.MaxValue = CommData.Cycle + 2;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message + "RunCovid ReadFileNew");
#endif
            }
        }

        private void ParseDpstr(string dpstr)       // Obsolete as of 5/1/2020
        {
            if (String.IsNullOrEmpty(dpstr))
                return;

            string[] s = dpstr.Split(' ');
            int len = s.Length - 1;             // because last string is empty;

            byte[] trim_buff = new byte[len];

            for (int i = 0; i < len; i++)
            {
                trim_buff[i] = Convert.ToByte(s[i]);
            }

            List<int> rlist = new List<int>();      // row index
            List<int> clist = new List<int>();      // col index                           

            int k = 0;

            byte version = trim_buff[k]; k++;
            byte sn1 = trim_buff[k]; k++;
            byte sn2 = trim_buff[k]; k++;

            int num_channels = trim_buff[k]; k++;
            int num_wells = trim_buff[k]; k++;
            int num_pages = trim_buff[k]; k++;

            CommData.KsIndex = num_wells;
            CommData.TdIndex = num_channels;

            CommData.ver = version;
            CommData.sn1 = sn1;
            CommData.sn2 = sn2;

            for (int i = 0; i < num_channels; i++)
            {
                for (int j = 0; j < num_wells; j++)
                {
                    int n = trim_buff[k]; k++;
                    rlist.Clear();
                    clist.Clear();
                    for (int l = 0; l < n; l++)
                    {
                        int row = trim_buff[k++]; // k++;
                        int col = trim_buff[k]; k++;

                        rlist.Add(row);
                        clist.Add(col);
                    }
                    CommData.row_index[i, j] = new List<int>(rlist);
                    CommData.col_index[i, j] = new List<int>(clist);
                }
            }

            CommData.dpinfo_loaded = true;
            CommData.UpdateDarkMap();
        }

        public int ParseDicList()
        {
            try
            {
                if (CommData.experimentModelData.ampData.Count > 0)
                {
                    CommData.diclist = CommData.experimentModelData.ampData;
                    //CommData.row_index = CommData.experimentModelData.row_index;
                    //CommData.col_index = CommData.experimentModelData.col_index;
                    //CommData.dpinfo_loaded = true;

                    string dpstr = CommData.experimentModelData.dpStr;

                    CommData.ParseDpstr(dpstr);

                    //string[] s2 = dpstr.Split(' ');
                    //int len = s2.Length - 1;

                    //byte[] trim_buff = new byte[len];

                    //for (int i = 0; i < len; i++)
                    //{
                    //    trim_buff[i] = Convert.ToByte(s2[i]);
                    //}

                    //List<int> rlist = new List<int>();      // row index
                    //List<int> clist = new List<int>();      // col index                           

                    //int k = 0;

                    //char version = (char)trim_buff[k]; k++;
                    //int sn1 = trim_buff[k]; k++;
                    //int sn2 = trim_buff[k]; k++;

                    //int num_channels = trim_buff[k]; k++;
                    //int num_wells = trim_buff[k]; k++;
                    //int num_pages = trim_buff[k]; k++;

                    //if (CommData.row_index[0, 0] == null && CommData.col_index[0, 0] == null || true)           // This means dp data not loaded yet from flash
                    //{
                    //    CommData.KsIndex = num_wells;
                    //    // CommData.flash_loaded = true;                                                   // treated the same as flash loaded.
                    //    CommData.TdIndex = num_channels;

                    //    for (int i = 0; i < num_channels; i++)
                    //    {
                    //        for (int j = 0; j < num_wells; j++)
                    //        {
                    //            int n = trim_buff[k]; k++;
                    //            rlist.Clear();
                    //            clist.Clear();
                    //            for (int l = 0; l < n; l++)
                    //            {
                    //                int row = trim_buff[k++]; // k++;
                    //                int col = trim_buff[k]; k++;

                    //                rlist.Add(row);
                    //                clist.Add(col);
                    //            }
                    //            CommData.row_index[i, j] = new List<int>(rlist);
                    //            CommData.col_index[i, j] = new List<int>(clist);
                    //        }
                    //    }
                    //    CommData.dpinfo_loaded = true;
                    //}
                }
                else
                {
                    return 0;
                }

                if (true)
                {
                    foreach (var item in CommData.diclist.Keys)
                    {
                        if (CommData.diclist[item].Count == 0) continue;

                        //    CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);

                        //=======================Zhimin: deal with bad cycle number=================

                        int m, n, c;

                        m = Convert.ToInt32(CommData.diclist[item].Count);
                        n = CommData.imgFrame;


                        if (m % n != 0)
                        {
#if DEBUG
                            MessageBox.Show("File corruption detected, missing rows");
#endif
                            c = m / n + 1; // (int)Math.Round(Convert.ToDouble(m / n)) + 1;
                        }
                        else
                        {
                            c = m / n;
                        }

                        CommData.Cycle = c;

                        //==============================

                        int chan = GetChan(item);
                        int skip = 0;
                        for (int i = 1; i <= CommData.Cycle; i++)
                        {
                            //   int index=i-1;
                            int k = (i * 12) - 1 - skip;

                            if (k >= CommData.diclist[item].Count)
                            {
                                factorValue[chan, i] = 1;
#if DEBUG
                                MessageBox.Show("File corruption detected, factor data in wrong row");
#endif
                                continue;
                            }

                            char[] charSeparator = new char[] { ' ' };

                            string[] strs = CommData.diclist[item][k].Split(charSeparator, StringSplitOptions.RemoveEmptyEntries);
                            factorValue[chan, i] = CommData.GetFactor(Convert.ToInt32(strs[11]));

                            int rn = Convert.ToInt32(strs[12]);

                            //Debug.Assert(rn == 11);

                            if (rn != 11 && rn < 12)
                            {
#if DEBUG
                                MessageBox.Show("File corruption detected, factor data not right position");
#endif
                                skip += rn + 1;

                                k = (i * 12) - 1 - skip;
                                strs = CommData.diclist[item][k].Split(charSeparator, StringSplitOptions.RemoveEmptyEntries);
                                factorValue[chan, i] = CommData.GetFactor(Convert.ToInt32(strs[11]));
                            }
                            else if (rn > 12)
                            {
                                factorValue[chan, i] = 1;
                                continue;
                            }

                            if (i == 1)
                                factorValue[chan, 0] = factorValue[chan, i];
                        }
                    }
                    XdcAxisRange.MaxValue = CommData.Cycle + 2;
                }

                return CommData.experimentModelData.ampData.Count;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "RunCovid ParseDic");
                return 0;
            }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ChooseM != null)
        //    {
        //        ChooseM("1", null);
        //    }
        //}

        private void gdA1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            string chan = "Chip#2";
            DrawLine(chan, 4, grid.Tag.ToString());
        }

        public void DrawLineNew()
        {
            //if (KSList.Count == 0)
            //{
            //    dcXYDiagram2D.Series.Clear();
            //    return;
            //}
            //int count = dcXYDiagram2D.Series.Count;


            //for (int i = 0; i < KSList.Count; i++)
            //{
            //    for (int k = 0; k < dcXYDiagram2D.Series.Count; k++)
            //    {
            //        LineSeries2D ls = dcXYDiagram2D.Series[i] as LineSeries2D;
            //        string[] ks = ls.Tag.ToString().Split(':');
            //        if (!KSList.Contains(ks[1]))
            //        {
            //            dcXYDiagram2D.Series.Remove(dcXYDiagram2D.Series[i]);
            //        }
            //    }
            //}
            dcXYDiagram2D.Series.Clear();
            dcXYDiagram2D.ActualAxisX.ConstantLinesInFront.Clear();
            dcXYDiagram2D.ActualAxisY.ConstantLinesInFront.Clear();

            // refresh

            //=========

            int count = 0;

            foreach (var chan in ChanList)
            {
                foreach (var ks in KSList)
                {
                    count = DrawLine(chan, 4, ks);
                }
            }

            SetTextVisbile();

            /*            LineSeries2D line = null;

                        foreach (var item in dcXYDiagram2D.Series)
                        {
                            if (item.Tag.ToString() == "" || true)
                            {
                                line = item as LineSeries2D;
                                //                    rawL = item as LineSeries2D;

                                LineStyle ls = new LineStyle();
                                // ls.DashStyle = DashStyles.Dot;

                                ls.Thickness = 1;

                                line.LineStyle = ls;

                                break;
                            }
                        } */

            string bj;

            if (norm_top)
            {
                bj = "Ct Threshold: " + (threshold).ToString("0.0") + "%";

                ConstantLine thLine = new ConstantLine()      // Zhimin: Ct threshold line, Fixme: This is adding a lot of lines, I only need one.
                {
                    Value = 50 * threshold,
                    Title = new ConstantLineTitle()
                    {
                        Content = bj
                    }
                };

                LineStyle ls2 = new LineStyle();
                ls2.DashStyle = DashStyles.Dot;

                thLine.LineStyle = ls2;
                thLine.Brush = new SolidColorBrush(Color.FromRgb(200, 160, 120));

                int minCt = Convert.ToInt32(txtMinCt.Text);

                if (count >= minCt) dcXYDiagram2D.ActualAxisY.ConstantLinesInFront.Add(thLine);

            }

        }

        const bool showCtLine = false;

        public int DrawLine(string chan, int ks, string currks)
        {
            if (!CommData.diclist.Keys.Contains(chan) || CommData.diclist[chan].Count == 0) return 0;

            List<ChartData> cdlist = CommData.GetChartData(chan, 4, currks);
            if (cdlist.Count == 0) return 0;
            double y_max_value = cdlist.Max(a => a.y);
            double y_min_value = cdlist.Min(a => a.y);
            //int minvalue_y = cdlist.Min(a => a.y);
            //int maxvalue_y = cdlist.Max(a => a.y);
            //dcAxisRange.MinValue = y_min_value-100;
            //dcAxisRange.MaxValue = y_max_value + 100;

            bool isadd = false;
            LineSeries2D dxcLs1 = null;
            LineSeries2D rawL; // Zhimin: Raw line

            SolidColorBrush myBrush;

            /*
            LineSeries2D thLine; // Zhimin: Raw line
            thLine = new LineSeries2D();
            thLine.Tag = "my tag";
            thLine.DisplayName = "my name";
            thLine.MarkerVisible = false;
            thLine.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            thLine.Brush = new SolidColorBrush(Color.FromRgb(180, 180, 180));

            LineStyle ls = new LineStyle();
            ls.DashStyle = DashStyles.DashDotDot;
            thLine.LineStyle = ls;

            SeriesPoint p0 = new SeriesPoint();
            SeriesPoint p1 = new SeriesPoint();

            p0.Argument = "0";
            p1.Argument = "1";
            p0.Value = 5;
            thLine.Points.Add(p0);
            //thLine.Points.Insert(5, p0);
            p1.Value = 50;
            thLine.Points.Add(p1);
            //thLine.Points.Insert(5, p1);
            */
            rawL = new LineSeries2D();
            rawL.Tag = chan + ":" + currks;
            rawL.DisplayName = chan + ":" + currks;
            rawL.MarkerVisible = true;
            //                rawL.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

            int currChan = 0;
            int ksindex = -1;

            switch (chan)
            {
                case "Chip#1":
                    currChan = 0;
                    myBrush = new SolidColorBrush(Color.FromRgb(24, 60, 209));
                    break;
                case "Chip#2":
                    currChan = 1;
                    myBrush = new SolidColorBrush(Color.FromRgb(83, 182, 97));
                    break;
                case "Chip#3":
                    currChan = 2;
                    myBrush = new SolidColorBrush(Color.FromRgb(245, 195, 66));
                    break;
                case "Chip#4":
                    currChan = 3;
                    myBrush = new SolidColorBrush(Color.FromRgb(234, 51, 35));
                    break;
                default:
                    myBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                    break;
            }

            if (CommData.KsIndex == 16)
            {
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "A5":
                        ksindex = 4;
                        break;
                    case "A6":
                        ksindex = 5;
                        break;
                    case "A7":
                        ksindex = 6;
                        break;
                    case "A8":
                        ksindex = 7;
                        break;
                    case "B1":
                        ksindex = 8;
                        break;
                    case "B2":
                        ksindex = 9;
                        break;
                    case "B3":
                        ksindex = 10;
                        break;
                    case "B4":
                        ksindex = 11;
                        break;
                    case "B5":
                        ksindex = 12;
                        break;
                    case "B6":
                        ksindex = 13;
                        break;
                    case "B7":
                        ksindex = 14;
                        break;
                    case "B8":
                        ksindex = 15;
                        break;
                }
            }
            else
            {
#if TwoByFour
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "B1":
                        ksindex = 4;
                        break;
                    case "B2":
                        ksindex = 5;
                        break;
                    case "B3":
                        ksindex = 6;
                        break;
                    case "B4":
                        ksindex = 7;
                        break;
                }
#else
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "A5":
                        ksindex = 4;
                        break;
                    case "A6":
                        ksindex = 5;
                        break;
                    case "A7":
                        ksindex = 6;
                        break;
                    case "A8":
                        ksindex = 7;
                        break;
                }
#endif
            }


            rawL.Brush = new SolidColorBrush(Color.FromRgb(185, 185, 185));

            double ct = m_CTValue[currChan, ksindex];
            string bj = /* chan + ":" + */ currks + ": " + ct.ToString("0.00");
            bool falsepos = m_falsePositive[currChan, ksindex];

            foreach (var item in dcXYDiagram2D.Series)
            {
                if (item.Tag.ToString() == bj)
                {
                    dxcLs1 = item as LineSeries2D;
                    //                    rawL = item as LineSeries2D;
                    isadd = false;
                    break;
                }
            }

            if (dxcLs1 == null)
            {
                isadd = true;

                dxcLs1 = new LineSeries2D();
                dxcLs1.Tag = chan + ":" + currks;
                dxcLs1.DisplayName = chan + ":" + currks;
                dxcLs1.MarkerVisible = false;

                //                dxcLs1.CrosshairLabelPattern = "{S} {A} : {V}";
                dxcLs1.CrosshairLabelPattern = "{S} : {V}";

#if ENGLISH_VER
                // dxcLs1.DisplayName = "Channel " + (currChan + 1).ToString() + "-" + currks;

                dxcLs1.DisplayName = GetSampleName(currks) + " - " + currks + "(" + "Chan" + (currChan + 1).ToString() + ")";
#else
                dxcLs1.DisplayName = "通道" + (currChan + 1).ToString() + "-" + currks;
#endif


                dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

                LineStyle lss = new LineStyle();
                lss.Thickness = 1;

                dxcLs1.LineStyle = lss;


                dxcLs1.Brush = myBrush;

            }

            int count = cdlist.Count;

#if VARIABLE_CYCLES

            //======3-6-2020 Add Get actual cycle number for the well, feature for Lumin========

            char[] carr;    // char array
            carr = currks.ToCharArray();

            int wi = Convert.ToInt32(carr[1].ToString()) - 1;

            int row;    // 0: A, 1: B

            if (carr[0] == 'A')
                row = 0;
            else
                row = 1;

            int ai = CommData.experimentModelData.sampleAssayIndex[row, wi];

            int actual_cycle = count;

            if (ai >= 0 && ai < CommData.assayList.Count - 1)
            {
                Assay assay = CommData.assayList[ai];
                CyclerParam cp = assay.cyclerParam;

                actual_cycle = cp.numCycles;                // actual number of cycles for the assay
            }

            if (actual_cycle < count - 1)
                count = actual_cycle + 1;

            if (ct > Convert.ToDouble(actual_cycle))
            {
                falsepos = true;
                m_falsePositive[currChan, ksindex] = true;
            }

            //==================================================================================
#endif

            //for (int n=0;n<cdlist.Count;n++)
            //{
            //    bool flag = false;
            //    foreach (var sp1 in dxcLs1.Points)
            //    {
            //        if (sp1.Argument == cdlist[n].x.ToString())
            //        {
            //            flag = true;
            //            break;
            //        }
            //    }
            //    if (flag == true) continue;
            //    //SeriesPoint sp = new SeriesPoint();
            //    //sp.Argument = "0";
            //    //sp.Value = -200;
            //    //dxcLs1.Points.Add(sp);

            //    SeriesPoint sp = new SeriesPoint();
            //    sp.Argument = cdlist[n].x.ToString();
            //    sp.Value = cdlist[n].y / factorValue[currChan, n];
            //    dxcLs1.Points.Add(sp);
            //}


            //if (isadd == true)
            //{
            //    dcXYDiagram2D.Series.Add(dxcLs1);
            //}

            //dxcLs1 = new LineSeries2D();
            //dxcLs1.Tag = chan + ":" + currks;
            //dxcLs1.DisplayName = chan + ":" + currks;
            //dxcLs1.MarkerVisible = false;
            //dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            //dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(155, 9, 243));
            for (int i = 0; i < count; i++)
            {
                SeriesPoint sp = new SeriesPoint();
                sp.Argument = i.ToString();
                //sp.Value = m_zData[currChan, ksindex, i] / factorValue[currChan, i];
                sp.Value = Math.Round(m_zData[currChan, ksindex, i], 2);
                dxcLs1.Points.Add(sp);

                SeriesPoint spr = new SeriesPoint();
                spr.Argument = i.ToString();
                //                spr.Value = m_yData[currChan, ksindex, i] - m_mean[currChan, ksindex];
                spr.Value = Math.Round(m_zData2[currChan, ksindex, i], 2);
                rawL.Points.Add(spr);

            }
            dcXYDiagram2D.Series.Add(dxcLs1);

            //if(bTest) dcXYDiagram2D.Series.Add(thLine);

            if (bTest) dcXYDiagram2D.Series.Add(rawL);

            ConstantLine constantLine = new ConstantLine()      // Zhimin: Ct line
            {
                Value = ct,
                Title = new ConstantLineTitle()
                {
                    Content = bj //  "Ct: 21"
                }
            };

            LineStyle ls = new LineStyle();
            ls.DashStyle = DashStyles.Dash;
            ls.Thickness = 1;

            constantLine.LineStyle = ls;
            constantLine.Brush = myBrush; //  new SolidColorBrush(Color.FromRgb(24, 60, 209));

            if (ct > 0.1 && !falsepos && showCtLine) dcXYDiagram2D.ActualAxisX.ConstantLinesInFront.Add(constantLine);

            /*            if (norm_top)
                        {
                            bj = "Ct Threshold: " + (threshold).ToString("0.0") + "%";

                            ConstantLine thLine = new ConstantLine()      // Zhimin: Ct threshold line, Fixme: This is adding a lot of lines, I only need one.
                            {
                                Value = 50 * threshold,
                                Title = new ConstantLineTitle()
                                {
                                    Content = bj
                                }
                            };

                            LineStyle ls2 = new LineStyle();
                            ls2.DashStyle = DashStyles.Dot;

                            thLine.LineStyle = ls2;
                            thLine.Brush = new SolidColorBrush(Color.FromRgb(200, 160, 120));

                            if (count > 13) dcXYDiagram2D.ActualAxisY.ConstantLinesInFront.Add(thLine);

                        }
            */
            radChart.Animate();

            return count;
        }

        private string GetSampleName(string well)
        {
            char[] carr;    // char array
            carr = well.ToCharArray();

            int index = Convert.ToInt32(carr[1].ToString()) - 1;

            int row;    // 0: A, 1: B

            if (carr[0] == 'A')
                row = 0;
            else
                row = 1;

            experiment emd = CommData.experimentModelData;

            return emd.sampleName[row, index];
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox rb = sender as CheckBox;
            ChanList.Add(rb.Tag.ToString());
            DrawLineNew();
            //if (!string.IsNullOrEmpty(currKS))
            //{
            //    DrawLine(currChan, 4, currKS);
            //}
        }

        private void gdA1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox rb = sender as CheckBox;
            KSList.Add(rb.Tag.ToString());
            DrawLineNew();
            //DrawLine(currChan, 4, currKS);
        }

        private void gdA1_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox rb = sender as CheckBox;
            KSList.Remove(rb.Tag.ToString());
            DrawLineNew();
        }

        private void chan1_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox rb = sender as CheckBox;
            ChanList.Remove(rb.Tag.ToString());
            DrawLineNew();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            OpenReadFile();
            ReadCCurveShow();
            DrawLineNew();
        }

        private void rbcs_Click(object sender, RoutedEventArgs e)
        {
            threshold = Convert.ToDouble(txtCtThreshold.Text);

            if (threshold < 3) threshold = 3;
            else if (threshold > 50) threshold = 50;

            CommData.experimentModelData.curfitCtTh = 0.01 * threshold;


            ReadCCurveShow();
            DrawLineNew();

            //if(CommData.cloudPending == 2)
            //{
            //    CommData.cloudPending = 3;
            //}
        }

        private void rbReport_Click(object sender, RoutedEventArgs e)
        {
            PrintReport();
        }

        public void PrintReport()
        {
            try
            {
                string timestr = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString(" hhmmss"));
                string fname = string.Format("Images\\ADCData_{0}.png", timestr);
                string path = AppDomain.CurrentDomain.BaseDirectory + fname;
                RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)this.radChart.ActualWidth, (int)this.radChart.ActualHeight, 96d, 96d, PixelFormats.Default);
                targetBitmap.Render(this.radChart); PngBitmapEncoder saveEncoder = new PngBitmapEncoder();
                saveEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                System.IO.FileStream fs = System.IO.File.Create(path);
                saveEncoder.Save(fs);
                fs.Close();

                string FilePath = AppDomain.CurrentDomain.BaseDirectory + "Report/报告模板.xlsx";
                string excelnewFilePath = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report\\{0}报告模板.xlsx", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));
                string pdfnewFilePath = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report\\{0}报告模板.pdf", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));

                ExcelEdit ExcelEdit = new ExcelEdit();
                ExcelEdit.Open(FilePath);

                ExcelEdit.ws = ExcelEdit.GetSheet("Sheet1");
                Microsoft.Office.Interop.Excel.Range Cell1 = (Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[7, "B"];
                Microsoft.Office.Interop.Excel.Range Cell2 = (Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[22, "J"];

                Microsoft.Office.Interop.Excel.Range SourceRange = ExcelEdit.ws.get_Range(Cell1, Cell2);

                ExcelEdit.InsertPicture(SourceRange, ExcelEdit.ws, path);

                if (CommData.KsIndex == 4)
                {
                    /*                    ExcelEdit.ws.Cells[29, "C"] = Math.Round(m_CTValue[0, 0], 2);
                                        ExcelEdit.ws.Cells[29, "D"] = m_CTValue[0, 1];
                                        ExcelEdit.ws.Cells[29, "E"] = m_CTValue[0, 2];
                                        ExcelEdit.ws.Cells[29, "F"] = m_CTValue[0, 3];

                                        ExcelEdit.ws.Cells[30, "C"] = m_CTValue[1, 0];
                                        ExcelEdit.ws.Cells[30, "D"] = m_CTValue[1, 1];
                                        ExcelEdit.ws.Cells[30, "E"] = m_CTValue[1, 2];
                                        ExcelEdit.ws.Cells[30, "F"] = m_CTValue[1, 3];

                                        ExcelEdit.ws.Cells[31, "C"] = m_CTValue[2, 0];
                                        ExcelEdit.ws.Cells[31, "D"] = m_CTValue[2, 1];
                                        ExcelEdit.ws.Cells[31, "E"] = m_CTValue[2, 2];
                                        ExcelEdit.ws.Cells[31, "F"] = m_CTValue[2, 3];

                                        ExcelEdit.ws.Cells[32, "C"] = m_CTValue[3, 0];
                                        ExcelEdit.ws.Cells[32, "D"] = m_CTValue[3, 1];
                                        ExcelEdit.ws.Cells[32, "E"] = m_CTValue[3, 2];
                                        ExcelEdit.ws.Cells[32, "F"] = m_CTValue[3, 3];
                                        */
                    for (int i = 0; i < MAX_CHAN; i++)
                    {
                        string[] ct = new string[4];

                        for (int j = 0; j < 4; j++)
                        {
                            if (CommData.IsEmptyWell(i, j))
                            {
                                ct[j] = "";
                            }
                            else if (m_CTValue[i, j] > 0.01 && !m_falsePositive[i, j])
                            {
                                ct[j] = m_CTValue[i, j].ToString("0.00"); // Math.Round(m_CTValue[i, j], 2);
                            }
                            else
                            {
                                ct[j] = "阴性";
                            }
                        }

                        ExcelEdit.ws.Cells[29 + i, "C"] = ct[0];
                        ExcelEdit.ws.Cells[29 + i, "D"] = ct[1];
                        ExcelEdit.ws.Cells[29 + i, "E"] = ct[2];
                        ExcelEdit.ws.Cells[29 + i, "F"] = ct[3];
                    }
                }
                else if (CommData.KsIndex == 8)
                {
                    /*                    ExcelEdit.ws.Cells[29, "C"] = m_CTValue[0, 0];
                                        ExcelEdit.ws.Cells[29, "D"] = m_CTValue[0, 1];
                                        ExcelEdit.ws.Cells[29, "E"] = m_CTValue[0, 2];
                                        ExcelEdit.ws.Cells[29, "F"] = m_CTValue[0, 3];

                                        ExcelEdit.ws.Cells[30, "C"] = m_CTValue[1, 0];
                                        ExcelEdit.ws.Cells[30, "D"] = m_CTValue[1, 1];
                                        ExcelEdit.ws.Cells[30, "E"] = m_CTValue[1, 2];
                                        ExcelEdit.ws.Cells[30, "F"] = m_CTValue[1, 3];

                                        ExcelEdit.ws.Cells[31, "C"] = m_CTValue[2, 0];
                                        ExcelEdit.ws.Cells[31, "D"] = m_CTValue[2, 1];
                                        ExcelEdit.ws.Cells[31, "E"] = m_CTValue[2, 2];
                                        ExcelEdit.ws.Cells[31, "F"] = m_CTValue[2, 3];

                                        ExcelEdit.ws.Cells[32, "C"] = m_CTValue[3, 0];
                                        ExcelEdit.ws.Cells[32, "D"] = m_CTValue[3, 1];
                                        ExcelEdit.ws.Cells[32, "E"] = m_CTValue[3, 2];
                                        ExcelEdit.ws.Cells[32, "F"] = m_CTValue[3, 3];

                                        ExcelEdit.ws.Cells[35, "C"] = m_CTValue[0, 4];
                                        ExcelEdit.ws.Cells[35, "D"] = m_CTValue[0, 5];
                                        ExcelEdit.ws.Cells[35, "E"] = m_CTValue[0, 6];
                                        ExcelEdit.ws.Cells[35, "F"] = m_CTValue[0, 7];

                                        ExcelEdit.ws.Cells[36, "C"] = m_CTValue[1, 4];
                                        ExcelEdit.ws.Cells[36, "D"] = m_CTValue[1, 5];
                                        ExcelEdit.ws.Cells[36, "E"] = m_CTValue[1, 6];
                                        ExcelEdit.ws.Cells[36, "F"] = m_CTValue[1, 7];

                                        ExcelEdit.ws.Cells[37, "C"] = m_CTValue[2, 4];
                                        ExcelEdit.ws.Cells[37, "D"] = m_CTValue[2, 5];
                                        ExcelEdit.ws.Cells[37, "E"] = m_CTValue[2, 6];
                                        ExcelEdit.ws.Cells[37, "F"] = m_CTValue[2, 7];

                                        ExcelEdit.ws.Cells[38, "C"] = m_CTValue[3, 4];
                                        ExcelEdit.ws.Cells[38, "D"] = m_CTValue[3, 5];
                                        ExcelEdit.ws.Cells[38, "E"] = m_CTValue[3, 6];
                                        ExcelEdit.ws.Cells[38, "F"] = m_CTValue[3, 7];
                    */
                    for (int i = 0; i < MAX_CHAN; i++)
                    {
                        string[] ct = new string[8];

                        for (int j = 0; j < 8; j++)
                        {
                            if (CommData.IsEmptyWell(i, j))
                            {
                                ct[j] = "";
                            }
                            else if (m_CTValue[i, j] > 0.01 && !m_falsePositive[i, j])
                            {
                                ct[j] = m_CTValue[i, j].ToString("0.00"); // Math.Round(m_CTValue[i, j], 2);
                            }
                            else
                            {
                                ct[j] = "阴性";
                            }
                        }

                        ExcelEdit.ws.Cells[29 + i, "C"] = ct[0];
                        ExcelEdit.ws.Cells[29 + i, "D"] = ct[1];
                        ExcelEdit.ws.Cells[29 + i, "E"] = ct[2];
                        ExcelEdit.ws.Cells[29 + i, "F"] = ct[3];

                        ExcelEdit.ws.Cells[35 + i, "C"] = ct[4];
                        ExcelEdit.ws.Cells[35 + i, "D"] = ct[5];
                        ExcelEdit.ws.Cells[35 + i, "E"] = ct[6];
                        ExcelEdit.ws.Cells[35 + i, "F"] = ct[7];
                    }
                }
                else if (CommData.KsIndex > 8)
                {
                    for (int i = 0; i < MAX_CHAN; i++)
                    {
                        string[] ct = new string[16];

                        for (int j = 0; j < 16; j++)
                        {
                            if (CommData.IsEmptyWell(i, j))
                            {
                                ct[j] = "";
                            }
                            else if (m_CTValue[i, j] > 0.01 && !m_falsePositive[i, j])
                            {
                                ct[j] = m_CTValue[i, j].ToString("0.00"); // Math.Round(m_CTValue[i, j], 2);
                            }
                            else
                            {
                                ct[j] = "阴性";
                            }
                        }

                        ExcelEdit.ws.Cells[29 + i, "C"] = ct[0];
                        ExcelEdit.ws.Cells[29 + i, "D"] = ct[1];
                        ExcelEdit.ws.Cells[29 + i, "E"] = ct[2];
                        ExcelEdit.ws.Cells[29 + i, "F"] = ct[3];
                        ExcelEdit.ws.Cells[29 + i, "G"] = ct[4];
                        ExcelEdit.ws.Cells[29 + i, "H"] = ct[5];
                        ExcelEdit.ws.Cells[29 + i, "I"] = ct[6];
                        ExcelEdit.ws.Cells[29 + i, "J"] = ct[7];

                        ExcelEdit.ws.Cells[35 + i, "C"] = ct[8];
                        ExcelEdit.ws.Cells[35 + i, "D"] = ct[9];
                        ExcelEdit.ws.Cells[35 + i, "E"] = ct[10];
                        ExcelEdit.ws.Cells[35 + i, "F"] = ct[11];
                        ExcelEdit.ws.Cells[35 + i, "G"] = ct[12];
                        ExcelEdit.ws.Cells[35 + i, "H"] = ct[13];
                        ExcelEdit.ws.Cells[35 + i, "I"] = ct[14];
                        ExcelEdit.ws.Cells[35 + i, "J"] = ct[15];
                    }
                }

                if (CommData.experimentModelData != null)
                {
                    ExcelEdit.ws.Cells[5, "C"] = CommData.experimentModelData.emname;
                    //ExcelEdit.ws.Cells[5, "J"] = CommData.experimentModelData.emdatetime;
                    ExcelEdit.ws.Cells[29, "B"] = CommData.experimentModelData.chanonetype + "：" + CommData.experimentModelData.chanonedes;
                    ExcelEdit.ws.Cells[30, "B"] = CommData.experimentModelData.chantwotype + "：" + CommData.experimentModelData.chantwodes;
                    ExcelEdit.ws.Cells[31, "B"] = CommData.experimentModelData.chanthreetype + "：" + CommData.experimentModelData.chanthreedes;
                    ExcelEdit.ws.Cells[32, "B"] = CommData.experimentModelData.chanfourtype + "：" + CommData.experimentModelData.chanfourdes;
                    ExcelEdit.ws.Cells[35, "B"] = CommData.experimentModelData.chanonetype + "：" + CommData.experimentModelData.chanonedes;
                    ExcelEdit.ws.Cells[36, "B"] = CommData.experimentModelData.chantwotype + "：" + CommData.experimentModelData.chantwodes;
                    ExcelEdit.ws.Cells[37, "B"] = CommData.experimentModelData.chanthreetype + "：" + CommData.experimentModelData.chanthreedes;
                    ExcelEdit.ws.Cells[38, "B"] = CommData.experimentModelData.chanfourtype + "：" + CommData.experimentModelData.chanfourdes;
                    ExcelEdit.ws.Cells[52, "I"] = CommData.experimentModelData.emdatetime;
                }

                if (CommData.experimentModelData.DebugModelDataList != null)
                {
                    ExcelEdit.ws.Cells[40, "C"] = CommData.experimentModelData.DebugModelDataList[0].Initaldenaturation;
                    ExcelEdit.ws.Cells[41, "C"] = CommData.experimentModelData.DebugModelDataList[0].InitaldenaTime;

                    ExcelEdit.ws.Cells[42, "C"] = CommData.experimentModelData.DebugModelDataList[0].Denaturating;
                    ExcelEdit.ws.Cells[43, "C"] = CommData.experimentModelData.DebugModelDataList[0].DenaturatingTime;
                    ExcelEdit.ws.Cells[44, "C"] = CommData.experimentModelData.DebugModelDataList[0].Annealing;
                    ExcelEdit.ws.Cells[45, "C"] = CommData.experimentModelData.DebugModelDataList[0].AnnealingTime;
                    ExcelEdit.ws.Cells[46, "C"] = CommData.experimentModelData.DebugModelDataList[0].Extension;
                    ExcelEdit.ws.Cells[47, "C"] = CommData.experimentModelData.DebugModelDataList[0].ExtensionTime;
                    ExcelEdit.ws.Cells[48, "C"] = CommData.experimentModelData.DebugModelDataList[0].Holdon;
                    ExcelEdit.ws.Cells[49, "C"] = CommData.experimentModelData.DebugModelDataList[0].HoldonTime;
                    ExcelEdit.ws.Cells[50, "C"] = CommData.experimentModelData.DebugModelDataList[0].Cycle;
                }

                //for (int i = 13; i < 1000; i++)
                //{
                //    if (string.IsNullOrEmpty(((Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[i, "B"]).Text))
                //    {
                //        break;
                //    }
                //    ExcelModel em = new ExcelModel();
                //    em.mc = ((Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[i, "B"]).Text;
                //    em.value1 = ((Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[i, "E"]).Text;
                //    HPList.Add(em);
                //}

                ExcelEdit.SaveAs(excelnewFilePath);

                ExcelEdit.Close();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "文本文件(*.pdf)|*.pdf|所有文件|*.*";//设置文件类型
                //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
                sfd.DefaultExt = "PDF";//设置默认格式（可以不设）
                sfd.AddExtension = true;//设置自动在文件名中添加扩展名
                if (sfd.ShowDialog() == true)
                {

                    pdfnewFilePath = sfd.FileName;
                    bool success = ExcelConvertPDF(excelnewFilePath, pdfnewFilePath, Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF);
                    if (success) MessageBox.Show("PDF 文件保存成功");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string PrintReportImage()
        {
            string rpath = "";

            try
            {
                string timestr = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString(" hhmmss"));
                string fname = string.Format("Images\\ADCData_{0}.png", timestr);
                string path = AppDomain.CurrentDomain.BaseDirectory + fname;
                RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)this.radChart.ActualWidth, (int)this.radChart.ActualHeight, 96d, 96d, PixelFormats.Default);
                targetBitmap.Render(this.radChart); PngBitmapEncoder saveEncoder = new PngBitmapEncoder();
                saveEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                System.IO.FileStream fs = System.IO.File.Create(path);
                saveEncoder.Save(fs);
                fs.Close();

                rpath = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nNot ready to print amplification curve image");
            }

            return rpath;
        }

        public void ReadCCurveShow()
        {
            try
            {
                m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
                CCurveShow CCurveShow = new CCurveShow();

                //=========pick up threshold==========

//                threshold = Convert.ToDouble(txtCtThreshold.Text);
//                if (threshold < 5) threshold = 5;
//                else if (threshold > 50) threshold = 50;

                CCurveShow.log_threshold[0] = (float)(threshold * 0.01);
                CCurveShow.log_threshold[1] = (float)(threshold * 0.01);
                CCurveShow.log_threshold[2] = (float)(threshold * 0.01);
                CCurveShow.log_threshold[3] = (float)(threshold * 0.01);

                int minCt = Convert.ToInt32(txtMinCt.Text);

                if (minCt < 5) minCt = 5;
                else if (minCt > 18) minCt = 25;

//                CCurveShow.MIN_CT = minCt;

                //====================================

                CCurveShow.InitData();

                if (!norm_top)
                {
                    CCurveShow.norm_top = false;
                }
                else
                {
                    CCurveShow.norm_top = true;
                }

                List<string> kslist = new List<string>();//定义孔数

                if (CommData.KsIndex == 16)
                {
                    kslist.Add("A1");
                    kslist.Add("A2");
                    kslist.Add("A3");
                    kslist.Add("A4");
                    kslist.Add("A5");
                    kslist.Add("A6");
                    kslist.Add("A7");
                    kslist.Add("A8");
                    kslist.Add("B1");
                    kslist.Add("B2");
                    kslist.Add("B3");
                    kslist.Add("B4");
                    kslist.Add("B5");
                    kslist.Add("B6");
                    kslist.Add("B7");
                    kslist.Add("B8");
                }
                else if(CommData.KsIndex == 8)
                {
#if TwoByFour
                kslist.Add("A1");
                kslist.Add("A2");
                kslist.Add("A3");
                kslist.Add("A4");
                kslist.Add("B1");
                kslist.Add("B2");
                kslist.Add("B3");
                kslist.Add("B4");
#else
                    kslist.Add("A1");
                    kslist.Add("A2");
                    kslist.Add("A3");
                    kslist.Add("A4");
                    kslist.Add("A5");
                    kslist.Add("A6");
                    kslist.Add("A7");
                    kslist.Add("A8");
#endif
                }
                else
                {
                    kslist.Add("A1");
                    kslist.Add("A2");
                    kslist.Add("A3");
                    kslist.Add("A4");
                }

                /*                List<string> tdlist = new List<string>();//定义通道
                                tdlist.Add("Chip#1");
                                tdlist.Add("Chip#2");
                                tdlist.Add("Chip#3");
                                tdlist.Add("Chip#4"); */

                List<string> tdlist = new List<string>();//定义通道
                if (CommData.cboChan1 == 1 || true)
                {
                    tdlist.Add("Chip#1");
                }
                if (CommData.cboChan2 == 1 || true)
                {
                    tdlist.Add("Chip#2");
                }
                if (CommData.cboChan3 == 1 || true)
                {
                    tdlist.Add("Chip#3");
                }
                if (CommData.cboChan4 == 1 || true)
                {
                    tdlist.Add("Chip#4");
                }

                int[] cyclenum = new int[MAX_CHAN];



                for (int i = 0; i < tdlist.Count /*CommData.diclist.Count*/; i++)
                {
                    List<ChartData> cdlist;
                    for (int n = 0; n < kslist.Count; n++)
                    {
                        cdlist = CommData.GetChartData(tdlist[i], 0, kslist[n]);//获取选点值
                        for (int k = 0; k < cdlist.Count; k++)
                        {
                            m_yData[i, n, k] = cdlist[k].y; // Zhimin: disabled as I will divide factor value in Commdata. / factorValue[GetChan(tdlist[i]), k];
                        }
                    }

                    cdlist = CommData.GetChartData(tdlist[i], 0, "C0"); //dark pix
                    for (int k = 0; k < cdlist.Count; k++)
                    {
                        m_bData[i, k] = cdlist[k].y;        // 
                    }

                    if (CommData.diclist.Count > 0 && CommData.diclist.ContainsKey(tdlist[i]))
                    {
                        // cyclenum = Convert.ToInt32(CommData.diclist[tdlist[i]].Count / CommData.imgFrame);

                        // Zhimin: correct number of cycle corruption

                        int l, m, n;

                        l = Convert.ToInt32(CommData.diclist[tdlist[i]].Count);
                        m = CommData.imgFrame;

                        if (l % m != 0)
                        {
#if DEBUG
                            MessageBox.Show("File corruption detected, missing rows");
#endif
                            n = l / m + 1; //  (int)Math.Round(Convert.ToDouble(l / m)) + 1;
                        }
                        else
                        {
                            n = l / m;
                        }

                        cyclenum[i] = n + 1;

                        // =============================================

                    }
                }
                CCurveShow.m_yData = m_yData;
                CCurveShow.m_bData = m_bData;
                CCurveShow.m_Size = cyclenum;
                CCurveShow.ifactor = factorValue;
                CCurveShow.UpdateAllcurve();
                m_zData = CCurveShow.m_zData;
                m_zData2 = CCurveShow.m_zData2;
                m_CTValue = CCurveShow.m_CTValue;
                m_mean = CCurveShow.m_mean;
                m_falsePositive = CCurveShow.m_falsePositive;
                m_Confidence = CCurveShow.m_Confidence;

                for (int i = 0; i< MAX_CHAN; i++)
                {
                    for(int j=0; j<MAX_WELL; j++)
                    {
                        CommData.CTValue[i,j] = CCurveShow.m_CTValue[i,j];
                        CommData.falsePositive[i, j] = CCurveShow.m_falsePositive[i, j];

                    }
                }

                /*            int chans = CommData.diclist.Keys.Count;
                            for (int i = 0; i < chans; i++)
                            {
                                for (int n = 0; n < 4; n++)
                                {
                                    TextBlock text = new TextBlock();
                                    text.Tag = i + "," + n;
                                    text.Visibility = Visibility.Collapsed;
                                    text.VerticalAlignment = VerticalAlignment.Center;
                                    text.Text = m_CTValue[i, n].ToString("0.00");
                                    gdMainA.Children.Add(text);
                                    Grid.SetRow(text, i + 1);
                                    Grid.SetColumn(text, n + 1);
                                }
                            }
                            if (CommData.KsIndex == 8)
                            {
                                for (int i = 0; i < chans; i++)
                                {
                                    int currIndex = 0;
                                    for (int n = 4; n < MAX_WELL; n++)
                                    {
                                        TextBlock text = new TextBlock();
                                        text.Tag = i + "," + n;
                                        text.Visibility = Visibility.Collapsed;
                                        text.VerticalAlignment = VerticalAlignment.Center;
                                        text.Text = m_CTValue[i, n].ToString("0.00");
                                        gdMainB.Children.Add(text);
                                        Grid.SetRow(text, i + 1);
                                        Grid.SetColumn(text, currIndex + 1);
                                        currIndex++;
                                    }
                                }
                            }
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "RunCovid analysis");
            }
        }

        // This method not only update the result table, but also calculate the results, 
        // as well as prep report string and cloud update string.

        public void SetTextVisbile()
        {
            const int NUM_ROW = 64;

            experiment emd = CommData.experimentModelData;

            bool posOK = true;
            bool negOK = true;

            bool posAvail = false;
            bool negAvail = false;

            for (int i = 0; i < 16; i++)     // wells
            {
                int wi = i;
                int row = 0;

                if (i >= 8)
                {
                    wi -= 8;
                    row = 1;
                }

                if (!string.IsNullOrEmpty(emd.sampleName[row, wi]))
                {
                    //int aindex = emd.sampleAssayIndex[row, wi];

                    //if (aindex < 0) continue;
                    //else if (aindex > CommData.assayList.Count - 1) continue;

                    //Assay assay = CommData.assayList[aindex];
                    string type = emd.sampleType[row, wi];

                    for (int j = 0; j < 4; j++)     // channels
                    {
                    //    ChannelParam cparam = assay.channelParamLists[j];

                        if (true) // (cparam.active)
                        {
                            float nokct = 40; // cparam.negCtrlOKCt;
                            float pokctstart = 1; // cparam.posCtrlOKCtStart;
                            float pokctend = 40; // cparam.posCtrlOKCtEnd;
                            //float stdslope = cparam.stdCurveSlope;
                            //float stdinter = cparam.stdCurveIntercept;
                            string stype = "Unknown"; //  cparam.type;
                            //float lodct = cparam.lodCt;

                            float ct = (float)m_CTValue[j, i];

                            if (m_falsePositive[j, i])
                                ct = 0;                            

                            if (type == "Positive control" && stype != "IC")
                            {
                                posAvail = true;
                                if (ct < pokctend && ct > pokctstart)
                                {
                                    // posOK = true;
                                }
                                else
                                {
                                    posOK = false;
                                }
                            }
                            
                            else if (type == "Negative control")
                            {
                                negAvail = true;

                                if (stype != "IC")
                                {
                                    if (ct > nokct || ct < 0.1)
                                    {
                                        negOK = true;
                                    }
                                    else
                                    {
                                        negOK = false;
                                    }
                                }
                                else
                                {
                                    if (ct < 30 || ct > 0.1)
                                    {
                                        negOK = true;
                                    }
                                    else
                                    {
                                        negOK = false;
                                    }
                                }
                            }                            
                        }
                    }
                }
            }

            //==========Check result==========

            //string[] result = new string[NUM_ROW];
            //string[] target = new string[NUM_ROW];
            //string[] concentration = new string[NUM_ROW];

            List<string> nameList = new List<string>();
            List<string> resultList = new List<string>();
            List<string> targetList = new List<string>();
            List<string> concList = new List<string>();

            List<string> sampleList = new List<string>();
            List<string> ctList = new List<string>();
            List<string> typeList = new List<string>();

            List<string> siteList = new List<string>();
            List<string> locationList = new List<string>();
            List<int> siteIdList = new List<int>();
            List<int> locationIdList = new List<int>();
            List<DateTime> timeList = new List<DateTime>();

            List<string> ctEList = new List<string>();
            List<string> ctNList = new List<string>();

            List<string> ncList = new List<string>();
            List<string> pcList = new List<string>();


            for (int i = 0; i < 16; i++)     // wells
            {
                int wi = i;
                int row = 0;

                if(i >= 8)
                {
                    wi -= 8;
                    row = 1;
                }

                if (!string.IsNullOrEmpty(emd.sampleName[row, wi]))
                {
                    //int aindex = emd.sampleAssayIndex[row, wi];

                    //if (aindex < 0) continue;
                    //else if (aindex > CommData.assayList.Count - 1) continue;

                    //Assay assay = CommData.assayList[aindex];
                    string type = emd.sampleType[row, wi];

                    if (type != "Unknown") continue;

                    string str;

                    if (row == 0) str = "A" + (wi + 1).ToString();
                    else str = "B" + (wi + 1).ToString();

                    sampleList.Add(emd.sampleName[row, wi]);
                    nameList.Add(str);

                    if (posOK && posAvail)
                        pcList.Add("Pass");
                    else if (posAvail)
                        pcList.Add("Fail");
                    else
                        pcList.Add("NA");

                    if (negOK && negAvail)
                        ncList.Add("Pass");
                    else if (negAvail)
                        ncList.Add("Fail");
                    else
                        ncList.Add("NA");

                    float ctE = 0, ctN = 0;
                    bool icOK = true;

                    bool rnok = false, iacok=false, n1pos=false;    

                    for (int j = 0; j < 4; j++)     // channels
                    {
                        // ChannelParam cparam = assay.channelParamLists[j];

                        if (true) // cparam.active)
                        {
                            float nokct = 40; // cparam.negCtrlOKCt;
                            float pokctstart = 10; // cparam.posCtrlOKCtStart;
                            float pokctend = 36; // cparam.posCtrlOKCtEnd;
                            float stdslope = 1; //  cparam.stdCurveSlope;
                            float stdinter = 1; // cparam.stdCurveIntercept;
                            string stype = "Unknown"; //  cparam.type;
                            float lodct = 35; // cparam.lodCt;

                            float ct = (float)m_CTValue[j, i];

                            if (m_falsePositive[j, i])
                                ct = 0;                                                          

                            //if (type == "Unknown" && stype == "IC" && j == 0)
                            //{
                            //    if (ct < pokctend && ct > pokctstart)
                            //    {
                            //        str = "Pass";
                            //        icOK = true;
                            //    }
                            //    else
                            //    {
                            //        str = "Fail";
                            //        icOK = false;
                            //    }

                            //    ctList.Add(str);
                            //}


                            //typeList.Add(type);

                            if (type == "Unknown" && j == 2)
                            {
                                ctE = ct;
                                ctEList.Add(ct.ToString("0.00"));

                                if (ct < 40 && ct > 5)
                                    n1pos = true;
                            }

                            if (type == "Unknown" && j == 3)
                            {
                                ctN = ct;
                                ctNList.Add(ct.ToString("0.00"));

                                if (ct < 30 && ct > 5) iacok = true;
                            }

                            if (type == "Unknown" && j == 0)
                            {
                                //ctN = ct;
                                ctList.Add(ct.ToString("0.00"));

                                if (ct < 36 && ct > 5) rnok = true;
                            }


                            // target[i] = cparam.name;
                            // targetList.Add(cparam.name);

                            //siteList.Add(emd.sampleSite[row, wi]);
                            //locationList.Add(emd.sampleLocation[row, wi]);
                            //siteIdList.Add(emd.sampleSiteId[row, wi]);
                            //locationIdList.Add(emd.sampleLocationId[row, wi]);
                            //timeList.Add(emd.sampleTime[row, wi]);

                            string cstr;

                            if (type == "Unknown" && j == 3)
                            {
                                //if (lodct > 0.1)
                                //{
                                //    if (ct < lodct && ct > 0.1)
                                //    {
                                //        str = "Positive";
                                //    }
                                //    else
                                //    {
                                //        str = "Negative";
                                //        //cstr = "< LOD";
                                //    }
                                //}
                                //else
                                //{
                                //    if (ct > 0.1)
                                //    {
                                //        str = "Positive";
                                //    }
                                //    else
                                //    {
                                //        str = "Negative";
                                //        //cstr = "< LOD";
                                //    }
                                //}

                                //if (!(posOK && negOK))
                                //    str = "Run control failed";
                                //else if(!negAvail)
                                //    str = "Invalid result";
                                //else if (ctN < 40 && ctN > 0.1)
                                //    str = "Detected";
                                //else if (ctE < 40 && ctE > 0.1 && (ctN > 40 || ctN < 0.1))
                                //    str = "Inconclusive";
                                //else if (icOK && (ctE > 40 || ctE < 0.1) && (ctN > 40 || ctN < 0.1))
                                //    str = "Non-detect";
                                //else
                                //    str = "Invalid result";

                                if (!(posOK && negOK))
                                    str = "Run control failed";
                                else if(!negAvail)
                                    str = "Invalid result";
                                else if (iacok && rnok)
                                {
                                    if (n1pos)
                                    {
                                        str = "POSITIVE";
                                    }
                                    else
                                    {
                                        str = "NEGATIVE";
                                    }
                                }
                                else
                                {
                                    str = "REPEAT TEST";
                                }

                                resultList.Add(str); 

                                  
                            }
                                                  
                            // concList.Add(cstr);
                        }
                    }
                }
            }

          

            //================================

            for (int i = 0; i < NUM_ROW; i++)
            {
                string currName;

                if (i >= ctList.Count)
                    currName = "";
                else
                    currName = ctList[i];

                string currTag = "ic" + "," + i;
                // string currTip = GetCtTip(chan, ks);
                foreach (var item in gdResult.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock textBlock = item as TextBlock;
                        if (textBlock.Tag != null && textBlock.Tag.ToString() == currTag)
                        {
                            textBlock.Visibility = Visibility.Visible;
                            textBlock.Text = currName;
                            // textBlock.VerticalAlignment = VerticalAlignment.Center;
                            //TextBlock.ToolTip = currTip;             
                            break;
                        }
                    }
                }
            }

            for (int i=0; i< NUM_ROW; i++)
            {
                string currName;

                if (i >= sampleList.Count)
                    currName = "";
                else
                    currName = sampleList[i];

                string currTag = "n" + "," + i;
                // string currTip = GetCtTip(chan, ks);
                foreach (var item in gdResult.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock textBlock = item as TextBlock;
                        if (textBlock.Tag != null && textBlock.Tag.ToString() == currTag)
                        {
                            textBlock.Visibility = Visibility.Visible;
                            textBlock.Text = currName;
                           // textBlock.VerticalAlignment = VerticalAlignment.Center;
                            //TextBlock.ToolTip = currTip;             
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < NUM_ROW; i++)
            {
                string currName;

                if (i >= ctEList.Count)
                    currName = "";
                else
                    currName = ctEList[i];
                string currTag = "eg" + "," + i;
                // string currTip = GetCtTip(chan, ks);
                foreach (var item in gdResult.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock TextBlock = item as TextBlock;
                        if (TextBlock.Tag != null && TextBlock.Tag.ToString() == currTag)
                        {
                            TextBlock.Visibility = Visibility.Visible;
                            TextBlock.Text = currName;
                            //TextBlock.ToolTip = currTip;             
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < NUM_ROW; i++)
            {
                string res;

                if (i >= resultList.Count)
                    res = "";
                else
                    res = resultList[i];
                string currTag = "r" + "," + i;
                // string currTip = GetCtTip(chan, ks);
                foreach (var item in gdResult.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock TextBlock = item as TextBlock;
                        if (TextBlock.Tag != null && TextBlock.Tag.ToString() == currTag)
                        {
                            TextBlock.Visibility = Visibility.Visible;
                            TextBlock.Text = res;
                            //TextBlock.ToolTip = currTip;             
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < NUM_ROW; i++)
            {
                string res;
                if (i >= ctNList.Count)
                    res = "";
                else
                    res = ctNList[i];
                string currTag = "ng" + "," + i;
                // string currTip = GetCtTip(chan, ks);
                foreach (var item in gdResult.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock TextBlock = item as TextBlock;
                        if (TextBlock.Tag != null && TextBlock.Tag.ToString() == currTag)
                        {
                            TextBlock.Visibility = Visibility.Visible;
                            TextBlock.Text = res;
                            //TextBlock.ToolTip = currTip;             
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < NUM_ROW; i++)
            {
                string res;
                if (i >= ncList.Count)
                    res = "";
                else
                    res = ncList[i];
                string currTag = "nc" + "," + i;
                // string currTip = GetCtTip(chan, ks);
                foreach (var item in gdResult.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock TextBlock = item as TextBlock;
                        if (TextBlock.Tag != null && TextBlock.Tag.ToString() == currTag)
                        {
                            TextBlock.Visibility = Visibility.Visible;
                            TextBlock.Text = res;
                            //TextBlock.ToolTip = currTip;             
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < NUM_ROW; i++)
            {
                string res;
                if (i >= pcList.Count)
                    res = "";
                else
                    res = pcList[i];
                string currTag = "pc" + "," + i;
                // string currTip = GetCtTip(chan, ks);
                foreach (var item in gdResult.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock TextBlock = item as TextBlock;
                        if (TextBlock.Tag != null && TextBlock.Tag.ToString() == currTag)
                        {
                            TextBlock.Visibility = Visibility.Visible;
                            TextBlock.Text = res;
                            //TextBlock.ToolTip = currTip;             
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < NUM_ROW; i++)
            {
                string res;
                if (i >= nameList.Count)
                    res = "";
                else
                    res = nameList[i];
                string currTag = "w" + "," + i;
                // string currTip = GetCtTip(chan, ks);
                foreach (var item in gdResult.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock TextBlock = item as TextBlock;
                        if (TextBlock.Tag != null && TextBlock.Tag.ToString() == currTag)
                        {
                            TextBlock.Visibility = Visibility.Visible;
                            TextBlock.Text = res;
                            //TextBlock.ToolTip = currTip;             
                            break;
                        }
                    }
                }
            }

            // return;

            try
            {
                //string result_str = "Well, Sample, Negative control, Positive control, MS2, E Gene, N2 Gene, Result\r\n";

                //for (int i = 0; i < NUM_ROW; i++)
                //{
                //    if (i >= ctEList.Count) continue;
                //    result_str += nameList[i] + ", " + sampleList[i] + ", " + ncList[i] + ", " + pcList[i] + ", " + ctList[i] + ", " + ctEList[i] + ", " + ctNList[i] + ", " + resultList[i] + "\r\n";
                //}

                // Zhimin notes, this logic is not perfect. What if one of the list is empty?

                // BioGX Logic

                string result_str = "Well, Sample, Negative control, Positive control, RNase P, N1 Gene, IAC, Result\r\n";

                for (int i = 0; i < NUM_ROW; i++)
                {
                    if (i >= ctEList.Count) continue;
                    result_str += nameList[i] + ", " + sampleList[i] + ", " + ncList[i] + ", " + pcList[i] + ", " + ctList[i] + ", " + ctEList[i] + ", " + ctNList[i] + ", " + resultList[i] + "\r\n";
                }

                CommData.csvString = result_str;
            }

            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message + "RunCovid set visible routine ");
#endif
            }

            
        }

        public string GetChanIndex(string chan, string currks)
        {
            int currChan = 0;
            int ksindex = -1;

            switch (chan)
            {
                case "Chip#1":
                    currChan = 0;
                    break;
                case "Chip#2":
                    currChan = 1;
                    break;
                case "Chip#3":
                    currChan = 2;
                    break;
                case "Chip#4":
                    currChan = 3;
                    break;
            }


            if (CommData.KsIndex == 16)
            {
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "A5":
                        ksindex = 4;
                        break;
                    case "A6":
                        ksindex = 5;
                        break;
                    case "A7":
                        ksindex = 6;
                        break;
                    case "A8":
                        ksindex = 7;
                        break;
                    case "B1":
                        ksindex = 8;
                        break;
                    case "B2":
                        ksindex = 9;
                        break;
                    case "B3":
                        ksindex = 10;
                        break;
                    case "B4":
                        ksindex = 11;
                        break;
                    case "B5":
                        ksindex = 12;
                        break;
                    case "B6":
                        ksindex = 13;
                        break;
                    case "B7":
                        ksindex = 14;
                        break;
                    case "B8":
                        ksindex = 15;
                        break;
                }
            }
            else
            {
#if TwoByFour
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "B1":
                        ksindex = 4;
                        break;
                    case "B2":
                        ksindex = 5;
                        break;
                    case "B3":
                        ksindex = 6;
                        break;
                    case "B4":
                        ksindex = 7;
                        break;
                }
#else
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "A5":
                        ksindex = 4;
                        break;
                    case "A6":
                        ksindex = 5;
                        break;
                    case "A7":
                        ksindex = 6;
                        break;
                    case "A8":
                        ksindex = 7;
                        break;
                }
#endif
            }

            return currChan + "," + ksindex;
        }

        public string GetCtValue(string chan, string currks)
        {
            int currChan = -1;
            int ksindex = -1;

            switch (chan)
            {
                case "Chip#1":
                    currChan = 0;
                    break;
                case "Chip#2":
                    currChan = 1;
                    break;
                case "Chip#3":
                    currChan = 2;
                    break;
                case "Chip#4":
                    currChan = 3;
                    break;
            }

            if (CommData.KsIndex == 16)
            {
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "A5":
                        ksindex = 4;
                        break;
                    case "A6":
                        ksindex = 5;
                        break;
                    case "A7":
                        ksindex = 6;
                        break;
                    case "A8":
                        ksindex = 7;
                        break;
                    case "B1":
                        ksindex = 8;
                        break;
                    case "B2":
                        ksindex = 9;
                        break;
                    case "B3":
                        ksindex = 10;
                        break;
                    case "B4":
                        ksindex = 11;
                        break;
                    case "B5":
                        ksindex = 12;
                        break;
                    case "B6":
                        ksindex = 13;
                        break;
                    case "B7":
                        ksindex = 14;
                        break;
                    case "B8":
                        ksindex = 15;
                        break;
                }
            }
            else
            {
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "B1":
                        ksindex = 4;
                        break;
                    case "B2":
                        ksindex = 5;
                        break;
                    case "B3":
                        ksindex = 6;
                        break;
                    case "B4":
                        ksindex = 7;
                        break;
                }
            }

            if (currChan < 0 || ksindex < 0)
            {
                return "Empty";
            }
            else
            {
                if (m_CTValue[currChan, ksindex] < 1)
#if ENGLISH_VER
                    return "Neg";
                else if (m_falsePositive[currChan, ksindex]) {
                    CommData.CTValue[currChan, ksindex] = 0;
                    return "[Neg]";  // m_CTValue[currChan, ksindex].ToString("[0.00]");
                }
#else
                return "阴性";
                else if(m_falsePositive[currChan, ksindex]) {
                    CommData.CTValue[currChan, ksindex] = 0;
                    return "[阴性]";  // m_CTValue[currChan, ksindex].ToString("[0.00]");
                }
#endif
                else
                    return m_CTValue[currChan, ksindex].ToString("0.00");
            }
        }

        public string GetCtTip(string chan, string currks)
        {
            int currChan = -1;
            int ksindex = -1;

            switch (chan)
            {
                case "Chip#1":
                    currChan = 0;
                    break;
                case "Chip#2":
                    currChan = 1;
                    break;
                case "Chip#3":
                    currChan = 2;
                    break;
                case "Chip#4":
                    currChan = 3;
                    break;
            }

            if (CommData.KsIndex == 16)
            {
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "A5":
                        ksindex = 4;
                        break;
                    case "A6":
                        ksindex = 5;
                        break;
                    case "A7":
                        ksindex = 6;
                        break;
                    case "A8":
                        ksindex = 7;
                        break;
                    case "B1":
                        ksindex = 8;
                        break;
                    case "B2":
                        ksindex = 9;
                        break;
                    case "B3":
                        ksindex = 10;
                        break;
                    case "B4":
                        ksindex = 11;
                        break;
                    case "B5":
                        ksindex = 12;
                        break;
                    case "B6":
                        ksindex = 13;
                        break;
                    case "B7":
                        ksindex = 14;
                        break;
                    case "B8":
                        ksindex = 15;
                        break;
                }
            }
            else
            {
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "B1":
                        ksindex = 4;
                        break;
                    case "B2":
                        ksindex = 5;
                        break;
                    case "B3":
                        ksindex = 6;
                        break;
                    case "B4":
                        ksindex = 7;
                        break;
                }
            }

            if (currChan < 0 || ksindex < 0)
            {
                return "Empty";
            }
            else
            {
                if (m_CTValue[currChan, ksindex] < 1)
#if ENGLISH_VER
                    return "Negative";
#else
                   return "阴性";                
#endif
                else if (m_falsePositive[currChan, ksindex])
                    return m_CTValue[currChan, ksindex].ToString("[0.00]") + " " + m_Confidence[currChan, ksindex];
                else
                    return m_CTValue[currChan, ksindex].ToString("0.00") + " " + m_Confidence[currChan, ksindex];
            }
        }

        public int GetChan(string chan)
        {
            int currChan = -1;

            switch (chan)
            {
                case "Chip#1":
                    currChan = 0;
                    break;
                case "Chip#2":
                    currChan = 1;
                    break;
                case "Chip#3":
                    currChan = 2;
                    break;
                case "Chip#4":
                    currChan = 3;
                    break;
            }
            return currChan;
        }

        private void rbcstrim_Click(object sender, RoutedEventArgs e)
        {
            TrimReader TrimReader = new Anitoa.TrimReader();

            TrimReader.ReadTrimFile();
            //int result = TrimReader.ADCCorrection(5, (byte)0x34, (byte)0x23, 12, 1, 0, 0);
            //result = TrimReader.ADCCorrection(5, (byte)0xaa, (byte)0xbb, 12, 1, 0, 0);

            int result = TrimReader.TocalADCCorrection(5, 0x10, 0x23, 12, 1, 0, 0);
        }

        /// <summary>
        /// 将excel文档转换成PDF格式
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param> 
        /// <param name="targetType"></param>
        /// <returns></returns>
        private bool ExcelConvertPDF(string sourcePath, string targetPath, Microsoft.Office.Interop.Excel.XlFixedFormatType targetType)
        {
            bool result;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.ApplicationClass application = null;
            Microsoft.Office.Interop.Excel.Workbook workBook = null;
            try
            {
                application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                object target = targetPath;
                object type = targetType;
                workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing, missing, missing);

                workBook.ExportAsFixedFormat(targetType, target, Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard, true, false, missing, missing, missing, missing);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Name == "cbNorm")
            {
                norm_top = true;
                ReadCCurveShow();
                DrawLineNew();
            }
            else
            {
                bTest = true;
            }
        }

        private void gdAB_MouseEnter(object sender, MouseEventArgs e)
        {
            LineSeries2D line = null;
            CheckBox rb = sender as CheckBox;

            String tag = rb.Tag.ToString();

            foreach (var item in dcXYDiagram2D.Series)
            {
                if (item.Tag.ToString().Contains(tag))
                {
                    line = item as LineSeries2D;
                    //                    rawL = item as LineSeries2D;

                    LineStyle ls = new LineStyle();
                    // ls.DashStyle = DashStyles.Dot;

                    ls.Thickness = 3;

                    line.LineStyle = ls;

//                    break;
                }
            }
        }

        private void gdAB_MouseLeave(object sender, MouseEventArgs e)
        {
            LineSeries2D line = null;

            foreach (var item in dcXYDiagram2D.Series)
            {
                if (item.Tag.ToString() == "" || true)
                {
                    line = item as LineSeries2D;
                    //                    rawL = item as LineSeries2D;

                    LineStyle ls = new LineStyle();
                    // ls.DashStyle = DashStyles.Dot;

                    ls.Thickness = 1;

                    line.LineStyle = ls;

//                    break;
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Name == "cbNorm")
            {
                norm_top = false;
                ReadCCurveShow();
                DrawLineNew();
            }
            else
            {
                bTest = false;
            }
        }

        public void Reinit()
        {

            foreach (var item in gdMainA.Children)
            {
                if (item is TextBlock)
                {
                    TextBlock tb = item as TextBlock;

                    if (tb.Tag != null)
                    {
                        if (tb.Tag.ToString().Contains(","))
                        {
                            tb.Tag = null;
                            tb.Text = null;
                        }
                    }
//                    gdMainA.Children.Remove(tb);
//                    break;
                }
            }

            foreach (var item in gdMainB.Children)
            {
                if (item is TextBlock)
                {
                    TextBlock tb = item as TextBlock;
                    if (tb.Tag != null)
                    {
                        if (tb.Tag.ToString().Contains(","))
                        {
                            tb.Tag = null;
                            tb.Text = null;
                        }
                    }
                    //                    gdMainB.Children.Remove(tb);
                    //                    break;
                }
            }


            if (CommData.KsIndex == 4)
            {
                //2019.04.03
                gdA5.IsEnabled = false;
                gdA6.IsEnabled = false;
                gdA7.IsEnabled = false;
                gdA8.IsEnabled = false;
                gdB1.IsEnabled = false;
                gdB2.IsEnabled = false;
                gdB3.IsEnabled = false;
                gdB4.IsEnabled = false;
                gdB5.IsEnabled = false;
                gdB6.IsEnabled = false;
                gdB7.IsEnabled = false;
                gdB8.IsEnabled = false;

                gdA5.Opacity = 0.3;
                gdA6.Opacity = 0.3;
                gdA7.Opacity = 0.3;
                gdA8.Opacity = 0.3;

                gdB1.Opacity = 0.3;
                gdB2.Opacity = 0.3;
                gdB3.Opacity = 0.3;
                gdB4.Opacity = 0.3;
                gdB5.Opacity = 0.3;
                gdB6.Opacity = 0.3;
                gdB7.Opacity = 0.3;
                gdB8.Opacity = 0.3;

                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int n = 0; n < 4; n++)                 // Zhimin: 4 wells in the first row.
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + n;
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainA.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                }
            }
            else if (CommData.KsIndex == 8)
            {
#if TwoByFour
                //2019.04.03
                gdA5.IsEnabled = false;
                gdA6.IsEnabled = false;
                gdA7.IsEnabled = false;
                gdA8.IsEnabled = false;
                gdB5.IsEnabled = false;
                gdB6.IsEnabled = false;
                gdB7.IsEnabled = false;
                gdB8.IsEnabled = false;

                gdA5.Opacity = 0.3;
                gdA6.Opacity = 0.3;
                gdA7.Opacity = 0.3;
                gdA8.Opacity = 0.3;
                gdB5.Opacity = 0.3;
                gdB6.Opacity = 0.3;
                gdB7.Opacity = 0.3;
                gdB8.Opacity = 0.3;
#else
                gdB1.IsEnabled = false;
                gdB2.IsEnabled = false;
                gdB3.IsEnabled = false;
                gdB4.IsEnabled = false;
                gdB5.IsEnabled = false;
                gdB6.IsEnabled = false;
                gdB7.IsEnabled = false;
                gdB8.IsEnabled = false;

                gdB1.Opacity = 0.3;
                gdB2.Opacity = 0.3;
                gdB3.Opacity = 0.3;
                gdB4.Opacity = 0.3;
                gdB5.Opacity = 0.3;
                gdB6.Opacity = 0.3;
                gdB7.Opacity = 0.3;
                gdB8.Opacity = 0.3;
#endif

                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + n;
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainA.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                    for (int n = 0; n < 4; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + (n + 4);
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainB.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                }
            }
            else if (CommData.KsIndex == 16)
            {
                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int n = 0; n < 8; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + n;
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainA.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                    for (int n = 0; n < 8; n++)
                    {
                        TextBlock text = new TextBlock();
                        text.Tag = i + "," + (n + 8);
                        text.Visibility = Visibility.Collapsed;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Text = "0.0";
                        gdMainB.Children.Add(text);
                        Grid.SetRow(text, i + 1);
                        Grid.SetColumn(text, n + 1);
                    }
                }
            }

            if (CommData.TdIndex < 3)
            {
                chan3.IsEnabled = false;
                chan3.Opacity = 0.3;
            }

            if (CommData.TdIndex < 4)
            {
                chan4.IsEnabled = false;
                chan4.Opacity = 0.3;
            }

        }
    }
}
