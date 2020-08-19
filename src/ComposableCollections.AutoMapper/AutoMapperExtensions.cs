using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ComposableCollections.Dictionary;

namespace ComposableCollections
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Creates an IQueryableReadOnlyDictionary that maps from the source's value to the specified value.
        /// This uses the AutoMapper ProjectTo IQueryable extension method.
        /// </summary>
        public static IQueryableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(
            this IQueryableDictionary<TKey, TInnerValue> innerValues, IConfigurationProvider configurationProvider,
            IMapper mapper = null) where TValue : class
        {
            return new AutoMapperQueryableDictionary<TKey, TValue, TInnerValue>(innerValues, configurationProvider, mapper);
        }
        
        /// <summary>
        /// Creates an IQueryableReadOnlyDictionary that maps from the source's value to the specified value.
        /// This uses the AutoMapper ProjectTo IQueryable extension method.
        /// </summary>
        public static IQueryableReadOnlyDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(
            this IQueryableReadOnlyDictionary<TKey, TInnerValue> innerValues, IConfigurationProvider configurationProvider,
            IMapper mapper = null) where TValue : class
        {
            return new AutoMapperQueryableReadOnlyDictionary<TKey, TValue, TInnerValue>(innerValues, configurationProvider, mapper);
        }

        /// <summary>
        /// Tells AutoMapper that when mapping from a T1 to a T2, construct the T2 using a cache
        /// that is shared across multiple calls to Map.
        /// </summary>
        public static IMappingExpression<T1, T2> ConstructUsing<T1, T2>(this IMappingExpression<T1, T2> source,
            PreserveReferencesState preserveReferencesState, Func<T1, T2> constructor)
        {
            preserveReferencesState.Initialize(constructor);
            var cache = preserveReferencesState.GetCache<T1, T2>();
            return source.ConstructUsing(x => cache[x]);
        }

        /// <summary>
        /// Tells AutoMapper that when mapping from a T1 to a T2, construct the T2 using a cache
        /// that is shared across multiple calls to Map.
        /// </summary>
        public static IMappingExpression<T1, T2> ConstructUsing<T1, T2>(this IMappingExpression<T1, T2> source,
            PreserveReferencesState preserveReferencesState) where T2 : new()
        {
            preserveReferencesState.Initialize<T1, T2>();
            var cache = preserveReferencesState.GetCache<T1, T2>();
            return source.ConstructUsing(x => cache[x]);
        }
        
        /// <summary>
        /// Tells AutoMapper that when mapping from a T1 to a T2, construct the T2 using a cache
        /// that is shared across multiple calls to Map.
        /// Assumes that a call to preserveReferencesState.Initialize{T1, T2} has already been made.
        /// </summary>
        public static IMappingExpression<T1, T2> ConstructUsingPreInitialized<T1, T2>(this IMappingExpression<T1, T2> source,
            PreserveReferencesState preserveReferencesState)
        {
            var cache = preserveReferencesState.GetCache<T1, T2>();
            return source.ConstructUsing(x => cache[x]);
        }
        
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
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, PreserveReferencesState preserveReferencesState, Func<TValue, TInnerValue> innerValueFactory, Func<TInnerValue, TValue> valueFactory) where TValue : class
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TValue, TInnerValue>()
                    .ConstructUsing(preserveReferencesState, innerValueFactory)
                    .ReverseMap()
                    .ConstructUsing(preserveReferencesState, valueFactory);
            });
            var mapper = mapperConfig.CreateMapper();

            return new AnonymousMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TValue, TInnerValue>(value), (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, PreserveReferencesState preserveReferencesState, Func<TValue, TInnerValue> innerValueFactory) where TValue : class, new()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TValue, TInnerValue>()
                    .ConstructUsing(preserveReferencesState, innerValueFactory)
                    .ReverseMap()
                    .ConstructUsing(preserveReferencesState);
            });
            var mapper = mapperConfig.CreateMapper();

            return new AnonymousMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TValue, TInnerValue>(value), (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, PreserveReferencesState preserveReferencesState, Func<TInnerValue, TValue> valueFactory) where TValue : class where TInnerValue : new()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TValue, TInnerValue>()
                    .ConstructUsing(preserveReferencesState)
                    .ReverseMap()
                    .ConstructUsing(preserveReferencesState, valueFactory);
            });
            var mapper = mapperConfig.CreateMapper();

            return new AnonymousMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TValue, TInnerValue>(value), (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, PreserveReferencesState preserveReferencesState) where TValue : class, new() where TInnerValue : new()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TValue, TInnerValue>()
                    .ConstructUsing(preserveReferencesState)
                    .ReverseMap()
                    .ConstructUsing(preserveReferencesState);
            });
            var mapper = mapperConfig.CreateMapper();

            return new AnonymousMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TValue, TInnerValue>(value), (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }
    }
}