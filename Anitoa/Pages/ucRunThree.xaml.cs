using DevExpress.Xpf.Charts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using ViliPetek.LinearAlgebra;

namespace Anitoa.Pages
{
    /// <summary>
    /// ucRunThree.xaml 的交互逻辑
    /// </summary>
    public partial class ucRunThree : UserControl
    {
        private static int MAX_ROW = 2;
        private static int MAX_WELL_PER_ROW = 8;

        private List<string> StdList = new List<string>();
        private List<string> UnkList = new List<string>();

        private List<double> std_ct = new List<double>();
        private List<double> std_conc = new List<double>();

        private List<double> unk_ct = new List<double>();
        private List<double> unk_conc = new List<double>();

        private int cur_chan = 0;

        private CheckBox cur_cb;

        private double p1 = -2.5;
        private double p2 = 40;

        private bool bUpdateAssay = false;

        public event EventHandler ChooseM;
        public ucRunThree()
        {
            InitializeComponent();
            this.Loaded += ucRunThree_Loaded;
        }

        void ucRunThree_Loaded(object sender, RoutedEventArgs e)
        {
            radChart.Animate();

            cboChansel.SelectedIndex = -1;

            cur_chan = CommData.curStdChan;

            cboChansel.SelectedIndex = cur_chan;

            // clear_all();

            //AutoAddStandards();
            //AutoAddUnkowns();

            txtExpTime.Text = CommData.experimentModelData.emdatetime.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseM != null)
            {
                ChooseM("1", null);
            }
        }

        /*
        private void gdA1_Checked(object sender, RoutedEventArgs e)
        {

            CheckBox rb = sender as CheckBox;

            txtSelect.Text = rb.Tag.ToString();

            cur_cb = rb;

        }

        private void gdA1_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        */

        private void AutoAddStandards()
        {
            if (cur_chan < 0) return;

            experiment exp = CommData.experimentModelData;
            string well;

            if (exp.assayChanTypeIndex[cur_chan] > 0)
            {
                UpdateStdTable();
                return; 
            }
                                            // Channel is internal control

            for(int row = 0; row <MAX_ROW; row++)
            {
                for(int i=0; i<MAX_WELL_PER_ROW; i++)
                {
                    if(exp.sampleType[row, i] == "Standard")
                    {
                        well = (row == 0) ? "A" : "B";
                        well += (i+1).ToString();
                        StdList.Add(well);
                    }
                }
            }

            UpdateStdTable();
        }

        private void AutoAddUnkowns()
        {
            if (cur_chan < 0) return;

            experiment exp = CommData.experimentModelData;
            string well;

            for (int row = 0; row < MAX_ROW; row++)
            {
                for (int i = 0; i < MAX_WELL_PER_ROW; i++)
                {
                    if (exp.sampleType[row, i] == "Unknown")
                    {
                        well = (row == 0) ? "A" : "B";
                        well += (i + 1).ToString();
                        UnkList.Add(well);
                    }
                }
            }

            UpdateStdTable1();
        }

        private void Add_Std(object sender, RoutedEventArgs e)
        {
             cur_cb.Background = new SolidColorBrush(Color.FromRgb(230, 160, 20));
            //             gdA1.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            //             gdA1.Visibility = Visibility.Visible;

            StdList.Add(cur_cb.Tag.ToString());

            UpdateStdTable();
        }

        private void Add_Unk(object sender, RoutedEventArgs e)
        {
            cur_cb.Background = new SolidColorBrush(Color.FromRgb(30, 160, 220));

            UnkList.Add(cur_cb.Tag.ToString());

            UpdateStdTable1();
        }

        private void UpdateStdTable()
        {
            //string wstr = "txtStd";
            //string wstr2 = "txtStdConc";

            try
            {

                foreach (var item in gridStd.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock tb = item as TextBlock;
                        if (tb.Name.ToString().Contains("txtStd"))
                        {
                            tb.Tag = null;
                            tb.Text = "";
                        }
                        else if (tb.Name.ToString().Contains("txtStdConc"))
                        {
                            tb.Tag = null;
                            tb.Text = "";
                        }
                    }
                }

                int index = 1;
                foreach (var well in StdList)
                {
                    string wstr = "txtStd";
                    wstr += index.ToString();
                    foreach (var item in gridStd.Children)
                    {
                        if (item is TextBlock)
                        {
                            TextBlock tb = item as TextBlock;
                            if (tb.Name.ToString() == wstr)
                            {
                                tb.Tag = null;
                                tb.Text = well;
                            }
                        }
                    }

                    int k_index = GetWellIndex(well);
                    double ct = CommData.CTValue[cur_chan, k_index];
                    wstr = "txtStdCt";
                    string wstr2 = "txtStdConc";
                    wstr += index.ToString();
                    wstr2 += index.ToString();


                    char[] carr;    // char array
                    carr = well.ToCharArray();

                    int idx = Convert.ToInt32(carr[1].ToString()) - 1;

                    int row;    // 0: A, 1: B

                    if (carr[0] == 'A')
                        row = 0;
                    else
                        row = 1;

                    string quant = CommData.experimentModelData.sampleQuant[row, idx];

                    double qty = Convert.ToDouble(quant);

                    string quante2 = qty.ToString("e2");

                    foreach (var item in gridStd.Children)
                    {
                        if (item is TextBlock)
                        {
                            TextBlock tb = item as TextBlock;
                            if (tb.Name.ToString() == (wstr))
                            {
                                tb.Tag = null;
                                tb.Text = ct.ToString("0.00");
                            }
                            else if (tb.Name.ToString() == (wstr2))
                            {
                                tb.Tag = null;
                                tb.Text = quante2;
                            }
                        }
                    }

                    index++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateStdTable1()
        {
            foreach (var item in gridUnk.Children)
            {
                if (item is TextBlock)
                {
                    TextBlock tb = item as TextBlock;
                    if (tb.Name.ToString().Contains("txtUnk"))
                    {
                        tb.Tag = null;
                        tb.Text = "";
                    }
                    else if (tb.Name.ToString().Contains("txtUnkConc"))
                    {
                        tb.Tag = null;
                        tb.Text = "";
                    }
                    else if (tb.Name.ToString().Contains("txtUnkCt"))
                    {
                        tb.Tag = null;
                        tb.Text = "";
                    }
                }
            }
            try
            {
                int index = 1;
                foreach (var well in UnkList)
                {
                    string wstr = "txtUnk";
                    wstr += index.ToString();
                    foreach (var item in gridUnk.Children)
                    {
                        if (item is TextBlock)
                        {
                            TextBlock tb = item as TextBlock;
                            if (tb.Name.ToString() == (wstr))
                            {
                                tb.Tag = null;
                                tb.Text = well;
                            }
                        }
                    }

                    int k_index = GetWellIndex(well);
                    double ct = CommData.CTValue[cur_chan, k_index];
                    wstr = "txtUnkCt";
                    wstr += index.ToString();

                    foreach (var item in gridUnk.Children)
                    {
                        if (item is TextBlock)
                        {
                            TextBlock tb = item as TextBlock;
                            if (tb.Name.ToString() == (wstr))
                            {
                                tb.Tag = null;
                                tb.Text = ct.ToString("0.00");
                            }
                        }
                    }

                    index++;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public int GetWellIndex(string currks)
        {
            int ksindex = -1;

            if (CommData.KsIndex == 16)
            {
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "A5":
                        ksindex = 4;
                        break;
                    case "A6":
                        ksindex = 5;
                        break;
                    case "A7":
                        ksindex = 6;
                        break;
                    case "A8":
                        ksindex = 7;
                        break;
                    case "B1":
                        ksindex = 8;
                        break;
                    case "B2":
                        ksindex = 9;
                        break;
                    case "B3":
                        ksindex = 10;
                        break;
                    case "B4":
                        ksindex = 11;
                        break;
                    case "B5":
                        ksindex = 12;
                        break;
                    case "B6":
                        ksindex = 13;
                        break;
                    case "B7":
                        ksindex = 14;
                        break;
                    case "B8":
                        ksindex = 15;
                        break;
                }
            }
            else
            {
                switch (currks)
                {
                    case "A1":
                        ksindex = 0;
                        break;
                    case "A2":
                        ksindex = 1;
                        break;
                    case "A3":
                        ksindex = 2;
                        break;
                    case "A4":
                        ksindex = 3;
                        break;
                    case "B1":
                        ksindex = 4;
                        break;
                    case "B2":
                        ksindex = 5;
                        break;
                    case "B3":
                        ksindex = 6;
                        break;
                    case "B4":
                        ksindex = 7;
                        break;
                }
            }

            return ksindex;
        }

        private void Refresh()
        {
            std_conc.Clear();
            std_ct.Clear();

            int index = 1;
            foreach (var well in StdList)
            {
                string wstr;

                int k_index = GetWellIndex(well);
                double ct = CommData.CTValue[cur_chan, k_index];

                wstr = "txtStdConc";
                wstr += index.ToString();

                std_ct.Add(ct);

                double conc = 0;

                foreach (var item in gridStd.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock tb = item as TextBlock;
                        if (tb.Name.ToString() == (wstr))
                        {
                            // tb.Tag = null;
                            if(tb.Text.Count() > 0)
                                conc = Convert.ToDouble(tb.Text);
                        }
                    }
                }

                std_conc.Add(conc);

                index++;
            }

            DrawStdCurve();
        }

        private void DrawStdCurve()
        {
            dcXYDiagram2D.Series.Clear();

            LineSeries2D dxcLs = new LineSeries2D();
            dxcLs.Tag = "Std Curve";
            dxcLs.DisplayName = "Std Curve";
            dxcLs.MarkerVisible = true;
           
            dxcLs.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            dxcLs.Brush = new SolidColorBrush(Color.FromRgb(230, 160, 20));

            LineStyle lstyle = new LineStyle();
            lstyle.Thickness = 1;
            lstyle.DashStyle = DashStyles.Dash;

            dxcLs.LineStyle = lstyle;

            int count = std_ct.Count();

            if (count <= 0) return;

            List<double> log_list = new List<double>();
            double min_x, max_x;

            try
            {
                dxcLs.Tag = "Std Curve";
                dxcLs.DisplayName = "Ct vs. Log(Q)";

                //                dxcLs1.CrosshairLabelPattern = "{S} {A} : {V}";
                dxcLs.CrosshairLabelPattern = "{S} : {V} , {A}";

                for (int i = 0; i < count; i++)
                {
                    SeriesPoint sp = new SeriesPoint();

                    //double id = std_ct[i];
                    //sp.Argument = id.ToString();

                    //sp.Value = Math.Log10(std_conc[i]); 
                    //dxcLs.Points.Add(sp);

                    double id = Math.Log10(std_conc[i]);
                    id = Math.Round(id, 2);
                    sp.Argument = id.ToString();

                    sp.Value = Math.Round(std_ct[i], 2);

                    dxcLs.Points.Add(sp);

                    log_list.Add(id);
                }


                dxcLs.CrosshairLabelVisibility = false; //  CommData.showCrosshairLabel;

                //dxcLs.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;

                min_x = log_list.Min();
                max_x = log_list.Max();

                dcAxisRange.MinValue = 3;
                dcAxisRange.MaxValue = 50;

                dcAxisRange.MinValue = std_ct.Min() - 2;
                dcAxisRange.MaxValue = std_ct.Max() + 2;

                dcAxisRangeX.MinValue = log_list.Min() - 1;
                dcAxisRangeX.MaxValue = log_list.Max() + 1;


                dcXYDiagram2D.Series.Add(dxcLs);

                // Curve fit

                int size = count;

                double[] yy = new double[size];
                double[] xx = new double[size];

                double[] xx1 = new double[11];

                for (int i = 0; i < size; i++)
                {
                    //xx[i] = std_ct[i];
                    //yy[i] = Math.Log10(std_conc[i]);

                    yy[i] = std_ct[i];
                    xx[i] = Math.Log10(std_conc[i]);
                }

                for (int i = 0; i < 11; i++)
                {
                    //                    xx1[i] = (double)(i);

                    xx1[i] = min_x - 0.5 + 0.1 * (double)i * (max_x + 1 - min_x);
                }

                var polyfit = new PolyFit(xx, yy, 1);
                var fitted = polyfit.Fit(xx);

                p2 = polyfit.Coeff[0];
                p1 = polyfit.Coeff[1];

                //=======R^2=============

                double m = 0;

                for (int i = 0; i < size; i++)
                {
                    m += yy[i];
                }

                m /= size;

                double tot = 0;
                double res = 0;
                double r2;

                for (int i = 0; i < size; i++)
                {
                    tot += (yy[i] - m) * (yy[i] - m);
                    res += (yy[i] - fitted[i]) * (yy[i] - fitted[i]);
                }

                if (tot > 0)
                {
                    r2 = 1 - (res / tot);
                }
                else
                {
                    r2 = 1;
                }

                //txtParam1.Text = polyfit.Coeff[1].ToString("0.00");
                //txtParam2.Text = polyfit.Coeff[0].ToString("0.00");

                txtParam1.Text = p1.ToString("0.00");
                txtParam2.Text = p2.ToString("0.00");
                txtParamR2.Text = r2.ToString("0.00000");

                // Transfer StdCurve to Assay

                if (bUpdateAssay)
                {
                    experiment exp = CommData.experimentModelData;

                    exp.assayChanStdSlope[cur_chan] = txtParam1.Text;
                    exp.assayChanStdIntercept[cur_chan] = txtParam2.Text;
                }

                // Amp efficiency

                double expo = -1 / p1;
                double effy = Math.Pow(10, expo) - 1;

                effy *= 100;
                txtAmpEff.Text = "Amp. efficiency = " + effy.ToString("0.0") + "%";

                Debug.Assert(cur_chan >= 0);

                CommData.stdR2[cur_chan] = r2.ToString("0.00000");
                CommData.stdEff[cur_chan] = effy.ToString("0.0");

                //=======================

                var fitted1 = polyfit.Fit(xx1);

                LineSeries2D dxcLs1 = new LineSeries2D();
                dxcLs1.Tag = "Std Curve1";
                // dxcLs1.DisplayName = "Std Curve1";
                dxcLs1.MarkerVisible = false;

                dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
                dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(30, 160, 220));

                LineStyle lstyle1 = new LineStyle();
                lstyle1.Thickness = 1;
                lstyle1.DashStyle = DashStyles.Solid;

                dxcLs1.LineStyle = lstyle1;

                dxcLs1.DisplayName = "Ct vs. Log(Q)";

                //                dxcLs1.CrosshairLabelPattern = "{S} {A} : {V}";
                dxcLs1.CrosshairLabelPattern = "{S} : {V} , {A}";

                for (int i = 0; i < 11; i++)
                {
                    SeriesPoint sp = new SeriesPoint();

                    double id = xx1[i];

                    sp.Argument = id.ToString("0.00");

                    sp.Value = Math.Round(fitted1[i], 2);
                    dxcLs1.Points.Add(sp);
                }

                dcXYDiagram2D.Series.Add(dxcLs1);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Draw Std Curve");
            }
            //===========

            radChart.Animate();
        }

        private void Find_unknown(object sender, RoutedEventArgs e)
        {
            unk_conc.Clear();
            unk_ct.Clear();

            foreach (var well in UnkList)
            {
                int k_index = GetWellIndex(well);
                double ct = CommData.CTValue[cur_chan, k_index];

                unk_ct.Add(ct);
            }

            int size = std_ct.Count();

            if (size < 2)
            {
                MessageBox.Show("Number of standards cannot be less than 2");
                return;
            }

            DrawStdCurve1();        // Curvefitting is done here. Unknowns are calculated

            try
            {
                int index = 1;

                foreach (var well in UnkList)
                {
                    string wstr = "txtUnkConc";
                    wstr += index.ToString();
                    double conc = Math.Pow(10, unk_conc[index - 1]);

                    foreach (var item in gridUnk.Children)
                    {
                        if (item is TextBlock)
                        {
                            TextBlock tb = item as TextBlock;
                            if (tb.Name.ToString() == (wstr))
                            {
                                tb.Tag = null;
                                tb.Text = conc.ToString("e2");
                            }
                        }
                    }

                    index++;

                    //============
                    // Put unknown conc back to CommData

                    char[] carr;    // char array
                    carr = well.ToCharArray();

                    int idx = Convert.ToInt32(carr[1].ToString()) - 1;

                    int row;    // 0: A, 1: B

                    if (carr[0] == 'A')
                        row = 0;
                    else
                        row = 1;

                    CommData.experimentModelData.sampleQuant[row, idx] = conc.ToString("0");


                    //============
                }

                CommData.OutputCsvString();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private double[] InvFit(double[] y)
        {
            int size = y.Length;
            double[] x = new double[size];

            try
            {
                double pp1 = 1 / p1;
                double pp2 = - p2 / p1;

                for (int i= 0; i< size; i++)
                {
                    x[i] = pp1 * y[i] + pp2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return x;
        }

        private void DrawStdCurve1()
        {
            int size = std_ct.Count();

            //double[] yy = new double[size];
            //double[] xx = new double[size];

            //for (int i = 0; i < size; i++)
            //{
            //    xx[i] = std_ct[i];
            //    yy[i] = Math.Log10(std_conc[i]);
            //}

            //var polyfit = new PolyFit(xx, yy, 1);

            //============

            size = unk_ct.Count();

            double[] xx1 = new double[size];

            for (int i = 0; i < size; i++)
            {
                xx1[i] = unk_ct[i];
            }

            //var fitted = polyfit.Fit(xx1);

            if (String.IsNullOrEmpty(txtParam1.Text) || String.IsNullOrEmpty(txtParam2.Text))
                return;

            p1 = Convert.ToDouble(txtParam1.Text);
            p2 = Convert.ToDouble(txtParam2.Text);

            var fitted = InvFit(xx1);

            for (int i = 0; i < size; i++)
            {
                unk_conc.Add(fitted[i]);
            }

            LineSeries2D dxcLs1 = new LineSeries2D();
            dxcLs1.Tag = "Std Curve2";
            dxcLs1.DisplayName = "Std Curve2";
            dxcLs1.MarkerVisible = true;

            dxcLs1.AnimationAutoStartMode = AnimationAutoStartMode.SetStartState;
            dxcLs1.Brush = new SolidColorBrush(Color.FromRgb(30, 160, 220));

            LineStyle lstyle1 = new LineStyle();
            lstyle1.Thickness = 3;
            lstyle1.DashStyle = DashStyles.Solid;

            dxcLs1.LineStyle = lstyle1;

            for (int i = 0; i < size; i++)
            {
                SeriesPoint sp = new SeriesPoint();

                //double id = xx1[i];
                //sp.Argument = id.ToString();

                //sp.Value = fitted[i];
                //dxcLs1.Points.Add(sp);

                double id = fitted[i];
                sp.Argument = id.ToString("0.00");

                sp.Value = xx1[i];
                dxcLs1.Points.Add(sp);
            }

            dcXYDiagram2D.Series.Add(dxcLs1);

            //===========

            radChart.Animate();
        }

        private void Clear_all(object sender, RoutedEventArgs e)
        {
            clear_all();
        }

        private void clear_all()
        { 
            StdList.Clear();
            UnkList.Clear();

            std_conc.Clear();
            std_ct.Clear();
            unk_conc.Clear();
            unk_ct.Clear();

            dcXYDiagram2D.Series.Clear();

            UpdateStdTable();
            UpdateStdTable1();           

            foreach (var item in gridUnk.Children)
            {
                if (item is TextBlock)
                {
                    TextBlock tb = item as TextBlock;
                    if (tb.Name.ToString() == ("txtUnk"))
                    {
                        tb.Tag = null;
                        tb.Text = "";
                    }
                }
            }

            foreach (var item in gridStd.Children)
            {
                if (item is TextBlock)
                {
                    TextBlock tb = item as TextBlock;
                    if (tb.Name.ToString() == ("txtStd"))
                    {
                        tb.Tag = null;
                        tb.Text = "";
                    }
                }
            }

            AutoAddStandards();
            AutoAddUnkowns();

            if (cur_chan < 0) return;

            experiment exp = CommData.experimentModelData;

            txtParam1.Text = exp.assayChanStdSlope[cur_chan];
            txtParam2.Text = exp.assayChanStdIntercept[cur_chan];

            /*
            if (String.IsNullOrEmpty(exp.chanonedes))
                return;

            switch (cur_chan)
            {
                case 0:
                    txtChanName.Text = exp.chanonedes;
                    break;
                case 1:
                    txtChanName.Text = exp.chantwodes;
                    break;
                case 2:
                    txtChanName.Text = exp.chanthreedes;
                    break;
                case 3:
                    txtChanName.Text = exp.chanfourdes;
                    break;

            }
            */

            Refresh();

        }

        private void Chan_sel(object sender, SelectionChangedEventArgs e)
        {          
            cur_chan = cboChansel.SelectedIndex;

            if (cur_chan >= 0) CommData.curStdChan = cur_chan;      // Valid selection

            experiment exp = CommData.experimentModelData;
            if (String.IsNullOrEmpty(exp.chanonedes))
                return;

            switch (cur_chan)
            {
                case 0:
                    txtChanName.Text = exp.chanonedes;
                    break;
                case 1:
                    txtChanName.Text = exp.chantwodes;
                    break;
                case 2:
                    txtChanName.Text = exp.chanthreedes;
                    break;
                case 3:
                    txtChanName.Text = exp.chanfourdes;
                    break;

            }

            clear_all();
        }

        private void cbAssay_Checked(object sender, RoutedEventArgs e)
        {
            bUpdateAssay = true;
        }

        private void cbAssay_Unchecked(object sender, RoutedEventArgs e)
        {
            bUpdateAssay = false;
        }      

        private void clickExportChart(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "PNG File (*.png)|*.png|All files|*.*";//设置文件类型
                                                                //sfd.FileName = string.Format("{0}传导模块", DateTime.Now.ToString("yyyyMMddHHmmss"));//设置默认文件名
            sfd.FileName = CommData.experimentModelData.emname + "-Std-Chart";
            sfd.DefaultExt = "PNG";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名

            if (sfd.ShowDialog() == true)
            {
                try
                {

                    string path = sfd.FileName;
                    RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)this.radChart.ActualWidth, (int)this.radChart.ActualHeight, 96d, 96d, PixelFormats.Default);
                    targetBitmap.Render(this.radChart); PngBitmapEncoder saveEncoder = new PngBitmapEncoder();
                    saveEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                    System.IO.FileStream fs = System.IO.File.Create(path);
                    saveEncoder.Save(fs);
                    fs.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\nNot successful with printing amplification curve image");
                }
            }
        }
    }
}
