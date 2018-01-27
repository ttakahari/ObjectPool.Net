using System;
using System.Collections.Concurrent;

namespace ObjectPool.Net
{
    public class SimpleObjectPoolClient<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly Func<T> _valueFactory;

        public SimpleObjectPoolClient(Func<T> valueFactory)
        {
            _queue        = new ConcurrentQueue<T>();
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public T Get()
        {
            if (!_queue.TryDequeue(out var value))
            {
                value = _valueFactory.Invoke();
            }

            return value;
        }

        public void Set(T value)
        {
            _queue.Enqueue(value);
        }
    }
}
