#define GD_MOMENTUM

#define ALL_SEL

// #define NORMALIZE_SIG

#define ALIGN_BASE

#define POLYFIT_OUTLIER         // outlier remove using polyfit

#define ENGLISH_VER

#define DARK_CORRECT

// #define CHECK_DARK           // make the last well show dark curve instead for visual debug

// #define NEG_CLIP

// #define SHOW_RAW

// #define AVG_STDEV

// #define NO_CT                // Temporarily disable Ct detection for debug purpose

#define GAP_REMOVE


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ViliPetek.LinearAlgebra;


namespace Anitoa
{
    public class CCurveShow
    {
        const int numChannels = 4;
        int numWells = 16;
        int MIN_CT = 13;                    // minimal allowed CT
        int CT_TH_MULTI = 8;                // 8 this is for pre calc of Ct, this is the level of ct multiple when threshold is 10%

        const double OUTLIER_THRESHOLD = 2.8;
        int START_CYCLE = 3;

        float cheat_factor = 0.1f;                  // Fitted curve added with some hint of raw data
        float cheat_factor2 = 0.5f;                 // The fake "raw" data
        float cheat_factorNeg = 0.33f;              // Suppress signal is judged negative

        bool hide_org = true;
        public bool norm_top = true; 

        private static int MAX_CHAN = 4;
        private static int MAX_WELL = 16;
        private static int MAX_CYCL = 501;          //

        public double[, ,] m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,] m_bData = new double[MAX_CHAN, MAX_CYCL];
        public double[,] m_bFactor = new double[MAX_CHAN, MAX_CYCL];
        public double[, ,] m_zData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,] m_CTValue = new double[MAX_CHAN, MAX_WELL];

        public double[,] m_mean = new double[MAX_CHAN, MAX_WELL];
        public bool[,] m_falsePositive = new bool[MAX_CHAN, MAX_WELL];
        public string[,] m_Confidence = new string[MAX_CHAN, MAX_WELL];
        public string[] m_Advisory = new string[MAX_CHAN];

        public double[,] m_stdev = new double[MAX_CHAN, MAX_WELL];
        public double[,] m_slope = new double[MAX_CHAN, MAX_WELL];
        public double[,] m_intercept = new double[MAX_CHAN, MAX_WELL];

        public double[,,] m_zData2 = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,,] m_yDataCopy = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];

        //double[] x = new double[MAX_CYCL];
        //double[] y = new double[MAX_CYCL];
        double[,] k = new double[MAX_WELL, MAX_CHAN];
        double[,] r = new double[MAX_WELL, MAX_CHAN];
        double[,] t = new double[MAX_WELL, MAX_CHAN];

        double delta_k, delta_r, delta_t;
        int[,] fit_count = new int[MAX_WELL, MAX_CHAN];

        public int[] m_Size = new int[MAX_CHAN];            // Zhimin: now per channel
        public double[,] ifactor = new double[MAX_CHAN, MAX_CYCL];
        public float[] log_threshold = new float[] { 0.12f, 0.12f, 0.12f, 0.12f };      // It is actually percent threshold
        float[] ct_offset = new float[MAX_CHAN];
        double[] m_max_k = new double[MAX_CHAN];

        Random ran = new Random();
        private bool no_alignment = false;

        private double NEG_SLOPE_START = -0.5;
        private double NORM_TOP_VAL = 50;               // 50 * 100 = 5000


        public void InitData()
        {
            numWells = CommData.KsIndex;
            int i, j;

            for (i = 0; i < MAX_CHAN; i++)
            {
                for (j = 0; j < numWells; j++)
                {
                    m_CTValue[i, j] = 0;
                    m_mean[i, j] = 0;
                    m_falsePositive[i, j] = false;
                    m_stdev[i, j] = 0;
                }
                for(j=0; j<MAX_CYCL; j++)
                {
                    m_bFactor[i, j] = 0;
                }
            }

            for (i = 0; i < MAX_WELL; i++)
            {
                for (j = 0; j < MAX_CHAN; j++)
                {
                    k[i, j] = 15;
                    r[i, j] = 0.3;
                    t[i, j] = 25;
                    fit_count[i, j] = 0;
                }
            }

            for (i = 0; i < 4; i++)
            {
                ct_offset[i] = (float)Math.Log(1 / log_threshold[i] - 1);
            }

#if SHOW_RAW

            cheat_factor = 1.0f;                    // Fitted curve added with some hint of raw data
            cheat_factor2 = 1.0f;                   // The fake "raw" data
            cheat_factorNeg = 1.0f;                 // Suppress signal is judged negative
#endif

        }

        public void UpdateAllcurve()
        {
            if(CommData.rawData)
            {
                no_alignment = true;

                cheat_factor = 0.1f;                    // Fitted curve added with some hint of raw data
                cheat_factor2 = 1.0f;                   // The fake "raw" data
                cheat_factorNeg = 1.0f;                 // Suppress signal is judged negative
            }
            else
            {
                no_alignment = false;

                cheat_factor = 0.1f;                  // Fitted curve added with some hint of raw data
                cheat_factor2 = 0.5f;                 // The fake "raw" data
                cheat_factorNeg = 0.25f;              // Suppress signal is judged negative
            }

            MIN_CT = CommData.experimentModelData.curfitMinCt;
            START_CYCLE = CommData.experimentModelData.curfitStartCycle;

            if (MIN_CT < 5)
                MIN_CT = 5;

            if (START_CYCLE < 1)
                START_CYCLE = 1;

            if (MIN_CT < START_CYCLE + 3) MIN_CT = START_CYCLE + 3;

            CalculateCT();

            for (int iy = 0; iy < MAX_CHAN; iy++)
            {
                double ct = 0;
                for (int frameindex = 0; frameindex < numWells; frameindex++)
                {
                    double[] yData = new double[MAX_CYCL];

                    for (int i = 0; i < m_Size[iy]; i++)
                    {
                        yData[i] = m_yData[iy, frameindex, i];
                    }

                    int size = m_Size[iy];

                    if (size < START_CYCLE)
                    {
                        for (int i = 0; i < size; i++)
                        {
                            m_zData[iy, frameindex, i] = yData[i];
                            m_zData2[iy, frameindex, i] = yData[i];
                        }

                        continue;
                    }

                    double currbase = 0;
                    int k;

                    if (size > MIN_CT)
                    {                        
                        CrosstalkCorrect(ref yData, size, iy, frameindex);

                        for (k = START_CYCLE; k < MIN_CT; k++)
                        {
                            currbase += yData[k];
                        }

                        if (currbase > 0) currbase /= (MIN_CT - START_CYCLE);

                        yData[0] += 0.5 * (currbase - yData[0]);	// Replace the first data at index 0 with half way to base value, so the curve look better.

#if !ALIGN_BASE
                        currbase = 0;
#endif

                        if (no_alignment)
                        {
                            currbase = 0;
                        }

                        for (k = 0; k < size; k++)
                        {
#if DARK_CORRECT
                            yData[k] -= currbase + m_bFactor[iy, k];
#else
                            yData[k] -= currbase;
#endif

                            if (CommData.noDarkCorrect)
                            {
                                yData[k] += m_bFactor[iy, k];
                            }

                            if (m_slope[iy, frameindex] < NEG_SLOPE_START && !CommData.noDarkCorrect)
                            {
                                yData[k] -= m_intercept[iy, frameindex] + k * m_slope[iy, frameindex] - currbase;
                            }

#if CHECK_DARK
                            if (frameindex == numWells - 1) yData[k] = m_bFactor[iy, k];
#endif
                            if(CommData.showDarkCurve)
                            {
                                if (frameindex == numWells - 1) yData[k] = m_bFactor[iy, k];
                            }
                            		
                            yData[k] *= cheat_factorNeg;               // So it is well below the threshold; cheating is done to beautify the curve

#if NEG_CLIP
                            if (yData[k] < -25)
                                yData[k] = -25;
#endif
                        }
                    }

                    //============= Test code: distortion repair==============
#if GAP_REMOVE
                    if (CommData.gap_loc[iy] > MIN_CT + 3 && !CommData.noGapRemove)    // 
                    {
                        int gap = CommData.gap_loc[iy];     // gap cycle location

                        GapRemove(ref yData, size, gap);
                    }
#endif
                    //========================================================

                    ct = m_CTValue[iy, frameindex];

                    if (hide_org && ct >= START_CYCLE + 1 && size >= MIN_CT + 7)          // if positive, proceed to curve fitting
                        continue;

                    for (int i = 0; i < size; i++)
                    {
                        m_zData[iy, frameindex, i] = yData[i];
                        m_zData2[iy, frameindex, i] = yData[i];
                    }

                    m_mean[iy, frameindex] = currbase;                      // ???
                }

                for (int frameindex = 0; frameindex < numWells; frameindex++)
                {
                    DrawSigCurve(iy, frameindex);
                }

                CheckFalsePositive(iy);
            }

            NormalizeTop();
        }

        public void CalculateCT()
        {
            // if(IsCT() > 0 && !m_bThChange) return;		//CT valus have been calculated,need not been calculated again.
            // No, always recalc initial Ct.

            for (int i = 0; i < numChannels; i++)
            {
                double[] yData = new double[MAX_CYCL];                

                DarkCorrect(i);

                for (int j = 0; j < numWells; j++)
                {
                    int size = m_Size[i];

                    for (int n = 0; n < size; n++)
                    {
                        if (CommData.noDarkCorrect)
                        {
                            yData[n] = m_yData[i, j, n];
                        }
                        else
                        {
#if DARK_CORRECT
                            yData[n] = m_yData[i, j, n] - m_bFactor[i, n];
#else
                            yData[j, n] = m_yData[i, j, n];
#endif
                        }

#if CHECK_DARK
                        if (j == numWells - 1) 
                            yData[n] = m_bFactor[i, n];
#endif
                    }

                    CrosstalkCorrect(ref yData, size, i, j);

                    if (size < MIN_CT)      //data has not enough to perform Ct calculation
                        continue;

                    OutlierRemove(ref yData, size, i, j);

                    CalcMeanStdev(yData, size, i, j);

                    PivotBase(ref yData, size, i, j);

                    //============= Test code: distortion repair==============
#if GAP_REMOVE
                    if (CommData.gap_loc[i] > MIN_CT + 3 && !CommData.noGapRemove)    // 
                    {
                        int gap = CommData.gap_loc[i];

                        GapRemove(ref yData, size, gap);
                    }
#endif
                    //========================================================

                    for (int n = 0; n < size; n++)
                    {
                        m_yDataCopy[i, j, n] = yData[n];
                    }

                }   // iterate through all wells first

                double std_mean = 0;

                for (int j = 0; j < numWells; j++)
                {
                    std_mean += m_stdev[i, j];
                }

                std_mean /= numWells;

                for (int j = 0; j < numWells; j++)
                {
                    int size = m_Size[i];

                    if (size < MIN_CT)      //data has not enough to perform Ct calculation
                        continue;

                    for (int n = 0; n < size; n++)
                    {
                        yData[n] = m_yDataCopy[i, j, n];
                    }

                    CalcCt(yData, size, i, j);
                }
            }
        }

        public void DrawSigCurve(int iy, int frameindex)
        {
            double[] val = new double[MAX_CYCL];
            double[] cyc = new double[MAX_CYCL];
            double[] val2 = new double[MAX_CYCL];

            double[] x = new double[MAX_CYCL];
            double[] y = new double[MAX_CYCL];

            double mean = 0;
            double ct = m_CTValue[iy, frameindex];
            double[] yData = new double[MAX_CYCL];
            int size = m_Size[iy];

            for (int i = 0; i < size; i++)
            {
                yData[i] = m_yData[iy, frameindex, i];
                x[i] = (double)i;
            }

            if (size < MIN_CT + 7 || ct < START_CYCLE + 1)
                return;            

            CrosstalkCorrect(ref yData, size, iy, frameindex);

            for (int i = START_CYCLE; i < MIN_CT; i++)
            {
                mean += yData[i];
            }

            mean /= MIN_CT - START_CYCLE;

            yData[0] += 0.5 * (mean - yData[0]);				// Replace the first data at index 0 with half way to base value, so the curve look better.

            for (int i = 0; i < size; i++)
            {
                yData[i] -= mean + m_bFactor[iy, i];

                if(m_slope[iy, frameindex] < NEG_SLOPE_START && !CommData.noDarkCorrect)
                {
                    yData[i] -= m_intercept[iy, frameindex] + x[i] * m_slope[iy, frameindex] - mean;
                }
            }

            //============= Test code: distortion repair==============

#if GAP_REMOVE
            if (CommData.gap_loc[iy] > MIN_CT + 3 && !CommData.noGapRemove)    // 
            {
                int gap = CommData.gap_loc[iy];

                GapRemove(ref yData, size, gap);
            }
#endif
            //========================================================

            for (int i = 0; i < size; i++)
            {                
                y[i] = yData[i];
            }

            if (fit_count[frameindex, iy] < 1)
            {
                t[frameindex, iy] = ct + 4;
                k[frameindex, iy] = y[size - 1] / 130;			// a little smaller to ensure better converge
                if (size - (int)ct < 4)
                {
                    k[frameindex, iy] *= 1.5;
                }
                curvefit(x, y, frameindex, iy, 700 + 250, size);
            }
            else if (fit_count[frameindex, iy] < 900)
            {
                curvefit(x, y, frameindex, iy, 250, size);
            }
            else
            {
                curvefit(x, y, frameindex, iy, 1, size);
            }

            for (int i = 0; i < size; i++)
            {
                cyc[i] = x[i];
                val[i] = sigmoid(x[i], k[frameindex, iy], r[frameindex, iy], t[frameindex, iy]);
                val2[i] = val[i];                  

                val[i] += cheat_factor * (yData[i] - val[i]);     	    // Some cheating :)
                val2[i] += cheat_factor2 * (yData[i] - val2[i]);        // Some cheating :)

#if !ALIGN_BASE
                val[i] += mean;
                val2[i] += mean;
#endif
                if (no_alignment)
                {
                    val[i] += mean;
                    val2[i] += mean;
                }
            }

            for (int i = 0; i < size; i++)
            {
                m_zData[iy, frameindex, i] = val[i];    //  + mean;
                m_zData2[iy, frameindex, i] = val2[i];  //  + mean;
            }

            m_mean[iy, frameindex] = mean;          // update m_mean here?
        }

        int RSIZE = 9;

        public int curvefit(double[] x, double[] y, int well, int color, int iter, int size)
        {
            double delta_ko = 0;
            double delta_ro = 0;
            double delta_to = 0;

            for (int j = 0; j < iter; j++)
            {

                delta_k = delta_r = delta_t = 0;

#if ALL_SEL

                for (int i = 3; i < size; i++)
                {
                    jacob(x[i], y[i], k[well, color], r[well, color], t[well, color], y[size - 1], fit_count[well, color]);
                }
#else
                int rsize = RSIZE;		// reduced size;
                int[] rindx = new int[RSIZE];
 
                if (rsize > size) rsize = size;

                for (int i = 0; i < rsize; i++)
                {
 
                    //rindx[i] =rand() % (size - 3) + 3;

                    rindx[i] = ran.Next(0, size - 3) + 3;
                }

                for (int i = 0; i < rsize; i++)
                {
                    jacob(x[rindx[i]], y[rindx[i]], k[well, color], r[well, color], t[well, color], y[size - 1], fit_count[well, color]);
                }
#endif

#if GD_MOMENTUM
                delta_k += 0.8 * delta_ko;
                delta_r += 0.8 * delta_ro;
                delta_t += 0.8 * delta_to;
#endif

                if ((k[well, color] > 300 && delta_k > 0) || (k[well, color] < 0 && delta_k < 0))
                    delta_k = 0;

                if ((r[well, color] > 0.65 && delta_r > 0) || (r[well, color] < 0.2 && delta_r < 0))
                    delta_r = 0;

                if ((t[well, color] > 60 && delta_t > 0) || (t[well, color] < 8 && delta_t < 0))
                    delta_t = 0;

                k[well, color] += delta_k;
                r[well, color] += delta_r;
                t[well, color] += delta_t;

                if (r[well, color] > 0.7)
                    r[well, color] = 0.7;
                else if (r[well, color] < 0.15)
                    r[well, color] = 0.15;

                fit_count[well, color] += 1;

                delta_ko = delta_k;
                delta_ro = delta_r;
                delta_to = delta_t;
            }

            double ct = t[well, color] - ct_offset[color] / r[well, color];

            if (ct >= START_CYCLE + 1 && ct <= size)
            {
                m_CTValue[color, well] = ct;
            }
//          else
//          {
//              MessageBox.Show("Minimum Ct too high, please re-adjust");
//              MessageBox.Show("Ct下限太高， 请调整");
//          }

            return 0;
        }

        public int jacob(double x, double y, double k, double r, double t, double endy, int fit_count)
        {
            double e = Math.Exp(-r * (x - t));

            double dydk = 100 / (1 + e);
            double dydr = 100 * k * e * (x - t) / ((1 + e) * (1 + e));
            double dydt = -100 * k * e * r / ((1 + e) * (1 + e));

            double yy = 100 * k / (1 + e);

            double rate = 8e-8;

            if (fit_count > 1000)
            {
                rate *= 0.1;
            }
            else if (fit_count > 400)
            {
                rate *= 0.3;
            }

            if (endy > 100)
                rate *= 500 / endy;

            delta_k += rate * (y - yy) * dydk;
            delta_r += 0.5 * rate * (y - yy) * dydr;
            delta_t += rate * (y - yy) * dydt;

            return 0;
        }


        public double sigmoid(double x, double k, double r, double t)
        {
            double y = 100 * k / (1 + Math.Exp(-r * (x - t)));
            return y;
        }


        private void DarkCorrect(int i)         // i is channel
        {
            double[] bData = new double[MAX_CYCL];
            int bsize = m_Size[i];

            if (bsize < 1)
                return;

            for (int n = 0; n < bsize; n++)
            {
                bData[n] = m_bData[i, n];
            }

            double bmean = 0;

            for (int n = 1; n < MIN_CT; n++)        // exclude first point
            {
                bmean += bData[n];
            }
            bmean /= MIN_CT - 1;

            double[] y = new double[bsize - 1];
            double[] x = new double[bsize - 1];

            for (int k = 0; k < bsize - 1; k++)
            {
                y[k] = bData[k + 1];
                x[k] = (double)k + 1;
            }

            var bpolyfit = new PolyFit(x, y, 3);
            var bfitted = bpolyfit.Fit(x);

            double[] bFactor = new double[bsize];
            for (int k = 0; k < bsize - 1; k++)
            {
                bFactor[k + 1] = bfitted[k] - bmean;
            }

            bFactor[0] = 0;

            for (int k = 0; k < bsize; k++)
            {
                m_bFactor[i, k] = bFactor[k];
            }
        }

        private void CrosstalkCorrect(ref double[] yData, int size, int i, int j)
        {
            for (int n = 0; n < size; n++)
            {
                if (i == 0 && m_Size[1] == m_Size[0] && CommData.experimentModelData.crossTalk21 > 0.01)
                {
                    yData[n] -= CommData.experimentModelData.crossTalk21 * m_yData[1, j, n];
                }

                if (i == 1 && m_Size[1] == m_Size[0] && CommData.experimentModelData.crossTalk12 > 0.01)
                {
                    yData[n] -= CommData.experimentModelData.crossTalk12 * m_yData[0, j, n];
                }

                if (i == 2 && m_Size[1] == m_Size[2] && CommData.experimentModelData.crossTalk23 > 0.01)
                {
                    yData[n] -= CommData.experimentModelData.crossTalk23 * m_yData[1, j, n];
                }

                if (i == 1 && m_Size[1] == m_Size[2] && CommData.experimentModelData.crossTalk32 > 0.01)
                {
                    yData[n] -= CommData.experimentModelData.crossTalk32 * m_yData[2, j, n];
                }

                if (i == 3 && m_Size[3] == m_Size[2] && CommData.experimentModelData.crossTalk34 > 0.01)
                {
                    yData[n] -= CommData.experimentModelData.crossTalk34 * m_yData[2, j, n];
                }

                if (i == 2 && m_Size[3] == m_Size[2] && CommData.experimentModelData.crossTalk43 > 0.01)
                {
                    yData[n] -= CommData.experimentModelData.crossTalk43 * m_yData[3, j, n];
                }
            }
        }

        private void OutlierRemove(ref double[] yData, int size, int i, int j)
        {

#if POLYFIT_OUTLIER

            double[] y = new double[size];
            double[] x = new double[size];
            double[] diff = new double[size];

            for (int k = 0; k < size; k++)
            {
                y[k] = yData[k];
                x[k] = (double)k;
            }

            var polyfit = new PolyFit(x, y, 5);
            var fitted = polyfit.Fit(x);

            for (int k = 0; k < size; k++)
            {
                diff[k] = y[k] - fitted[k];
            }

            double sum = diff.Sum();
            double mean = sum / diff.Count();

            double accum = 0.0;

            for (int k = 0; k < diff.Count(); k++)
            {
                accum += (diff[k] - mean) * (diff[k] - mean);
            }
            double stdev = Math.Sqrt(accum / diff.Count());         //方差

            for (int k = 0; k < size; k++)
            {
                if (diff[k] > OUTLIER_THRESHOLD * stdev || diff[k] < -OUTLIER_THRESHOLD * stdev)
                {
                    yData[k] = fitted[k];
                }
            }            
       
#else

            for (int n = 3; n < MIN_CT; n++)
            {
                tempData.Add(yData[n]);
            }

            sum = tempData.Sum();

            //calculate CT value
            //double sum = std::accumulate(std::begin(tempData), std::end(tempData), 0.0);		
            mean = sum / tempData.Count; //mean 

            accum = 0.0;
            for (int m = 0; m < tempData.Count; m++)
            {
                accum += (tempData[m] - mean) * (tempData[m] - mean);
            }

            //std::for_each (std::begin(tempData), std::end(tempData), [&](const double d) {  
            //    accum  += (d-mean)*(d-mean);  
            //});  
            stdev = Math.Sqrt(accum / tempData.Count); //方差

            //========== outlier data remove=================
            //std::for_each(std::begin(tempData), std::end(tempData), [&](double &d) {
            //    if (d - mean > 2 * stdev || d - mean < -2 * stdev) d = mean;
            //});


            for (int v = 0; v < tempData.Count; v++)
            {
                if (tempData[v] - mean > 2.5 * stdev || tempData[v] - mean < -2.5 * stdev)
                {
                    tempData[v] = mean;
                    yData[v + 3] = mean;
                }
            }
#endif
        }

        private void PivotBase(ref double[] yData, int size, int i, int j)        
        {
            int MIN_CT2 = MIN_CT + 12;

            if (size < MIN_CT2)
                return;

            double[] y = new double[MIN_CT2 - 3];
            double[] x = new double[MIN_CT2 - 3];
            double[] diff = new double[MIN_CT2 - 3];

            for (int k = 0; k < MIN_CT2 - 3; k++)
            {
                y[k] = yData[k + 3];
                x[k] = (double)k + 3;
            }

            var polyfit2 = new PolyFit(x, y, 1);
            var fitted2 = polyfit2.Fit(x);

            double pvt_slope = polyfit2.Coeff[1];

            if (pvt_slope > NEG_SLOPE_START)
                return;

            for (int k = 0; k < MIN_CT2 - 3; k++)
            {
                diff[k] = y[k] - fitted2[k];
            }

            double accum = 0.0;

            for (int k = 0; k < diff.Count(); k++)
            {
                accum += diff[k] * diff[k];
            }

            double stdev2 = Math.Sqrt(accum / diff.Count());         //方差

            if (stdev2 < 10)    // stdev cannot be too small, as image sensor will have stdev even without input signal.
                stdev2 = 10;

            for (int k = 0; k < size; k++)
            {
                yData[k] -= polyfit2.Coeff[0] + (double)k * polyfit2.Coeff[1];
                yData[k] += m_mean[i,j];
            }

            m_stdev[i, j] = stdev2;
            m_intercept[i, j] = polyfit2.Coeff[0];
            m_slope[i, j] = polyfit2.Coeff[1];
        }

        private void CalcMeanStdev(double[] yData, int size, int i, int j)
        {
            List<double> tempData = new List<double>();

            for (int n = START_CYCLE; n < MIN_CT; n++)
            {
                tempData.Add(yData[n]);
            }

            double sum = tempData.Sum();
            double mean = sum / tempData.Count;

            double accum = 0.0;

            for (int t = 0; t < tempData.Count; t++)
            {
                accum += (tempData[t] - mean) * (tempData[t] - mean);
            }

            double stdev = Math.Sqrt(accum / tempData.Count); //方差

            if (stdev < 10)    // stdev cannot be too small, as image sensor will have stdev even without input signal.
                stdev = 10;

            m_stdev[i, j] = stdev;
            m_mean[i, j] = mean;
        }

        private void CalcCt(double[] yData, int size, int i, int j)
        {
            double mean = m_mean[i, j];

#if AVG_STDEV
                    double stdev = std_mean;        // 
#else
            double stdev = m_stdev[i, j];
#endif

            double multiple = log_threshold[i] * CT_TH_MULTI * 10;

            if (multiple < 5)
                multiple = 5;

            if (m_slope[i, j] < NEG_SLOPE_START && !CommData.noDarkCorrect)     // increase threshold if I use slope correction
                multiple *= 1.2;

            double yvalue = stdev * multiple + mean;

            double first = yData[2];

#if NO_CT
                    continue;
#endif

            if (CommData.noCt)
                return;

            int index = 0;

            for (int g = 3; g < size; g++)
            {
                if (yvalue > first && yvalue <= yData[g])
                {
                    break;
                }
                else
                {
                    first = yData[g];
                    index++;
                }
            }

            if (index == 0 || index == size - 3)
            {
                m_CTValue[i, j] = 0;
            }
            else
            {
                index = index + 3;
                while (yData[index] - yData[index - 1] == 0) index++;
                m_CTValue[i, j] = index - (yData[index] - yvalue) / (yData[index] - yData[index - 1]);
                m_CTValue[i, j] = m_CTValue[i, j] > 0 ? m_CTValue[i, j] : 0;
            }
        }

        private void GapRemove(ref double[] ydata, int size, int gap)
        {
            int fsize = 7;

            if (fsize > gap)
                fsize = gap;

            double[] y = new double[fsize];
            double[] x = new double[fsize];
            double[] xx = new double[fsize + 1];
            double diff = 0;

            for (int k = 0; k < fsize; k++)
            {
                y[k] = ydata[gap - fsize + k];
                x[k] = (double)(gap - fsize + k);
                xx[k] = (double)(gap - fsize + k);
            }

            xx[fsize] = gap;

            var polyfit = new PolyFit(x, y, 3);
            var fitted = polyfit.Fit(xx);

            diff = ydata[gap] - fitted[fsize];

            for (int k = gap; k < size; k++)
            {
                ydata[k] -= diff;
            }
        }

        private void NormalizeTop()
        {
            double max_k_all = 0;

            for (int iy = 0; iy < MAX_CHAN; iy++)
            {
                if (m_max_k[iy] > max_k_all) max_k_all = m_max_k[iy];
            }

            for (int iy = 0; iy < MAX_CHAN; iy++)
            {
                for (int frameindex = 0; frameindex < numWells; frameindex++)
                {
                    int size = m_Size[iy];

                    if (norm_top && m_CTValue[iy, frameindex] < START_CYCLE + 1)              // Treat negative data so they scale correctly.
                    {
                        double max_k = m_max_k[iy];
                        if (max_k < 3)
                        {
                            max_k = max_k_all;
                        }

                        if (max_k < 10)
                            max_k = 10;

                        for (int i = 0; i < size; i++)
                        {
                            m_zData[iy, frameindex, i] *= NORM_TOP_VAL / max_k;
                            m_zData2[iy, frameindex, i] *= NORM_TOP_VAL / max_k;
                        }
                        continue;
                    }

                    if (norm_top && !m_falsePositive[iy, frameindex])
                    {
                        for (int i = 0; i < size; i++)
                        {
                            m_zData[iy, frameindex, i] = m_zData[iy, frameindex, i] * NORM_TOP_VAL / k[frameindex, iy];
                            m_zData2[iy, frameindex, i] = m_zData2[iy, frameindex, i] * NORM_TOP_VAL / k[frameindex, iy];
                        }
                    }
                    else if (m_falsePositive[iy, frameindex])       // If false positive, I will suppress it
                    {
                        for (int i = 0; i < size; i++)
                        {
                            m_zData2[iy, frameindex, i] *= cheat_factorNeg;
                            m_zData[iy, frameindex, i] = m_zData2[iy, frameindex, i];
                        }
                    }
                }
            }
        }

        const double r_th = 0.21;
        const double MAX_K_LOW = 10; // for 1000
        const double MAX_EFF_LOW = 0.5;

        public void CheckFalsePositive(int iy)
        {
            double max_k = 0;
            int max_i = 0;

            for (int frameindex = 0; frameindex < numWells; frameindex++)
            {
                if (m_CTValue[iy, frameindex] < 0.1)
                    continue;

                if (max_k < k[frameindex, iy])
                {
                    max_k = k[frameindex, iy];
                    max_i = frameindex;
                }
            }

            m_max_k[iy] = max_k;

            if(max_k < 10 && max_k > 0.1 && CommData.experimentModelData.gainMode[iy] == 1)
            {
                m_Advisory[iy] = "Channel " + (iy + 1).ToString() + " signal too weak. Please change gain mode to high for this channel for the next experiment.";
            }

            double[] eff = new double[numWells];
            double max_eff = 0;
            int max_ei = 0;

            for (int frameindex = 0; frameindex < numWells; frameindex++)
            {
                eff[frameindex] = Math.Exp(r[frameindex, iy]) - 1;         // amplification efficiency

                if (max_eff < eff[frameindex])
                {
                    max_eff = eff[frameindex];
                    max_ei = frameindex;
                }            
            }

            for (int frameindex = 0; frameindex < numWells; frameindex++)
            {
                m_falsePositive[iy, frameindex] = false;
                if (m_CTValue[iy, frameindex] < 0.1) continue;

                double ratio_k = 0;
                double confi = 0;
                double ratio_eff = 0;
                double snr = 0;

                if (max_k > 0) ratio_k = k[frameindex, iy] / max_k;

                if (max_eff > 0) ratio_eff = eff[frameindex] / max_eff;

                //if (r[max_i, iy] < r_th) {
                //    ratio_k *= 0.3;    // ratio discounted. If the max curve have a very slow slope
                //}

                if (max_k < MAX_K_LOW)
                {
                    ratio_k *= max_k / MAX_K_LOW;    // ratio discounted. If the max curve have a very small magnitude
                }

                if (max_eff < MAX_EFF_LOW)
                {
                    ratio_eff *= max_eff / MAX_EFF_LOW;
                }

                //confi = (ratio - 0.1) * 1.11 * 0.4 + (r[frameindex, iy] - 0.19) * 2.17 * 0.6; // Weighted average of 2 score, r generally range 0.19 to 0.65
                confi = (ratio_k - 0.1) * 1.11 * 0.8 + (ratio_eff - 0.1) * 1.11 * 0.2; // Weighted average of 2 score, r generally range 0.19 to 0.65

                if (confi < 0 || ratio_k < 0.03 || ratio_eff < 0.05)
                        confi = 0;

                snr = k[frameindex, iy] / m_stdev[iy, frameindex];

                //                m_Confidence[iy, frameindex] = "阳性可信度:" + (confi * 100).ToString("0") + "% "
                //                                                    + "ratio: " + (ratio * 100).ToString("0") + "%  " + "r: " + r[frameindex, iy].ToString("0.00") + " k: " + k[frameindex, iy].ToString("0.00") + " stdev: " + m_stdev[ iy, frameindex].ToString("0.00");       // full confidence is 100

                //              double myr = r[frameindex, iy];
                //              double eff = Math.Exp(myr) - 1;         // amplification efficiency

                //                m_Confidence[iy, frameindex] = "阳性可信度: "
                //                                                    + "sat_ratio: " + (ratio * 100).ToString("0") + "%  " + "rise rate: " + r[frameindex, iy].ToString("0.00") + "(efficiency: " + (eff*100).ToString("0.0") + "%) " + " sat_multiple " + (k[frameindex, iy] * 100 / m_stdev[iy, frameindex]).ToString("0.00") + " (stdev: " + m_stdev[iy, frameindex].ToString("0.00") + ")";       // full confidence is 100

#if ENGLISH_VER
                //      m_Confidence[iy, frameindex] = "max_k: " + (max_k*100).ToString("0.0") + " - Relative confidence: " + (confi * 100).ToString("0") + " % " + "Saturation ratio: " + (ratio_k * 100).ToString("0") + "%  " + "Amplification efficiency: " + (eff[frameindex] * 100).ToString("0.0") + "% " + " SnR: " + (k[frameindex, iy] * 100 / m_stdev[iy, frameindex]).ToString("0.00") + " (Base RMS noise: " + m_stdev[iy, frameindex].ToString("0.00") + ")";       // full confidence is 100
                m_Confidence[iy, frameindex] = "Relative strength: " + (confi * 100).ToString("0.0") + "%; " + "Amplification efficiency: " + (eff[frameindex] * 100).ToString("0.0") + "%; " + " SnR: " + (snr * 100).ToString("0.00"); // + " (Base RMS noise: " + m_stdev[iy, frameindex].ToString("0.00") + ")";       // full confidence is 100
#else
                // m_Confidence[iy, frameindex] = " -- 饱和值比例: " + (ratio * 100).ToString("0") + "%  " + "扩增效率: " + (eff[frameindex] * 100).ToString("0.0") + "% " + " 信噪比 " + (k[frameindex, iy] * 100 / m_stdev[iy, frameindex]).ToString("0.00") + " (本底噪音: " + m_stdev[iy, frameindex].ToString("0.00") + ")";       // full confidence is 100
                m_Confidence[iy, frameindex] = " -- 相对可信度: " + (confi * 100).ToString("0.0") + " % " + "扩增效率: " + (eff[frameindex] * 100).ToString("0.0") + "% " + " 信噪比 " + (snr * 100).ToString("0.00") + " (本底噪音: " + m_stdev[iy, frameindex].ToString("0.00") + ")";       // full confidence is 100
#endif

                if (confi < CommData.experimentModelData.confiTh)
                {
                    m_falsePositive[iy, frameindex] = true;
                }
                else
                {
                    if(snr < CommData.experimentModelData.snrTh || eff[frameindex] < CommData.experimentModelData.ampEffTh)
                    {
                        m_falsePositive[iy, frameindex] = true;
                    }
                }               
            }
        }

    }
}
