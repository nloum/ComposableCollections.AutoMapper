using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AutoMapper;
using ComposableCollections.Dictionary;

namespace ComposableCollections
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Creates a facade on top of the specified IComposableDictionary that keeps tracks of changes and occasionally
        /// flushes them to the specified IComposableDictionary.
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, IMapper mapper = null) where TValue : class
        {
            if (mapper == null)
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TValue, TInnerValue>()
                        .PreserveReferences()
                        .ReverseMap();
                });
                mapper = mapperConfig.CreateMapper();
            }

            return new AnonymousMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TValue, TInnerValue>(value), (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }

        /// <summary>
        /// Creates a facade on top of the specified IComposableDictionary that keeps tracks of changes and occasionally
        /// flushes them to the specified IComposableDictionary. Also this caches the converted values.
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithCachedMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, IMapper mapper = null, IComposableDictionary<TKey, TValue> cache = null, bool proactivelyConvertAllValues = false) where TValue : class
        {
            if (mapper == null)
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TValue, TInnerValue>()
                        .PreserveReferences()
                        .ReverseMap();
                });
                mapper = mapperConfig.CreateMapper();
            }

            return new AnonymousCachedMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TValue, TInnerValue>(value), (id, value) => mapper.Map<TInnerValue, TValue>(value), cache, proactivelyConvertAllValues);
        }
        
        /// <summary>
        /// Creates a facade on top of the specified IComposableDictionary that keeps tracks of changes and occasionally
        /// flushes them to the specified IComposableDictionary.
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, Func<TValue, TKey> key, Func<TInnerValue, TKey> innerKey, IMapper mapper = null) where TValue : class
        {
            if (mapper == null)
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TValue, TInnerValue>()
                        .PreserveReferences()
                        .ReverseMap();
                });
                mapper = mapperConfig.CreateMapper();
            }

            return new AnonymousBulkMapDictionary<TKey, TValue, TInnerValue>(source, 
                keyValues => mapper.Map<IEnumerable<TValue>, IEnumerable<TInnerValue>>(keyValues.Select(kvp => kvp.Value)).Select(value => new KeyValue<TKey, TInnerValue>(innerKey(value), value)),
                keyValues => mapper.Map<IEnumerable<TInnerValue>, IEnumerable<TValue>>(keyValues.Select(kvp => kvp.Value)).Select(value => new KeyValue<TKey, TValue>(key(value), value)));
        }

        /// <summary>
        /// Creates a facade on top of the specified IComposableDictionary that keeps tracks of changes and occasionally
        /// flushes them to the specified IComposableDictionary. Also this caches the converted values.
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithCachedMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, Func<TValue, TKey> key, Func<TInnerValue, TKey> innerKey, IMapper mapper = null, IComposableDictionary<TKey, TValue> cache = null, bool proactivelyConvertAllValues = false) where TValue : class
        {
            if (mapper == null)
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TValue, TInnerValue>()
                        .PreserveReferences()
                        .ReverseMap();
                });
                mapper = mapperConfig.CreateMapper();
            }

            return new AnonymousCachedBulkMapDictionary<TKey, TValue, TInnerValue>(source, 
                keyValues => mapper.Map<IEnumerable<TValue>, IEnumerable<TInnerValue>>(keyValues.Select(kvp => kvp.Value)).Select(value => new KeyValue<TKey, TInnerValue>(innerKey(value), value)),
                keyValues => mapper.Map<IEnumerable<TInnerValue>, IEnumerable<TValue>>(keyValues.Select(kvp => kvp.Value)).Select(value => new KeyValue<TKey, TValue>(key(value), value)));
        }
    }
}