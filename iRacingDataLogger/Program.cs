using iRacingSdkWrapper;
using CsvHelper;
using CsvHelper.Configuration;

namespace IRDataLogger
{
    internal class IRDataLogger
    {
        const string cFileName = "IRPerfData.csv";
        const int cScale = 100;

        public static SdkWrapper _iRSDKWrapper = new SdkWrapper();
        public static List<PerformanceData> gDataCollected = new List<PerformanceData>();
        public static bool gWriteHeader = true;
        public static double gLastReplayFrameNum = -1;

        public class PerformanceData
        {
            public double ReplaySessionNum { get; set; }
            public int ReplaySessionTime { get; set; }
            public double ReplayFrameNum { get; set; }
            public double FrameRate { get; set; }
            public double CpuUsageBG { get; set; }
            public double CpuUsageFG { get; set; }
            public double GpuUsage { get; set; }
        }

        static void OnTelemetryUpdated(object ?sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            double ReplayFrameNum = e.TelemetryInfo.ReplayFrameNum.Value;
            if (gLastReplayFrameNum == ReplayFrameNum)
            {
                Console.WriteLine("Simulation paused, skipping data collection.");
                return;
            }

            gLastReplayFrameNum = ReplayFrameNum;
            double ReplaySessionNum = e.TelemetryInfo.ReplaySessionNum.Value;
            int ReplaySessionTime = (int)e.TelemetryInfo.ReplaySessionTime.Value;

            double FrameRate = _iRSDKWrapper.GetTelemetryValue<float>("FrameRate").Value;
            double CpuUsageBG = cScale * _iRSDKWrapper.GetTelemetryValue<float>("CpuUsageBG").Value;
            double CpuUsageFG = cScale * _iRSDKWrapper.GetTelemetryValue<float>("CpuUsageFG").Value;
            double GpuUsage = cScale * _iRSDKWrapper.GetTelemetryValue<float>("GpuUsage").Value;

            PerformanceData thisSample = new PerformanceData
            {
                ReplaySessionNum = ReplaySessionNum,
                ReplaySessionTime = ReplaySessionTime,
                ReplayFrameNum = ReplayFrameNum,
                FrameRate = FrameRate,
                CpuUsageBG = CpuUsageBG,
                CpuUsageFG = CpuUsageFG,
                GpuUsage = GpuUsage
            };

            gDataCollected.Add(thisSample);

            if (gWriteHeader)
            {
                Console.WriteLine($"ReplaySessionNum,ReplaySessionTime,ReplayFrameNum,FrameRate,CpuUsageFG,CpuUsageBG,GpuUsage");
                gWriteHeader = false;
            }
            Console.WriteLine($"{ReplaySessionNum},{ReplaySessionTime},{ReplayFrameNum},{FrameRate},{CpuUsageFG},{CpuUsageBG},{GpuUsage}");
        }

        static void Main(string[] args)
        {
            _iRSDKWrapper.TelemetryUpdated += OnTelemetryUpdated;
            _iRSDKWrapper.TelemetryUpdateFrequency = 4;
            _iRSDKWrapper.Start();

            Console.WriteLine("Ready to collect iRacing performance data from your iRacing replay.\nEnter to quit.");
            Console.ReadLine(); ;

            _iRSDKWrapper.Stop();
            _iRSDKWrapper = null;

            if (gDataCollected.Count == 0)
            {
                Console.Write($"No Data Collected.");
                return;
            }

            using (var stream = File.Open(cFileName, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(gDataCollected);
            }

            Console.Write($"Data Collected.\n{gDataCollected.Count().ToString()} samples.\nSaved to: {cFileName}");
        }
    }
}
