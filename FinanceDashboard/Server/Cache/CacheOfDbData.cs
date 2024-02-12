using FinanceDashboard.Server.Data;

namespace FinanceDashboard.Server.Cache
{
    internal class CacheOfDbData<T> where T : class
    {
        internal CacheOfDbData(TimeSpan timeOut, Func<FinanceDashboardContext, Task<T>> loadFunction)
        {
            _timeOut = timeOut;
            _loadFunction = loadFunction;
        }

        private readonly Func<FinanceDashboardContext, Task<T>> _loadFunction;

        private readonly TimeSpan _timeOut;
        private DateTime? _lastLoadTimeUtc;
        private T? _items;

        internal void Invalidate() => _lastLoadTimeUtc = null;

        internal T GetContent(FinanceDashboardContext context)
        {
            if (_lastLoadTimeUtc == null || DateTime.UtcNow - _lastLoadTimeUtc.Value > _timeOut)
            {
                lock (this)
                {
                    if (_lastLoadTimeUtc == null || DateTime.UtcNow - _lastLoadTimeUtc.Value > _timeOut)
                    {
                        var task = _loadFunction(context);
                        task.Wait();
                        _items = task.Result;
                        _lastLoadTimeUtc = DateTime.UtcNow;
                    }
                }
            }
            return _items!;
        }
    }
}
