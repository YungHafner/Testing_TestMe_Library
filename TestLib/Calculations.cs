using System;

namespace TestLib
{
    public class Calculations
    {
        public string[] AvailablePeriods(TimeSpan[] startTimes,
                                         int[] durations,
                                         TimeSpan beginWorkingTime,
                                         TimeSpan endWorkingTime,
                                         int consultationTime)
        {
            if (startTimes == null)
                startTimes = new TimeSpan[0];
            if (durations == null)
                durations = new int[0];

            if (consultationTime > 120)
                throw new ArgumentException("consultationTime должен быть меньше 2 часов");

            //мой код
            if (consultationTime < 0)
                throw new ArgumentException("consultationTime должен быть положительным числом");

            if (endWorkingTime < beginWorkingTime)
                throw new ArgumentException("Часы конца рабочего дня не могут быть меньше чем часы начала рабочего дня");

            if (startTimes.Length != durations.Length) 
                throw new ArgumentException("Не совпадают значения startTimes и durations, а они неразлучны");
            //
            if(durations.Any(s=> s <= 0))
                throw new ArgumentException("durations содержит отрицательное или нулевое значение");

            var queue = new Queue<(TimeSpan, int)>();
            for (int i = 0; i < startTimes.Length; i++)
                queue.Enqueue((startTimes[i], durations[i]));

            var consultTimeSpan = TimeSpan.FromMinutes(consultationTime);
            var expectedPeriods = new List<(TimeSpan, TimeSpan)>();
            var start = beginWorkingTime;
            while (start < endWorkingTime)
            {
                var next = start.Add(consultTimeSpan);
                if (queue.Count > 0)
                {
                    var busyTime = queue.Peek().Item1;
                    if (next >= busyTime)
                    {
                        if (start < busyTime && busyTime - start >= consultTimeSpan)
                        {
                            expectedPeriods.Add(new(start, busyTime));
                        }
                        var time = queue.Dequeue();
                        start = time.Item1.Add(TimeSpan.FromMinutes(time.Item2));
                        continue;
                    }
                }

                if (start > endWorkingTime)
                {
                    break;
                }

                expectedPeriods.Add(new(start, next));
                start = next;
            }


            return expectedPeriods.Select(s => $"{s.Item1.ToString(@"hh\:mm")} - {s.Item2.ToString(@"hh\:mm")}").ToArray();
        }
    }
}