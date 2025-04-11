using Syncfusion.Blazor.Data;

namespace MobileApp.Models
{
    public class HeartConditionMonitor
    {
        private const int interval = 69;

        private readonly Queue<float> ecgQueue = new();
        private readonly object lockObj = new();
        private int counter = 0;
        private string heartCondition = string.Empty;

        public string HeartCondition
        { 
            get => heartCondition; 
        }

        public List<ECGDataPoint> GetECGChartData()
        {
            lock (lockObj)
            {
                return ecgQueue.Select((value, index) => new ECGDataPoint
                {
                    Time = index,
                    Value = value
                }).ToList();
            }
        }

        public void AddData(float data)
        {
            lock (lockObj)
            {
                ecgQueue.Enqueue(data);
                if (ecgQueue.Count > 180)
                {
                    ecgQueue.Dequeue();
                }

                counter++;
                if (counter % interval == 0)
                {
                    heartCondition = ClassifyHeartCondition();
                    Console.WriteLine($"Heart Condition: {heartCondition}");
                    counter = 0;
                }
            }
        }

        public string ClassifyHeartCondition()
        {
            lock (lockObj)
            {
                List<float> lastWave = new List<float>();
                int lastPRSegmentIndex = 0;
                int rPeakIndex = 0;
                int firstSTSegmentIndex = 0;

                if (ecgQueue.Count > interval)
                {
                    lastWave = ecgQueue.Skip(ecgQueue.Count - interval).ToList();
                    rPeakIndex = lastWave.IndexOf(lastWave.Max());
                    lastPRSegmentIndex = rPeakIndex - 3;
                    firstSTSegmentIndex = rPeakIndex + 3;

                    if (lastWave[firstSTSegmentIndex] >= (lastWave[lastPRSegmentIndex] + 2.5f))
                    {
                        return "STEMI";
                    }
                    else if (lastWave[firstSTSegmentIndex] <= (lastWave[lastPRSegmentIndex] - 2.5f))
                    {
                        return "NSTEMI";
                    }
                    else
                    {
                        return "Healthy Heart";
                    }
                }
                else
                {
                    return "Need more data";
                }
            }
        }
    }
}