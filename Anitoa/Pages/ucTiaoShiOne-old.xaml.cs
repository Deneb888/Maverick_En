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
    /// ucTiaoShiOne.xaml 的交互逻辑
    /// </summary>
    public partial class ucTiaoShiOne : UserControl
    {
        public event EventHandler Start0K;
        public ucTiaoShiOne()
        {
            InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "rbStart":
                    if (Start0K != null)
                    {
                       
                        Start0K("rbStart", null);
                        //rbReadImg.IsEnabled = false;
                        //rbReadImg.Background = new SolidColorBrush(Color.FromRgb(115, 117, 120));
                        rbReadImg.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "rbReadImg":
                    if (Start0K != null)
                    {
                        Start0K("rbReadImg", null);
                    }
                    break;
                case "rbClose":
                    if (Start0K != null)
                    {
                       
                        Start0K("rbClose", null);
                        //rbReadImg.IsEnabled = true;
                        //rbReadImg.Background = new SolidColorBrush(Color.FromRgb(7, 86, 187));
                        rbReadImg.Visibility = Visibility.Visible;
                    }
                    break;
                case "rbhqjfsj":
                    if (Start0K != null)
                    {
                        CommData.experimentModelData.enAutoInt = false;
                        Start0K("rbhqjfsj", null);
                    }
                    break;
            }
        }

        public void Clear()
        {
            txtPt.Text = "";
            txtPi.Text = "";
            txtImg.Text = "";
        }

        private void rbClear_Click(object sender, RoutedEventArgs e)
        {
            txtPi.Text = "";
            txtPt.Text = "";
        }

        private void rbClearBuff_Click(object sender, RoutedEventArgs e)
        {
            txtImg.Text = "";

            MyCanvas.Children.Clear();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbReadImg_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbStart_Checked(object sender, RoutedEventArgs e)
        {

        }
        public void drawPix(int x, int y, byte gray)
        {
            // MyCanvas.InvalidateMeasure();

            Rectangle blueRectangle = new Rectangle();
            blueRectangle.Height = 10;
            blueRectangle.Width = 10;
            // Create a blue and a black Brush  
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Color.FromRgb(gray, gray, gray);
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            // Set Rectangle's width and color  
            //blueRectangle.StrokeThickness = 1;
            blueRectangle.Stroke = blueBrush;
            // Fill rectangle with blue color  
            blueRectangle.Fill = blueBrush;
            // Add Rectangle to the Grid.  
            MyCanvas.Children.Add(blueRectangle);
            Canvas.SetLeft(blueRectangle, x * 10);
            Canvas.SetTop(blueRectangle, y * 10);
        }
    }
}
