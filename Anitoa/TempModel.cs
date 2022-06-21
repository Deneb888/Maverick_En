using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anitoa
{
    class TempModel
    {
        float dt = 0.035f, x, x0, y, xd, yd, drive;
        // float init_temp;
        float c = 12, c2 = 1.5f, kp = 300, ki = 0.2f, l = 0.3f, sige = 0;
        float xdl1, xdl2, xdl3, xdl4;

        float h_bd = 256, l_bd = -90;
        // float dn_temp, dn_time, an_temp, an_time, ex_temp, ex_time;
        float t;

        public void SetInitTemp(float init_temp)
        {
            y = init_temp;
            x = init_temp;
            x0 = init_temp;
            xdl1 = xdl2 = xdl3 = xdl4 = init_temp;
        }

        public void SimDt()
        {
            sige = sige + (x0 - xdl4);
            drive = -kp * xdl4 + kp * x0 + ki * sige;

            if (drive > h_bd)
                drive = h_bd;
            else if (drive < l_bd)
                drive = l_bd;

            xd = (-l * x + drive) / c;
            x = x + xd * dt;

            xdl4 = xdl3;
            xdl3 = xdl2;
            xdl2 = xdl1;
            xdl1 = x;

            yd = (-y + x) / c2;
            y = y + yd * dt;

            t += dt;

            //v_x.push_back(y);
            //v_t.push_back(t);
            //v_s.push_back(x0);
        }

        public float SimStep(float set_point, float time_period, float ramp_speed, float ov_temp, float ov_time)
        {
            float timer = 0;
            float diff;
            float ramp_time = 0;
            float timeout_time = 1000;
            bool target = false;

            time_period *= 1.1f;
            time_period += 3;

            bool dir = false;    // up
            if (set_point < x)
                dir = true;    // down

            float ovs = 0;
            if (!dir)
                ovs = ov_temp;
            else
                ovs = -ov_temp;

            diff = Math.Abs(set_point + ovs - x0);

            if (ramp_speed > 0)
            {
                ramp_time = diff / ramp_speed;
            }

            while (timer < timeout_time)
            {
                if (timer < ramp_time)
                {
                    if (!dir)
                    {
                        x0 += ramp_speed * dt;
                    }
                    else
                    {
                        x0 -= ramp_speed * dt;
                    }
                }
                else if (timer < ramp_time + ov_time)
                {
                    x0 = set_point + ovs;
                }
                else x0 = set_point;

                SimDt();
                timer += dt;
                diff = Math.Abs(x - set_point);

                if (diff < 0.5 && !target)
                {
                    timeout_time = timer + time_period + ov_time;
                    target = true;
                }
            }

            return timer;
        }
    }
}
