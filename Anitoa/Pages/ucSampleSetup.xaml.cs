using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anitoa.Pages
{
    public partial class ucSampleSetup : Window
    {
        public ucSampleSetup()
        {
            InitializeComponent();
            this.Loaded += ucSampleSetup_Loaded;
        }

        void ucSampleSetup_Loaded(object sender, RoutedEventArgs e)
        {
            //ComboBoxItem cbi = new ComboBoxItem();
            //cbi.Content = "dfdfgd";
            //cboAssay.Items.Add(cbi);

            int n = CommData.assayList.Count();

            for(int i=0; i<n; i++)
            {
                ComboBoxItem cboi = new ComboBoxItem();
                cboi.Content = CommData.assayList[i].assayName;
                cboAssay.Items.Add(cboi);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void cbTypeChange(object sender, SelectionChangedEventArgs e)
        {
            int cur_type = cboType.SelectedIndex;

            if (cboQuantUnit == null || txtQuant == null)
                return;

            if(cur_type < 3)
            {
                cboQuantUnit.IsEnabled = false;
                cboQuantUnit.Opacity = 0.6;

                txtQuant.IsEnabled = false;
                txtQuant.Opacity = 0.6;

                lbQuantity.Opacity = 0.6;
                lbUnits.Opacity = 0.6;
            }
            else
            {
                cboQuantUnit.IsEnabled = true;
                cboQuantUnit.Opacity = 1.0;

                txtQuant.IsEnabled = true;
                txtQuant.Opacity = 1.0;

                lbQuantity.Opacity = 1.0;
                lbUnits.Opacity = 1.0;
            }
        }
    }
}
