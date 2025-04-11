namespace MobileApp.Models
{
    public class HeartConditionMonitor
    {
        private readonly Queue<float> ecgQueue = new();
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

        public void AddData(float data)
        {
            lock (lockObj)
            {
                ecgQueue.Enqueue(data);
                if (ecgQueue.Count > 180)
                {
                    ecgQueue.Dequeue();
                }
            }
        }

        public string ClassifyHeartCondition()
        {
            lock (lockObj)
            {
                const int requiredDataLength = 140;
                if (ecgQueue.Count < requiredDataLength) return "Tidak Cukup Data";

                var dataArray = ecgQueue.Skip(ecgQueue.Count - requiredDataLength).ToArray();
                List<int> rPeakIndices = new List<int>();
                float rThreshold = 0.7f;

                for (int i = 1; i < dataArray.Length - 1; i++)
                {
                    if (dataArray[i] > rThreshold && dataArray[i] > dataArray[i - 1] && dataArray[i] > dataArray[i + 1])
                    {
                        rPeakIndices.Add(i);
                    }
                }

                if (rPeakIndices.Count < 2) return "Tidak Cukup Data";

                List<int> rPeakIntervals = new List<int>();
                for (int i = 1; i < rPeakIndices.Count; i++)
                {
                    rPeakIntervals.Add(rPeakIndices[i] - rPeakIndices[i - 1]);
                }

                double avgInterval = rPeakIntervals.Average() * 10;
                int heartRate = (int)(60000 / avgInterval);

                float stSegmentDeviation = 0;
                float tWaveDeviation = 0;
                int sIndex = rPeakIndices[0] + 2;
                int tStartIndex = rPeakIndices[0] + 10;

                for (int i = sIndex; i < tStartIndex; i++)
                {
                    stSegmentDeviation += (dataArray[i] - 0.5f);
                }

                for (int i = tStartIndex; i < tStartIndex + 10; i++)
                {
                    tWaveDeviation += (dataArray[i] - 0.52f);
                }

                stSegmentDeviation /= (tStartIndex - sIndex);
                tWaveDeviation /= 10;

                if (stSegmentDeviation > 0.04f)
                    return "STEMI";
                else if (stSegmentDeviation < -0.02f || tWaveDeviation < -0.01f)
                    return "NSTEMI";
                else
                    return "Normal";
            }
        }
    }
}