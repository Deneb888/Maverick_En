#define ENGLISH_VER

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
    public partial class ucCyCling : UserControl
    {
        public int currCycle = 1;
        public event EventHandler TempChanaged;
        public event EventHandler AddRemoveFUN;
        public double currswd = 0;
        public double currewd = 0;
        public int currfun = 0;
        private DebugModelData currDebugModelData;
        public List<BaseData> bdlist = new List<BaseData>();
        public ucCyCling(double startwd, double endwd,DebugModelData dmd,int cycleindex)
        {
            InitializeComponent();
            this.currswd = startwd;
            this.currewd = endwd;
            this.currDebugModelData = dmd;
            this.currCycle = cycleindex;
            this.Loaded += ucCyCling_Loaded;
        }

        void ucCyCling_Loaded(object sender, RoutedEventArgs e)
        {
            cboPZ.IsChecked = true;
            bdlist = new List<BaseData>();
            ucStepInfo ucStepInfo = null;
            if (currDebugModelData != null)
            {
                cboPZ.IsChecked = currDebugModelData.ifpz == 0 ? true : false;
                txtCycle.Text = currDebugModelData.Cycle.ToString();
                if (currDebugModelData.Denaturating > 0)
                {
                    ucStepInfo = new Pages.ucStepInfo(currDebugModelData.DenaturatingStart, currDebugModelData.Denaturating, 1, currDebugModelData.DenaturatingTime);
                    ucStepInfo.Tag = 1;
                    ucStepInfo.currCycle = currCycle;
                    ucStepInfo.DelStep += ucStepInfo_DelStep;
                    ucStepInfo.AddStep += ucStepInfo_AddStep;
                }
                else
                {
                    ucStepInfo = null;
                }
            }
            else
            {
                ucStepInfo = new Pages.ucStepInfo(currswd, currswd, 1, 10);
                ucStepInfo.Tag = 1;
                ucStepInfo.currCycle = currCycle;
                ucStepInfo.DelStep += ucStepInfo_DelStep;
                ucStepInfo.AddStep += ucStepInfo_AddStep;
            }

            if (ucStepInfo != null)
            {
                //ucStepInfo.Tag = Guid.NewGuid();
                ucStepInfo.TempChanaged += ucStepInfo_TempChanaged;
                ucStepInfo.txtStep.Text = "Step 1";
                gdMain.Children.Add(ucStepInfo);
                BaseData bd = new BaseData();
                bd.id = 1;
                bd.name = "Step1";
                bdlist.Add(bd);           
            }

            if (currDebugModelData != null)
            {
                if (currDebugModelData.Annealing > 0)
                {
                    ucStepInfo = new Pages.ucStepInfo(currDebugModelData.AnnealingStart, currDebugModelData.Annealing, 1, currDebugModelData.AnnealingTime);
                    ucStepInfo.Tag = 2;
                    ucStepInfo.currCycle = currCycle;
                    ucStepInfo.DelStep += ucStepInfo_DelStep;
                    ucStepInfo.AddStep += ucStepInfo_AddStep;
                }
                else
                {
                    ucStepInfo = null;
                }
            }
            else
            {
                ucStepInfo = new Pages.ucStepInfo(currswd, currewd, 1, 15);
                ucStepInfo.Tag = 2;
                ucStepInfo.currCycle = currCycle;
                ucStepInfo.DelStep += ucStepInfo_DelStep;
                ucStepInfo.AddStep += ucStepInfo_AddStep;
            }

            if (ucStepInfo != null)
            {
                //ucStepInfo.Tag = Guid.NewGuid();
                ucStepInfo.TempChanaged += ucStepInfo_TempChanaged;
                ucStepInfo.txtStep.Text = "Step 2";
                gdMain.Children.Add(ucStepInfo);
                BaseData bd = new BaseData();
                bd.id = 2;
                bd.name = "Step2";
                bdlist.Add(bd);

            }

            if (currDebugModelData != null)
            {
                if (currDebugModelData.Extension > 0)
                {
                    ucStepInfo = new Pages.ucStepInfo(currDebugModelData.ExtensionStart, currDebugModelData.Extension, 1, currDebugModelData.ExtensionTime);
                    ucStepInfo.Tag = 3;
                    ucStepInfo.currCycle = currCycle;
                    ucStepInfo.DelStep += ucStepInfo_DelStep;
                    ucStepInfo.AddStep += ucStepInfo_AddStep;
                }
                else
                {
                    ucStepInfo = null;
                }
            }
            else
            {
                //ucStepInfo = new Pages.ucStepInfo(currewd, currewd + 12, 1, 20);
                ucStepInfo = new Pages.ucStepInfo(currewd, 72, 1, 20);                  // preferred extension temp
                ucStepInfo.Tag = 3;
                ucStepInfo.currCycle = currCycle;
                ucStepInfo.DelStep += ucStepInfo_DelStep;
                ucStepInfo.AddStep += ucStepInfo_AddStep;
            }

            if (ucStepInfo != null)
            {
                //ucStepInfo.Tag = Guid.NewGuid();
                ucStepInfo.TempChanaged += ucStepInfo_TempChanaged;
                ucStepInfo.txtStep.Text = "Step 3";
                gdMain.Children.Add(ucStepInfo);
                BaseData bd = new BaseData();
                bd.id = 3;
                bd.name = "Step3";
                bdlist.Add(bd);

            }

            if (currDebugModelData != null)
            {
                if (currDebugModelData.Step4 > 0)
                {
                    ucStepInfo = new Pages.ucStepInfo(currDebugModelData.Step4Start, currDebugModelData.Step4, 1, currDebugModelData.Step4Time);
                    ucStepInfo.Tag = 4;
                    ucStepInfo.currCycle = currCycle;
                    ucStepInfo.DelStep += ucStepInfo_DelStep;
                    ucStepInfo.AddStep += ucStepInfo_AddStep;
                }
                else
                {
                    ucStepInfo = null;
                }
            }
            else
            {
                //ucStepInfo = new Pages.ucStepInfo(currswd, currswd, 1, 10);
                ucStepInfo = null;
            }

            if (ucStepInfo != null)
            {
                //ucStepInfo.Tag = Guid.NewGuid();
                ucStepInfo.TempChanaged += ucStepInfo_TempChanaged;
                ucStepInfo.txtStep.Text = "Step 4";
                gdMain.Children.Add(ucStepInfo);
                BaseData bd = new BaseData();
                bd.id = 4;
                bd.name = "Step4";
                bdlist.Add(bd);

            }
            //BaseData BaseData = new BaseData();
            //BaseData.id = -1;
            //BaseData.name = "无";
            //bdlist.Insert(0, BaseData);
            cboPZJD.DisplayMemberPath = "name";
            cboPZJD.SelectedValuePath = "id";
            cboPZJD.ItemsSource = bdlist;
            cboPZJD.SelectedIndex = bdlist.Count - 1;

            if(currDebugModelData != null)
                cboPZJD.SelectedIndex = currDebugModelData.stageIndex - 1;

            foreach (var item in gdMain.Children)
            {
                ucStepInfo ucStepInfo_last = item as ucStepInfo;
                if (ucStepInfo_last.Tag.ToString() == gdMain.Children.Count.ToString())
                {
                    ucStepInfo_last.bdAdd.Visibility = Visibility.Visible;
                    break;
                }

            }
        }

        void ucStepInfo_AddStep(object sender, EventArgs e)
        {
            if (gdMain.Children.Count == 4)
            {
#if ENGLISH_VER
                MessageBox.Show("Cannot exceed 4 steps");
#else
                MessageBox.Show("不能多于4个步骤");
#endif
                return;
            }
            ucStepInfo ucStepInfo = sender as ucStepInfo;
            ucStepInfo new_ucStepInfo = new Pages.ucStepInfo(ucStepInfo.currewd, ucStepInfo.currewd, 1, 10);
            new_ucStepInfo.Tag = gdMain.Children.Count + 1;
            new_ucStepInfo.DelStep += ucStepInfo_DelStep;
            new_ucStepInfo.AddStep += ucStepInfo_AddStep;
            new_ucStepInfo.TempChanaged += ucStepInfo_TempChanaged;
            new_ucStepInfo.txtStep.Text = string.Format("Step {0}", gdMain.Children.Count + 1);
            gdMain.Children.Add(new_ucStepInfo);
            int indexcount = 1;
            bdlist.Clear();
            foreach (var item in gdMain.Children)
            {
                ucStepInfo ucStepInfo_last = item as ucStepInfo;
                if (ucStepInfo_last.Tag.ToString() == gdMain.Children.Count.ToString())
                {
                    ucStepInfo_last.bdAdd.Visibility = Visibility.Visible;
                }
                else
                {
                    ucStepInfo_last.bdAdd.Visibility = Visibility.Collapsed;
                }
                BaseData bd = new BaseData();
                bd.id = indexcount;
                bd.name = string.Format("Step{0}",indexcount);
                bdlist.Add(bd);
                indexcount++;
            }
            cboPZJD.DisplayMemberPath = "name";
            cboPZJD.SelectedValuePath = "id";
            cboPZJD.ItemsSource = bdlist;
            cboPZJD.SelectedIndex = bdlist.Count - 1;
        }

        void ucStepInfo_DelStep(object sender, EventArgs e)
        {
            ucStepInfo ucStepInfo = sender as ucStepInfo;
           // ucStepInfo.SetTemp(temp);
            if (gdMain.Children.Count <= 2)
            {
#if ENGLISH_VER
                MessageBox.Show("Cannot be less than 2 step");
#else
                MessageBox.Show("不能少于2个步骤");
#endif
                return;
            }

            if (ucStepInfo.Tag.ToString() == "1")
            {
                foreach (var item in gdMain.Children)
                {
                    ucStepInfo ucStepInfo_last = item as ucStepInfo;
                    if (ucStepInfo_last.Tag.ToString() == "2")
                    {
                        ucStepInfo_last.SetTemp(ucStepInfo.currswd);
                        break;
                    }
                }
                gdMain.Children.Remove(ucStepInfo);

                for (int i = 1; i <= gdMain.Children.Count; i++)
                {
                    ucStepInfo curr_ucStepInfo = gdMain.Children[i - 1] as ucStepInfo;
                    curr_ucStepInfo.Tag = i;
                }
            }

            if (ucStepInfo.Tag.ToString() == gdMain.Children.Count.ToString())
            {
               
                if (TempChanaged != null)
                {
                    List<string> list = new List<string>();
                    list.Add(gdMain.Children.Count.ToString());
                    list.Add(ucStepInfo.currswd.ToString());
                    list.Add(ucStepInfo.bzstr);
                    TempChanaged(list, e);
                }
                gdMain.Children.Remove(ucStepInfo);
            }
            if (ucStepInfo.Tag.ToString() != "1" && ucStepInfo.Tag.ToString() != gdMain.Children.Count.ToString())
            {
                int curr_tag = Convert.ToInt32(ucStepInfo.Tag) + 1;
                foreach (var item in gdMain.Children)
                {
                    ucStepInfo ucStepInfo_last = item as ucStepInfo;
                    if (ucStepInfo_last.Tag.ToString() == curr_tag.ToString())
                    {
                        ucStepInfo_last.SetTemp(ucStepInfo.currswd);
                        break;
                    }
                }
                gdMain.Children.Remove(ucStepInfo);
                for (int i = 1; i <= gdMain.Children.Count; i++)
                {
                    ucStepInfo curr_ucStepInfo = gdMain.Children[i - 1] as ucStepInfo;
                    curr_ucStepInfo.Tag = i;
                }
            }

            int indexcount = 1;
            bdlist.Clear();
            foreach (var item in gdMain.Children)
            {
                 ucStepInfo ucStepInfo_last = item as ucStepInfo;
                 if (ucStepInfo_last.Tag.ToString() == gdMain.Children.Count.ToString())
                 {
                     ucStepInfo_last.bdAdd.Visibility = Visibility.Visible;
                 }
                 else
                 {
                     ucStepInfo_last.bdAdd.Visibility = Visibility.Collapsed;
                 }
                 BaseData bd = new BaseData();
                 bd.id = indexcount;
                 bd.name = string.Format("Step{0}", indexcount);
                 bdlist.Add(bd);
                 indexcount++;
            }
            cboPZJD.DisplayMemberPath = "name";
            cboPZJD.SelectedValuePath = "id";
            cboPZJD.ItemsSource = bdlist;
            cboPZJD.SelectedIndex = bdlist.Count - 1;
          
        }

        void ucStepInfo_TempChanaged(object sender, EventArgs e)
        {
           if(TempChanaged!=null)
           {
               //List<string> list = sender as List<string>;
               //currewd = Convert.ToDouble(list[1]);
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

        private void cboPZJD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CommData.CurrPZJD = Convert.ToInt32(cboPZJD.SelectedValue);
        }

        private void cboPZ_Checked(object sender, RoutedEventArgs e)
        {
            cboPZJD.Visibility = Visibility.Visible;
        }

        private void cboPZ_Unchecked(object sender, RoutedEventArgs e)
        {
            cboPZJD.Visibility = Visibility.Hidden;
        }

        private void txtCycle_Changed(object sender, TextChangedEventArgs e)
        {
            try
            {
                int nc = Convert.ToInt32(txtCycle.Text);

                if (nc <= 0)
                {
                    MessageBox.Show("Value cannot be zero or negative.");
                    txtCycle.Text = "1";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
