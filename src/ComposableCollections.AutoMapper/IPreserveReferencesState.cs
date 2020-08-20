using System;
using ComposableCollections.Dictionary;

namespace ComposableCollections
{
    public interface IPreserveReferencesState
    {
        bool Initialize<TKey, TValue>() where TValue : new();
        bool Initialize<TKey, TValue>(Func<TKey, TValue> constructor);
        IComposableDictionary<TKey, TValue> GetCache<TKey, TValue>();
        void Clear();
    }
}