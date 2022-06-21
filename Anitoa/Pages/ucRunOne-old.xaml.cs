//#define DEBUG_FILE

using DevExpress.Xpf.Charts;
using System;
using System.Collections.Generic;
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
    /// ucRunOne.xaml 的交互逻辑
    /// </summary>
    public partial class ucRunOne : UserControl
    {
        private static int MAX_CHAN = 4;
        private static int MAX_WELL = 16;
        private static int MAX_CYCL = 501;
        public double[, ,] m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[, ,] m_zData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,] m_CTValue = new double[MAX_CHAN, MAX_WELL];
        public double[,] factorValue = new double[MAX_CHAN, MAX_CYCL];
        public event EventHandler ChooseM;
        private int index = 0;
        private List<string> ChanList = new List<string>();
        private List<string> KSList = new List<string>();
        private float cycle_time_estimate = 47;

        public ucRunOne()
        {
            InitializeComponent();
            this.Loaded += ucRunOne_Loaded;
        }

        void ucRunOne_Loaded(object sender, RoutedEventArgs e)
        {
//            txtClyde.Text = CommData.Cycle.ToString();
            txtClyde.Text = CommData.experimentModelData.CyderNum.ToString();

            //dcAxisRange.MinValue = -100;

            //XdcAxisRange.MinValue = 0;
            //XdcAxisRange.MaxValue = CommData.Cycle;
            //dcAxisRange.MaxValue = 6000;
            radChart.Animate();

            if (!string.IsNullOrEmpty(CommData.F_Path))
            {
#if DEBUG_FILE
                ReadFileNew(".\\ImgData\\problem1.txt", 0);
#else
                ReadFileNew(CommData.F_Path, 0);
#endif

//                ReadCCurveShow();
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

            gdA1.IsChecked = true;
            gdA2.IsChecked = true;
            gdA3.IsChecked = true;
            gdA4.IsChecked = true;

//            gdA5.IsChecked = true;
//            gdA6.IsChecked = true;
//            gdA7.IsChecked = true;
//            gdA8.IsChecked = true;

            chan1.IsChecked = true;
            chan2.IsChecked = true;

            if(CommData.TdIndex < 3)
            {
                chan3.IsEnabled = false;
            }
            if(CommData.TdIndex < 4)
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
                gdA1.IsEnabled = true;
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
            }

            // Todo : check if test running.
            if (CommData.currCycleState == 0 && false)
            {
                rbStop.IsEnabled = false;
                rbStop.Opacity = 0.3;
            }

            // Zhimin Ding added: Estimate cycle time

            if (CommData.experimentModelData.DebugModelDataList == null)
                return;

            TempModel Tm = new TempModel();

            Tm.SetInitTemp(72);
            Tm.SimStep(72, 3, -1, 0, 0);

            float ct = Tm.SimStep((float)CommData.experimentModelData.DebugModelDataList[0].Denaturating,
                (float)CommData.experimentModelData.DebugModelDataList[0].DenaturatingTime, -1, 2, 2);

            ct += Tm.SimStep((float)CommData.experimentModelData.DebugModelDataList[0].Annealing,
                (float)CommData.experimentModelData.DebugModelDataList[0].AnnealingTime, -1, 2, 2);

            if(CommData.experimentModelData.DebugModelDataList[0].StepCount > 2)
                ct += Tm.SimStep((float)CommData.experimentModelData.DebugModelDataList[0].Extension,
                (float)CommData.experimentModelData.DebugModelDataList[0].ExtensionTime, -1, 2, 2);

            cycle_time_estimate = ct;

            int currCycle = Convert.ToInt32(txtcurrC.Text);
            int TocalCycle = Convert.ToInt32(txtClyde.Text);
            int adder = 0;

            if (currCycle < 1)
                adder = 120 + (int)CommData.experimentModelData.DebugModelDataList[0].InitaldenaTime;

            int ToaTimeCount = adder + (int)cycle_time_estimate * (TocalCycle - currCycle + 1);

            if (CommData.currCycleState > 2 || CommData.currCycleState < 1) ToaTimeCount = 0;

            TimeSpan t = new TimeSpan(0, 0, ToaTimeCount);
            txtsysj.Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
       
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type">0历史文件数据1正在进行的数据</param>
        public void ReadFileNew(string path, int type)
        {
            try
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
                        //                   CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);

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

                        break;
                    }

                    XdcAxisRange.MaxValue = CommData.Cycle;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message);
#endif
            }

            if (CommData.currCycleState > 0)
            {
                rbStop.IsEnabled = true;
                rbStop.Opacity = 1.0;
            }

            /*              foreach (var item in CommData.diclist.Keys)
                            {
                                if (CommData.diclist[item].Count == 0) continue;

                                //    CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);

                                //=======================Zhimin: deal with bad cycle number=================

                                int m, n, c;

                                m = Convert.ToInt32(CommData.diclist[item].Count);
                                n = CommData.imgFrame;


                                if (m % n != 0) {
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
                                for (int i = 1; i
                                    <= CommData.Cycle; i++)
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

                                    string[] strs = CommData.diclist[item][k].Split(' ');
                                    factorValue[chan, i] = CommData.GetFactor(Convert.ToInt32(strs[11]));

                                    int rn = Convert.ToInt32(strs[12]);

                                    //Debug.Assert(rn == 11);

                                    if (rn != 11 && rn
                                        <12)
                                    {
#if DEBUG
                                        MessageBox.Show("File corruption detected, factor data not right position");
#endif
                                        skip += rn + 1;

                                        k = (i * 12) - 1 - skip;
                                        strs = CommData.diclist[item][k].Split(' ');
                                        factorValue[chan, i] = CommData.GetFactor(Convert.ToInt32(strs[11]));
                                    }
                                    else if (rn >12)
                                    {
                                        factorValue[chan, i] = 1;
                                        continue;
                                    }

                                    if (i == 1)
                                        factorValue[chan, 0] = factorValue[chan, i];
                                }
                           }
                        }
            */
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
            if (!CommData.diclist.Keys.Contains(chan) || CommData.diclist[chan].Count == 0)
                return;

            List<ChartData> cdlist = CommData.GetChartData(chan, 4, currks);
            if (cdlist.Count == 0)
                return;
            //LineSeries2D dxcLs1 = new LineSeries2D();
            //dxcLs1.Tag = chan + ":" + currks;
            //dxcLs1.DisplayName = chan + ":" + currks;
            //dxcLs1.MarkerVisible = false;
            //dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

            //switch (chan)
            //{
            //    case "Chip#1":
            //        dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(24, 60, 209));
            //        break;
            //    case "Chip#2":
            //        dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(83, 182, 97));
            //        break;
            //    case "Chip#3":
            //        dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(245, 195, 66));
            //        break;
            //    case "Chip#4":
            //        dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(234, 51, 35));
            //        break;
            //}
            //foreach (var item in cdlist)
            //{
            //    SeriesPoint sp = new SeriesPoint();
            //    sp.Argument = item.x.ToString();
            //    sp.Value = item.y;
            //    dxcLs1.Points.Add(sp);
            //}
            //dcXYDiagram2D.Series.Add(dxcLs1);

            int currChan = 0;
            int ksindex = -1;

            LineSeries2D dxcLs1 = new LineSeries2D();
            dxcLs1.Tag = chan + ":" + currks;
            dxcLs1.DisplayName = chan + ":" + currks;
            dxcLs1.MarkerVisible = false;
            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

            switch (chan)
            {
                case "Chip#1":
                    currChan = 0;
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(24, 60, 209));
                    break;
                case "Chip#2":
                    currChan = 1;
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(83, 182, 97));
                    break;
                case "Chip#3":
                    currChan = 2;
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(245, 195, 66));
                    break;
                case "Chip#4":
                    currChan = 3;
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



            int count = cdlist.Count;
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
                sp.Value = m_zData[currChan, ksindex, i];
                dxcLs1.Points.Add(sp);

            }
            dcXYDiagram2D.Series.Add(dxcLs1);

            radChart.Animate();
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
            dcXYDiagram2D.Series.Clear();
            foreach (var chan in ChanList)
            {
                foreach (var ks in KSList)
                {
                    DrawLine(chan, 4, ks);
                }
            }
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
            if (ChooseM != null)
            {
                ChooseM(sender, e);
            }
        }

        public void Clear()
        {
            dcXYDiagram2D.Series.Clear();
        }

      public  double TimeCount = 0;
        private void txtcurrC_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtcurrC.Text != "0")
                {
                    
                    int currCycle = Convert.ToInt32(txtcurrC.Text);
                    int TocalCycle = Convert.ToInt32(txtClyde.Text);
                    if (txtcurrC.Text == "1")
                    {
                        DateTime dt1 = Convert.ToDateTime(CommData.experimentModelData.emdatetime);
                        DateTime dt2 = DateTime.Now;
                        TimeCount = dt2.Subtract(dt1).TotalSeconds;
                    }

                    //double ToaTimeCount = TimeCount * (TocalCycle - currCycle);

                    int adder = 0;

                    if (currCycle < 1)
                        adder = 120 + (int)CommData.experimentModelData.DebugModelDataList[0].InitaldenaTime;

                    int ToaTimeCount = adder + (int)cycle_time_estimate * (TocalCycle - currCycle + 1);

                    if (CommData.currCycleState > 2 || CommData.currCycleState < 1) ToaTimeCount = 0;

                    TimeSpan t = new TimeSpan(0, 0, ToaTimeCount);
                    txtsysj.Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
                }
            }
            catch (Exception ex)
            {
                
            }
           
        }

        public void ReadCCurveShow()
        {
            m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
            CCurveShow CCurveShow = new CCurveShow();
            CCurveShow.InitData();
            List<string> kslist = new List<string>();//定义孔数

            if(CommData.KsIndex == 16)
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

            int[] cyclenum = new int[MAX_CHAN];

            for (int i = 0; i < tdlist.Count; i++)
            {
                for (int n = 0; n < kslist.Count; n++)
                {
                    List<ChartData> cdlist = CommData.GetChartData(tdlist[i], 0, kslist[n]);//获取选点值
                    for (int k = 0; k < cdlist.Count; k++)
                    {
//                      m_yData[i, n, k] = cdlist[k].y / CommData.m_factorData[GetChan(tdlist[i]), k];

//                        double factor = CommData.m_factorData[GetChan(tdlist[i]), k];

#if DEBUG_FILE
                        factor = 1;
#endif
                        // factor = factorValue[GetChan(tdlist[i]), k];

//                        if (factor < 0.001)
//                            factor = 1;                 // Zhimin added: temp solution to avoid divide by 0.

                        m_yData[i, n, k] = cdlist[k].y; // Zhimin: factor value no longer divided here. Now it is done in CommData / factor;
                    }
                }
                if (CommData.diclist.Count > 0 && CommData.diclist.ContainsKey(tdlist[i]))
                {
                    // cyclenum = Convert.ToInt32(CommData.diclist[tdlist[i]].Count / CommData.imgFrame);

                    //=============== Zhimin: deal with data corruption==============

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

                    //====================================================
                }
            }

            CCurveShow.m_yData = m_yData;
            CCurveShow.m_Size = cyclenum;
            CCurveShow.ifactor = factorValue;
            CCurveShow.UpdateAllcurve();
            m_zData = CCurveShow.m_zData;
            m_CTValue = CCurveShow.m_CTValue;
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

        private void rbStop_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}