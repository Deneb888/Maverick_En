using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anitoa.Pages
{
    public partial class ucAssaySetup : Window
    {
        public ucAssaySetup()
        {
            InitializeComponent();
            this.Loaded += ucAssaySetup_Loaded;
        }

        void ucAssaySetup_Loaded(object sender, RoutedEventArgs e)
        {
            //ComboBoxItem cbi = new ComboBoxItem();
            //cbi.Content = "dfdfgd";
            //cboAssay.Items.Add(cbi);

            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
