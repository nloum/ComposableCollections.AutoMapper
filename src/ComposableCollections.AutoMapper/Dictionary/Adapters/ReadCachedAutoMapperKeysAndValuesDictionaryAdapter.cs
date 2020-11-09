using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class
        ReadCachedAutoMapperKeysAndValuesDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue> :
            ReadCachedMappingKeysAndValuesDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue>,
            IComposableDictionary<TKey, TValue>
    {
        private readonly IReadCachedDictionary<TSourceKey, TSourceValue> _innerValues;
        private readonly IMapper _mapper;

        public ReadCachedAutoMapperKeysAndValuesDictionaryAdapter(IReadCachedDictionary<TSourceKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>, mapper.Map<TValue, TSourceValue>, mapper.Map<TSourceKey, TKey>, mapper.Map<TKey, TSourceKey>)
        {
            _innerValues = innerValues;
            _mapper = mapper;
        }
    }
}