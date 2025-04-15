﻿using MobileApp.Services;
using Syncfusion.Blazor.Data;
using System.Diagnostics;

namespace MobileApp.Models
{
    public class HeartConditionMonitor
    {
        private const int interval = 69;

        private Queue<float> ecgQueue = new();
        private readonly object lockObj = new();
        List<float> lastWave = new List<float>();
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

        public void ClearData()
        {
            lock (lockObj)
            {
                ecgQueue = new Queue<float>();
                counter = 0;
                heartCondition = string.Empty;
            }
        }

        public string ClassifyHeartCondition()
        {
            lock (lockObj)
            {
                int lastPRSegmentIndex = 0;
                int rPeakIndex = 0;
                int firstSTSegmentIndex = 0;

                if (ecgQueue.Count > interval)
                {
                    lastWave = ecgQueue.Skip(ecgQueue.Count - interval).ToList();
                    rPeakIndex = lastWave.IndexOf(lastWave.Max());
                    lastPRSegmentIndex = rPeakIndex - 3;
                    firstSTSegmentIndex = rPeakIndex + 4;

                    try
                    {
                        if (lastWave[firstSTSegmentIndex] >= (lastWave[lastPRSegmentIndex] + 0.25f))
                        {
                            if (heartCondition != "STEMI")
                            {
                                RealtimeLocation.GetCurrentLocation();
                                CurrentLocation currentLocation = RealtimeLocation.CurrentLocation;
                                Console.WriteLine($"Street: {currentLocation?.Street}");
                                Console.WriteLine($"SubLocality: {currentLocation?.SubLocality}");
                                Console.WriteLine($"Locality: {currentLocation?.Locality}");
                                Console.WriteLine($"SubAdminArea: {currentLocation?.SubAdminArea}");
                                Console.WriteLine($"AdminArea: {currentLocation?.AdminArea}");
                                Console.WriteLine($"PostalCode: {currentLocation?.PostalCode}");
                                string location = $"{currentLocation?.Street}, {currentLocation?.SubLocality}, {currentLocation?.Locality?.Remove(0, 10)}, {currentLocation?.SubAdminArea}";
                                Console.WriteLine($"Address: {location}");
                                string message = $"Panggilan darurat serangan jantung dari MERCy. Atas nama {App.CurrentUser?.Username}. Lokasi pada {location}";
                                EmergencyCall emergencyCall = new EmergencyCall();
                                emergencyCall.SendEmergencyMessage(["628995109742"], message);
                            }
                            return "STEMI";
                        }
                        else if (lastWave[firstSTSegmentIndex] <= (lastWave[lastPRSegmentIndex] - 0.25f))
                        {
                            return "NSTEMI";
                        }
                        else
                        {
                            return "Healthy Heart";
                        }
                    }
                    catch
                    {
                        return heartCondition;
                    }
                }
                else
                {
                    return "Need more data";
                }
            }
        }

        public int CalculateHeartRate()
        {
            lock (lockObj)
            {
                List<int> rPeaks = new List<int>();

                if (ecgQueue.Count > interval)
                {
                    lastWave = ecgQueue.Skip(ecgQueue.Count - interval).ToList();

                    Stopwatch s = new Stopwatch();
                    s.Start();
                    while (s.Elapsed < TimeSpan.FromSeconds(6))
                    {
                        Console.WriteLine($"Last: {lastWave.Last()}");
                        Console.WriteLine($"Last Index: {lastWave.IndexOf(lastWave.Last())}");
                        //if ()
                        //{

                        //}
                    }

                    s.Stop();
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}