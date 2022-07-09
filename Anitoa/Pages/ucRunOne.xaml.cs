// #define DEBUG_FILE

#define ENGLISH_VER

// #define Lumin_Lite

// #define TwoByFour

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
        public double[,] m_bData = new double[MAX_CHAN, MAX_CYCL];
        public double[, ,] m_zData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,,] m_zdData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,] m_CTValue = new double[MAX_CHAN, MAX_WELL];
        public double[,] factorValue = new double[MAX_CHAN, MAX_CYCL];

        public event EventHandler ChooseM;
//        private int index = 0;

        private List<string> ChanList = new List<string>();
        private List<string> KSList = new List<string>();

        private float cycle_time_estimate = 47;

        // public bool meltMode = false;

        public static float start_temp = 50;
        public static float end_temp = 90;

        public ucRunOne()
        {
            InitializeComponent();
            this.Loaded += ucRunOne_Loaded;
        }

        void ucRunOne_Loaded(object sender, RoutedEventArgs e)
        {
//            txtClyde.Text = CommData.Cycle.ToString();
            txtClyde.Text = CommData.experimentModelData.CyderNum.ToString();

            if(String.IsNullOrEmpty(txtClyde.Text))
            {
                MessageBox.Show("Null string found");
                txtClyde.Text = "0";
            }

            //dcAxisRange.MinValue = -100;

            //XdcAxisRange.MinValue = 0;
            //XdcAxisRange.MaxValue = CommData.Cycle;
            //dcAxisRange.MaxValue = 6000;
            radChart.Animate();

            radChart0.Animate();
            radChart1.Animate();          

            if (!string.IsNullOrEmpty(CommData.F_Path) && false)        // disabled for now
            {
#if DEBUG_FILE
                // ReadFileNew(".\\ImgData\\problem1.txt", 0);
                ReadFileNew(".\\ImgData\\sample2c.txt", 0);

#else
                //                if(CommData.deviceFound)
                // ReadFileNew(CommData.F_Path, 0);

                CommData.ReadFileData(CommData.F_Path, 0, 1, 0);


                if (CommData.currCycleState > 0)
                {
                    rbStop.IsEnabled = true;
                    rbStop.Opacity = 1.0;
                }

//                DrawLineNew();
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

            }

            // chan1.IsChecked = true;
            //            chan2.IsChecked = true;

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

            /*
                        //2019.04.03
                        if ((CommData.cboChan3 + CommData.cboChan4) == 0)
                        {
                            chan3.IsEnabled = false;
                            chan4.IsEnabled = false;

                            chan3.Opacity = 0.3;
                            chan4.Opacity = 0.3;
                        }
            */
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

//                gdA1.IsChecked = true;
//                gdA2.IsChecked = true;
//                gdA3.IsChecked = true;
//                gdA4.IsChecked = true;

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

#elif TwoByFour
#else
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

                    gdB1.IsEnabled = true;
                    gdB2.IsEnabled = true;
                    gdB3.IsEnabled = true;
                    gdB4.IsEnabled = true;

                    gdB1.Opacity = 1.0;
                    gdB2.Opacity = 1.0;
                    gdB3.Opacity = 1.0;
                    gdB4.Opacity = 1.0;
                }
                else
                {

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

                    gdA5.IsEnabled = true;
                    gdA6.IsEnabled = true;
                    gdA7.IsEnabled = true;
                    gdA8.IsEnabled = true;

                    gdA5.Opacity = 1.0;
                    gdA6.Opacity = 1.0;
                    gdA7.Opacity = 1.0;
                    gdA8.Opacity = 1.0;
                }
#endif

                /*                gdA1.IsChecked = true;
                                gdA2.IsChecked = true;
                                gdA3.IsChecked = true;
                                gdA4.IsChecked = true;

                                gdB1.IsChecked = true;
                                gdB2.IsChecked = true;
                                gdB3.IsChecked = true;
                                gdB4.IsChecked = true; */
            }
            else // 16
            {
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

            }

            /*            else
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

                            gdA1.IsChecked = true;
                            gdA2.IsChecked = true;
                            gdA3.IsChecked = true;
                            gdA4.IsChecked = true;
                            gdA5.IsChecked = true;
                            gdA6.IsChecked = true;
                            gdA7.IsChecked = true;
                            gdA8.IsChecked = true;
                            gdB1.IsChecked = true;
                            gdB2.IsChecked = true;
                            gdB3.IsChecked = true;
                            gdB4.IsChecked = true;
                            gdB5.IsChecked = true;
                            gdB6.IsChecked = true;
                            gdB7.IsChecked = true;
                            gdB8.IsChecked = true;
                        }
            */
            // Todo : check if test running.
            //            if (CommData.currCycleState == 0 && false)  // disabled for now
            if (!CommData.deviceFound)  // disabled for now
            {
                rbStop.IsEnabled = false;
                rbStop.Opacity = 0.3;
            }
            else
            {
                rbStop.IsEnabled = true;
                rbStop.Opacity = 1;
            }

            EstimateCycleTime();

            //            InitCheckStatus();          // WILL move this to only when start experiment.

            if (CommData.experimentModelData != null && 
                CommData.experimentModelData.DebugModelDataList != null && 
                CommData.experimentModelData.DebugModelDataList.Count > 0)
            {
                DebugModelData dmd = CommData.experimentModelData.DebugModelDataList[0];

                start_temp = (float)dmd.MeltStart;
                end_temp = (float)dmd.MeltEnd;
            }

            //XdcAxisRange.MinValue = stmp;
            //XdcAxisRange.MaxValue = etmp + 3;

            DrawLineNew();
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

        private void EstimateCycleTime()
        {
            // Zhimin Ding added: Estimate cycle time

            if (CommData.experimentModelData.DebugModelDataList == null)
                return;

            try
            {

                TempModel Tm = new TempModel();

                Tm.SetInitTemp(72);
                Tm.SimStep(72, 3, -1, 0, 0);

                float ct = Tm.SimStep((float)CommData.experimentModelData.DebugModelDataList[0].Denaturating,
                    (float)CommData.experimentModelData.DebugModelDataList[0].DenaturatingTime, -1, 2, 2);

                ct += Tm.SimStep((float)CommData.experimentModelData.DebugModelDataList[0].Annealing,
                    (float)CommData.experimentModelData.DebugModelDataList[0].AnnealingTime, -1, 2, 2);

                if (CommData.experimentModelData.DebugModelDataList[0].StepCount > 2)
                    ct += Tm.SimStep((float)CommData.experimentModelData.DebugModelDataList[0].Extension,
                    (float)CommData.experimentModelData.DebugModelDataList[0].ExtensionTime, -1, 2, 2);

                cycle_time_estimate = ct;

                int currCycle = CommData.currCycleNum; //Convert.ToInt32(txtcurrC.Text);
                int TocalCycle = CommData.experimentModelData.CyderNum; // Convert.ToInt32(txtClyde.Text);
                int adder = 0;

                if (currCycle < 1)
                    adder = 120 + (int)CommData.experimentModelData.DebugModelDataList[0].InitaldenaTime;

                int ToaTimeCount = adder + (int)cycle_time_estimate * (TocalCycle - currCycle + 1);

                if (CommData.currCycleState > 2 || CommData.currCycleState < 1) ToaTimeCount = 0;

                TimeSpan t = new TimeSpan(0, 0, ToaTimeCount);

                txtsysj.Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);

                /*

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
                        txtCurState.Text = "Inprogress";
                        break;
                    case 3:
                        txtCurState.Text = "Finished";
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
                        txtCurState.Text = "结束";
                        break;
                }
#endif
            */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Aha 1");
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type">0历史文件数据1正在进行的数据</param>
        public void ReadFileNewNotused(string path, int type)
        {
            /*            try
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

                            CommData.experimentModelData.ampData = CommData.diclist;

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

                            }
                        }
                        catch (Exception e)
                        {
#if DEBUG
                            MessageBox.Show(e.Message);
#endif
                        }
            */

            CommData.ReadFileData(path, type, 1, 0);

            XdcAxisRange.MaxValue = CommData.Cycle + 2;

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
            DrawLine(chan, grid.Tag.ToString());
        }

        public void DrawLine(string chan, string currks)
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
//            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetFinalState;

            LineStyle ls = new LineStyle();
            ls.Thickness = 1;
            dxcLs1.LineStyle = ls;

            // dxcLs1.CrosshairLabelPattern = "{S} {A} : {V}";
            dxcLs1.CrosshairLabelPattern = "{S} : {V}";

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
#if TwoByFour
#else
                if (CommData.well_format > 1)
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
                }
#endif
            }

#if ENGLISH_VER
            //dxcLs1.DisplayName = "Channel " + (currChan + 1).ToString() + "-" + currks;

            dxcLs1.DisplayName = currks + "(" + "Chan" + (currChan + 1).ToString() + ")";
#else
            dxcLs1.DisplayName = "通道" + (currChan + 1).ToString() + "-" + currks;
#endif

            int count = cdlist.Count;
            //dxcLs1 = new LineSeries2D();
            //dxcLs1.Tag = chan + ":" + currks;
            //dxcLs1.DisplayName = chan + ":" + currks;
            //dxcLs1.MarkerVisible = false;
            //dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            //dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(155, 9, 243));

            int cycle_shown = 0;

            if (!String.IsNullOrEmpty(txtCycles.Text))
            {

                cycle_shown = Convert.ToInt32(txtCycles.Text) + 1;


                if (count > cycle_shown)
                    count = cycle_shown;
                
            }

            for (int i = 0; i < count; i++)
            {
                double zdata = m_zData[currChan, ksindex, i];

                SeriesPoint sp = new SeriesPoint();
                sp.Argument = i.ToString();
                // sp.Value = m_zData[currChan, ksindex, i] / factorValue[currChan, i];
                // sp.Value = m_zData[currChan, ksindex, i];
                sp.Value = Math.Round(zdata, 2);
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

            if (!CommData.run1MeltMode)
            {
                XdcAxisRange.MinValue = 0;
                XdcAxisRange.MaxValue = CommData.Cycle + 2;
                ReadCCurveShow();
            }
            else {
                XdcAxisRange.MinValue = start_temp;
                XdcAxisRange.MaxValue = end_temp + 3;

                ReadCCurveShowMelt();

//                txtsysj.Text = "--";
            }

            dcXYDiagram2D.Series.Clear();

            foreach (var chan in ChanList)
            {
                foreach (var ks in KSList)
                {
                    if (!CommData.run1MeltMode)
                    {
                        DrawLine(chan, ks);
                    }
                    else
                    {
                        DrawLineMelt(chan, ks);
                    }
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
#if ENGLISH_VER
            if (MessageBox.Show("Confirm Force Stop?", "System Message", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;
#else
            if (MessageBox.Show("你确定要停止吗？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;
#endif

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
            return;
            try
            {
                if (txtcurrC.Text != "0" || CommData.currCycleState == 3)
                {

                    if(String.IsNullOrEmpty(txtClyde.Text)) {
                        txtClyde.Text = "0";
                    }

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
                            txtCurState.Text = "Cycling";
                            break;
                        case 3:
                            txtCurState.Text = "Finished";
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
                            txtCurState.Text = "结束";
                            break;
                    }
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "Aha 2");
#endif
            }
        }

        public void UpdateCurrCycleNum()
        {
            try
            {
                txtcurrC.Text = CommData.currCycleNum.ToString();

                txtsysj.Text = CommData.EstimateRemainTime();

                txtClyde.Text = CommData.CycleThisPeriod.ToString();

                if(CommData.experimentModelData.DebugModelDataList.Count > 1)
                {
                    txtcurrC.Text += " ( Period: " + (CommData.currCyclePeriodIndex + 1).ToString() + " )";
                }

            /*

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
                        txtCurState.Text = "Cycling";
                        break;
                    case 3:
                        txtCurState.Text = "Finished";
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
                        txtCurState.Text = "结束";
                        break;
                }
#endif

                */
            }

            catch (Exception ex)
            {
//#if DEBUG
                MessageBox.Show(ex.Message + "RunOne update CurrCycleNum");
//#endif
            }
        }

        public void UpdateCurrCycleNumMelt()
        {
            //txtcurrC.Text = CommData.currCycleNum.ToString();

            txtsysj.Text = CommData.EstimateRemainTimeMelt();

            //txtClyde.Text = CommData.CycleThisPeriod.ToString();
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
#if TwoByFour
#else
                if (CommData.well_format > 1)
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
                    kslist.Add("A5");
                    kslist.Add("A6");
                    kslist.Add("A7");
                    kslist.Add("A8");
                }
#endif
            }
            else
            {
                kslist.Add("A1");
                kslist.Add("A2");
                kslist.Add("A3");
                kslist.Add("A4");
            }

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

            for (int i = 0; i < tdlist.Count; i++)
            {
                List<ChartData> cdlist;
                for (int n = 0; n < kslist.Count; n++)
                {
                    cdlist = CommData.GetChartData(tdlist[i], 0, kslist[n]);//获取选点值
                    for (int k = 0; k < cdlist.Count; k++)
                    {
//                      m_yData[i, n, k] = cdlist[k].y / CommData.m_factorData[GetChan(tdlist[i]), k];

//                        double factor = CommData.m_factorData[GetChan(tdlist[i]), k];

#if DEBUG_FILE
                        // factor = 1;
#endif
                        // factor = factorValue[GetChan(tdlist[i]), k];

//                        if (factor < 0.001)
//                            factor = 1;                 // Zhimin added: temp solution to avoid divide by 0.

                        m_yData[i, n, k] = cdlist[k].y; // Zhimin: factor value no longer divided here. Now it is done in CommData / factor;
                    }
                }

                cdlist = CommData.GetChartData(tdlist[i], 0, "C0"); //dark pix
                for (int k = 0; k < cdlist.Count; k++)
                {
                    m_bData[i, k] = cdlist[k].y;        // 
                }
                /*                if (CommData.diclist.Count > 0 && CommData.diclist.ContainsKey(tdlist[i]))
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
                                */

                cyclenum[i] = CommData.GetCycleNum(tdlist[i]) + 1;          // plus 1 because the point at 0 and 1 is replicated.
            }

            int cycle_shown = 0;

            if (!String.IsNullOrEmpty(txtCycles.Text)) {

                cycle_shown = Convert.ToInt32(txtCycles.Text) + 1;

                for (int i=0; i<MAX_CHAN; i++)
                {
                    if (cyclenum[i] > cycle_shown)
                        cyclenum[i] = cycle_shown;
                }
            }


            CCurveShow.norm_top = false;
            CCurveShow.m_yData = m_yData;
            CCurveShow.m_bData = m_bData;
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

                    ls.Thickness = 1;

                    line.LineStyle = ls;

                    //                    break;
                }
            }
        }

        private void rbRefresh_Click(object sender, RoutedEventArgs e)
        {

            int cycle_shown = 0;

            if (!String.IsNullOrEmpty(txtCycles.Text))
            {

                cycle_shown = Convert.ToInt32(txtCycles.Text) + 1;

                if (cycle_shown > 50) cycle_shown = 50;

                txtCycles.Text = cycle_shown.ToString();


            }

            DrawLineNew();
        }

        public void UpdateTempCurve()
        {
            dcXYDiagram2D0.Series.Clear();
            dcXYDiagram2D0.ActualAxisX.ConstantLinesInFront.Clear();
            dcXYDiagram2D1.Series.Clear();
            dcXYDiagram2D1.ActualAxisX.ConstantLinesInFront.Clear();

            LineSeries2D dxcLs = new LineSeries2D();
            dxcLs.Tag = "PI temp";
            dxcLs.DisplayName = "PI temp";
            dxcLs.MarkerVisible = false;

            DateTime t0 = Convert.ToDateTime(CommData.program_start_time);
            DateTime t1 = DateTime.Now;

            double tick = t1.Subtract(t0).TotalSeconds;

            int count = CommData.temp_history[0].Count;       // Zhimin modified 5-5-2019. Drop the last data point because it is usually bad

            tick /= count;

            dxcAxisRange.MaxValueInternal = Convert.ToDouble(count) * tick + 1;
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
                double id = Convert.ToDouble(i) * tick;
                sp.Argument = id.ToString();

                sp.Value = Convert.ToDouble(CommData.temp_history[0].ElementAt(i));
                dxcLs.Points.Add(sp);
            }

            dcXYDiagram2D0.Series.Add(dxcLs);

            //========================

            LineSeries2D dxcLs1 = new LineSeries2D();
            dxcLs1.Tag = "PT temp";
            dxcLs1.DisplayName = "PT temp";
            dxcLs1.MarkerVisible = false;

            int count1 = CommData.temp_history[1].Count;       // Zhimin modified 5-5-2019. Drop the last data point because it is usually bad
            dxcAxisRange1.MaxValueInternal = Convert.ToDouble(count1) * tick + 1;

            if (count1 > 10)
                dxcAreaX1.GridSpacing = Convert.ToDouble(count1) * 0.1;

            //dxcLs1 = new LineSeries2D();
            //dxcLs1.Tag = chan + ":" + currks;
            //dxcLs1.DisplayName = chan + ":" + currks;
            //dxcLs1.MarkerVisible = false;
            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(243, 9, 143));

            for (int i = 0; i < count1; i++)
            {
                SeriesPoint sp = new SeriesPoint();
                double id = Convert.ToDouble(i) * tick;
                sp.Argument = id.ToString();

                sp.Value = Convert.ToDouble(CommData.temp_history[1].ElementAt(i));
                dxcLs1.Points.Add(sp);
            }

            dcXYDiagram2D1.Series.Add(dxcLs1);

            //========================

            radChart0.Animate();
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
                    txtCurState.Text = "Cycling";
                    break;
                case 3:
                    txtCurState.Text = "Cooling";
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
                        txtCurState.Text = "冷却";
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

        private void DrawLineMelt(string chan, string currks)
        {
            int currChan = 0;
            int ksindex = -1;

            LineSeries2D dxcLs1 = new LineSeries2D();
            dxcLs1.Tag = chan + ":" + currks;
            dxcLs1.DisplayName = chan + ":" + currks;
            dxcLs1.MarkerVisible = false;
            //            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

            LineStyle ls = new LineStyle();
            ls.Thickness = 1;
            dxcLs1.LineStyle = ls;

            switch (chan)
            {
                case "Chip#1":
                    currChan = 0;
                    //channame = "Chan1";
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(24, 60, 209));
                    break;
                case "Chip#2":
                    currChan = 1;
                    //channame = "Chan2";
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(83, 182, 97));
                    break;
                case "Chip#3":
                    currChan = 2;
                    //channame = "Chan3";
                    dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(245, 195, 66));
                    break;
                case "Chip#4":
                    currChan = 3;
                    //channame = "Chan4";
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
                sp.Value = m_zdData[currChan, ksindex, i];
                dxcLs1.Points.Add(sp);

            }
            dcXYDiagram2D.Series.Add(dxcLs1);

#if Melt_Crosshair

            SolidColorBrush myBrush;

            myBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));

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

            LineStyle ls = new LineStyle();
            ls.DashStyle = DashStyles.Dash;

            constantLine.LineStyle = ls;
            constantLine.Brush = myBrush; //  new SolidColorBrush(Color.FromRgb(24, 60, 209));

            dcXYDiagram2D.ActualAxisX.ConstantLinesInFront.Add(constantLine);

#endif

            //            radChart.Animate();
        }

        public void ReadCCurveShowMelt()
        {
            m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
            CCurveShowMet CCurveShowMet = new CCurveShowMet();
            CCurveShowMet.InitData();

            // CCurveShowMet.bShowRaw = cbRaw.IsChecked == true;

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
                MessageBox.Show(ex.Message + "RunOne ReadCurveShowMelt");
#endif
            }

        }
    }
}