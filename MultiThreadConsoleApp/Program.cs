using System;
using System.Diagnostics;

namespace MultiThreadConsoleApp
{
    internal class Program
    {
        static int ThreadCount = 4;
        static int ElementCount = 1000;

        private static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // split task in multiple threads, main thread fills queue, other threads take element and do some calculation
            // processing element is not depended on other elements or some ordering (output of console.writelines is not ordered, input queue is ordered by i)
            var queue = new MyQueue(ThreadCount, processElement);

            for (int i = 0; i < ElementCount; i++)
            {
                var newElement = new MyQueueElement
                {
                    Name = $"Element {i}",
                    Counter = i
                };

                queue.Add2Queue(newElement);
            }

            stopwatch.Stop();

            TimeSpan ts = stopwatch.Elapsed;

            var elapsedTime = string.Format("Runtime {0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            Console.WriteLine(elapsedTime);
            Console.WriteLine("Done. Press any key to exit");
            Console.ReadKey();

        }

        //perform calculation
        private static MyQueueResult processElement(MyQueueElement element)
        {
            MyQueueResult result = new MyQueueResult
            {
                Name = element.Name,
                Result = element.Counter * 2
            };

            Console.WriteLine($"Name : {result.Name}, Result: {result.Result}");

            return result;
        }
    }
}