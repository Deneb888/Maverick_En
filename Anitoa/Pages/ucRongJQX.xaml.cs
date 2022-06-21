//#define DEBUG_FILE

#define ENGLISH_VER

// #define TwoByFour

using DevExpress.Xpf.Charts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace Anitoa.Pages
{
    /// <summary>
    /// ucRongJQX.xaml 的交互逻辑
    /// </summary>
    public partial class ucRongJQX : UserControl
    {
        private static int MAX_CHAN = 4;
        private static int MAX_WELL = 16;
        private static int MAX_CYCL = 501;
        public double[, ,] m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[, ,] m_zData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[, ,] m_zdData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,] m_CTValue = new double[MAX_CHAN, MAX_WELL];
        public double[,] factorValue = new double[MAX_CHAN, MAX_CYCL];
        public event EventHandler ChooseM;
        private int index = 0;
        private List<string> ChanList = new List<string>();
        private List<string> KSList = new List<string>();
        public static float start_temp = 60;

        private int[] m_cyclenum = new int[MAX_CHAN];
        private double[,] m_mtemp = new double[MAX_CHAN, MAX_CYCL];

        public ucRongJQX()
        {
            InitializeComponent();
            this.Loaded += ucRongJQX_Loaded;
        }

        void ucRongJQX_Loaded(object sender, RoutedEventArgs e)
        {
            //txtClyde.Text = CommData.Cycle.ToString();
            //dcAxisRange.MinValue = -100;

            //XdcAxisRange.MinValue = 0;
            //XdcAxisRange.MaxValue = CommData.Cycle;
            //dcAxisRange.MaxValue = 6000;
            // radChart.Animate();

            if (!string.IsNullOrEmpty(CommData.F_Path2))
            {

#if DEBUG_FILE
                ReadFileNew(".\\ImgData\\MTest.txt", 0);
#else
                if (File.Exists(CommData.F_Path2))
                {
                    if (CommData.currCycleState == 0)
                    {
                        ReadFileNew(CommData.F_Path2, 0);
                        DrawLineNew();
                    }
                }
                else
                {
                    int cnt = ParseDicList();
                    if(cnt > 0) DrawLineNew();
                }
#endif

                CommData.run1MeltMode = true;
            }
            else
            {
                int cnt = ParseDicList();
                if (cnt > 0) DrawLineNew();
            }

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
            }

            // txtStartTmp.Text = "45";

            if (CommData.experimentModelData != null && CommData.experimentModelData.DebugModelDataList != null && CommData.experimentModelData.DebugModelDataList.Count > 0)
            {

                DebugModelData dmd = CommData.experimentModelData.DebugModelDataList[0];

                txtHidTmp.Text = dmd.Hotlid.ToString();
                txtStartTmp.Text = dmd.MeltStart.ToString();
                txtEndTmp.Text = dmd.MeltEnd.ToString();

            }

            float stmp = Convert.ToSingle(txtStartTmp.Text);
            float etmp = Convert.ToSingle(txtEndTmp.Text);

            XdcAxisRange.MinValue = stmp;
            XdcAxisRange.MaxValue = etmp + 3;

            start_temp = stmp;

            InitData();

            double th = CommData.experimentModelData.meltDetTh;
            txtDetectTh.Text = th.ToString("0.0");

            return;

            chan1.IsChecked = true;
            // chan2.IsChecked = true;

            if (CommData.TdIndex < 3)
            {
                chan3.IsEnabled = false;
            }
            if (CommData.TdIndex < 4)
            {
                chan4.IsEnabled = false;
            }

            //2019.04.03
            if ((CommData.cboChan3 + CommData.cboChan4) == 0)
            {
                chan3.IsEnabled = false;
                chan4.IsEnabled = false;

                chan3.Opacity = 0.3;
                chan4.Opacity = 0.3;
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

            }
            else if (CommData.KsIndex == 8)
            {
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

            }
            else
            {
/*                gdA1.IsEnabled = true;
                gdA2.IsEnabled = true;
                gdA3.IsEnabled = true;
                gdA4.IsEnabled = true;
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
*/            }
        }

        public void InitData()
        {
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

                //gdA1.IsEnabled = true;
                //gdA2.IsEnabled = true;
                //gdA3.IsEnabled = true;
                //gdA4.IsEnabled = true;

                //gdA1.Opacity = 1.0;
                //gdA2.Opacity = 1.0;
                //gdA3.Opacity = 1.0;
                //gdA4.Opacity = 1.0;

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
                if (CommData.well_format > 1)
                {
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

                    //gdA1.IsEnabled = true;
                    //gdA2.IsEnabled = true;
                    //gdA3.IsEnabled = true;
                    //gdA4.IsEnabled = true;

                    //gdA1.Opacity = 1.0;
                    //gdA2.Opacity = 1.0;
                    //gdA3.Opacity = 1.0;
                    //gdA4.Opacity = 1.0;

                    gdB1.IsEnabled = true;
                    gdB2.IsEnabled = true;
                    gdB3.IsEnabled = true;
                    gdB4.IsEnabled = true;

                    gdB1.Opacity = 1.0;
                    gdB2.Opacity = 1.0;
                    gdB3.Opacity = 1.0;
                    gdB4.Opacity = 1.0;

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
                else
                {
                    gdA5.IsEnabled = true;
                    gdA6.IsEnabled = true;
                    gdA7.IsEnabled = true;
                    gdA8.IsEnabled = true;
                    gdB5.IsEnabled = false;
                    gdB6.IsEnabled = false;
                    gdB7.IsEnabled = false;
                    gdB8.IsEnabled = false;

                    gdA5.Opacity = 1.0;
                    gdA6.Opacity = 1.0;
                    gdA7.Opacity = 1.0;
                    gdA8.Opacity = 1.0;
                    gdB5.Opacity = 0.3;
                    gdB6.Opacity = 0.3;
                    gdB7.Opacity = 0.3;
                    gdB8.Opacity = 0.3;

                    //gdA1.IsEnabled = true;
                    //gdA2.IsEnabled = true;
                    //gdA3.IsEnabled = true;
                    //gdA4.IsEnabled = true;

                    //gdA1.Opacity = 1.0;
                    //gdA2.Opacity = 1.0;
                    //gdA3.Opacity = 1.0;
                    //gdA4.Opacity = 1.0;

                    gdB1.IsEnabled = false;
                    gdB2.IsEnabled = false;
                    gdB3.IsEnabled = false;
                    gdB4.IsEnabled = false;

                    gdB1.Opacity = 0.3;
                    gdB2.Opacity = 0.3;
                    gdB3.Opacity = 0.3;
                    gdB4.Opacity = 0.3;

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
                }
            }
            else if (CommData.KsIndex == 16)
            {
                gdA5.IsEnabled = true;
                gdA6.IsEnabled = true;
                gdA7.IsEnabled = true;
                gdA8.IsEnabled = true;
                gdB5.IsEnabled = true;
                gdB6.IsEnabled = true;
                gdB7.IsEnabled = true;
                gdB8.IsEnabled = true;

                gdA5.Opacity = 1.0;
                gdA6.Opacity = 1.0;
                gdA7.Opacity = 1.0;
                gdA8.Opacity = 1.0;
                gdB5.Opacity = 1.0;
                gdB6.Opacity = 1.0;
                gdB7.Opacity = 1.0;
                gdB8.Opacity = 1.0;

                //gdA1.IsEnabled = true;
                //gdA2.IsEnabled = true;
                //gdA3.IsEnabled = true;
                //gdA4.IsEnabled = true;

                //gdA1.Opacity = 1.0;
                //gdA2.Opacity = 1.0;
                //gdA3.Opacity = 1.0;
                //gdA4.Opacity = 1.0;

                gdB1.IsEnabled = true;
                gdB2.IsEnabled = true;
                gdB3.IsEnabled = true;
                gdB4.IsEnabled = true;

                gdB1.Opacity = 1.0;
                gdB2.Opacity = 1.0;
                gdB3.Opacity = 1.0;
                gdB4.Opacity = 1.0;

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

            //            InitCheckStatus();

           
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
        /// <param name="path"></param>
        /// <param name="type">0历史文件数据1正在进行的数据</param>
        /// "1" - type means pass dpstr header too.
        public void ReadFileNew(string path, int type)
        {
//            if (type == 2) return;

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
                bool dpheader = false;
                string dpstr = "";

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
                        if (!dpheader)
                        {
                            if (item.Contains("Chip#"))
                                continue;
                            CommData.diclist[name].Add(item);
                        }
                        else
                        {
                            dpstr = item;

                            if(type > 0) CommData.ParseDpstr(dpstr);

                            if (!String.IsNullOrEmpty(CommData.experimentModelData.dpStr) && CommData.experimentModelData.dpStr != dpstr)
                            {
                                MessageBox.Show("Equipment ID in data file mismatches current equipment ID.", "Maverick Message");
                            }
                        }
                    }
                }
                /*                if (type == 0)
                                {
                                    foreach (var item in CommData.diclist.Keys)
                                    {
                                        if (CommData.diclist[item].Count == 0) continue;
                                        //CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);
                                        //XdcAxisRange.MaxValue = CommData.Cycle;
                                        break;
                                    }
                                }
                                */

                CommData.experimentModelData.meltData = CommData.diclist;

                if(type > 0 && !string.IsNullOrEmpty(dpstr))
                    CommData.experimentModelData.dpStr = dpstr;
            }

            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message + "RJQX warn ReadFileNew");
#endif
            }
        }

        private void ParseDpstr(string dpstr)       // Obsolete as of 5/1/2020
        {
            if (string.IsNullOrEmpty(dpstr))
                return;

            string[] s = dpstr.Split(' ');
            int len = s.Length - 1;                 // because last string character is empty;

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
        }

        public int ParseDicList()
        {
            if (CommData.experimentModelData.meltData == null) return 0;

            try
            {
                if (CommData.experimentModelData.meltData.Count > 0)
                {
                    CommData.diclist = CommData.experimentModelData.meltData;

                    string dpstr = CommData.experimentModelData.dpStr;
                    CommData.ParseDpstr(dpstr);
                }

                return CommData.experimentModelData.meltData.Count;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "RJQX parseDic");
                return 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("1", null);
            }
        }

        private void gdA1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            string chan = "Chip#2";
            DrawLine(chan, 4, grid.Tag.ToString());
        }

        public void DrawLine(string chan, int ks, string currks)
        {
            // This one does not seem to be used.

            Debug.Assert(false);

            if (!CommData.diclist.Keys.Contains(chan) || CommData.diclist[chan].Count == 0)
                return;

            List<ChartDataNew> cdlist = CommData.GetChartDataByRJQX(chan, 4, currks);
            if (cdlist.Count == 0)
                return;

            LineSeries2D dxcLs1 = new LineSeries2D();
            dxcLs1.Tag = chan + ":" + currks;
            dxcLs1.DisplayName = chan + ":" + currks;
            dxcLs1.MarkerVisible = false;
//            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

            switch (chan)
            {
                case "Chip#1":
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(24, 60, 209));
                    break;
                case "Chip#2":
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(83, 182, 97));
                    break;
                case "Chip#3":
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(245, 195, 66));
                    break;
                case "Chip#4":
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(234, 51, 35));
                    break;
            }
            foreach (var item in cdlist)
            {
                SeriesPoint sp = new SeriesPoint();
                sp.Argument = item.x;
                sp.Value = Convert.ToDouble(item.y);
                dxcLs1.Points.Add(sp);
            }
            dcXYDiagram2D.Series.Add(dxcLs1);

           

//            radChart.Animate();
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

            ReadCCurveShow();

            SetTextVisbile();
            dcXYDiagram2D.Series.Clear();
            dcXYDiagram2D.ActualAxisX.ConstantLinesInFront.Clear();

            OutputCsvString();

            /*  foreach (var chan in ChanList)
              {
                  foreach (var ks in KSList)
                  {
                      DrawLine(chan, 4, ks);
                  }
              }
              */

            for (int i = 0; i < ChanList.Count; i++)
            {
                for (int n = 0; n < KSList.Count; n++)
                {
                    DeawLine(ChanList[i], KSList[n]);
                }
            }
        }

        private void gdA1_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox rb = sender as CheckBox;
            KSList.Remove(rb.Tag.ToString());
            DrawLineNew();
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

                    //line.Visible = false;

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

                    //line.Visible = true;

                }
            }
        }

        private void chan1_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox rb = sender as CheckBox;
            ChanList.Remove(rb.Tag.ToString());
            DrawLineNew();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM(null, null);
            }
        }

        public void Clear()
        {
            dcXYDiagram2D.Series.Clear();
            dcXYDiagram2D.ActualAxisX.ConstantLinesInFront.Clear();

        }

        public double TimeCount = 0;
        private void txtcurrC_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //if (txtcurrC.Text != "0")
                //{

                //    int currCycle = Convert.ToInt32(txtcurrC.Text);
                //    int TocalCycle = Convert.ToInt32(txtClyde.Text);
                //    if (txtcurrC.Text == "1")
                //    {
                //        DateTime dt1 = Convert.ToDateTime(CommData.experimentModelData.emdatetime);
                //        DateTime dt2 = DateTime.Now;
                //        TimeCount = dt2.Subtract(dt1).TotalSeconds;
                //    }

                //    double ToaTimeCount = TimeCount * (TocalCycle - currCycle);
                //    TimeSpan t = new TimeSpan(0, 0, (int)ToaTimeCount);
                //    txtsysj.Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
                //}
            }
            catch (Exception ex)
            {

            }

        }

        private void rbRefresh_Click(object sender, RoutedEventArgs e)
        {
            float stmp = Convert.ToSingle(txtStartTmp.Text);
            float etmp = Convert.ToSingle(txtEndTmp.Text);

            XdcAxisRange.MinValue = stmp;
            XdcAxisRange.MaxValue = etmp + 3;

            start_temp = stmp;

            CommData.experimentModelData.meltDetTh = Convert.ToDouble(txtDetectTh.Text);

            DrawLineNew();
        }

        public void ReadCCurveShow()
        {
            m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
            CCurveShowMet CCurveShowMet = new CCurveShowMet();
            CCurveShowMet.InitData();
            CCurveShowMet.bShowRaw = cbRaw.IsChecked == true;

            CCurveShowMet.DetectTh = Convert.ToDouble(txtDetectTh.Text);


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
            else if (CommData.KsIndex == 8)
            {
                kslist.Add("A1");
                kslist.Add("A2");
                kslist.Add("A3");
                kslist.Add("A4");
                kslist.Add("B1");
                kslist.Add("B2");
                kslist.Add("B3");
                kslist.Add("B4");

            }
            else
            {
                kslist.Add("A1");
                kslist.Add("A2");
                kslist.Add("A3");
                kslist.Add("A4");
            }

            List<string> tdlist = new List<string>();//定义通道
            if (CommData.cboChan1 == 1)
            {
                tdlist.Add("Chip#1");
            }
            if (CommData.cboChan2 == 1)
            {
                tdlist.Add("Chip#2");
            }
            if (CommData.cboChan3 == 1)
            {
                tdlist.Add("Chip#3");
            }
            if (CommData.cboChan4 == 1)
            {
                tdlist.Add("Chip#4");
            }

            double[,] mtemp = new double[MAX_CHAN, MAX_CYCL];
            int cyclenum = 0;

            for (int i = 0; i < tdlist.Count /*CommData.diclist.Count*/; i++)
            {
                for (int n = 0; n < kslist.Count; n++)
                {


                    List<ChartDataNew> cdlist = CommData.GetChartDataByRJQX(tdlist[i], 0, kslist[n]);
                    //List<ChartData> cdlist = CommData.GetChartData(tdlist[i], 0, kslist[n]);//获取选点值
                    for (int k = 0; k < cdlist.Count; k++)
                    {
                        mtemp[i, k] = Convert.ToDouble(cdlist[k].x);
                        m_yData[i, n, k] = Convert.ToDouble(cdlist[k].y); //  / CommData.m_factorData[GetChan(tdlist[i]), k];
                    }
                }
                if (CommData.diclist.Count > 0)
                {
                    //                    cyclenum = Convert.ToInt32(CommData.diclist[tdlist[i]].Count / CommData.imgFrame);
                    cyclenum = CommData.GetCycleNum(tdlist[i]);
                }
            }

            int[] m_Size = new int[MAX_CHAN];
            m_mtemp = mtemp;

            for (int n = 0; n < tdlist.Count; n++)
            {
                m_Size[n] = cyclenum;
            }

            m_cyclenum = m_Size;

            try
            {
                CCurveShowMet.m_yData = m_yData;
                CCurveShowMet.m_Size = m_Size;
                CCurveShowMet.mtemp = mtemp;
                CCurveShowMet.ifactor = CommData.m_factorData;
                CCurveShowMet.start_temp = start_temp;
                CCurveShowMet.UpdateAllcurve();
                m_zData = CCurveShowMet.m_zData;
                m_zdData = CCurveShowMet.m_zdData;
                m_CTValue = CCurveShowMet.m_CTValue;


                for (int i = 0; i < MAX_CHAN; i++)
                {
                    for (int j = 0; j < MAX_WELL; j++)
                    {
                        CommData.MTValue[i, j] = CCurveShowMet.m_CTValue[i, j];
                    }
                }
            }
            /*    for (int i = 0; i < tdlist.Count; i++)
                {
                    for (int n = 0; n < kslist.Count; n++)
                    {
                        DeawLine(tdlist[i], kslist[n]);
                    }
                }
            }
            */


            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "RJQX read data");
#endif
            }

        }

        private void DeawLine(string chan, string currks)
        {
            int currChan = 0;
            int ksindex = -1;

            LineSeries2D dxcLs1 = new LineSeries2D();
            dxcLs1.Tag = chan + ":" + currks;

            //dxcLs1.DisplayName = chan + ":" + currks;

            dxcLs1.DisplayName = GetSampleName(currks) + " - " + currks + "(" + "Chan" + (currChan + 1).ToString() + ")";

            dxcLs1.MarkerVisible = false;
            //            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

            dxcLs1.CrosshairLabelPattern = "{S} : {V}";

            dxcLs1.CrosshairLabelVisibility = CommData.showCrosshairLabel;

            LineStyle ls = new LineStyle();
            ls.Thickness = 1;
            dxcLs1.LineStyle = ls;

            string channame = ""; 

            switch (chan)
            {
                case "Chip#1":
                    currChan = 0;
                    channame = "Chan1";
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(24, 60, 209));
                    break;
                case "Chip#2":
                    currChan = 1;
                    channame = "Chan2";
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(83, 182, 97));
                    break;
                case "Chip#3":
                    currChan = 2;
                    channame = "Chan3";
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(245, 195, 66));
                    break;
                case "Chip#4":
                    currChan = 3;
                    channame = "Chan4";
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(234, 51, 35));
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
                switch (currks)                     // This is for 2 X 4 format
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

            if (ksindex < 0)
                return;

            List<ChartDataNew> cdlist = CommData.GetChartDataByRJQX(chan, 0, currks);

            int count = cdlist.Count - 1;       // Zhimin modified 5-5-2019. Drop the last data point because it is usually bad
            //dxcLs1 = new LineSeries2D();
            //dxcLs1.Tag = chan + ":" + currks;
            //dxcLs1.DisplayName = chan + ":" + currks;
            //dxcLs1.MarkerVisible = false;
            //dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            //dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(155, 9, 243));
            for (int i = 0; i < count; i++)
            {
                SeriesPoint sp = new SeriesPoint();
                sp.Argument = cdlist[i].x;
                //sp.Value = m_zData[currChan, ksindex, i] / factorValue[currChan, i];
                sp.Value = Math.Round(m_zdData[currChan, ksindex, i], 2); // m_zdData[currChan, ksindex, i];
                dxcLs1.Points.Add(sp);

            }
            dcXYDiagram2D.Series.Add(dxcLs1);

            SolidColorBrush myBrush;

            // myBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));

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


            double mt = m_CTValue[currChan, ksindex];

            string bj = channame + "-" + currks + ": " + mt.ToString("0.00");


            ConstantLine constantLine = new ConstantLine()      // Zhimin: Ct line
            {
                Value = m_CTValue[currChan, ksindex],
                Title = new ConstantLineTitle()
                {
                    Content = bj //  "Ct: 21"
                }
            };

            LineStyle ls2 = new LineStyle();
            ls2.DashStyle = DashStyles.Dash;
            ls2.Thickness = 1;

            constantLine.LineStyle = ls2;
            constantLine.Brush = myBrush; //  new SolidColorBrush(Color.FromRgb(24, 60, 209));

            if (CommData.showCtCrosshair)
                dcXYDiagram2D.ActualAxisX.ConstantLinesInFront.Add(constantLine);

            //            radChart.Animate();
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

        private void rbStart_Click(object sender, RoutedEventArgs e)
        {
            RJQXModelData model = new RJQXModelData();
            model.startTmp = txtStartTmp.Text.Trim();
            model.endTmp = txtEndTmp.Text.Trim();
            model.HotTmp = txtHidTmp.Text.Trim();
            model.rate = "0";
            if (ChooseM != null)
            {
                ChooseM(model, null);
            }
        }

        private void rbOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenReadFile();
            ReadCCurveShow();
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        public void OpenReadFile()
        {
            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.Filter = "所有文件|*.*";//若打开指定类型的文件只需修改Filter，如打开txt文件，改为*.txt即可
            pOpenFileDialog.Multiselect = false;
            pOpenFileDialog.Title = "打开文件";
            if (pOpenFileDialog.ShowDialog() == true)
            {
                string path = pOpenFileDialog.FileName;
                CommData.experimentModelData.ImgFileName2 = path;

                // ReadFileNewForfactor(path, 0);
                ReadFileNew(path, 1);
            }

        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type">0历史文件数据1正在进行的数据</param>
        public void ReadFileNewForfactor(string path, int type)
        {

            StreamReader sr = new StreamReader(path, Encoding.Default);
            var line = System.IO.File.ReadAllLines(path);
            string[] ss = line.ToArray();
            sr.Close();
            CommData.diclist = new Dictionary<string, List<string>>();
            string name = "";
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
                    if (!dpheader)
                    {
                        if (item.Contains("Chip#"))
                            continue;
                        CommData.diclist[name].Add(item);
                    }
                }
            }
            if (type == 0)
            {
                foreach (var item in CommData.diclist.Keys)
                {
                    if (CommData.diclist[item].Count == 0) continue;
                    CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);

                    int chan = GetChan(item);
                    for (int i = 1; i <= CommData.Cycle; i++)
                    {
                        //   int index=i-1;
                        int k = (i * 12) - 1;
                        string[] strs = CommData.diclist[item][k].Split(' ');
                        factorValue[chan, i] = CommData.GetFactor(Convert.ToInt32(strs[11]));
                        if (i == 1) factorValue[chan, 0] = factorValue[chan, i];
                    }
                }

                CommData.m_factorData = factorValue;
            }

        }

        public void SetTextVisbile()
        {
            //            if (gdMainA == null) return;

            foreach (var item in gdMainA.Children)
            {
                if (item is TextBlock)
                {
                    TextBlock TextBlock = item as TextBlock;
                    if (TextBlock.Tag != null)
                    {
                        TextBlock.Visibility = Visibility.Collapsed;
                        //                        TextBlock.Text += "e";        // debug feature     
                    }
                }
            }
            foreach (var item in gdMainB.Children)
            {
                if (item is TextBlock)
                {
                    TextBlock TextBlock = item as TextBlock;
                    if (TextBlock.Tag != null)
                    {
                        TextBlock.Visibility = Visibility.Collapsed;
                    }
                }
            }

            foreach (var chan in ChanList)
            {
                foreach (var ks in KSList)
                {
                    bool flag = false;
                    string currTag = GetChanIndex(chan, ks);
                    string currCt = GetCtValue(chan, ks);
//                    string currTip = GetCtTip(chan, ks);
                    foreach (var item in gdMainA.Children)
                    {
                        if (item is TextBlock)
                        {
                            TextBlock TextBlock = item as TextBlock;
                            if (TextBlock.Tag != null && TextBlock.Tag.ToString() == currTag)
                            {
                                TextBlock.Visibility = Visibility.Visible;
                                TextBlock.Text = currCt;
                                //TextBlock.ToolTip = currTip;
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag == false)
                    {
                        foreach (var item in gdMainB.Children)
                        {
                            if (item is TextBlock)
                            {
                                TextBlock TextBlock = item as TextBlock;
                                if (TextBlock.Tag != null && TextBlock.Tag.ToString() == currTag)
                                {
                                    TextBlock.Visibility = Visibility.Visible;
                                    TextBlock.Text = currCt;
                                    //TextBlock.ToolTip = currTip;
                                    break;
                                }
                            }
                        }
                    }

                }
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
            else { 
                return m_CTValue[currChan, ksindex].ToString("0.00");
            }
        }

        public string PrintReportImage()
        {
            string rpath = "";

            try
            {
                string timestr = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString(" hhmmss"));
                string fname = string.Format("Images\\MltData_{0}.png", timestr);
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
                MessageBox.Show(ex.Message + "\r\nNot ready to print melt curve image");
            }

            return rpath;
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

        public void OutputCsvString()
        {

            const int NUM_ROW = 32;

            //==========Check result==========

            List<string> nameList = new List<string>();
            List<string> resultList = new List<string>();
            List<string> targetList = new List<string>();
            List<string> concList = new List<string>();

            List<string> sampleList = new List<string>();
            List<string> mtList = new List<string>();
            List<string> typeList = new List<string>();

            experiment emd = CommData.experimentModelData;

            for (int i = 0; i < CommData.KsIndex; i++)     // wells
            {
                int wi = i;
                int row = 0;

#if TwoByFour
                if (CommData.KsIndex == 8)
                {
                    if (i >= 4)
                    {
                        wi -= 4;
                        row = 1;
                    }
                }
                else
                {
                    if (i >= 8)
                    {
                        wi -= 8;
                        row = 1;
                    }
                }
#else
                if (CommData.well_format > 1)
                {
                    if (CommData.KsIndex == 8)
                    {
                        if (i >= 4)
                        {
                            wi -= 4;
                            row = 1;
                        }
                    }
                    else
                    {
                        if (i >= 8)
                        {
                            wi -= 8;
                            row = 1;
                        }
                    }
                }
                else
                {
                    if (i >= 8)
                    {
                        wi -= 8;
                        row = 1;
                    }
                }
#endif

                if (!string.IsNullOrEmpty(emd.sampleName[row, wi]))
                {

                    string type = emd.sampleType[row, wi];

                    for (int j = 0; j < 4; j++)     // channels
                    {

                        if (emd.AssayChanEn(j)) // 
                        {

                            float mt = (float)m_CTValue[j, i];

                            string str;

                            if (row == 0) str = "A" + (wi + 1).ToString() + "-" + (j + 1).ToString();
                            else str = "B" + (wi + 1).ToString() + "-" + (j + 1).ToString();

                            string cstr = null;

                            sampleList.Add(emd.sampleName[row, wi]);
                            typeList.Add(type);
                            mtList.Add(mt.ToString("0.00"));

                            nameList.Add(str);
                            // target[i] = cparam.name;
                            targetList.Add(emd.AssayChanName(j)); //  (cparam.name);                            

                            str = "";

                            resultList.Add(str);

                            concList.Add(cstr);
                        }
                    }
                }
            }

            string result_str = "Well-Chan, Sample, Type, Target, Melt Temp, Result\r\n";

            for (int i = 0; i < NUM_ROW; i++)
            {
                if (i >= nameList.Count) continue;
                result_str += nameList[i] + ", " + sampleList[i] + ", " + typeList[i] + ", " + targetList[i] + ", " + mtList[i] + ", " + resultList[i] + "\r\n";
            }

            CommData.mtCSVString = result_str;
        }      

        public bool PrintCSVReport(string csvPath)
        {
            try
            {
                string csvString = "Experiment Name, " + CommData.experimentModelData.emname + "\r\n";

                //csvString += "Experiment Time, " + CommData.experimentModelData.emdatetime + "\r\n";
                csvString += "Melt Temperature Value Result:" + "\r\n";
                csvString += CommData.mtCSVString;

                System.IO.File.WriteAllText(csvPath, csvString, System.Text.Encoding.UTF8);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "CSV Print");
                return false;
            }
        }

        public bool PrintCSVRawData(string csvPath)
        {
            try
            {
                string csvString = "Experiment Name, " + CommData.experimentModelData.emname + "\r\n";
                //csvString += "Experiment Time, " + CommData.experimentModelData.emdatetime + "\r\n";

                csvString += "Melt Raw Data:" + "\r\n";

                //===========================
                // Dump raw data
                //===========================

                //csvString += CommData.csvString;

/*                for (int i = 0; i < CommData.TdIndex; i++)
                {
                    for (int j = 0; j < CommData.KsIndex; j++)
                    {
                        if (m_cyclenum[i] > 0)
                        {
                            csvString += "Well" + (j + 1).ToString() + "-Chan" + (i + 1).ToString() + ",";
                        }
                        for (int k = 0; k < m_cyclenum[i]; k++)
                        {
                            csvString += m_zData[i, j, k].ToString("0.00") + ", ";
                        }
                        csvString += "\r\n";
                    }
                }
*/
                int i, j, k;

                for (i = 0; i < CommData.TdIndex; i++)
                {
                    for (j = 0; j < CommData.KsIndex; j++)
                    {
                        if (m_cyclenum[i] > 0)
                        {
                            csvString += "Well" + (j + 1).ToString() + "-Chan" + (i + 1).ToString() + ",";
                        }
                    }
                }

                csvString += "\r\n";

                for (k = 0; k < MAX_CYCL; k++)
                {
                    for (i = 0; i < CommData.TdIndex; i++)
                    {
                        for (j = 0; j < CommData.KsIndex; j++)
                        {
                            if (m_cyclenum[i] > 0 && k < m_cyclenum[i])
                            {
                                csvString += m_zdData[i, j, k].ToString("0.00") + ", ";
                            }
                            else
                            {
                                continue;
                            }

                        }
                    }

                    csvString += "\r\n";
                }


                //===========================

                System.IO.File.WriteAllText(csvPath, csvString, System.Text.Encoding.UTF8);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Melt CSV Raw Print");
                return false;
            }
        }

        private void clickPrintMt(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "CSV File (*.csv)|*.csv|All files|*.*";//设置文件类型
                                                                //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
            sfd.FileName = CommData.experimentModelData.emname + "-Melt";
            sfd.DefaultExt = "CSV";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名

            if (sfd.ShowDialog() == true)
            {

                string csvNewFilePath = sfd.FileName;
                //bool success = ExcelConvertPDF(excelnewFilePath, pdfnewFilePath, Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF);

                bool success = PrintCSVReport(csvNewFilePath);

                if (success)
                    MessageBox.Show("CSV Saved Successfully");
            }
        }

        private void clickExportRaw(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "CSV File (*.csv)|*.csv|All files|*.*";//设置文件类型
                                                                //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
            sfd.FileName = CommData.experimentModelData.emname + "-MeltRaw";
            sfd.DefaultExt = "CSV";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名

            if (sfd.ShowDialog() == true)
            {

                string csvNewFilePath = sfd.FileName;
                //bool success = ExcelConvertPDF(excelnewFilePath, pdfnewFilePath, Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF);

                bool success = PrintCSVRawData(csvNewFilePath);

                if (success)
                    MessageBox.Show("CSV Saved Successfully");
            }
        }

        private void clickExportChart(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "PNG File (*.png)|*.png|All files|*.*";//设置文件类型
                                                                //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
            sfd.FileName = CommData.experimentModelData.emname + "-Melt-Chart";
            sfd.DefaultExt = "PNG";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名

            if (sfd.ShowDialog() == true)
            {
                try
                {

                    string path = sfd.FileName;
                    RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)this.radChart.ActualWidth, (int)this.radChart.ActualHeight, 96d, 96d, PixelFormats.Default);
                    targetBitmap.Render(this.radChart); PngBitmapEncoder saveEncoder = new PngBitmapEncoder();
                    saveEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                    System.IO.FileStream fs = System.IO.File.Create(path);
                    saveEncoder.Save(fs);
                    fs.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\nNot successful with printing amplification curve image");
                }
            }
        }

    }
}
