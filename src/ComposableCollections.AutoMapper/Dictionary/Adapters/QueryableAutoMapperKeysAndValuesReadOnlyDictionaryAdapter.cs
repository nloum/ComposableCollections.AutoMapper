using System.Linq;
using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class QueryableAutoMapperKeysAndValuesReadOnlyDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue> :
        MappingKeysAndValuesReadOnlyDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue>,
        IQueryableReadOnlyDictionary<TKey, TValue>
    {
        private readonly IMapper _mapper;
        private readonly IQueryableDictionary<TSourceKey, TSourceValue> _innerValues;

        public QueryableAutoMapperKeysAndValuesReadOnlyDictionaryAdapter(IQueryableReadOnlyDictionary<TSourceKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>, mapper.Map<TSourceKey, TKey>, mapper.Map<TKey, TSourceKey>)
        {
            _mapper = mapper;
        }

        public IQueryable<TValue> Values => _mapper.ProjectTo<TValue>(_innerValues.Values);
    }
}