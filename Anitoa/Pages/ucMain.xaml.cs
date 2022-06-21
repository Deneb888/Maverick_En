// #define Lumin_Lite

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
    /// ucMain.xaml 的交互逻辑
    /// </summary>
    public partial class ucMain : UserControl
    {
        public event EventHandler rbChecked;
        public ucMain()
        {
            InitializeComponent();
            this.Loaded += ucMain_Loaded;
        }

        void ucMain_Loaded(object sender, RoutedEventArgs e)
        {
            SetTSVisbile();

           
        }

        private void SetTSVisbile()
        {
#if (!Lumin_Lite)
            if (CommData.user != null)
            {
                // xitongtiaoshi.Visibility = CommData.user.Utype == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
#endif
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "yhsetting":
                    if (rbChecked != null)
                    {
                        rbChecked("1", null);
                    }
                    break;
                case "fenxidata":
                    if (rbChecked != null)
                    {
                        rbChecked("2", null);
                    }
                    break;
                case "baogaodayin":
                    if (rbChecked != null)
                    {
#if (!Lumin_Lite)
                        rbChecked("3", null);
#else
                        rbChecked("1", null);
                        CommData.load_template = true;
#endif
                    }
                    break;
                case "xitongtiaoshi":
                    if (rbChecked != null)
                    {
                        rbChecked("4", null);
                    }
                    break;
            }
        }

        private void yhsetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border rb = sender as Border;
            switch (rb.Name)
            {
                case "yhsetting":
                    if (rbChecked != null)
                    {
                        rbChecked("1", null);
                    }
                    break;
                case "fenxidata":
                    if (rbChecked != null)
                    {
                        rbChecked("2", null);
                    }
                    break;
                case "baogaodayin":
                    if (rbChecked != null)
                    {
#if (!Lumin_Lite)
                        rbChecked("3", null);
#else
                        CommData.load_template = true;
                        rbChecked("1", null);
#endif
                    }
                    break;
                case "xitongtiaoshi":
                    if (rbChecked != null)
                    {
                        rbChecked("4", null);
                    }
                    break;
                case "runexp":
                    if (rbChecked != null)
                    {
                        rbChecked("15", null);
                    }
                    break;
            }
        }
    }
}
