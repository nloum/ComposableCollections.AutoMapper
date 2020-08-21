using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ComposableCollections.Dictionary;
using SimpleMonads;

namespace ComposableCollections
{
    public class AutoMapperQueryableDictionary<TKey, TValue, TInnerValue> : MapDictionaryBase<TKey, TValue, TInnerValue>, IQueryableDictionary<TKey, TValue>
    {
        private readonly IQueryableDictionary<TKey, TInnerValue> _innerValues;
        private readonly Func<TKey, Expression<Func<TValue, bool>>> _compareId;
        private IConfigurationProvider _configurationProvider;
        private readonly IMapper _mapper;

        public AutoMapperQueryableDictionary(IQueryableDictionary<TKey, TInnerValue> innerValues, Func<TKey, Expression<Func<TValue, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper = null) : base(innerValues)
        {
            _innerValues = innerValues;
            _compareId = compareId;
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

        public override bool TryGetValue(TKey key, out TValue value)
        {
            value = Values.Where(_compareId(key)).SingleOrDefault();
            return value != null;
        }

        //public override IEnumerable<TValue> Values { get; }
        
        // public TValue this[TKey key]
        // {
        //     get {
        //         
        //     }
        //     set => base[key] = value;
        // }

        //IEnumerable<TValue> IComposableReadOnlyDictionary<TKey, TValue>.Values => Values;

        bool IComposableDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }
    }
}