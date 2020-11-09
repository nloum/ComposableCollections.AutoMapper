using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class AutoMapperKeysAndValuesReadOnlyDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue> : MappingKeysAndValuesReadOnlyDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue>, IComposableReadOnlyDictionary<TKey, TValue>
    {
        public AutoMapperKeysAndValuesReadOnlyDictionaryAdapter(IComposableReadOnlyDictionary<TSourceKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>, mapper.Map<TSourceKey, TKey>, mapper.Map<TKey, TSourceKey>)
        {
        }
    }
}