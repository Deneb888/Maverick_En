//#define DEBUG_FILE

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
        public ucRongJQX()
        {
            InitializeComponent();
            this.Loaded += ucRunOne_Loaded;
        }

        void ucRunOne_Loaded(object sender, RoutedEventArgs e)
        {
            //txtClyde.Text = CommData.Cycle.ToString();
            //dcAxisRange.MinValue = -100;

            //XdcAxisRange.MinValue = 0;
            //XdcAxisRange.MaxValue = CommData.Cycle;
            //dcAxisRange.MaxValue = 6000;
            radChart.Animate();

            if (!string.IsNullOrEmpty(CommData.F_Path))
            {
                //ReadFileNew(CommData.F_Path, 1);
                //ReadCCurveShow();

#if DEBUG_FILE
                ReadFileNew(".\\ImgData\\MTest.txt", 0);
#else
                ReadFileNew(CommData.F_Path, 1);
#endif
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


        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type">0历史文件数据1正在进行的数据</param>
        public void ReadFileNew(string path, int type)
        {
            if (type == 2) return;
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
                        //CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);
                        //XdcAxisRange.MaxValue = CommData.Cycle;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message);
#endif
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

            Debug.Assert(true);

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
             dcXYDiagram2D.Series.Clear();
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

        public void ReadCCurveShow()
        {
            m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
            CCurveShowMet CCurveShowMet = new CCurveShowMet();
            CCurveShowMet.InitData();
            List<string> kslist = new List<string>();//定义孔数
            kslist.Add("A1");
            kslist.Add("A2");
            kslist.Add("A3");
            kslist.Add("A4");
            kslist.Add("B1");
            kslist.Add("B2");
            kslist.Add("B3");
            kslist.Add("B4");

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

            for (int i = 0; i < /*tdlist.Count*/ CommData.diclist.Count; i++)
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
                    cyclenum = Convert.ToInt32(CommData.diclist[tdlist[i]].Count / CommData.imgFrame);
                }
            }

            int[] m_Size = new int[MAX_CHAN];
            for (int n = 0; n < tdlist.Count; n++)
            {
                m_Size[n] = cyclenum;
            }

            try
            {
                CCurveShowMet.m_yData = m_yData;
                CCurveShowMet.m_Size = m_Size;
                CCurveShowMet.mtemp = mtemp;
                CCurveShowMet.ifactor = CommData.m_factorData;
                CCurveShowMet.UpdateAllcurve();
                m_zData = CCurveShowMet.m_zData;
                m_zdData = CCurveShowMet.m_zdData;
                m_CTValue = CCurveShowMet.m_CTValue;
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

            }
           


           

        }


        private void DeawLine(string chan, string currks)
        {
            int currChan = 0;
            int ksindex = -1;

            LineSeries2D dxcLs1 = new LineSeries2D();
            dxcLs1.Tag = chan + ":" + currks;
            dxcLs1.DisplayName = chan + ":" + currks;
            dxcLs1.MarkerVisible = false;
//            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

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
                sp.Value = m_zdData[currChan, ksindex, i];
                dxcLs1.Points.Add(sp);

            }
            dcXYDiagram2D.Series.Add(dxcLs1);
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
                ReadFileNewForfactor(path, 0);
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

    }
}
