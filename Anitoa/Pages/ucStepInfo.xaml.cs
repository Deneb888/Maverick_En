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
    /// ucStepInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ucStepInfo : UserControl
    {
        public int currCycle = 1;
        public event EventHandler AddStep;
        public event EventHandler DelStep;
        public event EventHandler TempChanaged;
        public double currswd = 0;
        public double currewd = 0;
        public int currType = 0;
        ucwdItem ucwdItem = null;
        public string bzstr = "";
        public double currExecutetime = 0;

        private bool lockTemp = false, lockTime = false;

        public ucStepInfo(double startwd, double endwd, int type, double executeTime, bool ltmp, bool ltime)
        {
            InitializeComponent();
            this.currswd = startwd;
            this.currewd = endwd;
            currType = type;
            currExecutetime = executeTime;
            this.Loaded += ucStepInfo_Loaded;

            lockTemp = ltmp;
            lockTime = ltime;
        }

        public ucStepInfo(double startwd, double endwd, int type, double executeTime)
        {
            InitializeComponent();
            this.currswd = startwd;
            this.currewd = endwd;
            currType = type;
            currExecutetime = executeTime;
            this.Loaded += ucStepInfo_Loaded;
        }

        void ucStepInfo_Loaded(object sender, RoutedEventArgs e)
        {
            bzstr = Guid.NewGuid().ToString();
            bdAdd.Visibility = bdRemove.Visibility = currType == 0 ? Visibility.Collapsed : Visibility.Visible;

            if(lockTime)
                ucwdItem = new ucwdItem(currswd, currewd, currExecutetime, false, true);
            else
                ucwdItem = new ucwdItem(currswd, currewd, currExecutetime);

            ucwdItem.Tag = "0";
            ucwdItem.TempChanaged += ucwdItem_TempChanaged;
            gdMain.Children.Add(ucwdItem);
        }

        void ucwdItem_TempChanaged(object sender, EventArgs e)
        {
            if (TempChanaged != null)
            {
                List<string> list = new List<string>();
                list.Add(this.Tag.ToString());
                list.Add(sender.ToString());
                list.Add(bzstr);
                currewd = Convert.ToDouble(sender);
                TempChanaged(list, e);
            }
        }

        public void SetTemp(double temp)
        {
            currswd = temp;
            ucwdItem.BindDrawNew(temp);
        }

        public void SetTemp2(double temp)
        {
            currswd = temp;
            ucwdItem.BindDrawNew2(temp);
        }

        private void gdMain_MouseMove(object sender, MouseEventArgs e)
        {
            gdMain.Background = new SolidColorBrush(Color.FromRgb(230,230,240));
        }

        private void gdMain_MouseLeave(object sender, MouseEventArgs e)
        {
            gdMain.Background = Brushes.White;
        }

        private void bdAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border bd = sender as Border;
            switch (bd.Name)
            {
                case "bdAdd":
                    if (AddStep != null)
                    {
                        AddStep(this, null);
                    }
                    break;
                case "bdRemove":
                    if (DelStep != null)
                    {
                        DelStep(this, null);
                    }
                    break;
            }

        }
    }
}
