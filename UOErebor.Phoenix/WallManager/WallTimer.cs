using System;
using System.Timers;

namespace Phoenix.Plugins.WallManager
{
    public class WallTimer
    {
        DateTime start;
        public WallTimer(int delay, string Name)
        {
            start = DateTime.Now;
            DateTime ReqTime = DateTime.Now;
            Timer w;
            w = new Timer(delay);
            w.Elapsed += (sender, e) => W_Elapsed(sender, e, ReqTime, Name, delay, w);

            w.Start();
        }

        private void W_Elapsed(object sender, ElapsedEventArgs e, DateTime startTime, string name, int delay, Timer w)
        {
            for (int i = 0; i < 8; i++)
            {
                if (DateTime.Now - startTime >= TimeSpan.FromMilliseconds(delay * i)
                    && DateTime.Now - startTime < TimeSpan.FromMilliseconds(delay * (i + 1)))
                {
                    UO.PrintInformation("{0} zed za {1}", name, 8 - i);
                    return;
                }
            }
            if (DateTime.Now - startTime > TimeSpan.FromSeconds(7))
            {
                w.Close();
                w.Stop();
                w.Dispose();
            }

        }
    }
}