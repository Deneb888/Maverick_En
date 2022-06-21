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
    /// ucWenDuItem.xaml 的交互逻辑
    /// </summary>
    public partial class ucWenDuItem : UserControl
    {
        Line arrow;
        public ucWenDuItem()
        {
            InitializeComponent();
           
            arrow = new Line();
            arrow.Y2 = 35;
            arrow.X1 = (double)r1.GetValue(Canvas.LeftProperty);
            arrow.Y1 = (double)r1.GetValue(Canvas.TopProperty) + r1.Height / 2;
            arrow.Stroke = Brushes.Red;
            arrow.StrokeThickness = 1;
            canvas.Children.Add(arrow);
        }

        bool trackingMouseMove = false;
        Point mousePosition;
        private void ellipse1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            mousePosition = e.GetPosition(null);
            double deltaH = e.GetPosition(null).X - arrow.X1;
            trackingMouseMove = true;
            if (null != element)
            {
                element.CaptureMouse();
                element.Cursor = Cursors.Hand;
            }

        }

        private void ellipse1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            trackingMouseMove = false;
            element.ReleaseMouseCapture();

            mousePosition.X = mousePosition.Y = 0;
            element.Cursor = null;
        }

        private void ellipse1_MouseMove(object sender, MouseEventArgs e)
        {


            FrameworkElement element = sender as FrameworkElement;
            if (trackingMouseMove)
            {
                double deltaV = e.GetPosition(null).Y - mousePosition.Y;
                double deltaH = e.GetPosition(null).X - mousePosition.X;
                double newTop = deltaV + (double)element.GetValue(Canvas.TopProperty);
                double newLeft = deltaH + (double)element.GetValue(Canvas.LeftProperty);

                element.SetValue(Canvas.TopProperty, newTop);
                element.SetValue(Canvas.LeftProperty, newLeft);
                if (element.Name == "r1")
                {

                    arrow.X1 = arrow.X1 + deltaH;
                    arrow.Y1 = arrow.Y1 + deltaV;
                }
                mousePosition = e.GetPosition(null);
            }
        }
    }
}
