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
    /// ucHold.xaml 的交互逻辑
    /// </summary>
    public partial class ucHold : UserControl
    {
        public event EventHandler TempChanaged;
        public event EventHandler AddRemoveFUN;
        public int currindex = 0;
        public double currswd = 0;
        public double currewd = 0;
        public int currfun = 0;
        private DebugModelData currDebugModelData;
        private int currType = 0;
        public ucHold(double startwd, double endwd,DebugModelData dmd,int type)
        {
            InitializeComponent();
            this.currswd = startwd;
            this.currewd = endwd;
            this.currDebugModelData = dmd;
            currType = type;
            this.Loaded += ucHold_Loaded;
        }

        void ucHold_Loaded(object sender, RoutedEventArgs e)
        {
            if (currType == 0 || currType == 2)
            {
                bdAdd.Visibility = bdRemove.Visibility = Visibility.Visible;
            }
            ucStepInfo ucStepInfo = null;
            if (currDebugModelData == null)
            {
                ucStepInfo = new Pages.ucStepInfo(currswd, currewd, 0, 60);
            }
            else
            {
                if (currType == 0)
                {
                    ucStepInfo = new Pages.ucStepInfo(currDebugModelData.InitaldenaturationStart, currDebugModelData.Initaldenaturation, 0, currDebugModelData.InitaldenaTime);
                }
                else if (currType == 2)
                {
                    ucStepInfo = new Pages.ucStepInfo(currDebugModelData.InitaldenaturationStart2, currDebugModelData.Initaldenaturation2, 0, currDebugModelData.InitaldenaTime2);
                }
                else
                {
                    ucStepInfo = new Pages.ucStepInfo(currDebugModelData.HoldonStart, currDebugModelData.Holdon, 0, currDebugModelData.HoldonTime);
                }

            }
           
            ucStepInfo.Tag = Guid.NewGuid();
            ucStepInfo.TempChanaged += ucStepInfo_TempChanaged;
            gdMain.Children.Add(ucStepInfo);
        }

        void ucStepInfo_TempChanaged(object sender, EventArgs e)
        {
            if (TempChanaged != null)
            {
                List<string> list = sender as List<string>;
                currewd = Convert.ToDouble(list[1]);
                TempChanaged(sender, e);
            }
        }

        private void bdAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border bd = sender as Border;
            switch (bd.Name)
            {
                case "bdAdd":
                    currfun = 0;
                    break;
                case "bdRemove":
                    currfun = 1;
                    break;
            }
            if (AddRemoveFUN != null)
            {
                AddRemoveFUN(this, null);
            }
        }
    }
}
