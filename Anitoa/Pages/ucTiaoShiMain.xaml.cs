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
    /// ucTiaoShiMain.xaml 的交互逻辑
    /// </summary>
    public partial class ucTiaoShiMain : UserControl
    {
        public event EventHandler rbChecked;
        public ucTiaoShiMain()
        {
            InitializeComponent();
            this.Loaded += ucTiaoShiMain_Loaded;
        }

        void ucTiaoShiMain_Loaded(object sender, RoutedEventArgs e)
        {
            rbone.IsChecked = true;
        }

        private void rbwdxl_Checked(object sender, RoutedEventArgs e)
        {
            gdMain.Children.Clear();
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "rbone":
                    ucTiaoShiOne ucTiaoShiOne = new ucTiaoShiOne();
                    gdMain.Children.Add(ucTiaoShiOne);
                    break;
                case "rbtwo":
                    ucTiaoShiTwo ucTiaoShiTwo = new ucTiaoShiTwo();
                    gdMain.Children.Add(ucTiaoShiTwo);
                    break;
                case "rbthree":
                    ucTiaoShiThree ucTiaoShiThree = new ucTiaoShiThree();
                    gdMain.Children.Add(ucTiaoShiThree);
                    break;
            }
        }
    }
}
