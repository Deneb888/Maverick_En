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
    /// ucTiaoShiThree.xaml 的交互逻辑
    /// </summary>
    public partial class ucTiaoShiThree : UserControl
    {
        public event EventHandler SaveConfigOK;
        public ucTiaoShiThree()
        {
            InitializeComponent();
            this.Loaded += ucTiaoShiThree_Loaded;
            this.Unloaded += ucTiaoShiThree_Unloaded;
        }

        void ucTiaoShiThree_Unloaded(object sender, RoutedEventArgs e)
        {
            if (CommData.currCycleState != 0) return;
            SaveConfig();
        }

        void ucTiaoShiThree_Loaded(object sender, RoutedEventArgs e)
        {
            rbjd3.IsChecked = true;
            if (CommData.currCycleState != 0) return;
            SaveConfig();

            // txtCrossTalk12.Text = (100 * CommData.experimentModelData.crossTalk12).ToString("0.0");

            txtOverShootTemp.Text = CommData.experimentModelData.overTemp.ToString("0.0");
            txtUnderShootTemp.Text = CommData.experimentModelData.underTemp.ToString("0.0");
            txtOverShootTime.Text = CommData.experimentModelData.overTime.ToString("0.0");
            txtUnderShootTime.Text = CommData.experimentModelData.underTime.ToString("0.0");
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            switch (rb.Name)
            {
                case "rbSave":
                    SaveConfig();
                    // CommData.AddExperiment(CommData.experimentModelData);
                    break;
                case "rbReadTemp":
                    if (SaveConfigOK != null)
                    {
                        SaveConfigOK("ReadTemp", null);
                    }
                    break;
                case "rbOverShoot":
                    if (SaveConfigOK != null)
                    {
                        SaveConfigOK("OverShoot", null);
                    }
                    break;
            }
        }

        public void SetPTTemp()
        { 
        
        }


        public void SetPITemp()
        {

        }

        private void SaveConfig()
        {
            try
            {
                DebugModelData DebugModelData = new DebugModelData();
                //DebugModelData.Annealing = Convert.ToDouble(txtAnnealing.Text);
                //DebugModelData.AnnealingTime = Convert.ToDouble(txtAnnealingTime.Text);
                //DebugModelData.Cycle = Convert.ToInt32(txtCycle.Text);
                //DebugModelData.Denaturating = Convert.ToDouble(txtDenaturating.Text);
                //DebugModelData.DenaturatingTime = Convert.ToDouble(txtDenaturatingTime.Text);
                //DebugModelData.Extension = Convert.ToDouble(txtExtension.Text);
                //DebugModelData.ExtensionTime = Convert.ToDouble(txtExtensionTime.Text);
                //DebugModelData.Holdon = Convert.ToDouble(txtHoldon.Text);
                //DebugModelData.HoldonTime = Convert.ToDouble(txtHoldonTime.Text);
                //DebugModelData.Hotlid = Convert.ToDouble(txtHotlid.Text);
                //DebugModelData.InitaldenaTime = Convert.ToDouble(txtInitaldenaTime.Text);
                //DebugModelData.Initaldenaturation = Convert.ToDouble(txtInitaldenaturation.Text);
                //DebugModelData.StepCount = 3;
                //DebugModelData.InitDenatureStepCount = 1;

                if (SaveConfigOK != null)
                {
                    // SaveConfigOK(DebugModelData, null);              // 12-25-2020 Change by Zhimin, do nothing
                }
            }
            catch (Exception ex)
            {

            }          
        }

        private void rbjd1_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch(rb.Name)
            {
                case "rbjd1":
                    CommData.CurrPZJD = 1;
                    break;
                case "rbjd2":
                    CommData.CurrPZJD = 2;
                    break;
                case "rbjd3":
                    CommData.CurrPZJD = 3;
                    break;
            }
        }

        public void Clear()
        {
            txtPt.Text = "";
            txtPi.Text = "";
        }

        private void rbClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
    }
}
