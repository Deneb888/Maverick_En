//#define GD_MOMENTUM

//#define ALL_SEL

//#define BY_PASS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ViliPetek.LinearAlgebra;

namespace Anitoa
{
    public class CCurveShowMet
    {
        int numWells = 4;
        
        //int MIN_CT = 12;//之前15 // minimal allowed CT
        //int CT_TH_MULTI = 5;// 5 instead of 10, this is pre calc of Ct anyway

        //float log_threshold = 0.11f;
        //float cheat_factor = 0.06f;
        //float cheat_factor_org = 0.06f;
        //bool hide_org = true;

        private static int MAX_CHAN = 4;
        private static int MAX_WELL = 16;
        private static int MAX_CYCL = 501;//400
        public double[, ,] m_yData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[, ,] m_zData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[, ,] m_zdData = new double[MAX_CHAN, MAX_WELL, MAX_CYCL];
        public double[,] m_CTValue = new double[MAX_CHAN, MAX_WELL];
        public double[,] mtemp = new double[MAX_CHAN, MAX_CYCL];
        double[,] m_xAxis = new double[MAX_CHAN, MAX_CYCL];

        double[] x = new double[MAX_CYCL];
        double[] y = new double[MAX_CYCL];
        double[,] k = new double[MAX_WELL, 4];
        double[,] r = new double[MAX_WELL, 4];
        double[,] t = new double[MAX_WELL, 4];
        //double delta_k, delta_r, delta_t;
        //int[,] fit_count = new int[MAX_WELL, 4];
        public int[] m_Size = new int[MAX_CHAN];

        public double[,] ifactor = new double[MAX_CHAN, MAX_CYCL];


        //float[] log_threshold = new float[] { 0.11f, 0.11f, 0.11f, 0.11f };

        //float[] ct_offset = new float[4];
        public float start_temp = 50;
        const int START_CYCLE = 10;
        public bool bShowRaw;

        public double DetectTh = 400;

        public void InitData()
        {
            numWells = CommData.KsIndex;
            int i, j;
            for (i = 0; i < MAX_CHAN; i++)
            {
                for (j = 0; j < numWells; j++)
                {
                    m_CTValue[i, j] = 0;
                }
            }
        }

        public void UpdateAllcurve()
        {
            for (int iy = 0; iy < MAX_CHAN; iy++)
            {
                int datasize = m_Size[iy];
                int i;
                int size = m_Size[iy];

                if (size == 0) continue;

                for (int frameindex = 0; frameindex < numWells; frameindex++)
                {
                    double[] yData = new double[MAX_CYCL];

                    for (i = 0; i < datasize; i++)
                    {
                        yData[i] = m_yData[iy, frameindex, i];
                    }

                    double[] z = new double[MAX_CYCL];
                    double[] y = new double[MAX_CYCL];

                    double eta = 0.2;

                    z[0] = yData[0];

                    for (i = 1; i < size; i++)
                    {
                        z[i] = z[i - 1] + eta * (yData[i] - z[i - 1]);
                    }

                    for (i = 0; i < size; i++)
                    {
                        y[i] = z[i];
                    }

                    for (i = size - 2; i >= 0; i--)
                    {
                        z[i] = z[i + 1] + eta * (y[i] - z[i + 1]);
                    }

                    for (i = 0; i < size; i++)
                    {
                        m_zData[iy, frameindex, i] = z[i];
                    }
                }

                for (i = 0; i < size; i++)
                {
                    m_xAxis[iy, i] = mtemp[iy, i];
                }

                for (int frameindex = 0; frameindex < numWells; frameindex++)
                {
                    DrawMeltCurve(iy, frameindex);
                }
            }
        }

        public void DrawMeltCurve(int iy, int frameindex)
        {

            double[] yData = new double[MAX_CYCL];
            double[] y = new double[MAX_CYCL];
            double[] mt = new double[MAX_CYCL];

            double[] yData_Lowpass = new double[MAX_CYCL];

            int size = m_Size[iy];
            int i;

            try
            {
                for (i = 0; i < size; i++)
                {
                    yData[i] = m_yData[iy, frameindex, i];
                }

                //==========factor for int time=================

                //           for (int k = 0; k < size; k++)
                //           {
                //               yData[k] /= ifactor[iy, k];
                //           }

                OutlierRemove(ref yData, size);

                for (i = 0; i < size; i++)
                {
                    y[i] = yData[i];
                    mt[i] = mtemp[iy, i];
                }

                // Melt process

                double[] z = new double[MAX_CYCL];
                double eta = 0.2;

                z[0] = y[0];

                for (i = 1; i < size; i++)
                {
                    z[i] = z[i - 1] + eta * (y[i] - z[i - 1]);
                }

                for (i = 0; i < size; i++)
                {
                    y[i] = z[i];
                }

                for (i = size - 2; i >= 0; i--)
                {
                    z[i] = z[i + 1] + eta * (y[i] - z[i + 1]);
                }

                for (i = 0; i < size; i++)
                {
                    y[i] = z[i];

                    yData_Lowpass[i] = z[i];
                }

                z[0] = 5;

                for (i = 1; i < size; i++)
                {
                    z[i] = -10 * (y[i] - y[i - 1]);
                }

                for (i = 0; i < size; i++)
                {
                    y[i] = z[i] * 4;
                }

                //====================Filter the differential =================

                z[0] = y[0];

                for (i = 1; i < size; i++)
                {
                    z[i] = z[i - 1] + eta * (y[i] - z[i - 1]);
                }

                for (i = 0; i < size; i++)
                {
                    y[i] = z[i];
                }

                for (i = size - 2; i >= 0; i--)
                {
                    z[i] = z[i + 1] + eta * (y[i] - z[i + 1]);
                }

                int maxi = 0;

                // Skip until certain temperature. we will skip these points in display also so it won't look weird

                while (mt[maxi] < start_temp && maxi < size - START_CYCLE)
                {
                    maxi++;
                }

                double max = z[maxi];

                for (i = maxi + 1; i < size; i++)
                {
                    if (z[i] > max)
                    {
                        max = z[i];
                        maxi = i;
                    }
                }

                double mtemp_n = 0;

                if (maxi == 0)
                {
                    mtemp_n = mt[0];
                }
                /*else if (maxi >= size - 1)
                {
                    mtemp_n = mt[maxi];
                }*/
                else if (maxi >= size - 2)          // This is because the last point is discarded in display
                {
                    mtemp_n = mt[maxi - 1];
                }
                else
                {
                    double left_slop = (z[maxi] - z[maxi - 1]) / (mt[maxi] - mt[maxi - 1]);
                    double right_slop = (z[maxi + 1] - z[maxi]) / (mt[maxi + 1] - mt[maxi]);
                    double percent = -left_slop / (right_slop - left_slop);
                    mtemp_n = mt[maxi - 1] + percent * (mt[maxi + 1] - mt[maxi]);
                }

                if (max > DetectTh)
                {
                    m_CTValue[iy, frameindex] = mtemp_n;
                }
                else
                {
                    m_CTValue[iy, frameindex] = 0;
                }

                for (i = 0; i < size; i++)
                {
                    if (bShowRaw)
                    {
                        m_zdData[iy, frameindex, i] = yData_Lowpass[i];
                    }
                    else
                    {
                        m_zdData[iy, frameindex, i] = z[i];
                    }
#if BY_PASS
                    //m_zdData[iy, frameindex, i] = yData_Lowpass[i];             // By pass to show raw data
                    m_zdData[iy, frameindex, i] = yData[i];                     // By pass to show raw data
#endif
                }
            }
            catch (Exception ex)
            {
                m_CTValue[iy, frameindex] = 0;
            }
        }

        const double OUTLIER_THRESHOLD = 2.8;

        private void OutlierRemove(ref double[] yData, int size)
        {
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
        }

    }
}
