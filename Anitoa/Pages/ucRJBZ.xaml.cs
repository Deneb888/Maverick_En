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
    /// ucCyCling.xaml 的交互逻辑
    /// </summary>
    public partial class ucRJBZ : UserControl
    {
        public int currCycle = 1;
        public event EventHandler TempChanaged;
        public event EventHandler AddRemoveFUN;
        public double currswd = 0;
        public double currewd = 0;
        public int currfun = 0;
        private DebugModelData currDebugModelData;
        public List<BaseData> bdlist = new List<BaseData>();
        public ucRJBZ(double startwd, double endwd, DebugModelData dmd)
        {
            InitializeComponent();
            this.currswd = startwd;
            this.currewd = endwd;
            this.currDebugModelData = dmd;
            this.Loaded += ucRJBZ_Loaded;

//            if(dmd != null) currswd = dmd.MeltStart;
        }

        void ucRJBZ_Loaded(object sender, RoutedEventArgs e)
        {
            double htmp = 50, stime = 1, etime = 1;

            if (currDebugModelData != null)
            {
                htmp = currDebugModelData.Holdon;
                stime = currDebugModelData.MeltStartTime;
                etime = currDebugModelData.MeltEndTime;
            }

            ucStepInfo ucStepInfo = new ucStepInfo(htmp, currswd, 0, stime, false, true);
            ucStepInfo.Tag = "0";
            ucStepInfo.TempChanaged += ucStepInfo_TempChanaged;                         // only first step routed to event handler. Need to fix.
            gdMain.Children.Add(ucStepInfo);

            ucStepInfo = new ucStepInfo(currswd, currewd, 0, etime, false, true);
            ucStepInfo.Tag = "1";
            ucStepInfo.TempChanaged += ucStepInfo_TempChanaged;
            ucStepInfo.txtStep.Text = "Step 2";
            gdMain.Children.Add(ucStepInfo);
        }

        void ucStepInfo_TempChanaged(object sender, EventArgs e)
        {
            List<string> list = sender as List<string>;
            currewd = Convert.ToDouble(list[1]);

//            int tag = Convert.ToInt32(list[0]);

//            ucStepInfo ucStepInfo = gdMain.Children[tag] as ucStepInfo;
//            if(tag == 0) ucStepInfo.SetTemp(currewd);

            if(TempChanaged != null)
                TempChanaged(sender, e);
        }
    }
}
