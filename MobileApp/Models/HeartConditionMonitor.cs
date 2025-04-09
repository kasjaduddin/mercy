namespace MobileApp.Models
{
    public class HeartConditionMonitor
    {
        private readonly Queue<ushort> ecgQueue = new Queue<ushort>();
        private readonly object lockObj = new object();

        public void AddData(ushort data)
        {
            lock (lockObj)
            {
                ecgQueue.Enqueue(data);
                if (ecgQueue.Count > 100)
                    ecgQueue.Dequeue();
            }
        }
    }
}
