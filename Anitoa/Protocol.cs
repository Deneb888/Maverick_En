using Synoxo.USBHidDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Anitoa
{
    public class Protocol
    {

        public Protocol()
        {
            Init();
        }

        private void Init()
        {

        }

        /// <summary>
        /// 将给定的数组转换成符合格式要求的字符串
        /// </summary>
        /// <param name="data">给定的数组</param>
        /// <param name="startIndex">起始字节序号</param>
        /// <param name="length">转换长度</param>
        /// <param name="prefix">前缀，例如 0x </param>
        /// <param name="splitString">分隔符</param>
        /// <param name="fromBase">进制10或16,其它值一律按16进制处理</param>
        /// <returns>字符串</returns>
        public string BytesToString(byte[] data, int startIndex, int length, string prefix, string splitString, int fromBase)
        {
            string _return = "";
            if (data == null)
                return _return;
            for (int i = 0; i < length; i++)
            {
                if (startIndex + i < data.Length)
                {
                    switch (fromBase)
                    {
                        case 10:
                            _return += string.Format("{0}{1:d3}", prefix, data[i + startIndex]);
                            break;
                        default:
                            _return += string.Format("{0}{1:X2}", prefix, data[i + startIndex]);
                            break;
                    }
                    if (i < data.Length - 1)
                        _return += splitString;
                }
            }
            return _return;
        }

        /// <summary>
        /// 将给定的字符串按照给定的分隔符和进制转换成字节数组
        /// </summary>
        /// <param name="str">给定的字符串</param>
        /// <param name="splitString">分隔符</param>
        /// <param name="fromBase">给定的进制</param>
        /// <returns>转换后的字节数组</returns>
        public byte[] StringToBytes(string str, string[] splitString, int fromBase)
        {
            string[] strBytes = str.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            if (strBytes == null || strBytes.Length == 0)
                return null;
            byte[] _return = new byte[strBytes.Length];
            for (int i = 0; i < strBytes.Length; i++)
            {
                try
                {
                    _return[i] = Convert.ToByte(strBytes[i], fromBase);
                }
                catch (SystemException)
                {
                    MessageBox.Show("发现不可转换的字符串->" + strBytes[i]);
                }
            }
            return _return;
        }

        public byte[] SetMeltCurve(DeviceManagement myDevManager, string start, string end)
        {
            float rate = 1;

            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];
            byte[] TxData = new byte[64];

            var ftempture = (float)Convert.ToDouble(start);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];

            ftempture = (float)Convert.ToDouble(end);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[4] = hData[0];
            OperBuf[5] = hData[1];
            OperBuf[6] = hData[2];
            OperBuf[7] = hData[3];

            ftempture = (float)Convert.ToDouble(rate);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[8] = hData[0];
            OperBuf[9] = hData[1];
            OperBuf[10] = hData[2];
            OperBuf[11] = hData[3];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x0e;		//data length
            TxData[3] = 0x0b;		//data type
            TxData[4] = 0x01;		// start

            TxData[5] = OperBuf[0];
            TxData[6] = OperBuf[1];
            TxData[7] = OperBuf[2];
            TxData[8] = OperBuf[3];

            TxData[9] = OperBuf[4];
            TxData[10] = OperBuf[5];
            TxData[11] = OperBuf[6];
            TxData[12] = OperBuf[7];

            TxData[13] = OperBuf[8];
            TxData[14] = OperBuf[9];
            TxData[15] = OperBuf[10];
            TxData[16] = OperBuf[11];

            for (int i = 1; i < 17; i++)
                TxData[17] += TxData[i];
            if (TxData[17] == 0x17)
                TxData[17] = 0x18;

            TxData[18] = 0x17;
            TxData[19] = 0x17;

            string res1 = this.BytesToString(TxData, 0, TxData.Length, "0x", " ", 16);
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            return inputdatas;
        }

        public byte[] SetMeltCurve(DeviceManagement myDevManager, string start, string end, int startTime)
        {
            float rate = 1;

            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];
            byte[] TxData = new byte[64];

            var ftempture = (float)Convert.ToDouble(start);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];

            ftempture = (float)Convert.ToDouble(end);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[4] = hData[0];
            OperBuf[5] = hData[1];
            OperBuf[6] = hData[2];
            OperBuf[7] = hData[3];

            ftempture = (float)Convert.ToDouble(rate);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[8] = hData[0];
            OperBuf[9] = hData[1];
            OperBuf[10] = hData[2];
            OperBuf[11] = hData[3];

            byte[] stTime = BitConverter.GetBytes(startTime);
            OperBuf[12] = stTime[1];
            OperBuf[13] = stTime[0];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x13;		//data length
            TxData[3] = 0x0b;		//data type
            TxData[4] = 0x03;		// start + enable start time

            TxData[5] = OperBuf[0];
            TxData[6] = OperBuf[1];
            TxData[7] = OperBuf[2];
            TxData[8] = OperBuf[3];

            TxData[9] = OperBuf[4];
            TxData[10] = OperBuf[5];
            TxData[11] = OperBuf[6];
            TxData[12] = OperBuf[7];

            TxData[13] = OperBuf[8];
            TxData[14] = OperBuf[9];
            TxData[15] = OperBuf[10];
            TxData[16] = OperBuf[11];

            TxData[17] = OperBuf[12];
            TxData[18] = OperBuf[13];

            for (int i = 1; i < 19; i++)
                TxData[19] += TxData[i];
            if (TxData[19] == 0x17)
                TxData[19] = 0x18;

            TxData[20] = 0x17;
            TxData[21] = 0x17;

            string res1 = this.BytesToString(TxData, 0, TxData.Length, "0x", " ", 16);
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            return inputdatas;
        }

        /// <summary>
        /// peltier设定，循环参数    
        /// </summary>
        /// <returns></returns>
        public byte[] SetPeltier(DeviceManagement myDevManager, DebugModelData currDebugModelData)
        {
            byte[] inputdatas = new byte[64];           // Zhimin: 16 enough?
            byte[] OperBuf = new byte[18];
            byte[] TxData = new byte[64];

            //取Dennature编辑框中的数据
            string stempture = currDebugModelData.Denaturating.ToString();
            string stime = currDebugModelData.DenaturatingTime.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var dena_Time = Convert.ToInt32(stime);
            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];

            //取Anneal编辑框中的数据
            stempture = currDebugModelData.Annealing.ToString();
            stime = currDebugModelData.AnnealingTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];
            var Annealing_Time = Convert.ToInt32(stime);
            byte[] AnnealingTimes = BitConverter.GetBytes(Annealing_Time);
            //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[10] = AnnealingTimes[1];
            OperBuf[11] = AnnealingTimes[0];

            //取Inter Extension编辑框中的数据
            stempture = currDebugModelData.Extension.ToString();
            stime = currDebugModelData.ExtensionTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[12] = hData[0];
            OperBuf[13] = hData[1];
            OperBuf[14] = hData[2];
            OperBuf[15] = hData[3];
            var Extension_Time = Convert.ToInt32(stime);
            byte[] ExtensionTimes = BitConverter.GetBytes(Extension_Time);
            OperBuf[16] = ExtensionTimes[1];
            OperBuf[17] = ExtensionTimes[0];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x16;		//data length
            TxData[3] = 0x03;		//data type, date edit first byte TXC
            TxData[4] = 0x03;		//RegBuf[18];						
            TxData[5] = 0x01;        //RegBuf[20];	
            //设置拍照阶段
            if (currDebugModelData.stageIndex == 1)
            {
                TxData[6] = 0x13;       // RegBuf[21];
            }
            if (currDebugModelData.stageIndex == 2)
            {
                TxData[6] = 0x23;       // RegBuf[21];
            }
            if (currDebugModelData.stageIndex == 3)
            {
                TxData[6] = 0x03;       // RegBuf[21];
            }

            TxData[7] = OperBuf[0];	//dennature数据
            TxData[8] = OperBuf[1];
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];//Anneal数据
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];
            TxData[19] = OperBuf[12];//Inter extension数据
            TxData[20] = OperBuf[13];
            TxData[21] = OperBuf[14];
            TxData[22] = OperBuf[15];
            TxData[23] = OperBuf[16];
            TxData[24] = OperBuf[17];
            for (int i = 1; i < 25; i++)
            {
                TxData[25] += TxData[i];
            }

            if (TxData[25] == 0x17)
                TxData[25] = 0x18;
            else
                TxData[25] = TxData[25];
            TxData[26] = 0x17;
            TxData[27] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }


        /// <summary>
        /// 一个温度参数设置
        /// </summary>
        /// <returns></returns>
        public byte[] SetPCRCyclTempTime1Seg(DeviceManagement myDevManager, DebugModelData currDebugModelData)
        {
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];
            byte[] TxData = new byte[64];

            //取Dennature编辑框中的数据
            string stempture = currDebugModelData.Denaturating.ToString();
            string stime = currDebugModelData.DenaturatingTime.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];

            var dena_Time = Convert.ToInt32(stime);

            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];

            TxData[0] = 0xaa;			//preamble code
            TxData[1] = 0x13;			//command  TXC
            TxData[2] = 0x10;			//data length
            TxData[3] = 0x03;			//data type, date edit first byte TXC
            TxData[4] = 0x03;			//					
            TxData[5] = 0x01;			//	
            TxData[6] = 0x02;			// 
            TxData[7] = OperBuf[0];			//dennature
            TxData[8] = OperBuf[1];
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            for (int i = 1; i < 13; i++)
                TxData[13] += TxData[i];
            if (TxData[13] == 0x17)
                TxData[13] = 0x18;
            //	else
            //		TxData[19] = TxData[19];
            TxData[14] = 0x17;
            TxData[15] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            return inputdatas;
        }

        /// <summary>
        /// 两个温度参数设置
        /// </summary>
        /// <returns></returns>
        public byte[] SetPCRCyclTempTime2Seg(DeviceManagement myDevManager, DebugModelData currDebugModelData)
        {
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];
            byte[] TxData = new byte[64];

            //取Dennature编辑框中的数据
            string stempture = currDebugModelData.Denaturating.ToString();
            string stime = currDebugModelData.DenaturatingTime.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var dena_Time = Convert.ToInt32(stime);
            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];

            //取Anneal编辑框中的数据
            stempture = currDebugModelData.Annealing.ToString();
            stime = currDebugModelData.AnnealingTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];

            var Annealing_Time = Convert.ToInt32(stime);
            byte[] AnnealingTimes = BitConverter.GetBytes(Annealing_Time);
            //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[10] = AnnealingTimes[1];
            OperBuf[11] = AnnealingTimes[0];

            TxData[0] = 0xaa;			//preamble code
            TxData[1] = 0x13;			//command  TXC
            TxData[2] = 0x10;			//data length
            TxData[3] = 0x03;			//data type, date edit first byte TXC
            TxData[4] = 0x03;			//					
            TxData[5] = 0x01;			//	
            TxData[6] = 0x02;			// 
            TxData[7] = OperBuf[0];			//dennature
            TxData[8] = OperBuf[1];
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];		//Anneal
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];

            for (int i = 1; i < 19; i++)
                TxData[19] += TxData[i];
            if (TxData[19] == 0x17)
                TxData[19] = 0x18;
            //	else
            //		TxData[19] = TxData[19];
            TxData[20] = 0x17;
            TxData[21] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            return inputdatas;
        }

        /// <summary>
        /// 四个温度参数设置
        /// </summary>
        /// <returns></returns>
        public byte[] SetPCRCyclTempTime4Seg(DeviceManagement myDevManager, DebugModelData currDebugModelData)
        {
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[32];

            //取Dennature编辑框中的数据
            string stempture = currDebugModelData.Denaturating.ToString();
            string stime = currDebugModelData.DenaturatingTime.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var dena_Time = Convert.ToInt32(stime);
            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];

            //取Anneal编辑框中的数据
            stempture = currDebugModelData.Annealing.ToString();
            stime = currDebugModelData.AnnealingTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];
            var Annealing_Time = Convert.ToInt32(stime);
            byte[] AnnealingTimes = BitConverter.GetBytes(Annealing_Time);
            //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[10] = AnnealingTimes[1];
            OperBuf[11] = AnnealingTimes[0];

            //取Inter Extension编辑框中的数据
            stempture = currDebugModelData.Extension.ToString();
            stime = currDebugModelData.ExtensionTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[12] = hData[0];
            OperBuf[13] = hData[1];
            OperBuf[14] = hData[2];
            OperBuf[15] = hData[3];
            var Extension_Time = Convert.ToInt32(stime);
            byte[] ExtensionTimes = BitConverter.GetBytes(Extension_Time);
            OperBuf[16] = ExtensionTimes[1];
            OperBuf[17] = ExtensionTimes[0];

            //取Step4编辑框中的数据
            stempture = currDebugModelData.Step4.ToString();
            stime = currDebugModelData.Step4Time.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[18] = hData[0];
            OperBuf[19] = hData[1];
            OperBuf[20] = hData[2];
            OperBuf[21] = hData[3];
            var Step4_Time = Convert.ToInt32(stime);
            byte[] Step4Times = BitConverter.GetBytes(Step4_Time);
            OperBuf[22] = Step4Times[1];
            OperBuf[23] = Step4Times[0];

            byte[] TxData = new byte[64];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x1c;		//data length
            TxData[3] = 0x03;		//data type, date edit first byte TXC
            TxData[4] = 0x03;		//RegBuf[18];						
            TxData[5] = 0x01;        //RegBuf[20];	
            //设置拍照阶段
            if (currDebugModelData.stageIndex == 1)
            {
                TxData[6] = 0x13;       // RegBuf[21];
            }
            if (currDebugModelData.stageIndex == 2)
            {
                TxData[6] = 0x23;       // RegBuf[21];
            }
            if (currDebugModelData.stageIndex == 3)
            {
                TxData[6] = 0x03;       // RegBuf[21];
            }
            if (currDebugModelData.stageIndex == 4)
            {
                TxData[6] = 0x04;       // RegBuf[21];
            }

            TxData[7] = OperBuf[0];	//dennature数据
            TxData[8] = OperBuf[1];
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];//Anneal数据
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];
            TxData[19] = OperBuf[12];//Inter extension数据
            TxData[20] = OperBuf[13];
            TxData[21] = OperBuf[14];
            TxData[22] = OperBuf[15];
            TxData[23] = OperBuf[16];
            TxData[24] = OperBuf[17];
            TxData[25] = OperBuf[18];//Step4Time数据
            TxData[26] = OperBuf[19];
            TxData[27] = OperBuf[20];
            TxData[28] = OperBuf[21];
            TxData[29] = OperBuf[22];
            TxData[30] = OperBuf[23];
            for (int i = 1; i < 31; i++)
            {
                TxData[31] += TxData[i];
            }

            if (TxData[31] == 0x17)
                TxData[31] = 0x18;
            else
                TxData[31] = TxData[31];
            TxData[32] = 0x17;
            TxData[33] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        /// <summary>
        /// 设循环参数及启动循环
        /// </summary>
        /// <returns></returns>
        public byte[] SetCycleParameters(DeviceManagement myDevManager, DebugModelData currDebugModelData, int currCycleindex)
        {
            //CommData.Cycle = Convert.ToInt32(currDebugModelData.Cycle);
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];

            //取Initail denaturation编辑框中的数据
            string stempture = currDebugModelData.Initaldenaturation.ToString();
            string stime = "";
            if (currCycleindex > 0)
            {
                stempture = currDebugModelData.Denaturating.ToString();
                //DebugModelData newDebugModelData = dmdlist[currCycleindex - 1];
                //if (newDebugModelData.StepCount == 1)
                //{
                //    stempture = newDebugModelData.Denaturating.ToString();
                //}
                //if (newDebugModelData.StepCount == 2)
                //{
                //    stempture = newDebugModelData.Annealing.ToString();
                //}
                //if (newDebugModelData.StepCount == 3)
                //{
                //    stempture = newDebugModelData.Extension.ToString();
                //}
                //if (newDebugModelData.StepCount == 4)
                //{
                //    stempture = newDebugModelData.Step4.ToString();
                //}
                stime = "1";
            }
            else
            {
                stime = currDebugModelData.InitaldenaTime.ToString();
            }

            //string stime = currDebugModelData.InitaldenaTime.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var dena_Time = Convert.ToInt32(stime);

            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];


            //取hold on 编辑框中的数据
            stempture = currDebugModelData.Holdon.ToString();
            stime = currDebugModelData.HoldonTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];

            var HoldonTime = Convert.ToInt32(stime);
            byte[] HoldonTimes = BitConverter.GetBytes(HoldonTime);
            //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[10] = HoldonTimes[1];
            OperBuf[11] = HoldonTimes[0];


            //cycle编辑框值
            var cycle = currDebugModelData.Cycle.ToString();
            //var itime = Convert.ToByte(cycle);	//将编辑框中整型字符串转成byte
            OperBuf[12] = Convert.ToByte(cycle);

            byte[] TxData = new byte[64];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x10;		//data length
            TxData[3] = 0x04;		//data type, date edit first byte TXC
            TxData[4] = 0x01;		//real data, start
            TxData[5] = OperBuf[12];	//cycle setting
            TxData[6] = 0x00;       //
            TxData[7] = OperBuf[0];	//inital dennature数据	
            TxData[8] = OperBuf[1];  //
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];	//extern extension数据
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];
            for (int i = 1; i < 19; i++)
                TxData[19] += TxData[i];
            if (TxData[19] == 0x17)
                TxData[19] = 0x18;
            else
                TxData[19] = TxData[19];
            TxData[20] = 0x17;
            TxData[21] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        /// <summary>
        /// 设循环参数及启动循环
        /// </summary>
        /// <returns></returns>
        public byte[] SetCycleParameters2(DeviceManagement myDevManager, DebugModelData currDebugModelData, int currCycleindex)
        {
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[20];

            //取Initail denaturation编辑框中的数据
            string stempture = currDebugModelData.Initaldenaturation.ToString();
            string stime = "";
            if (currCycleindex > 0)         // Zhimin: Why?
            {
                stempture = currDebugModelData.Denaturating.ToString();
                stime = "1";
            }
            else
            {
                stime = currDebugModelData.InitaldenaTime.ToString();
            }


            //string stime = currDebugModelData.InitaldenaTime.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var dena_Time = Convert.ToInt32(stime);

            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];

            //======================================

            stempture = currDebugModelData.Initaldenaturation2.ToString();
            stime = currDebugModelData.InitaldenaTime2.ToString();

            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];
            var dena_Time2 = Convert.ToInt32(stime);

            byte[] denaTimes2 = BitConverter.GetBytes(dena_Time2);
            OperBuf[10] = denaTimes2[1];
            OperBuf[11] = denaTimes2[0];

            //======================================

            //var itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            //OperBuf[4] = Convert.ToByte(itime >> 8);
            //OperBuf[5] = itime;

            //取hold on 编辑框中的数据
            stempture = currDebugModelData.Holdon.ToString();
            stime = currDebugModelData.HoldonTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[12] = hData[0];
            OperBuf[13] = hData[1];
            OperBuf[14] = hData[2];
            OperBuf[15] = hData[3];

            var HoldonTime = Convert.ToInt32(stime);
            byte[] HoldonTimes = BitConverter.GetBytes(HoldonTime);
            //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[16] = HoldonTimes[1];
            OperBuf[17] = HoldonTimes[0];


            //cycle编辑框值
            var cycle = currDebugModelData.Cycle.ToString();
            //var itime = Convert.ToByte(cycle);	//将编辑框中整型字符串转成byte
            OperBuf[18] = Convert.ToByte(cycle);

            byte[] TxData = new byte[64];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x16;		//data length
            TxData[3] = 0x04;		//data type, date edit first byte TXC
            TxData[4] = 0x01;		//real data, start
            TxData[5] = OperBuf[18];	//cycle setting
            TxData[6] = 0x01;           // new CFG file
            TxData[7] = OperBuf[0];	//inital dennature数据	
            TxData[8] = OperBuf[1];  //
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];	//inital dennature2数据
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];
            TxData[19] = OperBuf[12];	//extern extension数据
            TxData[20] = OperBuf[13];
            TxData[21] = OperBuf[14];
            TxData[22] = OperBuf[15];
            TxData[23] = OperBuf[16];
            TxData[24] = OperBuf[17];
            for (int i = 1; i < 25; i++)
                TxData[25] += TxData[i];
            if (TxData[25] == 0x17)
                TxData[25] = 0x18;

            TxData[26] = 0x17;
            TxData[27] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 24);

            return inputdatas;
        }

        /// <summary>
        /// 设循环参数及启动循环
        /// </summary>
        /// <returns></returns>
        public byte[] SetCycleParameters2(DeviceManagement myDevManager, DebugModelData currDebugModelData, int currCycleindex, byte cfg)
        {
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[20];

            //取Initail denaturation编辑框中的数据
            string stempture = currDebugModelData.Initaldenaturation.ToString();
            string stime = "";
            if (currCycleindex > 0)         // Zhimin: Why?
            {
                stempture = currDebugModelData.Denaturating.ToString();
                stime = "1";
            }
            else
            {
                stime = currDebugModelData.InitaldenaTime.ToString();
            }


            //string stime = currDebugModelData.InitaldenaTime.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var dena_Time = Convert.ToInt32(stime);

            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];

            //======================================

            stempture = currDebugModelData.Initaldenaturation2.ToString();
            stime = currDebugModelData.InitaldenaTime2.ToString();

            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];
            var dena_Time2 = Convert.ToInt32(stime);

            byte[] denaTimes2 = BitConverter.GetBytes(dena_Time2);
            OperBuf[10] = denaTimes2[1];
            OperBuf[11] = denaTimes2[0];

            //======================================

            //var itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            //OperBuf[4] = Convert.ToByte(itime >> 8);
            //OperBuf[5] = itime;

            //取hold on 编辑框中的数据
            stempture = currDebugModelData.Holdon.ToString();
            stime = currDebugModelData.HoldonTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[12] = hData[0];
            OperBuf[13] = hData[1];
            OperBuf[14] = hData[2];
            OperBuf[15] = hData[3];

            var HoldonTime = Convert.ToInt32(stime);
            byte[] HoldonTimes = BitConverter.GetBytes(HoldonTime);
            //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[16] = HoldonTimes[1];
            OperBuf[17] = HoldonTimes[0];


            //cycle编辑框值
            var cycle = currDebugModelData.Cycle.ToString();
            //var itime = Convert.ToByte(cycle);	//将编辑框中整型字符串转成byte
            OperBuf[18] = Convert.ToByte(cycle);

            byte[] TxData = new byte[64];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x16;		//data length
            TxData[3] = 0x04;		//data type, date edit first byte TXC
            TxData[4] = 0x01;		//real data, start
            TxData[5] = OperBuf[18];	//cycle setting
            TxData[6] = cfg;           // new CFG file
            TxData[7] = OperBuf[0];	//inital dennature数据	
            TxData[8] = OperBuf[1];  //
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];	//inital dennature2数据
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];
            TxData[19] = OperBuf[12];	//extern extension数据
            TxData[20] = OperBuf[13];
            TxData[21] = OperBuf[14];
            TxData[22] = OperBuf[15];
            TxData[23] = OperBuf[16];
            TxData[24] = OperBuf[17];
            for (int i = 1; i < 25; i++)
                TxData[25] += TxData[i];
            if (TxData[25] == 0x17)
                TxData[25] = 0x18;

            TxData[26] = 0x17;
            TxData[27] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 24);

            return inputdatas;
        }

        /// <summary>
        /// 设图像板MASK
        /// </summary>
        /// <returns></returns>
        public byte[] SetImgMask(DeviceManagement myDevManager, DebugModelData currDebugModelData)
        {

            byte[] inputdatas = new byte[64];
            int PCRMask = 0;
            int ck1 = CommData.cboChan1;
            int ck2 = CommData.cboChan2;
            int ck3 = CommData.cboChan3;
            int ck4 = CommData.cboChan4;
            PCRMask = (ck4 << 3) | (ck3 << 2) | (ck2 << 1) | ck1;

            if (currDebugModelData != null && currDebugModelData.ifpz == 1)
            {
                PCRMask = 0;
            }

            byte[] TxData = new byte[64];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  TXC
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x24;		//data type
            TxData[4] = (byte)PCRMask;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;


            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res1 = this.BytesToString(TxData, 0, TxData.Length, "0x", " ", 16);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }


        /// <summary>
        /// 热盖温度设置
        /// </summary>
        /// <returns></returns>
        public byte[] SetHotTemperature(DeviceManagement myDevManager, DebugModelData currDebugModelData)
        {

            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];

            //取hot lid 编辑框中的数据
            string stempture = currDebugModelData.Hotlid.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];

            byte[] TxData = new byte[64];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x10;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x01;		//data type, date edit first byte
            TxData[4] = 0x01;		//real data
            TxData[5] = OperBuf[0];	//tp第一字节				
            TxData[6] = OperBuf[1];
            TxData[7] = OperBuf[2];
            TxData[8] = OperBuf[3];	//tp最后一字节
            TxData[9] = 0x00;		//time低字节
            TxData[10] = 0x00;		//time高字节
            TxData[11] = 0x00;		//预留位
            TxData[12] = 0x00;
            TxData[13] = 0x00;
            TxData[14] = 0x00;
            for (int i = 1; i < 15; i++)
            {
                TxData[15] += TxData[i];
            }
            if (TxData[15] == 0x17)
                TxData[15] = 0x18;
            else
                TxData[15] = TxData[15];
            TxData[16] = 0x17;		//back code
            TxData[17] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        public byte[] SetHotTemperatureNew(DeviceManagement myDevManager, DebugModelData currDebugModelData, string hotTmp)
        {

            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];

            //取hot lid 编辑框中的数据
            string stempture = hotTmp;
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] TxData = new byte[64];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x10;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x01;		//data type, date edit first byte
            TxData[4] = 0x01;		//real data
            TxData[5] = OperBuf[0];	//tp第一字节				
            TxData[6] = OperBuf[1];
            TxData[7] = OperBuf[2];
            TxData[8] = OperBuf[3];	//tp最后一字节
            TxData[9] = 0x00;		//time低字节
            TxData[10] = 0x00;		//time高字节
            TxData[11] = 0x00;		//预留位
            TxData[12] = 0x00;
            TxData[13] = 0x00;
            TxData[14] = 0x00;
            for (int i = 1; i < 15; i++)
            {
                TxData[15] += TxData[i];
            }
            if (TxData[15] == 0x17)
                TxData[15] = 0x18;
            else
                TxData[15] = TxData[15];
            TxData[16] = 0x17;		//back code
            TxData[17] = 0x17;		//back code
            string res1 = this.BytesToString(TxData, 0, TxData.Length, "0x", " ", 16);
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public byte[] CloseDev(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];

            //取Initail denaturation编辑框中的数据
            string stempture = "95";
            string stime = "120";
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[4] = Convert.ToByte(itime >> 8);
            OperBuf[5] = itime;

            //取hold on 编辑框中的数据
            stempture = "50";
            stime = "10";
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];
            itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[10] = Convert.ToByte(itime >> 8);
            OperBuf[11] = itime;


            //cycle编辑框值
            stime = "50";
            itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[12] = itime;

            byte[] TxData = new byte[64];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x10;		//data length
            TxData[3] = 0x04;		//data type, date edit first byte TXC
            TxData[4] = 0x00;		//real data, start
            TxData[5] = OperBuf[12];	//cycle setting
            TxData[6] = 0x00;       //
            TxData[7] = OperBuf[0];	//inital dennature数据	
            TxData[8] = OperBuf[1];  //
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];	//extern extension数据
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];
            for (int i = 1; i < 19; i++)
                TxData[19] += TxData[i];
            if (TxData[19] == 0x17)
                TxData[19] = 0x18;
            else
                TxData[19] = TxData[19];
            TxData[20] = 0x17;
            TxData[21] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            //CommData.experimentModelData.endatetime = DateTime.Now;
            //bool flag = CommData.AddExperiment(CommData.experimentModelData);

            return inputdatas;
        }

        /// <summary>
        /// 查询温度板循环状态
        /// </summary>
        public byte[] ReadTempCycleState(DeviceManagement myDevManager)
        {

            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x14;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x15;		//data type, date edit first byte
            TxData[4] = 0x00;		//real data
            TxData[5] = 0x00;		//预留位

            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            if (inputdatas[5] == 0)//0当前没有跑循环
            {

            }

            return inputdatas;
        }

        /// <summary>
        /// 读取风扇状态
        /// </summary>
        public byte[] ReadFanState(DeviceManagement myDevManager)
        {

            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x10;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x0A;		//data type, date edit first byte
            TxData[4] = 0x01;		//real data
            TxData[5] = 0x00;		//预留位

            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            /*            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                        {
                            ucTiaoShiThree.txtFanState.Text = inputdatas[5].ToString();

                        });
                        //Thread.Sleep(100);
                        ISImgRead();
                        //ReadAllImg();
            */
            return inputdatas;
        }

        /// <summary>
        /// 停止风扇
        /// </summary>
        public byte[] StopFan(DeviceManagement myDevManager)
        {

            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x10;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x0A;		//data type, date edit first byte
            TxData[4] = 0x01;		//real data
            TxData[5] = 0x00;		//预留位

            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            if (inputdatas[5] == 0)//0当前没有跑循环
            {

            }


            return inputdatas;
        }

        /// <summary>
        /// 设置chan
        /// </summary>
        public byte[] SetChan(DeviceManagement myDevManager)
        {

            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x23;		//data type, date edit first byte
            TxData[4] = 0x80;		//real data
            TxData[5] = 0x00;		//预留位

            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        /// <summary>
        /// 读出当前循环数和阶段数
        /// </summary>
        public byte[] GetCycldNum(DeviceManagement myDevManager)
        {

            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x14;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x01;		//data type, date edit first byte
            TxData[4] = 0x00;		//real data
            TxData[5] = 0x00;		//预留位

            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            /*
                        currCycleNum = inputdatas[5];
                        CommData.currCycleNum = currCycleNum;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                        {
                            ucTiaoShiThree.txtCycleNum.Text = inputdatas[5].ToString();
                            ucRunOne.txtcurrC.Text = inputdatas[5].ToString();
                            ucTiaoShiOne.txtClycle.Text = inputdatas[5].ToString();
                        });
                        //Thread.Sleep(100);
                        ReadFanState();
            */

            return inputdatas;
        }


        /// <summary>
        /// 设置KP
        /// </summary>
        public void SetKP0Zone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//0Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x01;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void SetKP1Zone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//1Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x11;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        /// <summary>
        /// 设置KI
        /// </summary>
        public void SetKI0Zone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//0Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x02;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void SetKI1Zone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//1Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x12;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        /// <summary>
        /// 设置KD
        /// </summary>
        public void SetKD0Zone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//1Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x04;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }
        public void SetKD1Zone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//1Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x14;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        /// <summary>
        /// 设置KL
        /// </summary>
        public void SetKL0Zone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//1Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x08;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void SetKL1Zone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//1Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x18;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }
        /// <summary>
        /// 设置Zone
        /// </summary>
        public void SetZone(DeviceManagement myDevManager)
        {
            byte[] OperBuf = new byte[18];
            string stempture = "";//1Zone
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x11;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x09;		//data type
            TxData[4] = OperBuf[0];
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];

            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }


        public void OpeLedon(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x23;		//data type, led control
            TxData[4] = 0x81;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void OpeLenoff(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x23;		//data type, led control
            TxData[4] = 0x80;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }


        public void OpeLed2on(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x23;		//data type, led control
            TxData[4] = 0x82;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void OpeLed2off(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x23;		//data type, led control
            TxData[4] = 0x80;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void OpeLed3on(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x23;		//data type, led control
            TxData[4] = 0x84;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void OpeLed3off(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x23;		//data type, led control
            TxData[4] = 0x80;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void OpeLed4on(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x23;		//data type, led control
            TxData[4] = 0x88;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public void OpeLed4off(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x23;		//data type, led control
            TxData[4] = 0x80;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
        }

        public byte[] ReadPITemperature(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[16];
            byte[] TxData = new byte[18];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x10;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x02;		//data type, date edit first byte
            TxData[4] = 0x01;		//real data
            TxData[5] = 0x00;		//预留位
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            TxData[8] = 0x00;
            TxData[9] = 0x00;
            TxData[10] = 0x00;
            TxData[11] = 0x00;
            TxData[12] = 0x00;
            TxData[13] = 0x00;
            TxData[14] = 0x00;
            for (int i = 1; i < 15; i++)
            {
                TxData[15] += TxData[i];
            }
            if (TxData[15] == 0x17)
                TxData[15] = 0x18;
            else
                TxData[15] = TxData[15];

            TxData[16] = 0x17;		//back code
            TxData[17] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            return inputdatas;
        }

        public byte[] ReadPTTemperature(DeviceManagement myDevManager)
        {

            byte[] inputdatas = new byte[16];

            byte[] TxData = new byte[18];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x10;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x02;		//data type, date edit first byte
            TxData[4] = 0x02;		//real data
            TxData[5] = 0x00;		//预留位
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            TxData[8] = 0x00;
            TxData[9] = 0x00;
            TxData[10] = 0x00;
            TxData[11] = 0x00;
            TxData[12] = 0x00;
            TxData[13] = 0x00;
            TxData[14] = 0x00;
            for (int i = 1; i < 15; i++)
            {
                TxData[15] += TxData[i];
            }
            if (TxData[15] == 0x17)
                TxData[15] = 0x18;
            else
                TxData[15] = TxData[15];
            TxData[16] = 0x17;		//back code
            TxData[17] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        public byte[] ReadTemperatureAndStateBatchMode(DeviceManagement myDevManager)
        {

            byte[] inputdatas = new byte[64];

            byte[] TxData = new byte[18];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x14;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x0D;		//data type, date edit first byte
            TxData[4] = 0x00;		//real data
            TxData[5] = 0x00;		//预留位
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            TxData[8] = 0x00;
            TxData[9] = 0x00;
            TxData[10] = 0x00;
            TxData[11] = 0x00;
            TxData[12] = 0x00;
            TxData[13] = 0x00;
            TxData[14] = 0x00;
            for (int i = 1; i < 15; i++)
            {
                TxData[15] += TxData[i];
            }
            if (TxData[15] == 0x17)
                TxData[15] = 0x18;
            else
                TxData[15] = TxData[15];
            TxData[16] = 0x17;		//back code
            TxData[17] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }


        public byte[] SetGainMode(DeviceManagement myDevManager, int gain_mode)
        {
            byte[] inputdatas = new byte[16];
            byte[] TxData = new byte[18];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x02;		//data length
            TxData[3] = 0x07;		//data type, date edit first byte
            if (gain_mode == 1)//1低Gain 0高Gain
            {
                TxData[4] = 0x01;
            }
            else
            {
                TxData[4] = 0x00;
            }

            for (int i = 1; i < 5; i++)
            {
                TxData[5] += TxData[i];
            }
            if (TxData[5] == 0x17)
                TxData[5] = 0x18;
            else
                TxData[5] = TxData[5];
            TxData[6] = 0x17;		//back code
            TxData[7] = 0x17;		//back code

            bool flag = myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            return inputdatas;
        }

        public byte[] SelSensor(DeviceManagement myDevManager, int c)
        {
            byte[] inputdatas = new byte[16];
            byte[] TxData = new byte[18];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x03;		//data length
            TxData[3] = 0x26;		//data type
            TxData[4] = Convert.ToByte(c - 1);		//real data
            TxData[5] = 0x00;
            for (int i = 1; i < 6; i++)
            {
                TxData[6] += TxData[i];
            }
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            return inputdatas;
        }

        public byte[] SetIntergrationTime(DeviceManagement myDevManager, float InTime)
        {
            byte[] inputdatas = new byte[16];
            byte[] OperBuf = new byte[18];
            byte[] TxData = new byte[18];

            var ftempture = InTime;
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x20;		//data type, date edit first byte
            TxData[4] = OperBuf[0];		//real data, date edit second byte
            TxData[5] = OperBuf[1];
            TxData[6] = OperBuf[2];
            TxData[7] = OperBuf[3];
            //0x01 means send vedio data
            //0x00 means stop vedio data
            for (int i = 1; i < 8; i++)
            {
                TxData[8] += TxData[i];
            }
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;		//back code
            TxData[10] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            //txtt.Text = res;
            return inputdatas;
        }


        public void ResetParams(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[9];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x03;		//data length
            TxData[3] = 0x0F;		//data type, dat edit first byte
            TxData[4] = 0x00;		//real data, data edit second byte
            TxData[5] = 0x00;		//real data, data edit third byte
            TxData[6] = 0x13;		//check sum
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

        }

        public void SetRampgen(DeviceManagement myDevManager, int rampgen)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[8];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x02;		//data length
            TxData[3] = 0x01;		//data type, date edit first byte
            TxData[4] = (byte)rampgen;	//real data, date edit second byte
            //0x01 means send video data
            //0x00 means stop video data
            for (int i = 1; i < 5; i++)
                TxData[5] += TxData[i];
            if (TxData[5] == 0x17)
                TxData[5] = 0x18;
            else
                TxData[5] = TxData[5];
            TxData[6] = 0x17;		//back code
            TxData[7] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
        }

        public void SetTXbin(DeviceManagement myDevManager, byte txbin)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[8];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x02;		//data length
            TxData[3] = 0x08;		//data type, date edit first byte
            TxData[4] = txbin;	//real data, date edit second byte
            //0x01 means send vedio data
            //0x00 means stop vedio data
            for (int i = 1; i < 5; i++)
                TxData[5] += TxData[i];
            if (TxData[5] == 0x17)
                TxData[5] = 0x18;
            else
                TxData[5] = TxData[5];
            TxData[6] = 0x17;		//back code
            TxData[7] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
        }

        public void SetRange(DeviceManagement myDevManager, int range)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[8];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x02;		//data length
            TxData[3] = 0x02;		//data type, date edit first byte
            TxData[4] = (byte)range;	//real data, date edit second byte
            //0x01 means send vedio data
            //0x00 means stop vedio data
            for (int i = 1; i < 5; i++)
                TxData[5] += TxData[i];
            if (TxData[5] == 0x17)
                TxData[5] = 0x18;
            else
                TxData[5] = TxData[5];
            TxData[6] = 0x17;		//back code
            TxData[7] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
        }

        public void SetV15(DeviceManagement myDevManager, int v15)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[8];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x02;		//data length
            TxData[3] = 0x05;		//data type, date edit first byte
            TxData[4] = (byte)v15;	//real data, date edit second byte
            for (int i = 1; i < 5; i++)
                TxData[5] += TxData[i];
            if (TxData[5] == 0x17)
                TxData[5] = 0x18;
            else
                TxData[5] = TxData[5];
            TxData[6] = 0x17;		//back code
            TxData[7] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
        }

        public void SetV20(DeviceManagement myDevManager, int v20)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[8];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x02;		//data length
            TxData[3] = 0x04;		//data type, date edit first byte
            TxData[4] = (byte)v20;		//real data, date edit second byte
            //0x01 means send vedio data
            //0x00 means stop vedio data
            for (int i = 1; i < 5; i++)
                TxData[5] += TxData[i];
            if (TxData[5] == 0x17)
                TxData[5] = 0x18;
            else
                TxData[5] = TxData[5];
            TxData[6] = 0x17;		//back code
            TxData[7] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
        }

        public void SetLEDConfig(DeviceManagement myDevManager, int IndvEn, int Chan1, int Chan2, int Chan3, int Chan4)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[8];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x01;		//command
            TxData[2] = 0x02;		//data length
            TxData[3] = 0x23;		//data type, date edit first byte

            if (IndvEn == 0)
            {
                TxData[4] = Chan1 == 1 ? (byte)1 : (byte)0;									//real data, date edit second byte
            }
            else
            {
                TxData[4] = 0x80;
                if (Chan1 == 1)
                    TxData[4] |= 1;
                if (Chan2 == 1)
                    TxData[4] |= 2;
                if (Chan3 == 1)
                    TxData[4] |= 4;
                if (Chan4 == 1)
                    TxData[4] |= 8;
            }

            for (int i = 1; i < 5; i++)
                TxData[5] += TxData[i];

            if (TxData[5] == 0x17)
                TxData[5] = 0x18;
            else
                TxData[5] = TxData[5];

            TxData[6] = 0x17;		//back code
            TxData[7] = 0x17;		//back code
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
        }

        public byte[] ReadPITemperatureNew(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[16];
            byte[] TxData = new byte[18];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x10;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x02;		//data type, date edit first byte
            TxData[4] = 0x01;		//real data
            TxData[5] = 0x00;		//预留位
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            TxData[8] = 0x00;
            TxData[9] = 0x00;
            TxData[10] = 0x00;
            TxData[11] = 0x00;
            TxData[12] = 0x00;
            TxData[13] = 0x00;
            TxData[14] = 0x00;
            for (int i = 1; i < 15; i++)
            {
                TxData[15] += TxData[i];
            }
            if (TxData[15] == 0x17)
                TxData[15] = 0x18;
            else
                TxData[15] = TxData[15];
            TxData[16] = 0x17;		//back code
            TxData[17] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        public byte[] ReadPTTemperatureNew(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[16];
            byte[] TxData = new byte[18];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x10;		//command
            TxData[2] = 0x0C;		//data length
            TxData[3] = 0x02;		//data type, date edit first byte
            TxData[4] = 0x02;		//real data
            TxData[5] = 0x00;		//预留位
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            TxData[8] = 0x00;
            TxData[9] = 0x00;
            TxData[10] = 0x00;
            TxData[11] = 0x00;
            TxData[12] = 0x00;
            TxData[13] = 0x00;
            TxData[14] = 0x00;
            for (int i = 1; i < 15; i++)
            {
                TxData[15] += TxData[i];
            }
            if (TxData[15] == 0x17)
                TxData[15] = 0x18;
            else
                TxData[15] = TxData[15];
            TxData[16] = 0x17;		//back code
            TxData[17] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        public byte[] ISImgRead(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x15;		//command  TXC
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x01;		//data type
            TxData[4] = 0x00;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        public byte[] GetCycleState(DeviceManagement myDevManager)
        {
            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[11];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x14;		//command  
            TxData[2] = 0x05;		//data length
            TxData[3] = 0x15;		//data type
            TxData[4] = 0x00;
            TxData[5] = 0x00;
            TxData[6] = 0x00;
            TxData[7] = 0x00;
            for (int i = 1; i < 8; i++)
                TxData[8] += TxData[i];
            if (TxData[8] == 0x17)
                TxData[8] = 0x18;
            else
                TxData[8] = TxData[8];
            TxData[9] = 0x17;
            TxData[10] = 0x17;
            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 10000);

            return inputdatas;
        }
        /// <summary>
        /// 设循环参数及启动循环
        /// </summary>
        /// <returns></returns>
        public byte[] SetOvershootParameters(DeviceManagement myDevManager, DebugModelData currDebugModelData, float ov, float und, float ov_time, float und_time)
        {
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[16];

            float overtime = ov_time, overtemp = ov, undertime = und_time, undertemp = und;

            var fvar = Convert.ToSingle(overtime);
            byte[] hData = BitConverter.GetBytes(fvar);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];

            fvar = Convert.ToSingle(overtemp);
            hData = BitConverter.GetBytes(fvar);
            OperBuf[4] = hData[0];
            OperBuf[5] = hData[1];
            OperBuf[6] = hData[2];
            OperBuf[7] = hData[3];

            fvar = Convert.ToSingle(undertime);
            hData = BitConverter.GetBytes(fvar);
            OperBuf[8] = hData[0];
            OperBuf[9] = hData[1];
            OperBuf[10] = hData[2];
            OperBuf[11] = hData[3];

            fvar = Convert.ToSingle(undertemp);
            hData = BitConverter.GetBytes(fvar);
            OperBuf[12] = hData[0];
            OperBuf[13] = hData[1];
            OperBuf[14] = hData[2];
            OperBuf[15] = hData[3];

            byte[] TxData = new byte[64];

            TxData[0] = 0xaa;		    //preamble code
            TxData[1] = 0x13;		    //command  TXC
            TxData[2] = 0x12;		    //data length
            TxData[3] = 0x08;		    //data type, date edit first byte TXC
            TxData[4] = 0x0;		    //real data, reserved

            TxData[5] = OperBuf[0];	    //	
            TxData[6] = OperBuf[1];     //
            TxData[7] = OperBuf[2];
            TxData[8] = OperBuf[3];
            TxData[9] = OperBuf[4];
            TxData[10] = OperBuf[5];
            TxData[11] = OperBuf[6];	//
            TxData[12] = OperBuf[7];
            TxData[13] = OperBuf[8];
            TxData[14] = OperBuf[9];
            TxData[15] = OperBuf[10];
            TxData[16] = OperBuf[11];
            TxData[17] = OperBuf[12];	//
            TxData[18] = OperBuf[13];
            TxData[19] = OperBuf[14];
            TxData[20] = OperBuf[15];

            for (int i = 1; i < 21; i++)
                TxData[21] += TxData[i];

            if (TxData[21] == 0x17)
                TxData[21] = 0x18;

            TxData[22] = 0x17;
            TxData[23] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 24);

            return inputdatas;
        }

        /// <summary>
        /// 设循环参数及启动循环
        /// </summary>
        /// <returns></returns>
        public byte[] SetCycleParameters(DeviceManagement myDevManager, DebugModelData currDebugModelData, int currCycleindex, byte cfg)
        {
            byte[] inputdatas = new byte[64];
            byte[] OperBuf = new byte[18];

            string stempture = currDebugModelData.Initaldenaturation.ToString();
            string stime = currDebugModelData.InitaldenaTime.ToString();

            //            if (currCycleindex > 0)
            //            {
            //                stempture = currDebugModelData.Denaturating.ToString();
            //                stime = "1";
            //            }
            //            else
            //            {
            //                stime = currDebugModelData.InitaldenaTime.ToString();
            //            }

            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var dena_Time = Convert.ToInt32(stime);

            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];

            stempture = currDebugModelData.Holdon.ToString();
            stime = currDebugModelData.HoldonTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];

            var HoldonTime = Convert.ToInt32(stime);
            byte[] HoldonTimes = BitConverter.GetBytes(HoldonTime);
            OperBuf[10] = HoldonTimes[1];
            OperBuf[11] = HoldonTimes[0];

            var cycle = currDebugModelData.Cycle.ToString();
            OperBuf[12] = Convert.ToByte(cycle);

            byte[] TxData = new byte[64];
            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x10;		//data length
            TxData[3] = 0x04;		//data type, date edit first byte TXC
            TxData[4] = 0x01;		//real data, start
            TxData[5] = OperBuf[12];	//cycle setting
            TxData[6] = cfg;       //
            TxData[7] = OperBuf[0];	//inital dennature数据	
            TxData[8] = OperBuf[1];  //
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];	//extern extension数据
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];
            for (int i = 1; i < 19; i++)
                TxData[19] += TxData[i];
            if (TxData[19] == 0x17)
                TxData[19] = 0x18;
            else
                TxData[19] = TxData[19];
            TxData[20] = 0x17;
            TxData[21] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);

            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        /// <summary>
        /// peltier设定，循环参数    
        /// </summary>
        /// <returns></returns>
        public byte[] SetPeltier2(DeviceManagement myDevManager, DebugModelData currDebugModelData)
        {
            byte[] inputdatas = new byte[64];           // 
            byte[] OperBuf = new byte[18];
            byte[] TxData = new byte[64];

            //取Dennature编辑框中的数据
            string stempture = currDebugModelData.Denaturating.ToString();
            string stime = currDebugModelData.DenaturatingTime.ToString();
            var ftempture = (float)Convert.ToDouble(stempture);
            byte[] hData = BitConverter.GetBytes(ftempture);
            OperBuf[0] = hData[0];
            OperBuf[1] = hData[1];
            OperBuf[2] = hData[2];
            OperBuf[3] = hData[3];
            var dena_Time = Convert.ToInt32(stime);
            byte[] denaTimes = BitConverter.GetBytes(dena_Time);
            OperBuf[4] = denaTimes[1];
            OperBuf[5] = denaTimes[0];

            //取Anneal编辑框中的数据
            stempture = currDebugModelData.Annealing.ToString();
            stime = currDebugModelData.AnnealingTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[6] = hData[0];
            OperBuf[7] = hData[1];
            OperBuf[8] = hData[2];
            OperBuf[9] = hData[3];
            var Annealing_Time = Convert.ToInt32(stime);
            byte[] AnnealingTimes = BitConverter.GetBytes(Annealing_Time);
            //itime = Convert.ToByte(stime);	//将编辑框中整型字符串转成byte
            OperBuf[10] = AnnealingTimes[1];
            OperBuf[11] = AnnealingTimes[0];

            //取Inter Extension编辑框中的数据
            stempture = currDebugModelData.Extension.ToString();
            stime = currDebugModelData.ExtensionTime.ToString();
            ftempture = (float)Convert.ToDouble(stempture);
            hData = BitConverter.GetBytes(ftempture);
            OperBuf[12] = hData[0];
            OperBuf[13] = hData[1];
            OperBuf[14] = hData[2];
            OperBuf[15] = hData[3];
            var Extension_Time = Convert.ToInt32(stime);
            byte[] ExtensionTimes = BitConverter.GetBytes(Extension_Time);
            OperBuf[16] = ExtensionTimes[1];
            OperBuf[17] = ExtensionTimes[0];

            byte num_cycles = (byte)currDebugModelData.Cycle;
            int step_count = currDebugModelData.StepCount;

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x13;		//command  TXC
            TxData[2] = 0x16;		//data length
            TxData[3] = 0x03;		//data type, date edit first byte TXC
            TxData[4] = num_cycles;		//RegBuf[18];						
            TxData[5] = 0x01;        //RegBuf[20];	
            //设置拍照阶段
            if (currDebugModelData.stageIndex == 1)
            {
                TxData[6] = (byte)(0x10 + step_count);       // RegBuf[21];
            }
            if (currDebugModelData.stageIndex == 2)
            {
                TxData[6] = (byte)(0x20 + step_count);       // RegBuf[21];
            }
            if (currDebugModelData.stageIndex == 3)
            {
                TxData[6] = (byte)(0x0 + step_count);       // RegBuf[21];
            }

            TxData[7] = OperBuf[0];	//dennature数据
            TxData[8] = OperBuf[1];
            TxData[9] = OperBuf[2];
            TxData[10] = OperBuf[3];
            TxData[11] = OperBuf[4];
            TxData[12] = OperBuf[5];
            TxData[13] = OperBuf[6];//Anneal数据
            TxData[14] = OperBuf[7];
            TxData[15] = OperBuf[8];
            TxData[16] = OperBuf[9];
            TxData[17] = OperBuf[10];
            TxData[18] = OperBuf[11];
            TxData[19] = OperBuf[12];//Inter extension数据
            TxData[20] = OperBuf[13];
            TxData[21] = OperBuf[14];
            TxData[22] = OperBuf[15];
            TxData[23] = OperBuf[16];
            TxData[24] = OperBuf[17];
            for (int i = 1; i < 25; i++)
            {
                TxData[25] += TxData[i];
            }

            if (TxData[25] == 0x17)
                TxData[25] = 0x18;
            else
                TxData[25] = TxData[25];
            TxData[26] = 0x17;
            TxData[27] = 0x17;

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 1000);
            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }

        /// <summary>
        /// 读取 Stop Reason
        /// </summary>
        public byte[] ReadStopReason(DeviceManagement myDevManager)
        {

            byte[] inputdatas = new byte[64];
            byte[] TxData = new byte[64];

            TxData[0] = 0xaa;		//preamble code
            TxData[1] = 0x14;		//command
            TxData[2] = 0x0c;		//data length
            TxData[3] = 0x16;		//data type, date edit first byte
            TxData[4] = 0x01;		//real data, doesn't matter
            TxData[5] = 0x00;		//预留位

            for (int i = 1; i < 6; i++)
                TxData[6] += TxData[i];
            if (TxData[6] == 0x17)
                TxData[6] = 0x18;
            else
                TxData[6] = TxData[6];
            TxData[7] = 0x17;		//back code
            TxData[8] = 0x17;		//back code

            myDevManager.InputAndOutputReports(0, false, TxData, ref inputdatas, 100000);
//            string res = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);

            return inputdatas;
        }
    }
}
