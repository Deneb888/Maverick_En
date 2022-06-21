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
    /// ucReportTWO.xaml 的交互逻辑
    /// </summary>
    public partial class ucReportTWO : UserControl
    {
        public event EventHandler ChooseM;
        public ucReportTWO()
        {
            InitializeComponent();
            this.Loaded += ucReportTWO_Loaded;
        }

        void ucReportTWO_Loaded(object sender, RoutedEventArgs e)
        {
            radChart.Animate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("1", null);
            }
        }
    }
}
