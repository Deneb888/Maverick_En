using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anitoa
{
    public class DebugModelData
    {
        public double Hotlid { get; set; }
        public double InitaldenaturationStart { get; set; }
        public double Initaldenaturation { get; set; }
        public double InitaldenaTime { get; set; }
        public double DenaturatingStart { get; set; }
        public double Denaturating { get; set; }
        public double DenaturatingTime { get; set; }
        public double AnnealingStart { get; set; }
        public double Annealing { get; set; }
        public double AnnealingTime { get; set; }
        public double ExtensionStart { get; set; }
        public double Extension { get; set; }
        public double ExtensionTime { get; set; }
        public double HoldonStart { get; set; }
        public double Holdon { get; set; }
        public double HoldonTime { get; set; }
        public int Cycle { get; set; }
        /// <summary>
        /// 第几段的启动循环
        /// </summary>
        public int stageIndex { get; set; }

        public double Step4Start { get; set; }
        public double Step4 { get; set; }
        public double Step4Time { get; set; }

        public double InitaldenaturationStart2 { get; set; }
        public double Initaldenaturation2 { get; set; }
        public double InitaldenaTime2 { get; set; }

        public double MeltStart { get; set; }
        public double MeltEnd { get; set; }
        public double MeltStartTime { get; set; }
        public double MeltEndTime { get; set; }
        
        public int ifpz { get; set; }

        /// <summary>
        /// 几步
        /// </summary>
        public int StepCount { get; set; }
        public int InitDenatureStepCount { get; set; }

        /// <summary>
        /// 循环状态
        /// </summary>
        public int cyclestate { get; set; }     // not really used

        public bool enMelt { get; set; }

        public DebugModelData()
        {
            MeltStartTime = 1;
            MeltEndTime = 1;
        }
    }
}
