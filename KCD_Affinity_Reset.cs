using System;
using System.Diagnostics;


namespace KCD_Affinity_Reset
{
    class Program
    {
        static void Main(string[] args)
        {
            //Find if Kingdom Come is running
            Process[] KCD = Process.GetProcessesByName("KingdomCome");

            //If running, proceed.
            if (KCD.Length != 0)

            {

                double[] affectedCore = GetAffectedCore();

                Console.WriteLine("KingdomCome running!");

                Console.WriteLine("Max performance on Core {0}: {1}", affectedCore[0], affectedCore[1]);

                string strBitMask = GetBitMask(Convert.ToInt32(affectedCore[0]), Convert.ToInt32(affectedCore[2]));

                Console.WriteLine("Current bitmask binary: {0}, integer: {1}", strBitMask, Convert.ToInt32(strBitMask, 2));

                int currentProcessAffinity = KCD[0].ProcessorAffinity.ToInt32();

                Console.WriteLine("KCD Processor affinity bitmask: {0}", Convert.ToString(currentProcessAffinity, 2));

                Console.WriteLine("Reset Processor affinity for KCD? Y/N");

                if (Console.ReadLine().ToLower() == "y")
                {

                    Console.WriteLine("Affinity will be reset!");

                    System.Threading.Thread.Sleep(250);

                    KCD[0].ProcessorAffinity = (IntPtr)Convert.ToInt32(strBitMask, 2);

                    Console.WriteLine("Affinity set to {0}.", KCD[0].ProcessorAffinity);

                    System.Threading.Thread.Sleep(250);

                    KCD[0].ProcessorAffinity = (IntPtr)currentProcessAffinity;

                    affectedCore = GetAffectedCore();

                    Console.WriteLine("Affinity was successfully changed.");

                    Console.WriteLine("Max performance on Core {0}: {1}", affectedCore[0], affectedCore[1]);
                }

                else

                {

                    Console.WriteLine("Affinity will not be changed.");

                }

                
            }

            else

            {
                Console.WriteLine("KCD not running.");
                
            }

            Console.WriteLine("Press any key to stop.");

            Console.ReadKey();

        }

        static string GetBitMask(int iAffectedCore, int iMaxCores) 
        {

            string strBitMask = "";

            for (int i = 0; i < iMaxCores; i++)

            {

                if (i == iAffectedCore)

                    strBitMask = strBitMask + "0";

                else

                    strBitMask = strBitMask + "1";

            }

            return strBitMask;
        }

        static int GetCoreCount()
        {
            int coreCount = 0;

            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())

            {

                coreCount += int.Parse(item["NumberOfCores"].ToString());

            }

            return coreCount;
        }

        static double[] GetAffectedCore()
        {

            double[] result = new double[3];

            double coreCount = Convert.ToDouble(GetCoreCount());
            
            double maxPerf = 0;

            double currentPerf = 0;

            double affectedCore = 0;
            
            PerformanceCounter[] pc = new PerformanceCounter[Convert.ToInt32(coreCount)];

            for (int i = 0; i < coreCount; i++)
            {
                currentPerf = 0;

                pc[i] = new PerformanceCounter("Processor", "% Processor Time", i.ToString());

                pc[i].NextValue();

                System.Threading.Thread.Sleep(100);

                currentPerf = pc[i].NextValue();

                if (currentPerf > maxPerf)
                {

                    maxPerf = currentPerf;

                    affectedCore = i;
                }
            }

            result[0] = affectedCore;

            result[1] = maxPerf;

            result[2] = coreCount;

            return result;

        }


    }
}
