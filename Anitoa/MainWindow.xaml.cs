// #define LOGIN_OK

#define ENGLISH_VER

#define INIT_HIGH_GAIN

//#define Lumin_Lite

// #define MELT_PREHOLD

#define FIND_STOP_REASON

#define DN_AUTOINT

#define DIRECT_MELT

#define POST_15000

// #define COVID_RESULT

// #define MELT_SAVE_FILE

using Anitoa.Pages;
using Microsoft.Win32;
using Newtonsoft.Json;
using Synoxo.USBHidDevice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Anitoa
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量定义
        /// <summary>
        /// HID对象
        /// </summary>
        private DeviceManagement MyDeviceManagement = new DeviceManagement();
        private static System.Timers.Timer tmrReadTimeout;
        private static System.Timers.Timer tmrReadTmp;
        private static System.Timers.Timer tmrContinuousDataCollect;
        private static System.Timers.Timer tmrReadImages;
        private static System.Timers.Timer tmrJF;
        private static System.Timers.Timer tmrRJQX;

        private ucTiaoShiOne ucTiaoShiOne = new ucTiaoShiOne();
        private ucTiaoShiTwo ucTiaoShiTwo = new ucTiaoShiTwo();
        private ucTiaoShiThree ucTiaoShiThree = new ucTiaoShiThree();
        private ucRunTwo ucRunTwo = new ucRunTwo();
        private ucRunCovid ucRunCovid = new ucRunCovid();
        private ucRunThree ucRunThree = new ucRunThree();
        private ucSettingOne ucSettingOne = new ucSettingOne();
        private ucSettingTwo ucSettingTwo = new ucSettingTwo();
        private ucRongJQX ucRongJQX = new ucRongJQX();
        private DebugModelData currDebugModelData;
        private ucRunOne ucRunOne = new ucRunOne();
        //private ucReportThree ucReportThree = new ucReportThree();
        private ucReportOne ucReportOne = new ucReportOne();

        private Protocol ucProtocol = new Protocol();

        List<DebugModelData> dmdlist = null;

        private string txtVendorID = "0x0683";
        private string txtProductID = "0x5850";

        private TrimReader TrimReader = new TrimReader();

        bool isCycleComplete = false;
        Thread ThreadImg, ThreadTemp, ThreadOther;

        //int imgFrame = 12;
        bool flag = false;

        //int cboChan1 = 1;
        //int cboChan2 = 1;
        //int cboChan3 = 1;
        //int cboChan4 = 1;
        FileStream fs = null;
        string F_Path = "";
        string F_Path_temp_PI = "";
        string F_Path_temp_PE = "";
        // 供异步线程调用显示过程信息
        private delegate void MarshalToForm(String action, String textToAdd);

        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc); //清理内存相关

        private static int MAX_CHAN = 4;
        private static int MAX_WELL = 16;
        private static int MAX_CYCL = 501;
        bool[] m_dynIntTime = new bool[MAX_CHAN];
        float[] m_factorIntTime = new float[MAX_CHAN];
        int[] m_maxPixVal = new int[MAX_CHAN];
        double[,] m_factorData = new double[MAX_CHAN, 501];             // same as max cycle for rjqx

        //float int_time1 = 1;
        //float int_time2 = 1;
        //float int_time3 = 1;
        //float int_time4 = 1;

//        float int_time_1 = 1;
//        float int_time_2 = 1;
//        float int_time_3 = 1;
//        float int_time_4 = 1;

        //    float g_pTrimDlg = 0;
        int currCycleindex = 0;
        int currCycleNum = 0;
        int xhindex = 1;

        int nextCycleIndex = 0;

        FileStream MyFS;        // Zhimin added
        StreamWriter MySW;      // 创建写入流 Zhimin added

        string dpstr;

        private static int AutoInt_Target;
        // float over_shoot = 3, under_shoot = 3, over_time = 4, under_time = 4;
        private static bool forceStopFlag = false;
        private const int AUTOINT_TARGET_AMP = 1200;
        private bool tmrThread = false;

        //private bool directMelt = false;

        private bool meltCmdSent = false;

        System.Threading.Mutex mutex;

        #endregion 变量定义

        #region 构造函数

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            var splash = new ucSplash();
            splash.Show();

            bool mutexCreated = false;
            mutex = new System.Threading.Mutex(true, @"Local\Anitoa.exe", out mutexCreated);

            //TrimReader.LoadData();
            TrimReader.ReadTrimFile();

            bool res = FindTheHid();
            bool res2 = CommData.ReadDatapositionFile();

            this.MyDeviceManagement.WhenUsbEvent += new DeviceManagement.usbEventsHandler(MyDeviceManagement_WhenUsbEvent);

            if (res == true)
            {
                ResetTrim();//下位机复位
                SetInitData();
#if ENGLISH_VER
                txtHid.Text = "HID Device Connected"; // "HID设备已连接";
#else
                txtHid.Text = "HID设备已连接";
#endif
                bd1.Visibility = Visibility.Visible;
                Startup();

                CommData.deviceFound = true;

                //Thread.Sleep(300);
                //ZDCJJFSJ();
            }
            else
            {
#if ENGLISH_VER
                txtHid.Text = "HID Device Not Found"; // "HID设备未连接";
#else
                txtHid.Text = "HID设备未连接";
#endif
                bd2.Visibility = Visibility.Visible;
            }

            if (res && !res2)
            {
                ReadFlash();
                //MessageBox.Show("从设备Flash存储器里读出Trim数据。");

#if !Lumin_Lite
                // CommData.InitWellNames();
#endif

            }

            splash.Close();

            //========Test=========

            //CommData.KsIndex = 16;
            //CommData.InitWellNames();

            //====================

            if (!mutexCreated)
            {
                if (MessageBox.Show("Another instance of this program running. This will cause problem when the hardware is connected. Exit?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes) // "要退出实验吗？", "确认退出"#else
                {
                    //some interesting behaviour here

                    mutex.Close();

                    AllowSleep();
                    this.Close();
                    Application.Current.Shutdown();
                    Environment.Exit(0);
                }
            }

        }
#endregion

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            //加载登录界面

#if LOGIN_OK
            ucLogin login = new ucLogin();
            login.LoginOK += login_LoginOK;
            gridMain.Children.Add(login);
#else

            spxzsy.Visibility = Visibility.Visible;
            //txtUser.Text = CommData.user.Uname;
            yhsetting.Visibility = rboRun.Visibility = fenxidata.Visibility = baogaodayin.Visibility = xitongtiaoshi.Visibility = Visibility.Collapsed;
            SetTSVisbile();
            //BindComboxData();

            gridMain.Children.Clear();

            ucMain ucMain = new ucMain();
            ucMain.rbChecked += ucMain_rbChecked;
            gridMain.Children.Add(ucMain);

#endif

            ucSettingOne.ChooseM += ucSettingOne_ChooseM;
            ucSettingTwo.SettingOK += ucSettingTwo_SettingOK;
            ucSettingTwo.MeltOK += ucSettingTwo_MeltOK;
            ucTiaoShiThree.SaveConfigOK += ucTiaoShiThree_SaveConfigOK;
            ucTiaoShiThree.txtCycleNum.TextChanged += txtCycleNum_TextChanged;
            ucRunOne.ChooseM += ucRunOne_ChooseM;
            ucTiaoShiTwo.SetParametersOK += ucTiaoShiTwo_SetParametersOK;
            ucTiaoShiTwo.Load_Flash += ReadEEPROM;
            ucRongJQX.ChooseM += ucRongJQX_ChooseM;
            ucReportOne.ChooseM += ucReportOne_ChooseM;
            //ucReportThree.ChooseM += ucReportThree_ChooseM;
            // ucRunCovid.ChooseM += ucRunCovid_ChooseM;

            BindTempConfig();
            ucTiaoShiOne.Start0K += ucTiaoShiOne_Start0K;
            CreateWhiteFile();//创建文件--写入图片数据

            CommData.experimentModelData.CbooChan1 = true;
            CommData.experimentModelData.CbooChan2 = false;
            CommData.experimentModelData.CbooChan3 = false;
            CommData.experimentModelData.CbooChan4 = false;

            CommData.experimentModelData.enAutoInt = true;

            //Thread initTh = new Thread(new ThreadStart(ThreadProc));
            //initTh.Start();

            /*if (initTh.IsAlive)
            {
                initTh.Abort();
            }
            */

            txtCredit.ToolTip = CommData.verInfo;

            CommData.gap_loc[0] = new List<int>();
            CommData.gap_loc[1] = new List<int>();
            CommData.gap_loc[2] = new List<int>();
            CommData.gap_loc[3] = new List<int>();
        }

        private void ThreadProc()
        {
            MessageBox.Show("初始化... 请稍等。", "系统提示");
        }

        void ucRongJQX_ChooseM(object sender, EventArgs e)
        {
            //            ResetTrim();//下位机复位
            SetInitData();
            CommData.IFMet = 1;
            RJQXModelData model = sender as RJQXModelData;
            ucProtocol.SetImgMask(MyDeviceManagement, currDebugModelData);
            Thread.Sleep(500);
            ucProtocol.SetHotTemperatureNew(MyDeviceManagement, currDebugModelData, model.HotTmp);
            Thread.Sleep(500);
            ucProtocol.SetMeltCurve(MyDeviceManagement, model.startTmp, model.endTmp);
            Thread.Sleep(500);

            forceStopFlag = false;
            tmrRJQX.Start();

        }

        void ucSettingTwo_MeltOK(object sender, EventArgs e)
        {
            if (tmrThread)
                return;

            dmdlist = sender as List<DebugModelData>;
            SetInitData();                        

            if (CommData.experimentModelData.enAutoInt && !meltCmdSent)
            {
                // xitongtiaoshi.IsChecked = true;         // Zhimin Added: switch to TiaoShiOne
                // AutoInt_Target = 3200;
                CommData.preMelt = true;
                ZDCJJFSJ();
            }
            else
            {
                CommData.IFMet = 1;                         // Zhimin movied here 12-27-2020
                StartMelt();
            }
        }

        /// <summary>
        /// 开启实验 - Melt curve
        /// </summary>
        private void StartMelt()
        {
            try
            {
                if (CheckPowerLid() > 0)
                    return;

#if MELT_SAVE_FILE
                CreateWhiteFile_New();

                if (dpstr != null && dpstr.Length > 0)
                {
                    OpenText();
                    WriteText(dpstr);
                    CloseText();
                }
#else
                if (CommData.experimentModelData.meltData != null) CommData.experimentModelData.meltData.Clear();
                CommData.experimentModelData.meltData = new Dictionary<string, List<string>>();  // 7-16-20
#endif

                xhindex = 1;

                if (!meltCmdSent)
                {
                    byte[] rxdata;

                    rxdata = ucProtocol.SetImgMask(MyDeviceManagement, currDebugModelData);
                    DebugLog("Set image mask", ref rxdata);
                    Thread.Sleep(200);

                    rxdata = ucProtocol.SetHotTemperature(MyDeviceManagement, currDebugModelData);
                    DebugLog("Set hot lid temp", ref rxdata);
                    Thread.Sleep(200);
                    currDebugModelData = dmdlist[0];

                    int stime = Convert.ToInt32(currDebugModelData.MeltStartTime);
#if MELT_PREHOLD
                    rxdata = ucProtocol.SetMeltCurve(MyDeviceManagement, currDebugModelData.MeltStart.ToString(), currDebugModelData.MeltEnd.ToString(), stime);
                    DebugLog("Issued Melt start command with pre-hold", ref rxdata);
#else
                    rxdata = ucProtocol.SetMeltCurve(MyDeviceManagement, currDebugModelData.MeltStart.ToString(), currDebugModelData.MeltEnd.ToString());
                    DebugLog("Issued Melt start command", ref rxdata);
#endif
                }
                CommData.experimentModelData.meltemdatetime = DateTime.Now;
                CommData.EstimateCycleTimeMelt();
                CommData.cycle_start_time = DateTime.Now;

                Thread.Sleep(500);

                if (tmrRJQX == null)
                {
                    Startup();
                }
                else              //  5/2//2020 Lets not clear the temperature history
                {
                    CommData.temp_history[0].Clear();
                    CommData.temp_history[1].Clear();
                }

                CommData.program_start_time = DateTime.Now;

                forceStopFlag = false;
                tmrRJQX.Start();
                tmrThread = true;
                //CommData.run1MeltMode = true;

                //if (ucRunOne != null)     //  5/2//2020 Lets not clear the temperature history
                //{
                //    ucRunOne.Clear();
                //}

                rboRun.IsChecked = true;
                // y2.IsChecked = true;

                ucSettingTwo.bdName.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /*
        private byte[] SetMeltCurve(string start, string end)
        {
            float rate = 1;
            byte[] inputdatas = new byte[16];
            byte[] OperBuf = new byte[18];
            var ftempture = (float)Convert.ToDouble(start);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];

            ftempture = (float)Convert.ToDouble(end);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[4] = hData[0];
            OperBuf[5] = hData[1];
            OperBuf[6] = hData[2];
            OperBuf[7] = hData[3];

            ftempture = (float)Convert.ToDouble(rate);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[8] = hData[0];
            OperBuf[9] = hData[1];
            OperBuf[10] = hData[2];
            OperBuf[11] = hData[3];
            byte[] TxData = new byte[20];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x0e;		//data length
            TxData[3] = 0x0b;		//data type
            TxData[4] = 0x01;		// start

            TxData[5] = OperBuf[0];
            TxData[6] = OperBuf[1];
            TxData[7] = OperBuf[2];
            TxData[8] = OperBuf[3];

            TxData[9] = OperBuf[4];
            TxData[10] = OperBuf[5];
            TxData[11] = OperBuf[6];
            TxData[12] = OperBuf[7];

            TxData[13] = OperBuf[8];
            TxData[14] = OperBuf[9];
            TxData[15] = OperBuf[10];
            TxData[16] = OperBuf[11];

            for (int i = 1; i < 17; i++)
                TxData[17] += TxData[i];
            if (TxData[17] == 0x17)
                TxData[17] = 0x18;

            TxData[18] = 0x17;
            TxData[19] = 0x17;
            string res1 = this.BytesToString(TxData, 0, TxData.Length, "0x", " ", 16);
            this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            return inputdatas;
        }
        */

        void ucTiaoShiTwo_SetParametersOK(object sender, EventArgs e)
        {
            CommData.experimentModelData.crossTalk21 = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtCrossTalk21.Text.ToString());
            CommData.experimentModelData.crossTalk12 = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtCrossTalk12.Text.ToString());

            CommData.experimentModelData.crossTalk23 = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtCrossTalk23.Text.ToString());
            CommData.experimentModelData.crossTalk32 = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtCrossTalk32.Text.ToString());

            CommData.experimentModelData.crossTalk34 = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtCrossTalk34.Text.ToString());
            CommData.experimentModelData.crossTalk43 = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtCrossTalk43.Text.ToString());

            CommData.experimentModelData.ampEffTh = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtAmpEffTh.Text.ToString());
            CommData.experimentModelData.snrTh = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtSnRTh.Text.ToString());
            CommData.experimentModelData.confiTh = 0.01 * Convert.ToDouble(ucTiaoShiTwo.txtConfiTh.Text.ToString());

            CommData.int_time_1 = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan1.Text);
            CommData.int_time_2 = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan2.Text);
            CommData.int_time_3 = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan3.Text);
            CommData.int_time_4 = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan4.Text);

            if (CommData.deviceFound && CommData.currCycleState == 0)
                SetIntergrationTimeAndGain();
        }

        private void SetTSVisbile()
        {
#if LOGIN_OK
            if (CommData.user != null)
            {
                xitongtiaoshi.Visibility = CommData.user.Utype == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
#elif !Lumin_Lite
            // xitongtiaoshi.Visibility = Visibility.Visible;       // for Lumin Lite
#endif
        }

        void txtCycleNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCycleComplete = true;
        }


        private void BindTempConfig()
        {
            try
            {
                currDebugModelData = new DebugModelData();
                //currDebugModelData.Annealing = Convert.ToDouble(ucTiaoShiThree.txtAnnealing.Text);
                //currDebugModelData.AnnealingTime = Convert.ToDouble(ucTiaoShiThree.txtAnnealingTime.Text);
                //currDebugModelData.Cycle = Convert.ToInt32(ucTiaoShiThree.txtCycle.Text);
                //currDebugModelData.Denaturating = Convert.ToDouble(ucTiaoShiThree.txtDenaturating.Text);
                //currDebugModelData.DenaturatingTime = Convert.ToDouble(ucTiaoShiThree.txtDenaturatingTime.Text);
                //currDebugModelData.Extension = Convert.ToDouble(ucTiaoShiThree.txtExtension.Text);
                //currDebugModelData.ExtensionTime = Convert.ToDouble(ucTiaoShiThree.txtExtensionTime.Text);
                //currDebugModelData.Holdon = Convert.ToDouble(ucTiaoShiThree.txtHoldon.Text);
                //currDebugModelData.HoldonTime = Convert.ToDouble(ucTiaoShiThree.txtHoldonTime.Text);
                //currDebugModelData.Hotlid = Convert.ToDouble(ucTiaoShiThree.txtHotlid.Text);
                //currDebugModelData.InitaldenaTime = Convert.ToDouble(ucTiaoShiThree.txtInitaldenaTime.Text);
                //currDebugModelData.Initaldenaturation = Convert.ToDouble(ucTiaoShiThree.txtInitaldenaturation.Text);
                currDebugModelData.stageIndex = 3;
                currDebugModelData.StepCount = 3;
                currDebugModelData.InitDenatureStepCount = 1;
                CommData.Cycle = Convert.ToInt32(currDebugModelData.Cycle);
            }
            catch (Exception ex)
            {

            }
        }

        void login_LoginOK(object sender, EventArgs e)
        {
            spxzsy.Visibility = Visibility.Visible;
            txtUser.Text = CommData.user.Uname;
            yhsetting.Visibility = rboRun.Visibility = fenxidata.Visibility = baogaodayin.Visibility = /*xitongtiaoshi.Visibility = */ Visibility.Visible;
            SetTSVisbile();
            BindComboxData();
            gridMain.Children.Clear();
            switch (sender.ToString())
            {
                case "1":
                    ucMain ucMain = new ucMain();
                    ucMain.rbChecked += ucMain_rbChecked;
                    gridMain.Children.Add(ucMain);
                    break;
            }
        }

        private void BindComboxData()
        {
            List<experimentExt> experimentList = CommData.GetExperiment();
            cboExperiment.ItemsSource = experimentList;
            cboExperiment.DisplayMemberPath = "experimentname";
            cboExperiment.SelectedValuePath = "experimentid";
        }

        void ucMain_rbChecked(object sender, EventArgs e)
        {
            spMD.Visibility = Visibility.Visible;
            gridMain.Children.Clear();
            yhsetting.Visibility = rboRun.Visibility = fenxidata.Visibility = baogaodayin.Visibility = /*xitongtiaoshi.Visibility = */ Visibility.Visible;

            switch (sender.ToString())
            {
                case "1"://用户设置
                    yhsetting.IsChecked = true;
                    //ucSettingOne ucSettingOne = new ucSettingOne();
                    //ucSettingOne.ChooseM += ucSettingOne_ChooseM;
                    //gridMain.Children.Add(ucSettingOne);
                    break;
                case "2"://分析数据
                    fenxidata.IsChecked = true;
                    //ucRunTwo ucRunTwo = new ucRunTwo();
                    //ucRunTwo.ChooseM += ucRunTwo_ChooseM;
                    //gridMain.Children.Add(ucRunTwo);
                    break;
                case "3"://报告打印
                    baogaodayin.IsChecked = true;
                    //ucReportOne ucReportOne = new ucReportOne();
                    //ucReportOne.ChooseM += ucReportOne_ChooseM;
                    //gridMain.Children.Add(ucReportOne);
                    break;
                case "4"://系统调试
                    xitongtiaoshi.IsChecked = true;
                    //ucTiaoShiMain ucTiaoShiMain = new ucTiaoShiMain();
                    //ucTiaoShiMain.rbChecked += ucTiaoShiMain_rbChecked;
                    //gridMain.Children.Add(ucTiaoShiMain);
                    break;
                case "15"://Run test
                    rboRun.IsChecked = true;
                    break;
            }
        }

        void ucTiaoShiMain_rbChecked(object sender, EventArgs e)
        {
            gridMain.Children.Clear();
            switch (sender.ToString())
            {
                case "1"://Main control
                    //ucSettingOne ucSettingOne = new ucSettingOne();
                    //ucSettingOne.ChooseM += ucSettingOne_ChooseM;
                    //gridMain.Children.Add(ucSettingOne);
                    break;
                case "2"://Imager parameters
                    //ucRunTwo ucRunTwo = new ucRunTwo();
                    //ucRunTwo.ChooseM += ucRunTwo_ChooseM;
                    //gridMain.Children.Add(ucRunTwo);
                    break;
                case "3"://Cyder parameters
                    //ucReportOne ucReportOne = new ucReportOne();
                    //ucReportOne.ChooseM += ucReportOne_ChooseM;
                    //gridMain.Children.Add(ucReportOne);
                    break;
            }
        }

        void ucSettingOne_ChooseM(object sender, EventArgs e)
        {
            string msg = sender.ToString();

            if (msg == "Next")
            {
                s2.IsChecked = true;
                gridMain.Children.Clear();
                gridMain.Children.Add(ucSettingTwo);
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucRunOne.InitCheckStatus();
                ucRunTwo.InitCheckStatus();
                ucRongJQX.InitCheckStatus();
#if COVID_RESULT              
                ucRunCovid.InitCheckStatus();
#endif
            });

            if (msg == "OpenExp")
            {
                string dlog = CommData.experimentModelData.DebugLog;
//                if (!String.IsNullOrEmpty(dlog)) {
                    DebugLogEmpty(dlog);
//                }
            }
        }

        void ucSettingTwo_ChooseM(object sender, EventArgs e)
        {
            gridMain.Children.Clear();
            ucRunOne ucRunOne = new ucRunOne();
            ucRunOne.ChooseM += ucRunOne_ChooseM;
            gridMain.Children.Add(ucRunOne);
        }

        void ucRunOne_ChooseM(object sender, EventArgs e)
        {
            //gridMain.Children.Clear();
            //ucRunTwo = new ucRunTwo();
            //ucRunTwo.ChooseM += ucRunTwo_ChooseM;
            //gridMain.Children.Add(ucRunTwo);

            // ucProtocol.CloseDev(MyDeviceManagement);

            forceStopFlag = true;

            // Thread.Sleep(100);

            /*if (tmrReadImages != null)
            {
                tmrReadImages.Dispose();
                tmrReadImages = null;
            }
            */
        }        

        void ucRunTwo_ChooseM(object sender, EventArgs e)
        {
            gridMain.Children.Clear();
            ucRunThree ucRunThree = new ucRunThree();
            ucRunThree.ChooseM += ucRunThree_ChooseM;
            gridMain.Children.Add(ucRunThree);
        }

        void ucRunThree_ChooseM(object sender, EventArgs e)
        {
            gridMain.Children.Clear();
            ucRunFour ucRunFour = new ucRunFour();
            ucRunFour.ChooseM += ucRunFour_ChooseM;
            gridMain.Children.Add(ucRunFour);
        }

        void ucRunFour_ChooseM(object sender, EventArgs e)
        {
            gridMain.Children.Clear();
            ucReportOne ucReportOne = new ucReportOne();
            ucReportOne.ChooseM += ucReportOne_ChooseM;
            gridMain.Children.Add(ucReportOne);
        }

        void ucReportOne_ChooseM(object sender, EventArgs e)
        {
            string msg = sender.ToString();

            if (msg == "1")
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {

                    //                ucRunTwo.PrintReport();
                    string img_path = "", img_path2 = "";

                    img_path = ucRunTwo.PrintReportImage();



                    bool enMelt = false;        // 

                    if (CommData.experimentModelData.DebugModelDataList != null && CommData.experimentModelData.DebugModelDataList.Count > 0)
                    {
                        DebugModelData dmd = CommData.experimentModelData.DebugModelDataList[0];
                        enMelt = dmd.enMelt;
                    }

                    if (enMelt)
                        img_path2 = ucRongJQX.PrintReportImage();

                    PrintReport(img_path, img_path2);
                });
            }
            else if(msg == "2")
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "CSV File (*.csv)|*.csv|All files|*.*";//设置文件类型
                //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
                sfd.FileName = CommData.experimentModelData.emname;
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

            /*
            gridMain.Children.Clear();
            ucReportTWO ucReportTWO = new ucReportTWO();
            ucReportTWO.ChooseM += ucReportTWO_ChooseM;
            gridMain.Children.Add(ucReportTWO);
            */
        }

        void ucReportTWO_ChooseM(object sender, EventArgs e)
        {
            gridMain.Children.Clear();
            ucReportThree ucReportThree = new ucReportThree();
            ucReportThree.ChooseM += ucReportThree_ChooseM;
            gridMain.Children.Add(ucReportThree);
        }

        void ucReportThree_ChooseM(object sender, EventArgs e)
        {            
            forceStopFlag = true;
        }

        /// <summary>
        /// 主消息发送
        /// </summary>
        /// <param name="bydedatas"></param>
        public void OperCalMainMsg(byte[] bydedatas)
        {

        }


#region 函数定义
        ///  <summary>
        ///  Perform actions that must execute when the program starts.
        ///  </summary>
        private void Startup()
        {
            try
            {
                tmrReadImages = new System.Timers.Timer(500);
                tmrReadImages.Elapsed += new ElapsedEventHandler(ReadImagesTime);
                tmrReadImages.Stop();

                tmrJF = new System.Timers.Timer(100);
                tmrJF.Elapsed += new ElapsedEventHandler(ReadSetTime);
                tmrJF.Stop();

                tmrRJQX = new System.Timers.Timer(50);
                tmrRJQX.Elapsed += new ElapsedEventHandler(ReadRJQXTmp);
                tmrRJQX.Stop();

                CommData.temp_history[0] = new List<float>();
                CommData.temp_history[1] = new List<float>();                

                //                this.MyDeviceManagement.WhenUsbEvent += new DeviceManagement.usbEventsHandler(MyDeviceManagement_WhenUsbEvent);
            }
            catch (Exception ex)
            {
                //this.richTextBox_Msg.Text += ex.Message;
            }
        }

        public void ReadOther()
        {
            GetCycldNum();
            Thread.Sleep(100);
            ReadFanState();
        }
        private void ReadImagesTime(object source, ElapsedEventArgs e)
        {
            try
            {
                //tmrContinuousDataCollect.Stop();
                //tmrReadTmp.Stop();

                tmrReadImages.Stop();
                //ReadTempNew();
                //Thread.Sleep(100);
                //GetCycldNum();
                //Thread.Sleep(100);
                //ReadFanState();
                //Thread.Sleep(100);
                //ISImgRead();
                //tmrReadImages.Start();
                ////tmrReadTmp.Start();

                // ReadPITemperature();

#if DN_AUTOINT
                bool r = false;

                if (currDebugModelData.InitaldenaTime >= 60)
                {
                    r = DnAutoint();
                }

                if (!r)
                {
                    ReadTemperatureAndStateBatchMode();
                    PreventSleep();
                }
                else
                {
                    Thread.Sleep(100);
                    tmrReadImages.Start();
                }
#else
                ReadTemperatureAndStateBatchMode();
                PreventSleep();
#endif
            }
            catch (Exception ex)
            {
                //this.richTextBox_Msg.Text += ex.Message + "\r\n";
                //throw;
                MessageBox.Show(ex.Message + "Main: ReadImagesTime");
            }
        }

        private void ClearGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                //以下系统进程没有权限，所以跳过，防止出错影响效率。  
                if ((process.ProcessName == "System") && (process.ProcessName == "Idle"))
                    continue;
                try
                {
                    EmptyWorkingSet(process.Handle);
                }
                catch
                {
                }
            }
        }


        private void ReadTemp(object source, ElapsedEventArgs e)
        {
            try
            {
                tmrReadTmp.Stop();
                ReadTempNew();
                tmrReadTmp.Start();

            }
            catch (Exception ex)
            {
                ////this.richTextBox_Msg.Text += ex.Message + "\r\n";
                //throw;
            }
        }

        public void ReadTempNew()
        {
            ReadPITemperature();
            Thread.Sleep(100);
            ReadPTTemperature();
            Thread.Sleep(100);

        }

        void MyDeviceManagement_WhenUsbEvent(object sender, USBEventArgs e)
        {
            string stat = "";
            switch (e.Status)
            {
                case USBDeviceStateEnum.Put_In:
                    stat = "--插入";

#if ENGLISH_VER
                    txtHid.Text = "HID Device Connected"; // "HID设备已连接";
#else
                    txtHid.Text = "HID设备已连接";
#endif
                    bd1.Visibility = Visibility.Visible;
                    bd2.Visibility = Visibility.Collapsed;

                    CommData.deviceFound = true;

                    break;
                case USBDeviceStateEnum.Put_Out:
                    stat = "--拔出";

#if ENGLISH_VER
                    txtHid.Text = "HID Device Not Present"; // "HID设备未连接";
#else
                    txtHid.Text = "HID设备未连接";
#endif
                    bd2.Visibility = Visibility.Visible;
                    bd1.Visibility = Visibility.Collapsed;

                    CommData.deviceFound = false;

                    break;
            }
            if (this.MyDeviceManagement.DeviceCount > 0)
            {

            }
        }

        ///  <summary>
        ///  用VID和PID查找HID设备
        ///  </summary>
        ///  <returns>
        ///   True： 找到设备
        ///  </returns>
        private Boolean FindTheHid()
        {
            int myVendorID = 0x0683;
            int myProductID = 0x5850;
            if (this.txtVendorID != null && this.txtProductID != null)
            {
                int vid = 0;
                int pid = 0;
                try
                {
                    vid = Convert.ToInt32(this.txtVendorID, 16);
                    pid = Convert.ToInt32(this.txtProductID, 16);
                    myVendorID = vid;
                    myProductID = pid;
                }
                catch (SystemException e)
                {
                    //this.richTextBox_Msg.Text += e.Message;
                }
            }
            return this.MyDeviceManagement.findHidDevices(ref myVendorID, ref myProductID);//, this);

        }

        /// <summary>
        /// 刷新设备列表
        /// </summary>
        private void RefreshDeviceList()
        {
            if (this.MyDeviceManagement.DeviceCount > 0)
            {
                //this.comboBox_DeviceList.Items.Clear();
                //for (int i = 0; i < this.MyDeviceManagement.DeviceCount; i++)
                //{
                //    this.comboBox_DeviceList.Items.Add(this.MyDeviceManagement[i].myDevicePathName.ToString());
                //}
                //this.comboBox_DeviceList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 将给定的数组转换成符合格式要求的字符串
        /// </summary>
        /// <param name="data">给定的数组</param>
        /// <param name="startIndex">起始字节序号</param>
        /// <param name="length">转换长度</param>
        /// <param name="prefix">前缀，例如 0x </param>
        /// <param name="splitString">分隔符</param>
        /// <param name="fromBase">进制10或16,其它值一律按16进制处理</param>
        /// <returns>字符串</returns>
        public string BytesToString(byte[] data, int startIndex, int length, string prefix, string splitString, int fromBase)
        {
            string _return = "";
            if (data == null)
                return _return;
            for (int i = 0; i < length; i++)
            {
                if (startIndex + i < data.Length)
                {
                    switch (fromBase)
                    {
                        case 10:
                            _return += string.Format("{0}{1:d3}", prefix, data[i + startIndex]);
                            break;
                        default:
                            _return += string.Format("{0}{1:X2}", prefix, data[i + startIndex]);
                            break;
                    }
                    if (i < data.Length - 1)
                        _return += splitString;
                }
            }
            return _return;
        }

        /// <summary>
        /// 将给定的字符串按照给定的分隔符和进制转换成字节数组
        /// </summary>
        /// <param name="str">给定的字符串</param>
        /// <param name="splitString">分隔符</param>
        /// <param name="fromBase">给定的进制</param>
        /// <returns>转换后的字节数组</returns>
        public byte[] StringToBytes(string str, string[] splitString, int fromBase)
        {
            string[] strBytes = str.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            if (strBytes == null || strBytes.Length == 0)
                return null;
            byte[] _return = new byte[strBytes.Length];
            for (int i = 0; i < strBytes.Length; i++)
            {
                try
                {
                    _return[i] = Convert.ToByte(strBytes[i], fromBase);
                }
                catch (SystemException)
                {
                    MessageBox.Show("发现不可转换的字符串->" + strBytes[i]);
                }
            }
            return _return;
        }

        ///  <summary>
        ///  定义异步线程发送Msg函数
        ///  </summary>
        ///  <param name="action"> 命令标识 ID </param>
        ///  <param name="formText"> 命令内容. </param>
        private void AccessForm(String action, String formText)
        {
            //if (this.richTextBox_Msg != null && !this.richTextBox_Msg.IsDisposed)
            //{
            //    switch (action.ToUpper())
            //    {
            //        case "CLEAR":
            //            this.richTextBox_Msg.Clear();
            //            break;
            //        case "ADDLINE":
            //            if (!formText.EndsWith("\n"))
            //                formText += "\r\n";
            //            this.richTextBox_Msg.Text += formText;
            //            break;
            //        case "ADD":
            //            this.richTextBox_Msg.Text += formText;
            //            break;
            //    }
            //}
        }

        ///  <summary>
        ///  异步线程显示过程信息函数 
        ///  </summary>
        ///  <param name="action"> 命令字符串标识 </param>
        ///  <param name="textToDisplay"> 命令内容 </param>
        private void MyMarshalToForm(String action, String textToDisplay)
        {
            object[] args = { action, textToDisplay };
            MarshalToForm MarshalToFormDelegate = null;
            //  The AccessForm routine contains the code that accesses the form.
            MarshalToFormDelegate = new MarshalToForm(AccessForm);
            //  Execute AccessForm, passing the parameters in args.
            Dispatcher.Invoke(MarshalToFormDelegate, args);
        }

        ///  <summary>
        ///  Called if the user changes the Vendor ID or Product ID in the text box.
        ///  </summary>
        private void DeviceHasChanged()
        {
            //try
            //{
            //    this.MyDeviceManagement.StopReceiveDeviceNotificationHandle();
            //    this.cmdInputReportBufferSize.Enabled = false;
            //}
            //catch (Exception ex)
            //{
            //    this.richTextBox_Msg.Text += ex.Message + "\r\n";
            //    throw;
            //}
        }


        ///  <summary>
        ///  搜集设备数据.
        ///  </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        ///  <remarks>
        ///  此计数器只有在 "连续发送" 模式下才会使能，实现连续传输
        ///  </remarks>
        private void OnDataCollect(object source, ElapsedEventArgs e)
        {
            try
            {

                //    ThreadTemp = new Thread(ReadTempNew);//读温度
                //    ThreadTemp.IsBackground = true;
                //    ThreadImg = new Thread(ISImgRead); //读图片
                //    ThreadImg.IsBackground = true;
                //    ThreadOther = new Thread(ReadOther);  //读取其它数据
                //    ThreadOther.IsBackground = true;


                //Task t1 = new Task(ReadTempNew);
                //Task t2 = t1.ContinueWith(ISImgRead);
                //Task t3 = t1.ContinueWith(ReadOther);

                tmrContinuousDataCollect.Stop();
                ReadTempNew();
                //Thread.Sleep(150);
                GetCycldNum();
                //Thread.Sleep(150);
                ReadFanState();
                //Thread.Sleep(150);
                ISImgRead();
                //Thread.Sleep(150);
                tmrContinuousDataCollect.Start();
            }
            catch (Exception ex)
            {
                ////this.richTextBox_Msg.Text += ex.Message + "\r\n";
                //throw;
            }
        }

        /// <summary>
        /// 系统超时计数器
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnReadTimeout(object source, ElapsedEventArgs e)
        {
            try
            {
                MyMarshalToForm("ADDLINE", "读取报告超时!");
                this.MyDeviceManagement.CloseCommunications();
                tmrReadTimeout.Stop();
            }
            catch (Exception ex)
            {

            }

            //  Enable requesting another transfer.
        }

        ///  <summary>
        ///  Sends an Output report, then retrieves an Input report.
        ///  Assumes report ID = 0 for both reports.
        ///  </summary>
        private byte[] ReadAndWriteToDevice()
        {
            try
            {

                byte[] inputdatas = new byte[64];
                bool success = MyDeviceManagement.ReadReportFromDevice(0, false, ref inputdatas, 1500);
                if (inputdatas[0] == 0x00)
                {
                    //tmrContinuousDataCollect.Stop();
                }
                string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {

                    TextBlock textbox = new TextBlock();
                    textbox.Text = res;
                    textbox.Width = 600;
                    textbox.ToolTip = res;

                    ucTiaoShiOne.txtImg.AppendText(res + "\r\n");
                    ucTiaoShiOne.txtImg.ScrollToEnd();
                    WhiteText(res);
                    //if (ucTiaoShiOne.txtImg.Children.Count == 12)
                    //{
                    //    tmrContinuousDataCollect.Stop();
                    //}

                });


                return inputdatas;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

#endregion 函数定义

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ReadAndWriteToDevice();
            ucProtocol.SetImgMask(MyDeviceManagement, currDebugModelData);
            ucProtocol.SetHotTemperature(MyDeviceManagement, currDebugModelData);
            ucProtocol.SetPeltier(MyDeviceManagement, currDebugModelData);

            if (currDebugModelData.InitDenatureStepCount < 2)
            {
                ucProtocol.SetCycleParameters(MyDeviceManagement, currDebugModelData, currCycleindex);
            }
            else
            {
                ucProtocol.SetCycleParameters2(MyDeviceManagement, currDebugModelData, currCycleindex);
            }
        }

        /// <summary>
        /// peltier设定，循环参数    
        /// </summary>
        /// <returns></returns>
        /*        private byte[] SetPeltier()
                {

                    byte[] inputdatas = new byte[16];

                    byte[] OperBuf = new byte[18];

                    //取Dennature编辑框中的数据
                    string stempture = currDebugModelData.Denaturating.ToString();
                    string stime = currDebugModelData.DenaturatingTime.ToString();
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    var dena_Time = Convert.ToInt32(stime);
                    byte[] denaTimes = BitConverter.GetBytes(dena_Time);
                    OperBuf[4] = denaTimes[1];
                    OperBuf[5] = denaTimes[0];

                    //取Anneal编辑框中的数据
                    stempture = currDebugModelData.Annealing.ToString();
                    stime = currDebugModelData.AnnealingTime.ToString();
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[6] = hData[0];
                    OperBuf[7] = hData[1];
                    OperBuf[8] = hData[2];
                    OperBuf[9] = hData[3];
                    var Annealing_Time = Convert.ToInt32(stime);
                    byte[] AnnealingTimes = BitConverter.GetBytes(Annealing_Time);
                    //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    OperBuf[10] = AnnealingTimes[1];
                    OperBuf[11] = AnnealingTimes[0];

                    //取Inter Extension编辑框中的数据
                    stempture = currDebugModelData.Extension.ToString();
                    stime = currDebugModelData.ExtensionTime.ToString();
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[12] = hData[0];
                    OperBuf[13] = hData[1];
                    OperBuf[14] = hData[2];
                    OperBuf[15] = hData[3];
                    var Extension_Time = Convert.ToInt32(stime);
                    byte[] ExtensionTimes = BitConverter.GetBytes(Extension_Time);
                    OperBuf[16] = ExtensionTimes[1];
                    OperBuf[17] = ExtensionTimes[0];


                    byte[] TxData = new byte[28];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x13;		//command  TXC
                    TxData[2] = 0x16;		//data length
                    TxData[3] = 0x03;		//data type, date edit first byte TXC
                    TxData[4] = 0x03;		//RegBuf[18];						
                    TxData[5] = 0x01;        //RegBuf[20];	
                    //设置拍照阶段
                    if (currDebugModelData.stageIndex == 1)
                    {
                        TxData[6] = 0x13;       // RegBuf[21];
                    }
                    if (currDebugModelData.stageIndex == 2)
                    {
                        TxData[6] = 0x23;       // RegBuf[21];
                    }
                    if (currDebugModelData.stageIndex == 3)
                    {
                        TxData[6] = 0x03;       // RegBuf[21];
                    }

                    TxData[7] = OperBuf[0];	//dennature数据
                    TxData[8] = OperBuf[1];
                    TxData[9] = OperBuf[2];
                    TxData[10] = OperBuf[3];
                    TxData[11] = OperBuf[4];
                    TxData[12] = OperBuf[5];
                    TxData[13] = OperBuf[6];//Anneal数据
                    TxData[14] = OperBuf[7];
                    TxData[15] = OperBuf[8];
                    TxData[16] = OperBuf[9];
                    TxData[17] = OperBuf[10];
                    TxData[18] = OperBuf[11];
                    TxData[19] = OperBuf[12];//Inter extension数据
                    TxData[20] = OperBuf[13];
                    TxData[21] = OperBuf[14];
                    TxData[22] = OperBuf[15];
                    TxData[23] = OperBuf[16];
                    TxData[24] = OperBuf[17];
                    for (int i = 1; i < 25; i++)
                    {
                        TxData[25] += TxData[i];
                    }

                    if (TxData[25] == 0x17)
                        TxData[25] = 0x18;
                    else
                        TxData[25] = TxData[25];
                    TxData[26] = 0x17;
                    TxData[27] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    return inputdatas;
                }
        */

        /*
                /// <summary>
                /// 一个温度参数设置
                /// </summary>
                /// <returns></returns>
                private byte[] SetPCRCyclTempTime1Seg()
                {
                    byte[] inputdatas = new byte[16];

                    byte[] OperBuf = new byte[18];

                    //取Dennature编辑框中的数据
                    string stempture = currDebugModelData.Denaturating.ToString();
                    string stime = currDebugModelData.DenaturatingTime.ToString();
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];

                    var dena_Time = Convert.ToInt32(stime);

                    byte[] denaTimes = BitConverter.GetBytes(dena_Time);
                    OperBuf[4] = denaTimes[1];
                    OperBuf[5] = denaTimes[0];


                    //var itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    //OperBuf[4] = Convert.ToByte(itime >> 8);
                    //OperBuf[5] = itime;

                    byte[] TxData = new byte[16];
                    TxData[0] = 0xaa;			//preamble code
                    TxData[1] = 0x13;			//command  TXC
                    TxData[2] = 0x10;			//data length
                    TxData[3] = 0x03;			//data type, date edit first byte TXC
                    TxData[4] = 0x03;			//					
                    TxData[5] = 0x01;			//	
                    TxData[6] = 0x02;			// 
                    TxData[7] = OperBuf[0];			//dennature
                    TxData[8] = OperBuf[1];
                    TxData[9] = OperBuf[2];
                    TxData[10] = OperBuf[3];
                    TxData[11] = OperBuf[4];
                    TxData[12] = OperBuf[5];
                    for (int i = 1; i < 13; i++)
                        TxData[13] += TxData[i];
                    if (TxData[13] == 0x17)
                        TxData[13] = 0x18;
                    //	else
                    //		TxData[19] = TxData[19];
                    TxData[14] = 0x17;
                    TxData[15] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                    return inputdatas;
                }

                /// <summary>
                /// 两个温度参数设置
                /// </summary>
                /// <returns></returns>
                private byte[] SetPCRCyclTempTime2Seg()
                {
                    byte[] inputdatas = new byte[16];

                    byte[] OperBuf = new byte[18];

                    //取Dennature编辑框中的数据
                    string stempture = currDebugModelData.Denaturating.ToString();
                    string stime = currDebugModelData.DenaturatingTime.ToString();
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    var dena_Time = Convert.ToInt32(stime);
                    byte[] denaTimes = BitConverter.GetBytes(dena_Time);
                    OperBuf[4] = denaTimes[1];
                    OperBuf[5] = denaTimes[0];

                    //取Anneal编辑框中的数据
                    stempture = currDebugModelData.Annealing.ToString();
                    stime = currDebugModelData.AnnealingTime.ToString();
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[6] = hData[0];
                    OperBuf[7] = hData[1];
                    OperBuf[8] = hData[2];
                    OperBuf[9] = hData[3];

                    var Annealing_Time = Convert.ToInt32(stime);
                    byte[] AnnealingTimes = BitConverter.GetBytes(Annealing_Time);
                    //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    OperBuf[10] = AnnealingTimes[1];
                    OperBuf[11] = AnnealingTimes[0];
                    byte[] TxData = new byte[22];
                    TxData[0] = 0xaa;			//preamble code
                    TxData[1] = 0x13;			//command  TXC
                    TxData[2] = 0x10;			//data length
                    TxData[3] = 0x03;			//data type, date edit first byte TXC
                    TxData[4] = 0x03;			//					
                    TxData[5] = 0x01;			//	
                    TxData[6] = 0x02;			// 
                    TxData[7] = OperBuf[0];			//dennature
                    TxData[8] = OperBuf[1];
                    TxData[9] = OperBuf[2];
                    TxData[10] = OperBuf[3];
                    TxData[11] = OperBuf[4];
                    TxData[12] = OperBuf[5];
                    TxData[13] = OperBuf[6];		//Anneal
                    TxData[14] = OperBuf[7];
                    TxData[15] = OperBuf[8];
                    TxData[16] = OperBuf[9];
                    TxData[17] = OperBuf[10];
                    TxData[18] = OperBuf[11];

                    for (int i = 1; i < 19; i++)
                        TxData[19] += TxData[i];
                    if (TxData[19] == 0x17)
                        TxData[19] = 0x18;
                    //	else
                    //		TxData[19] = TxData[19];
                    TxData[20] = 0x17;
                    TxData[21] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                    return inputdatas;
                }

                /// <summary>
                /// 四个温度参数设置
                /// </summary>
                /// <returns></returns>
                private byte[] SetPCRCyclTempTime4Seg()
                {
                    byte[] inputdatas = new byte[16];
                    byte[] OperBuf = new byte[18];
                    //取Dennature编辑框中的数据
                    string stempture = currDebugModelData.Denaturating.ToString();
                    string stime = currDebugModelData.DenaturatingTime.ToString();
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    var dena_Time = Convert.ToInt32(stime);
                    byte[] denaTimes = BitConverter.GetBytes(dena_Time);
                    OperBuf[4] = denaTimes[1];
                    OperBuf[5] = denaTimes[0];

                    //取Anneal编辑框中的数据
                    stempture = currDebugModelData.Annealing.ToString();
                    stime = currDebugModelData.AnnealingTime.ToString();
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[6] = hData[0];
                    OperBuf[7] = hData[1];
                    OperBuf[8] = hData[2];
                    OperBuf[9] = hData[3];
                    var Annealing_Time = Convert.ToInt32(stime);
                    byte[] AnnealingTimes = BitConverter.GetBytes(Annealing_Time);
                    //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    OperBuf[10] = AnnealingTimes[1];
                    OperBuf[11] = AnnealingTimes[0];

                    //取Inter Extension编辑框中的数据
                    stempture = currDebugModelData.Extension.ToString();
                    stime = currDebugModelData.ExtensionTime.ToString();
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[12] = hData[0];
                    OperBuf[13] = hData[1];
                    OperBuf[14] = hData[2];
                    OperBuf[15] = hData[3];
                    var Extension_Time = Convert.ToInt32(stime);
                    byte[] ExtensionTimes = BitConverter.GetBytes(Extension_Time);
                    OperBuf[16] = ExtensionTimes[1];
                    OperBuf[17] = ExtensionTimes[0];

                    //取Step4编辑框中的数据
                    stempture = currDebugModelData.Step4.ToString();
                    stime = currDebugModelData.Step4Time.ToString();
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[18] = hData[0];
                    OperBuf[19] = hData[1];
                    OperBuf[20] = hData[2];
                    OperBuf[21] = hData[3];
                    var Step4_Time = Convert.ToInt32(stime);
                    byte[] Step4Times = BitConverter.GetBytes(Step4_Time);
                    OperBuf[22] = Step4Times[1];
                    OperBuf[23] = Step4Times[0];


                    byte[] TxData = new byte[34];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x13;		//command  TXC
                    TxData[2] = 0x16;		//data length
                    TxData[3] = 0x03;		//data type, date edit first byte TXC
                    TxData[4] = 0x03;		//RegBuf[18];						
                    TxData[5] = 0x01;        //RegBuf[20];	
                    //设置拍照阶段
                    if (currDebugModelData.stageIndex == 1)
                    {
                        TxData[6] = 0x13;       // RegBuf[21];
                    }
                    if (currDebugModelData.stageIndex == 2)
                    {
                        TxData[6] = 0x23;       // RegBuf[21];
                    }
                    if (currDebugModelData.stageIndex == 3)
                    {
                        TxData[6] = 0x03;       // RegBuf[21];
                    }
                    if (currDebugModelData.stageIndex == 4)
                    {
                        TxData[6] = 0x04;       // RegBuf[21];
                    }

                    TxData[7] = OperBuf[0];	//dennature数据
                    TxData[8] = OperBuf[1];
                    TxData[9] = OperBuf[2];
                    TxData[10] = OperBuf[3];
                    TxData[11] = OperBuf[4];
                    TxData[12] = OperBuf[5];
                    TxData[13] = OperBuf[6];//Anneal数据
                    TxData[14] = OperBuf[7];
                    TxData[15] = OperBuf[8];
                    TxData[16] = OperBuf[9];
                    TxData[17] = OperBuf[10];
                    TxData[18] = OperBuf[11];
                    TxData[19] = OperBuf[12];//Inter extension数据
                    TxData[20] = OperBuf[13];
                    TxData[21] = OperBuf[14];
                    TxData[22] = OperBuf[15];
                    TxData[23] = OperBuf[16];
                    TxData[24] = OperBuf[17];
                    TxData[25] = OperBuf[18];//Step4Time数据
                    TxData[26] = OperBuf[19];
                    TxData[27] = OperBuf[20];
                    TxData[28] = OperBuf[21];
                    TxData[29] = OperBuf[22];
                    TxData[30] = OperBuf[23];
                    for (int i = 1; i < 31; i++)
                    {
                        TxData[31] += TxData[i];
                    }

                    if (TxData[31] == 0x17)
                        TxData[31] = 0x18;
                    else
                        TxData[31] = TxData[31];
                    TxData[32] = 0x17;
                    TxData[33] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);


                    return inputdatas;
                }
        */

        /*
                /// <summary>
                /// 设循环参数及启动循环
                /// </summary>
                /// <returns></returns>
                private byte[] SetCycleParameters()
                {
                    //CommData.Cycle = Convert.ToInt32(currDebugModelData.Cycle);
                    byte[] inputdatas = new byte[16];

                    byte[] OperBuf = new byte[18];

                    //取Initail denaturation编辑框中的数据
                    string stempture = currDebugModelData.Initaldenaturation.ToString();
                    string stime = "";
                    if (currCycleindex > 0)
                    {
                        stempture = currDebugModelData.Denaturating.ToString();
                        //DebugModelData newDebugModelData = dmdlist[currCycleindex - 1];
                        //if (newDebugModelData.StepCount == 1)
                        //{
                        //    stempture = newDebugModelData.Denaturating.ToString();
                        //}
                        //if (newDebugModelData.StepCount == 2)
                        //{
                        //    stempture = newDebugModelData.Annealing.ToString();
                        //}
                        //if (newDebugModelData.StepCount == 3)
                        //{
                        //    stempture = newDebugModelData.Extension.ToString();
                        //}
                        //if (newDebugModelData.StepCount == 4)
                        //{
                        //    stempture = newDebugModelData.Step4.ToString();
                        //}
                        stime = "1";
                    }
                    else
                    {
                        stime = currDebugModelData.InitaldenaTime.ToString();
                    }



                    //string stime = currDebugModelData.InitaldenaTime.ToString();
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    var dena_Time = Convert.ToInt32(stime);

                    byte[] denaTimes = BitConverter.GetBytes(dena_Time);
                    OperBuf[4] = denaTimes[1];
                    OperBuf[5] = denaTimes[0];


                    //var itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    //OperBuf[4] = Convert.ToByte(itime >> 8);
                    //OperBuf[5] = itime;


                    //取hold on 编辑框中的数据
                    stempture = currDebugModelData.Holdon.ToString();
                    stime = currDebugModelData.HoldonTime.ToString();
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[6] = hData[0];
                    OperBuf[7] = hData[1];
                    OperBuf[8] = hData[2];
                    OperBuf[9] = hData[3];

                    var HoldonTime = Convert.ToInt32(stime);
                    byte[] HoldonTimes = BitConverter.GetBytes(HoldonTime);
                    //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    OperBuf[10] = HoldonTimes[1];
                    OperBuf[11] = HoldonTimes[0];


                    //cycle编辑框值
                    var cycle = currDebugModelData.Cycle.ToString();
                    //var itime = Convert.ToByte(cycle);	//将编辑框中整型字符串转成byte
                    OperBuf[12] = Convert.ToByte(cycle);

                    byte[] TxData = new byte[22];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x13;		//command  TXC
                    TxData[2] = 0x10;		//data length
                    TxData[3] = 0x04;		//data type, date edit first byte TXC
                    TxData[4] = 0x01;		//real data, start
                    TxData[5] = OperBuf[12];	//cycle setting
                    TxData[6] = 0x00;       //
                    TxData[7] = OperBuf[0];	//inital dennature数据	
                    TxData[8] = OperBuf[1];  //
                    TxData[9] = OperBuf[2];
                    TxData[10] = OperBuf[3];
                    TxData[11] = OperBuf[4];
                    TxData[12] = OperBuf[5];
                    TxData[13] = OperBuf[6];	//extern extension数据
                    TxData[14] = OperBuf[7];
                    TxData[15] = OperBuf[8];
                    TxData[16] = OperBuf[9];
                    TxData[17] = OperBuf[10];
                    TxData[18] = OperBuf[11];
                    for (int i = 1; i < 19; i++)
                        TxData[19] += TxData[i];
                    if (TxData[19] == 0x17)
                        TxData[19] = 0x18;
                    else
                        TxData[19] = TxData[19];
                    TxData[20] = 0x17;
                    TxData[21] = 0x17;



                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);


                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    return inputdatas;
                }

                /// <summary>
                /// 设循环参数及启动循环
                /// </summary>
                /// <returns></returns>
                private byte[] SetCycleParameters2()
                {
                    byte[] inputdatas = new byte[24];
                    byte[] OperBuf = new byte[20];

                    //取Initail denaturation编辑框中的数据
                    string stempture = currDebugModelData.Initaldenaturation.ToString();
                    string stime = "";
                    if (currCycleindex > 0)         // Zhimin: Why?
                    {
                        stempture = currDebugModelData.Denaturating.ToString();
                        stime = "1";
                    }
                    else
                    {
                        stime = currDebugModelData.InitaldenaTime.ToString();
                    }


                    //string stime = currDebugModelData.InitaldenaTime.ToString();
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    var dena_Time = Convert.ToInt32(stime);

                    byte[] denaTimes = BitConverter.GetBytes(dena_Time);
                    OperBuf[4] = denaTimes[1];
                    OperBuf[5] = denaTimes[0];

                    //======================================

                    stempture = currDebugModelData.Initaldenaturation2.ToString();
                    stime = currDebugModelData.InitaldenaTime2.ToString();

                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[6] = hData[0];
                    OperBuf[7] = hData[1];
                    OperBuf[8] = hData[2];
                    OperBuf[9] = hData[3];
                    var dena_Time2 = Convert.ToInt32(stime);

                    byte[] denaTimes2 = BitConverter.GetBytes(dena_Time);
                    OperBuf[10] = denaTimes2[1];
                    OperBuf[11] = denaTimes2[0];

                    //======================================

                    //var itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    //OperBuf[4] = Convert.ToByte(itime >> 8);
                    //OperBuf[5] = itime;

                    //取hold on 编辑框中的数据
                    stempture = currDebugModelData.Holdon.ToString();
                    stime = currDebugModelData.HoldonTime.ToString();
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[12] = hData[0];
                    OperBuf[13] = hData[1];
                    OperBuf[14] = hData[2];
                    OperBuf[15] = hData[3];

                    var HoldonTime = Convert.ToInt32(stime);
                    byte[] HoldonTimes = BitConverter.GetBytes(HoldonTime);
                    //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    OperBuf[16] = HoldonTimes[1];
                    OperBuf[17] = HoldonTimes[0];


                    //cycle编辑框值
                    var cycle = currDebugModelData.Cycle.ToString();
                    //var itime = Convert.ToByte(cycle);	//将编辑框中整型字符串转成byte
                    OperBuf[18] = Convert.ToByte(cycle);

                    byte[] TxData = new byte[28];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x13;		//command  TXC
                    TxData[2] = 0x16;		//data length
                    TxData[3] = 0x04;		//data type, date edit first byte TXC
                    TxData[4] = 0x01;		//real data, start
                    TxData[5] = OperBuf[18];	//cycle setting
                    TxData[6] = 0x01;           // new CFG file
                    TxData[7] = OperBuf[0];	//inital dennature数据	
                    TxData[8] = OperBuf[1];  //
                    TxData[9] = OperBuf[2];
                    TxData[10] = OperBuf[3];
                    TxData[11] = OperBuf[4];
                    TxData[12] = OperBuf[5];
                    TxData[13] = OperBuf[6];	//inital dennature2数据
                    TxData[14] = OperBuf[7];
                    TxData[15] = OperBuf[8];
                    TxData[16] = OperBuf[9];
                    TxData[17] = OperBuf[10];
                    TxData[18] = OperBuf[11];
                    TxData[19] = OperBuf[12];	//extern extension数据
                    TxData[20] = OperBuf[13];
                    TxData[21] = OperBuf[14];
                    TxData[22] = OperBuf[15];
                    TxData[23] = OperBuf[16];
                    TxData[24] = OperBuf[17];
                    for (int i = 1; i < 25; i++)
                        TxData[25] += TxData[i];
                    if (TxData[25] == 0x17)
                        TxData[25] = 0x18;

                    TxData[26] = 0x17;
                    TxData[27] = 0x17;

                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 24);

                    return inputdatas;
                }
        */
        /*
                /// <summary>
                /// 设图像板MASK
                /// </summary>
                /// <returns></returns>
                private byte[] SetImgMask()
                {

                    byte[] inputdatas = new byte[16];
                    int PCRMask = 0;
                    int ck1 = CommData.cboChan1;
                    int ck2 = CommData.cboChan2;
                    int ck3 = CommData.cboChan3;
                    int ck4 = CommData.cboChan4;
                    PCRMask = (ck4 << 3) | (ck3 << 2) | (ck2 << 1) | ck1;

                    if (currDebugModelData != null && currDebugModelData.ifpz == 1)
                    {
                        PCRMask = 0;
                    }

                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  TXC
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x24;		//data type
                    TxData[4] = (byte)PCRMask;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;


                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res1 = this.BytesToString(TxData, 0, TxData.Length, "0x", " ", 16);

                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    return inputdatas;
                }


                /// <summary>
                /// 热盖温度设置
                /// </summary>
                /// <returns></returns>
                private byte[] SetHotTemperature()
                {

                    byte[] inputdatas = new byte[16];
                    byte[] OperBuf = new byte[18];

                    //取hot lid 编辑框中的数据
                    string stempture = currDebugModelData.Hotlid.ToString();
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] TxData = new byte[18];

                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x10;		//command
                    TxData[2] = 0x0C;		//data length
                    TxData[3] = 0x01;		//data type, date edit first byte
                    TxData[4] = 0x01;		//real data
                    TxData[5] = OperBuf[0];	//tp第一字节				
                    TxData[6] = OperBuf[1];
                    TxData[7] = OperBuf[2];
                    TxData[8] = OperBuf[3];	//tp最后一字节
                    TxData[9] = 0x00;		//time低字节
                    TxData[10] = 0x00;		//time高字节
                    TxData[11] = 0x00;		//预留位
                    TxData[12] = 0x00;
                    TxData[13] = 0x00;
                    TxData[14] = 0x00;
                    for (int i = 1; i < 15; i++)
                    {
                        TxData[15] += TxData[i];
                    }
                    if (TxData[15] == 0x17)
                        TxData[15] = 0x18;
                    else
                        TxData[15] = TxData[15];
                    TxData[16] = 0x17;		//back code
                    TxData[17] = 0x17;		//back code

                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    return inputdatas;
                }

                private byte[] SetHotTemperatureNew(string hotTmp)
                {

                    byte[] inputdatas = new byte[16];
                    byte[] OperBuf = new byte[18];

                    //取hot lid 编辑框中的数据
                    string stempture = hotTmp;
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] TxData = new byte[18];

                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x10;		//command
                    TxData[2] = 0x0C;		//data length
                    TxData[3] = 0x01;		//data type, date edit first byte
                    TxData[4] = 0x01;		//real data
                    TxData[5] = OperBuf[0];	//tp第一字节				
                    TxData[6] = OperBuf[1];
                    TxData[7] = OperBuf[2];
                    TxData[8] = OperBuf[3];	//tp最后一字节
                    TxData[9] = 0x00;		//time低字节
                    TxData[10] = 0x00;		//time高字节
                    TxData[11] = 0x00;		//预留位
                    TxData[12] = 0x00;
                    TxData[13] = 0x00;
                    TxData[14] = 0x00;
                    for (int i = 1; i < 15; i++)
                    {
                        TxData[15] += TxData[i];
                    }
                    if (TxData[15] == 0x17)
                        TxData[15] = 0x18;
                    else
                        TxData[15] = TxData[15];
                    TxData[16] = 0x17;		//back code
                    TxData[17] = 0x17;		//back code
                    string res1 = this.BytesToString(TxData, 0, TxData.Length, "0x", " ", 16);
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    return inputdatas;
                }
        */

        float pi_temp = 0;

        private byte[] ReadPITemperature()
        {
            /*    byte[] inputdatas = new byte[16];

                byte[] TxData = new byte[18];
                TxData[0] = 0xaa;		//preamble code
                TxData[1] = 0x10;		//command
                TxData[2] = 0x0C;		//data length
                TxData[3] = 0x02;		//data type, date edit first byte
                TxData[4] = 0x01;		//real data
                TxData[5] = 0x00;		//预留位
                TxData[6] = 0x00;
                TxData[7] = 0x00;
                TxData[8] = 0x00;
                TxData[9] = 0x00;
                TxData[10] = 0x00;
                TxData[11] = 0x00;
                TxData[12] = 0x00;
                TxData[13] = 0x00;
                TxData[14] = 0x00;
                for (int i = 1; i < 15; i++)
                {
                    TxData[15] += TxData[i];
                }
                if (TxData[15] == 0x17)
                    TxData[15] = 0x18;
                else
                    TxData[15] = TxData[15];
                TxData[16] = 0x17;		//back code
                TxData[17] = 0x17;		//back code

                this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);

                string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            */

            byte[] inputdatas = ucProtocol.ReadPITemperature(MyDeviceManagement);
            byte[] buffers = new byte[] { inputdatas[5], inputdatas[6], inputdatas[7], inputdatas[8] };

            float t = BitConverter.ToSingle(buffers, 0);
            WhiteTempPIText(t.ToString());
            CommData.temp_history[0].Add(t);
            pi_temp = t;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucTiaoShiOne.txtPi.AppendText(t.ToString() + "\r\n");
                ucTiaoShiOne.txtPi.ScrollToEnd();
                //ucTiaoShiThree.txtPi.AppendText(t.ToString() + "\r\n");
                //ucTiaoShiThree.txtPi.ScrollToEnd();
            });
            //Thread.Sleep(100);
            ReadPTTemperature();
            //txtwd.Text = res;
            //currtxtwd.Text = t.ToString();
            return inputdatas;
        }

        static int th_count = 0;            // temp history count
        static float elapsed_time = 0;

        private byte[] ReadPTTemperature()
        {

            /*    byte[] inputdatas = new byte[16];

                byte[] TxData = new byte[18];
                TxData[0] = 0xaa;		//preamble code
                TxData[1] = 0x10;		//command
                TxData[2] = 0x0C;		//data length
                TxData[3] = 0x02;		//data type, date edit first byte
                TxData[4] = 0x02;		//real data
                TxData[5] = 0x00;		//预留位
                TxData[6] = 0x00;
                TxData[7] = 0x00;
                TxData[8] = 0x00;
                TxData[9] = 0x00;
                TxData[10] = 0x00;
                TxData[11] = 0x00;
                TxData[12] = 0x00;
                TxData[13] = 0x00;
                TxData[14] = 0x00;
                for (int i = 1; i < 15; i++)
                {
                    TxData[15] += TxData[i];
                }
                if (TxData[15] == 0x17)
                    TxData[15] = 0x18;
                else
                    TxData[15] = TxData[15];
                TxData[16] = 0x17;		//back code
                TxData[17] = 0x17;		//back code


                this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);

                string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            */

            byte[] inputdatas = ucProtocol.ReadPTTemperature(MyDeviceManagement);
            byte[] buffers = new byte[] { inputdatas[5], inputdatas[6], inputdatas[7], inputdatas[8] };

            float t = BitConverter.ToSingle(buffers, 0);

            WhiteTempPEText(t.ToString());
            CommData.temp_history[1].Add(t);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucTiaoShiOne.txtPt.AppendText(t.ToString() + "\r\n");
                ucTiaoShiOne.txtPt.ScrollToEnd();
                //ucTiaoShiThree.txtPt.AppendText(t.ToString() + "\r\n");
                //ucTiaoShiThree.txtPt.ScrollToEnd();

                elapsed_time += 1.0f;

                TimeSpan tt = new TimeSpan(0, 0, (int)elapsed_time);

                //ucReportThree.txtPTTemp.Text = t.ToString("0.00");
                //ucReportThree.txtPITemp.Text = pi_temp.ToString("0.00");
                //ucReportThree.txtElapsedTime.Text = string.Format("{0:00}:{1:00}:{2:00}", tt.Hours, tt.Minutes, tt.Seconds);

                th_count++;

                if (th_count > 5)
                {
                    th_count = 0;
                    //ucReportThree.UpdateTempCurve();
                    ucRunOne.UpdateTempCurve();
                }
            });
            //Thread.Sleep(100);
            GetCycldNum();
            //txtwd.Text = res;
            //currtxtwd.Text = t.ToString();
            return inputdatas;
        }

        static int old_step = 0;

        private byte[] ReadTemperatureAndStateBatchMode()
        {        
            byte[] inputdatas = ucProtocol.ReadTemperatureAndStateBatchMode(MyDeviceManagement);
            byte[] lid_buffers = new byte[] { inputdatas[5], inputdatas[6], inputdatas[7], inputdatas[8] };
            byte[] pelt_buffers = new byte[] { inputdatas[9], inputdatas[10], inputdatas[11], inputdatas[12] };

            float lid_temp = BitConverter.ToSingle(lid_buffers, 0);
            float pelt_temp = BitConverter.ToSingle(pelt_buffers, 0);

            WhiteTempPIText(lid_temp.ToString());
            CommData.temp_history[0].Add(lid_temp);

            WhiteTempPEText(pelt_temp.ToString());
            CommData.temp_history[1].Add(pelt_temp);

            int header = inputdatas[0];
            int state = inputdatas[18];

            CommData.currCycleState = state;
            // currDebugModelData.cyclestate = state;

            currCycleNum = inputdatas[19] + 1;      // 1 + finished cycles

            int step = inputdatas[20] + 1;

            if (CommData.currCycleState == 2)
            {
                if (currCycleNum > CommData.currCycleNum)      // cycle number increased
                {
                    CommData.currCycleNum = currCycleNum;
                    CommData.cycle_start_time = DateTime.Now;
                    CommData.CycleThisPeriod = currDebugModelData.Cycle;

                    string str = "Enter new cycle, cycle number = " + currCycleNum.ToString();
                    DebugLog(str);                    
                }
                else if(currCycleNum == 1 && nextCycleIndex > currCycleindex && step == 1)      // detect first cycle of a multiperiod
                {
                    currCycleindex = nextCycleIndex;
                    CommData.currCyclePeriodIndex = currCycleindex;
                    currDebugModelData = dmdlist[currCycleindex];
                    // to do re-estimate cycle time too. 

                    string str = "Enter new cycle, cycle number = " + currCycleNum.ToString();
                    DebugLog(str);

                    byte[] rxdata = ucProtocol.SetImgMask(MyDeviceManagement, currDebugModelData);
                    DebugLog("Set Img Masks", ref rxdata);

                    CommData.currCycleNum = currCycleNum;
                    CommData.cycle_start_time = DateTime.Now;
                    CommData.CycleThisPeriod = currDebugModelData.Cycle;
                }

                if(old_step == 1 && step == 2)
                {
                    if (CommData.currCycleNum == currDebugModelData.Cycle)
                    {
                        LastCycleReached();
                        DebugLog("Last cycle reached");
                    }
                }

                ISImgRead();            // Only check during normal run state

                if (CommData.remainTime < -10 && CommData.currCycleNum == currDebugModelData.Cycle)
                {
                    forceStopFlag = true;
                }
            }
            else if (CommData.currCycleState == 3)
            {
                if (CommData.currCycleNum > 0)
                {
                    string str = "Cycling finished (state=" + state.ToString() + ")";
                    DebugLog(str);
                }

                CommData.currCycleNum = 0;
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                elapsed_time += 1.0f;
                TimeSpan tt = new TimeSpan(0, 0, (int)elapsed_time);

                //ucReportThree.txtPITemp.Text = lid_temp.ToString("0.00");
                //ucReportThree.txtPTTemp.Text = pelt_temp.ToString("0.00");
                //ucReportThree.txtElapsedTime.Text = string.Format("{0:00}:{1:00}:{2:00}", tt.Hours, tt.Minutes, tt.Seconds);

                th_count++;

                if (th_count > 5)
                {
                    th_count = 0;

                    //ucReportThree.UpdateTempCurve();
                    ucRunOne.UpdateTempCurve();
                    ucRunOne.UpdateCurrCycleNum();
                }

                ucTiaoShiOne.txtPi.AppendText(lid_temp.ToString() + "\r\n");
                ucTiaoShiOne.txtPi.ScrollToEnd();
                ucTiaoShiOne.txtPt.AppendText(pelt_temp.ToString() + "\r\n");
                ucTiaoShiOne.txtPt.ScrollToEnd();
                ucTiaoShiOne.txtCycleState.Text = state.ToString();
                ucTiaoShiOne.txtClycle.Text = inputdatas[19].ToString();

                //ucRunOne.txtcurrC.Text = currCycleNum.ToString();

            });
            //Thread.Sleep(100);

//            if (CommData.currCycleState == 2)             // Only check during normal run state
//            {
//                ISImgRead();
//            }

//              txtwd.Text = res;
//              currtxtwd.Text = t.ToString();

            // What if the program directly goes into Melt
            if (CommData.currCycleState == 4 || CommData.currCycleState == 5)         //
            {
                if (meltCmdSent && ucSettingTwo.iscz)
                {
                    CommData.experimentModelData.endatetime = DateTime.Now;
                    DebugLog("Trigger direct melt...");
                    tmrThread = false;
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        Thread.Sleep(1000);
                        ucSettingTwo.TriggerMelt();
                    });

                    return inputdatas;      // exiting without restart the timer.
                }
                else
                {
                    ucProtocol.CloseDev(MyDeviceManagement);
                    Thread.Sleep(300);

                    string str = "Abnormal -- enterring into Melt state, close device here -- " + state.ToString();
                    DebugLog(str);
                }

                // No need, next time state will become zero.

                /*
                if (ucSettingTwo.iscz)          // Starting melt
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        ucSettingTwo.TriggerMelt();
                    });
                }
                */
            }

            if (forceStopFlag)
            {
                byte[] rxdata = ucProtocol.CloseDev(MyDeviceManagement);
                DebugLog("Force stop -- Issued close device command", ref rxdata);

                forceStopFlag = false;
                Thread.Sleep(100);

                //CommData.experimentModelData.endatetime = DateTime.Now;
                ////                bool res = CommData.AddExperiment(CommData.experimentModelData);              // Zhimin changed. Dont save it here.

                //this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                //{
                //    string str = ucTiaoShiOne.txtdebug.Text;
                //    CommData.experimentModelData.DebugLog = str;

                //    ucRunOne.UpdateTempCurve();         // This is just to display the "Ready" state
                //    ucRunOne.UpdateCurrCycleNum();
                //});

                //CommData.expSaved = false;
                //tmrThread = false;
            }

            //============================================

            if (state == 0 && header != 0)
            {
                Thread.Sleep(100);

                string strr = "Enter ready state -- " + state.ToString();
                DebugLog(strr);

                CommData.experimentModelData.endatetime = DateTime.Now;
                //                bool res = CommData.AddExperiment(CommData.experimentModelData);              // Zhimin changed. Dont save it here.

                

                CommData.expSaved = false;
                tmrThread = false;

#if FIND_STOP_REASON

                byte[] rxdata = ucProtocol.ReadStopReason(MyDeviceManagement);
                DebugLog("Get Stop Reason", ref rxdata);

                byte ShutDownInfo = inputdatas[5];
                byte FaultLog = inputdatas[6];

                if(ShutDownInfo == 0)
                {
                    DebugLog("Normal stopping");
                }
                else
                {
                    DebugLog("Abnormal stopping" + ShutDownInfo.ToString("x") + " and " + FaultLog.ToString("x"));
                }

#endif

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    string str = ucTiaoShiOne.txtdebug.Text;
                    CommData.experimentModelData.DebugLog = str;

                    ucRunOne.UpdateTempCurve();         // This is just to display the "Ready" state
                    ucRunOne.UpdateCurrCycleNum();
                });

                if (ucSettingTwo.iscz)          // Starting melt
                {
                    Thread.Sleep(1000);         // This is kind of important. Without this, auto cal for melt tends to fail.
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        ucSettingTwo.TriggerMelt();
                    });
                }
            }

            else
            {
                Thread.Sleep(100);
                tmrReadImages.Start();
            }

            //===============

            old_step = step;

            return inputdatas;
        }

        private void LastCycleReached()
        {
            if (dmdlist.Count > 1 && currCycleindex < dmdlist.Count - 1)
            {
                nextCycleIndex = currCycleindex + 1;
                // currDebugModelData = dmdlist[currCycleindex];
                Byte[] rxdata = ucProtocol.SetPeltier2(MyDeviceManagement, dmdlist[nextCycleIndex]);
                DebugLog("Set PCR cycle temp cmd - multi_period", ref rxdata);
            }

#if DIRECT_MELT
            else if (ucSettingTwo.iscz)          // Starting melt
            {
                currDebugModelData = dmdlist[0];

                byte[] rxdata;

                rxdata = ucProtocol.SetImgMask(MyDeviceManagement, currDebugModelData);
                DebugLog("Set image mask", ref rxdata);
                Thread.Sleep(200);

                rxdata = ucProtocol.SetHotTemperature(MyDeviceManagement, currDebugModelData);
                DebugLog("Set hot lid temp", ref rxdata);
                Thread.Sleep(200);

                //                rxdata = ucProtocol.SetMeltCurve(MyDeviceManagement, currDebugModelData.MeltStart.ToString(), currDebugModelData.MeltEnd.ToString());

                int stime = Convert.ToInt32(currDebugModelData.MeltStartTime);

#if MELT_PREHOLD
                    rxdata = ucProtocol.SetMeltCurve(MyDeviceManagement, currDebugModelData.MeltStart.ToString(), currDebugModelData.MeltEnd.ToString(), stime);
                    DebugLog("Issued Melt start command with pre-hold", ref rxdata);
#else
                rxdata = ucProtocol.SetMeltCurve(MyDeviceManagement, currDebugModelData.MeltStart.ToString(), currDebugModelData.MeltEnd.ToString());
                DebugLog("Issued Melt start command", ref rxdata);
#endif
                meltCmdSent = true;
            }
#endif

        }

        public void SetIntergrationTimeAndGain()
        {
            /*
            SelSensor(1); 
            ucProtocol.SetGainMode(MyDeviceManagement);
            SelSensor(2);
            ucProtocol.SetGainMode(MyDeviceManagement);
            SelSensor(3);
            ucProtocol.SetGainMode(MyDeviceManagement);
            SelSensor(4);
            ucProtocol.SetGainMode(MyDeviceManagement);
            */

            for (int i = 0; i < MAX_CHAN; i++)
            {
                int gm = CommData.experimentModelData.gainMode[i];
                SelSensor(i + 1);
                ucProtocol.SetGainMode(MyDeviceManagement, gm);
            }

/*            if (CommData.gain_mode == 1)
            {
                SetV20(CommData.auto_v20[0, 0], 1);
                SetV20(CommData.auto_v20[1, 0], 2);
                SetV20(CommData.auto_v20[2, 0], 3);
                SetV20(CommData.auto_v20[3, 0], 4);
            }
            else
            {
                SetV20(CommData.auto_v20[0, 1], 1);
                SetV20(CommData.auto_v20[1, 1], 2);
                SetV20(CommData.auto_v20[2, 1], 3);
                SetV20(CommData.auto_v20[3, 1], 4);
            }
            */

            for (int i=0; i<MAX_CHAN; i++)
            {
                if(CommData.experimentModelData.gainMode[i] == 1)
                {
                    SetV20(CommData.auto_v20[i, 0], i + 1);
                }
                else
                {
                    SetV20(CommData.auto_v20[i, 1], i + 1);
                }
            }

            //m_factorIntTime[0] = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan1.Text);
            //m_factorIntTime[1] = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan2.Text);
            //m_factorIntTime[2] = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan3.Text);
            //m_factorIntTime[3] = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan4.Text);

            SetSensor(1, (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan1.Text));
            SetSensor(2, (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan2.Text));
            SetSensor(3, (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan3.Text));
            SetSensor(4, (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan4.Text));

            CommData.int_time_1 = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan1.Text);
            //            ucProtocol.SelSensor(MyDeviceManagement, 1);
            //            ucProtocol.SetIntergrationTime(MyDeviceManagement, int_time_1);

            CommData.int_time_2 = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan2.Text);
            //            ucProtocol.SelSensor(MyDeviceManagement, 2);
            //            ucProtocol.SetIntergrationTime(MyDeviceManagement, int_time_2);

            CommData.int_time_3 = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan3.Text);
            //            ucProtocol.SelSensor(MyDeviceManagement, 3);
            //            ucProtocol.SetIntergrationTime(MyDeviceManagement, int_time_3);

            CommData.int_time_4 = (float)Convert.ToDouble(ucTiaoShiTwo.txtITChan4.Text);
            //            ucProtocol.SelSensor(MyDeviceManagement, 4);
            //            ucProtocol.SetIntergrationTime(MyDeviceManagement, int_time_4);

            //MessageBox.Show("参数设置成功!!!");
        }

        public void SetIntergrationTimeAndGain(float int1, float int2, float int3, float int4)
        {
/*            SelSensor(1);
            ucProtocol.SetGainMode(MyDeviceManagement);
            SelSensor(2);
            ucProtocol.SetGainMode(MyDeviceManagement);
            SelSensor(3);
            ucProtocol.SetGainMode(MyDeviceManagement);
            SelSensor(4);
            ucProtocol.SetGainMode(MyDeviceManagement);
*/
            for (int i = 0; i < MAX_CHAN; i++)
            {
                int gm = CommData.experimentModelData.gainMode[i];
                SelSensor(i + 1);
                ucProtocol.SetGainMode(MyDeviceManagement, gm);
            }

            /*            if (CommData.gain_mode == 1)
                        {
                            SetV20(CommData.auto_v20[0, 0], 1);
                            SetV20(CommData.auto_v20[1, 0], 2);
                            SetV20(CommData.auto_v20[2, 0], 3);
                            SetV20(CommData.auto_v20[3, 0], 4);
                        }
                        else
                        {
                            SetV20(CommData.auto_v20[0, 1], 1);
                            SetV20(CommData.auto_v20[1, 1], 2);
                            SetV20(CommData.auto_v20[2, 1], 3);
                            SetV20(CommData.auto_v20[3, 1], 4);
                        }
            */
            for (int i = 0; i < MAX_CHAN; i++)
            {
                if (CommData.experimentModelData.gainMode[i] == 1)
                {
                    SetV20(CommData.auto_v20[i, 0], i + 1);
                }
                else
                {
                    SetV20(CommData.auto_v20[i, 1], i + 1);
                }
            }

            SetSensor(1, int1);
            SetSensor(2, int2);
            SetSensor(3, int3);
            SetSensor(4, int4);

            CommData.int_time_1 = int1;
            CommData.int_time_2 = int2;
            CommData.int_time_3 = int3;
            CommData.int_time_4 = int4;
        }

        /*        private byte[] SetGainMode()
                {
                    byte[] inputdatas = new byte[16];
                    byte[] TxData = new byte[18];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x02;		//data length
                    TxData[3] = 0x07;		//data type, date edit first byte
                    if (CommData.gain_mode == 1)//1低Gain 0高Gain
                    {
                        TxData[4] = 0x01;
                    }
                    else
                    {
                        TxData[4] = 0x00;
                    }


                    //0x01 means send vedio data
                    //0x00 means stop vedio data
                    for (int i = 1; i < 5; i++)
                    {
                        TxData[5] += TxData[i];
                    }
                    if (TxData[5] == 0x17)
                        TxData[5] = 0x18;
                    else
                        TxData[5] = TxData[5];
                    TxData[6] = 0x17;		//back code
                    TxData[7] = 0x17;		//back code
                    bool flag = this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                    return inputdatas;
                }
        */


        private void SetSensor(int c, float IntTime)
        {
            ucProtocol.SelSensor(MyDeviceManagement, c);

            byte[] rxdata = ucProtocol.SetIntergrationTime(MyDeviceManagement, IntTime);
            // DebugLog("Issued int time command-" + c.ToString() + "-" + IntTime.ToString("0.0"), ref rxdata);

            // return;         // turn off retry

            // in case of failure

            int cnt = 0;

            while(rxdata[1] != 0)
            {
                if(cnt >= 8)
                {
                    DebugLog("8 retries still failed");
                    break;
                }

                DebugLog("Command failure...retry");

                ucProtocol.SelSensor(MyDeviceManagement, c);
                rxdata = ucProtocol.SetIntergrationTime(MyDeviceManagement, IntTime);

                DebugLog("Issued int time command-" + c.ToString() + "-" + IntTime.ToString("0.0"), ref rxdata);

                cnt++;
            }
        }

        /*
            private byte[] SetIntergrationTime(float InTime)
            {
                byte[] inputdatas = new byte[16];
                byte[] OperBuf = new byte[18];
                byte[] TxData = new byte[18];

                var ftempture = InTime;
                byte[] hData = BitConverter.GetBytes(ftempture);
                OperBuf[0] = hData[0];
                OperBuf[1] = hData[1];
                OperBuf[2] = hData[2];
                OperBuf[3] = hData[3];
                TxData[0] = 0xaa;		//preamble code
                TxData[1] = 0x01;		//command
                TxData[2] = 0x05;		//data length
                TxData[3] = 0x20;		//data type, date edit first byte
                TxData[4] = OperBuf[0];		//real data, date edit second byte
                TxData[5] = OperBuf[1];
                TxData[6] = OperBuf[2];
                TxData[7] = OperBuf[3];
                //0x01 means send vedio data
                //0x00 means stop vedio data
                for (int i = 1; i < 8; i++)
                {
                    TxData[8] += TxData[i];
                }
                if (TxData[8] == 0x17)
                    TxData[8] = 0x18;
                else
                    TxData[8] = TxData[8];
                TxData[9] = 0x17;		//back code
                TxData[10] = 0x17;		//back code
                this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                //txtt.Text = res;
                return inputdatas;
            }
    */

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //ReadTemperature();
        }

        /*
                /// <summary>
                /// 关闭设备
                /// </summary>
                /// <returns></returns>
                private byte[] CloseDev()
                {

                    byte[] inputdatas = new byte[16];

                    byte[] OperBuf = new byte[18];

                    //取Initail denaturation编辑框中的数据
                    string stempture = "95";
                    string stime = "120";
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    var itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    OperBuf[4] = Convert.ToByte(itime >> 8);
                    OperBuf[5] = itime;

                    //取hold on 编辑框中的数据
                    stempture = "50";
                    stime = "10";
                    ftempture = (float)Convert.ToDouble(stempture);
                    hData = BitConverter.GetBytes(ftempture);
                    OperBuf[6] = hData[0];
                    OperBuf[7] = hData[1];
                    OperBuf[8] = hData[2];
                    OperBuf[9] = hData[3];
                    itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    OperBuf[10] = Convert.ToByte(itime >> 8);
                    OperBuf[11] = itime;


                    //cycle编辑框值
                    stime = "50";
                    itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
                    OperBuf[12] = itime;

                    byte[] TxData = new byte[22];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x13;		//command  TXC
                    TxData[2] = 0x10;		//data length
                    TxData[3] = 0x04;		//data type, date edit first byte TXC
                    TxData[4] = 0x00;		//real data, start
                    TxData[5] = OperBuf[12];	//cycle setting
                    TxData[6] = 0x00;       //
                    TxData[7] = OperBuf[0];	//inital dennature数据	
                    TxData[8] = OperBuf[1];  //
                    TxData[9] = OperBuf[2];
                    TxData[10] = OperBuf[3];
                    TxData[11] = OperBuf[4];
                    TxData[12] = OperBuf[5];
                    TxData[13] = OperBuf[6];	//extern extension数据
                    TxData[14] = OperBuf[7];
                    TxData[15] = OperBuf[8];
                    TxData[16] = OperBuf[9];
                    TxData[17] = OperBuf[10];
                    TxData[18] = OperBuf[11];
                    for (int i = 1; i < 19; i++)
                        TxData[19] += TxData[i];
                    if (TxData[19] == 0x17)
                        TxData[19] = 0x18;
                    else
                        TxData[19] = TxData[19];
                    TxData[20] = 0x17;
                    TxData[21] = 0x17;

                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    //CommData.experimentModelData.endatetime = DateTime.Now;
                    //bool flag = CommData.AddExperiment(CommData.experimentModelData);

                    return inputdatas;
                }
        */

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            /*
            ucProtocol.CloseDev(MyDeviceManagement);
            if (tmrReadImages != null)
            {
                tmrReadImages.Dispose();
                tmrReadImages = null;
            }
            */

            forceStopFlag = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //SetIntergrationTime();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ReadImg();
        }

        private byte[] ReadImg()
        {
            byte[] inputdatas = new byte[64];

            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x02;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x02;		//data type, date edit first byte
            TxData[4] = 0xff;		//real data
            TxData[5] = 0x00;		//预留位

            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code

            bool success = this.MyDeviceManagement.WriteReportToDevice(0, false, TxData);
            if (success == true)
            {
                Thread.Sleep(500);                          // Zhimin: ??? Should trace into this to see if this 500 is really necessary
                //index = 0;
                //tmrContinuousDataCollect.Start();

            }

            //this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);

            //string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            //TextBlock textbox = new TextBlock();
            //textbox.Text = res;
            //textbox.Width = 600;
            //textbox.ToolTip = res;
            //ucTiaoShiOne.txtImg.Children.Add(textbox);
            //ucTiaoShiOne.scrollsImg.ScrollToEnd();
            ////txtimg.Text = res;
            //Thread.Sleep(500);
            //tmrContinuousDataCollect.Enabled = true;
            //tmrContinuousDataCollect.Start();
            return inputdatas;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void rbMin_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            switch (rb.Name)
            {
                case "rbClose":
                    bool res = true;
                    if (!CommData.expSaved && CommData.experimentModelData.emname != null)
                    {
#if ENGLISH_VER
                        if (MessageBox.Show("Save Experiment File?", "File Save Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes) // "要存实验文件吗？", "确认存文件"
#else
                        if (MessageBox.Show("要存实验文件吗？", "确认存文件", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
#endif
                        {
#if LOGIN_OK
                            res = CommData.AddExperiment(CommData.experimentModelData);
#else
                            if (ucSettingTwo != null)
                            {
                                ucSettingTwo.SaveExp();
                            }
#endif
                        }
                    }
                    if (res)
                    {
#if ENGLISH_VER
                        if (MessageBox.Show("Exit Experiment?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes) // "要退出实验吗？", "确认退出"#else
#else
                        if (MessageBox.Show("要退出实验吗？", "确认退出", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
#endif
                        {
                            //some interesting behaviour here
                            mutex.Close();

                            AllowSleep();
                            this.Close();
                            Application.Current.Shutdown();
                            Environment.Exit(0);
                        }
                    }
                    break;
                case "rbMax":
                    this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    break;
                case "rbMin":
                    this.WindowState = WindowState.Minimized;
                    break;
            }
        }

        private void yhsetting_Click(object sender, RoutedEventArgs e)
        {
            spMD.Visibility = Visibility.Visible;
            string index = "0";
            RadioButton rb = sender as RadioButton;
            gridMain.Children.Clear();
            switch (rb.Name)
            {
                case "yhsetting":
                    index = "0";
                    break;
                case "fenxidata":
                    index = "1";
                    break;
                case "baogaodayin":
                    index = "2";
                    break;
                case "xitongtiaoshi":
                    index = "3";
                    break;
                case "rboRun":
                    index = "4";
                    //gridMain.Children.Add(ucRunOne);
                    break;
            }
            SetMdVisibity(index);
        }

        private void Image_SettingsClick(object sender, MouseButtonEventArgs e)
        {
            spMD.Visibility = Visibility.Visible;
            string index = "0";            
            gridMain.Children.Clear();

            index = "3";

            yhsetting.IsChecked = false;
            rboRun.IsChecked = false;
            fenxidata.IsChecked = false;
            baogaodayin.IsChecked = false;

            SetMdVisibity(index);
        }

        private void SetMdVisibity(string index)
        {
            int currMd = 0;
            foreach (var item in spMD.Children)
            {
                RadioButton rb = item as RadioButton;
                if (rb.Tag.ToString() == index)
                {
                    rb.Visibility = Visibility.Visible;
                    if (currMd == 0)
                    {
                        rb.IsChecked = false;
                        rb.IsChecked = true;
                    }
                    currMd = 1;
                }
                else
                {
                    rb.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            gridMain.Children.Clear();
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "s1":
                    //ucSettingOne ucSettingOne = new ucSettingOne();
                    gridMain.Children.Add(ucSettingOne);
                    break;
                case "s2":
                    //ucSettingTwo = new ucSettingTwo();

                    gridMain.Children.Add(ucSettingTwo);
                    break;
                case "s3":
                    //ucSettingTwo = new ucSettingTwo();

                    //gridMain.Children.Add(ucRongJQX);
                    break;
                case "y1":
                    gridMain.Children.Add(ucRunOne);
                    break;
                case "y2":
                    gridMain.Children.Add(ucRongJQX);
                    break;
                case "y3":
                    //                    ucReportThree ucReportThree = new ucReportThree();
                    //gridMain.Children.Add(ucReportThree);
                    break;

                case "f1":
                    //ReadFile();
                    //CommData.Cycle=currDebugModelData.Cycle
                    //ucRunTwo = new ucRunTwo();

#if COVID_RESULT                    
                    gridMain.Children.Add(ucRunCovid);
#else
                    gridMain.Children.Add(ucRunTwo);
#endif

                    break;
                case "f2":
                    //                    ucRunThree ucRunThree = new ucRunThree();
                    gridMain.Children.Add(ucRunThree);
                    break;
                case "f3":
                    //ucRunFour ucRunFour = new ucRunFour();
                    gridMain.Children.Add(ucRongJQX); //  ucRunFour);
                    break;
                case "b1":
                    //                    ucReportOne ucReportOne = new ucReportOne();
                    gridMain.Children.Add(ucReportOne);
                    break;
                case "b2":
                    ucReportTWO ucReportTWO = new ucReportTWO();
                    gridMain.Children.Add(ucReportTWO);
                    break;
                //                case "b3":
                //                    ucReportThree ucReportThree = new ucReportThree();
                //                    gridMain.Children.Add(ucReportThree);
                //                    break;
                case "t1":
                    //ucTiaoShiOne ucTiaoShiOne = new ucTiaoShiOne();
                    gridMain.Children.Add(ucTiaoShiOne);
                    break;
                case "t2":
                    //ucTiaoShiTwo ucTiaoShiTwo = new ucTiaoShiTwo();
                    gridMain.Children.Add(ucTiaoShiTwo);
                    break;
                case "t3":
                    //ucTiaoShiThree ucTiaoShiThree = new ucTiaoShiThree();
                    //ucTiaoShiThree.SaveConfigOK += ucTiaoShiThree_SaveConfigOK;
                    gridMain.Children.Add(ucTiaoShiThree);
                    break;
            }
        }

        void ucSettingTwo_SettingOK(object sender, EventArgs e)
        {
            if (tmrThread)
            {
                ucSettingTwo.bdName.Visibility = Visibility.Collapsed;
                return;
            }

            CommData.IFMet = 0;
            dmdlist = sender as List<DebugModelData>;
            //ResetTrim();//下位机复位
            currCycleindex = 0;
            CommData.currCyclePeriodIndex = 0;

            currDebugModelData = dmdlist[currCycleindex];

            nextCycleIndex = 0;

#if DN_AUTOINT

            SetInitData();

            //            if (currDebugModelData.InitaldenaTime < 60)
            //            {
            //                MessageBox.Show("Initial denature time too short (< 60)");
            //                return;
            //            }

            if (currDebugModelData.InitaldenaTime >= 60 || !CommData.experimentModelData.enAutoInt)
            {
                StartESY();
            }
            else
            {
                ZDCJJFSJ();
            }
#else

            if (CommData.experimentModelData.enAutoInt)
            {
#if !Lumin_Lite
                xitongtiaoshi.IsChecked = true;         // Zhimin Added: switch to TiaoShiOne Comment out for Lumin Lite
#endif
                ZDCJJFSJ();
            }
            else
            {
                StartESY();
            }
#endif
            ucSettingTwo.bdName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 开启实验
        /// </summary>
        private void StartESY()
        {
            try
            {
                if (CheckPowerLid() > 0) return; ;

#if DN_AUTOINT
                if (!CommData.experimentModelData.enAutoInt || currDebugModelData.InitaldenaTime < 60)    // otherwise it will be done after DnAutoInt
                {
#endif
                    //开始实验的时候重新创建新的文件
                    CreateWhiteFile();

                    if (dpstr != null && dpstr.Length > 0)
                    {
                        OpenText();
                        WriteText(dpstr);
                        CloseText();
                    }
#if DN_AUTOINT
                }
#endif

                SaveJsonData();

                //currCycleindex = 0;
                //dmdlist = sender as List<DebugModelData>;
                //currDebugModelData = dmdlist[currCycleindex];

                StartSY();
                // currCycleindex = 1;
                xhindex = 1;

                if (ucTiaoShiOne != null)
                {
                    ucTiaoShiOne.Clear();
                }
                if (ucRunOne != null)
                {
                    ucRunOne.Clear();
                }
                if (ucTiaoShiThree != null)
                {
                    ucTiaoShiThree.Clear();
                }

                rboRun.IsChecked = true;
                // y3.IsChecked = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        public void ReadFile()
        {
            try
            {
                OpenFileDialog pOpenFileDialog = new OpenFileDialog();
                pOpenFileDialog.Filter = "所有文件|*.*";//若打开指定类型的文件只需修改Filter，如打开txt文件，改为*.txt即可
                pOpenFileDialog.Multiselect = false;
                pOpenFileDialog.Title = "打开文件";
                if (pOpenFileDialog.ShowDialog() == true)
                {
                    string path = pOpenFileDialog.FileName;
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
                }
                foreach (var item in CommData.diclist.Keys)
                {
                    CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);
                    break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucTiaoShiThree_SaveConfigOK(object sender, EventArgs e)
        {
            if (sender is DebugModelData)
            {
                currDebugModelData = sender as DebugModelData;
                CommData.Cycle = currDebugModelData.Cycle;
            }
            else
            {
                switch (sender.ToString())
                {
                    case "ReadTemp":
                        //tmrReadTmp.Start();
                        break;
                    case "OverShoot":
                        if (ucTiaoShiThree != null)
                        {
                            String str = ucTiaoShiThree.txtOverShootTemp.Text;
                            CommData.experimentModelData.overTemp = Convert.ToSingle(str);
                            str = ucTiaoShiThree.txtUnderShootTemp.Text;
                            CommData.experimentModelData.underTemp = Convert.ToSingle(str);

                            str = ucTiaoShiThree.txtOverShootTime.Text;
                            CommData.experimentModelData.overTime = Convert.ToSingle(str);
                            str = ucTiaoShiThree.txtUnderShootTime.Text;
                            CommData.experimentModelData.underTime = Convert.ToSingle(str);
                        }
                        break;
                }
            }

            //SetImgMask();
            //SetHotTemperature();
            //SetPeltier();
            //SetCycleParameters();
        }

        void ucTiaoShiOne_Start0K(object sender, EventArgs e)
        {
            switch (sender.ToString())
            {
                case "rbStart":
                    //开始实验的时候重新创建新的文件
                    CreateWhiteFile();
                    StartSY();
                    break;
                case "rbReadImg":
                    ReadAllImg();
                    break;
                case "rbClose":

                    forceStopFlag = true;

                    //ucProtocol.CloseDev(MyDeviceManagement);
                    //                    if (tmrReadImages != null)
                    //                    {
                    //                        tmrReadImages.Dispose();
                    //                        tmrReadImages = null;
                    //                    }
                    break;
                case "rbhqjfsj":            // "Huo Qu Ji Fen Shi Jian"
                    ZDCJJFSJ();             // integration time
                    break;
                case "rbFindHID":
                    ReadEEPROM(sender, e);
                    break;
            }

        }

        private void StartSY()
        {
            try
            {
                if (currDebugModelData != null)
                {
//                    CommData.IFMet = 0;
                    CommData.Cycle = currDebugModelData.Cycle;
                    CommData.experimentModelData.CyderNum = CommData.Cycle;
                    CommData.experimentModelData.emdatetime = DateTime.Now;
                    // CommData.experimentModelData.endatetime = DateTime.Now;
                    CommData.cycle_start_time = DateTime.Now;
                    CommData.program_start_time = DateTime.Now;

                    CommData.EstimateCycleTime();
                    CommData.currCycleNum = 0;

                    CommData.CycleThisPeriod = currDebugModelData.Cycle;

                    //SetIntergrationTime();
                    byte[] rxdata = ucProtocol.SetChan(MyDeviceManagement);
                    DebugLog("Set channels", ref rxdata);
                    Thread.Sleep(100);
                    rxdata = ucProtocol.SetOvershootParameters(MyDeviceManagement, currDebugModelData,
                        CommData.experimentModelData.overTemp, CommData.experimentModelData.underTemp,
                        CommData.experimentModelData.overTime, CommData.experimentModelData.underTime); // over_shoot, under_shoot, over_time, under_time);
                    DebugLog("Set overshoot params", ref rxdata);
                    Thread.Sleep(100);
                    rxdata = ucProtocol.SetImgMask(MyDeviceManagement, currDebugModelData);
                    DebugLog("Set Img Masks", ref rxdata);
                    Thread.Sleep(100);
                    rxdata = ucProtocol.SetHotTemperature(MyDeviceManagement, currDebugModelData);
                    DebugLog("Set hot lid temp", ref rxdata);
                    Thread.Sleep(100);

                    switch (currDebugModelData.StepCount)
                    {
                        case 1:
                            rxdata = ucProtocol.SetPCRCyclTempTime1Seg(MyDeviceManagement, currDebugModelData);
                            DebugLog("Set PCR cycle temp cmd", ref rxdata);
                            break;
                        case 2:
                            rxdata = ucProtocol.SetPCRCyclTempTime2Seg(MyDeviceManagement, currDebugModelData);
                            DebugLog("Set PCR cycle temp cmd", ref rxdata);
                            break;
                        case 3:
                            rxdata = ucProtocol.SetPeltier(MyDeviceManagement, currDebugModelData);
                            DebugLog("Set PCR cycle temp cmd", ref rxdata);
                            break;
                        case 4:
                            rxdata = ucProtocol.SetPCRCyclTempTime4Seg(MyDeviceManagement, currDebugModelData);
                            DebugLog("Set PCR cycle temp cmd", ref rxdata);
                            break;
                    }


                    if (currDebugModelData.InitDenatureStepCount < 2)
                    {
                        if(dmdlist.Count <= 1) rxdata = ucProtocol.SetCycleParameters(MyDeviceManagement, currDebugModelData, currCycleindex);
                        else rxdata = ucProtocol.SetCycleParameters(MyDeviceManagement, currDebugModelData, currCycleindex, 0x10);
                        DebugLog("Set PCR cycle para cmd", ref rxdata);
                    }
                    else
                    {
                        if (dmdlist.Count <= 1) rxdata = ucProtocol.SetCycleParameters2(MyDeviceManagement, currDebugModelData, currCycleindex);
                        else rxdata = ucProtocol.SetCycleParameters2(MyDeviceManagement, currDebugModelData, currCycleindex, 0x11);
                        DebugLog("Set PCR cycle para cmd", ref rxdata);
                    }

                    //if (currDebugModelData != null)
                    //{
                    //    double xhtime = currDebugModelData.DenaturatingTime + currDebugModelData.AnnealingTime + currDebugModelData.ExtensionTime;
                    //    tmrReadImages = new System.Timers.Timer(xhtime);
                    //    tmrReadImages.Elapsed += new ElapsedEventHandler(ReadImagesTime);
                    //    tmrReadImages.Start();
                    //}

                    if (tmrReadImages == null)
                    {
                        Startup();
                    }

                    SetInitData();
                    CommData.temp_history[0].Clear();
                    CommData.temp_history[1].Clear();

                    elapsed_time = 0;
                    Thread.Sleep(100);

                    forceStopFlag = false;
                    tmrReadImages.Start();

                    tmrThread = true;
                    //CommData.run1MeltMode = false;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Aha + Main");
            }
        }


        //        int currStep = 0;

        private void ISImgRead()
        {
            /*    byte[] inputdatas = new byte[64];
                byte[] TxData = new byte[11];
                TxData[0] = 0xaa;		//preamble code
                TxData[1] = 0x15;		//command  TXC
                TxData[2] = 0x05;		//data length
                TxData[3] = 0x01;		//data type
                TxData[4] = 0x00;
                TxData[5] = 0x00;
                TxData[6] = 0x00;
                TxData[7] = 0x00;
                for (int i = 1; i < 8; i++)
                    TxData[8] += TxData[i];
                if (TxData[8] == 0x17)
                    TxData[8] = 0x18;
                else
                    TxData[8] = TxData[8];
                TxData[9] = 0x17;
                TxData[10] = 0x17;
                this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);
                string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            */
            byte[] inputdatas = ucProtocol.ISImgRead(MyDeviceManagement);

            string str;

            str = "Poll image ... returned:" + inputdatas[5].ToString();

            if (inputdatas[5] != 0)//   if (inputdatas[5] != 0 && isCycleComplete==true)
            {
                string m = Convert.ToString(inputdatas[5], 2);//判断chan是否存在图像
                char[] arr = m.ToCharArray();
                Array.Reverse(arr);

                if (arr.Length > 0)
                {
                    if (arr.Length >= 1 && arr[0] == Convert.ToChar("1") && CommData.cboChan1 == 1)
                    {
                        byte type = CommData.imgFrame == 12 ? (byte)0x02 : (byte)0x08;
                        ReadImgByPType(type, 1);

                    }
                    if (arr.Length >= 2 && arr[1] == Convert.ToChar("1") && CommData.cboChan2 == 1)
                    {
                        byte type = CommData.imgFrame == 12 ? (byte)0x12 : (byte)0x18;
                        ReadImgByPType(type, 2);
                    }
                    if (arr.Length >= 3 && arr[2] == Convert.ToChar("1") && CommData.cboChan3 == 1)
                    {
                        byte type = CommData.imgFrame == 12 ? (byte)0x22 : (byte)0x28;
                        ReadImgByPType(type, 3);
                    }
                    if (arr.Length >= 4 && arr[3] == Convert.ToChar("1") && CommData.cboChan4 == 1)
                    {
                        byte type = CommData.imgFrame == 12 ? (byte)0x32 : (byte)0x38;
                        ReadImgByPType(type, 4);
                    }

                    ReadFileData();

                    DebugLog("Read image data");
                }

                isCycleComplete = false;
                //if (currCycleNum == currDebugModelData.Cycle-1)
                //{
                //    if (currCycleindex < dmdlist.Count)
                //    {
                //        currDebugModelData = dmdlist[currCycleindex];
                //        StartSY();
                //        currCycleindex++;
                //    }

                //}
                //if (currDebugModelData.cyclestate == 3)//当前循环已跑完
                //{
                //    if (currCycleindex < dmdlist.Count)
                //    {
                //        currDebugModelData = dmdlist[currCycleindex];
                //        StartSY();
                //        currCycleindex++;
                //    }

                //}

            }
            //Thread.Sleep(100);
            //tmrReadImages.Start();

            // GetCycleState();          // 8-19-2019 change, no longer doing this  
        }


        public void ReadImgByPType(byte type, int chan)
        {
            string chanstr = "Chip#" + chan;

#if MELT_SAVE_FILE
            OpenText();
            WriteText("Chip#" + chan);
#else
            if (CommData.IFMet == 0)
            {
                OpenText();
                WriteText("Chip#" + chan);
            }
            else
            {
                string name = chanstr;
                if (!CommData.experimentModelData.meltData.Keys.Contains(name))
                {
                    CommData.experimentModelData.meltData[name] = new List<string>();
                }
            }
#endif

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {

                ucTiaoShiOne.txtImg.AppendText(chanstr + "\r\n");
                ucTiaoShiOne.txtImg.ScrollToEnd();
            });
            //tmrContinuousDataCollect.Stop();
            //            byte[] inputdatas = new byte[64];

            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x02;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = type;		//data type, date edit first byte
            TxData[4] = 0xff;		//real data
            TxData[5] = 0x00;		//预留位

            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code
            //this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);
            //string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            bool success = this.MyDeviceManagement.WriteReportToDevice(0, false, TxData);

            bool repeat = true;
            byte[] inputdatas = new byte[64];
            int k = 0;

            if (success == true)
            {
                //index = 0;
                //tmrContinuousDataCollect.Start();
                //for (int i = 0; i < CommData.imgFrame; i++)

                while (repeat)
                {
                    Thread.Sleep(10);
                    inputdatas = ReadImages(chan, k);
                    k++;
                    int readrow = inputdatas[5];
                    int header = inputdatas[0];

                    if (readrow + 1 == CommData.imgFrame)
                        repeat = false;

                    if (header == 0)
                        repeat = false;
                }
            }
#if MELT_SAVE_FILE
            CloseText();
#else
            if (CommData.IFMet == 0)
            {
                CloseText();
            }
#endif
        }


        public void ReadAllImg()
        {
            //tmrContinuousDataCollect.Stop();
            //Thread.Sleep(200);
            //tmrReadImages.Stop();
            if (CommData.cboChan1 == 1)
            {
                byte type = CommData.imgFrame == 12 ? (byte)0x02 : (byte)0x08;
                ReadImgByPType(type, 1);

            }
            if (CommData.cboChan2 == 1)
            {
                byte type = CommData.imgFrame == 12 ? (byte)0x12 : (byte)0x18;
                ReadImgByPType(type, 2);
            }
            if (CommData.cboChan3 == 1)
            {
                byte type = CommData.imgFrame == 12 ? (byte)0x22 : (byte)0x28;
                ReadImgByPType(type, 3);
            }
            if (CommData.cboChan4 == 1)
            {
                byte type = CommData.imgFrame == 12 ? (byte)0x32 : (byte)0x38;
                ReadImgByPType(type, 4);
            }
            //tmrReadImages.Start();
        }

        public void CreateWhiteFile_Nosave()
        {
            string timestr = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString(" hhmmss"));
            string fname = string.Format("ImgData\\ADCData_{0}.txt", timestr);
            string path = AppDomain.CurrentDomain.BaseDirectory + fname;
            if (!File.Exists(path))
            {
                try
                {
                    fs = new FileStream(fname, FileMode.Create);
                    fs.Close();
                    //fs.Dispose();
                    F_Path = path;
                    //                    CommData.F_Path = F_Path;

                    //                    CommData.experimentModelData.ImgFileName = fname;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void CreateWhiteFile()
        {
            string timestr = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString(" hhmmss"));
            string fname = string.Format("ImgData\\ADCData_{0}.txt", timestr);
            string path = AppDomain.CurrentDomain.BaseDirectory + fname;
            if (!File.Exists(path))
            {
                try
                {
                    fs = new FileStream(fname, FileMode.Create);
                    fs.Close();
                    //fs.Dispose();
                    F_Path = path;
                    CommData.F_Path = F_Path;

                    CommData.experimentModelData.ImgFileName = fname;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            fname = string.Format("ImgData\\ADCData_{0}_PITemp.txt", timestr);
            path = AppDomain.CurrentDomain.BaseDirectory + fname;
            if (!File.Exists(path))
            {
                try
                {
                    fs = new FileStream(fname, FileMode.Create);
                    fs.Close();
                    fs.Dispose();
                    F_Path_temp_PI = path;
                }
                catch (Exception ex)
                {

                }
            }

            fname = string.Format("ImgData\\ADCData_{0}_PETemp.txt", timestr);
            path = AppDomain.CurrentDomain.BaseDirectory + fname;
            if (!File.Exists(path))
            {
                try
                {
                    fs = new FileStream(fname, FileMode.Create);
                    fs.Close();
                    fs.Dispose();
                    F_Path_temp_PE = path;

                }
                catch (Exception ex)
                {

                }
            }
        }

        public void CreateWhiteFile_New()       // for Melt
        {
            string timestr = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString(" hhmmss"));
            string fname = string.Format("ImgData\\MeltData_{0}.txt", timestr);
            string path = AppDomain.CurrentDomain.BaseDirectory + fname;
            if (!File.Exists(path))
            {
                try
                {
                    fs = new FileStream(fname, FileMode.Create);
                    fs.Close();
                    //fs.Dispose();
                    F_Path = path;
                    CommData.F_Path2 = F_Path;

                    CommData.experimentModelData.ImgFileName2 = fname;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            fname = string.Format("ImgData\\ADCData_{0}_PITemp.txt", timestr);
            path = AppDomain.CurrentDomain.BaseDirectory + fname;
            if (!File.Exists(path))
            {
                try
                {
                    fs = new FileStream(fname, FileMode.Create);
                    fs.Close();
                    fs.Dispose();
                    F_Path_temp_PI = path;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "MainWindow: CreatWhiteFileNew");
                }
            }

            fname = string.Format("ImgData\\ADCData_{0}_PETemp.txt", timestr);
            path = AppDomain.CurrentDomain.BaseDirectory + fname;
            if (!File.Exists(path))
            {
                try
                {
                    fs = new FileStream(fname, FileMode.Create);
                    fs.Close();
                    fs.Dispose();
                    F_Path_temp_PE = path;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "MainWindow: CreatWhiteFileNew");
                }
            }
        }

        private void WhiteText(string text)
        {

            //FileStream ofs = new FileStream(F_Path, FileMode.Open);
            try
            {
                FileStream _file = new FileStream(F_Path, System.IO.FileMode.Append, System.IO.FileAccess.Write, FileShare.Write);
                //FileStream _file = new FileStream(F_Path, FileMode.Create, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(_file); // 创建写入流
                sw.WriteLine(text); // 写入 text
                sw.Flush();
                sw.Close();
                _file.Close();
                _file.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "WhiteText main");
            }

            //ofs.Close();
        }

        private void OpenText()
        {
            try
            {
                MyFS = new FileStream(F_Path, System.IO.FileMode.Append, System.IO.FileAccess.Write, FileShare.Write);
                MySW = new StreamWriter(MyFS); // 创建写入流
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Open Text main");
            }
        }

        private void WriteText(string text)
        {
            try
            {
                if (MySW != null && MySW.BaseStream != null)
                {
                    MySW.WriteLine(text); // 
                }
                //MySW.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "WriteText main");
            }
        }

        private void CloseText()
        {
            try
            {
                MySW.Close();
                MyFS.Close();
                MyFS.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "CloseText main");
            }
        }

        private void WhiteTempPIText(string text)
        {
            return;     // Zhimin: forget about it for now

            //FileStream ofs = new FileStream(F_Path, FileMode.Open);
            try
            {
                FileStream _file = new FileStream(F_Path_temp_PI, System.IO.FileMode.Append, System.IO.FileAccess.Write, FileShare.Write);
                //FileStream _file = new FileStream(F_Path, FileMode.Create, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(_file); // 创建写入流
                sw.WriteLine(text); // 写入Hello World
                sw.Flush();
                sw.Close();
                _file.Close();
            }
            catch (Exception ex)
            {

            }



            //ofs.Close();
        }

        private void WhiteTempPEText(string text)
        {
            return;     // Zhimin: forget about it for now

            //FileStream ofs = new FileStream(F_Path, FileMode.Open);
            try
            {
                FileStream _file = new FileStream(F_Path_temp_PE, System.IO.FileMode.Append, System.IO.FileAccess.Write, FileShare.Write);
                //FileStream _file = new FileStream(F_Path, FileMode.Create, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(_file); // 创建写入流
                sw.WriteLine(text); // 写入Hello World
                sw.Flush();
                sw.Close();
                _file.Close();
            }
            catch (Exception ex)
            {

            }



            //ofs.Close();
        }


        /*
                /// <summary>
                /// 查询温度板循环状态
                /// </summary>
                public byte[] ReadTempCycleState()
                {

                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[9];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x14;		//command
                    TxData[2] = 0x0C;		//data length
                    TxData[3] = 0x15;		//data type, date edit first byte
                    TxData[4] = 0x00;		//real data
                    TxData[5] = 0x00;		//预留位

                    for (int i = 1; i < 6; i++)
                        TxData[6] += TxData[i];
                    if (TxData[6] == 0x17)
                        TxData[6] = 0x18;
                    else
                        TxData[6] = TxData[6];
                    TxData[7] = 0x17;		//back code
                    TxData[8] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    if (inputdatas[5] == 0)//0当前没有跑循环
                    {

                    }


                    return inputdatas;
                }
        */
        /// <summary>
        /// 读取风扇状态
        /// </summary>
        public byte[] ReadFanState()
        {

            /*              byte[] inputdatas = new byte[64];
                                  byte[] TxData = new byte[9];
                                    TxData[0] = 0xaa;		//preamble code
                                    TxData[1] = 0x10;		//command
                                    TxData[2] = 0x0C;		//data length
                                    TxData[3] = 0x0A;		//data type, date edit first byte
                                    TxData[4] = 0x01;		//real data
                                    TxData[5] = 0x00;		//预留位

                                    for (int i = 1; i < 6; i++)
                                        TxData[6] += TxData[i];
                                    if (TxData[6] == 0x17)
                                        TxData[6] = 0x18;
                                    else
                                        TxData[6] = TxData[6];
                                    TxData[7] = 0x17;		//back code
                                    TxData[8] = 0x17;		//back code
                                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);
                                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            */

            byte[] inputdatas = ucProtocol.ReadFanState(MyDeviceManagement);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucTiaoShiThree.txtFanState.Text = inputdatas[5].ToString();

            });
            //Thread.Sleep(100);
            ISImgRead();
            GetCycleState();    // Added 8-19-2019.

            //ReadAllImg();
            return inputdatas;
        }

        /*        /// <summary>
                /// 停止风扇
                /// </summary>
                public byte[] StopFan()
                {

                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[9];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x10;		//command
                    TxData[2] = 0x0C;		//data length
                    TxData[3] = 0x0A;		//data type, date edit first byte
                    TxData[4] = 0x01;		//real data
                    TxData[5] = 0x00;		//预留位

                    for (int i = 1; i < 6; i++)
                        TxData[6] += TxData[i];
                    if (TxData[6] == 0x17)
                        TxData[6] = 0x18;
                    else
                        TxData[6] = TxData[6];
                    TxData[7] = 0x17;		//back code
                    TxData[8] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    if (inputdatas[5] == 0)//0当前没有跑循环
                    {

                    }


                    return inputdatas;
                }
        */
        /*
                /// <summary>
                /// 设置chan
                /// </summary>
                public byte[] SetChan()
                {

                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[9];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x0C;		//data length
                    TxData[3] = 0x23;		//data type, date edit first byte
                    TxData[4] = 0x80;		//real data
                    TxData[5] = 0x00;		//预留位

                    for (int i = 1; i < 6; i++)
                        TxData[6] += TxData[i];
                    if (TxData[6] == 0x17)
                        TxData[6] = 0x18;
                    else
                        TxData[6] = TxData[6];
                    TxData[7] = 0x17;		//back code
                    TxData[8] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

                    return inputdatas;
                }
        */

        /// <summary>
        /// 读出当前循环数和阶段数
        /// </summary>
        public byte[] GetCycldNum()
        {

            /*          byte[] inputdatas = new byte[64];
                        byte[] TxData = new byte[9];
                        TxData[0] = 0xaa;		//preamble code
                        TxData[1] = 0x14;		//command
                        TxData[2] = 0x0C;		//data length
                        TxData[3] = 0x01;		//data type, date edit first byte
                        TxData[4] = 0x00;		//real data
                        TxData[5] = 0x00;		//预留位

                        for (int i = 1; i < 6; i++)
                            TxData[6] += TxData[i];
                        if (TxData[6] == 0x17)
                            TxData[6] = 0x18;
                        else
                            TxData[6] = TxData[6];
                        TxData[7] = 0x17;		//back code
                        TxData[8] = 0x17;		//back code
                        this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);
                        string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                        */

            byte[] inputdatas = ucProtocol.GetCycldNum(MyDeviceManagement);
            currCycleNum = inputdatas[5];

            if (CommData.currCycleState == 2)
            {
                if (currCycleNum < CommData.experimentModelData.CyderNum)
                {
                    currCycleNum = inputdatas[5] + 1;
                }
                else
                {
                    currCycleNum = 0;
                }
            }
            else if (CommData.currCycleState == 3)
            {
                currCycleNum = 0;
            }

            CommData.currCycleNum = currCycleNum;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucTiaoShiThree.txtCycleNum.Text = inputdatas[5].ToString();

                ucTiaoShiOne.txtClycle.Text = inputdatas[5].ToString();

                //                if (CommData.currCycleState <= 2)
                ucRunOne.txtcurrC.Text = currCycleNum.ToString();
                //                else
                //                    ucRunOne.txtcurrC.Text = currCycleNum.ToString("0.0");

                /*
                                switch (CommData.currCycleState)
                                {
                                    case 0:
                                        ucRunOne.txtcurrC.Text = "Ready";
                                        break;
                                    case 1:
                                        ucRunOne.txtcurrC.Text = "Starting";
                                        break;
                                    case 2:
                                        ucRunOne.txtcurrC.Text = currCycleNum.ToString();
                                        break;
                                    case 3:
                                        ucRunOne.txtcurrC.Text = "Finished";
                                        break;
                                }
                */
            });

            //Thread.Sleep(100);
            ReadFanState();
            return inputdatas;
        }

        /*
                /// <summary>
                /// 设置KP
                /// </summary>
                public void SetKP0Zone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//0Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x01;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void SetKP1Zone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//1Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x11;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                /// <summary>
                /// 设置KI
                /// </summary>
                public void SetKI0Zone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//0Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x02;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void SetKI1Zone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//1Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x12;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                /// <summary>
                /// 设置KD
                /// </summary>
                public void SetKD0Zone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//1Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x04;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }
                public void SetKD1Zone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//1Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x14;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                /// <summary>
                /// 设置KL
                /// </summary>
                public void SetKL0Zone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//1Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x08;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void SetKL1Zone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//1Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x18;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }
                /// <summary>
                /// 设置Zone
                /// </summary>
                public void SetZone()
                {
                    byte[] OperBuf = new byte[18];
                    string stempture = "";//1Zone
                    var ftempture = (float)Convert.ToDouble(stempture);
                    byte[] hData = BitConverter.GetBytes(ftempture);
                    OperBuf[0] = hData[0];
                    OperBuf[1] = hData[1];
                    OperBuf[2] = hData[2];
                    OperBuf[3] = hData[3];
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x11;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x09;		//data type
                    TxData[4] = OperBuf[0];
                    TxData[5] = OperBuf[1];
                    TxData[6] = OperBuf[2];
                    TxData[7] = OperBuf[3];

                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;		//back code
                    TxData[10] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }


                public void OpeLedon()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x23;		//data type, led control
                    TxData[4] = 0x81;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void OpeLenoff()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x23;		//data type, led control
                    TxData[4] = 0x80;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }


                public void OpeLed2on()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x23;		//data type, led control
                    TxData[4] = 0x82;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void OpeLed2off()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x23;		//data type, led control
                    TxData[4] = 0x80;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void OpeLed3on()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x23;		//data type, led control
                    TxData[4] = 0x84;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void OpeLed3off()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x23;		//data type, led control
                    TxData[4] = 0x80;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void OpeLed4on()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x23;		//data type, led control
                    TxData[4] = 0x88;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }

                public void OpeLed4off()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[11];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command  
                    TxData[2] = 0x05;		//data length
                    TxData[3] = 0x23;		//data type, led control
                    TxData[4] = 0x80;
                    TxData[5] = 0x00;
                    TxData[6] = 0x00;
                    TxData[7] = 0x00;
                    for (int i = 1; i < 8; i++)
                        TxData[8] += TxData[i];
                    if (TxData[8] == 0x17)
                        TxData[8] = 0x18;
                    else
                        TxData[8] = TxData[8];
                    TxData[9] = 0x17;
                    TxData[10] = 0x17;
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                    string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                }
        */

        public void TestA()
        {
            //string m = Convert.ToString("0x0f", 2);//判断chan是否存在图像
            //char[] arr = m.ToCharArray();
            //string m1 = Convert.ToString(0x01, 2);//判断chan是否存在图像
            //char[] arr1 = m1.ToCharArray();
        }


        private byte[] ReadImages(int chan, int k)
        {
            try
            {
                byte[] inputdatas = new byte[64];
                bool success = MyDeviceManagement.ReadReportFromDevice(0, false, ref inputdatas, 2000);
                //if (inputdatas[6] == 0)
                //{
                //    return null;
                //}

                //string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
                //string ss = "";
                //for (int i = 0; i < inputdatas.Length; i++)
                //{
                //    if (string.IsNullOrEmpty(ss))
                //    {
                //        ss = inputdatas[i].ToString();
                //    }
                //    else
                //    {
                //        ss += " " + inputdatas[i].ToString();
                //    }
                //}
                //WhiteText(res);

                if (!success)
                    DebugLog("Image row read failure.");

                int bcont = 0;
                if (CommData.IFMet == 0)
                {
                    bcont = CommData.imgFrame + 1;
                }
                else
                {
                    bcont = CommData.imgFrame + 5;
                }

                System.UInt16[] TxData = new System.UInt16[bcont];

                // byte lByte = 0x00;

                List<int> datalist = new List<int>();
                for (int NumData = 0; NumData < CommData.imgFrame; NumData++)			//将每两个byte整合成一个word
                {
                    //lByte = inputdatas[NumData * 2 + 6];				//取出低4位byte
                    //lByte <<= 4;				                        //将低4位byte左移4位
                    //TxData[NumData] = inputdatas[NumData * 2 + 7];		//将高8位byte赋值给word变量
                    //TxData[NumData] <<= 8;						//word buffer左移8位，将高8位byte数据放置到高8位
                    //TxData[NumData] |= lByte;				//将低4位byte放到word buffer低8位
                    //TxData[NumData] >>= 4;						//将word buffer整体右移4位，变成有效12位数据



                    int value = TrimReader.TocalADCCorrection(NumData, inputdatas[NumData * 2 + 7], inputdatas[NumData * 2 + 6], CommData.imgFrame, chan, CommData.experimentModelData.gainMode[chan - 1], 0);
                    TxData[NumData] = (ushort)value;


                }

                TxData[CommData.imgFrame] = inputdatas[5];
                if (CommData.IFMet == 1)
                {
                    if (inputdatas[5] == 0)
                    {
                        byte[] buffers = new byte[4];
                        buffers[0] = inputdatas[CommData.imgFrame * 2 + 6];
                        buffers[1] = inputdatas[CommData.imgFrame * 2 + 7];
                        buffers[2] = inputdatas[CommData.imgFrame * 2 + 8];
                        buffers[3] = inputdatas[CommData.imgFrame * 2 + 9];

                        float t = BitConverter.ToSingle(buffers, 0);

                        // TxData[bcont - 1] = (ushort)t;

                        TxData[bcont - 4] = buffers[0];
                        TxData[bcont - 3] = buffers[1];
                        TxData[bcont - 2] = buffers[2];
                        TxData[bcont - 1] = buffers[3];

                        int nb = inputdatas[3];
                        String str;
                        str = "Melt:" + nb.ToString() + " " + t.ToString() + "\r\n";

                        if (nb > 28)
                        {
                            Debug.WriteLine(str);

                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                            {

                                ucTiaoShiOne.txtdebug.AppendText(str);
                                ucTiaoShiOne.txtdebug.ScrollToEnd();

                            });
                        }
                    }
                }

                string res = "";
                string newres = "";
                for (int i = 0; i < TxData.Length; i++)
                {
                    if (i == 0)
                    {
                        res = TxData[i].ToString();
                        newres = TxData[i].ToString();
                    }
                    else
                    {
                        if (i != 11 && i != 23)
                        {
                            res += " " + TxData[i].ToString();
                            newres += "    " + TxData[i].ToString();

                        }
                        else
                        {
                            if (k == 11 || k == 23)
                            {
#if POST_15000
                            res += " " + TxData[i].ToString() + " " + GetFactValueByXS(chan);
                            newres += "    " + TxData[i].ToString() + "    " + GetFactValueByXS(chan);
#else
                            res += " " + GetFactValueByXS(chan);
                            newres += "    " + GetFactValueByXS(chan);
#endif
                            }
                            else
                            {
                                res += " " + TxData[i].ToString();
                                newres += "    " + TxData[i].ToString();
                            }
                        }
                    }
                }

#if MELT_FILE_SAVE
                WriteText(res);
#else
                if (CommData.IFMet == 0)
                {
                    WriteText(res);
                }
                else
                {
                    string name = "Chip#" + chan;
                    CommData.experimentModelData.meltData[name].Add(res);
                }
#endif
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {

                    ucTiaoShiOne.txtImg.AppendText(newres + "\r\n");
                    ucTiaoShiOne.txtImg.ScrollToEnd();

                    int xoff = 0, yoff = 0;

                    switch (chan)
                    {
                        case 1:
                            xoff = 0;
                            yoff = 0;
                            break;
                        case 2:
                            xoff = 12;
                            yoff = 0;
                            break;
                        case 3:
                            xoff = 0;
                            yoff = 12;
                            break;
                        case 4:
                            xoff = 12;
                            yoff = 12;
                            break;
                    }

                    if (chan > 0)
                    {
                        for (int i = 0; i < CommData.imgFrame; i++)
                        {
                            int g = (TxData[i] - 100) / 5;
                            if (g < 0) g = 0;
                            else if (g > 255) g = 255;
                            byte gray = (byte)(g);
                            ucTiaoShiOne.drawPix(i + xoff, k + yoff, gray);
                        }
                    }

                });

                return inputdatas;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "MainWindow: ReadImages");
                return null;
            }
        }

        private string GetFactValueByXS(int n)
        {
            int chan = n - 1;
            int currCycle = xhindex - 1;
            double value = 5000 + m_factorData[chan, currCycle] * 10000;
            return Convert.ToInt32(value).ToString();
        }


        public void ReadFileData()
        {
            try
            {
                // Zhimin, this read is needed for GetMaxData for Update PCRCurve， But this have been done twice, need to consolidate.

/*              StreamReader sr = new StreamReader(F_Path, Encoding.Default);
                var line = System.IO.File.ReadAllLines(F_Path);
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
*/

                CommData.ReadFileData(F_Path, 0, 0, 0);

                //            for (int i = 1; i <= MAX_CHAN; i++)
                //            {
                //                UpdatePCRCurve(i, 0);
                //            }

                if (CommData.cboChan1 == 1) UpdatePCRCurve(1, 0);
                if (CommData.cboChan2 == 1) UpdatePCRCurve(2, 0);
                if (CommData.cboChan3 == 1) UpdatePCRCurve(3, 0);
                if (CommData.cboChan4 == 1) UpdatePCRCurve(4, 0);

                DynamicUpdateIntTime();
                CommData.m_factorData = m_factorData;           // Actually not used

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    if (ucRunOne != null)
                    {
                        //                        ucRunOne.ReadFileNew(CommData.F_Path, 0);     

                        CommData.run1MeltMode = false;
                        ucRunOne.DrawLineNew();

                        if (rboRun.IsChecked == false)
                            rboRun.IsChecked = true;

                        y1.IsChecked = true;
                    }
                });

            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message + "MainWindow: ReadFileData");
#endif
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void ResetTrim()
        {
            if (CommData.currCycleNum > 0)
            {
                return;
            }

            SelSensor(1);
            ucProtocol.ResetParams(MyDeviceManagement);
            SelSensor(2);
            ucProtocol.ResetParams(MyDeviceManagement);
            SelSensor(3);
            ucProtocol.ResetParams(MyDeviceManagement);
            SelSensor(4);
            ucProtocol.ResetParams(MyDeviceManagement);

            SelSensor(1);
            ucProtocol.SetRampgen(MyDeviceManagement, CommData.chan1_rampgen);
            ucProtocol.SetTXbin(MyDeviceManagement, 0xf);
            ucProtocol.SetRange(MyDeviceManagement, 0x0f);
            ucProtocol.SetV15(MyDeviceManagement, CommData.chan1_auto_v15);

            SelSensor(2);
            ucProtocol.SetRampgen(MyDeviceManagement, CommData.chan2_rampgen);
            ucProtocol.SetTXbin(MyDeviceManagement, 0xf);
            ucProtocol.SetRange(MyDeviceManagement, 0x0f);
            ucProtocol.SetV15(MyDeviceManagement, CommData.chan2_auto_v15);

            SelSensor(3);
            ucProtocol.SetRampgen(MyDeviceManagement, CommData.chan3_rampgen);
            ucProtocol.SetTXbin(MyDeviceManagement, 0xf);
            ucProtocol.SetRange(MyDeviceManagement, 0x0f);
            ucProtocol.SetV15(MyDeviceManagement, CommData.chan3_auto_v15);

            SelSensor(4);
            ucProtocol.SetRampgen(MyDeviceManagement, CommData.chan4_rampgen);
            ucProtocol.SetTXbin(MyDeviceManagement, 0xf);
            ucProtocol.SetRange(MyDeviceManagement, 0x0f);
            ucProtocol.SetV15(MyDeviceManagement, CommData.chan4_auto_v15);
/*
#if INIT_HIGH_GAIN
            CommData.gain_mode = 0;           // initialize to high gain mode, consistent with HW default

            SetV20(CommData.chan1_auto_v20[1], 1);
            SetV20(CommData.chan2_auto_v20[1], 2);
            SetV20(CommData.chan3_auto_v20[1], 3);
            SetV20(CommData.chan4_auto_v20[1], 4);
#else
            CommData.gain_mode = 1;           // initialize to low gain mode, consistent with HW default

            SetV20(CommData.chan1_auto_v20[0], 1);
            SetV20(CommData.chan2_auto_v20[0], 2);
            SetV20(CommData.chan3_auto_v20[0], 3);
            SetV20(CommData.chan4_auto_v20[0], 4);
#endif
*/
            for(int i=0; i<MAX_CHAN; i++)
            {
                if(CommData.experimentModelData.gainMode[i] == 0)
                {
                    SetV20(CommData.chan1_auto_v20[1], i + 1);
                }
                else
                {
                    SetV20(CommData.chan4_auto_v20[0], i + 1);
                }
            }

            // CommData.int_time1 = CommData.int_time2 = CommData.int_time3 = CommData.int_time4 = 1;

            ucProtocol.SetLEDConfig(MyDeviceManagement, 1, 1, 1, 1, 1);			// Set Multi LED mode, first enable all channels, then disable all channels.
            Thread.Sleep(100);								                    // Why do we need to do this
            ucProtocol.SetLEDConfig(MyDeviceManagement, 1, 0, 0, 0, 0);

            SetIntergrationTimeAndGain();

            // ucProtocol.SetImgMask(MyDeviceManagement, currDebugModelData);
        }

        private void SelSensor(int n)
        {
            if (n < 1 || n > 4) return;
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x03;		//data length
            TxData[3] = 0x26;		//data type
            TxData[4] = Convert.ToByte(n - 1);		//real data
            TxData[5] = 0x00;
            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code
            this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

        }

        /*        private void ResetParams()
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[9];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x03;		//data length
                    TxData[3] = 0x0F;		//data type, dat edit first byte
                    TxData[4] = 0x00;		//real data, data edit second byte
                    TxData[5] = 0x00;		//real data, data edit third byte
                    TxData[6] = 0x13;		//check sum
                    TxData[7] = 0x17;		//back code
                    TxData[8] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);

                }

                private void SetRampgen(int rampgen)
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[8];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x02;		//data length
                    TxData[3] = 0x01;		//data type, date edit first byte
                    TxData[4] = (byte)rampgen;	//real data, date edit second byte
                    //0x01 means send video data
                    //0x00 means stop video data
                    for (int i = 1; i < 5; i++)
                        TxData[5] += TxData[i];
                    if (TxData[5] == 0x17)
                        TxData[5] = 0x18;
                    else
                        TxData[5] = TxData[5];
                    TxData[6] = 0x17;		//back code
                    TxData[7] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                }

                private void SetTXbin(byte txbin)
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[8];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x02;		//data length
                    TxData[3] = 0x08;		//data type, date edit first byte
                    TxData[4] = txbin;	//real data, date edit second byte
                    //0x01 means send vedio data
                    //0x00 means stop vedio data
                    for (int i = 1; i < 5; i++)
                        TxData[5] += TxData[i];
                    if (TxData[5] == 0x17)
                        TxData[5] = 0x18;
                    else
                        TxData[5] = TxData[5];
                    TxData[6] = 0x17;		//back code
                    TxData[7] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                }

                private void SetRange(byte range)
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[8];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x02;		//data length
                    TxData[3] = 0x02;		//data type, date edit first byte
                    TxData[4] = range;	//real data, date edit second byte
                    //0x01 means send vedio data
                    //0x00 means stop vedio data
                    for (int i = 1; i < 5; i++)
                        TxData[5] += TxData[i];
                    if (TxData[5] == 0x17)
                        TxData[5] = 0x18;
                    else
                        TxData[5] = TxData[5];
                    TxData[6] = 0x17;		//back code
                    TxData[7] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                }

                private void SetV15(int v15)
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[8];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x02;		//data length
                    TxData[3] = 0x05;		//data type, date edit first byte
                    TxData[4] = (byte)v15;	//real data, date edit second byte
                    for (int i = 1; i < 5; i++)
                        TxData[5] += TxData[i];
                    if (TxData[5] == 0x17)
                        TxData[5] = 0x18;
                    else
                        TxData[5] = TxData[5];
                    TxData[6] = 0x17;		//back code
                    TxData[7] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                }
        */
        private void SetV20(int v20, int i)
        {
            SelSensor(i);
            ucProtocol.SetV20(MyDeviceManagement, v20);
        }
        /*
                private void SetV20(int v20)
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[8];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x02;		//data length
                    TxData[3] = 0x04;		//data type, date edit first byte
                    TxData[4] = (byte)v20;		//real data, date edit second byte
                    //0x01 means send vedio data
                    //0x00 means stop vedio data
                    for (int i = 1; i < 5; i++)
                        TxData[5] += TxData[i];
                    if (TxData[5] == 0x17)
                        TxData[5] = 0x18;
                    else
                        TxData[5] = TxData[5];
                    TxData[6] = 0x17;		//back code
                    TxData[7] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                }

                private void SetLEDConfig(int IndvEn, int Chan1, int Chan2, int Chan3, int Chan4)
                {
                    byte[] inputdatas = new byte[64];
                    byte[] TxData = new byte[8];
                    TxData[0] = 0xaa;		//preamble code
                    TxData[1] = 0x01;		//command
                    TxData[2] = 0x02;		//data length
                    TxData[3] = 0x23;		//data type, date edit first byte

                    if (IndvEn == 0)
                    {
                        TxData[4] = Chan1 == 1 ? (byte)1 : (byte)0;									//real data, date edit second byte
                    }
                    else
                    {
                        TxData[4] = 0x80;
                        if (Chan1 == 1)
                            TxData[4] |= 1;
                        if (Chan2 == 1)
                            TxData[4] |= 2;
                        if (Chan3 == 1)
                            TxData[4] |= 4;
                        if (Chan4 == 1)
                            TxData[4] |= 8;
                    }

                    for (int i = 1; i < 5; i++)
                        TxData[5] += TxData[i];

                    if (TxData[5] == 0x17)
                        TxData[5] = 0x18;
                    else
                        TxData[5] = TxData[5];

                    TxData[6] = 0x17;		//back code
                    TxData[7] = 0x17;		//back code
                    this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100);
                }
        */
        private void GetCycleState()
        {
            /*    byte[] inputdatas = new byte[64];
                byte[] TxData = new byte[11];
                TxData[0] = 0xaa;		//preamble code
                TxData[1] = 0x14;		//command  
                TxData[2] = 0x05;		//data length
                TxData[3] = 0x15;		//data type
                TxData[4] = 0x00;
                TxData[5] = 0x00;
                TxData[6] = 0x00;
                TxData[7] = 0x00;
                for (int i = 1; i < 8; i++)
                    TxData[8] += TxData[i];
                if (TxData[8] == 0x17)
                    TxData[8] = 0x18;
                else
                    TxData[8] = TxData[8];
                TxData[9] = 0x17;
                TxData[10] = 0x17;
                this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);
        */

            byte[] inputdatas = ucProtocol.GetCycleState(MyDeviceManagement);

            int header = inputdatas[0];
            int n = inputdatas[5];
            CommData.currCycleState = n;
            currDebugModelData.cyclestate = n;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                if (ucTiaoShiOne != null)
                {
                    ucTiaoShiOne.txtCycleState.Text = n.ToString();
                }
            });

            if (CommData.currCycleState == 3)//当前循环已跑完
            {
                if (currCycleindex < dmdlist.Count)
                {
                    // tmrReadImages.Stop();    // timer already stopped at this point
                    ucProtocol.CloseDev(MyDeviceManagement);
                    Thread.Sleep(100);

                    //                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    //                    {
                    //                      ucRunOne.txtcurrC.Text = currCycleNum.ToString();
                    //                    });

                    currDebugModelData = dmdlist[currCycleindex];
                    StartSY();                  // for multi cycle, but not used. causes exit
                    currCycleindex++;
                }
            }

            if (n == 0 && header != 0)
            {
                Thread.Sleep(100);
                // tmrReadImages.Stop();

                CommData.experimentModelData.endatetime = DateTime.Now;
                //                bool res = CommData.AddExperiment(CommData.experimentModelData);              // Zhimin changed. Dont save it here.
                CommData.expSaved = false;

                if (ucSettingTwo.iscz)          // Starting melt
                {
                    Thread.Sleep(1000);         // This is kind of important. Without this, auto cal for melt tends to fail.
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        ucSettingTwo.TriggerMelt();
                    });
                }
            }
            else if (forceStopFlag)
            {
                ucProtocol.CloseDev(MyDeviceManagement);
                forceStopFlag = false;
                Thread.Sleep(300);
                CommData.experimentModelData.endatetime = DateTime.Now;
                //                bool res = CommData.AddExperiment(CommData.experimentModelData);              // Zhimin changed. Dont save it here.
                CommData.expSaved = false;
            }
            else
            {
                Thread.Sleep(100);
                tmrReadImages.Start();
            }
        }

        private void cboExperiment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            experimentExt experimentExt = cboExperiment.SelectedItem as experimentExt;
            if (experimentExt != null)
            {
                string setdatapath = AppDomain.CurrentDomain.BaseDirectory + experimentExt.SetFileName;

                if (experimentExt.SetFileName.Count() > 0) CommData.experimentModelData = JsonConvert.DeserializeObject<experiment>(File.ReadAllText(setdatapath));
                string endTime = experimentExt.endTime;
                if (endTime.Count() > 0) CommData.experimentModelData.endatetime = Convert.ToDateTime(endTime);
                CommData.experimentModelData.CyderNum = experimentExt.CyderNum;
                CommData.experimentModelData.SetFileName = experimentExt.SetFileName;
                CommData.Cycle = experimentExt.CyderNum;
                CommData.F_Path = AppDomain.CurrentDomain.BaseDirectory + experimentExt.ImgFileName;
                //CommData.F_Path2 = AppDomain.CurrentDomain.BaseDirectory + experimentExt.ImgFileName2;
                CommData.experimentModelData.emname = experimentExt.experimentname;
                if (ucSettingOne != null)
                {
                    ucSettingOne.InitData();
                }

                if (ucSettingTwo != null)
                {
                    ucSettingTwo.initData();
                }

#if COVID_RESULT
                if (ucRunCovid != null)
                {
                    ucRunCovid.InitData();
                }
#else
                if (ucRunTwo != null)
                {
                    ucRunTwo.InitData();
                }
#endif
            }
        }

        // This is the return to home button
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            spxzsy.Visibility = Visibility.Collapsed;

            yhsetting.IsChecked = rboRun.IsChecked = fenxidata.IsChecked = baogaodayin.IsChecked = false;
            
            gridMain.Children.Clear();
            txtUser.Text = "";

            yhsetting.Visibility = rboRun.Visibility = fenxidata.Visibility = baogaodayin.Visibility = xitongtiaoshi.Visibility = Visibility.Collapsed;


#if LOGIN_OK

            //if (tmrReadImages != null)
            //{
            //    tmrReadImages.Stop();
            //    tmrReadImages.Dispose();
            //    tmrReadImages = null;
            //}

            ucLogin login = new ucLogin();
            login.LoginOK += login_LoginOK;
            gridMain.Children.Add(login);
#else
            spxzsy.Visibility = Visibility.Visible;
            //txtUser.Text = CommData.user.Uname;
            // yhsetting.Visibility = rboRun.Visibility = fenxidata.Visibility = baogaodayin.Visibility = Visibility.Visible;
            SetTSVisbile();
            //BindComboxData();
            gridMain.Children.Clear();

            spMD.Visibility = Visibility.Collapsed;

            ucMain ucMain = new ucMain();
            ucMain.rbChecked += ucMain_rbChecked;
            gridMain.Children.Add(ucMain);

#endif
        }

        /// <summary>
        /// 初始化值
        /// </summary>
        public void SetInitData()
        {
            for (int i = 0; i < MAX_CHAN; i++)
            {
                m_dynIntTime[i] = false;
                m_factorIntTime[i] = (float)1.0;
                m_maxPixVal[i] = AutoInt_Target / 2;

                for (int n = 0; n < 100; n++)
                {
                    m_factorData[i, n] = 1;
                }
            }

            dn_cnt = 0;
        }

        public void UpdatePCRCurve(int PCRNum, int pixelNum)
        {
            List<int> list = GetMaxValue(PCRNum);
            if (list.Count > 1)
            {
                int max = list[0];
                int last_max = list[1];
                if (max + (max - last_max) > 3300)
                {
                    m_dynIntTime[PCRNum - 1] = true;
                }
            }
        }


        public void DynamicUpdateIntTime()
        {
            for (int i = 0; i < MAX_CHAN; i++)
            {
                //		if (m_factorInt[i].empty()) {
                //			m_factorInt[i].push_back(m_factorIntTime[i]);    // First time push twice
                //		}
                //		m_factorInt[i].push_back(m_factorIntTime[i]);

                if (m_dynIntTime[i] && m_factorIntTime[i] > 0.01)
                {
                    m_factorIntTime[i] *= (float)0.5;

                    // Call to update Int time
                    float new_factor;
                    new_factor = DynamicUpdateIntTime(m_factorIntTime[i], i);	// done here because we need to set int time before auto trigger happens.
                    m_factorIntTime[i] = new_factor;
                    m_dynIntTime[i] = false;
                }

                m_factorData[i, xhindex] = m_factorIntTime[i];
            }

            xhindex++;

        }

        public float DynamicUpdateIntTime(float factor, int chan)
        {
            /*            switch (chan)
                        {
                            case 0:
                                int_time1 = int_time_1;
                                //int_time1 = (float)Math.Round(Convert.ToDouble(int_time1 * factor));
                                int_time1 = (float)Math.Floor(Convert.ToDouble(int_time1 * factor));
                                SetSensor(1, int_time1);
                                return int_time1 / int_time_1;
                            // break;

                            case 1:
                                int_time2 = int_time_2;
                                //int_time2 = (float)Math.Round(Convert.ToDouble(int_time2 * factor));
                                int_time2 = (float)Math.Floor(Convert.ToDouble(int_time2 * factor));
                                SetSensor(2, int_time2);
                                return int_time2 / int_time_2;
                            //		break;

                            case 2:
                                int_time3 = int_time_3;
                                //int_time3 = (float)Math.Round(Convert.ToDouble(int_time3 * factor));
                                int_time3 = (float)Math.Floor(Convert.ToDouble(int_time3 * factor));
                                SetSensor(3, int_time3);
                                return int_time3 / int_time_3;
                            //		break;

                            case 3:
                                int_time4 = int_time_4;
                                int_time4 = (float)Math.Round(Convert.ToDouble(int_time4 * factor));
                                //int_time4 = (float)Math.Floor(Convert.ToDouble(int_time4 * factor));
                                SetSensor(4, int_time4);
                                return int_time4 / int_time_4;
                            //		break;

                            default:
                                return 1;
                        }
            */

            float int_time_org = 1;         // original
            float int_time_act = 1;         // actual

            switch (chan)
            {
                case 0:
                    int_time_org = CommData.int_time_1;
                    break;
                case 1:
                    int_time_org = CommData.int_time_2;
                    break;
                case 2:
                    int_time_org = CommData.int_time_3;
                    break;
                case 3:
                    int_time_org = CommData.int_time_4;
                    break;
                default:
                    break;
            }

            int_time_act = (float)Math.Floor(Convert.ToDouble(int_time_org * factor));
            if (int_time_act < 1.0f)
            {
                int_time_act = 1.0f;
            }
            SetSensor(chan + 1, int_time_act + 0.5f);        // Because internally it will be rounded down

            DebugLog("Int time cut chan: " + (chan+1).ToString() + " frame: " + CommData.currCycleNum.ToString() + " new int time: " + int_time_act.ToString("0.000"));

            // Modified by Zhimin Nov 2019. Observed that actual int time in effect has some distortion. 

            if(int_time_act <= 20.0f)
            {
                int_time_act *= 0.971f;
            }
            else
            {
                int_time_act += 1.5f;
            }

            int_time_org += 1.5f;

            return int_time_act / int_time_org;
        }


        public List<int> GetMaxValue(int PCRNum)
        {
            List<int> list = new List<int>();

            try
            {
                string chip = "";
                switch (PCRNum)
                {
                    case 1:
                        chip = "Chip#1";
                        break;
                    case 2:
                        chip = "Chip#2";
                        break;
                    case 3:
                        chip = "Chip#3";
                        break;
                    case 4:
                        chip = "Chip#4";
                        break;
                }

                if (CommData.diclist.Count == 0)
                {
                    return list;
                }
                else if (CommData.diclist[chip] != null && CommData.diclist[chip].Count > 0)
                {
                    int nm1 = Convert.ToInt32(CommData.diclist[chip].Count / CommData.imgFrame) - 1;
                    int max = GetValue(chip, nm1);
                    list.Add(max);
                    int last_max = m_maxPixVal[PCRNum - 1];
                    list.Add(last_max);

                    m_maxPixVal[PCRNum - 1] = max;
                }
            }                       
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "GetMaxValue in main");
            }

            return list;
        }

        public int GetValue(string chip, int skip)
        {
            try
            {
                List<int> listOne = new List<int>();
                List<string> strlist = CommData.diclist[chip].Skip(skip * CommData.imgFrame).Take(CommData.imgFrame).ToList();

                int n_row = strlist.Count;
                if(n_row != 12)
                {
#if DEBUG
                    MessageBox.Show("Data corruption found in GetValue in main");
#endif
                }

                for (int k = 0; k < strlist.Count; k++)
                {
                    string[] datalist = strlist[k].Split(' ');

                    int n_col = datalist.Length;
                    if(n_col < 12)
                    {
#if DEBUG
                        MessageBox.Show("Data corruption found in GetValue in main");
#endif
                    }

                    for (int j = 0; j < 12 /*datalist.Length*/; j++)
                    {
                        if ((j == 11 || j == 23) && k == 11)
                            continue;
                        if (string.IsNullOrEmpty(datalist[j]))
                            continue;
                        listOne.Add(Convert.ToInt32(datalist[j]));
                    }
                }

                int max = listOne.Max(a => a);
                return max;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "GetValue in main");
                return 0;
            }
        }

        int jfindex = 0;

        float[] opt_int_time = new float[MAX_CHAN];
        int[] max_read_list = new int[MAX_CHAN];
        float[] inc_factor = new float[MAX_CHAN];
        int[] max_read_0 = new int[MAX_CHAN];

        private void ReadSetTime(object source, ElapsedEventArgs ele)
        {
            try
            {
                tmrJF.Stop();
                if (jfindex > 5)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        ucTiaoShiOne.txtdebug.AppendText("Int Time Calibration Done\r\n");
                        ucTiaoShiOne.txtdebug.ScrollToEnd();
                        ucSettingTwo.bdName.Visibility = Visibility.Collapsed;
                        if (CommData.cboChan1 == 1)
                        {
                            ucTiaoShiTwo.txtITChan1.Text = opt_int_time[0].ToString();
                        }
                        if (CommData.cboChan2 == 1)
                        {
                            ucTiaoShiTwo.txtITChan2.Text = opt_int_time[1].ToString();
                        }
                        if (CommData.cboChan3 == 1)
                        {
                            ucTiaoShiTwo.txtITChan3.Text = opt_int_time[2].ToString();
                        }
                        if (CommData.cboChan4 == 1)
                        {
                            ucTiaoShiTwo.txtITChan4.Text = opt_int_time[3].ToString();
                        }

                        SetIntergrationTimeAndGain();
                        tmrThread = false;

                        if (CommData.experimentModelData.enAutoInt)
                        {
                            if (!CommData.preMelt)
                            {
                                StartESY();
                            }
                            else
                            {
                                CommData.preMelt = false;
                                CommData.IFMet = 1;
                                StartMelt();
                            }
                        }
                    });

                    jfindex = 0;
                    return;
                }
                else
                {
                    ReadAllImg();
                    ReadFileDataBySD();
                    AutocalibInt();
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (CommData.cboChan1 == 1)
                        {
                            string res1 = string.Format("chip#1:max_read:{0};  opt_int_time：{1}", max_read_list[0], opt_int_time[0]);
                            ucTiaoShiOne.txtdebug.AppendText(res1 + "\r\n");
                            ucTiaoShiOne.txtdebug.ScrollToEnd();
                        }
                        if (CommData.cboChan2 == 1)
                        {
                            string res2 = string.Format("chip#2:max_read:{0};  opt_int_time：{1}", max_read_list[1], opt_int_time[1]);
                            ucTiaoShiOne.txtdebug.AppendText(res2 + "\r\n");
                            ucTiaoShiOne.txtdebug.ScrollToEnd();
                        }
                        if (CommData.cboChan3 == 1)
                        {
                            string res3 = string.Format("chip#3:max_read:{0};  opt_int_time：{1}", max_read_list[2], opt_int_time[2]);
                            ucTiaoShiOne.txtdebug.AppendText(res3 + "\r\n");
                            ucTiaoShiOne.txtdebug.ScrollToEnd();
                        }
                        if (CommData.cboChan4 == 1)
                        {
                            string res4 = string.Format("chip#4:max_read:{0};  opt_int_time：{1}", max_read_list[3], opt_int_time[3]);
                            ucTiaoShiOne.txtdebug.AppendText(res4 + "\r\n");
                            ucTiaoShiOne.txtdebug.ScrollToEnd();
                        }
                    });
                    jfindex++;
                    tmrJF.Start();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "Read Set Time main");
            }
        }

        // Auto int time "Zhi Dong Jiao Jie Ji Fen Shi Jian"
        public void ZDCJJFSJ()
        {
            for (int i = 0; i < MAX_CHAN; i++)
            {        
                SetSensor(i + 1, 1.0f);
            }
            Thread.Sleep(300);
            
            CreateWhiteFile_Nosave();
            ReadAllImg();       // Bleed over... Clear out any possible left over image in buffer
            Thread.Sleep(300);

            for (int i = 0; i < MAX_CHAN; i++)
            {
                if (!CommData.preMelt)
                {
                    AutoInt_Target = AUTOINT_TARGET_AMP;
                }
                else
                {
                    AutoInt_Target = 3200;
                }

                opt_int_time[i] = 1;
                max_read_list[i] = 30;
                inc_factor[i] = 0;
                max_read_0[i] = 20;
                SetSensor(i + 1, (float)Math.Round(opt_int_time[i]));
            }
            Thread.Sleep(300);

            CreateWhiteFile_Nosave();

            //SetGainMode();
            //            SetSensor(1, (float)1.0);
            //            SetSensor(2, (float)1.0);
            //            SetSensor(3, (float)1.0);
            //            SetSensor(4, (float)1.0);
            ucTiaoShiTwo.txtITChan1.Text = "1";
            ucTiaoShiTwo.txtITChan2.Text = "1";
            ucTiaoShiTwo.txtITChan3.Text = "1";
            ucTiaoShiTwo.txtITChan4.Text = "1";
            Thread.Sleep(1000);

            tmrJF.Start();
            tmrThread = true;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ZDCJJFSJ();
        }

        public void ReadFileDataBySD()
        {
            try
            {
                StreamReader sr = new StreamReader(F_Path, Encoding.Default);
                var line = System.IO.File.ReadAllLines(F_Path);
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
                //for (int i = 1; i <= MAX_CHAN; i++)
                //{
                //    UpdatePCRCurve(i, 0);
                //}

                //DynamicUpdateIntTime();
                //CommData.m_factorData = m_factorData;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public double[,,] ReadCCurveShow()
        {
            double[,,] m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
            CCurveShow CCurveShow = new CCurveShow();
            CCurveShow.InitData();                              // Zhimin, what is this doing here
            List<string> kslist = new List<string>();//定义孔数

            /*
            kslist.Add("A1");
            kslist.Add("A2");
            kslist.Add("A3");
            kslist.Add("A4");
            kslist.Add("B1");
            kslist.Add("B2");
            kslist.Add("B3");
            kslist.Add("B4");
            */

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

            for (int i = 0; i < tdlist.Count; i++)
            {
                for (int n = 0; n < kslist.Count; n++)
                {
                    //List<ChartData> cdlist = CommData.GetChartData(tdlist[i], 0, kslist[n]);//获取选点值
                    List<ChartData> cdlist = CommData.GetMaxChartData(tdlist[i], 0, kslist[n]);//获取选点值
                    for (int k = 0; k < cdlist.Count; k++)
                    {
                        m_yData[GetChan(tdlist[i]), n, k] = cdlist[k].y;
                    }
                }
            }

            return m_yData;
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

        public void AutocalibInt()
        {
            int max_read;
            float inc;
            float i_factor = 0;
            float t_factor = 1.0f;

            for (int i = 0; i < MAX_CHAN; i++)
            {
                max_read = GetMaxChanRead(i);
                max_read_list[i] = max_read;

                if (jfindex == 0)
                {
                    max_read_0[i] = max_read;
                }
                else if (jfindex == 1)
                {
                    max_read -= max_read_0[i];
                    if (max_read < 20) max_read = 20;

                    float top;

                    switch (i)
                    {
                        case 0:
                            top = 0.5f;
                            t_factor = 1.0f;
                            break;
                        case 1:
                            top = 0.6f;
                            t_factor = 1.0f;
                            break;
                        case 2:
                            top = 1.0f;
                            t_factor = 1.5f;
                            break;
                        case 3:
                            top = 0.8f;
                            t_factor = 1.7f;
                            break;
                        default:
                            top = 0;
                            break;
                    }

                    i_factor = top / (float)max_read;
                    inc_factor[i] = i_factor;
                }

                inc = (float)Math.Round(Convert.ToSingle((t_factor * AutoInt_Target - max_read) * inc_factor[i]), 2);    // slowly approach the opt int time to avoid saturation

                if (inc < 0) inc = 0;

                if (jfindex == 0)
                {
                    opt_int_time[i] = 2;
                }
                else
                {
                    opt_int_time[i] += inc;
                }

                if (opt_int_time[i] > 600)
                    opt_int_time[i] = 600;

                //float a = (float)Math.Round(opt_int_time[i]);
                SetSensor(i + 1, (float)Math.Round(opt_int_time[i]));
                opt_int_time[i] = (float)Math.Round(opt_int_time[i]);

            }
        }

        public int GetMaxChanRead(int ch)
        {
            double[,,] m_yData = ReadCCurveShow();
            double[,] yData = new double[MAX_WELL, MAX_CYCL];
            double max = 0;
            for (int i = 0; i < CommData.KsIndex; i++)
            {
                for (int n = 0; n < MAX_CYCL; n++)
                {
                    yData[i, n] = m_yData[ch, i, n];
                    if (m_yData[ch, i, n] > max)
                    {
                        max = m_yData[ch, i, n];
                    }
                }
            }
            return Convert.ToInt32(max);
        }


        private void ISImgRead_New()
        {
            byte[] TxData = new byte[11];

            /*  
                        byte[] inputdatas = new byte[64];
                        byte[] TxData = new byte[11];
                        TxData[0] = 0xaa;		//preamble code
                        TxData[1] = 0x15;		//command  TXC
                        TxData[2] = 0x05;		//data length
                        TxData[3] = 0x01;		//data type
                        TxData[4] = 0x00;
                        TxData[5] = 0x00;
                        TxData[6] = 0x00;
                        TxData[7] = 0x00;
                        for (int i = 1; i < 8; i++)
                            TxData[8] += TxData[i];
                        if (TxData[8] == 0x17)
                            TxData[8] = 0x18;
                        else
                            TxData[8] = TxData[8];
                        TxData[9] = 0x17;
                        TxData[10] = 0x17;
            */
            byte[] inputdatas = ucProtocol.ISImgRead(MyDeviceManagement);
            //            this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);
            //            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            if (inputdatas[5] != 0)//   if (inputdatas[5] != 0 && isCycleComplete==true)
            {
                string m = Convert.ToString(inputdatas[5], 2);//判断chan是否存在图像
                char[] arr = m.ToCharArray();
                Array.Reverse(arr);

                if (arr.Length > 0)
                {
                    if (arr.Length >= 1 && arr[0] == Convert.ToChar("1") && CommData.cboChan1 == 1)
                    {
                        byte type = CommData.imgFrame == 12 ? (byte)0x02 : (byte)0x08;
                        ReadImgByPType(type, 1);

                    }
                    if (arr.Length >= 2 && arr[1] == Convert.ToChar("1") && CommData.cboChan2 == 1)
                    {
                        byte type = CommData.imgFrame == 12 ? (byte)0x12 : (byte)0x18;
                        ReadImgByPType(type, 2);
                    }
                    if (arr.Length >= 3 && arr[2] == Convert.ToChar("1") && CommData.cboChan3 == 1)
                    {
                        byte type = CommData.imgFrame == 12 ? (byte)0x22 : (byte)0x28;
                        ReadImgByPType(type, 3);
                    }
                    if (arr.Length >= 4 && arr[3] == Convert.ToChar("1") && CommData.cboChan4 == 1)
                    {
                        byte type = CommData.imgFrame == 12 ? (byte)0x32 : (byte)0x38;
                        ReadImgByPType(type, 4);
                    }
                    ReadFileData_New();
                }
            }

            GetCycleStateNew();
        }

        const bool method_1 = false;

        static int flo_update_cnt = 0;

        static int flo_update_cnt2 = 0;

        public void ReadFileData_New()
        {
            try
            {
                // Zhimin, this read is needed for GetMaxData for Update PCRCurve， But this have been done twice, need to consolidate.
                if (method_1)
                {
                    StreamReader sr = new StreamReader(F_Path, Encoding.Default);
                    var line = System.IO.File.ReadAllLines(F_Path);

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
                }
                else
                {
                    // CommData.ReadFileData(F_Path, 0, 0, 1);            // program 1 for Melt Data

                    if (CommData.experimentModelData.meltData.Count > 0)
                    {
                        CommData.diclist = CommData.experimentModelData.meltData;
                    }
                }

                if (CommData.cboChan1 == 1)
                    UpdatePCRCurve(1, 0);

                if (CommData.cboChan2 == 1)
                    UpdatePCRCurve(2, 0);

                if (CommData.cboChan3 == 1)
                    UpdatePCRCurve(3, 0);

                if (CommData.cboChan4 == 1)
                    UpdatePCRCurve(4, 0);

                DynamicUpdateIntTime();
                CommData.m_factorData = m_factorData;       // actually not used

                flo_update_cnt++;
                if (flo_update_cnt > 10)
                {
                    flo_update_cnt = 0;
                }

                flo_update_cnt2++;
                if (flo_update_cnt2 > 4)
                {
                    flo_update_cnt2 = 0;
                }

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    //if (flo_update_cnt == 0)
                    //{
                    //    if (rboRun.IsChecked == false)
                    //        rboRun.IsChecked = true;

                    //    y2.IsChecked = true;            // Modified 1/24/20 to use RunOne to monitor Fluorescence during Melt.
                    //}
                    if (flo_update_cnt2 == 0)
                    {
                        if (ucRunOne != null)
                        {
                            //                        ucRongJQX.ReadFileNew(CommData.F_Path2, 0);       // Didn't I just do it at above? CommData.F_Path2 is F_Path in main
                            // ucRongJQX.DrawLineNew();
                            CommData.run1MeltMode = true;
                            ucRunOne.DrawLineNew();                     // Added 1/24/20 to use RunOne to monitor Fluorescence during Melt.
                        }
                    }
                });
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message + "Read File New in main");
#endif
            }
        }

        private void ReadRJQXTmp(object source, ElapsedEventArgs e)
        {
            tmrRJQX.Stop();
            ReadPITemperatureNew();

            //s3.IsChecked = true;
            //gridMain.Children.Clear();

            int st = CommData.currCycleState;

            if (st != 0)
            {
                if (!forceStopFlag)
                {
                    tmrRJQX.Start();
                }
                else
                {
                    byte[] rxdata = ucProtocol.CloseDev(MyDeviceManagement);
                    DebugLog("Force stop -- Issued close device command", ref rxdata);

                    forceStopFlag = false;
                    Thread.Sleep(100);
                    tmrRJQX.Start();



                    //  CommData.currCycleState = 0;        // Force to 0 because I am no longer going to read state

                    // hmm... Maybe I should continue run the timer and the next cycle, it will become state 0.

                }
            }
            else
            {
                CommData.experimentModelData.meltendatetime = DateTime.Now;
                //                bool res = CommData.AddExperiment(CommData.experimentModelData);              // Zhimin changed. Dont save it here.
                CommData.expSaved = false;
                tmrThread = false;

                DebugLog("Melt analysis finished (state = 0, ready state).");
                //CommData.expSaved = false;
                //tmrThread = false;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    string str = ucTiaoShiOne.txtdebug.Text;
                    CommData.experimentModelData.DebugLog = str;

                    ucRunOne.UpdateTempCurve();         // This is just to display the "Ready" state
                    ucRunOne.UpdateCurrCycleNum();
                });
            }
        }

        private byte[] ReadPITemperatureNew()
        {

            /*            byte[] inputdatas = new byte[16];

                        byte[] TxData = new byte[18];
                        TxData[0] = 0xaa;		//preamble code
                        TxData[1] = 0x10;		//command
                        TxData[2] = 0x0C;		//data length
                        TxData[3] = 0x02;		//data type, date edit first byte
                        TxData[4] = 0x01;		//real data
                        TxData[5] = 0x00;		//预留位
                        TxData[6] = 0x00;
                        TxData[7] = 0x00;
                        TxData[8] = 0x00;
                        TxData[9] = 0x00;
                        TxData[10] = 0x00;
                        TxData[11] = 0x00;
                        TxData[12] = 0x00;
                        TxData[13] = 0x00;
                        TxData[14] = 0x00;
                        for (int i = 1; i < 15; i++)
                        {
                            TxData[15] += TxData[i];
                        }
                        if (TxData[15] == 0x17)
                            TxData[15] = 0x18;
                        else
                            TxData[15] = TxData[15];
                        TxData[16] = 0x17;		//back code
                        TxData[17] = 0x17;		//back code



                        this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);

                        string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            */

            byte[] inputdatas = ucProtocol.ReadPITemperatureNew(MyDeviceManagement);
            byte[] buffers = new byte[] { inputdatas[5], inputdatas[6], inputdatas[7], inputdatas[8] };

            float t = BitConverter.ToSingle(buffers, 0);
            WhiteTempPIText(t.ToString());
            CommData.temp_history[0].Add(t);
            pi_temp = t;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucTiaoShiOne.txtPi.AppendText(t.ToString() + "\r\n");
                ucTiaoShiOne.txtPi.ScrollToEnd();
                //ucTiaoShiThree.txtPi.AppendText(t.ToString() + "\r\n");
                //ucTiaoShiThree.txtPi.ScrollToEnd();
            });
            //Thread.Sleep(100);
            ReadPTTemperatureNew();
            //txtwd.Text = res;
            //currtxtwd.Text = t.ToString();
            return inputdatas;
        }

        private byte[] ReadPTTemperatureNew()
        {

            /*            byte[] inputdatas = new byte[16];

                        byte[] TxData = new byte[18];
                        TxData[0] = 0xaa;		//preamble code
                        TxData[1] = 0x10;		//command
                        TxData[2] = 0x0C;		//data length
                        TxData[3] = 0x02;		//data type, date edit first byte
                        TxData[4] = 0x02;		//real data
                        TxData[5] = 0x00;		//预留位
                        TxData[6] = 0x00;
                        TxData[7] = 0x00;
                        TxData[8] = 0x00;
                        TxData[9] = 0x00;
                        TxData[10] = 0x00;
                        TxData[11] = 0x00;
                        TxData[12] = 0x00;
                        TxData[13] = 0x00;
                        TxData[14] = 0x00;
                        for (int i = 1; i < 15; i++)
                        {
                            TxData[15] += TxData[i];
                        }
                        if (TxData[15] == 0x17)
                            TxData[15] = 0x18;
                        else
                            TxData[15] = TxData[15];
                        TxData[16] = 0x17;		//back code
                        TxData[17] = 0x17;		//back code



                        this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);

                        string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            */

            byte[] inputdatas = ucProtocol.ReadPTTemperatureNew(MyDeviceManagement);
            byte[] buffers = new byte[] { inputdatas[5], inputdatas[6], inputdatas[7], inputdatas[8] };

            float t = BitConverter.ToSingle(buffers, 0);
            WhiteTempPEText(t.ToString());

            CommData.temp_history[1].Add(t);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucTiaoShiOne.txtPt.AppendText(t.ToString() + "\r\n");
                ucTiaoShiOne.txtPt.ScrollToEnd();
                //ucTiaoShiThree.txtPt.AppendText(t.ToString() + "\r\n");
                //ucTiaoShiThree.txtPt.ScrollToEnd();

                //ucReportThree.txtPTTemp.Text = t.ToString("0.00");
                //ucReportThree.txtPITemp.Text = pi_temp.ToString("0.00");
                //                ucReportThree.txtElapsedTime.Text = string.Format("{0:00}:{1:00}:{2:00}", tt.Hours, tt.Minutes, tt.Seconds);

                th_count++;

                if (th_count > 5)
                {
                    th_count = 0;
                    //ucReportThree.UpdateTempCurve();
                    ucRunOne.UpdateTempCurve();
                    ucRunOne.UpdateCurrCycleNumMelt();

                    //TimeSpan tt = new TimeSpan(0, 0, (int)CommData.GetElapsedTime());                   
                    //ucReportThree.txtElapsedTime.Text = string.Format("{0:00}:{1:00}:{2:00}", tt.Hours, tt.Minutes, tt.Seconds);
                }
            });
            //Thread.Sleep(100);
            ISImgRead_New();
            //txtwd.Text = res;
            //currtxtwd.Text = t.ToString();
            return inputdatas;
        }

        private void GetCycleStateNew()
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x14;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x15;		//data type
            TxData[4] = 0x00;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;

            this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);

            int n = inputdatas[5];
            int h = inputdatas[0];

            if (h != 0)
            {
                Debug.Assert(h == 0xaa);
                CommData.currCycleState = n;
                currDebugModelData.cyclestate = n;
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                if (ucTiaoShiOne != null)
                {
                    ucTiaoShiOne.txtCycleState.Text = n.ToString();
                }
                if (n == 0)
                {
                    if (ucRongJQX != null)
                    {
                        ucRongJQX.ReadCCurveShow();             // Why?
                    }
                }
            });
        }

        const int EPKT_SZ = 52;
        const int NUM_EPKT = 4;
        const int TRIM_IMAGER_SIZE = 12;

        byte[,] EepromBuff = new byte[16 + 4 * NUM_EPKT, EPKT_SZ + 1];          // DP file can use upto 16 pages

        private void ReadEEPROM(object sender, EventArgs e)
        {

            //================

            if (!CommData.deviceFound)
            {
                var splash = new ucSplash();

                //                TrimReader.ReadTrimFile();

                bool res = FindTheHid();
                //                bool res2 = CommData.ReadDatapositionFile();

                if (res)
                {
                    ResetTrim();    //下位机复位
                    SetInitData();
#if ENGLISH_VER
                    txtHid.Text = "HID Device Connected"; // "HID设备已连接";
#else
                    txtHid.Text = "HID设备已连接";
#endif
                    bd1.Visibility = Visibility.Visible;
                    bd2.Visibility = Visibility.Collapsed;

                    Startup();

                    CommData.deviceFound = true;
                }
                /*                else
                                {
#if ENGLISH_VER
                                    txtHid.Text = "HID Device Not Present"; // "HID设备未连接";
#else
                                txtHid.Text = "HID设备未连接";
#endif
                                    bd2.Visibility = Visibility.Visible;
                                    bd1.Visibility = Visibility.Collapsed;
                                }
                */
                else
                {
                    MessageBox.Show("HID Device not found");
                }

                if (res)
                {
                    splash.Show();
                    ReadFlash();
                    //MessageBox.Show("从设备Flash存储器里读出Trim数据。");
                    splash.Close();
                }

            }

            //================

            return;

            ReadFlash();

#if ENGLISH_VER
            MessageBox.Show("Read Trim Data from Device"); //  ("从设备Flash存储器里读出Trim数据。");
#else
            System.Windows.MessageBox.Show("从设备Flash存储器里读出Trim数据。");
#endif
        }

        private void ReadFlash()
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x04;		//command  
            TxData[2] = 0x02;		//data length
            TxData[3] = 0x2d;		//data type
            TxData[4] = 0x00;       // chan num, for qPCR it is always 0

            for (int i = 1; i < 5; i++)
                TxData[5] += TxData[i];

            if (TxData[5] == 0x17)
                TxData[5] = 0x18;

            TxData[6] = 0x17;       //back code
            TxData[7] = 0x17;		//back code

            this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);

            int index = inputdatas[7];      // For command type 2d EEPROM read command
            int npages = inputdatas[6];

            MoveToEEPBuffer(inputdatas, index);

            bool ee_continue = index < npages - 1;

            while (ee_continue)
            {
                this.MyDeviceManagement.ReadReportFromDevice(0, ref inputdatas, 10000);
                index = inputdatas[7];      // For command type 2d EEPROM read command
                npages = inputdatas[6];

                MoveToEEPBuffer(inputdatas, index);

                ee_continue = index < npages - 1;
            }

            List<int> rlist = new List<int>();      // row index
            List<int> clist = new List<int>();      // col index

            byte[] trim_buff = new byte[2048];     // big enough to handle 16 well 4 channel

            for (int j = 0; j < EPKT_SZ; j++)
            {           // parity not copied
                trim_buff[j] = EepromBuff[0, j];        // copy first page
            }

            int k = 0;

            byte header = trim_buff[k]; k++;

            byte sn1 = 0, sn2 = 0, version = 0, well_format = 0;    // Well format 1: one row; 2: two row
            int num_channels = 4, num_wells = 16, num_pages = 1;
            StringBuilder sb = new StringBuilder();

            //byte[] ba = new byte[32];

            // id_str.Append((char)sn1);

            if (header != 0xa5)
            {
                version = header;
                sn1 = trim_buff[k]; k++;
                sn2 = trim_buff[k]; k++;

                num_channels = trim_buff[k]; k++;
                num_wells = trim_buff[k]; k++;
                num_pages = trim_buff[k]; k++;
            }
            else
            {
                version = trim_buff[k]; k++;
                num_pages = trim_buff[k]; k++;

                // id_str.clear();
                for (int i = 0; i < 32; i++)
                {
                    char idc = (char)trim_buff[k]; k++;
                    sb.Append(idc);
                }

                sn1 = trim_buff[k]; k++;
                sn2 = trim_buff[k]; k++;

                num_channels = trim_buff[k]; k++;
                num_wells = trim_buff[k]; k++;

                well_format = trim_buff[k]; k++;

                byte rsv;
                rsv = trim_buff[k]; k++;    // Reserved bytes
                rsv = trim_buff[k]; k++;
                rsv = trim_buff[k]; k++;
                rsv = trim_buff[k]; k++;
            }

            CommData.KsIndex = num_wells;
            CommData.TdIndex = num_channels;

            CommData.ver = version;
            CommData.sn1 = sn1;
            CommData.sn2 = sn2;
            CommData.header = header;
            CommData.well_format = well_format;

            CommData.id_str = sb.ToString();

            if (num_channels <= 2)
            {
                //CommData.cboChan3 = 0;
                //CommData.cboChan4 = 0;

                CommData.experimentModelData.CbooChan3 = false;
                CommData.experimentModelData.CbooChan4 = false;
            }

            for (int i = 1; i < num_pages; i++)
            {
                for (int j = 0; j < EPKT_SZ; j++)
                {           // parity not copied
                    trim_buff[i * EPKT_SZ + j] = EepromBuff[i, j];
                }
            }

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

            CommData.UpdateDarkMap();
            dpstr = Buf2String(trim_buff, k);

            CommData.dpstr = dpstr;
            CommData.experimentModelData.dpStr = dpstr;

            dpstr = "Chipdp\r\n" + dpstr;

            for (int ci = 0; ci < num_channels; ci++)
            {
                int index_start = num_pages + ci * NUM_EPKT;
                k = 0;

                for (int i = 0; i < NUM_EPKT; i++)
                {
                    for (int j = 0; j < EPKT_SZ; j++)
                    {           // parity not copied
                        trim_buff[i * EPKT_SZ + j] = EepromBuff[i + index_start, j];
                    }
                }

                byte b0 = trim_buff[k]; k++;        // Extract chip name
                byte b1 = trim_buff[k]; k++;
                byte b2 = trim_buff[k]; k++;

                for (int i = 0; i < TRIM_IMAGER_SIZE; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        CommData.kbi[ci, i, j] = Buf2Int(trim_buff, ref k);
                    }
                }

                for (int i = 0; i < TRIM_IMAGER_SIZE; i++)
                {
                    CommData.fpni[ci, 0, i] = Buf2Int(trim_buff, ref k);
                    CommData.fpni[ci, 1, i] = Buf2Int(trim_buff, ref k);
                }

                CommData.rampgen[ci] = trim_buff[k]; k++;
                CommData.range[ci] = trim_buff[k]; k++;
                CommData.auto_v20[ci, 0] = trim_buff[k]; k++;
                CommData.auto_v20[ci, 1] = trim_buff[k]; k++;
                CommData.auto_v15[ci] = trim_buff[k]; k++;
            }

            CommData.flash_loaded = true;

            ResetTrimFromFlash();
        }

        private void MoveToEEPBuffer(byte[] inputdatas, int index)
        {
            byte eeprom_parity = 0;

            for (int i = 0; i < EPKT_SZ + 1; i++)
            {
                EepromBuff[index, i] = inputdatas[8 + i];

                if (i < EPKT_SZ)
                {
                    eeprom_parity += inputdatas[8 + i];
                }
                else
                {
                    if (eeprom_parity != inputdatas[8 + i])
                    {
                        MessageBox.Show("Packet parity error!");
                    }
                }
            }
        }

        private int Buf2Int(byte[] buff, ref int k)
        {
            byte[] x = new byte[2] { buff[k + 1], buff[k] };
            int y = (int)BitConverter.ToInt16(x, 0);
            k += 2;
            return y;
        }

        private string Buf2String(byte[] buff, int size)
        {
            string rstr = "";

            //            rstr = "Chipdp\r\n";

            for (int i = 0; i < size; i++)
            {
                string str = string.Format("{0} ", buff[i]);
                rstr += str;
            }
            // rstr += "\r\n";

            return rstr;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void ResetTrimFromFlash()
        {
            if (CommData.currCycleNum > 0)
            {
                return;
            }

            //            SelSensor(1);
            //            ucProtocol.ResetParams(MyDeviceManagement);
            //            SelSensor(2);
            //            ucProtocol.ResetParams(MyDeviceManagement);
            //            SelSensor(3);
            //            ucProtocol.ResetParams(MyDeviceManagement);
            //            SelSensor(4);
            ucProtocol.ResetParams(MyDeviceManagement);

            SelSensor(1);
            ucProtocol.SetRampgen(MyDeviceManagement, CommData.rampgen[0]);
            ucProtocol.SetTXbin(MyDeviceManagement, 0xf);
            ucProtocol.SetRange(MyDeviceManagement, CommData.range[0]);
            ucProtocol.SetV15(MyDeviceManagement, CommData.auto_v15[0]);

            SelSensor(2);
            ucProtocol.SetRampgen(MyDeviceManagement, CommData.rampgen[1]);
            ucProtocol.SetTXbin(MyDeviceManagement, 0xf);
            ucProtocol.SetRange(MyDeviceManagement, CommData.range[1]);
            ucProtocol.SetV15(MyDeviceManagement, CommData.auto_v15[1]);

            SelSensor(3);
            ucProtocol.SetRampgen(MyDeviceManagement, CommData.rampgen[2]);
            ucProtocol.SetTXbin(MyDeviceManagement, 0xf);
            ucProtocol.SetRange(MyDeviceManagement, CommData.range[2]);
            ucProtocol.SetV15(MyDeviceManagement, CommData.auto_v15[2]);

            SelSensor(4);
            ucProtocol.SetRampgen(MyDeviceManagement, CommData.rampgen[3]);
            ucProtocol.SetTXbin(MyDeviceManagement, 0xf);
            ucProtocol.SetRange(MyDeviceManagement, CommData.range[3]);
            ucProtocol.SetV15(MyDeviceManagement, CommData.auto_v15[3]);

            /*
#if INIT_HIGH_GAIN
            CommData.gain_mode = 0;// initialize to high gain mode, consistent with HW default

            SetV20(CommData.auto_v20[0, 1], 1);
            SetV20(CommData.auto_v20[1, 1], 2);
            SetV20(CommData.auto_v20[2, 1], 3);
            SetV20(CommData.auto_v20[3, 1], 4);
#else
            CommData.gain_mode = 1;// initialize to low gain mode, consistent with HW default

            SetV20(CommData.auto_v20[0, 0], 1);
            SetV20(CommData.auto_v20[1, 0], 2);
            SetV20(CommData.auto_v20[2, 0], 3);
            SetV20(CommData.auto_v20[3, 0], 4);
#endif
*/

            for (int i = 0; i < MAX_CHAN; i++)
            {
                if (CommData.experimentModelData.gainMode[i] == 0)
                {
                    SetV20(CommData.chan1_auto_v20[1], i + 1);
                }
                else
                {
                    SetV20(CommData.chan4_auto_v20[0], i + 1);
                }
            }

            // CommData.int_time1 = CommData.int_time2 = CommData.int_time3 = CommData.int_time4 = 1;

            ucProtocol.SetLEDConfig(MyDeviceManagement, 1, 1, 1, 1, 1);			// Set Multi LED mode, first enable all channels, then disable all channels.
            Thread.Sleep(100);								                    // Why do we need to do this
            ucProtocol.SetLEDConfig(MyDeviceManagement, 1, 0, 0, 0, 0);

            SetIntergrationTimeAndGain();
        }

        /// <summary>
        /// 判断是否连接电源、热盖是否关闭 2019.04.16
        /// </summary>
        /// <returns></returns>
        private int CheckPowerLid()
        {
            //bool returnValue = false;

            byte[] inputdatas = new byte[16];
            byte[] TxData = new byte[12];

            TxData[0] = 0xAA;		//preamble code
            TxData[1] = 0x17;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x01;		//data type, date edit first byte
            TxData[4] = 0x00;		//real data
            TxData[5] = 0x00;		//预留位
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 9; i++)
            {
                TxData[9] += TxData[i];
            }
            if (TxData[9] == 0x17)
                TxData[9] = 0x18;
            else
                TxData[9] = TxData[9];

            TxData[10] = 0x17;		//back code
            TxData[11] = 0x17;		//back code

            this.MyDeviceManagement.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            int checkValue = 3;
            if (inputdatas.Length < 12) checkValue = 1;

            TextBlock txtLid = new TextBlock();
            TextBlock txtPower = new TextBlock();

            string str = "";

            string argValue = Convert.ToString(inputdatas[5], 2);

            if ("10".Equals(argValue))
            {
                txtLid.Text = "热盖";
                txtLid.Foreground = Brushes.LightGray;
                txtPower.Text = "主电源";
                txtPower.Foreground = Brushes.Red;
                checkValue = 1;

#if ENGLISH_VER
                str = "Lid is OK, main power not on. ";
#else
                str = "热盖 OK, 主电源 没有接. ";
#endif
            }
            else if ("1".Equals(argValue))
            {
                txtLid.Text = "热盖";
                txtLid.Foreground = Brushes.Red;
                txtPower.Text = "主电源";
                txtPower.Foreground = Brushes.LightGray;
                checkValue = 2;

#if ENGLISH_VER
                str = "The lid is not closed. Main power is OK. ";
#else
                str = "热盖没有关, 主电源 OK. ";
#endif
            }
            else if ("0".Equals(argValue))
            {
                txtLid.Text = "热盖";
                txtLid.Foreground = Brushes.LightGray;
                txtPower.Text = "主电源";
                txtPower.Foreground = Brushes.LightGray;
                checkValue = 0;

                //                str = "热盖 OK, 主电源 OK.";
            }
            else if ("11".Equals(argValue))
            {
                txtPower.Text = "主电源";
                txtPower.Foreground = Brushes.Red;
                txtLid.Text = "热盖";
                txtLid.Foreground = Brushes.Red;

#if ENGLISH_VER
                str = "Lid not closed, main power not applied. ";
#else
                str = "热盖没有关, 主电源也没有接. ";
#endif
            }

            if (checkValue > 0)
            {
#if ENGLISH_VER
                str += "Please correct and try again.";
#else
                str += "请矫正后再试.";
#endif

                MessageBox.Show(str);
            }

            return checkValue;
        }

        private void DebugLog(string str)
        {
            Debug.WriteLine(str);

            str += "\r\n";

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucTiaoShiOne.txtdebug.AppendText(str);
                ucTiaoShiOne.txtdebug.ScrollToEnd();
            });
        }

        private void DebugLogEmpty(string str)
        {
            Debug.WriteLine(str);
            str += "\r\n";

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {

                ucTiaoShiOne.txtdebug.Text = str;
                ucTiaoShiOne.txtdebug.ScrollToEnd();
            });
        }

        private void DebugLog(string str, ref byte[] rxd)
        {
            Debug.WriteLine(str);

            str += " Cmd: " + rxd[2].ToString("x") + " Type: " + rxd[4].ToString("x") + " Status: " + rxd[1].ToString("x") + "\r\n";

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {

                ucTiaoShiOne.txtdebug.AppendText(str);
                ucTiaoShiOne.txtdebug.ScrollToEnd();

            });
        }

        public void PrintReport(string img_path, string img_path2)
        {
            try
            {
                //string FilePath = AppDomain.CurrentDomain.BaseDirectory + "Report/报告模板.xlsx";
                //string excelnewFilePath = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report\\{0}报告模板.xlsx", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));
                //string pdfnewFilePath = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report\\{0}报告模板.pdf", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));

                string FilePath = AppDomain.CurrentDomain.BaseDirectory + "Report/report_template.xlsx";
                string excelnewFilePath = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report\\{0}report_template.xlsx", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));
                string pdfnewFilePath = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report\\{0}report_template.pdf", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));


                ExcelEdit ExcelEdit = new ExcelEdit();
                ExcelEdit.Open(FilePath);

                ExcelEdit.ws = ExcelEdit.GetSheet("Sheet1");
                Microsoft.Office.Interop.Excel.Range Cell1 = (Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[7, "B"];
                Microsoft.Office.Interop.Excel.Range Cell2 = (Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[22, "J"];
                Microsoft.Office.Interop.Excel.Range SourceRange = ExcelEdit.ws.get_Range(Cell1, Cell2);
                if(!string.IsNullOrEmpty(img_path))
                    ExcelEdit.InsertPicture(SourceRange, ExcelEdit.ws, img_path);

                // ExcelEdit.ws.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;

                ExcelEdit.ws.PageSetup.Zoom = false;
                ExcelEdit.ws.PageSetup.FitToPagesWide = 1;
                ExcelEdit.ws.PageSetup.FitToPagesTall = false;

                // To set up the page breaks, just go to report_template in excell and use View->PageBreakPreview option to set up page breaks.

                if (!string.IsNullOrEmpty(img_path2))
                {
                    Cell1 = (Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[57, "B"];
                    Cell2 = (Microsoft.Office.Interop.Excel.Range)ExcelEdit.ws.Cells[72, "J"];
                    SourceRange = ExcelEdit.ws.get_Range(Cell1, Cell2);

                    ExcelEdit.InsertPicture(SourceRange, ExcelEdit.ws, img_path2);
                }

                if (CommData.KsIndex == 4)
                {
                    for (int i = 0; i < MAX_CHAN; i++)
                    {
                        string[] ct = new string[4];

                        for (int j = 0; j < 4; j++)
                        {
                            if (CommData.IsEmptyWell(i, j))
                            {
                                ct[j] = "";
                            }
                            else if (CommData.CTValue[i, j] > 0.01 && !CommData.falsePositive[i, j])
                            {
                                ct[j] = CommData.CTValue[i, j].ToString("0.00"); // Math.Round(m_CTValue[i, j], 2);
                            }
                            else
                            {
#if ENGLISH_VER
                                ct[j] = "Neg";
#else
                                ct[j] = "阴性";
#endif
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
                    for (int i = 0; i < MAX_CHAN; i++)
                    {
                        string[] ct = new string[8];

                        for (int j = 0; j < 8; j++)
                        {
                            if (CommData.IsEmptyWell(i, j))
                            {
                                ct[j] = "";
                            }
                            else if (CommData.CTValue[i, j] > 0.01 && !CommData.falsePositive[i, j])
                            {
                                ct[j] = CommData.CTValue[i, j].ToString("0.00"); // Math.Round(m_CTValue[i, j], 2);
                            }
                            else
                            {
#if ENGLISH_VER
                                ct[j] = "Neg";
#else
                                ct[j] = "阴性";
#endif
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
                            else if (CommData.CTValue[i, j] > 0.01 && !CommData.falsePositive[i, j])
                            {
                                ct[j] = CommData.CTValue[i, j].ToString("0.00"); // Math.Round(m_CTValue[i, j], 2);
                            }
                            else
                            {
#if ENGLISH_VER
                                ct[j] = "Neg";
#else
                                ct[j] = "阴性";
#endif
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

                bool enMelt = false;        // Fixme

                if (CommData.experimentModelData.DebugModelDataList != null && CommData.experimentModelData.DebugModelDataList.Count > 0)
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

                    enMelt = CommData.experimentModelData.DebugModelDataList[0].enMelt;
                }

                //=================

                if(enMelt)
                {
                    if (CommData.KsIndex == 4)
                    {
                        for (int i = 0; i < MAX_CHAN; i++)
                        {
                            string[] mt = new string[4];

                            for (int j = 0; j < 4; j++)
                            {
                                if (CommData.IsEmptyWell(i, j))
                                {
                                    mt[j] = "";
                                }
                                else if (CommData.MTValue[i, j] > 0.01)
                                {
                                    mt[j] = CommData.MTValue[i, j].ToString("0.00"); 
                                }
                                else
                                {
                                    mt[j] = "";
                                }
                            }

                            ExcelEdit.ws.Cells[79 + i, "C"] = mt[0];
                            ExcelEdit.ws.Cells[79 + i, "D"] = mt[1];
                            ExcelEdit.ws.Cells[79 + i, "E"] = mt[2];
                            ExcelEdit.ws.Cells[79 + i, "F"] = mt[3];
                        }
                    }
                    else if (CommData.KsIndex == 8)
                    {
                        for (int i = 0; i < MAX_CHAN; i++)
                        {
                            string[] mt = new string[8];

                            for (int j = 0; j < 8; j++)
                            {
                                if (CommData.IsEmptyWell(i, j))
                                {
                                    mt[j] = "";
                                }
                                else if (CommData.MTValue[i, j] > 0.01)
                                {
                                    mt[j] = CommData.MTValue[i, j].ToString("0.00"); 
                                }
                                else
                                {
                                    mt[j] = "";
                                }
                            }

                            ExcelEdit.ws.Cells[79 + i, "C"] = mt[0];
                            ExcelEdit.ws.Cells[79 + i, "D"] = mt[1];
                            ExcelEdit.ws.Cells[79 + i, "E"] = mt[2];
                            ExcelEdit.ws.Cells[79 + i, "F"] = mt[3];

                            ExcelEdit.ws.Cells[85 + i, "C"] = mt[4];
                            ExcelEdit.ws.Cells[85 + i, "D"] = mt[5];
                            ExcelEdit.ws.Cells[85 + i, "E"] = mt[6];
                            ExcelEdit.ws.Cells[85 + i, "F"] = mt[7];
                        }
                    }
                    else if (CommData.KsIndex > 8)
                    {
                        for (int i = 0; i < MAX_CHAN; i++)
                        {
                            string[] mt = new string[16];

                            for (int j = 0; j < 16; j++)
                            {
                                if (CommData.IsEmptyWell(i, j))
                                {
                                    mt[j] = "";
                                }
                                else if (CommData.MTValue[i, j] > 0.01)
                                {
                                    mt[j] = CommData.MTValue[i, j].ToString("0.00"); 
                                }
                                else
                                {
                                    mt[j] = "";
                                }
                            }

                            ExcelEdit.ws.Cells[79 + i, "C"] = mt[0];
                            ExcelEdit.ws.Cells[79 + i, "D"] = mt[1];
                            ExcelEdit.ws.Cells[79 + i, "E"] = mt[2];
                            ExcelEdit.ws.Cells[79 + i, "F"] = mt[3];
                            ExcelEdit.ws.Cells[79 + i, "G"] = mt[4];
                            ExcelEdit.ws.Cells[79 + i, "H"] = mt[5];
                            ExcelEdit.ws.Cells[79 + i, "I"] = mt[6];
                            ExcelEdit.ws.Cells[79 + i, "J"] = mt[7];

                            ExcelEdit.ws.Cells[85 + i, "C"] = mt[8];
                            ExcelEdit.ws.Cells[85 + i, "D"] = mt[9];
                            ExcelEdit.ws.Cells[85 + i, "E"] = mt[10];
                            ExcelEdit.ws.Cells[85 + i, "F"] = mt[11];
                            ExcelEdit.ws.Cells[85 + i, "G"] = mt[12];
                            ExcelEdit.ws.Cells[85 + i, "H"] = mt[13];
                            ExcelEdit.ws.Cells[85 + i, "I"] = mt[14];
                            ExcelEdit.ws.Cells[85 + i, "J"] = mt[15];
                        }
                    }
                }

                //=================

                ExcelEdit.SaveAs(excelnewFilePath);

                ExcelEdit.Close();

                SaveFileDialog sfd = new SaveFileDialog();

#if ENGLISH_VER
                sfd.Filter = "PDF File (*.pdf)|*.pdf|All files|*.*";//设置文件类型
                //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
                sfd.DefaultExt = "PDF";//设置默认格式（可以不设）
                sfd.AddExtension = true;//设置自动在文件名中添加扩展名
                if (sfd.ShowDialog() == true)
                {

                    pdfnewFilePath = sfd.FileName;
                    bool success = ExcelConvertPDF(excelnewFilePath, pdfnewFilePath, Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF);
                    if (success)
                        MessageBox.Show("PDF Saved Successfully");
                }
#else
                sfd.Filter = "文本文件(*.pdf)|*.pdf|所有文件|*.*";//设置文件类型
                //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
                sfd.DefaultExt = "PDF";//设置默认格式（可以不设）
                sfd.AddExtension = true;//设置自动在文件名中添加扩展名
                if (sfd.ShowDialog() == true)
                {

                    pdfnewFilePath = sfd.FileName;
                    bool success = ExcelConvertPDF(excelnewFilePath, pdfnewFilePath, Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF);
                    if (success)
                        MessageBox.Show("PDF 文件保存成功");
                }
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        public static void PreventSleep()
        {
            ExecutionState res = SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired);

            if (res == 0)
            {
                MessageBox.Show("Set execution state failed");
            }
        }

        public static void AllowSleep()
        {
            ExecutionState res = SetThreadExecutionState(ExecutionState.EsContinuous);

            if (res == 0)
            {
                MessageBox.Show("Set execution state failed");
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [FlagsAttribute]
        private enum ExecutionState : uint
        {
            EsAwaymodeRequired = 0x00000040,
            EsContinuous = 0x80000000,
            EsDisplayRequired = 0x00000002,
            EsSystemRequired = 0x00000001
        }

        private int dn_cnt = 0;
        private const int base_cnt = 90;

        private bool DnAutoint()
        {
            dn_cnt++;

            if (dn_cnt < base_cnt || !CommData.experimentModelData.enAutoInt)
                return false;
            else if (dn_cnt == base_cnt)
            {
                StartDnAutoint();
                Thread.Sleep(300);

                //                return;
            }

            if (dn_cnt < base_cnt + 6)
            {
                jfindex = dn_cnt - base_cnt;

                ReadAllImg();
                ReadFileDataBySD();
                AutocalibInt();

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    if (CommData.cboChan1 == 1)
                    {
                        string res1 = string.Format("chip#1:max_read:{0};  opt_int_time：{1}", max_read_list[0], opt_int_time[0]);
                        ucTiaoShiOne.txtdebug.AppendText(res1 + "\r\n");
                        ucTiaoShiOne.txtdebug.ScrollToEnd();
                    }
                    if (CommData.cboChan2 == 1)
                    {
                        string res2 = string.Format("chip#2:max_read:{0};  opt_int_time：{1}", max_read_list[1], opt_int_time[1]);
                        ucTiaoShiOne.txtdebug.AppendText(res2 + "\r\n");
                        ucTiaoShiOne.txtdebug.ScrollToEnd();
                    }
                    if (CommData.cboChan3 == 1)
                    {
                        string res3 = string.Format("chip#3:max_read:{0};  opt_int_time：{1}", max_read_list[2], opt_int_time[2]);
                        ucTiaoShiOne.txtdebug.AppendText(res3 + "\r\n");
                        ucTiaoShiOne.txtdebug.ScrollToEnd();
                    }
                    if (CommData.cboChan4 == 1)
                    {
                        string res4 = string.Format("chip#4:max_read:{0};  opt_int_time：{1}", max_read_list[3], opt_int_time[3]);
                        ucTiaoShiOne.txtdebug.AppendText(res4 + "\r\n");
                        ucTiaoShiOne.txtdebug.ScrollToEnd();
                    }
                });
                Thread.Sleep(300);
                return true;
            }
            else if (dn_cnt == base_cnt + 6)
            {
                jfindex = 0;
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    ucTiaoShiOne.txtdebug.AppendText("Int Time Calibration Done\r\n");
                    ucTiaoShiOne.txtdebug.ScrollToEnd();
                    ucSettingTwo.bdName.Visibility = Visibility.Collapsed;

                    if (CommData.cboChan1 == 1)
                    {
                        ucTiaoShiTwo.txtITChan1.Text = opt_int_time[0].ToString();
                    }
                    if (CommData.cboChan2 == 1)
                    {
                        ucTiaoShiTwo.txtITChan2.Text = opt_int_time[1].ToString();
                    }
                    if (CommData.cboChan3 == 1)
                    {
                        ucTiaoShiTwo.txtITChan3.Text = opt_int_time[2].ToString();
                    }
                    if (CommData.cboChan4 == 1)
                    {
                        ucTiaoShiTwo.txtITChan4.Text = opt_int_time[3].ToString();
                    }
                });

                SetIntergrationTimeAndGain(opt_int_time[0], opt_int_time[1], opt_int_time[2], opt_int_time[3]);
                Thread.Sleep(300);
                //开始实验的时候重新创建新的文件

                CreateWhiteFile();

                if (dpstr != null && dpstr.Length > 0)
                {
                    OpenText();
                    WriteText(dpstr);
                    CloseText();
                }

                return true;
            }
            return false;
        }        

        private void StartDnAutoint()
        {
            for (int i = 0; i < MAX_CHAN; i++)
            {
                SetSensor(i + 1, 1.0f);
            }
            Thread.Sleep(300);

            CreateWhiteFile_Nosave();
            ReadAllImg();                   // Bleed over... Clear out any possible left over image in buffer
            CreateWhiteFile_Nosave();

            for (int i = 0; i < MAX_CHAN; i++)
            {
                if (!CommData.preMelt)
                {
                    AutoInt_Target = AUTOINT_TARGET_AMP;
                }
                else
                {
                    AutoInt_Target = 3200;
                }

                opt_int_time[i] = 1;
                max_read_list[i] = 30;
                inc_factor[i] = 0;
                max_read_0[i] = 20;

                SetSensor(i + 1, (float)Math.Round(opt_int_time[i]));
            }
            Thread.Sleep(300);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ucTiaoShiTwo.txtITChan1.Text = "1";
                ucTiaoShiTwo.txtITChan2.Text = "1";
                ucTiaoShiTwo.txtITChan3.Text = "1";
                ucTiaoShiTwo.txtITChan4.Text = "1";
            });
        }

        public bool PrintCSVReport(string csvPath)
        {
            try
            {
                string csvString = "Experiment Name, " + CommData.experimentModelData.emname + "\r\n";

                // csvString += "Experiment Time, " + CommData.experimentModelData.emdatetime + "\r\n";

                // csvString += "Experiment Time (start-end), " + CommData.experimentModelData.emdatetime + " - " + CommData.experimentModelData.endatetime + "\r\n";
                csvString += "PCR Experiment Time (start-end), " + CommData.experimentModelData.emdatetime + " - " + CommData.experimentModelData.endatetime + "\r\n";
                csvString += "Melt Experiment Time (start-end), " + CommData.experimentModelData.meltemdatetime + " - " + CommData.experimentModelData.meltendatetime + "\r\n";

                byte[] ba = new byte[1];
                ba[0] = CommData.ver;

                string ascii = System.Text.Encoding.ASCII.GetString(ba);

                if(CommData.header == 0xa5)
                {
                    ascii = CommData.id_str;
                }

                csvString += "Equipment ID, " + ascii + "-" + CommData.sn1.ToString() + "-" + CommData.sn2.ToString() + "\r\n";

                csvString += "Experiment Result - Ct:" + "\r\n";
                csvString += CommData.csvString;

                if (CommData.experimentModelData.DebugModelDataList != null)
                {
                    if (CommData.experimentModelData.DebugModelDataList[0].enMelt)
                    {
                        csvString += "Experiment Result - Melt:" + "\r\n";
                        csvString += CommData.mtCSVString;
                    }
                }

                csvString += CommData.printCyclerString();

                System.IO.File.WriteAllText(csvPath, csvString, System.Text.Encoding.UTF8);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "CSV Print");
                return false;
            }
        }

        private bool SaveJsonData()
        {
            try
            {
                string fname = string.Format("SetData\\SetData_{0}_{1}.Json", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString(" hhmmss"));
                string path = AppDomain.CurrentDomain.BaseDirectory + fname;
                string jsonstring = JsonConvert.SerializeObject(CommData.experimentModelData);
                System.IO.File.WriteAllText(path, jsonstring);

                CommData.experimentModelData.SetFileName = fname;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }
}
 