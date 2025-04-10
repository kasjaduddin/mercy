namespace MobileApp.Models
{
    public class HeartConditionMonitor
    {
        private readonly Queue<ushort> ecgQueue = new();
        private readonly object lockObj = new();

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

        public void AddData(ushort data)
        {
            lock (lockObj)
            {
                ecgQueue.Enqueue(data);
                if (ecgQueue.Count > 180)
                    ecgQueue.Dequeue();
            }
        }

        public int CalculateHeartRate()
        {
            lock (lockObj)
            {
                int requiredDataLength = 140;
                if (ecgQueue.Count < requiredDataLength) return -1;

                var dataArray = ecgQueue.Skip(ecgQueue.Count - requiredDataLength).ToArray();
                List<int> rPeakIndices = new List<int>();
                ushort rThreshold = 700;

                for (int i = 1; i < dataArray.Length - 1; i++)
                {
                    if (dataArray[i] > rThreshold && dataArray[i] > dataArray[i - 1] && dataArray[i] > dataArray[i + 1])
                    {
                        rPeakIndices.Add(i);
                    }
                }

                if (rPeakIndices.Count < 2) return -1;

                List<int> rPeakIntervals = new List<int>();
                for (int i = 1; i < rPeakIndices.Count; i++)
                {
                    rPeakIntervals.Add(rPeakIndices[i] - rPeakIndices[i - 1]);
                }

                double avgInterval = rPeakIntervals.Average() * 10;
                int heartRate = (int)(60000 / avgInterval);

                return heartRate;
            }
        }
    }
}