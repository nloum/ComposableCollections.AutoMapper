using System;
		        using System.Collections.Generic;
				using AutoMapper;
		        using System.Linq;
		        using System.Linq.Expressions;
		        using ComposableCollections.Common;
		        using ComposableCollections.Dictionary;
		        using ComposableCollections.Dictionary.Adapters;
		        using ComposableCollections.Dictionary.Decorators;
		        using ComposableCollections.Dictionary.Interfaces;
		        using ComposableCollections.Dictionary.Sources;
		        using ComposableCollections.Dictionary.Transactional;
		        using ComposableCollections.Dictionary.WithBuiltInKey;
		        using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;
		        using UtilityDisposables;

			        namespace ComposableCollections
		        {
        public static partial class DictionaryExtensions
        {
#region WithMapping - different key types
public static ICachedDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ICachedDictionary<TKey1, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>(source, (key, value) => new KeyValue<TKey2, TValue2>(mapper.Map<TKey1, TKey2>(key), mapper.Map<TValue1, TValue2>(value)),
(key, value) => new KeyValue<TKey1, TValue1>(mapper.Map<TKey2, TKey1>(key), mapper.Map<TValue2, TValue1>(value)),key => mapper.Map<TKey1, TKey2>(key),key => mapper.Map<TKey2, TKey1>(key));
var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey2, TValue2>(mappedSource);
    return new CachedDictionaryAdapter<TKey2, TValue2>(mappedSource, cachedMapSource.AsBypassCache, cachedMapSource.AsNeverFlush, () => {  cachedMapSource.FlushCache(); source.FlushCache(); }, cachedMapSource.GetWrites);
}
public static ICachedDisposableDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ICachedDisposableDictionary<TKey1, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>(source, (key, value) => new KeyValue<TKey2, TValue2>(mapper.Map<TKey1, TKey2>(key), mapper.Map<TValue1, TValue2>(value)),
(key, value) => new KeyValue<TKey1, TValue1>(mapper.Map<TKey2, TKey1>(key), mapper.Map<TValue2, TValue1>(value)),key => mapper.Map<TKey1, TKey2>(key),key => mapper.Map<TKey2, TKey1>(key));
var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey2, TValue2>(mappedSource);
    return new CachedDisposableDictionaryAdapter<TKey2, TValue2>(mappedSource, cachedMapSource.AsBypassCache, cachedMapSource.AsNeverFlush, () => {  cachedMapSource.FlushCache(); source.FlushCache(); }, cachedMapSource.GetWrites, source);
}
public static ICachedDisposableQueryableDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ICachedDisposableQueryableDictionary<TKey1, TValue1> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableDictionary<TKey1, TValue1, TKey2, TValue2>(source, compareId, configurationProvider, mapper);
var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey2, TValue2>(mappedSource);
    return new CachedDisposableQueryableDictionaryAdapter<TKey2, TValue2>(mappedSource, cachedMapSource.AsBypassCache, cachedMapSource.AsNeverFlush, () => {  cachedMapSource.FlushCache(); source.FlushCache(); }, cachedMapSource.GetWrites, source, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static ICachedQueryableDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ICachedQueryableDictionary<TKey1, TValue1> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableDictionary<TKey1, TValue1, TKey2, TValue2>(source, compareId, configurationProvider, mapper);
var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey2, TValue2>(mappedSource);
    return new CachedQueryableDictionaryAdapter<TKey2, TValue2>(mappedSource, cachedMapSource.AsBypassCache, cachedMapSource.AsNeverFlush, () => {  cachedMapSource.FlushCache(); source.FlushCache(); }, cachedMapSource.GetWrites, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static IComposableDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IComposableDictionary<TKey1, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>(source, (key, value) => new KeyValue<TKey2, TValue2>(mapper.Map<TKey1, TKey2>(key), mapper.Map<TValue1, TValue2>(value)),
(key, value) => new KeyValue<TKey1, TValue1>(mapper.Map<TKey2, TKey1>(key), mapper.Map<TValue2, TValue1>(value)),key => mapper.Map<TKey1, TKey2>(key),key => mapper.Map<TKey2, TKey1>(key));
    return mappedSource;
}
public static IComposableReadOnlyDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IComposableReadOnlyDictionary<TKey1, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingReadOnlyDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>(source,
(key, value) => new KeyValue<TKey2, TValue2>(mapper.Map<TKey1, TKey2>(key), mapper.Map<TValue1, TValue2>(value)),
key => mapper.Map<TKey1, TKey2>(key),
key => mapper.Map<TKey2, TKey1>(key));
    return mappedSource;
}
public static IDisposableDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IDisposableDictionary<TKey1, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>(source, (key, value) => new KeyValue<TKey2, TValue2>(mapper.Map<TKey1, TKey2>(key), mapper.Map<TValue1, TValue2>(value)),
(key, value) => new KeyValue<TKey1, TValue1>(mapper.Map<TKey2, TKey1>(key), mapper.Map<TValue2, TValue1>(value)),key => mapper.Map<TKey1, TKey2>(key),key => mapper.Map<TKey2, TKey1>(key));
    return new DisposableDictionaryAdapter<TKey2, TValue2>(mappedSource, source);
}
public static IDisposableQueryableDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IDisposableQueryableDictionary<TKey1, TValue1> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableDictionary<TKey1, TValue1, TKey2, TValue2>(source, compareId, configurationProvider, mapper);
    return new DisposableQueryableDictionaryAdapter<TKey2, TValue2>(mappedSource, source, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static IDisposableQueryableReadOnlyDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IDisposableQueryableReadOnlyDictionary<TKey1, TValue1> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableReadOnlyDictionary<TKey1, TValue1, TKey2, TValue2>(source, compareId, configurationProvider, mapper);
    return new DisposableQueryableReadOnlyDictionaryAdapter<TKey2, TValue2>(mappedSource, source, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static IDisposableReadOnlyDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IDisposableReadOnlyDictionary<TKey1, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingReadOnlyDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>(source,
(key, value) => new KeyValue<TKey2, TValue2>(mapper.Map<TKey1, TKey2>(key), mapper.Map<TValue1, TValue2>(value)),
key => mapper.Map<TKey1, TKey2>(key),
key => mapper.Map<TKey2, TKey1>(key));
    return new DisposableReadOnlyDictionaryAdapter<TKey2, TValue2>(mappedSource, source);
}
public static IQueryableDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IQueryableDictionary<TKey1, TValue1> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableDictionary<TKey1, TValue1, TKey2, TValue2>(source, compareId, configurationProvider, mapper);
    return new QueryableDictionaryAdapter<TKey2, TValue2>(mappedSource, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static IQueryableReadOnlyDictionary<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IQueryableReadOnlyDictionary<TKey1, TValue1> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
cfg.CreateMap<TKey1, TKey2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableReadOnlyDictionary<TKey1, TValue1, TKey2, TValue2>(source, compareId, configurationProvider, mapper);
    return new QueryableReadOnlyDictionaryAdapter<TKey2, TValue2>(mappedSource, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
#endregion

#region WithMapping - one key type
public static ICachedDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this ICachedDictionary<TKey, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source, (key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),
(key, value) => new KeyValue<TKey, TValue1>(key, mapper.Map<TValue2, TValue1>(value)),key => key,key => key);
var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey, TValue2>(mappedSource);
    return new CachedDictionaryAdapter<TKey, TValue2>(mappedSource, cachedMapSource.AsBypassCache, cachedMapSource.AsNeverFlush, () => {  cachedMapSource.FlushCache(); source.FlushCache(); }, cachedMapSource.GetWrites);
}
public static ICachedDisposableDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this ICachedDisposableDictionary<TKey, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source, (key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),
(key, value) => new KeyValue<TKey, TValue1>(key, mapper.Map<TValue2, TValue1>(value)),key => key,key => key);
var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey, TValue2>(mappedSource);
    return new CachedDisposableDictionaryAdapter<TKey, TValue2>(mappedSource, cachedMapSource.AsBypassCache, cachedMapSource.AsNeverFlush, () => {  cachedMapSource.FlushCache(); source.FlushCache(); }, cachedMapSource.GetWrites, source);
}
public static ICachedDisposableQueryableDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this ICachedDisposableQueryableDictionary<TKey, TValue1> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableDictionary<TKey, TValue1, TKey, TValue2>(source, compareId, configurationProvider, mapper);
var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey, TValue2>(mappedSource);
    return new CachedDisposableQueryableDictionaryAdapter<TKey, TValue2>(mappedSource, cachedMapSource.AsBypassCache, cachedMapSource.AsNeverFlush, () => {  cachedMapSource.FlushCache(); source.FlushCache(); }, cachedMapSource.GetWrites, source, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static ICachedQueryableDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this ICachedQueryableDictionary<TKey, TValue1> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableDictionary<TKey, TValue1, TKey, TValue2>(source, compareId, configurationProvider, mapper);
var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey, TValue2>(mappedSource);
    return new CachedQueryableDictionaryAdapter<TKey, TValue2>(mappedSource, cachedMapSource.AsBypassCache, cachedMapSource.AsNeverFlush, () => {  cachedMapSource.FlushCache(); source.FlushCache(); }, cachedMapSource.GetWrites, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static IComposableDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this IComposableDictionary<TKey, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source, (key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),
(key, value) => new KeyValue<TKey, TValue1>(key, mapper.Map<TValue2, TValue1>(value)),key => key,key => key);
    return mappedSource;
}
public static IComposableReadOnlyDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this IComposableReadOnlyDictionary<TKey, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingReadOnlyDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source,
(key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),
key => key,
key => key);
    return mappedSource;
}
public static IDisposableDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this IDisposableDictionary<TKey, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source, (key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),
(key, value) => new KeyValue<TKey, TValue1>(key, mapper.Map<TValue2, TValue1>(value)),key => key,key => key);
    return new DisposableDictionaryAdapter<TKey, TValue2>(mappedSource, source);
}
public static IDisposableQueryableDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this IDisposableQueryableDictionary<TKey, TValue1> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableDictionary<TKey, TValue1, TKey, TValue2>(source, compareId, configurationProvider, mapper);
    return new DisposableQueryableDictionaryAdapter<TKey, TValue2>(mappedSource, source, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static IDisposableQueryableReadOnlyDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this IDisposableQueryableReadOnlyDictionary<TKey, TValue1> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableReadOnlyDictionary<TKey, TValue1, TKey, TValue2>(source, compareId, configurationProvider, mapper);
    return new DisposableQueryableReadOnlyDictionaryAdapter<TKey, TValue2>(mappedSource, source, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static IDisposableReadOnlyDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this IDisposableReadOnlyDictionary<TKey, TValue1> source, IMapper mapper) {
if (mapper == null) {
var configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new MappingReadOnlyDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source,
(key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),
key => key,
key => key);
    return new DisposableReadOnlyDictionaryAdapter<TKey, TValue2>(mappedSource, source);
}
public static IQueryableDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this IQueryableDictionary<TKey, TValue1> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableDictionary<TKey, TValue1, TKey, TValue2>(source, compareId, configurationProvider, mapper);
    return new QueryableDictionaryAdapter<TKey, TValue2>(mappedSource, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
public static IQueryableReadOnlyDictionary<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this IQueryableReadOnlyDictionary<TKey, TValue1> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
if (configurationProvider == null) {
configurationProvider = new MapperConfiguration(cfg => {
cfg.CreateMap<TValue1, TValue2>().ReverseMap();
});
}
if (mapper == null) {
mapper = configurationProvider.CreateMapper();
}
var mappedSource = new AutoMapperQueryableReadOnlyDictionary<TKey, TValue1, TKey, TValue2>(source, compareId, configurationProvider, mapper);
    return new QueryableReadOnlyDictionaryAdapter<TKey, TValue2>(mappedSource, mapper.ProjectTo<TValue2>(source.Values, configurationProvider));
}
#endregion

#region WithMapping - transactional different key types
public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey2, TValue2>, ICachedDisposableDictionary<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey1, TValue1>, ICachedDisposableDictionary<TKey1, TValue1>> source, IMapper mapper) {
return new AnonymousTransactionalCollection<IDisposableReadOnlyDictionary<TKey2, TValue2>, ICachedDisposableDictionary<TKey2, TValue2>>(
() => source.BeginRead().WithMapping<TKey1, TValue1, TKey2, TValue2>(mapper),
() => source.BeginWrite().WithMapping<TKey1, TValue1, TKey2, TValue2>(mapper));
}
public static ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey2, TValue2>, ICachedDisposableQueryableDictionary<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey1, TValue1>, ICachedDisposableQueryableDictionary<TKey1, TValue1>> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
return new AnonymousTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey2, TValue2>, ICachedDisposableQueryableDictionary<TKey2, TValue2>>(
() => source.BeginRead().WithMapping<TKey1, TValue1, TKey2, TValue2>(compareId, configurationProvider, mapper),
() => source.BeginWrite().WithMapping<TKey1, TValue1, TKey2, TValue2>(compareId, configurationProvider, mapper));
}
public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey2, TValue2>, IDisposableDictionary<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey1, TValue1>, IDisposableDictionary<TKey1, TValue1>> source, IMapper mapper) {
return new AnonymousTransactionalCollection<IDisposableReadOnlyDictionary<TKey2, TValue2>, IDisposableDictionary<TKey2, TValue2>>(
() => source.BeginRead().WithMapping<TKey1, TValue1, TKey2, TValue2>(mapper),
() => source.BeginWrite().WithMapping<TKey1, TValue1, TKey2, TValue2>(mapper));
}
public static ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey2, TValue2>, IDisposableQueryableDictionary<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey1, TValue1>, IDisposableQueryableDictionary<TKey1, TValue1>> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
return new AnonymousTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey2, TValue2>, IDisposableQueryableDictionary<TKey2, TValue2>>(
() => source.BeginRead().WithMapping<TKey1, TValue1, TKey2, TValue2>(compareId, configurationProvider, mapper),
() => source.BeginWrite().WithMapping<TKey1, TValue1, TKey2, TValue2>(compareId, configurationProvider, mapper));
}
#endregion
#region WithMapping - transactional same key type
public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue2>, ICachedDisposableDictionary<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue1>, ICachedDisposableDictionary<TKey, TValue1>> source, IMapper mapper) {
return new AnonymousTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue2>, ICachedDisposableDictionary<TKey, TValue2>>(
() => source.BeginRead().WithMapping<TKey, TValue1, TValue2>(mapper),
() => source.BeginWrite().WithMapping<TKey, TValue1, TValue2>(mapper));
}
public static ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue2>, ICachedDisposableQueryableDictionary<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue1>, ICachedDisposableQueryableDictionary<TKey, TValue1>> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
return new AnonymousTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue2>, ICachedDisposableQueryableDictionary<TKey, TValue2>>(
() => source.BeginRead().WithMapping<TKey, TValue1, TValue2>(compareId, configurationProvider, mapper),
() => source.BeginWrite().WithMapping<TKey, TValue1, TValue2>(compareId, configurationProvider, mapper));
}
public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue2>, IDisposableDictionary<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue1>, IDisposableDictionary<TKey, TValue1>> source, IMapper mapper) {
return new AnonymousTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue2>, IDisposableDictionary<TKey, TValue2>>(
() => source.BeginRead().WithMapping<TKey, TValue1, TValue2>(mapper),
() => source.BeginWrite().WithMapping<TKey, TValue1, TValue2>(mapper));
}
public static ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue2>, IDisposableQueryableDictionary<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue1>, IDisposableQueryableDictionary<TKey, TValue1>> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
return new AnonymousTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue2>, IDisposableQueryableDictionary<TKey, TValue2>>(
() => source.BeginRead().WithMapping<TKey, TValue1, TValue2>(compareId, configurationProvider, mapper),
() => source.BeginWrite().WithMapping<TKey, TValue1, TValue2>(compareId, configurationProvider, mapper));
}
#endregion
#region WithMapping - read-only transactional different key types
public static IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey1, TValue1>> source, IMapper mapper) {
return new AnonymousReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey2, TValue2>>(
() => source.BeginRead().WithMapping<TKey1, TValue1, TKey2, TValue2>(mapper));
}
public static IReadOnlyTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IReadOnlyTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey1, TValue1>> source, Func<TKey2, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
return new AnonymousReadOnlyTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey2, TValue2>>(
() => source.BeginRead().WithMapping<TKey1, TValue1, TKey2, TValue2>(compareId, configurationProvider, mapper));
}
#endregion
#region WithMapping - read-only transactional same key type
public static IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue1>> source, IMapper mapper) {
return new AnonymousReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue2>>(
() => source.BeginRead().WithMapping<TKey, TValue1, TValue2>(mapper));
}
public static IReadOnlyTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this IReadOnlyTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue1>> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {
return new AnonymousReadOnlyTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue2>>(
() => source.BeginRead().WithMapping<TKey, TValue1, TValue2>(compareId, configurationProvider, mapper));
}
#endregion
}
}
