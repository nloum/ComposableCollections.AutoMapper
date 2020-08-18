using System;
using ComposableCollections.Dictionary;

namespace ComposableCollections
{
    public class PreserveReferencesState
    {
        private IComposableDictionary<Type, object> _composableDictionaries = new ComposableDictionary<Type, object>();
        
        public IComposableDictionary<TKey, TValue> GetCache<TKey, TValue>()
        {
            var key = typeof(IKeyValue<TKey, TValue>);
            if (!_composableDictionaries.TryGetValue(key, out var result))
            {
                var stronglyTypedResult = new ComposableDictionary<TKey, TValue>();
                _composableDictionaries.Add(key, stronglyTypedResult);
                return stronglyTypedResult;
            }
            else
            {
                var stronglyTypedResult = (IComposableDictionary<TKey, TValue>) result;
                return stronglyTypedResult;
            }
        }
    }
}