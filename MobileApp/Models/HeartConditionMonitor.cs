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
    }
}