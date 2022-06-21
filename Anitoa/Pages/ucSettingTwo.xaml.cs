#define ENGLISH_VER

// #define Lumin_Lite

using Microsoft.Win32;
using Newtonsoft.Json;
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

// Zhimin comment: practically,"foreach" iterate through the items in order. But from language point of view, this is not guaranteed. 
// the implementation here assumes in order iteration

namespace Anitoa.Pages
{
    /// <summary>
    /// ucSettingTwo.xaml 的交互逻辑
    /// </summary>
    public partial class ucSettingTwo : UserControl
    {
        public event EventHandler ChooseM;
        public event EventHandler SettingOK;
        public event EventHandler MeltOK;

        public bool iscz = false;

        public ucSettingTwo()
        {
            InitializeComponent();
            this.Loaded += ucSettingTwo_Loaded;
            this.Unloaded += ucSettingTwo_UnLoaded;

        }

        void ucSettingTwo_Loaded(object sender, RoutedEventArgs e)
        {
            spkd.Visibility = Visibility.Visible;
            // BindComBox();
            sviewer.Width = spkd.ActualWidth;
            spitems.Children.Clear();

            // Add Liuhh 2019.03.12            
            cbMode1.Checked += cb_Checked;
            cbMode1.Unchecked += cb_Unchecked;
            cbMode2.Checked += cb_Checked;
            cbMode2.Unchecked += cb_Unchecked;
            cbMode1.IsChecked = true;

            initData();

            // Todo : check if test running.
            if (!(iscz && CommData.deviceFound))
            {
                rbMelt.IsEnabled = false;
                rbMelt.Opacity = 0.3;
            }
            else
            {
                rbMelt.IsEnabled = true;
                rbMelt.Opacity = 1;
            }

            if (!CommData.deviceFound)  // disabled for now
            {
                rbStart.IsEnabled = false;
                rbStart.Opacity = 0.3;
            }
            else
            {
                rbStart.IsEnabled = true;
                rbStart.Opacity = 1;
            }
        }

        void ucSettingTwo_UnLoaded(object sender, RoutedEventArgs e)
        {
            List<DebugModelData> dmdlist = getDebugModelData();
            CommData.experimentModelData.programMode = cboCX.Text;
            CommData.experimentModelData.DebugModelDataList = dmdlist;
            //            bool res = SaveJsonData();

            CommData.expSaved = false;
        }

       

        public void initData()
        {
            if (CommData.experimentModelData != null && CommData.experimentModelData.DebugModelDataList != null && CommData.experimentModelData.DebugModelDataList.Count > 0)
            {
                foreach (var item in cboCX.Items)
                {
                    ComboBoxItem cbi = item as ComboBoxItem;
                    CheckBox cb = cbi.Content as CheckBox;
                    if (CommData.experimentModelData.programMode != null && CommData.experimentModelData.programMode.Contains(cb.Content.ToString()))
                    {
                        cb.IsChecked = true;
                    }
                }
                txtHotlid.Text = CommData.experimentModelData.DebugModelDataList[0].Hotlid.ToString();
                DebugModelData dmd = CommData.experimentModelData.DebugModelDataList[0];

                ucHold ucHold = new ucHold(45, 95, dmd, 0);
                ucHold.Tag = 0;
                ucHold.TempChanaged += ucHold_TempChanaged;
                ucHold.AddRemoveFUN += ucHold_AddRemoveFUN;
                spitems.Children.Add(ucHold);

                if(CommData.experimentModelData.DebugModelDataList[0].InitDenatureStepCount == 2)
                {
                    ucHold = new ucHold(30, 93, dmd, 2);
                    ucHold.Tag = 1;
                    ucHold.AddRemoveFUN += ucHold_AddRemoveFUN;
                    ucHold.TempChanaged += ucHold_TempChanaged;
#if ENGLISH_VER
                    ucHold.txtJDN.Text = "Pre-Denat2"; // "预变性阶段2";
#else
                    ucHold.txtJDN.Text = "预变性阶段2";
#endif
                    spitems.Children.Add(ucHold);
                }

                int index = 1;
                foreach (var item in CommData.experimentModelData.DebugModelDataList)
                {
                    ucCyCling ucCyCling = new ucCyCling(95, 60, item, index);
                    ucCyCling.Tag = index;
                    ucCyCling.TempChanaged += ucHold_TempChanaged;
                    ucCyCling.AddRemoveFUN += ucCyCling_AddRemoveFUN;
                    spitems.Children.Add(ucCyCling);
                    index++;
                }

                ucHold = new ucHold(60, 50, dmd, 1);
                ucHold.Tag = 2;
                ucHold.AddRemoveFUN += ucHold_AddRemoveFUN;
                ucHold.TempChanaged += ucHold_TempChanaged;
#if ENGLISH_VER
                ucHold.txtJDN.Text = "Hold";
#else
                ucHold.txtJDN.Text = "保温阶段";
#endif
                spitems.Children.Add(ucHold);

                iscz = CommData.experimentModelData.DebugModelDataList[0].enMelt;

                if (iscz)
                {
                    ucRJBZ ucRJBZ = new ucRJBZ(dmd.MeltStart, dmd.MeltEnd, dmd);
                    ucRJBZ.TempChanaged += ucHold_TempChanaged;
                    spitems.Children.Add(ucRJBZ);

                    cbMode2.IsChecked = true;
                }
            }
            else
            {
                if (CommData.isImport == false)     //正常界面加载
                {

                    ucHold ucHold = new ucHold(45, 95, null, 0);
                    ucHold.Tag = 0;
                    ucHold.TempChanaged += ucHold_TempChanaged;
                    ucHold.AddRemoveFUN += ucHold_AddRemoveFUN;
                    spitems.Children.Add(ucHold);

                    ucCyCling ucCyCling = new ucCyCling(95, 60, null, 1);
                    ucCyCling.Tag = 1;
                    ucCyCling.TempChanaged += ucHold_TempChanaged;
                    ucCyCling.AddRemoveFUN += ucCyCling_AddRemoveFUN;
                    spitems.Children.Add(ucCyCling);

                    ucHold = new ucHold(72, 50, null, 1);
                    ucHold.Tag = 2;
                    ucHold.AddRemoveFUN += ucHold_AddRemoveFUN;
                    ucHold.TempChanaged += ucHold_TempChanaged;

#if ENGLISH_VER
                    ucHold.txtJDN.Text = "Hold";
#else
                    ucHold.txtJDN.Text = "保温阶段"; 
#endif
                    spitems.Children.Add(ucHold);
                }
                else//导入文件数据
                {
                    // Not implemented
                }
            }

            EnAutoInt.IsChecked = CommData.experimentModelData.enAutoInt;
        }

        private void InitTempSet()
        {
            //CommData.experimentModelData.programMode = cboCX.Text;
            //CommData.experimentModelData.DebugModelDataList = dmdlist;
        }

        void ucCyCling_AddRemoveFUN(object sender, EventArgs e)
        {
            ucCyCling ucCyCling = sender as ucCyCling;
            int tag = Convert.ToInt32(ucCyCling.Tag) + 1;
            if (ucCyCling.currfun == 0)//添加
            {
                ucStepInfo ucStepInfo_last = ucCyCling.gdMain.Children[ucCyCling.gdMain.Children.Count - 1] as ucStepInfo;
                ucCyCling ucCyClingnew = new ucCyCling(ucStepInfo_last.currewd, ucStepInfo_last.currewd, null, ucStepInfo_last.currCycle + 1);
                ucCyClingnew.AddRemoveFUN += ucCyCling_AddRemoveFUN;
                ucCyClingnew.TempChanaged += ucHold_TempChanaged;
                ucCyClingnew.Tag = tag;
                spitems.Children.Insert(tag, ucCyClingnew);
            }
            else//删除
            {
                if ((spitems.Children.Count == 3 && !iscz && currCount() < 2) ||
                   (spitems.Children.Count == 4 && iscz && currCount() < 2) ||
                   (spitems.Children.Count == 5 && iscz && currCount() == 2) ||
                   (spitems.Children.Count == 4 && !iscz && currCount() == 2))
                {
                    //MessageBox.Show("不能删除");

#if ENGLISH_VER
                    MessageBox.Show("Cannot remove cycler step");
#else
                    MessageBox.Show("不能删除");
#endif

                    return;
                }

                SetTmpByTag(Convert.ToInt32(ucCyCling.Tag));
                spitems.Children.Remove(ucCyCling);

            }

            SetTag();
        }

        void ucHold_AddRemoveFUN(object sender, EventArgs e)
        {
            ucHold ucHold = sender as ucHold;
            int tag = Convert.ToInt32(ucHold.Tag) + 1;
            if (ucHold.currfun == 0)//添加
            {
                if (currCount() == 2)
                {
#if ENGLISH_VER
                    MessageBox.Show("Cannot have more than 2 Pre-denature step");
#else
                    MessageBox.Show("至多有2个预变性阶段");
#endif
                    return;
                }
                ucHold ucHoldnew = new ucHold(ucHold.currewd, ucHold.currewd, null, 2);
                ucHoldnew.AddRemoveFUN += ucHold_AddRemoveFUN;
                ucHoldnew.TempChanaged += ucHold_TempChanaged;
                ucHoldnew.Tag = tag;
                //                                ucHoldnew.currindex = 2;

#if ENGLISH_VER
                ucHoldnew.txtJDN.Text = "Pre-Denat2";
#else
                ucHoldnew.txtJDN.Text = "预变性阶段2";
#endif

                spitems.Children.Insert(tag, ucHoldnew);
            }
            else//删除
            {
                if (currCount() == 1)
                {
#if ENGLISH_VER
                    MessageBox.Show("There should be at least one pre-denature step");
#else
                    MessageBox.Show("至少有一个预变性阶段");
#endif
                    return;
                }

                SetTmpByTag(Convert.ToInt32(ucHold.Tag));
                spitems.Children.Remove(ucHold);
            }

            SetTag();
        }

        public void SetTmpByTag(int tag)
        {

            int qtag = tag - 1;
            if (qtag < 0)
            {
                qtag = 0;
            }
            int htag = tag + 1;
            double temp = 0;
            foreach (var item in spitems.Children)
            {
                if (item is ucHold)
                {
                    ucHold ucHold = item as ucHold;
                    if (Convert.ToInt32(ucHold.Tag) == qtag)
                    {
                        if (tag == 0)
                        {
                            temp = ucHold.currswd;
                        }
                        else
                        {
                            temp = ucHold.currewd;
                        }
                        break;
                    }
                }
                if (item is ucCyCling)
                {
                    ucCyCling ucCyCling = item as ucCyCling;
                    if (Convert.ToInt32(ucCyCling.Tag) == qtag)
                    {
                        int count = ucCyCling.gdMain.Children.Count;
                        ucStepInfo ucStepInfo = ucCyCling.gdMain.Children[count - 1] as ucStepInfo;
                        temp = ucStepInfo.currewd;
                        break;
                    }
                }
            }

            foreach (var item in spitems.Children)
            {
                if (item is ucHold)
                {
                    ucHold ucHold = item as ucHold;
                    if (Convert.ToInt32(ucHold.Tag) == htag)
                    {
                        ucStepInfo ucStepInfo = ucHold.gdMain.Children[0] as ucStepInfo;
                        ucStepInfo.SetTemp(temp);
                        break;
                    }
                }
                if (item is ucCyCling)
                {
                    ucCyCling ucCyCling = item as ucCyCling;
                    if (Convert.ToInt32(ucCyCling.Tag) == htag)
                    {
                        ucStepInfo ucStepInfo = ucCyCling.gdMain.Children[0] as ucStepInfo;
                        ucStepInfo.SetTemp(temp);
                        break;
                    }
                }
            }
        }

        public void SetTag()
        {
            return; //Zhimin: do not re-arrange the tag
            for (int i = 0; i < spitems.Children.Count; i++)
            {
                if (spitems.Children[i] is ucHold)
                {
                    ucHold ucHold = spitems.Children[i] as ucHold;
                    ucHold.Tag = i;
                }
                if (spitems.Children[i] is ucCyCling)
                {
                    ucCyCling ucCyCling = spitems.Children[i] as ucCyCling;
                    ucCyCling.Tag = i;
                }
            }
        }

        void ucHold_TempChanaged(object sender, EventArgs e)
        {
            chainTempControl(sender, e);

            List<string> list = sender as List<string>;
            List<UIElementCollection> ucStepInfolist = new List<UIElementCollection>();
            for (int i = 0; i < spitems.Children.Count; i++)
            {
                if (spitems.Children[i] is ucHold)
                {
                    ucHold ucHold = spitems.Children[i] as ucHold;
                    ucStepInfolist.Add(ucHold.gdMain.Children);
                }

                if (spitems.Children[i] is ucCyCling)
                {
                    ucCyCling ucCyCling = spitems.Children[i] as ucCyCling;
                    ucStepInfolist.Add(ucCyCling.gdMain.Children);
                }

                if (spitems.Children[i] is ucRJBZ)
                {
                    ucRJBZ ucRJBZ = spitems.Children[i] as ucRJBZ;
                    ucStepInfolist.Add(ucRJBZ.gdMain.Children);
                }
            }

            for (int i = 0; i < ucStepInfolist.Count; i++)
            {
                UIElementCollection UIElementCollections = ucStepInfolist[i] as UIElementCollection;
                for (int n = 0; n < UIElementCollections.Count; n++)
                {
                    ucStepInfo ucStepInfo = UIElementCollections[n] as ucStepInfo;
                    if (ucStepInfo.Tag.ToString() == list[0] && ucStepInfo.bzstr == list[2])
                    {
                        if (n != UIElementCollections.Count - 1)
                        {
                            ucStepInfo CurrucStepInfo = UIElementCollections[n + 1] as ucStepInfo;
                            CurrucStepInfo.SetTemp(Convert.ToDouble(list[1]));
                            return;
                        }
                        else
                        {
                            if (i != ucStepInfolist.Count - 1)
                            {
                                UIElementCollections = ucStepInfolist[i + 1] as UIElementCollection;
                                ucStepInfo CurrucStepInfo = UIElementCollections[0] as ucStepInfo;
                                CurrucStepInfo.SetTemp(Convert.ToDouble(list[1]));
                                return;
                            }
                            else if(false)
                            {
                                foreach (var item in spitems.Children)
                                {
                                    if (item is ucRJBZ)
                                    {
                                        ucRJBZ ucRJBZ = item as ucRJBZ;
                                        ucStepInfo CurrucStepInfo = ucRJBZ.gdMain.Children[0] as ucStepInfo;
                                        CurrucStepInfo.SetTemp(Convert.ToDouble(list[1]));
                                        break;
                                    }

                                }

                            }
                        }

                    }

                }
            }
        }

        void chainTempControl(object sender, EventArgs e)
        {
            List<string> list = sender as List<string>;
            List<UIElementCollection> ucStepInfolist = new List<UIElementCollection>();
            double temp = 0;

            for (int i = 0; i < spitems.Children.Count; i++)
            {
                if (spitems.Children[i] is ucHold)
                {
                    ucHold hold = spitems.Children[i] as ucHold;
                    if (Convert.ToInt32(hold.Tag) == 2)
                    {
                        ucStepInfolist.Add(hold.gdMain.Children);
                    }
                }
                else if (spitems.Children[i] is ucRJBZ)
                {
                    ucRJBZ rjbz = spitems.Children[i] as ucRJBZ;
                    ucStepInfolist.Add(rjbz.gdMain.Children);
                }
            }

            for (int i = 0; i < ucStepInfolist.Count; i++)
            {
                UIElementCollection UIElementCollections = ucStepInfolist[i] as UIElementCollection;
                if (UIElementCollections.Count > 0)
                {
                    ucStepInfo step = UIElementCollections[0] as ucStepInfo;

                    if (step.bzstr == list[2])
                        temp = step.currewd;
                }
            }

            for (int i = 0; i < ucStepInfolist.Count; i++)
            {
                UIElementCollection UIElementCollections = ucStepInfolist[i] as UIElementCollection;
                if (UIElementCollections.Count > 0)
                {
                    ucStepInfo step = UIElementCollections[0] as ucStepInfo;

                    if (temp > 0.1)
                    {
                        step.SetTemp2(Convert.ToDouble(temp));
                    }
                }
            }
        }

        void ucRJBZ_TempChanaged(object sender, EventArgs e)
        {
            List<string> list = sender as List<string>;
            List<UIElementCollection> ucStepInfolist = new List<UIElementCollection>();
            for (int i = 0; i < spitems.Children.Count; i++)
            {
                if (spitems.Children[i] is ucRJBZ)
                {
                    ucRJBZ ucRJBZ = spitems.Children[i] as ucRJBZ;
                    ucStepInfolist.Add(ucRJBZ.gdMain.Children);
                }
            }

            for (int i = 0; i < ucStepInfolist.Count; i++)
            {
                UIElementCollection UIElementCollections = ucStepInfolist[i] as UIElementCollection;
                for (int n = 0; n < UIElementCollections.Count; n++)
                {
                    ucStepInfo ucStepInfo = UIElementCollections[n] as ucStepInfo;
                    if (ucStepInfo.Tag.ToString() == list[0] && ucStepInfo.bzstr == list[2])
                    {
                    //    ucStepInfo CurrucStepInfo = UIElementCollections[n] as ucStepInfo;
                    //    CurrucStepInfo.SetTemp(Convert.ToDouble(list[1]));
                    //    return;

                        if (n != UIElementCollections.Count - 1)
                        {
                            ucStepInfo CurrucStepInfo = UIElementCollections[n + 1] as ucStepInfo;
                            CurrucStepInfo.SetTemp(Convert.ToDouble(list[1]));
                            return;
                        }
                        else
                        {
                            if (i != ucStepInfolist.Count - 1)
                            {
                                UIElementCollections = ucStepInfolist[i + 1] as UIElementCollection;
                                ucStepInfo CurrucStepInfo = UIElementCollections[0] as ucStepInfo;
                                CurrucStepInfo.SetTemp(Convert.ToDouble(list[1]));
                                return;
                            }
/*                            else
                            {
                                foreach (var item in spitems.Children)
                                {
                                    if (item is ucRJBZ)
                                    {
                                        ucRJBZ ucRJBZ = item as ucRJBZ;
                                        ucStepInfo CurrucStepInfo = ucRJBZ.gdMain.Children[0] as ucStepInfo;
                                        CurrucStepInfo.SetTemp(Convert.ToDouble(list[1]));
                                        break;
                                    }

                                }

                            }
                        */}
                        
                    }

                }
            }
        }


        public int currCount()
        {
            int count = 0;
            foreach (var item in spitems.Children)
            {
                if (item is ucHold)
                {
                    ucHold ucHold = item as ucHold;

#if ENGLISH_VER
                    if (ucHold.txtJDN.Text == "Pre-Denat" || ucHold.txtJDN.Text == "Pre-Denat2")
#else
                    if (ucHold.txtJDN.Text == "预变性阶段" || ucHold.txtJDN.Text == "预变性阶段2")
#endif
                    {
                        count += 1;
                    }
                   
                }
                if (item is ucCyCling)
                {
                    //ucCyCling ucCyCling = item as ucCyCling;
                    //ucCyCling.
                }
            }

            return count;

        }

        void ucHold_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void BindComBox()
        {
            cboCX.Items.Clear();
            CheckBox cb = new CheckBox();
            cb.Width = cboCX.ActualWidth;
            cb.Checked += cb_Checked;
            cb.Unchecked += cb_Unchecked;
            cb.Content = "变温扩增";
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = cb;
            cboCX.Items.Add(cbi);
            cb.IsChecked = true;

            //cb = new CheckBox();
            //cb.Width = cboCX.ActualWidth;
            //cb.Checked += cb_Checked;
            //cb.Unchecked += cb_Unchecked;
            //cb.Content = "等温扩增";
            //cbi = new ComboBoxItem();
            //cbi.Content = cb;
            //cboCX.Items.Add(cbi);

            cb = new CheckBox();
            cb.Width = cboCX.ActualWidth;
            cb.Checked += cb_Checked;
            cb.Unchecked += cb_Unchecked;
            cb.Content = "熔解曲线";
            cbi = new ComboBoxItem();
            cbi.Content = cb;
            cboCX.Items.Add(cbi);
            cb.IsChecked = iscz;
        }


        void cb_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
#if ENGLISH_VER
            if (cb.Content.ToString() == "Melting Curve")
#else
            if (cb.Content.ToString() == "熔解曲线")
#endif        
            {
                foreach (var item in spitems.Children)
                {
                    if (item is ucRJBZ)
                    {
                        ucRJBZ ucRJBZ = item as ucRJBZ;
                        spitems.Children.Remove(ucRJBZ);
                        iscz = false;

                        rbMelt.IsEnabled = false;
                        rbMelt.Opacity = 0.3;

                        break;
                    }

                }
            }

#if ENGLISH_VER
            if (cb.Content.ToString() == "Amplification")
#else
            if (cb.Content.ToString() == "变温扩增")
#endif
            {
                foreach (var item in spitems.Children)
                {
                    if (item is ucHold)
                    {
                        ucHold ucHold = item as ucHold;
                        ucHold.Visibility = Visibility.Collapsed;
                    }
                    if (item is ucCyCling)
                    {
                        ucCyCling ucCyCling = item as ucCyCling;
                        ucCyCling.Visibility = Visibility.Collapsed;
                    }

                }
            }

            SetCboText();
        }

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
#if ENGLISH_VER
            if (cb.Content.ToString() == "Amplification")
#else
            if (cb.Content.ToString() == "变温扩增")
#endif
            {
                foreach (var item in spitems.Children)
                {
                    if (item is ucHold)
                    {
                        ucHold ucHold = item as ucHold;
                        ucHold.Visibility = Visibility.Visible;
                    }
                    if (item is ucCyCling)
                    {
                        ucCyCling ucCyCling = item as ucCyCling;
                        ucCyCling.Visibility = Visibility.Visible;
                    }

                }
            }
#if ENGLISH_VER
            if (cb.Content.ToString() == "Melting Curve")
#else
            if (cb.Content.ToString() == "熔解曲线")
#endif        
            {
                if (iscz == false)
                {
                    //foreach (var item in spitems.Children)
                    //{
                    //    if (item is ucHold)
                    //    {
                    //        ucHold ucHold = item as ucHold;
                    //        if (ucHold.Tag.ToString() == "2")
                    //        {
                    //            ucRJBZ ucRJBZ = new ucRJBZ(ucHold.currewd, ucHold.currewd, null);
                    //            spitems.Children.Add(ucRJBZ);
                    //            iscz = true;
                    //            break;
                    //        }
                    //    }

                    //}


                    //                    ucHold ucHold = spitems.Children[spitems.Children.Count-1] as ucHold;
                    //                    ucRJBZ ucRJBZ = new ucRJBZ(ucHold.currewd, ucHold.currewd, null);

                    //=============

                    DebugModelData dmd =  null;
                    double holdTemp = 50;

                    if (CommData.experimentModelData != null && CommData.experimentModelData.DebugModelDataList != null && CommData.experimentModelData.DebugModelDataList.Count > 0)
                    {
                        CommData.experimentModelData.DebugModelDataList = getDebugModelData();
                        dmd = CommData.experimentModelData.DebugModelDataList[0];
                        holdTemp = dmd.Holdon;
                    }
                    else
                    {
                        List<DebugModelData> dmdlist = getDebugModelData();
                        CommData.experimentModelData.programMode = cboCX.Text;
                        CommData.experimentModelData.DebugModelDataList = dmdlist;
                        CommData.expSaved = false;

                        holdTemp = dmdlist[0].Holdon;
                    }

                    //=================

                    ucRJBZ ucRJBZ = new ucRJBZ(holdTemp, 90, dmd);
                    ucRJBZ.TempChanaged += ucHold_TempChanaged;
                    spitems.Children.Add(ucRJBZ);
                    iscz = true;

                    if (ucRJBZ.gdMain.Children.Count > 0) {
                        ucStepInfo ucStepInfo = ucRJBZ.gdMain.Children[0] as ucStepInfo;
                        ucStepInfo.SetTemp(60.5);
                    }

                    if (CommData.deviceFound)
                    {
                        rbMelt.IsEnabled = true;
                        rbMelt.Opacity = 1;
                    }
                }
            }

            SetCboText();
        }

        private void SetCboText()
        {
            string mmtext = "";
            foreach (var item in cboCX.Items)
            {
                ComboBoxItem cbi = item as ComboBoxItem;
                CheckBox cb = cbi.Content as CheckBox;
                if (cb.IsChecked == true)
                {
                    if (string.IsNullOrEmpty(mmtext))
                    {
                        mmtext = cb.Content.ToString();
                    }
                    else
                    {
                        mmtext += "+" + cb.Content.ToString();
                    }
                }
            }
            cboCX.Text = mmtext;

            //if (string.IsNullOrEmpty(mmtext))
            //{
            //    spitems.IsEnabled = false;
            //}
            //else
            //{
            //    spitems.IsEnabled = true;
            //}

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("1", null);
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            List<DebugModelData> dmdlist = getDebugModelData();
            CommData.experimentModelData.programMode = cboCX.Text;
            CommData.experimentModelData.DebugModelDataList = dmdlist;
            //bool res = SaveJsonData();
            switch (rb.Name)
            {
                case "rbMelt":

                    if (MeltOK != null)
                    {
              //          if (res == true)
              //          {
                            bdName.Visibility = Visibility.Visible;
                            MeltOK(dmdlist, null);
              //          }
                    }

                    break;
                case "rbStart":
                    if (SettingOK != null)
                    {
                //        if (res == true)
                //        {
                            bdName.Visibility = Visibility.Visible;                           
                            SettingOK(dmdlist, null);
                //        }
                    }
                    break;
            }
        }

        public void TriggerMelt()
        {
            List<DebugModelData> dmdlist = getDebugModelData();

            if (MeltOK != null)
            {
                MeltOK(dmdlist, null);
            }
        }

        private void cboCX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCboText();
        }


        private List<DebugModelData> getDebugModelData()
        {
#if Lumin_Lite
            return CommData.experimentModelData.DebugModelDataList;
#endif
            List<DebugModelData> dmdlist = new List<DebugModelData>();
            try
            {
                foreach (var item in spitems.Children)
                {
                    if (item is ucCyCling)
                    {
                        ucCyCling ucCyCling = item as ucCyCling;
                        DebugModelData currDebugModelData = new DebugModelData();
                        currDebugModelData.ifpz = ucCyCling.cboPZ.IsChecked == true ? 0 : 1;
                        currDebugModelData.Cycle = Convert.ToInt32(ucCyCling.txtCycle.Text);
                        currDebugModelData.Hotlid = Convert.ToDouble(txtHotlid.Text);
                        currDebugModelData.stageIndex = CommData.CurrPZJD;
                        currDebugModelData.StepCount = ucCyCling.gdMain.Children.Count;

                        currDebugModelData.MeltStart = 60;
                        currDebugModelData.MeltEnd = 90;        // initialize with something reasonable
                        currDebugModelData.enMelt = iscz;

                        for (int i = 0; i < ucCyCling.gdMain.Children.Count; i++)
                        {
                            ucStepInfo ucStepInfo = ucCyCling.gdMain.Children[i] as ucStepInfo;
                            ucwdItem ucwdItem = ucStepInfo.gdMain.Children[0] as ucwdItem;
                            if (i == 0)
                            {
                                currDebugModelData.DenaturatingStart = Convert.ToDouble(ucStepInfo.currswd.ToString("0.0"));
                                currDebugModelData.Denaturating = Convert.ToDouble(ucStepInfo.currewd.ToString("0.0"));
                                currDebugModelData.DenaturatingTime = Convert.ToDouble(ucwdItem.txttime.Text);
                            }
                            if (i == 1)
                            {
                                currDebugModelData.AnnealingStart = Convert.ToDouble(ucStepInfo.currswd.ToString("0.0"));
                                currDebugModelData.Annealing = Convert.ToDouble(ucStepInfo.currewd.ToString("0.0"));
                                currDebugModelData.AnnealingTime = Convert.ToDouble(ucwdItem.txttime.Text);
                            }
                            if (i == 2)
                            {
                                currDebugModelData.ExtensionStart = Convert.ToDouble(ucStepInfo.currswd.ToString("0.0"));
                                currDebugModelData.Extension = Convert.ToDouble(ucStepInfo.currewd.ToString("0.0"));
                                currDebugModelData.ExtensionTime = Convert.ToDouble(ucwdItem.txttime.Text);
                            }
                            if (i == 3)
                            {
                                currDebugModelData.Step4Start = Convert.ToDouble(ucStepInfo.currswd.ToString("0.0"));
                                currDebugModelData.Step4 = Convert.ToDouble(ucStepInfo.currewd.ToString("0.0"));
                                currDebugModelData.Step4Time = Convert.ToDouble(ucwdItem.txttime.Text);
                            }
                        }
                        dmdlist.Add(currDebugModelData);
                    }
                }

                foreach (var item in spitems.Children)
                {
                    if (item is ucHold)
                    {
                        ucHold hold = item as ucHold;
                        ucStepInfo stepInfo = hold.gdMain.Children[0] as ucStepInfo;
                        ucwdItem wdItem = stepInfo.gdMain.Children[0] as ucwdItem;
                        if (Convert.ToInt32(hold.Tag) == 0)
                        {
                            foreach (var dmd in dmdlist)
                            {
                                dmd.InitaldenaturationStart = Convert.ToDouble(stepInfo.currswd.ToString("0.0"));
                                dmd.Initaldenaturation = Convert.ToDouble(stepInfo.currewd.ToString("0.0"));
                                dmd.InitaldenaTime = Convert.ToDouble(wdItem.txttime.Text);
                                dmd.InitDenatureStepCount = 1;
                            }
                        }
                        else if (Convert.ToInt32(hold.Tag) == 1)
                        {
                            foreach (var dmd in dmdlist)
                            {
                                dmd.InitaldenaturationStart2 = Convert.ToDouble(stepInfo.currswd.ToString("0.0"));
                                dmd.Initaldenaturation2 = Convert.ToDouble(stepInfo.currewd.ToString("0.0"));
                                dmd.InitaldenaTime2 = Convert.ToDouble(wdItem.txttime.Text);
                                dmd.InitDenatureStepCount = 2;
                            }
                        }
                        else if(Convert.ToInt32(hold.Tag) == 2)
                        {
                            foreach (var dmd in dmdlist)
                            {
                                dmd.HoldonStart = Convert.ToDouble(stepInfo.currswd.ToString("0.0"));
                                dmd.Holdon = Convert.ToDouble(stepInfo.currewd.ToString("0.0"));
                                dmd.HoldonTime = Convert.ToDouble(wdItem.txttime.Text);
                            }
                        }
                    }
                }

                foreach (var item in spitems.Children)
                {
                    if (item is ucRJBZ)
                    {
                        ucRJBZ ucRJBZ = item as ucRJBZ;

                        for (int i = 0; i < ucRJBZ.gdMain.Children.Count; i++)
                        {
                            ucStepInfo ucStepInfo = ucRJBZ.gdMain.Children[i] as ucStepInfo;
                            ucwdItem ucwdItem = ucStepInfo.gdMain.Children[0] as ucwdItem;
                            if (i == 0)
                            {
                                foreach (var dmd in dmdlist)
                                {
                                    dmd.MeltStart = Convert.ToDouble(ucStepInfo.currewd.ToString("0.0"));
                                    dmd.MeltStartTime = Convert.ToDouble(ucwdItem.txttime.Text);
                                }
                            }
                            else if (i == 1)
                            {
                                foreach (var dmd in dmdlist)
                                {
                                    dmd.MeltEnd = Convert.ToDouble(ucStepInfo.currewd.ToString("0.0"));
                                    dmd.MeltEndTime = Convert.ToDouble(ucwdItem.txttime.Text);
                                }
                            }
                        }
                    }
                }

                return dmdlist;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dmdlist;
            }
        }
     

        private bool SaveJsonData() // not used
        {
            try
            {
                string fname = string.Format("SetData\\SetData_{0}_{1}.Json", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString(" hhmmss"));
                string path = AppDomain.CurrentDomain.BaseDirectory + fname;
                string jsonstring = JsonConvert.SerializeObject(CommData.experimentModelData);
                System.IO.File.WriteAllText(path, jsonstring);

                CommData.experimentModelData.SetFileName = fname;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private void rbLSave_Click(object sender, RoutedEventArgs e)
        {
            SaveExp();
        }

        public void SaveExp()
        {
            SaveFileDialog sfd = new SaveFileDialog();
#if ENGLISH_VER
            sfd.Filter = "Json document(*.Json)|*.Json|All files|*.*";  //设置文件类型
#else
            sfd.Filter = "Json文件(*.Json)|*.Json|所有文件|*.*";  //设置文件类型
#endif
            //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
            sfd.FileName = CommData.experimentModelData.emname;
            sfd.DefaultExt = "Json";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名
            if (sfd.ShowDialog() == true)
            {
                List<DebugModelData> dmdlist = getDebugModelData();
                CommData.experimentModelData.programMode = cboCX.Text;
                CommData.experimentModelData.DebugModelDataList = dmdlist;
                string path = sfd.FileName;
                string jsonstring = JsonConvert.SerializeObject(CommData.experimentModelData);
                System.IO.File.WriteAllText(path, jsonstring);

#if ENGLISH_VER
                MessageBox.Show("Experiment file saved");
#else
                MessageBox.Show("文件保存成功");
#endif

            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CommData.experimentModelData.enAutoInt = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CommData.experimentModelData.enAutoInt = false;
        }

        private void rbLSaveTmpl_Click(object sender, RoutedEventArgs e)
        {
            SaveExpTmpl();
        }

        public void SaveExpTmpl()
        {
            SaveFileDialog sfd = new SaveFileDialog();

#if ENGLISH_VER
            sfd.Filter = "Json file (*.Json)|*.Json|All files|*.*"; //设置文件类型
#else
            sfd.Filter = "Json文件(*.Json)|*.Json|所有文件|*.*";//设置文件类型
#endif
            //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
            sfd.DefaultExt = "Json";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名

            if (sfd.ShowDialog() == true)
            {
                List<DebugModelData> dmdlist = getDebugModelData();
                CommData.experimentModelData.programMode = cboCX.Text;

                string fn = CommData.experimentModelData.ImgFileName;
                string fn2 = CommData.experimentModelData.ImgFileName2;
                string sfn = CommData.experimentModelData.SetFileName;

                CommData.experimentModelData.ImgFileName = null;
                CommData.experimentModelData.ImgFileName2 =  null;
                CommData.experimentModelData.SetFileName = null;

                CommData.experimentModelData.DebugModelDataList = dmdlist;
                string path = sfd.FileName;
                string jsonstring = JsonConvert.SerializeObject(CommData.experimentModelData);
                System.IO.File.WriteAllText(path, jsonstring);

#if ENGLISH_VER
                MessageBox.Show("Template file saved");
#else
                MessageBox.Show("Template 文件保存成功");
#endif

                CommData.experimentModelData.ImgFileName = fn;
                CommData.experimentModelData.ImgFileName2 = fn2;
                CommData.experimentModelData.SetFileName = sfn;
            }
        }

        private void btResetParams(object sender, RoutedEventArgs e)
        {
            spitems.Children.Clear();

            ucHold ucHold = new ucHold(45, 95, null, 0);
            ucHold.Tag = 0;
            ucHold.TempChanaged += ucHold_TempChanaged;
            ucHold.AddRemoveFUN += ucHold_AddRemoveFUN;
            spitems.Children.Add(ucHold);

            ucCyCling ucCyCling = new ucCyCling(95, 60, null, 1);
            ucCyCling.Tag = 1;
            ucCyCling.TempChanaged += ucHold_TempChanaged;
            ucCyCling.AddRemoveFUN += ucCyCling_AddRemoveFUN;
            spitems.Children.Add(ucCyCling);

            ucHold = new ucHold(72, 50, null, 1);
            ucHold.Tag = 2;
            ucHold.AddRemoveFUN += ucHold_AddRemoveFUN;
            ucHold.TempChanaged += ucHold_TempChanaged;

#if ENGLISH_VER
            ucHold.txtJDN.Text = "Hold";
#else
                    ucHold.txtJDN.Text = "保温阶段"; 
#endif
            spitems.Children.Add(ucHold);

            iscz = false;

            cbMode2.IsChecked = false;
        }
    }
}
