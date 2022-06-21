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
    /// ucTiaoShiTwo.xaml 的交互逻辑
    /// </summary>
    public partial class ucTiaoShiTwo : UserControl
    {
        public event EventHandler SetParametersOK;
        public event EventHandler Load_Flash;

        public ucTiaoShiTwo()
        {
            InitializeComponent();
            this.Loaded += ucTiaoShiTwo_Loaded;
            this.Unloaded += ucTiaoShiTwo_Unloaded;
        }

        void ucTiaoShiTwo_Loaded(object sender, RoutedEventArgs e)
        {
            //rbo24F.IsChecked = true;

            txtCrossTalk12.Text = (100 * CommData.experimentModelData.crossTalk12).ToString("0.0");
            txtCrossTalk21.Text = (100 * CommData.experimentModelData.crossTalk21).ToString("0.0");

            txtCrossTalk32.Text = (100 * CommData.experimentModelData.crossTalk32).ToString("0.0");
            txtCrossTalk23.Text = (100 * CommData.experimentModelData.crossTalk23).ToString("0.0");

            txtCrossTalk34.Text = (100 * CommData.experimentModelData.crossTalk34).ToString("0.0");
            txtCrossTalk43.Text = (100 * CommData.experimentModelData.crossTalk43).ToString("0.0");

            txtConfiTh.Text = (100 * CommData.experimentModelData.confiTh).ToString("0.0");
            txtAmpEffTh.Text = (100 * CommData.experimentModelData.ampEffTh).ToString("0.0");
            txtSnRTh.Text = (100 * CommData.experimentModelData.snrTh).ToString("0.0");

            //ComboBoxItem cbi = cboGainChan1.SelectedItem as ComboBoxItem;
            cboGainChan1.SelectedIndex = CommData.experimentModelData.gainMode[0];
            cboGainChan2.SelectedIndex = CommData.experimentModelData.gainMode[1];
            cboGainChan3.SelectedIndex = CommData.experimentModelData.gainMode[2];
            cboGainChan4.SelectedIndex = CommData.experimentModelData.gainMode[3];

            txtITChan1.Text = CommData.int_time_1.ToString("0.0");
            txtITChan2.Text = CommData.int_time_2.ToString("0.0");
            txtITChan3.Text = CommData.int_time_3.ToString("0.0");
            txtITChan4.Text = CommData.int_time_4.ToString("0.0");

            txtStartCycle.Text = CommData.experimentModelData.curfitStartCycle.ToString();
            txtMinCt.Text = CommData.experimentModelData.curfitMinCt.ToString();
            txtCtTh.Text = (CommData.experimentModelData.curfitCtTh * 100).ToString("0.0");

            cboChan1.IsChecked = CommData.experimentModelData.CbooChan1;
            cboChan2.IsChecked = CommData.experimentModelData.CbooChan2;
            cboChan3.IsChecked = CommData.experimentModelData.CbooChan3;
            cboChan4.IsChecked = CommData.experimentModelData.CbooChan4;

            cboLabel.IsChecked = CommData.showCrosshairLabel;
            cboCtCrosshair.IsChecked = CommData.showCtCrosshair;
        }

        void ucTiaoShiTwo_Unloaded(object sender, RoutedEventArgs e)
        {          
            CommData.experimentModelData.gainMode[0] = cboGainChan1.SelectedIndex;
            CommData.experimentModelData.gainMode[1] = cboGainChan2.SelectedIndex;
            CommData.experimentModelData.gainMode[2] = cboGainChan3.SelectedIndex;
            CommData.experimentModelData.gainMode[3] = cboGainChan4.SelectedIndex;

            CommData.experimentModelData.curfitStartCycle = Convert.ToInt32(txtStartCycle.Text.ToString());
            CommData.experimentModelData.curfitMinCt = Convert.ToInt32(txtMinCt.Text.ToString());
            CommData.experimentModelData.curfitCtTh = 0.01 * Convert.ToDouble(txtCtTh.Text.ToString());

            CommData.experimentModelData.CbooChan1 = cboChan1.IsChecked.Value;
            CommData.experimentModelData.CbooChan2 = cboChan2.IsChecked.Value;
            CommData.experimentModelData.CbooChan3 = cboChan3.IsChecked.Value;
            CommData.experimentModelData.CbooChan4 = cboChan4.IsChecked.Value;

            if (SetParametersOK != null)
            {
                SetParametersOK("SetParameters", e);                
            }
        }

            private void cboChan1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb.Name)
            {
                case "cboChan1":
                    CommData.cboChan1 = 1;
                    break;
                case "cboChan2":
                    CommData.cboChan2 = 1;
                    break;
                case "cboChan3":
                    CommData.cboChan3 = 1;
                    break;
                case "cboChan4":
                    CommData.cboChan4 = 1;
                    break;
            }
        }

        private void cboChan1_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb.Name)
            {
                case "cboChan1":
                    CommData.cboChan1 = 0;
                    break;
                case "cboChan2":
                    CommData.cboChan2 = 0;
                    break;
                case "cboChan3":
                    CommData.cboChan3 = 0;
                    break;
                case "cboChan4":
                    CommData.cboChan4 = 0;
                    break;
            }
        }

        private void rbo12F_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch(rb.Name)
            {
                case "rbo12F":
                    CommData.imgFrame = 12;
                    break;
                case "rbo24F":
                    CommData.imgFrame = 24;
                    break;
            }
        }

        private void cboIG_Click(object sender, RoutedEventArgs e)
        {
            CommData.gain_mode = rboLG.IsChecked == true ? 1 : 0;

            CommData.experimentModelData.gainMode[0] = cboGainChan1.SelectedIndex;
            CommData.experimentModelData.gainMode[1] = cboGainChan2.SelectedIndex;
            CommData.experimentModelData.gainMode[2] = cboGainChan3.SelectedIndex;
            CommData.experimentModelData.gainMode[3] = cboGainChan4.SelectedIndex;
       
            if (SetParametersOK!=null)
            {
                SetParametersOK("SetParameters", e);
                MessageBox.Show("Parameter changes commited");
            }
        }

        private void rboLG_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton RadioButton = sender as RadioButton;
            switch (RadioButton.Name)
            {
                case "rboLG":
                    CommData.gain_mode = 1;
                    break;
                case "rboHG":
                    CommData.gain_mode = 0;
                    break;
            }
        }

        private void LoadFlash(object sender, RoutedEventArgs e)
        {
            if (Load_Flash != null)
            {
                Load_Flash("LoadFlash", e);
            }
        }

        private void cboRaw_Checked(object sender, RoutedEventArgs e)
        {
            CommData.rawData = true;
        }

        private void cboRaw_Unchecked(object sender, RoutedEventArgs e)
        {
            CommData.rawData = false;
        }

        private void cboNoCt_Checked(object sender, RoutedEventArgs e)
        {
            CommData.noCt = true;
        }

        private void cboNoCt_Unchecked(object sender, RoutedEventArgs e)
        {
            CommData.noCt = false;
        }

        private void cboNoDark_Checked(object sender, RoutedEventArgs e)
        {
            CommData.noDarkCorrect = true;
        }

        private void cboNoDark_Unchecked(object sender, RoutedEventArgs e)
        {
            CommData.noDarkCorrect = false;
        }

        private void cboGap_Checked(object sender, RoutedEventArgs e)
        {
            CommData.noGapRemove = true;
        }

        private void cboGap_Unchecked(object sender, RoutedEventArgs e)
        {
            CommData.noGapRemove = false;
        }

        private void cboDark_Checked(object sender, RoutedEventArgs e)
        {
            CommData.showDarkCurve = true;                      
        }

        private void cboDark_Unchecked(object sender, RoutedEventArgs e)
        {
            CommData.showDarkCurve = false;
        }

        private void cboCtCrosshair_Checked(object sender, RoutedEventArgs e)
        {
            CommData.showCtCrosshair = true;
        }

        private void cboCtCrosshair_Unchecked(object sender, RoutedEventArgs e)
        {
            CommData.showCtCrosshair = false;
        }

        private void cboLabel_Checked(object sender, RoutedEventArgs e)
        {
            CommData.showCrosshairLabel = true;
        }

        private void cboLabel_Unchecked(object sender, RoutedEventArgs e)
        {
            CommData.showCrosshairLabel = false;
        }
    }
}
