#define ENGLISH_VER

using Microsoft.Win32;
using Newtonsoft.Json;
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
    /// ucReportOne.xaml 的交互逻辑
    /// </summary>
    public partial class ucReportOne : UserControl
    {
        private bool saveTemplate = false;

        public event EventHandler ChooseM;

        public ucReportOne()
        {
            InitializeComponent();
            this.Loaded += ucReportOne_Loaded;
            this.Unloaded += ucReportOne_Unloaded;
        }

        void ucReportOne_Loaded(object sender, RoutedEventArgs e)
        {
            string opName = CommData.experimentModelData.OpName;

            if(!String.IsNullOrEmpty(opName))
            {
                txtOpName.Text = opName;
            }

            string opNotes = CommData.experimentModelData.OpNotes;

            if (!String.IsNullOrEmpty(opNotes))
            {
                txtOpNotes.Text = opNotes;
            }

            // ==== Overwrite =====

            txtOpNotes.Text = CommData.PrintCSVReport();

            //===============

            byte[] ba = new byte[1];
            ba[0] = CommData.ver;
            string ascii = System.Text.Encoding.ASCII.GetString(ba);

            if(CommData.header == 0xa5)
            {
                // ascii = CommData.id_str;

                string mystr = CommData.id_str;

                ascii = mystr.TrimStart(new Char[] { '0' });
            }

            txtInstID.Text = ascii + "-" + CommData.sn1.ToString() + "-" + CommData.sn2.ToString();
            txtNumWell.Text = CommData.KsIndex.ToString() + "(" + CommData.well_format.ToString() + ")";
            txtNumChan.Text = CommData.TdIndex.ToString();
            txtExpName.Text = CommData.experimentModelData.emname;
            txtStartTime.Text = CommData.experimentModelData.emdatetime.ToString();
            txtEndTime.Text = CommData.experimentModelData.endatetime.ToString();
            txtMeltStartTime.Text = CommData.experimentModelData.meltemdatetime.ToString();
            txtMeltEndTime.Text = CommData.experimentModelData.meltendatetime.ToString();
            txtFileName.Text = CommData.openFileName;

            cboTmpl.IsChecked = false;
            saveTemplate = false;               // this is probably not necessary

            // txtOpNotes.Text = CommData.csvString; // Put summary here
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("1", null);
            }
        }

        private void SaveExp(object sender, RoutedEventArgs e)
        {
            CaptureFormValues();

            SaveFileDialog sfd = new SaveFileDialog();

#if ENGLISH_VER
            sfd.Filter = "Json doc(*.Json)|*.Json|All files|*.*";//设置文件类型
#else
            sfd.Filter = "Json文件(*.Json)|*.Json|所有文件|*.*";//设置文件类型
#endif

            //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
            if(!saveTemplate)
                sfd.FileName = CommData.experimentModelData.emname;

            sfd.DefaultExt = "Json";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名

            if (sfd.ShowDialog() == true && CommData.experimentModelData.DebugModelDataList != null)
            {
                string path = sfd.FileName;

                if (!saveTemplate)
                {
                    string jsonstring = JsonConvert.SerializeObject(CommData.experimentModelData);
                    System.IO.File.WriteAllText(path, jsonstring);
                }
                else
                {
                    experiment exp_clone = CommData.experimentModelData.ShallowCopy();

                    exp_clone.ImgFileName = null;
                    exp_clone.ImgFileName2 = null;
                    exp_clone.SetFileName = null;

                    Dictionary<string, List<string>> empty_diclist = new Dictionary<string, List<string>>();
                    exp_clone.ampData = empty_diclist;
                    exp_clone.meltData = empty_diclist;
                    exp_clone.DebugLog = null;

                    string jsonstring = JsonConvert.SerializeObject(exp_clone);
                    System.IO.File.WriteAllText(path, jsonstring);
                }

#if ENGLISH_VER
                if (!saveTemplate)
                {
                    MessageBox.Show("Experiment file saved.");
                    CommData.expSaved = true;
                }
                else
                {
                    MessageBox.Show("Template file saved.");
                }
            }
            else
            {
                MessageBox.Show("File NOT saved.");
            }
#else

            MessageBox.Show("文件保存成功");
            }
            else
            {
                MessageBox.Show("实验还没有设置，文件未存");
            }
#endif
            /*          
                if (CommData.experimentModelData.emname != null)
                {
                    bool success;
                    success = CommData.AddExperiment(CommData.experimentModelData);
                    CommData.expSaved = true;
                    if(success) MessageBox.Show("实验文件已存");
                }
                else
                {
                    MessageBox.Show("实验名为空， 未存");
                }
            */

        }

        void ucReportOne_Unloaded(object sender, RoutedEventArgs e)
        {
            CaptureFormValues();
        }

        private void PrintReport(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("1", null);
            }
        }

        private void CaptureFormValues()
        {
            CommData.experimentModelData.OpName = txtOpName.Text;
            CommData.experimentModelData.OpNotes = txtOpNotes.Text;
        }

        private void SaveAssay(object sender, RoutedEventArgs e)
        {
            CommData.assayData.assayId = 1;
            CommData.assayData.assayName = "Legionella";
            CommData.assayData.assayVersion = "V1";
            CommData.assayData.cyclerParam.predenatureTemp = 95;
            CommData.assayData.cyclerParam.predenatureTime = 120;
            CommData.assayData.cyclerParam.numCycles = 40;
            CommData.assayData.cyclerParam.denatureTemp = 95;
            CommData.assayData.cyclerParam.denatureTime = 10;
            CommData.assayData.cyclerParam.annealingTemp = 60;
            CommData.assayData.cyclerParam.annealingTime = 15;
            CommData.assayData.cyclerParam.extensionEn = false;
            CommData.assayData.cyclerParam.meltEn = false;

            List<ChannelParam> chlist = new List<ChannelParam>();

            ChannelParam ch1 = new ChannelParam();
            ChannelParam ch2 = new ChannelParam();
            ChannelParam ch3 = new ChannelParam();
            ChannelParam ch4 = new ChannelParam();

            ch1.active = true;
            ch1.name = "Legionella species";
            ch1.type = "Target";
            ch1.finalUnits = "cells";
            ch1.negCtrlOKCt = 37;
            ch1.description = "Chan1";

            ch3.active = true;
            ch3.name = "Super IC";
            ch3.type = "IC";
            ch3.description = "Chan3";
            ch3.posCtrlOKCtStart = 32;
            ch3.posCtrlOKCtEnd = 36;

            chlist.Add(ch1);
            chlist.Add(ch2);
            chlist.Add(ch3);
            chlist.Add(ch4);

            CommData.assayData.channelParamLists = chlist;

            SaveFileDialog sfd = new SaveFileDialog();
#if ENGLISH_VER
            sfd.Filter = "Json doc(*.Json)|*.Json|All files|*.*";
#else
            sfd.Filter = "Json文件(*.Json)|*.Json|所有文件|*.*";
#endif
            sfd.FileName = "MyAssay"; //设置默认文件名
            sfd.DefaultExt = "Json";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名

            if (sfd.ShowDialog() == true)
            {
                string path = sfd.FileName;
                string jsonstring = JsonConvert.SerializeObject(CommData.assayData);
                System.IO.File.WriteAllText(path, jsonstring);
#if ENGLISH_VER
                MessageBox.Show("Assay file saved.");
            }
            else
            {
                MessageBox.Show("Assay file NOT saved.");
            }
#else
                MessageBox.Show("文件保存成功");
            }
            else
            {
                MessageBox.Show("未存");
            }
#endif
        }

        private void PrintReportCSV(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("2", null);
            }
        }

        private void cboTmpl_Checked(object sender, RoutedEventArgs e)
        {
            saveTemplate = true;
        }

        private void cboTmpl_Unchecked(object sender, RoutedEventArgs e)
        {
            saveTemplate = false;
        }
    }
}
