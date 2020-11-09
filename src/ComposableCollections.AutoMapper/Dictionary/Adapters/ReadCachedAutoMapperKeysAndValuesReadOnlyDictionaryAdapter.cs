using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class
        ReadCachedAutoMapperKeysAndValuesReadOnlyDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue> :
            ReadCachedMappingKeysAndValuesReadOnlyDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue>,
            IComposableReadOnlyDictionary<TKey, TValue>
    {
        private readonly IReadCachedReadOnlyDictionary<TSourceKey, TSourceValue> _innerValues;
        private readonly IMapper _mapper;

        public ReadCachedAutoMapperKeysAndValuesReadOnlyDictionaryAdapter(IReadCachedReadOnlyDictionary<TSourceKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>, mapper.Map<TSourceKey, TKey>, mapper.Map<TKey, TSourceKey>)
        {
            _innerValues = innerValues;
            _mapper = mapper;
        }
    }
}