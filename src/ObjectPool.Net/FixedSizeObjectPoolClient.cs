using System;
using System.Collections.Concurrent;

namespace ObjectPool.Net
{
    public class FixedSizeObjectPoolClient<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly int _capacity;
        private readonly Func<T> _valueFactory;

        public int DisposedCount { get; private set; }

        public int PoolCount { get; private set; }

        public FixedSizeObjectPoolClient(int capacity, Func<T> valueFactory)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"The argument '{nameof(capacity)}' must be greater than zero.");
            }

            _queue        = new ConcurrentQueue<T>();
            _capacity     = capacity;
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public T Get()
        {
            T value;

            while (!_queue.TryDequeue(out value))
            {
                if (PoolCount < _capacity)
                {
                    value = _valueFactory.Invoke();

                    PoolCount++;

                    break;
                }
            }

            return value;
        }

        public void Set(T value)
        {
            if (PoolCount < _capacity)
            {
                _queue.Enqueue(value);
            }
            else
            {
                DisposedCount++;
            }
        }

        public void Clear()
        {
            while (_queue.TryDequeue(out var _)) { }
        }
    }
}
