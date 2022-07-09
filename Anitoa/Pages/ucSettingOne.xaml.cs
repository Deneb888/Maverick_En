#define ENGLISH_VER

// #define Lumin_Lite

// #define TwoByFour

using Microsoft.Win32;
using Newtonsoft.Json;
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
    /// ucSettingOne.xaml 的交互逻辑
    /// </summary>
    public partial class ucSettingOne : UserControl
    {
        private bool bLoadTmpl = false;

        public event EventHandler ChooseM;
        public ucSettingOne()
        {
            InitializeComponent();
            this.Loaded += ucSettingOne_Loaded;
            this.Unloaded += ucSettingOne_Unloaded;
        }

        void ucSettingOne_Unloaded(object sender, RoutedEventArgs e)
        {
            Save();

            if (ChooseM != null)
            {
                ChooseM("Unload", null);
            }

            CommData.expSaved = false;
        }

        void ucSettingOne_Loaded(object sender, RoutedEventArgs e)
        {
           
            if (CommData.load_template)     // This is for LuminUltra
            {
                OpenReadFile();
                CommData.load_template = false;
            }
            else
            {
                InitData();
            }

            if(CommData.flash_loaded)
            {
                cbLoadTmpl.IsChecked = true;
            }

            OpenAssayFile();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("Next", null);
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            Save();

            if (ChooseM != null)
            {
                ChooseM("Next", null);
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void Save()
        {
            //            if(txtemname.Text.Count() == 0)
            //                txtemname.Text = "实验" + Convert.ToDateTime(emdatetime.SelectedDate);

            CommData.experimentModelData.emname = txtemname.Text;

            /*
            if (emdatetime.SelectedDate != null)
            {
                CommData.experimentModelData.emdatetime = Convert.ToDateTime(emdatetime.SelectedDate);
            }
            */
            if (cboChan1.SelectedItem != null)
            {
                ComboBoxItem cbi = cboChan1.SelectedItem as ComboBoxItem;
                CommData.experimentModelData.chanonetype = cbi.Content.ToString();
            }
            CommData.experimentModelData.chanonedes = chanonedes.Text;
            if (cboChan2.SelectedItem != null)
            {
                ComboBoxItem cbi = cboChan2.SelectedItem as ComboBoxItem;
                CommData.experimentModelData.chantwotype = cbi.Content.ToString();
            }
            CommData.experimentModelData.chantwodes = chantwodes.Text;
            if (cboChan3.SelectedItem != null)
            {
                ComboBoxItem cbi = cboChan3.SelectedItem as ComboBoxItem;
                CommData.experimentModelData.chanthreetype = cbi.Content.ToString();
            }
            CommData.experimentModelData.chanthreedes = chanthreedes.Text;
            if (cboChan4.SelectedItem != null)
            {
                ComboBoxItem cbi = cboChan4.SelectedItem as ComboBoxItem;
                CommData.experimentModelData.chanfourtype = cbi.Content.ToString();
            }
            CommData.experimentModelData.chanfourdes = chanfourdes.Text;

            CommData.experimentModelData.A1des = gdA1.Text;
            CommData.experimentModelData.A2des = gdA2.Text;
            CommData.experimentModelData.A3des = gdA3.Text;
            CommData.experimentModelData.A4des = gdA4.Text;
            CommData.experimentModelData.A5des = gdA5.Text;
            CommData.experimentModelData.A6des = gdA6.Text;
            CommData.experimentModelData.A7des = gdA7.Text;
            CommData.experimentModelData.A8des = gdA8.Text;

            CommData.experimentModelData.B1des = gdB1.Text;
            CommData.experimentModelData.B2des = gdB2.Text;
            CommData.experimentModelData.B3des = gdB3.Text;
            CommData.experimentModelData.B4des = gdB4.Text;
            CommData.experimentModelData.B5des = gdB5.Text;
            CommData.experimentModelData.B6des = gdB6.Text;
            CommData.experimentModelData.B7des = gdB7.Text;
            CommData.experimentModelData.B8des = gdB8.Text;

            CommData.experimentModelData.CbooChan1 = cbooChan1.IsChecked.Value;
            CommData.experimentModelData.CbooChan2 = cbooChan2.IsChecked.Value;
            CommData.experimentModelData.CbooChan3 = cbooChan3.IsChecked.Value;
            CommData.experimentModelData.CbooChan4 = cbooChan4.IsChecked.Value;

            //            return;     // temporarilly disable

#if Lumin_Lite

            if (CommData.assayList.Count() > 0)
            {
                List<DebugModelData> dmdlist = getDebugModelData();
                CommData.experimentModelData.programMode = "Amplification";
                CommData.experimentModelData.DebugModelDataList = dmdlist;
            }

#endif

            //experiment emd = CommData.experimentModelData;

            //emd.assayChanName[0] = chanonedes.Text;
            //emd.assayChanName[1] = chantwodes.Text;
            //emd.assayChanName[2] = chanthreedes.Text;
            //emd.assayChanName[3] = chanfourdes.Text;


        }

        public void InitData()
        {
            txtemname.Text = CommData.experimentModelData.emname;

            if (txtemname.Text.Count() == 0)
            {
#if ENGLISH_VER
                //                txtemname.Text = "Experiment " + Convert.ToDateTime(DateTime.Now);
                txtemname.Text = "Experiment_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
#else
                //txtemname.Text = "实验" + Convert.ToDateTime(DateTime.Now);
                txtemname.Text = "实验_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
#endif
            }

            /*
            if (CommData.experimentModelData.emdatetime == null)
            {
                CommData.experimentModelData.emdatetime = DateTime.Now;
                CommData.experimentModelData.endatetime = DateTime.Now;
            }
            */

            if (CommData.experimentModelData.emdatetime == null)
                emdatetime.SelectedDate = DateTime.Now; //  CommData.experimentModelData.emdatetime;
            else
                emdatetime.SelectedDate = CommData.experimentModelData.emdatetime;

            foreach (var item in cboChan1.Items)
            {
                ComboBoxItem cbi = item as ComboBoxItem;
                if (cbi.Content.ToString() == CommData.experimentModelData.chanonetype)
                {
                    cboChan1.SelectedItem = item;
                    break;
                }
            }
            chanonedes.Text = CommData.experimentModelData.chanonedes;

            foreach (var item in cboChan2.Items)
            {
                ComboBoxItem cbi = item as ComboBoxItem;
                if (cbi.Content.ToString() == CommData.experimentModelData.chantwotype)
                {
                    cboChan2.SelectedItem = item;
                    break;
                }
            }
            chantwodes.Text = CommData.experimentModelData.chantwodes;

            foreach (var item in cboChan3.Items)
            {
                ComboBoxItem cbi = item as ComboBoxItem;
                if (cbi.Content.ToString() == CommData.experimentModelData.chanthreetype)
                {
                    cboChan3.SelectedItem = item;
                    break;
                }
            }
            chanthreedes.Text = CommData.experimentModelData.chanthreedes;

            foreach (var item in cboChan4.Items)
            {
                ComboBoxItem cbi = item as ComboBoxItem;
                if (cbi.Content.ToString() == CommData.experimentModelData.chanfourtype)
                {
                    cboChan4.SelectedItem = item;
                    break;
                }
            }
            chanfourdes.Text = CommData.experimentModelData.chanfourdes;

            gdA1.Text = CommData.experimentModelData.A1des;
            gdA2.Text = CommData.experimentModelData.A2des;
            gdA3.Text = CommData.experimentModelData.A3des;
            gdA4.Text = CommData.experimentModelData.A4des;
            gdA5.Text = CommData.experimentModelData.A5des;
            gdA6.Text = CommData.experimentModelData.A6des;
            gdA7.Text = CommData.experimentModelData.A7des;
            gdA8.Text = CommData.experimentModelData.A8des;
            gdB1.Text = CommData.experimentModelData.B1des;
            gdB2.Text = CommData.experimentModelData.B2des;
            gdB3.Text = CommData.experimentModelData.B3des;
            gdB4.Text = CommData.experimentModelData.B4des;
            gdB5.Text = CommData.experimentModelData.B5des;
            gdB6.Text = CommData.experimentModelData.B6des;
            gdB7.Text = CommData.experimentModelData.B7des;
            gdB8.Text = CommData.experimentModelData.B8des;

            gdA1.ToolTip = "Double click to add or modify sample";
            gdA2.ToolTip = "Double click to add or modify sample";
            gdA3.ToolTip = "Double click to add or modify sample";
            gdA4.ToolTip = "Double click to add or modify sample";
            gdA5.ToolTip = "Double click to add or modify sample";
            gdA6.ToolTip = "Double click to add or modify sample";
            gdA7.ToolTip = "Double click to add or modify sample";
            gdA8.ToolTip = "Double click to add or modify sample";

            gdB1.ToolTip = "Double click to add or modify sample";
            gdB2.ToolTip = "Double click to add or modify sample";
            gdB3.ToolTip = "Double click to add or modify sample";
            gdB4.ToolTip = "Double click to add or modify sample";
            gdB5.ToolTip = "Double click to add or modify sample";
            gdB6.ToolTip = "Double click to add or modify sample";
            gdB7.ToolTip = "Double click to add or modify sample";
            gdB8.ToolTip = "Double click to add or modify sample";

            cbooChan1.IsChecked = CommData.experimentModelData.CbooChan1;
            cbooChan2.IsChecked = CommData.experimentModelData.CbooChan2;
            cbooChan3.IsChecked = CommData.experimentModelData.CbooChan3;
            cbooChan4.IsChecked = CommData.experimentModelData.CbooChan4;

            CommData.cboChan1 = Convert.ToInt32(CommData.experimentModelData.CbooChan1);
            CommData.cboChan2 = Convert.ToInt32(CommData.experimentModelData.CbooChan2);
            CommData.cboChan3 = Convert.ToInt32(CommData.experimentModelData.CbooChan3);
            CommData.cboChan4 = Convert.ToInt32(CommData.experimentModelData.CbooChan4);

            if (CommData.TdIndex <= 2)
            {
                cbooChan3.IsEnabled = false;
                cbooChan3.Opacity = 0.3;

                cbooChan4.IsEnabled = false;
                cbooChan4.Opacity = 0.3;
            }
            else
            {
                cbooChan3.IsEnabled = true;
                cbooChan3.Opacity = 1;

                cbooChan4.IsEnabled = true;
                cbooChan4.Opacity = 1;
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
                    gdB1.IsEnabled = true;
                    gdB2.IsEnabled = true;
                    gdB3.IsEnabled = true;
                    gdB4.IsEnabled = true;
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
                    gdA5.IsEnabled = true;
                    gdA6.IsEnabled = true;
                    gdA7.IsEnabled = true;
                    gdA8.IsEnabled = true;
                }
#endif
            }
            else  // 16
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
            }
        }

        private void rbOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenReadFile();
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        public void OpenReadFile()
        {
            if(!CommData.expSaved)
            {
                MessageBoxResult res = MessageBox.Show("Please Save Experiment Data Before Loading ... (Go to Report tab and choose Save Experiment)", "Save Data?", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes) return;
            }

            try
            {

                OpenFileDialog pOpenFileDialog = new OpenFileDialog();

#if ENGLISH_VER
                pOpenFileDialog.Filter = "Json File|*.json";//若打开指定类型的文件只需修改Filter，如打开txt文件，改为*.txt即可
                pOpenFileDialog.Multiselect = false;
                pOpenFileDialog.Title = "Open experiment or experiment template file";
#else
                pOpenFileDialog.Filter = "Json文件|*.json";//若打开指定类型的文件只需修改Filter，如打开txt文件，改为*.txt即可
                pOpenFileDialog.Multiselect = false;
                pOpenFileDialog.Title = "打开实验或模板文件";
#endif

                if (pOpenFileDialog.ShowDialog() == true)
                {
                    string path = pOpenFileDialog.FileName;
                    //string setdatapath = AppDomain.CurrentDomain.BaseDirectory + pOpenFileDialog.SafeFileName;
                    CommData.experimentModelData = JsonConvert.DeserializeObject<experiment>(File.ReadAllText(path));
                    CommData.openFileName = path;

                    if (!bLoadTmpl && CommData.flash_loaded)
                    {
                        MessageBoxResult res = MessageBox.Show("Load experiment as template", "Load Experiment?", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes) cbLoadTmpl.IsChecked = true;
                    }

                    if (bLoadTmpl)
                    {
                        CommData.experimentModelData.ImgFileName = null;
                        CommData.experimentModelData.ImgFileName2 = null;
                        CommData.experimentModelData.SetFileName = null;

                        CommData.experimentModelData.ampData.Clear();
                        if (CommData.experimentModelData.meltData != null)
                            CommData.experimentModelData.meltData.Clear();

                        if (!String.IsNullOrEmpty(CommData.dpstr))
                            CommData.experimentModelData.dpStr = CommData.dpstr;                        // Restore HW info from the current set

                        CommData.experimentModelData.DebugLog = null;

                        CommData.diclist.Clear();
                        CommData.F_Path = null;
                        CommData.F_Path2 = null;

                    }
                    else
                    {
                        //CommData.experimentModelData.ImgFileName = null;
                        CommData.F_Path = CommData.experimentModelData.ImgFileName;
                        CommData.F_Path2 = CommData.experimentModelData.ImgFileName2;


                        string dpstr = CommData.experimentModelData.dpStr;
                        CommData.ParseDpstr(dpstr);
                    }

                    if (CommData.TdIndex <= 2)
                    {
                        CommData.experimentModelData.CbooChan3 = false;
                        CommData.experimentModelData.CbooChan4 = false;
                    }

                    InitData();

                    ChooseM("OpenExp", null);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Your file is incompatible with this version of software. Please contact support with the following details: \"" + ex.Message + "SettingOne: OpenReadFile\"");

            }
        }

        public void OpenAssayFile()
        {
            try
            {
//                string path = "Assay.json";
//                CommData.assayData = JsonConvert.DeserializeObject<Assay>(File.ReadAllText(path));


                string path = "AssayList.json";
                CommData.assayList = JsonConvert.DeserializeObject<List<Assay>>(File.ReadAllText(path));

                /*
                // load multiple assay files from Assay directory

                List<Assay> alist = new List<Assay>();

                int k = 0;

                foreach (string file in Directory.EnumerateFiles("Assay", "*.json"))
                {
                    k++;
                    Assay newAssay = new Assay();
               
                    string contents = File.ReadAllText(file);
                    newAssay = JsonConvert.DeserializeObject<Assay>(contents);

                    alist.Add(newAssay);
                }

                CommData.assayList = alist;
                */

            }
            catch (Exception ex)
            {
#if Lumin_Lite
                MessageBox.Show(ex.Message + "SettingOne: OpenAssayFile");
#endif
            }
        }

        private void cbooChan_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb.Name)
            {
                case "cbooChan1":
                    CommData.cboChan1 = 1;
                    break;
                case "cbooChan2":
                    CommData.cboChan2 = 1;
                    break;
                case "cbooChan3":
                    CommData.cboChan3 = 1;
                    break;
                case "cbooChan4":
                    CommData.cboChan4 = 1;
                    break;
            }
        }

        private void cbooChan_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb.Name)
            {
                case "cbooChan1":
                    CommData.cboChan1 = 0;
                    break;
                case "cbooChan2":
                    CommData.cboChan2 = 0;
                    break;
                case "cbooChan3":
                    CommData.cboChan3 = 0;
                    break;
                case "cbooChan4":
                    CommData.cboChan4 = 0;
                    break;
            }
        }

        private void gd_SampleProperty(object sender, MouseButtonEventArgs e)
        {

            TextBox tb = sender as TextBox;

            string init_str;

            if (String.IsNullOrEmpty(tb.Text))
            {
                init_str = "";
            }
            else
            {
                init_str = tb.Text;
            }

            string tagstr = tb.Tag.ToString();

            char[] carr;    // char array
            carr = tagstr.ToCharArray();

            int index = Convert.ToInt32(carr[1].ToString()) - 1;

            int row;    // 0: A, 1: B

            if (carr[0] == 'A')
                row = 0;
            else
                row = 1;

            experiment emd = CommData.experimentModelData;

            ucSampleSetup modalWindow = new ucSampleSetup();

            modalWindow.txtSample.Text = init_str;
            modalWindow.cboType.SelectedIndex = emd.sampleTypeIndex[row, index];
            modalWindow.cboQuantUnit.SelectedIndex = emd.sampleQuantUnitIndex[row, index];
            modalWindow.cboAssay.SelectedIndex = emd.sampleAssayIndex[row, index];
            modalWindow.cboExtractMethod.SelectedIndex = emd.sampleExtractMethodIndex[row, index];
            modalWindow.txtQuant.Text = emd.sampleQuant[row, index];

            if (modalWindow.ShowDialog() == true)
            {
                string sn = modalWindow.txtSample.Text;
                tb.Text = sn;

                int y = modalWindow.cboType.SelectedIndex;
                ComboBoxItem cbi = modalWindow.cboType.SelectedItem as ComboBoxItem;
                string str = cbi.Content.ToString();

                emd.sampleName[row, index] = sn;
                emd.sampleType[row, index] = str;
                emd.sampleTypeIndex[row, index] = modalWindow.cboType.SelectedIndex;
                emd.sampleQuantUnit[row, index] = modalWindow.cboQuantUnit.Text;
                emd.sampleQuantUnitIndex[row, index] = modalWindow.cboQuantUnit.SelectedIndex;
                emd.sampleAssayIndex[row, index] = modalWindow.cboAssay.SelectedIndex;
                emd.sampleExtractMethodIndex[row, index] = modalWindow.cboExtractMethod.SelectedIndex;
                emd.sampleQuant[row, index] = modalWindow.txtQuant.Text;

                // scan channels

#if Lumin_Lite
                Assay ass = CommData.assayList[modalWindow.cboAssay.SelectedIndex];
                ChannelParam cp = ass.channelParamLists[0];

                if(cp.active == true)
                {
                    cbooChan1.IsChecked = true;
                    CommData.cboChan1 = 1;
                }
                 cp = ass.channelParamLists[1];
                if (cp.active == true)
                {
                    cbooChan2.IsChecked = true;
                    CommData.cboChan2 = 1;
                }
                 cp = ass.channelParamLists[2];
                if (cp.active == true)
                {
                    cbooChan3.IsChecked = true;
                    CommData.cboChan3 = 1;
                }
                 cp = ass.channelParamLists[3];
                if (cp.active == true)
                {
                    cbooChan4.IsChecked = true;
                    CommData.cboChan4 = 1;
                }
#endif
            }
        }

        private List<DebugModelData> getDebugModelData()
        {
            List<DebugModelData> dmdlist = new List<DebugModelData>();

            Assay assay = CommData.assayList[0];
            CyclerParam cp = assay.cyclerParam;

            try
            {

                DebugModelData currDebugModelData = new DebugModelData();

                currDebugModelData.ifpz = 0;    // 0 means paizhao
                currDebugModelData.Cycle = cp.numCycles;
                currDebugModelData.Hotlid = 105;

                if (cp.extensionEn)
                {
                    currDebugModelData.stageIndex = 3;
                    currDebugModelData.StepCount = 3;
                }
                else
                {
                    currDebugModelData.stageIndex = 2;
                    currDebugModelData.StepCount = 2;
                }

                currDebugModelData.DenaturatingStart = cp.denatureTemp;
                currDebugModelData.Denaturating = cp.denatureTemp;
                currDebugModelData.DenaturatingTime = cp.denatureTime;

                currDebugModelData.AnnealingStart = cp.denatureTemp;
                currDebugModelData.Annealing = cp.annealingTemp;
                currDebugModelData.AnnealingTime = cp.annealingTime;

                                //currDebugModelData.ExtensionStart = 60;       // Have to do this, making "Extension" 0 to show there is no extension segment
                                //currDebugModelData.Extension = 72;
                                //currDebugModelData.ExtensionTime = 20;


                currDebugModelData.InitaldenaturationStart = 40;
                currDebugModelData.Initaldenaturation = cp.predenatureTemp;
                currDebugModelData.InitaldenaTime = cp.predenatureTime;

                currDebugModelData.HoldonStart = 60;
                currDebugModelData.Holdon = 50;
                currDebugModelData.HoldonTime = 60;

                currDebugModelData.MeltStart = 60;
                currDebugModelData.MeltEnd = 90;        // initialize with something reasonable
                currDebugModelData.MeltStart = 10;
                currDebugModelData.MeltEnd = 10;        // initialize with something reasonable
                currDebugModelData.enMelt = false;

                dmdlist.Add(currDebugModelData);

                return dmdlist;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dmdlist;
            }
        }

        private void cboLoadTmpl_Checked(object sender, RoutedEventArgs e)
        {
            bLoadTmpl = true;
        }

        private void cboLoadTmpl_UnChecked(object sender, RoutedEventArgs e)
        {
            bLoadTmpl = false;
        }

        private void btnPropertyClick_Chan1(object sender, RoutedEventArgs e)
        {
            ucAssaySetup modalWindow = new ucAssaySetup();

            modalWindow.txtChannelName.Text = chanonedes.Text;

            experiment emd = CommData.experimentModelData;
         
            modalWindow.cboType.SelectedIndex = emd.assayChanTypeIndex[0];

            modalWindow.txtStdSlope.Text = emd.assayChanStdSlope[0];
            modalWindow.txtStdIntercept.Text = emd.assayChanStdIntercept[0];

            if (modalWindow.ShowDialog() == true)
            {
                chanonedes.Text = modalWindow.txtChannelName.Text;

                int y = modalWindow.cboType.SelectedIndex;
                ComboBoxItem cbi = modalWindow.cboType.SelectedItem as ComboBoxItem;
                string str = cbi.Content.ToString();

                emd.assayChanType[0] = str;
                emd.assayChanTypeIndex[0] = modalWindow.cboType.SelectedIndex;

                emd.assayChanStdSlope[0] = modalWindow.txtStdSlope.Text;
                emd.assayChanStdIntercept[0] = modalWindow.txtStdIntercept.Text;
            }
        }

        private string AssayChanProperty(int chan, string name)
        {
            ucAssaySetup modalWindow = new ucAssaySetup();
            modalWindow.txtChannelName.Text = name;
            string rname = name;

            experiment emd = CommData.experimentModelData;
            modalWindow.cboType.SelectedIndex = emd.assayChanTypeIndex[chan];

            modalWindow.txtStdSlope.Text = emd.assayChanStdSlope[chan];
            modalWindow.txtStdIntercept.Text = emd.assayChanStdIntercept[chan];

            if (modalWindow.ShowDialog() == true)
            {
                rname = modalWindow.txtChannelName.Text;

                int y = modalWindow.cboType.SelectedIndex;
                ComboBoxItem cbi = modalWindow.cboType.SelectedItem as ComboBoxItem;
                string str = cbi.Content.ToString();

                emd.assayChanType[chan] = str;
                emd.assayChanTypeIndex[chan] = modalWindow.cboType.SelectedIndex;

                emd.assayChanStdSlope[chan] = modalWindow.txtStdSlope.Text;
                emd.assayChanStdIntercept[chan] = modalWindow.txtStdIntercept.Text;
            }

            return rname;
        }

        private void btnPropertyClick_Chan2(object sender, RoutedEventArgs e)
        {
            string name = AssayChanProperty(1, chantwodes.Text);
            chantwodes.Text = name;
        }

        private void btnPropertyClick_Chan3(object sender, RoutedEventArgs e)
        {
            string name = AssayChanProperty(2, chanthreedes.Text);
            chanthreedes.Text = name;
        }

        private void btnPropertyClick_Chan4(object sender, RoutedEventArgs e)
        {
            string name = AssayChanProperty(3, chanfourdes.Text);
            chanfourdes.Text = name;
        }

        private void ResetParams(object sender, RoutedEventArgs e)
        {
            
            MessageBoxResult res = MessageBox.Show("This will clear all experiment setting parameters. Proceed?", "Clear Data?", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.No) return;

            experiment newExperiment = new experiment();

            CommData.experimentModelData = newExperiment;

            // CommData.F_Path = "";
            
            if(!CommData.deviceFound)       // If device is not attached, clear the device info also.
            {
                CommData.KsIndex = 16;
                CommData.TdIndex = 4;
                CommData.ver = Convert.ToByte('x');
                CommData.sn1 = 0;
                CommData.sn2 = 0;
            }

            InitData();
            ChooseM("OpenExp", null);       // This will clear debug log.

            return;
            

#if ENGLISH_VER
            //                txtemname.Text = "Experiment " + Convert.ToDateTime(DateTime.Now);
            txtemname.Text = "Experiment_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
#else
                //txtemname.Text = "实验" + Convert.ToDateTime(DateTime.Now);
                txtemname.Text = "实验_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
#endif

            gdA1.Text = "";
            gdA2.Text = "";
            gdA3.Text = "";
            gdA4.Text = "";
            gdA5.Text = "";
            gdA6.Text = "";
            gdA7.Text = "";
            gdA8.Text = "";
            gdB1.Text = "";
            gdB2.Text = "";
            gdB3.Text = "";
            gdB4.Text = "";
            gdB5.Text = "";
            gdB6.Text = "";
            gdB7.Text = "";
            gdB8.Text = "";

            chanonedes.Text = "";            
            chantwodes.Text = "";
            chanthreedes.Text = "";
            chanfourdes.Text = "";

            cboChan1.SelectedIndex = 0;
            cboChan2.SelectedIndex = 0;
            cboChan3.SelectedIndex = 0;
            cboChan4.SelectedIndex = 0;
        }
    }
}
