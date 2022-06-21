#define ENGLISH_VER

#define DARK_LEVEL
#define DARK_MANAGE
#define SAW_TOOTH2						// Newer Sawtooth algorithm. USe 2 pass low byte correction
#define NON_CONTIGUOUS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Anitoa
{
    public class TrimReader
    {
        //        int TRIM_IMAGER_SIZE = 12;

        double[,] kb = new double[12, 6];
        double[,] fpn = new double[2, 12];
        double[] tempcal = new double[12];
        int rampgen = 0;
        int[] auto_v20 = new int[2];
        int auto_v15 = 0;
        string version = "";

        int[,] kbi = new int[12, 6];
        int[,] fpni = new int[2, 12];

        public void LoadData()
        {
            //for (int i = 0; i < TRIM_IMAGER_SIZE; i++)
            //{
            //    kb[i][0] = 1;
            //    kb[i][1] = 0;
            //    kb[i][2] = 0;
            //    kb[i][3] = 0;

            //    fpn[0][i] = 0;
            //    fpn[1][i] = 0;
            //    if (i != 0) tempcal[i] = 1;
            //    else tempcal[i] = 0;
            //}

            //rampgen = 0x88;

            //auto_v20[0] = 0x8;
            //auto_v20[1] = 0x8;

            //auto_v15 = 0x8;
        }

        public void ReadTrimFile()
        {
            //判断相应月份文件夹是否存在，没有则创建
            string path = AppDomain.CurrentDomain.BaseDirectory + "trim/trim.dat";

            if (!System.IO.File.Exists(path))
            {
#if DEBUG

#if ENGLISH_VER
                MessageBox.Show("trim file not found！");
#else
                MessageBox.Show("trim文件不存在！");
#endif

#endif
                return;
            }

            StreamReader sr = new StreamReader(path, Encoding.Default);
            var line = System.IO.File.ReadAllLines(path);
            string[] sstr = line.ToArray();
            List<string> ss = new List<string>();
            foreach (var item in sstr)
            {
                if (string.IsNullOrEmpty(item)) continue;
                ss.Add(item);
            }
            sr.Close();

#if DEBUG

#if ENGLISH_VER
            MessageBox.Show("trim file found and loaded！");
#else
            MessageBox.Show("trim文件找到并装入！");
#endif

#endif

            int index = 0;
            for (int i = 0; i < ss.Count; i++)
            {
                if (ss[i].Contains("Version"))
                {
                    version = ss[i + 1];

                }
                if (ss[i].Contains("Kb"))
                {
                    kb = new double[12, 6];
                    List<string> kbs = ss.ToList().Skip(i + 1).Take(12).ToList();

                    for (int n = 0; n < kbs.Count; n++)
                    {
                        if (string.IsNullOrEmpty(kbs[n])) continue;
                        string[] strs = kbs[n].Split(',');
                        for (int j = 0; j < strs.Length; j++)
                        {
                            if (string.IsNullOrEmpty(strs[j])) continue;
                            kb[n, j] = Convert.ToDouble(strs[j].Trim());
                        }
                    }
                }

                if (ss[i].Contains("Fpn_lg"))
                {
                    fpn = new double[2, 12];
                    List<string> Fpn_lgs = ss.ToList().Skip(i + 1).Take(1).ToList();

                    string[] strs = Fpn_lgs[0].Split(',');
                    for (int j = 0; j < strs.Length; j++)
                    {
                        if (string.IsNullOrEmpty(strs[j])) continue;
                        fpn[0, j] = Convert.ToDouble(strs[j].Trim());
                    }
                }
                if (ss[i].Contains("Fpn_hg"))
                {
                    List<string> Fpn_hgs = ss.ToList().Skip(i + 1).Take(1).ToList();
                    string[] strs = Fpn_hgs[0].Split(',');
                    for (int j = 0; j < strs.Length; j++)
                    {
                        if (string.IsNullOrEmpty(strs[j])) continue;
                        fpn[1, j] = Convert.ToDouble(strs[j].Trim());
                    }
                }
                if (ss[i].Contains("AutoV20_lg"))
                {
                    auto_v20 = new int[2];
                    List<string> AutoV20_lgs = ss.ToList().Skip(i + 1).Take(1).ToList();
                    string mm = AutoV20_lgs[0].Trim();
                    uint a = Convert.ToUInt32(mm, 16);
                    auto_v20[0] = Convert.ToInt32(a);
                }

                if (ss[i].Contains("AutoV20_hg"))
                {
                    List<string> AutoV20_hgs = ss.ToList().Skip(i + 1).Take(1).ToList();
                    string mm = AutoV20_hgs[0].Trim();
                    uint a = Convert.ToUInt32(mm, 16);
                    auto_v20[1] = Convert.ToInt32(a);
                }

                if (ss[i].Contains("AutoV15"))
                {
                    List<string> AutoV15s = ss.ToList().Skip(i + 1).Take(1).ToList();
                    string mm = AutoV15s[0].Trim();
                    uint a = Convert.ToUInt32(mm, 16);
                    auto_v15 = Convert.ToInt32(a);
                }

                if (ss[i].Contains("Rampgen"))
                {
                    List<string> Rampgens = ss.ToList().Skip(i + 1).Take(1).ToList();
                    string mm = Rampgens[0].Trim();
                    uint a = Convert.ToUInt32(mm, 16);
                    rampgen = Convert.ToInt32(a);
                }

                if (ss[i].Contains("Temp_calib"))
                {
                    tempcal = new double[12];
                    List<string> Temp_calibs = ss.ToList().Skip(i + 1).Take(1).ToList();
                    string[] strs = Temp_calibs[0].Split(',');
                    for (int j = 0; j < strs.Length; j++)
                    {
                        if (string.IsNullOrEmpty(strs[j])) continue;
                        tempcal[j] = Convert.ToDouble(strs[j].Trim());
                    }

                    switch (index)
                    {
                        case 0:
                            CommData.chan1_kb = kb;
                            CommData.chan1_fpn = fpn;
                            CommData.chan1_tempcal = tempcal;
                            CommData.chan1_rampgen = rampgen;
                            CommData.chan1_auto_v20 = auto_v20;
                            CommData.chan1_auto_v15 = auto_v15;
                            break;
                        case 1:
                            CommData.chan2_kb = kb;
                            CommData.chan2_fpn = fpn;
                            CommData.chan2_tempcal = tempcal;
                            CommData.chan2_rampgen = rampgen;
                            CommData.chan2_auto_v20 = auto_v20;
                            CommData.chan2_auto_v15 = auto_v15;
                            break;
                        case 2:
                            CommData.chan3_kb = kb;
                            CommData.chan3_fpn = fpn;
                            CommData.chan3_tempcal = tempcal;
                            CommData.chan3_rampgen = rampgen;
                            CommData.chan3_auto_v20 = auto_v20;
                            CommData.chan3_auto_v15 = auto_v15;
                            break;
                        case 3:
                            CommData.chan4_kb = kb;
                            CommData.chan4_fpn = fpn;
                            CommData.chan4_tempcal = tempcal;
                            CommData.chan4_rampgen = rampgen;
                            CommData.chan4_auto_v20 = auto_v20;
                            CommData.chan4_auto_v15 = auto_v15;
                            break;
                    }
                    index++;
                }
            }
        }

        // NumData =  "Column Number"
        // pixelNum = "Frame Size", 12 or 24

        public int TocalADCCorrection(int NumData, Byte HighByte, Byte LowByte, int pixelNum, int PCRNum, int gain_mode, int flag)
        {
            if(CommData.flash_loaded || CommData.dpinfo_loaded)
            {
                return ADCCorrectioni(NumData, HighByte, LowByte, pixelNum, PCRNum, gain_mode, ref flag);
            }
            if (version == "0x2")
            {
                return ADCCorrection(NumData, HighByte, LowByte, pixelNum, PCRNum, gain_mode, flag);
            }
            else
            {
                return ADCCorrectionOld(NumData, HighByte, LowByte, pixelNum, PCRNum, gain_mode, flag);
            }
        }

        public int ADCCorrection(int NumData, Byte HighByte, Byte LowByte, int pixelNum, int PCRNum, int gain_mode, int flag)
        {

            SetTrim(PCRNum);

            int hb, lb, lbc, hbi;
            int hbln, lbp, hbhn;
            bool oflow = false, uflow = false; //  qerr_big=false;

            //	CString strbuf;
            double ioffset = 0;
            int result;

            hb = (int)HighByte;
            hbln = hb % 16;		//
            hbhn = hb / 16;		//

            int nd = 0;
            if (pixelNum == 12) nd = NumData;
            else nd = NumData >> 1;

            double k, b, c, h;

            c = kb[nd, 4];
            h = kb[nd, 5];
            double shrink = c * 0.0033;

            if (hb < 16)
            {
                k = kb[nd, 0];
                b = kb[nd, 1] + h * 0.5;			// 15 is just an empirical value, the first bump is raised higher. To do: what about reverse bump
                c = c + 0.1 * h;
            }
            else if (hb < 128)
            {
                k = kb[nd, 0];
                b = kb[nd, 1];					// 
            }
            else
            {
                k = kb[nd, 2];
                b = kb[nd, 3];					// 
            }

            ioffset = k * (double)hb + b;

            lb = (int)LowByte;
            lbc = lb + (int)ioffset;

            if (hb > 128)
            {
                hbi = 128 + (hb - 128) / 2;
            }
            else
            {
                hbi = hb;
            }

            // Use lbc, not hbln to calculate sawtooth correction, as hbln tends to be a little jittery	
            ioffset += ((double)lbc - 128) * (c - shrink * (double)hbi) / 12;		// 12/19/2016 modification, shrinking sawtooth.
            lbc = lb + (int)ioffset;					// re-calc lbc, 2 pass algorithm

            if (lbc > 255) lbc = 255;
            else if (lbc < 0) lbc = 0;

            lbp = hbln * 16 + 7;

            int lbpc = lbp - (int)ioffset;				// lpb - ioffset: low byte predicted from the high byte low nibble BEFORE correction
            int qerr = lbp - lbc;					// if the lbc is correct, this would be the quantization error. If it is too large, maybe lb was the saturated "stuck" version

            if (lbpc > 255 + 20)
            {					// We allow some correction error, because hbln may have randomly flipped.
                oflow = true; flag = 1;
            }
            else if (lbpc > 255 && qerr > 28)
            {		// Again we allow some tolerance because hbln may have drifted, leading to fake error
                oflow = true; flag = 2;
            }
            else if (lbpc > 191 && qerr > 52)
            {
                oflow = true; flag = 3;
            }
            else if (qerr > 96)
            {
                oflow = true; flag = 4;
            }
            else if (lbpc < -20)
            {
                uflow = true; flag = 5;
            }
            else if (lbpc < 0 && qerr < -28)
            {
                uflow = true; flag = 6;
            }
            else if (lbpc < 64 && qerr < -52)
            {
                uflow = true; flag = 7;
            }
            else if (qerr < -96)
            {
                uflow = true; flag = 8;
            }
            else
            {
                flag = 0;
            }

            if (oflow || uflow)
            {
                result = hb * 16 + 7;
            }
            else
            {
                result = hbhn * 256 + lbc;
            }

            //if (calib2) return result;

#if DARK_MANAGE

            if (gain_mode == 0)
                result += -(int)(fpn[1, nd]) + 100;		// high gain
            else
                result += -(int)(fpn[0, nd]) + 100;		// low gain

            if (result < 0) result = 0;

#endif

            return result;
        }

        public int ADCCorrectionOld(int NumData, Byte HighByte, Byte LowByte, int pixelNum, int PCRNum, int gain_mode, int flag)
        {

            SetTrim(PCRNum);

            int hb, lb, lbc;
            int hbln, lbp, hbhn;
            bool oflow = false, uflow = false; //  qerr_big=false;

            string strbuf;
            double ioffset = 0;
            int result;

            hb = (int)HighByte;

            int nd = 0;
            if (pixelNum == 12) nd = NumData;
            else nd = NumData >> 1;

            ioffset = kb[nd, 0] * (double)hb + kb[nd, 1];

#if NON_CONTIGUOUS

            if (hb >= 128)
            {
                ioffset += kb[nd, 3];
            }

#endif

            hbln = hb % 16;		//

            hbhn = hb / 16;		//

#if SAW_TOOTH		

	ioffset += (int)(Node[PCRNum - 1].kb[nd][2] * (hbln - 7));

#endif

            //		ioffset = (int)(Node[PCRNum-1].kb[nd][0]*hb + Node[PCRNum-1].kb[nd][1] + Node[PCRNum-1].kb[nd][2] *(hbln - 7));

            lb = (int)LowByte;

            lbc = lb + (int)ioffset;

#if SAW_TOOTH2							// Use lbc, not hbln to calculate sawtooth correction, as hbln tends to be a little unstable
            ioffset += kb[nd, 2] * ((double)lbc - 127) * (1 - (double)hb / 400) / 16;		// 12/19/2016 modification, shrinking sawtooth.
            lbc = lb + (int)ioffset;					// re-calc lbc, 2 pass algorithm
#endif

            lbp = hbln * 16 + 7;

            if (lbc > 255) lbc = 255;
            else if (lbc < 0) lbc = 0;

            int lbpc = lbp - (int)ioffset;				// lpb - ioffset: low byte predicted from the high byte low nibble BEFORE correction
            int qerr = lbp - lbc;					// if the lbc is correct, this would be the quantization error. If it is too large, maybe lb was the saturated "stuck" version

            if (lbpc > 255 + 20)
            {					// We allow some correction error, because hbln may have randomly flipped.
                oflow = true; flag = 1;
            }
            else if (lbpc > 255 && qerr > 28)
            {		// Again we allow some tolerance because hbln may have drifted, leading to fake error
                oflow = true; flag = 2;
            }
            else if (lbpc > 191 && qerr > 52)
            {
                oflow = true; flag = 3;
            }
            else if (qerr > 96)
            {
                oflow = true; flag = 4;
            }
            else if (lbpc < -20)
            {
                uflow = true; flag = 5;
            }
            else if (lbpc < 0 && qerr < -28)
            {
                uflow = true; flag = 6;
            }
            else if (lbpc < 64 && qerr < -52)
            {
                uflow = true; flag = 7;
            }
            else if (qerr < -96)
            {
                uflow = true; flag = 8;
            }
            else
            {
                flag = 0;
            }

            //	if(abs(qerr) > 84) qerr_big = true;

            if (oflow || uflow)
            {
                result = hb * 16 + 7;
            }
            else
            {
                result = hbhn * 256 + lbc;
            }

            //if (calib2) return result;

#if DARK_MANAGE

            if (gain_mode == 0)
                result += -(int)(fpn[1, nd]) + 100;		// high gain
            else
                result += -(int)(fpn[0, nd]) + 100;		// low gain

            if (result < 0) result = 0;

#endif

            return result;
        }

        const int DARK_LEVEL = 100;

        public int ADCCorrectioni(int NumData, byte HighByte, byte LowByte, int pixelNum, int PCRNum, int gain_mode, ref int flag)
        {
            SetTrim2(PCRNum);

            int hb, lb, lbc, hbi;
            int hbln, lbp, hbhn;
            bool oflow = false, uflow = false;

            int ioffset = 0;
            int result;

            const int intmax = 32767;
            const int intmax256 = 128;

            hb = (int)HighByte;
            hbln = hb % 16;     //
            hbhn = hb / 16;     //

            int nd = 0;
            if (pixelNum == 12) nd = NumData;
            else nd = NumData >> 1;

            int k, b, c, h;

            //	double shrink = 0.022;

            c = (int)(kbi[nd, 4]);
            h = (int)(kbi[nd, 5]);

            if (hb < 16)
            {
                k = (int)(kbi[nd, 0]);
                b = (int)(kbi[nd, 1]) + h / 2;         // 15 is just an empirical value, the first bump is raised higher. To do: what about reverse bump
                c = c + h / 10;
            }
            else if (hb < 128)
            {
                k = (int)(kbi[nd, 0]);
                b = (int)(kbi[nd, 1]);                 // 
            }
            else
            {
                k = (int)(kbi[nd, 2]);
                b = (int)(kbi[nd, 3]);                 // 
            }

            ioffset = k * hb / intmax + b / intmax256;

            lb = (int)LowByte;
            lbc = lb + ioffset;

            if (hb > 128)
            {
                hbi = 128 + (hb - 128) / 2;
            }
            else
            {
                hbi = hb;
            }

            // Use lbc, not hbln to calculate sawtooth correction, as hbln tends to be a little jittery	
            ioffset += (lbc - 128) * c * (300 - hbi) / (12 * 300 * intmax256);      // 12/19/2016 modification, shrinking sawtooth.
            lbc = lb + ioffset;                                             // re-calc lbc, 2 pass algorithm

            if (lbc > 255) lbc = 255;
            else if (lbc < 0) lbc = 0;

            lbp = hbln * 16 + 7;

            int lbpc = lbp - ioffset;               // lpb - ioffset: low byte predicted from the high byte low nibble BEFORE correction
            int qerr = lbp - lbc;                   // if the lbc is correct, this would be the quantization error. If it is too large, maybe lb was the saturated "stuck" version

            if (lbpc > 255 + 20)
            {                   // We allow some correction error, because hbln may have randomly flipped.
                oflow = true; flag = 1;
            }
            else if (lbpc > 255 && qerr > 28)
            {       // Again we allow some tolerance because hbln may have drifted, leading to fake error
                oflow = true; flag = 2;
            }
            else if (lbpc > 191 && qerr > 52)
            {
                oflow = true; flag = 3;
            }
            else if (qerr > 96)
            {
                oflow = true; flag = 4;
            }
            else if (lbpc < -20)
            {
                uflow = true; flag = 5;
            }
            else if (lbpc < 0 && qerr < -28)
            {
                uflow = true; flag = 6;
            }
            else if (lbpc < 64 && qerr < -52)
            {
                uflow = true; flag = 7;
            }
            else if (qerr < -96)
            {
                uflow = true; flag = 8;
            }
            else
            {
                flag = 0;
            }

            if (oflow || uflow)
            {
                result = hb * 16 + 7;
            }
            else
            {
                result = hbhn * 256 + lbc;
            }

# if DARK_MANAGE

            if (gain_mode == 0)
                result += -(int)(fpni[1, nd]) + DARK_LEVEL;        // high gain
            else
                result += -(int)(fpni[0, nd]) + DARK_LEVEL;        // low gain

            if (result < 0) result = 0;

#endif
            return result;
        }

        private void SetTrim(int PcrNum)
        {
            switch (PcrNum)
            {
                case 1:
                    kb = CommData.chan1_kb;
                    fpn = CommData.chan1_fpn;
                    tempcal = CommData.chan1_tempcal;
                    rampgen = CommData.chan1_rampgen;
                    auto_v20 = CommData.chan1_auto_v20;
                    auto_v15 = CommData.chan1_auto_v15;
                    break;
                case 2:
                    kb = CommData.chan2_kb;
                    fpn = CommData.chan2_fpn;
                    tempcal = CommData.chan2_tempcal;
                    rampgen = CommData.chan2_rampgen;
                    auto_v20 = CommData.chan2_auto_v20;
                    auto_v15 = CommData.chan2_auto_v15;
                    break;
                case 3:
                    kb = CommData.chan3_kb;
                    fpn = CommData.chan3_fpn;
                    tempcal = CommData.chan3_tempcal;
                    rampgen = CommData.chan3_rampgen;
                    auto_v20 = CommData.chan3_auto_v20;
                    auto_v15 = CommData.chan3_auto_v15;
                    break;
                case 4:
                    kb = CommData.chan4_kb;
                    fpn = CommData.chan4_fpn;
                    tempcal = CommData.chan4_tempcal;
                    rampgen = CommData.chan4_rampgen;
                    auto_v20 = CommData.chan4_auto_v20;
                    auto_v15 = CommData.chan4_auto_v15;
                    break;
            }
        }

        private void SetTrim2(int PcrNum)
        {
            int ci = PcrNum - 1;

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    kbi[i, j] = CommData.kbi[ci, i, j];
                }
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    fpni[i, j] = CommData.fpni[ci, i, j];
                }
            }

            for (int i = 0; i < 2; i++)
            {
                auto_v20[i] = CommData.auto_v20[ci, i];
            }

            rampgen = CommData.rampgen[ci];
            auto_v15 = CommData.auto_v15[ci];
        }
    }
}
