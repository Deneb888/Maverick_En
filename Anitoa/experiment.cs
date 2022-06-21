using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anitoa
{
    public class experiment
    {
        private static int MAX_CHAN = 4;
        private static int MAX_WELL = 16;

        private static int MAX_ROW = 2;
        private static int MAX_WELL_PER_ROW = 8;

        /// <summary>
        /// 实验名称
        /// </summary>
        public string emname { get; set; }

        /// <summary>
        /// 实验时间
        /// </summary>
        public DateTime? emdatetime { get; set; }

        /// <summary>
        /// 实验结束时间
        /// </summary>
        public DateTime? endatetime { get; set; }

        /// <summary>
        /// 实验时间-Melt
        /// </summary>
        public DateTime? meltemdatetime { get; set; }

        /// <summary>
        /// 实验结束时间-Melt
        /// </summary>
        public DateTime? meltendatetime { get; set; }

        /// <summary>
        /// 通道1类型
        /// </summary>
        public string chanonetype { get; set; }

        /// <summary>
        /// 通道1描述
        /// </summary>
        public string chanonedes { get; set; }

        /// <summary>
        /// 通道2类型
        /// </summary>
        public string chantwotype { get; set; }

        /// <summary>
        /// 通道2描述
        /// </summary>
        public string chantwodes { get; set; }

        /// <summary>
        /// 通道3类型
        /// </summary>
        public string chanthreetype { get; set; }

        /// <summary>
        /// 通道3描述
        /// </summary>
        public string chanthreedes { get; set; }

        /// <summary>
        /// 通道4类型
        /// </summary>
        public string chanfourtype { get; set; }

        /// <summary>
        /// 通道4描述
        /// </summary>
        public string chanfourdes { get; set; }

        /// <summary>
        /// A1描述
        /// </summary>
        public string A1des { get; set; }
        /// <summary>
        /// A2描述
        /// </summary>
        public string A2des { get; set; }
        /// <summary>
        /// A3描述
        /// </summary>
        public string A3des { get; set; }
        /// <summary>
        /// A4描述
        /// </summary>
        public string A4des { get; set; }

        /// <summary>
        /// A5描述
        /// </summary>
        public string A5des { get; set; }
        /// <summary>
        /// A6描述
        /// </summary>
        public string A6des { get; set; }
        /// <summary>
        /// A7描述
        /// </summary>
        public string A7des { get; set; }
        /// <summary>
        /// A8描述
        /// </summary>
        public string A8des { get; set; }

        /// <summary>
        /// B1描述
        /// </summary>
        public string B1des { get; set; }
        /// <summary>
        /// B2描述
        /// </summary>
        public string B2des { get; set; }
        /// <summary>
        /// B3描述
        /// </summary>
        public string B3des { get; set; }
        /// <summary>
        ///B4描述
        /// </summary>
        public string B4des { get; set; }

        /// <summary>
        /// B5描述
        /// </summary>
        public string B5des { get; set; }
        /// <summary>
        /// B6描述
        /// </summary>
        public string B6des { get; set; }
        /// <summary>
        /// B7描述
        /// </summary>
        public string B7des { get; set; }
        /// <summary>
        /// B8描述
        /// </summary>
        public string B8des { get; set; }

        /// <summary>
        /// 程序模式
        /// </summary>
        public string programMode { get; set; }

        /// <summary>
        /// 图像数据
        /// </summary>
        public string ImgFileName { get; set; }

        /// <summary>
        /// 图像数据 - Melt
        /// </summary>
        public string ImgFileName2 { get; set; }

        /// <summary>
        /// 设置数据
        /// </summary>
        public string SetFileName { get; set; }

        /// <summary>
        /// 循环数
        /// </summary>
        public int CyderNum { get; set; }

        /// <summary>
        /// 温度设置 
        /// </summary>
        public List<DebugModelData> DebugModelDataList { get; set; }

        public bool CbooChan1 { get; set; }
        public bool CbooChan2 { get; set; }
        public bool CbooChan3 { get; set; }
        public bool CbooChan4 { get; set; }

        public bool enAutoInt { get; set; }

        public Dictionary<string, List<string>> ampData;
        public Dictionary<string, List<string>> meltData;

        //public List<int>[,] row_index;
        //public List<int>[,] col_index;

        public string dpStr { get; set; }

        public string OpName { get; set; }
        public string OpNotes { get; set; }

        // Sample properties
        public string[,] sampleName; // = new string[MAX_CHAN, MAX_WELL];
        public string[,] sampleQuant; // = new string[MAX_CHAN, MAX_WELL];
        public string[,] sampleType; // = new string[MAX_CHAN, MAX_WELL];
        public int[,] sampleTypeIndex; // = new int[MAX_CHAN, MAX_WELL];
        public string[,] sampleQuantUnit; // = new string[MAX_CHAN, MAX_WELL];
        public int[,] sampleQuantUnitIndex; // = new int[MAX_CHAN, MAX_WELL];
        public int[,] sampleAssayIndex; // = new int[MAX_CHAN, MAX_WELL];
        public int[,] sampleExtractMethodIndex; // = new int[MAX_CHAN, MAX_WELL];

        // Assay channel properties
        // public string[] assayChanName;
        public string[] assayChanType;
        public int[] assayChanTypeIndex;

        public string[] assayChanStdSlope;
        public string[] assayChanStdIntercept;

        public string DebugLog { get; set; }

        public double crossTalk21 { get; set; }
        public double crossTalk12 { get; set; }

        public double crossTalk23 { get; set; }
        public double crossTalk32 { get; set; }

        public double crossTalk43 { get; set; }
        public double crossTalk34 { get; set; }

        public double confiTh { get; set; }
        public double ampEffTh { get; set; }
        public double snrTh { get; set; }

        public int[] gainMode; // 1: 低 low gain 0: 高 high gain

        public int curfitStartCycle, curfitMinCt;
        public double curfitCtTh;

        public float overTime { get; set; }
        public float overTemp { get; set; }
        public float underTime { get; set; }
        public float underTemp { get; set; }

        public double meltDetTh;

        public experiment()
        {
            sampleName = new string[MAX_ROW, MAX_WELL_PER_ROW];
            sampleQuant = new string[MAX_ROW, MAX_WELL_PER_ROW];
            sampleType = new string[MAX_ROW, MAX_WELL_PER_ROW];
            sampleTypeIndex = new int[MAX_ROW, MAX_WELL_PER_ROW];
            sampleQuantUnit = new string[MAX_ROW, MAX_WELL_PER_ROW];
            sampleQuantUnitIndex = new int[MAX_ROW, MAX_WELL_PER_ROW];
            sampleAssayIndex = new int[MAX_ROW, MAX_WELL_PER_ROW];
            sampleExtractMethodIndex = new int[MAX_ROW, MAX_WELL_PER_ROW];

            // assayChanName = new string[MAX_CHAN];
            assayChanType = new string[MAX_CHAN];
            assayChanTypeIndex = new int[MAX_CHAN];

            assayChanStdSlope = new string[MAX_CHAN];
            assayChanStdIntercept = new string[MAX_CHAN];

            crossTalk21 = 0.08;
            crossTalk12 = 0.09;
            crossTalk23 = 0.055;
            crossTalk32 = 0.035;
            crossTalk43 = 0;
            crossTalk34 = 0.05;

            confiTh = 0.175;
            ampEffTh = 0.2;
            snrTh = 0.125;

            gainMode = new int[MAX_CHAN];

            // 1: 低 low gain; 0: 高 high gain
            gainMode[0] = 0;
            gainMode[1] = 0;
            gainMode[2] = 0;
            gainMode[3] = 0;

            curfitStartCycle = 3;
            curfitMinCt = 13;
            curfitCtTh = 0.08;

            A1des = A2des = A3des = A4des = A5des = A6des = A7des = A8des = "";
            B1des = B2des = B3des = B4des = B5des = B6des = B7des = B8des = "";

            overTime = 4;
            overTemp = 3;
            underTime = 4;
            underTemp = 3;

            meltDetTh = 400;
        }

        public experiment ShallowCopy()
        {
            return (experiment)this.MemberwiseClone();
        }

        public bool AssayChanEn(int index)
        {
            switch(index)
            {
                case 0:
                    return CbooChan1;
                case 1:
                    return CbooChan2;
                case 2:
                    return CbooChan3;
                case 3:
                    return CbooChan4;
                default:
                    return false;
            }
        }

        public string AssayChanName(int index)
        {
            switch(index)
            {
                case 0:
                    return chanonedes;
                case 1:
                    return chantwodes;
                case 2:
                    return chanthreedes;
                case 3:
                    return chanfourdes;
                default:
                    return "";
            }
        }
    }
}
