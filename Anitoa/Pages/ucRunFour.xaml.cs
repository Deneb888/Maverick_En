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
    /// ucRunFour.xaml 的交互逻辑
    /// </summary>
    public partial class ucRunFour : UserControl
    {
        public event EventHandler ChooseM;
        public ucRunFour()
        {
            InitializeComponent();
            this.Loaded += ucRunFour_Loaded;
        }

        void ucRunFour_Loaded(object sender, RoutedEventArgs e)
        {
            radChart.Animate();
            radChart1.Animate();
            if (CommData.experimentModelData != null)
            {
                gdA1.Content = CommData.experimentModelData.A1des;
                gdA2.Content = CommData.experimentModelData.A2des;
                gdA3.Content = CommData.experimentModelData.A3des;
                gdA4.Content = CommData.experimentModelData.A4des;
                gdA5.Content = CommData.experimentModelData.A5des;
                gdA6.Content = CommData.experimentModelData.A6des;
                gdA7.Content = CommData.experimentModelData.A7des;
                gdA8.Content = CommData.experimentModelData.A8des;
                gdB1.Content = CommData.experimentModelData.B1des;
                gdB2.Content = CommData.experimentModelData.B2des;
                gdB3.Content = CommData.experimentModelData.B3des;
                gdB4.Content = CommData.experimentModelData.B4des;
                gdB5.Content = CommData.experimentModelData.B5des;
                gdB6.Content = CommData.experimentModelData.B6des;
                gdB7.Content = CommData.experimentModelData.B7des;
                gdB8.Content = CommData.experimentModelData.B8des;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("1", null);
            }
        }

        private void gdA1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void gdA1_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
