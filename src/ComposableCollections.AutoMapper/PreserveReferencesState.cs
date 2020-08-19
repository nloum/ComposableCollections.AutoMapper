using System;
using ComposableCollections.Dictionary;

namespace ComposableCollections
{
    public class PreserveReferencesState
    {
        private readonly object _lock = new object();
        private IComposableDictionary<Type, object> _composableDictionaries = new ComposableDictionary<Type, object>();
        private System.Collections.Generic.List<Action> _clearActions = new System.Collections.Generic.List<Action>();
        
        public IComposableDictionary<TKey, TValue> GetCache<TKey, TValue>()
        {
            lock (_lock)
            {
                var key = typeof(IKeyValue<TKey, TValue>);
                if (!_composableDictionaries.TryGetValue(key, out var result))
                {
                    var stronglyTypedResult = new ComposableDictionary<TKey, TValue>();
                    _composableDictionaries.Add(key, stronglyTypedResult);
                    _clearActions.Add(() => stronglyTypedResult.Clear());
                    return stronglyTypedResult;
                }
                else
                {
                    var stronglyTypedResult = (IComposableDictionary<TKey, TValue>) result;
                    return stronglyTypedResult;
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                foreach (var clearAction in _clearActions)
                {
                    clearAction();
                }
            }
        }
    }
}