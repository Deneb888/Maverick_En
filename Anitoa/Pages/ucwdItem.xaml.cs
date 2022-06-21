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
    /// ucwdItem.xaml 的交互逻辑
    /// </summary>
    public partial class ucwdItem : UserControl
    {
        private double currswd = 0;
        private double currewd = 0;

        private bool flag = false;
        public event EventHandler TempChanaged;
        public double currExecutetime = 0;

        private bool lockTemp = false, lockTime = false;

        public ucwdItem(double startwd, double  endwd ,double excutetime)
        {
            InitializeComponent();
            this.currswd = startwd;
            this.currewd = endwd;
            currExecutetime = excutetime;
            this.Loaded += ucwdItem_Loaded;
        }

        public ucwdItem(double startwd, double endwd, double excutetime, bool ltmp, bool ltime)
        {
            InitializeComponent();
            this.currswd = startwd;
            this.currewd = endwd;
            currExecutetime = excutetime;
            this.Loaded += ucwdItem_Loaded;

            lockTemp = ltmp;
            lockTime = ltime;
        }

        void ucwdItem_Loaded(object sender, RoutedEventArgs e)
        {
            slider.Value = currewd;

            txtwd.Text = currewd.ToString("0.0") ;
            txttime.Text = currExecutetime.ToString();

            BindDraw();
            flag = true;

            if (lockTime)
                txttime.IsEnabled = false;

            if (lockTemp)
                txtwd.IsEnabled = false;
        }

        private void BindDraw()
        {          
            double wd = 300 - (currswd - 9) * 3.3;
            Canvas.SetTop(bd1, wd);           

            Canvas.SetTop(bd2, 300 - (currewd - 9) * 3.3);
            Canvas.SetTop(spWD, 300 - (currewd - 9) * 3.3 - 23);
            Canvas.SetTop(spSJ, 300 - (currewd - 9) * 3.3 + 3);

            Point Point1 = new Point(Canvas.GetLeft(bd1), Canvas.GetTop(bd1));
            Point Point2 = new Point(Canvas.GetLeft(bd2), Canvas.GetTop(bd2));
         
            line.X1 = Point1.X;
            line.Y1 = Point1.Y;

            line.X2 = Point2.X;
            line.Y2 = Point2.Y;
        }


        public void BindDrawNew(double starttemp)
        {
            currswd = starttemp;
            flag = false;
            double wd = 300 - (starttemp - 9) * 3.3;
            Canvas.SetTop(bd1, wd);        

            Point Point1 = new Point(Canvas.GetLeft(bd1), Canvas.GetTop(bd1));
            Point Point2 = new Point(Canvas.GetLeft(bd2), Canvas.GetTop(bd2));

            line.X1 = Point1.X;
            line.Y1 = Point1.Y;

            line.X2 = Point2.X;
            line.Y2 = Point2.Y;

            flag = true;
        }

        public void BindDrawNew2(double temp)
        {
            slider.Value = temp;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //currewd = slider.Value * 3;
            //BindDraw();

            if (flag == false) return;
//            txtwd.Text = Convert.ToInt32(slider.Value).ToString();

            txtwd.Text = slider.Value.ToString("0.0");

            //Canvas.SetTop(spWD, 300 - Convert.ToDouble(slider.Value.ToString("0.0")) * 3 - 20);           // original
            //Canvas.SetTop(spSJ, 300 - Convert.ToDouble(slider.Value.ToString("0.0")) * 3 + 5); 
            //Canvas.SetTop(bd2, 300 - slider.Value * 3+2);

            Canvas.SetTop(spWD, 300 - (Convert.ToDouble(slider.Value.ToString("0.0")) - 9) * 3.3 - 23);             // slider range is 9 to 99. So range of 90 correspond to 300
            Canvas.SetTop(spSJ, 300 - (Convert.ToDouble(slider.Value.ToString("0.0")) - 9) * 3.3 + 3);
            Canvas.SetTop(bd2, 300 - (slider.Value - 9) * 3.3);

            Point Point1 = new Point(Canvas.GetLeft(bd1), Canvas.GetTop(bd1));
            Point Point2 = new Point(Canvas.GetLeft(bd2), Canvas.GetTop(bd2));

            line.X1 = Point1.X;
            line.Y1 = Point1.Y;

            line.X2 = Point2.X;
            line.Y2 = Point2.Y;

            if (TempChanaged != null)
            {
                TempChanaged(slider.Value, null);
            }

            txtwd.SelectionStart = txtwd.Text.Length;
        }

        private void txtwd_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtwd.Text.Trim()))
                    return;

                //string wd = txtwd.Text.Trim().Replace("°C", "");

                //slider.Value = Convert.ToDouble(txtwd.Text);        

                if (slider.Value < 40 && lockTime)       // this is RJQX
                {
                    MessageBox.Show("Melt temperature cannot be less than 40 °C.");

                    slider.Value = 40;
                }
            }
            catch (Exception ex)
            {
                
            }         
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (string.IsNullOrEmpty(txtwd.Text.Trim())) return;
                //string wd = txtwd.Text.Trim().Replace("°C", "");
                slider.Value = Convert.ToDouble(txtwd.Text.Trim());
            }
        }

        private void txttime_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(txttime.Text.Trim()))
                //    return;

                //string wd = txtwd.Text.Trim().Replace("°C", "");

                //slider.Value = Convert.ToDouble(txtwd.Text);        

                double time = Convert.ToDouble(txttime.Text);

                if (time < 0.99)
                {
                    MessageBox.Show("Time cannot be zero or negative.");
                    txttime.Text = "1";
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OnLostFocusHandler(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtwd.Text.Trim())) return;
            //string wd = txtwd.Text.Trim().Replace("°C", "");
            slider.Value = Convert.ToDouble(txtwd.Text.Trim());
        }
    }
}
