using System.Linq;
using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class QueryableAutoMapperKeysAndValuesDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue> :
        MappingKeysAndValuesDictionaryAdapter<TSourceKey, TSourceValue, TKey, TValue>,
        IQueryableDictionary<TKey, TValue>
    {
        private readonly IQueryableDictionary<TSourceKey, TSourceValue> _innerValues;
        private readonly IMapper _mapper;

        public QueryableAutoMapperKeysAndValuesDictionaryAdapter(IQueryableDictionary<TSourceKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>, mapper.Map<TValue, TSourceValue>, mapper.Map<TSourceKey, TKey>, mapper.Map<TKey, TSourceKey>)
        {
            _innerValues = innerValues;
            _mapper = mapper;
        }

        public IQueryable<TValue> Values => _mapper.ProjectTo<TValue>(_innerValues.Values);
    }
}