﻿using System;
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
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, PreserveReferencesState preserveReferencesState, IMapper mapper = null, bool proactivelyConvertAllValues = false) where TValue : class where TInnerValue : new()
        {
            if (mapper == null)
            {
                var valueCache = preserveReferencesState.GetCache<TInnerValue, TValue>();
                var innerValueCache = preserveReferencesState.GetCache<TValue, TInnerValue>();
                
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TValue, TInnerValue>()
                        .ConstructUsing((aggregateRoot, resolutionContext) =>
                        {
                            if (innerValueCache.TryGetValue(aggregateRoot, out var preExistingValue))
                            {
                                return preExistingValue;
                            }

                            var dbDto = new TInnerValue();
                            innerValueCache[aggregateRoot] = dbDto;
                            return dbDto;
                        })
                        .ReverseMap();
                });
                mapper = mapperConfig.CreateMapper();
            }

            return new AnonymousMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TValue, TInnerValue>(value), (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }
    }
}