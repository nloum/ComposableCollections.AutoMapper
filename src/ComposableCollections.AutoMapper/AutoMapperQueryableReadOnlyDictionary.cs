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
    public class AutoMapperQueryableReadOnlyDictionary<TKey1, TValue1, TKey2, TValue2> : QueryableToQueryableReadOnlyDictionaryAdapter<TKey2, TValue2>
    {
        public AutoMapperQueryableReadOnlyDictionary(IQueryableReadOnlyDictionary<TKey1, TValue1> innerValues, Expression<Func<TValue2, TKey2>> id, IConfigurationProvider configurationProvider, IMapper mapper)
            : base(mapper.ProjectTo<TValue2>(innerValues.Values, configurationProvider), id)
        {
        }
    }
}