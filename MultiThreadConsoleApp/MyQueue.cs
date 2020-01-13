using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreadConsoleApp
{
    public class MyQueue
    {
        /// <summary>
        /// queue containing elements to process
        /// </summary>
        private readonly BlockingCollection<MyQueueElement> _queue = new BlockingCollection<MyQueueElement>();

        /// <summary>
        /// use to end or abort calculation
        /// </summary>
        private static readonly CancellationTokenSource TokenSource = new CancellationTokenSource();

        /// <summary>
        /// array to save current working threads
        /// </summary>
        private readonly Task[] _tasks;

        public MyQueue(int numberOfThreads, Func<MyQueueElement, MyQueueResult> processElement)  
        {                                    
            _tasks = new Task[numberOfThreads];

            for (var i = 0; i < numberOfThreads; i++)
            {
                var consumerTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        foreach (var queueElement in _queue.GetConsumingEnumerable(TokenSource.Token))
                        {
                            if (TokenSource.IsCancellationRequested)
                            {
                                return;
                            }

                            // do some calculation, whatever needs to be done
                            var result = processElement(queueElement);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                });

                // append task to list of tasks
                _tasks[i] = consumerTask;
            }
        }

        public void Add2Queue(MyQueueElement newQueueElement)
        {
            _queue.Add(newQueueElement);
        }

        public void EndCalculation()
        {
            TokenSource.Cancel();

            Task.WaitAll(_tasks);
        }

        public bool Completed()
        {
            return _queue.IsCompleted;
        }
    }
}
