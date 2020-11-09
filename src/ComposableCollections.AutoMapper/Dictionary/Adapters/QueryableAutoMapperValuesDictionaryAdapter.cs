using System.Linq;
using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class QueryableAutoMapperValuesDictionaryAdapter<TKey, TSourceValue, TValue> :
        MappingValuesDictionaryAdapter<TKey, TSourceValue, TValue>,
        IQueryableDictionary<TKey, TValue>
    {
        private readonly IQueryableDictionary<TKey, TSourceValue> _innerValues;
        private readonly IMapper _mapper;

        public QueryableAutoMapperValuesDictionaryAdapter(IQueryableDictionary<TKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>, mapper.Map<TValue, TSourceValue>)
        {
            _innerValues = innerValues;
            _mapper = mapper;
        }

        public IQueryable<TValue> Values => _mapper.ProjectTo<TValue>(_innerValues.Values);
    }
}