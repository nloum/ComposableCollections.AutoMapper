using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    /// <summary>
    /// Using two abstract Convert methods, converts an IDictionaryEx{TKey, TInnerValue} to an
    /// IDictionaryEx{TKey, TValue} instance. This will lazily convert objects in the underlying innerValues.
    /// This class works whether innerValues changes underneath it or not.
    /// </summary>
    public class AutoMapperKeysAndValuesDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue> : MappingKeysAndValuesDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue>, IComposableDictionary<TKey, TValue>
    {
        public AutoMapperKeysAndValuesDictionaryAdapter(IComposableDictionary<TSourceKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>, mapper.Map<TValue, TSourceValue>, mapper.Map<TSourceKey, TKey>, mapper.Map<TKey, TSourceKey>)
        {
        }
    }
}