using System;
using System.Collections.Concurrent;
using System.Linq;

namespace ObjectPool.Net
{
    public class FixedSizeObjectPoolClient<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly int _capacity;

        public int DisposedCount { get; private set; }

        public FixedSizeObjectPoolClient(int capacity, Func<T> valueFactory)
        {
            if (capacity < 1)         throw new ArgumentOutOfRangeException(nameof(capacity), $"The argument '{nameof(capacity)}' must be greater than zero.");
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            _queue    = new ConcurrentQueue<T>(Enumerable.Range(0, capacity).Select(_ => valueFactory.Invoke()));
            _capacity = capacity;
        }

        public T Get()
        {
            while (true)
            {
                if (_queue.TryDequeue(out var value))
                {
                    return value;
                }
            }
        }

        public void Set(T value)
        {
            lock (_queue)
            {
                if (_queue.Count < _capacity)
                {
                    _queue.Enqueue(value);
                }
                else
                {
                    var generation = GC.GetGeneration(value);

                    GC.Collect(generation);

                    DisposedCount++;
                }
            }
        }
    }
}
