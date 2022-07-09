#define ENGLISH_VER

// #define TwoByFour

#define NO_SQLITE
// #define NO_DARKCORRECT

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Anitoa
{
    public class CommData
    {
        private static int MAX_CHAN = 4;
        private static int MAX_WELL = 16;

#if NO_SQLITE
#else
        static string dbPath = "Data Source =" + Environment.CurrentDirectory + "/container.sqlite";
        private static SqLiteHelper sql = new SqLiteHelper(dbPath);
#endif

        public static int cboChan1 = 1;
        public static int cboChan2 = 0;
        public static int cboChan3 = 0;
        public static int cboChan4 = 0;

        // public static int int_time1, int_time2, int_time3, int_time4 = 0;

        public static bool isComplet = false;//实验是否完成
        public static string F_Path = "";//当前文件路径
        public static string F_Path2 = "";//当前文件路径-Melt
        public static bool isImport = false;

        public static int currCycleNum = 0;//当前循环数
        public static t_user user;
        public static int CurrPZJD = 3;
        public static int KsIndex = MAX_WELL;
        public static int TdIndex = MAX_CHAN;       

        public static int Cycle = 0;
        public static int CycleThisPeriod = 0;
        public static int currCyclePeriodIndex = 0;
        public static int imgFrame = 12;
        public static Dictionary<string, List<string>> diclist = new Dictionary<string, List<string>>();
        public static experiment experimentModelData = new experiment();

        public static int currCycleState = 0;
        public static bool deviceFound = false;

        public static string currGuid = "";
        public static Dictionary<string, List<string>> positionlist = new Dictionary<string, List<string>>();

//        public static Dictionary<int, List<int>> row_index = new Dictionary<int, List<int>>();      // Zhimin added
//        public static Dictionary<int, List<int>> col_index = new Dictionary<int, List<int>>();      // data position indices

        public static List<int>[,] row_index = new List<int>[4, 16];
        public static List<int>[,] col_index = new List<int>[4, 16];

        public static int[,,] dark_map = new int[4, 12, 12];

        static int xdvalue = Convert.ToInt32(ConfigurationManager.AppSettings["xdvalue"].ToString());

        public static int gain_mode = 0;    //  1低gain 0高gain

        public static double[,] chan1_kb = new double[12, 6];
        public static double[,] chan1_fpn = new double[2, 12];
        public static double[] chan1_tempcal = new double[12];
        public static int chan1_rampgen = 0;
        public static int[] chan1_auto_v20 = new int[2];
        public static int chan1_auto_v15 = 0;

        public static double[,] chan2_kb = new double[12, 6];
        public static double[,] chan2_fpn = new double[2, 12];
        public static double[] chan2_tempcal = new double[12];
        public static int chan2_rampgen = 0;
        public static int[] chan2_auto_v20 = new int[2];
        public static int chan2_auto_v15 = 0;

        public static double[,] chan3_kb = new double[12, 6];
        public static double[,] chan3_fpn = new double[2, 12];
        public static double[] chan3_tempcal = new double[12];
        public static int chan3_rampgen = 0;
        public static int[] chan3_auto_v20 = new int[2];
        public static int chan3_auto_v15 = 0;

        public static double[,] chan4_kb = new double[12, 6];
        public static double[,] chan4_fpn = new double[2, 12];
        public static double[] chan4_tempcal = new double[12];
        public static int chan4_rampgen = 0;
        public static int[] chan4_auto_v20 = new int[2];
        public static int chan4_auto_v15 = 0;

        public static int[,,] kbi = new int[4, 12, 6];
        public static int[,,] fpni = new int[4, 2, 12];
        public static int[] rampgen = new int[4];
        public static int[] range = new int[4];
        public static int[,] auto_v20 = new int[4,2];
        public static int[] auto_v15 = new int[4];

        public static bool flash_loaded = false;
        public static bool dpinfo_loaded = false;

        public static double[,] m_factorData = new double[4, 100];
        public static int IFMet = 0;    // 0普通实验 1溶解曲线
        public static bool preMelt = false;
        public static bool expSaved = true;

        public static double[,] CTValue = new double[MAX_CHAN, MAX_WELL];
        public static bool[,] falsePositive = new bool[MAX_CHAN, MAX_WELL];
        public static double[,] MTValue = new double[MAX_CHAN, MAX_WELL];

        // public static string[,] sampleName = new string[MAX_CHAN, MAX_WELL];
        // public static string[,] sampleQuant = new string[MAX_CHAN, MAX_WELL];
        // public static string[,] sampleType = new string[MAX_CHAN, MAX_WELL];
        // public static int[,] sampleTypeIndex = new int[MAX_CHAN, MAX_WELL];
        // public static int[,] sampleQuantUnitIndex = new int[MAX_CHAN, MAX_WELL];
        // public static int[,] sampleAssayIndex = new int[MAX_CHAN, MAX_WELL];
        // public static int[,] sampleExtractMethodIndex = new int[MAX_CHAN, MAX_WELL];

        //        public static bool enAutoInt = true;

        public static bool rawData = false;
        public static bool noCt = false;
        public static bool noDarkCorrect = false;
        public static bool noGapRemove = false;
        public static bool showDarkCurve = false;
        public static bool showCtCrosshair = false;
        public static bool showCrosshairLabel = true;

        public static List<float>[] temp_history = new List<float>[2];
        public static byte ver, sn1, sn2, header, well_format;
        public static string id_str = "";
        public static bool load_template = false;

        public static float cycle_time_estimate;
        public static float cycle_time_estimate_melt;
        public static DateTime cycle_start_time;
        public static DateTime program_start_time;
        
        public static Assay assayData = new Assay();
        public static List<Assay> assayList = new List<Assay>();

        public static float int_time_1 = 10;
        public static float int_time_2 = 20;
        public static float int_time_3 = 100;
        public static float int_time_4 = 100;

        public static string dpstr;

        public static int[] cur_factor = new int[MAX_CHAN];
        public static List<int>[] gap_loc = new List<int>[MAX_CHAN];

        private static int old_factori = 15000;
        public static bool run1MeltMode = false;

        public static string csvString, mtCSVString;

        public static int curStdChan = 0;

        public static string[] stdR2 = new string[MAX_CHAN];
        public static string[] stdEff = new string[MAX_CHAN];
        public static int stdUnitIndex;

        public static string verInfo = "Release 1.08 (June 20, 2022)";

        public static int remainTime = 0;       // remaining time in s, can be negative

        public static string openFileName = null;

        public static bool ReadDatapositionFile()
        {
            //判断相应月份文件夹是否存在，没有则创建
            string path = AppDomain.CurrentDomain.BaseDirectory + "trim/dataposition.ini";
            if (!File.Exists(path))
            {
#if DEBUG
#if ENGLISH_VER
                MessageBox.Show("dataposition not found!");
#else
                MessageBox.Show("dataposition 文件不存在！");
#endif
#endif
                return false;
            }
            StreamReader sr = new StreamReader(path, Encoding.Default);
            var line = File.ReadAllLines(path);
            string[] ss = line.ToArray();
            sr.Close();

#if DEBUG
            MessageBox.Show("dataposition 找到并装入！");
#endif

            for (int i = 0; i < ss.Length; i++)
            {
                if (!ss[i].Contains("CHIP"))
                {
                    if (ss[i].Contains("NWELLS"))
                    {
                        string[] strs = ss[i].Split('=');
                        KsIndex = Convert.ToInt32(strs[1]);
                    }
                    if (ss[i].Contains("NCHANNELS"))
                    {
                        string[] strs = ss[i].Split('=');
                        TdIndex = Convert.ToInt32(strs[1]);
                    }

                    continue;
                }
                int startIndex = ss[i].IndexOf("=");//开始位置
                int ssindex = startIndex + 1;
                string str = ss[i].Substring(ssindex, ss[i].Length - ssindex);//从开始位置截取一个新的字符串

                string str1 = ss[i].Substring(0, startIndex);
                switch (str1)
                {
                    case "CHIP1":
                        positionlist["Chip#1"] = GetPList(str);
                        break;
                    case "CHIP2":
                        positionlist["Chip#2"] = GetPList(str);
                        break;
                    case "CHIP3":
                        positionlist["Chip#3"] = GetPList(str);
                        break;
                    case "CHIP4":
                        positionlist["Chip#4"] = GetPList(str);
                        break;
                }              
            }

            return true;
        }

        public static List<string> GetPList(string ss)
        {
            List<string> strlist = new List<string>();
            ss = ss.Replace("A", "0-").Replace("B", "1-").Replace("C", "2-").Replace("D", "3-").Replace("E", "4-").Replace("F", "5-").Replace("G", "6-")
                .Replace("H", "7-").Replace("I", "8-").Replace("J", "9-").Replace("K", "10-").Replace("L", "11-").Replace("M", "12-").Replace("N", "13-").Replace("O", "14-")
                .Replace("P", "15-").Replace("Q", "16-").Replace("R", "17-").Replace("S", "18-").Replace("T", "19-").Replace("U", "20-").Replace("V", "21-").Replace("W", "22-")
                .Replace("X", "23-");

            string[] sts = ss.Split(',');
            strlist = sts.ToList();
            return strlist;
        }

        // Zhimin: This one is used by RunOne and RunTwo. I added divide factor here.
        public static List<ChartData> GetChartData(string chan, int ks, string currks)
        {
            List<ChartData> cdlist = new List<ChartData>();

            if (diclist.Count == 0)
                return cdlist;

            if (!diclist.ContainsKey(chan))
                return cdlist;

            try
            {
                // int n = Convert.ToInt32(diclist[chan].Count / imgFrame);

                //=============== Zhimin: deal with data corruption==============
                /*
                                int l, m, n;
                                l = Convert.ToInt32(diclist[chan].Count);
                                m = imgFrame;

                                if (l % m != 0)
                                {
#if DEBUG
                                    MessageBox.Show("File corruption detected, missing rows");
#endif
                                    n = l / m + 1; // (int)Math.Round(Convert.ToDouble(l / m)) + 1;
                                }
                                else
                                {
                                    n = l / m;
                                }
                */
                //====================================================

                int n = GetCycleNum(chan);

                int ksindex = -1;

                if(KsIndex < 16)
                {
#if TwoByFour
#else
                    if (well_format > 1)
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
                            case "C0":              // dark pixes
                                ksindex = 16;
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
                            case "C0":              // dark pixes
                                ksindex = 16;
                                break;
                        }
                    }
#endif                   
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
                        case "C0":              // dark pixes
                            ksindex = 16;
                            break;
                    }
                }

                if (ksindex == -1)
                {
                    return cdlist;
                }

                int cindex = -1;

                switch (chan)
                {
                    case "Chip#1":
                        cindex = 0;
                        break;
                    case "Chip#2":
                        cindex = 1;
                        break;
                    case "Chip#3":
                        cindex = 2;
                        break;
                    case "Chip#4":
                        cindex = 3;
                        break;
                    default:
                        break;
                }

                int skip = 0;

                for (int i = 0; i < n; i++)     // n is number of image blocks
                {
                    ChartData cd = new ChartData();
                    cd.x = i;

                    int skip_boundary = i * imgFrame - skip;
                    int take_count = imgFrame; 

                    if(skip_boundary+imgFrame >= diclist[chan].Count)
                    {
                        take_count -= skip_boundary + imgFrame - diclist[chan].Count;

                        if(take_count < 0)
                        {
                            take_count = 0;

#if DEBUG
                        MessageBox.Show("File corruption detected, too many missing lines");
#endif

                            continue;
                        }

                        // continue;       // forget about the last block
                    }

                    List<string> strlist = diclist[chan].Skip(skip_boundary).Take(imgFrame).ToList();

                    Dictionary<int, List<string>> datalist = new Dictionary<int, List<string>>();
                    char[] charSeparator = new char[] { ' ' };

                    for (int k = 0; k < strlist.Count; k++)
                    {
                        datalist[k] = strlist[k].Split(charSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }

                    //int rn = Convert.ToInt32(datalist[11][12]);

                    //int factori = Convert.ToInt32(datalist[11][11]);

                    int rn, factori;

                    if (datalist[11].Count < 14)
                    {
                        rn = Convert.ToInt32(datalist[11][12]);
                        factori = Convert.ToInt32(datalist[11][11]);
                    }
                    else
                    {
                        rn = Convert.ToInt32(datalist[11][13]);
                        factori = Convert.ToInt32(datalist[11][12]);
                    }

                    if (i > 0)
                    {
                        if (factori < cur_factor[cindex])
                            gap_loc[cindex].Add(i + 1);
                    }
                    else
                    {
                        gap_loc[cindex].Clear();
                        cur_factor[cindex] = 15000;
                    }

                    cur_factor[cindex] = factori;

                    if (rn != 11 && rn < 12)
                    {
#if DEBUG
                        MessageBox.Show("File corruption detected, last row rn != 11");
#endif
                        skip += rn + 1;
                    }
                    else if(rn > 12)
                    {
#if DEBUG
                        MessageBox.Show("File corruption detected, rn corrupted");
#endif
                        cd.y = 0;
                        cdlist.Add(cd);
                        continue;
                    }

                    // Use flash version of data position index
                    int value = 0;

                    if (ksindex == 16)  // dark
                    {
                        int npoint = 11;
                        bool dark = true;
                        value = 0;
                        for (int j = 1; j < npoint; j++)
                        {
                            value += Convert.ToInt32(datalist[j][11]) - 100;    // last column
                            if (dark_map[cindex, j, 11] > 0)
                                dark = false;
                        }

                        if (!dark)
                        {
                            npoint = 11;
                            dark = true;
                            value = 0;
                            for (int j = 1; j < npoint; j++)
                            {
                                value += Convert.ToInt32(datalist[j][0]) - 100;  // first column
                                if (dark_map[cindex, j, 0] > 0)
                                    dark = false;
                            }
                            Debug.Assert(dark);
                        }

                        value = value * 4 / (npoint - 1);   // normalize to 4 pixels

                    }
                    else if(flash_loaded || dpinfo_loaded)
                    {
                        int npoint = row_index[cindex, ksindex].Count;

                        for (int j = 0; j < npoint; j++)
                        {
                            int row = row_index[cindex, ksindex].ElementAt(j);
                            int col = col_index[cindex, ksindex].ElementAt(j);
                            value += Convert.ToInt32(datalist[row][col]) - 100;
                        }
                    }
                    // Use Data position file version
                    else
                    {
                        if(positionlist.Count == 0)
                        {
                            MessageBox.Show("Position list is empty, this means flash not loaded and trim not loaded");
                            return cdlist;
                        }
                        
                        if(!positionlist.ContainsKey(chan))
                        {
#if DEBUG
                            MessageBox.Show("Position list does not contain the channel key");
#endif
                            return cdlist;
                        }

                        string ss = positionlist[chan][ksindex];


                        string[] newstrs = ss.Split('+');
                        foreach (var item in newstrs)
                        {
                            string[] nstrs = item.Split('-');

                            if (nstrs.Length > 1)
                            {
                                int j = Convert.ToInt32(nstrs[0]);
                                int k = Convert.ToInt32(nstrs[1]);
                                int v = Convert.ToInt32(datalist[k][j]) - 100;
                                value += v;
                            }
                        }
                    }

                    double vf = (double)value;
                    vf /= GetFactor(factori);
                    value = (int)vf;

                    cd.y = value;
                    cdlist.Add(cd);
                    if (i == 0)
                        cdlist.Add(cd); // Zhimin: index 0 take value from index 1
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "Get Chart Data CommData");
#endif
            }

            return cdlist;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public static t_user GetUser(string uname, string pass)
        {
            List<t_user> userlist = new List<t_user>();

#if NO_SQLITE
            if(uname == "user" || (uname == "admin" && pass == "123456")) 
            {
                t_user user = new t_user();
                user.Uname = uname;
                user.Utype = uname == "admin" ? 1 : 0;
                userlist.Add(user);
            }
#else
            string sqlstr = string.Format("select * from t_user where pass='{0}' and (Uname='{1}' or Email='{1}')", pass, uname);
            SQLiteDataReader reader = sql.ExecuteQuery(sqlstr);
            while (reader.Read())
            {
                t_user user = new t_user();
                user.UID = reader.GetInt32(reader.GetOrdinal("UID"));
                user.Uname = reader.GetString(reader.GetOrdinal("Uname"));
               
                //user.Phone = reader.GetString(reader.GetOrdinal("Phone"));
                //user.Email = reader.GetString(reader.GetOrdinal("Email"));
                user.Pass = reader.GetString(reader.GetOrdinal("Pass"));
                user.Utype = reader.GetInt32(reader.GetOrdinal("Utype"));
                userlist.Add(user);
            }
#endif
            if (userlist.Count > 0)
            {
                return userlist[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 添加实验
        /// </summary>
        /// <param name="TPID"></param>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public static bool AddExperiment(experiment experimentModel)
        {
#if NO_SQLITE
            return false;
#else
            string sqlstr = string.Format("INSERT INTO experiment(experimentname, startTime,endTime,CyderNum,ImgFileName,SetFileName,uid) VALUES ('{0}','{1}','{2}',{3},'{4}','{5}',{6})", experimentModel.emname, experimentModel.emdatetime, experimentModel.endatetime, experimentModel.CyderNum, experimentModel.ImgFileName, experimentModel.SetFileName, user.UID);
            bool success = sql.ExecuteSql(sqlstr);

            if(!success)
            {
                MessageBox.Show("实验名重复， 请更改再存");
            }

            return success;
//            string sqlstr = string.Format("INSERT OR REPLACE INTO experiment(experimentname, startTime,endTime,CyderNum,ImgFileName,SetFileName,uid) VALUES ('{0}','{1}','{2}',{3},'{4}','{5}',{6})", experimentModel.emname, experimentModel.emdatetime, experimentModel.endatetime, experimentModel.CyderNum, experimentModel.ImgFileName, experimentModel.SetFileName, CommData.user.UID);
//            return sql.ExecuteSql(sqlstr);
#endif
        }

        /// <summary>
        /// 添加实验
        /// </summary>
        /// <param name="TPID"></param>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public static List<experimentExt> GetExperiment()
        {
            List<experimentExt> experimentList = new List<experimentExt>();
#if NO_SQLITE
#else
            string sqlstr = string.Format("SELECT * from experiment where uid={0} ORDER BY experimentid DESC;", user.UID);
            SQLiteDataReader reader = sql.ExecuteQuery(sqlstr);
            while (reader.Read())
            {
                experimentExt prm = new experimentExt();
                prm.experimentid = reader.GetInt32(reader.GetOrdinal("experimentid"));
                prm.experimentname = reader.GetString(reader.GetOrdinal("experimentname"));
                prm.startTime = reader.GetString(reader.GetOrdinal("startTime"));
                prm.endTime = reader.GetString(reader.GetOrdinal("endTime"));
                prm.CyderNum = reader.GetInt32(reader.GetOrdinal("CyderNum"));
                prm.ImgFileName = reader.GetString(reader.GetOrdinal("ImgFileName"));
                prm.SetFileName = reader.GetString(reader.GetOrdinal("SetFileName"));
                experimentList.Add(prm);
            }
#endif
            return experimentList;
        }

        public static double GetFactor(int value)
        {
            double factor;
            if (value < 5000)
            {
                factor = 1;
            }
            else
            {
                factor = (double)(((double)value - 5000) / 10000);
            }

            Debug.Assert(factor > 0);

#if NO_DARKCORRECT
            return 1;
#else
            return factor;
#endif
        }

        // This is used for calibrate int time

        public static List<ChartData> GetMaxChartData(string chan, int ks, string currks)
        {
            List<ChartData> cdlist = new List<ChartData>();

            if (diclist.Count == 0)
                return cdlist;

            if (!diclist.ContainsKey(chan))
                return cdlist;

            try
            {
                // int n = Convert.ToInt32(diclist[chan].Count / imgFrame);

                int n = GetCycleNum(chan);

                //if (ks == 4)
                //{
                int ksindex = -1;
                if (KsIndex < 16)
                {
#if TwoByFour
#else
                    if (well_format > 1)
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
                        }
                    }
#endif
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
                if (ksindex == -1)
                {
                    return cdlist;
                }

                int cindex = -1;

                switch (chan)
                {
                    case "Chip#1":
                        cindex = 0;
                        break;
                    case "Chip#2":
                        cindex = 1;
                        break;
                    case "Chip#3":
                        cindex = 2;
                        break;
                    case "Chip#4":
                        cindex = 3;
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < n; i++)
                {
                    ChartData cd = new ChartData();
                    cd.x = i;
                    List<string> strlist = diclist[chan].Skip(i * imgFrame).Take(imgFrame).ToList();

                    Dictionary<int, List<string>> datalist = new Dictionary<int, List<string>>();
                    for (int k = 0; k < strlist.Count; k++)
                    {
                        datalist[k] = strlist[k].Split(' ').ToList();
                    }

                    //===========Use flash version of data position index============
                    int value = 0;

                    if (flash_loaded || dpinfo_loaded)
                    {
                        int npoint = row_index[cindex, ksindex].Count;

                        for (int j = 0; j < npoint; j++)
                        {
                            int row = row_index[cindex, ksindex].ElementAt(j);
                            int col = col_index[cindex, ksindex].ElementAt(j);
                            value += Convert.ToInt32(datalist[row][col]) - 100;
                        }
                    }
                    //================================================================
                    else
                    {
                        string ss = positionlist[chan][ksindex];
                        string[] newstrs = ss.Split('+');

                        foreach (var item in newstrs)
                        {
                            string[] nstrs = item.Split('-');
                            if (nstrs.Length > 1)
                            {
                                int j = Convert.ToInt32(nstrs[0]);
                                int k = Convert.ToInt32(nstrs[1]);
                                int v = Convert.ToInt32(datalist[k][j]) - 100 < 0 ? 0 : Convert.ToInt32(datalist[k][j]) - 100;
                                //int v = Convert.ToInt32(datalist[j][k])-100;
                                value += v;
                            }
                        }
                    }

                    cd.y = value;
                    cdlist.Add(cd);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "GetChart Max Data - CommData");
#endif
            }
            return cdlist;
        }

        public static List<ChartDataNew> GetChartDataByRJQX(string chan, int ks, string currks)
        {
            List<ChartDataNew> cdlist = new List<ChartDataNew>();

            if (diclist.Count == 0)
                return cdlist;

            if (!diclist.ContainsKey(chan))
                return cdlist;

            try
            {
                // int n = Convert.ToInt32(diclist[chan].Count / imgFrame);

                int n = GetCycleNum(chan);

                //if (ks == 4)
                //{
                int ksindex = -1;
                if (KsIndex < 16)
                {
#if TwoByFour
#else
                    if (well_format > 1)
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

                        }
                    }
#endif
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
                if (ksindex == -1)
                {
                    return cdlist;
                }

                int cindex = -1;

                switch (chan)
                {
                    case "Chip#1":
                        cindex = 0;
                        break;
                    case "Chip#2":
                        cindex = 1;
                        break;
                    case "Chip#3":
                        cindex = 2;
                        break;
                    case "Chip#4":
                        cindex = 3;
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < n; i++)
                {                   
                    List<string> strlist = diclist[chan].Skip(i * imgFrame).Take(imgFrame).ToList();
                    if (strlist.Count == 0)
                        continue;

                    if(strlist.Count < 12)
                    {
#if DEBUG
                        MessageBox.Show("File corruption detected, strlist.Count < 12");
#endif
                        continue;
                    }

                    ChartDataNew cd = new ChartDataNew();
                    string[] strs = strlist[0].Split(' ');

                    //为方便调试暂时注释掉的
                    //if (strs.Length < 14) continue;
                    //cd.x = strs[13];

                    if (strs.Count() < imgFrame + 5)
                    {
                        cd.x = "0.0"; // i.ToString();  //测试用, the first 5 frames are from AutoInt
                    }
                    // Zhimin Ding added 5-5-2019
                    else
                    {
                        byte[] buffers = new byte[4];
                        buffers[0] = Convert.ToByte(strs[imgFrame + 1]);
                        buffers[1] = Convert.ToByte(strs[imgFrame + 2]);
                        buffers[2] = Convert.ToByte(strs[imgFrame + 3]);
                        buffers[3] = Convert.ToByte(strs[imgFrame + 4]);

                        float t = BitConverter.ToSingle(buffers, 0);
                        cd.x = t.ToString();
                    }

                    Dictionary<int, List<string>> datalist = new Dictionary<int, List<string>>();
                    for (int k = 0; k < strlist.Count; k++)
                    {
                        datalist[k] = strlist[k].Split(' ').ToList();
                    }

                    int rn = Convert.ToInt32(datalist[11][12]);                   

                    if (rn != 11 && rn < 12)
                    {
#if DEBUG
                        MessageBox.Show("File corruption detected, last row rn != 11, in GetChart RJQX");
#endif
                    }

                    //===========Use flash version of data position index============
                    int value = 0;

                    if (flash_loaded || dpinfo_loaded)
                    {
                        int npoint = row_index[cindex, ksindex].Count;

                        for (int j = 0; j < npoint; j++)
                        {
                            int row = row_index[cindex, ksindex].ElementAt(j);
                            int col = col_index[cindex, ksindex].ElementAt(j);
                            value += Convert.ToInt32(datalist[row][col]) - 100;
                        }
                    }
                    //================================================================
                    else
                    {
                        string ss = positionlist[chan][ksindex];
                        string[] newstrs = ss.Split('+');

                        foreach (var item in newstrs)
                        {
                            string[] nstrs = item.Split('-');

                            if (nstrs.Length > 1)
                            {
                                int j = Convert.ToInt32(nstrs[0]);
                                int k = Convert.ToInt32(nstrs[1]);
                                int v = Convert.ToInt32(datalist[k][j]) - 100;
                                value += v;
                            }
                        }
                    }

                    int factori = Convert.ToInt32(datalist[11][11]);

                    if (i == 0)
                        old_factori = 15000;        // looks ike int time update in melt lags behind in HW, causing glitches

                    double vf = (double)value;
                    vf /= GetFactor(old_factori);
                    value = (int)vf;

                    old_factori = factori;

                    cd.y = value.ToString();
                    cdlist.Add(cd);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "GetChart Data RJQX CommData");
#endif
            }


            return cdlist;
        }

        public static void InitWellNames()
        {
            if(KsIndex == 4)
            {
                experimentModelData.A1des = "S1";
                experimentModelData.A2des = "S2";
                experimentModelData.A3des = "S3";
                experimentModelData.A4des = "S4";
            }
            else if(KsIndex == 8)
            {
                experimentModelData.A1des = "S1";
                experimentModelData.A2des = "S2";
                experimentModelData.A3des = "S3";
                experimentModelData.A4des = "S4";
                //experimentModelData.A5des = "S5";
                //experimentModelData.A6des = "S6";
                //experimentModelData.A7des = "S7";
                //experimentModelData.A8des = "S8";

                experimentModelData.B1des = "S5";
                experimentModelData.B2des = "S6";
                experimentModelData.B3des = "S7";
                experimentModelData.B4des = "S8";

            }
            else
            {
                experimentModelData.A1des = "S1";
                experimentModelData.A2des = "S2";
                experimentModelData.A3des = "S3";
                experimentModelData.A4des = "S4";
                experimentModelData.A5des = "S5";
                experimentModelData.A6des = "S6";
                experimentModelData.A7des = "S7";
                experimentModelData.A8des = "S8";

                experimentModelData.B1des = "S9";
                experimentModelData.B2des = "S10";
                experimentModelData.B3des = "S11";
                experimentModelData.B4des = "S12";
                experimentModelData.B5des = "S13";
                experimentModelData.B6des = "S14";
                experimentModelData.B7des = "S15";
                experimentModelData.B8des = "S16";
            }
        }

        // Parse data file and read data into dictionary. Update experiment ampdata and number of cycles.

        public static void ReadFileData(string path, int type, int caller, int program)
        {
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);

                if (sr == null)
                    return;

                var line = File.ReadAllLines(path);
                string[] ss = line.ToArray();
                sr.Close();

                diclist.Clear();     // Necessary? Free up memory

                diclist = new Dictionary<string, List<string>>();
                string name = "";
                bool dpheader = false;

                foreach (var item in ss)
                {
                    if (string.IsNullOrEmpty(item)) continue;

                    if (item.Contains("Chipdp"))
                    {
                        dpheader = true;
                    }
                    else if (item.Contains("Chip#"))
                    {
                        name = item;
                        dpheader = false;
                        if (!diclist.Keys.Contains(name))
                        {
                            diclist[name] = new List<string>();
                        }
                    }
                    else
                    {
                        if (!dpheader)
                        {
                            if (item.Contains("Chip#"))
                                continue;
                            diclist[name].Add(item);
                        }
                        else
                        {
                            string dpstr = item;
                            if (type > 0) ParseDpstr(dpstr);
                        }
                    }
                }

                if (program == 0)
                {
                    experimentModelData.ampData = diclist;
                }
                else if(program == 1)
                {
                    experimentModelData.meltData = diclist;
                }

                if (true)
                {
                    foreach (var item in diclist.Keys)
                    {
                        if (diclist[item].Count == 0) continue;
                        
                        //  CommData.Cycle = Convert.ToInt32(CommData.diclist[item].Count / CommData.imgFrame);

                        int m, n;

                        m = Convert.ToInt32(diclist[item].Count);
                        n = imgFrame;


                        if (m % n != 0)
                        {
#if DEBUG
                            MessageBox.Show("File corruption detected. Missing rows");
#endif
                            Cycle = m / n + 1; // (int)Math.Round(Convert.ToDouble(m / n)) + 1;
                        }
                        else
                        {
                            Cycle = m / n;
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "Read File Data CommData. Caller is:" + caller.ToString() + " (0: main; 1: RunOne)");
#endif
            }
        }

        public static void ParseDpstr(string dpstr)
        {
            if (string.IsNullOrEmpty(dpstr))
                return;

            string[] s = dpstr.Split(' ');
            int len = s.Length - 1;                 // because last string character is empty;

            byte[] trim_buff = new byte[len];

            for (int i = 0; i < len; i++)
            {
                trim_buff[i] = Convert.ToByte(s[i]);
            }

            ParseDpstr(trim_buff, len);

            return;
            
            List<int> rlist = new List<int>();      // row index
            List<int> clist = new List<int>();      // col index                           

            int k = 0;

            byte version = trim_buff[k]; k++;
            byte snumber1 = trim_buff[k]; k++;
            byte snumber2 = trim_buff[k]; k++;

            int num_channels = trim_buff[k]; k++;
            int num_wells = trim_buff[k]; k++;
            int num_pages = trim_buff[k]; k++;

            KsIndex = num_wells;
            TdIndex = num_channels;

            ver = version;
            sn1 = snumber1;
            sn2 = snumber2;

            for (int i = 0; i < num_channels; i++)
            {
                for (int j = 0; j < num_wells; j++)
                {
                    int n = trim_buff[k]; k++;
                    rlist.Clear();
                    clist.Clear();
                    for (int l = 0; l < n; l++)
                    {
                        int row = trim_buff[k++]; // k++;
                        int col = trim_buff[k]; k++;

                        rlist.Add(row);
                        clist.Add(col);
                    }
                    row_index[i, j] = new List<int>(rlist);
                    col_index[i, j] = new List<int>(clist);
                }
            }

            dpinfo_loaded = true;
            UpdateDarkMap();
        }

        public static void ParseDpstr(byte[] trim_buff, int len)
        {
            /*            trim_buff = new byte[len];

                        for (int i = 0; i < len; i++)
                        {
                            trim_buff[i] = Convert.ToByte(s[i]);
                        }

                        List<int> rlist = new List<int>();      // row index
                        List<int> clist = new List<int>();      // col index                           

                        int k = 0;

                        byte version = trim_buff[k]; k++;
                        byte snumber1 = trim_buff[k]; k++;
                        byte snumber2 = trim_buff[k]; k++;

                        int num_channels = trim_buff[k]; k++;
                        int num_wells = trim_buff[k]; k++;
                        int num_pages = trim_buff[k]; k++;

                        KsIndex = num_wells;
                        TdIndex = num_channels;

                        ver = version;
                        sn1 = snumber1;
                        sn2 = snumber2;

                        for (int i = 0; i < num_channels; i++)
                        {
                            for (int j = 0; j < num_wells; j++)
                            {
                                int n = trim_buff[k]; k++;
                                rlist.Clear();
                                clist.Clear();
                                for (int l = 0; l < n; l++)
                                {
                                    int row = trim_buff[k++]; // k++;
                                    int col = trim_buff[k]; k++;

                                    rlist.Add(row);
                                    clist.Add(col);
                                }
                                row_index[i, j] = new List<int>(rlist);
                                col_index[i, j] = new List<int>(clist);
                            }
                        }
            */

            //===================

            List<int> rlist = new List<int>();      // row index
            List<int> clist = new List<int>();      // col index

            int k = 0;

            byte header = trim_buff[k]; k++;

            byte sn1 = 0, sn2 = 0, version = 0, well_format = 0;
            int num_channels = 4, num_wells = 16, num_pages = 1;            

            StringBuilder sb = new StringBuilder();

            if (header != 0xa5)
            {
                version = header;
                sn1 = trim_buff[k]; k++;
                sn2 = trim_buff[k]; k++;

                num_channels = trim_buff[k]; k++;
                num_wells = trim_buff[k]; k++;
                num_pages = trim_buff[k]; k++;
            }
            else
            {
                version = trim_buff[k]; k++;
                num_pages = trim_buff[k]; k++;

                // id_str.clear();
                for (int i = 0; i < 32; i++)
                {
                    char idc = (char)trim_buff[k]; k++;
                    sb.Append(idc);
                }

                sn1 = trim_buff[k]; k++;
                sn2 = trim_buff[k]; k++;

                num_channels = trim_buff[k]; k++;
                num_wells = trim_buff[k]; k++;

                well_format = trim_buff[k]; k++;

                byte rsv;
                rsv = trim_buff[k]; k++;    // Reserved bytes
                rsv = trim_buff[k]; k++;
                rsv = trim_buff[k]; k++;
                rsv = trim_buff[k]; k++;
            }

            KsIndex = num_wells;
            TdIndex = num_channels;

            ver = version;
            CommData.sn1 = sn1;
            CommData.sn2 = sn2;
            CommData.header = header;

            CommData.id_str = sb.ToString();
            CommData.well_format = well_format;

            if (num_channels <= 2)
            {
                CommData.experimentModelData.CbooChan3 = false;
                CommData.experimentModelData.CbooChan4 = false;
            }

            for (int i = 0; i < num_channels; i++)
            {
                for (int j = 0; j < num_wells; j++)
                {
                    int n = trim_buff[k]; k++;
                    rlist.Clear();
                    clist.Clear();
                    for (int l = 0; l < n; l++)
                    {
                        int row = trim_buff[k++]; // k++;
                        int col = trim_buff[k]; k++;

                        rlist.Add(row);
                        clist.Add(col);
                    }

                    CommData.row_index[i, j] = new List<int>(rlist);
                    CommData.col_index[i, j] = new List<int>(clist);
                }
            }

            //===================

            dpinfo_loaded = true;
            UpdateDarkMap();
        }

        public static bool IsEmptyWell(int chan, int index)
        {
            if (chan + 1 > TdIndex)
                return true;

            switch(chan)
            {
                case 0:
                    if (cboChan1 == 0) return true;
                    else break;
                case 1:
                    if (cboChan2 == 0) return true;
                    else break;
                case 2:
                    if (cboChan3 == 0) return true;
                    else break;
                case 3:
                    if (cboChan4 == 0) return true;
                    else break;
                default:
                    break;
            }

            if (KsIndex == 4)
            {
                switch(index)
                {
                    case 0:
                        return string.IsNullOrEmpty(experimentModelData.A1des);
                    case 1:
                        return string.IsNullOrEmpty(experimentModelData.A2des);
                    case 2:
                        return string.IsNullOrEmpty(experimentModelData.A3des);
                    case 3:
                        return string.IsNullOrEmpty(experimentModelData.A4des);
                    default:
                        return true;
                }
            }

            else if(KsIndex == 8)
            {
                switch (index)
                {
                    case 0:
                        return string.IsNullOrEmpty(experimentModelData.A1des);
                    case 1:
                        return string.IsNullOrEmpty(experimentModelData.A2des);
                    case 2:
                        return string.IsNullOrEmpty(experimentModelData.A3des);
                    case 3:
                        return string.IsNullOrEmpty(experimentModelData.A4des);
                    case 4:
                        return string.IsNullOrEmpty(experimentModelData.B1des);
                    case 5:
                        return string.IsNullOrEmpty(experimentModelData.B2des);
                    case 6:
                        return string.IsNullOrEmpty(experimentModelData.B3des);
                    case 7:
                        return string.IsNullOrEmpty(experimentModelData.B4des);
                    default:
                        return true;
                }
            }

            else
            {
                switch (index)
                {
                    case 0:
                        return string.IsNullOrEmpty(experimentModelData.A1des);
                    case 1:
                        return string.IsNullOrEmpty(experimentModelData.A2des);
                    case 2:
                        return string.IsNullOrEmpty(experimentModelData.A3des);
                    case 3:
                        return string.IsNullOrEmpty(experimentModelData.A4des);
                    case 4:
                        return string.IsNullOrEmpty(experimentModelData.A5des);
                    case 5:
                        return string.IsNullOrEmpty(experimentModelData.A6des);
                    case 6:
                        return string.IsNullOrEmpty(experimentModelData.A7des);
                    case 7:
                        return string.IsNullOrEmpty(experimentModelData.A8des);

                    case 8:
                        return string.IsNullOrEmpty(experimentModelData.B1des);
                    case 9:
                        return string.IsNullOrEmpty(experimentModelData.B2des);
                    case 10:
                        return string.IsNullOrEmpty(experimentModelData.B3des);
                    case 11:
                        return string.IsNullOrEmpty(experimentModelData.B4des);
                    case 12:
                        return string.IsNullOrEmpty(experimentModelData.B5des);
                    case 13:
                        return string.IsNullOrEmpty(experimentModelData.B6des);
                    case 14:
                        return string.IsNullOrEmpty(experimentModelData.B7des);
                    case 15:
                        return string.IsNullOrEmpty(experimentModelData.B8des);

                    default:
                        return true;
                }
            }
        }

        public static int GetCycleNum(string td)        // "td" stands for TongDao, or Channel in English
        {
            try
            {
                if (diclist.Count == 0 || !diclist.ContainsKey(td))
                    return 0;

                // cyclenum = Convert.ToInt32(CommData.diclist[tdlist[i]].Count / CommData.imgFrame);

                int l, m, n;

                l = Convert.ToInt32(diclist[td].Count);
                m = imgFrame;

                if (l % m != 0)
                {
#if DEBUG
                    MessageBox.Show("File corruption detected, missing rows");
#endif
                    n = l / m + 1; //  (int)Math.Round(Convert.ToDouble(l / m)) + 1;
                }
                else
                {
                    n = l / m;
                }

                return n;
            //====================================================
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "CommData -- get cycle number");
#endif
                return 0;
            }

        }

        public static void EstimateCycleTime()
        {
            if (experimentModelData.DebugModelDataList == null)
                return;

            try
            {
                TempModel Tm = new TempModel();

                float init_temp;

                if(experimentModelData.DebugModelDataList[0].StepCount > 2)
                {
                    init_temp = 72;
                }
                else
                {
                    init_temp = 60;
                }

                Tm.SetInitTemp(init_temp);
                Tm.SimStep(init_temp, 3, -1, 0, 0);

                float ct = Tm.SimStep((float)experimentModelData.DebugModelDataList[0].Denaturating,
                    (float)experimentModelData.DebugModelDataList[0].DenaturatingTime, -1, experimentModelData.overTemp, experimentModelData.overTime);

                ct += Tm.SimStep((float)experimentModelData.DebugModelDataList[0].Annealing,
                    (float)experimentModelData.DebugModelDataList[0].AnnealingTime, -1, experimentModelData.underTemp, experimentModelData.underTime);

                if (experimentModelData.DebugModelDataList[0].StepCount > 2)
                    ct += Tm.SimStep((float)experimentModelData.DebugModelDataList[0].Extension,
                    (float)experimentModelData.DebugModelDataList[0].ExtensionTime, -1, experimentModelData.overTemp, experimentModelData.overTime);

                cycle_time_estimate = ct + 5;

                double diff = experimentModelData.DebugModelDataList[0].MeltEnd - experimentModelData.DebugModelDataList[0].MeltStart;
                cycle_time_estimate_melt = 7.8f * Convert.ToSingle(diff);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Est cycle time");
            }
        }

        public static void EstimateCycleTimeMelt()
        {
            if (experimentModelData.DebugModelDataList == null)
                return;

            try
            {
                double diff = experimentModelData.DebugModelDataList[0].MeltEnd - experimentModelData.DebugModelDataList[0].MeltStart;
                cycle_time_estimate_melt = 7.8f * Convert.ToSingle(diff);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Est cycle time melt");
            }
        }

        // include the current cycle
        public static string EstimateRemainTime()
        {
            //            int currCycle = currCycleNum;
            // int TocalCycle = experimentModelData.CyderNum;

            if (experimentModelData.DebugModelDataList.Count > 1)
                return EstimateRemainTime2();

            int TocalCycle = CommData.CycleThisPeriod;

            //            int adder = 60;    // Heat lid ramp up time;
            int ToaTimeCount = 0;

            if (currCycleState == 1 || currCycleState == 2)
            {
                if (currCycleNum < 1)
                {
                    int adder = 65 + (int)experimentModelData.DebugModelDataList[0].InitaldenaTime;

                    if (experimentModelData.DebugModelDataList[0].StepCount > 1)
                        adder += 3 + (int)experimentModelData.DebugModelDataList[0].InitaldenaTime2;

                    ToaTimeCount = adder + (int)(cycle_time_estimate * TocalCycle);
                }
                else
                {
                    ToaTimeCount = (int)(cycle_time_estimate * (TocalCycle - currCycleNum + 1));
                }

                ToaTimeCount += (int)experimentModelData.DebugModelDataList[0].HoldonTime + 2  - (int)GetElapsedTimeCycle();

                remainTime = ToaTimeCount;

                if (ToaTimeCount < 0) ToaTimeCount = 0;
            }

            TimeSpan t = new TimeSpan(0, 0, ToaTimeCount);
            string Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);

            if(experimentModelData.DebugModelDataList[0].enMelt)
            {

                TimeSpan tt = new TimeSpan(0, 0, (int)cycle_time_estimate_melt);
                string Text2 = string.Format("{0:00}:{1:00}:{2:00}", tt.Hours, tt.Minutes, tt.Seconds);

                Text += " (+melt: " + Text2 + ")";
            }

            return Text;
        }

        public static string EstimateRemainTime2()
        {
            int totalRemainCycles = 0;

            for(int i = currCyclePeriodIndex; i < experimentModelData.DebugModelDataList.Count; i++)
            {
                totalRemainCycles += experimentModelData.DebugModelDataList[i].Cycle;
            }

            int timeCount = 0;

            if (currCycleState == 1 || currCycleState == 2)
            {
                if (currCycleNum < 1 && currCyclePeriodIndex == 0)
                {
                    int adder = 65 + (int)experimentModelData.DebugModelDataList[0].InitaldenaTime;

                    if (experimentModelData.DebugModelDataList[0].StepCount > 1)
                        adder += 3 + (int)experimentModelData.DebugModelDataList[0].InitaldenaTime2;

                    timeCount = adder + (int)(cycle_time_estimate * totalRemainCycles);
                }
                else
                {
                    timeCount = (int)(cycle_time_estimate * (totalRemainCycles - currCycleNum + 1));
                }

                timeCount += (int)experimentModelData.DebugModelDataList[0].HoldonTime + 2 - (int)GetElapsedTimeCycle();

                if (timeCount < 0) timeCount = 0;
            }

            TimeSpan t = new TimeSpan(0, 0, timeCount);
            string Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);

            if (experimentModelData.DebugModelDataList[0].enMelt)
            {

                TimeSpan tt = new TimeSpan(0, 0, (int)cycle_time_estimate_melt);
                string Text2 = string.Format("{0:00}:{1:00}:{2:00}", tt.Hours, tt.Minutes, tt.Seconds);

                Text += " (+melt: " + Text2 + ")";
            }

            return Text;
        }

        // include the current cycle
        public static string EstimateRemainTimeMelt()
        {
            int ToaTimeCount = 0;

            DateTime t0 = Convert.ToDateTime(cycle_start_time);
            DateTime t1 = DateTime.Now;

            double et = t1.Subtract(t0).TotalSeconds;

            if (currCycleState == 4 || currCycleState == 5)
            {

                ToaTimeCount = (int)(cycle_time_estimate_melt);

                ToaTimeCount -= (int)et;

                if (ToaTimeCount < 0) ToaTimeCount = 0;
            }

            TimeSpan t = new TimeSpan(0, 0, ToaTimeCount);
            string Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
            return Text;
        }

        public static double GetElapsedTime()
        {
            DateTime t0 = Convert.ToDateTime(experimentModelData.emdatetime);
            DateTime t1 = DateTime.Now;

            double et = t1.Subtract(t0).TotalSeconds;

            return et;
        }

        public static double GetElapsedTimeCycle()
        {
            DateTime t0 = Convert.ToDateTime(cycle_start_time);
            DateTime t1 = DateTime.Now;

            double et = t1.Subtract(t0).TotalSeconds;

            //if(et > cycle_time_estimate && currCycleState == 2)
            //{
            //    et = cycle_time_estimate;
            //}

            return et;
        }

        public static void UpdateDarkMap()
        {
            for (int i = 0; i < TdIndex; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    for (int k = 0; k < 12; k++)
                    {
                        dark_map[i, j, k] = 0;
                    }
                }
            }

            for (int i = 0; i < TdIndex; i++)
            {
                for (int j = 0; j < KsIndex; j++)
                {
                    int npoint = row_index[i, j].Count;

                    for (int k = 0; k < npoint; k++)
                    {
                        int row = row_index[i, j].ElementAt(k);
                        int col = col_index[i, j].ElementAt(k);
                        dark_map[i, row, col] += 1;
                    }
                }
            }
        }

        public static string PrintCSVReport()
        {
            try
            {
                string outputString = "Experiment Name, " + experimentModelData.emname + "\r\n";

                outputString += "Experiment Time (start-end), " + experimentModelData.emdatetime + " - " + experimentModelData.endatetime + "\r\n";
                byte[] ba = new byte[1];
                ba[0] = CommData.ver;
                string ascii = System.Text.Encoding.ASCII.GetString(ba);

                if (CommData.header == 0xa5)
                {
                    ascii = CommData.id_str;
                }

                outputString += "Equipment ID, " + ascii + "-" + CommData.sn1.ToString() + "-" + CommData.sn2.ToString() + "\r\n";

                outputString += "Experiment Result - Ct:" + "\r\n";
                outputString += csvString;

                if (CommData.experimentModelData.DebugModelDataList != null)
                {
                    if (CommData.experimentModelData.DebugModelDataList[0].enMelt)
                    {
                        outputString += "Experiment Result - Melt:" + "\r\n";
                        outputString += mtCSVString;
                    }
                }
                outputString += printCyclerString();

                return outputString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "CSV Print CommData");
                return null;
            }
        }

        public static string printCyclerString()
        {
            string result = "Experiment Setting - Cycler:" + "\r\n";

            if (experimentModelData.DebugModelDataList == null || experimentModelData.DebugModelDataList.Count == 0)
                return result;

            DebugModelData dmd = experimentModelData.DebugModelDataList[0];

            result += "Initial Denature Temp., " + dmd.Initaldenaturation.ToString("0.0") + "\r\n";
            result += "Initial Denature Time, " + dmd.InitaldenaTime.ToString("0.0") + "\r\n";

            result += "Denature Temp., " + dmd.Denaturating.ToString("0.0") + "\r\n";
            result += "Denature Time, " + dmd.DenaturatingTime.ToString("0.0") + "\r\n";

            result += "Annealing Temp., " + dmd.Annealing.ToString("0.0") + "\r\n";
            result += "Annealing Time, " + dmd.AnnealingTime.ToString("0.0") + "\r\n";

            result += "Hold Temp., " + dmd.Holdon.ToString("0.0") + "\r\n";
            result += "Hold Time, " + dmd.HoldonTime.ToString("0.0") + "\r\n";

            result += "Number of cycles, " + dmd.Cycle.ToString() + "\r\n";

            if (dmd.StepCount > 2)
            {
                result += "Extension Temp., " + dmd.Extension.ToString("0.0") + "\r\n";
                result += "Extension Time, " + dmd.ExtensionTime.ToString("0.0") + "\r\n";
            }

            if (dmd.enMelt)
            {
                result += "Melt Start Temp., " + dmd.MeltStart.ToString("0.0") + "\r\n";
                result += "Melt End Temp., " + dmd.MeltEnd.ToString("0.0") + "\r\n";
            }

            result += "Hot Lid Temp., " + dmd.Hotlid.ToString("0.0") + "\r\n";

            result += "Standard Slope Parameters:" + "\r\n";
            experiment emd = experimentModelData;

            for (int i = 0; i < TdIndex; i++)
            {
                if (!string.IsNullOrEmpty(emd.assayChanStdSlope[i]))
                {
                    result += "Channel " + (i + 1).ToString() + " (" + emd.AssayChanName(i) + "), " + "Standard curve slope, " + emd.assayChanStdSlope[i] + "\r\n";
                    result += "Channel " + (i + 1).ToString() + " (" + emd.AssayChanName(i) + "), " + "Standard curve intercept, " + emd.assayChanStdIntercept[i] + "\r\n";
                    result += "Channel " + (i + 1).ToString() + " (" + emd.AssayChanName(i) + "), " + "Standard curve R2, " + CommData.stdR2[i] + "\r\n";
                    result += "Channel " + (i + 1).ToString() + " (" + emd.AssayChanName(i) + "), " + "Standard curve amp efficiency(%), " + CommData.stdEff[i] + "\r\n";
                }
            }

            return result;
        }

        public static void OutputCsvString()
        {

            const int NUM_ROW = 64;

            //==========Check result==========

            List<string> nameList = new List<string>();
            List<string> resultList = new List<string>();
            List<string> targetList = new List<string>();
            List<string> concList = new List<string>();

            List<string> sampleList = new List<string>();
            List<string> ctList = new List<string>();
            List<string> typeList = new List<string>();

            string quantUnit = "";

            experiment emd = CommData.experimentModelData;

            for (int i = 0; i < CommData.KsIndex; i++)     // wells
            {
                int wi = i;
                int row = 0;

#if TwoByFour
                if (CommData.KsIndex == 8)
                {
                    if (i >= 4)
                    {
                        wi -= 4;
                        row = 1;
                    }
                }
                else
                {
                    if (i >= 8)
                    {
                        wi -= 8;
                        row = 1;
                    }
                }
#else
                if (well_format > 1)
                {
                    if (CommData.KsIndex == 8)
                    {
                        if (i >= 4)
                        {
                            wi -= 4;
                            row = 1;
                        }
                    }
                    else
                    {
                        if (i >= 8)
                        {
                            wi -= 8;
                            row = 1;
                        }
                    }
                }
                else
                {
                    if (i >= 8)
                    {
                        wi -= 8;
                        row = 1;
                    }
                }
#endif

                if (!string.IsNullOrEmpty(emd.sampleName[row, wi]))
                {
                    // int aindex = emd.sampleAssayIndex[row, wi];
                    // Assay assay = CommData.assayList[aindex];


                    string type = emd.sampleType[row, wi];

                    for (int j = 0; j < 4; j++)     // channels
                    {
                        // ChannelParam cparam = assay.channelParamLists[j];

                        if (emd.AssayChanEn(j)) // (cparam.active)
                        {
                            //float nokct = cparam.negCtrlOKCt;
                            //float pokctstart = cparam.posCtrlOKCtStart;
                            //float pokctend = cparam.posCtrlOKCtEnd;
                            //float stdslope = cparam.stdCurveSlope;
                            //float stdinter = cparam.stdCurveIntercept;
                            //string stype = cparam.type;

                            float ct = (float)CTValue[j, i];

                            if (falsePositive[j, i])
                                ct = 0;

                            string str;

                            if (row == 0) str = "A" + (wi + 1).ToString() + "-" + (j + 1).ToString();
                            else str = "B" + (wi + 1).ToString() + "-" + (j + 1).ToString();

                            string cstr = null;

                            sampleList.Add(emd.sampleName[row, wi]);
                            typeList.Add(type);
                            ctList.Add(ct.ToString("0.00"));

                            nameList.Add(str);
                            // target[i] = cparam.name;
                            targetList.Add(emd.AssayChanName(j)); //  (cparam.name);

                            quantUnit = emd.sampleQuantUnit[row, wi];

                            //if (type == "Positive control" || stype == "IC")
                            //{
                            //    if (ct < pokctend && ct > pokctstart)
                            //    {
                            //        str = "Pass";
                            //    }
                            //    else
                            //    {
                            //        str = "Fail";
                            //    }
                            //}
                            //else if (type == "Unknown")
                            //{
                            //    if (ct < 36 && ct > 0.1)
                            //    {
                            //        str = "Detected";
                            //    }
                            //    else
                            //    {
                            //        str = "Undetected";
                            //    }

                            //    if (ct > 20 && Math.Abs(stdslope) > 0.1)
                            //    {
                            //        double x = (ct -  stdinter) / stdslope;
                            //        float cc = (float)Math.Pow(10, x);

                            //        float ratio = 0;
                            //        if(emd.sampleExtractMethodIndex[row, wi] == 1)     // field
                            //        {
                            //            ratio = assay.extractionLabElution / assay.extractionLabTest;
                            //        }
                            //        else
                            //        {
                            //            ratio = assay.extractionFieldElution / assay.extractionFieldTest;
                            //        }

                            //        float qt = Convert.ToSingle(emd.sampleQuant[row, wi]);

                            //        if (qt < 0.01) continue;

                            //        cc = cc * ratio / qt;

                            //        cc = cc / cparam.cellConversionFactor;

                            //        cstr = cc.ToString("e2") + " " + cparam.finalUnits + " / " + emd.sampleQuantUnit[row, wi];
                            //    }
                            //}
                            //else if (type == "Negative control")
                            //{
                            //    if (ct > nokct || ct < 0.1)
                            //    {
                            //        str = "Pass";
                            //    }
                            //    else
                            //    {
                            //        str = "Fail";
                            //    }
                            //}
                            //else if (type == "Standard")
                            //{
                            //}

                            str = "";

                            if (type == "Standard" || type == "Unknown")
                            {
                                cstr = emd.sampleQuant[row, wi];
                            }
                            else if (type == "Negative control")
                            {
                                if (ct > 38 || ct < 0.1)
                                {
                                    str = "Pass";
                                }
                                else
                                {
                                    str = "Fail";
                                }
                            }
                            else if (type == "Positive control")
                            {
                                if (ct < 44 && ct > 0.1)
                                {
                                    str = "Pass";
                                }
                                else
                                {
                                    str = "?";
                                }
                            }

                            resultList.Add(str);

                            concList.Add(cstr);
                        }
                    }
                }
            }

            string result_str = "Well-Chan, Sample, Type, Target, Ct, Result, Concentration";
            result_str += "(" + quantUnit + ")\r\n";

            for (int i = 0; i < NUM_ROW; i++)
            {
                if (i >= nameList.Count) continue;
                result_str += nameList[i] + ", " + sampleList[i] + ", " + typeList[i] + ", " + targetList[i] + ", " + ctList[i] + ", " + resultList[i] + ", " + concList[i] + "\r\n";
            }

            CommData.csvString = result_str;

        }

        public static List<int> GetImgFrame(string chan, int i)
        {
            List<int> cdlist = new List<int>();

            if (diclist.Count == 0)
                return cdlist;

            if (!diclist.ContainsKey(chan))
                return cdlist;

            try
            {

                int n = GetCycleNum(chan);

                int cindex = -1;

                switch (chan)
                {
                    case "Chip#1":
                        cindex = 0;
                        break;
                    case "Chip#2":
                        cindex = 1;
                        break;
                    case "Chip#3":
                        cindex = 2;
                        break;
                    case "Chip#4":
                        cindex = 3;
                        break;
                    default:
                        break;
                }

                int skip = 0;

                //                for (int i = 0; i < n; i++)     // n is number of image blocks
                //                {
                ChartData cd = new ChartData();
                cd.x = i;

                int skip_boundary = i * imgFrame - skip;
                int take_count = imgFrame;

                if (skip_boundary + imgFrame >= diclist[chan].Count)
                {
                    take_count -= skip_boundary + imgFrame - diclist[chan].Count;

                    if (take_count < 0)
                    {
                        take_count = 0;

#if DEBUG
                            MessageBox.Show("File corruption detected, too many missing lines");
#endif

                        // continue;
                    }

                    // continue;       // forget about the last block
                }

                List<string> strlist = diclist[chan].Skip(skip_boundary).Take(imgFrame).ToList();

                Dictionary<int, List<string>> datalist = new Dictionary<int, List<string>>();
                char[] charSeparator = new char[] { ' ' };

                for (int k = 0; k < strlist.Count; k++)
                {
                    datalist[k] = strlist[k].Split(charSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                //int rn = Convert.ToInt32(datalist[11][12]);

                //int factori = Convert.ToInt32(datalist[11][11]);

                int rn, factori;

                if (datalist[11].Count < 14)
                {
                    rn = Convert.ToInt32(datalist[11][12]);
                    factori = Convert.ToInt32(datalist[11][11]);
                }
                else
                {
                    rn = Convert.ToInt32(datalist[11][13]);
                    factori = Convert.ToInt32(datalist[11][12]);
                }

                //if (i > 0)
                //{
                //    if (factori < cur_factor[cindex])
                //        gap_loc[cindex].Add(i + 1);
                //}
                //else
                //{
                //    gap_loc[cindex].Clear();
                //    cur_factor[cindex] = 15000;
                //}

                //cur_factor[cindex] = factori;

                if (rn != 11 && rn < 12)
                {
#if DEBUG
                        MessageBox.Show("File corruption detected, last row rn != 11");
#endif
                    skip += rn + 1;
                }
                else if (rn > 12)
                {
#if DEBUG
                        MessageBox.Show("File corruption detected, rn corrupted");
#endif
                    //cd.y = 0;
                    //cdlist.Add(cd);
                    // continue;
                }

                // Use flash version of data position index
                //                    int value = 0;

                //                    if (ksindex == 16)  // dark
                //                    {
                //                        int npoint = 11;
                //                        bool dark = true;
                //                        value = 0;
                //                        for (int j = 1; j < npoint; j++)
                //                        {
                //                            value += Convert.ToInt32(datalist[j][11]) - 100;    // last column
                //                            if (dark_map[cindex, j, 11] > 0)
                //                                dark = false;
                //                        }

                //                        if (!dark)
                //                        {
                //                            npoint = 11;
                //                            dark = true;
                //                            value = 0;
                //                            for (int j = 1; j < npoint; j++)
                //                            {
                //                                value += Convert.ToInt32(datalist[j][0]) - 100;  // first column
                //                                if (dark_map[cindex, j, 0] > 0)
                //                                    dark = false;
                //                            }
                //                            Debug.Assert(dark);
                //                        }

                //                        value = value * 4 / (npoint - 1);   // normalize to 4 pixels

                //                    }
                //                    else if (flash_loaded || dpinfo_loaded)
                //                    {
                //                        int npoint = row_index[cindex, ksindex].Count;

                //                        for (int j = 0; j < npoint; j++)
                //                        {
                //                            int row = row_index[cindex, ksindex].ElementAt(j);
                //                            int col = col_index[cindex, ksindex].ElementAt(j);
                //                            value += Convert.ToInt32(datalist[row][col]) - 100;
                //                        }
                //                    }
                //                    // Use Data position file version
                //                    else
                //                    {
                //                        if (positionlist.Count == 0)
                //                        {
                //                            MessageBox.Show("Position list is empty, this means flash not loaded and trim not loaded");
                //                            return cdlist;
                //                        }

                //                        if (!positionlist.ContainsKey(chan))
                //                        {
                //#if DEBUG
                //                            MessageBox.Show("Position list does not contain the channel key");
                //#endif
                //                            return cdlist;
                //                        }

                //                        string ss = positionlist[chan][ksindex];


                //                        string[] newstrs = ss.Split('+');
                //                        foreach (var item in newstrs)
                //                        {
                //                            string[] nstrs = item.Split('-');

                //                            if (nstrs.Length > 1)
                //                            {
                //                                int j = Convert.ToInt32(nstrs[0]);
                //                                int k = Convert.ToInt32(nstrs[1]);
                //                                int v = Convert.ToInt32(datalist[k][j]) - 100;
                //                                value += v;
                //                            }
                //                        }
                //                    }

                int value = 0, row, col;

                for (row = 0; row < CommData.imgFrame; row++)
                {
                    for (col = 0; col < CommData.imgFrame; col++)
                    {
                        value = Convert.ToInt32(datalist[row][col]);
                        cdlist.Add(value);
                    }
                }


                //                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "Get ImgFrame CommData");
#endif
            }

            return cdlist;
        }

        public static List<int> GetDP(string chan)
        {
            List<int> cdlist = new List<int>();

            if (diclist.Count == 0)
                return cdlist;

            if (!diclist.ContainsKey(chan))
                return cdlist;

            try
            {

                // Use flash version of data position index    

                int cindex = -1;

                switch (chan)
                {
                    case "Chip#1":
                        cindex = 0;
                        break;
                    case "Chip#2":
                        cindex = 1;
                        break;
                    case "Chip#3":
                        cindex = 2;
                        break;
                    case "Chip#4":
                        cindex = 3;
                        break;
                    default:
                        break;
                }

                for (int ksindex = 0; ksindex < KsIndex; ksindex++)

                    if (flash_loaded || dpinfo_loaded)
                    {
                        int npoint = row_index[cindex, ksindex].Count;

                        //int row, col;

                        for (int j = 0; j < npoint; j++)
                        {
                            //int row = row_index[cindex, ksindex].ElementAt(j);
                            //int col = col_index[cindex, ksindex].ElementAt(j);
                            cdlist.Add(row_index[cindex, ksindex].ElementAt(j));
                            cdlist.Add(col_index[cindex, ksindex].ElementAt(j));
                        }
                    }
                    // Use Data position file version
                    else
                    {
                        if (positionlist.Count == 0)
                        {
                            MessageBox.Show("Position list is empty, this means flash not loaded and trim not loaded");
                            return cdlist;
                        }

                        if (!positionlist.ContainsKey(chan))
                        {
#if DEBUG
                        MessageBox.Show("Position list does not contain the channel key");
#endif
                            return cdlist;
                        }

                        string ss = positionlist[chan][ksindex];

                        string[] newstrs = ss.Split('+');
                        foreach (var item in newstrs)
                        {
                            string[] nstrs = item.Split('-');

                            if (nstrs.Length > 1)
                            {
                                int j = Convert.ToInt32(nstrs[0]);
                                int k = Convert.ToInt32(nstrs[1]);

                                cdlist.Add(j);
                                cdlist.Add(k);
                            }
                        }
                    }

                //                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "Get ImgFrame CommData");
#endif
            }

            return cdlist;
        }

        public static List<string> GetStringList(string chan, int i)
        {
            List<string> stringlist = new List<string>();

            if (diclist.Count == 0)
                return stringlist;

            if (!diclist.ContainsKey(chan))
                return stringlist;

            try
            {

                int skip = 0;

                //                for (int i = 0; i < n; i++)     // n is number of image blocks
                //                {
                //ChartData cd = new ChartData();
                //cd.x = i;

                int skip_boundary = i * imgFrame - skip;
                int take_count = imgFrame;

                if (skip_boundary + imgFrame >= diclist[chan].Count)
                {
                    take_count -= skip_boundary + imgFrame - diclist[chan].Count;

                    if (take_count < 0)
                    {
                        take_count = 0;

#if DEBUG
                        MessageBox.Show("File corruption detected, too many missing lines");
#endif

                        // continue;
                    }

                    // continue;       // forget about the last block
                }

                stringlist = diclist[chan].Skip(skip_boundary).Take(imgFrame).ToList();



                //                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + "Get ImgFrame CommData");
#endif
            }

            return stringlist;
        }
    }
}
