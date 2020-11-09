using System.Linq;
using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class QueryableAutoMapperValuesReadOnlyDictionaryAdapter<TKey, TSourceValue, TValue> :
        MappingValuesReadOnlyDictionaryAdapter<TKey, TSourceValue, TValue>,
        IQueryableReadOnlyDictionary<TKey, TValue>
    {
        private readonly IMapper _mapper;
        private readonly IQueryableDictionary<TKey, TSourceValue> _innerValues;

        public QueryableAutoMapperValuesReadOnlyDictionaryAdapter(IQueryableReadOnlyDictionary<TKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>)
        {
            _mapper = mapper;
        }

        public IQueryable<TValue> Values => _mapper.ProjectTo<TValue>(_innerValues.Values);
    }
}