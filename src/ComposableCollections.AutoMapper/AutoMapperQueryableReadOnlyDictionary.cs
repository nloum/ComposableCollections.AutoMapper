using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections
{
    public class AutoMapperQueryableReadOnlyDictionary<TKey1, TValue1, TKey2, TValue2> : MappingReadOnlyDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>, IQueryableReadOnlyDictionary<TKey2, TValue2>
    {
        private readonly IQueryableReadOnlyDictionary<TKey1, TValue1> _innerValues;
        private readonly Func<TKey2, Expression<Func<TValue2, bool>>> _compareId;
        private IConfigurationProvider _configurationProvider;
        private readonly IMapper _mapper;

        public AutoMapperQueryableReadOnlyDictionary(IQueryableReadOnlyDictionary<TKey1, TValue1> innerValues, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) : base(innerValues)
        {
            _innerValues = innerValues;
            _compareId = compareId;
            _configurationProvider = configurationProvider;
            _mapper = mapper;
        }

        protected override IKeyValue<TKey2, TValue2> Convert(TKey1 key, TValue1 value)
        {
            return new KeyValue<TKey2, TValue2>(_mapper.Map<TKey1, TKey2>(key), _mapper.Map<TValue1, TValue2>(value));
        }

        protected override TKey1 ConvertToKey1(TKey2 key)
        {
            return _mapper.Map<TKey2, TKey1>(key);
        }

        protected override TKey2 ConvertToKey2(TKey1 key)
        {
            return _mapper.Map<TKey1, TKey2>(key);
        }

        public new IQueryable<TValue2> Values => _innerValues.Values.ProjectTo<TValue2>(_configurationProvider);

        public override bool TryGetValue(TKey2 key, out TValue2 value)
        {
            value = Values.Where(_compareId(key)).SingleOrDefault();
            return value != null;
        }
    }
}