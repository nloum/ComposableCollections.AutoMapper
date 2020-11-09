using System.Linq;
using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class
        ReadCachedQueryableAutoMapperKeysAndValuesReadOnlyDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue> :
            ReadCachedMappingKeysAndValuesReadOnlyDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue>,
            IReadCachedQueryableReadOnlyDictionary<TKey, TValue>
    {
        private readonly IReadCachedQueryableReadOnlyDictionary<TSourceKey, TSourceValue> _innerValues;
        private readonly IMapper _mapper;

        public ReadCachedQueryableAutoMapperKeysAndValuesReadOnlyDictionaryAdapter(IReadCachedQueryableReadOnlyDictionary<TSourceKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>, mapper.Map<TSourceKey, TKey>, mapper.Map<TKey, TSourceKey>)
        {
            _innerValues = innerValues;
            _mapper = mapper;
        }

        public IQueryable<TValue> Values => _mapper.ProjectTo<TValue>(_innerValues.Values);
    }
}