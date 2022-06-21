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
        private int index;
        List<int> cdlist;

        public ucTiaoShiOne()
        {
            InitializeComponent();
            this.Loaded += ucTiaoShiOne_Loaded;
            //this.Unloaded += ucTiaoShiTwo_Unloaded;

            index = 0;
        }

        void ucTiaoShiOne_Loaded(object sender, RoutedEventArgs e)
        {
            txtCycleState.Text = CommData.currCycleState.ToString();

            cboChansel.SelectedIndex = 0;
        }

        private string ChipNum = "Chip#1";

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "rbStart":
                    
                    if (index >= CommData.GetCycleNum(ChipNum))
                        index = 0;

                    cdlist = CommData.GetImgFrame(ChipNum, index);

                    int value = 0, row, col;

                    for (row = 0; row < CommData.imgFrame; row++)
                    {
                        for (col = 0; col < CommData.imgFrame; col++)
                        {
                            value = cdlist.ElementAt(row * 12 + col);

                            int g = (value - 100) / 5;
                            if (g < 0) g = 0;
                            else if (g > 255) g = 255;
                            byte gray = (byte)(g);

                            drawPix(col, row, gray);
                        }
                    }

                    List<string> stringlist = CommData.GetStringList(ChipNum, index);

                    //drawDP(5, 6, 0xff);
                    //drawDP(6, 5, 0xff);
                    //drawDP(6, 6, 0xff);

                    int n = stringlist.Count;

                    for (int i = 0; i < n; i++)
                    {
                        txtImg.AppendText(stringlist[i] + "\r\n");
                        txtImg.ScrollToEnd();
                    }

                    index++;
                    txtCycleNum.Text = index.ToString();

                    if (Start0K != null && false)
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
                    
                    cdlist = CommData.GetDP(ChipNum);

                    int nn = cdlist.Count / 2;

                    int step = 0;

                    for (int i = 0; i < nn; i++)
                    {

                        //drawDP(cdlist.ElementAt(step), cdlist.ElementAt(step + 1), 0xff);

                        drawDP(cdlist.ElementAt(step + 1), cdlist.ElementAt(step), 0xff);

                        step += 2;

                    }

                    
                    if (Start0K != null && false)
                    {
                        //Add By Liuhh 2019.03.25
                        try
                        {
                        //    if (MessageBox.Show("你确定要停止吗？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                        //        return;
                            if (MessageBox.Show("Confirm Force Stop?", "System Message", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                                return;

                            Start0K("rbClose", null);
                            //rbReadImg.IsEnabled = true;
                            //rbReadImg.Background = new SolidColorBrush(Color.FromRgb(7, 86, 187));
                            rbReadImg.Visibility = Visibility.Visible;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message, "System Message");
                        }
                           
                    }
                    break;
                case "rbhqjfsj":
                    if (Start0K != null)
                    {
                        CommData.experimentModelData.enAutoInt = false;             // automatically turn off auto int
                        Start0K("rbhqjfsj", null);
                    }
                    break;
                case "rbFindHID":
                    if (Start0K != null)
                    {
                        Start0K("rbFindHID", null);
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

        public void drawDP(int x, int y, byte gray)
        {
            Rectangle blueRectangle = new Rectangle();
            blueRectangle.Height = 10;
            blueRectangle.Width = 10;
            
            // Create a blue and a black Brush  
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Color.FromRgb(gray, gray, gray);
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.White;
            // Set Rectangle's width and color  
            //blueRectangle.StrokeThickness = 1;
            blueRectangle.Stroke = blueBrush;
            // Fill rectangle with blue color  
            // blueRectangle.Fill = blueBrush;
            // Add Rectangle to the Grid.  
            MyCanvas.Children.Add(blueRectangle);
            Canvas.SetLeft(blueRectangle, x * 10);
            Canvas.SetTop(blueRectangle, y * 10);
        }

        private void rbSaveImgData(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtImg.Text);
        }

        private void rbCopyPt(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtPt.Text);
        }

        private void rbCopyPi(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtPi.Text);
        }

        private void Chan_sel(object sender, SelectionChangedEventArgs e)
        {
            int cur_chan = cboChansel.SelectedIndex;

         //   if (cur_chan >= 0) CommData.curStdChan = cur_chan;      // Valid selection

         //   experiment exp = CommData.experimentModelData;
          //  if (String.IsNullOrEmpty(exp.chanonedes))
          //      return;

            switch (cur_chan)
            {
                case 0:
                    ChipNum = "Chip#1";
                    break;
                case 1:
                    ChipNum = "Chip#2";
                    break;
                case 2:
                    ChipNum = "Chip#3";
                    break;
                case 3:
                    ChipNum = "Chip#4";
                    break;
                default:
                    break;

            }
        }
    }
}
