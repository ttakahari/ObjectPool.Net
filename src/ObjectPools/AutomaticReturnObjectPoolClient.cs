using System;
using System.Collections.Concurrent;

namespace ObjectPools
{
    public class AutomaticReturnObjectPoolClient<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly Func<T> _valueFactory;

        public AutomaticReturnObjectPoolClient(Func<T> valueFactory)
        {
            _queue        = new ConcurrentQueue<T>();
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public AutomaticReturnPoolingObject<T> Get()
        {
            if (!_queue.TryDequeue(out var value))
            {
                value = _valueFactory.Invoke();
            }

            return new AutomaticReturnPoolingObject<T>(value, v => _queue.Enqueue(v));
        }
    }
}
