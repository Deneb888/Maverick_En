using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anitoa
{
    public class t_experiment
    {
        /// <summary>
        /// 实验id
        /// </summary>
        public int experimentid { get; set; }
        /// <summary>
        /// 实验名称
        /// </summary>
        public string experimentname { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }
        /// <summary>
        /// 循环次数
        /// </summary>
        public string CyderNum { get; set; }

    }
}
