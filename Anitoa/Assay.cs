using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anitoa
{
    public class CyclerParam
    {
        public float    predenatureTemp { get; set; }
        public float    predenatureTime { get; set; }
        public float    denatureTemp { get; set; }
        public float    denatureTime { get; set; }
        public float    annealingTemp { get; set; }
        public float    annealingTime { get; set; }
        public float    extensionTemp { get; set; }
        public float    extensionTime { get; set; }
        public bool     extensionEn { get; set; }
        public float    rtTemp { get; set; }
        public float    rtTime { get; set; }
        public bool     rtEn { get; set; }
        public bool     meltEn { get; set; }
        public int      numCycles { get; set; }
    }

    public class ChannelParam
    {
        public bool     active { get; set; }
        public string   name { get; set; }
        public string   type { get; set; }
        public string   finalUnits { get; set; }
        public string   description { get; set; }
        public float    negCtrlOKCt { get; set; }
        public float    posCtrlOKCtStart { get; set; }
        public float    posCtrlOKCtEnd { get; set; }
        public float    stdCurveSlope { get; set; }
        public float    stdCurveIntercept { get; set; }
        public float    posCtrlConcentration { get; set; }
        public string   posCtrlUnits { get; set; }
        public float    cellConversionFactor { get; set; }
        public float    loq { get; set; }
        public string   lod { get; set; }

    }

    public class Assay
    {
        public int assayId { get; set; }
        public string assayName { get; set; }
        public string assayVersion { get; set; }

        public int extractionFieldElution { get; set; }
        public int extractionFieldTest { get; set; }
        public int extractionLabElution { get; set; }
        public int extractionLabTest { get; set; }

        public CyclerParam cyclerParam = new CyclerParam();
        public List<ChannelParam> channelParamLists { get; set; }

        public Assay()
        {
            
        }
    }
}