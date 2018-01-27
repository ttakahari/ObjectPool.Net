using System;

namespace ObjectPool.Net
{
    public class AutomaticReturnPoolingObject<T> : IDisposable
    {
        private bool _disposed;

        private readonly T _value;
        private readonly Action<T> _returnAction;

        public T Value => _disposed ? throw new ObjectDisposedException(nameof(Value)) : _value;

        internal AutomaticReturnPoolingObject(T value, Action<T> returnAction)
        {
            _value        = value;
            _returnAction = returnAction ?? throw new ArgumentNullException(nameof(returnAction));
        }

        ~AutomaticReturnPoolingObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _returnAction.Invoke(Value);
            }

            _disposed = true;
        }
    }
}
