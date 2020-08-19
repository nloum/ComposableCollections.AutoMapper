using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ComposableCollections.Dictionary;

namespace ComposableCollections
{
    public class AutoMapperQueryableDictionary<TKey, TValue, TInnerValue> : MapDictionaryBase<TKey, TValue, TInnerValue>, IQueryableDictionary<TKey, TValue>
    {
        private readonly IQueryableDictionary<TKey, TInnerValue> _innerValues;
        private IConfigurationProvider _configurationProvider;
        private readonly IMapper _mapper;

        public AutoMapperQueryableDictionary(IQueryableDictionary<TKey, TInnerValue> innerValues, IConfigurationProvider configurationProvider, IMapper mapper = null) : base(innerValues)
        {
            _innerValues = innerValues;
            _configurationProvider = configurationProvider;

            if (mapper == null)
            {
                _mapper = _configurationProvider.CreateMapper();
            }
            
            _mapper = mapper;
        }

        protected override TInnerValue Convert(TKey key, TValue value)
        {
            return _mapper.Map<TValue, TInnerValue>(value);
        }

        protected override TValue Convert(TKey key, TInnerValue innerValue)
        {
            return _mapper.Map<TInnerValue, TValue>(innerValue);
        }

        public new IQueryable<TValue> Values => _innerValues.Values.ProjectTo<TValue>(_configurationProvider);
    }
}