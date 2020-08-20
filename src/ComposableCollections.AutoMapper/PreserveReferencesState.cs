using System;
using ComposableCollections.Dictionary;

namespace ComposableCollections
{
    public class PreserveReferencesState : IPreserveReferencesState
    {
        private readonly object _lock = new object();
        private IComposableDictionary<Type, object> _composableDictionaries = new ComposableDictionary<Type, object>();
        private System.Collections.Generic.List<Action> _clearActions = new System.Collections.Generic.List<Action>();
        
        public bool Initialize<TKey, TValue>() where TValue : new() {
            lock (_lock)
            {
                var key = typeof(IKeyValue<TKey, TValue>);
                return _composableDictionaries.TryAdd(key, () =>
                {
                    var result = new ComposableDictionary<TKey, TValue>()
                        .WithDefaultValue(_ => new TValue());
                    _clearActions.Add(() => result.Clear());
                    return result;
                });
            }
        }
        
        public bool Initialize<TKey, TValue>(Func<TKey, TValue> constructor)
        {
            lock (_lock)
            {
                var key = typeof(IKeyValue<TKey, TValue>);
                return _composableDictionaries.TryAdd(key, () =>
                {
                    var result = new ComposableDictionary<TKey, TValue>()
                        .WithDefaultValue(constructor);
                    _clearActions.Add(() => result.Clear());
                    return result;
                });
            }
        }
        
        public IComposableDictionary<TKey, TValue> GetCache<TKey, TValue>()
        {
            lock (_lock)
            {
                var key = typeof(IKeyValue<TKey, TValue>);
                return (IComposableDictionary<TKey, TValue>) _composableDictionaries[key];
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