using System;
using System.Linq.Expressions;
using AutoMapper;
using ComposableCollections.Dictionary;

namespace ComposableCollections
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Tells AutoMapper that when mapping from a T1 to a T2, construct the T2 using a cache
        /// that is shared across multiple calls to Map.
        /// </summary>
        public static IMappingExpression<T1, T2> ConstructUsing<T1, T2>(this IMappingExpression<T1, T2> source,
            IPreserveReferencesState preserveReferencesState, Func<T1, T2> constructor)
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
            IPreserveReferencesState preserveReferencesState) where T2 : new()
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
            IPreserveReferencesState preserveReferencesState)
        {
            var cache = preserveReferencesState.GetCache<T1, T2>();
            return source.ConstructUsing(x => cache[x]);
        }

        /// <summary>
        /// Creates an IQueryableReadOnlyDictionary that maps from the source's value to the specified value.
        /// This uses the AutoMapper ProjectTo IQueryable extension method.
        /// </summary>
        public static IQueryableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(
            this IQueryableDictionary<TKey, TInnerValue> source, Func<TKey, Expression<Func<TValue, bool>>> compareId,
            IConfigurationProvider configurationProvider, IMapper mapper = null)
        {
            return new AutoMapperQueryableDictionary<TKey, TValue, TInnerValue>(source, compareId, configurationProvider, mapper);
        }
        
        /// <summary>
        /// Creates an IQueryableReadOnlyDictionary that maps from the source's value to the specified value.
        /// This uses the AutoMapper ProjectTo IQueryable extension method.
        /// </summary>
        public static ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue>, IDisposableQueryableDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(
            this ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TInnerValue>, IDisposableQueryableDictionary<TKey, TInnerValue>> source, Func<TKey, Expression<Func<TValue, bool>>> compareId, IConfigurationProvider configurationProvider,
            IMapper mapper = null)
        {
            return source.Select(x => new DisposableQueryableReadOnlyDictionaryDecorator<TKey, TValue>(x.WithMapping<TKey, TValue, TInnerValue>(configurationProvider, mapper), x),
                x => new DisposableQueryableDictionaryDecorator<TKey, TValue>(x.WithMapping<TKey, TValue, TInnerValue>(compareId, configurationProvider, mapper), x));
        }
        
        /// <summary>
        /// Creates an IQueryableReadOnlyDictionary that maps from the source's value to the specified value.
        /// This uses the AutoMapper ProjectTo IQueryable extension method.
        /// </summary>
        public static IQueryableReadOnlyDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(
            this IQueryableReadOnlyDictionary<TKey, TInnerValue> source, IConfigurationProvider configurationProvider,
            IMapper mapper = null)
        {
            return new AutoMapperQueryableReadOnlyDictionary<TKey, TValue, TInnerValue>(source, configurationProvider, mapper);
        }

        /// <summary>
        /// Creates an IQueryableReadOnlyDictionary that maps from the source's value to the specified value.
        /// This uses the AutoMapper ProjectTo IQueryable extension method.
        /// </summary>
        public static IReadOnlyTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(
            this IReadOnlyTransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TInnerValue>> source, IConfigurationProvider configurationProvider,
            IMapper mapper = null)
        {
            return source.Select(x => new DisposableQueryableReadOnlyDictionaryDecorator<TKey, TValue>(x.WithMapping<TKey, TValue, TInnerValue>(configurationProvider, mapper), x));
        }

        /// <summary>
        /// Creates a facade on top of the specified IComposableDictionary that keeps tracks of changes and occasionally
        /// flushes them to the specified IComposableDictionary.
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, IMapper mapper = null)
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
        /// flushes them to the specified IComposableDictionary.
        /// </summary>
        public static IComposableReadOnlyDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableReadOnlyDictionary<TKey, TInnerValue> source, IMapper mapper = null)
        {
            if (mapper == null)
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TInnerValue, TValue>()
                        .PreserveReferences();
                });
                mapper = mapperConfig.CreateMapper();
            }

            return new AnonymousReadOnlyMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }

        /// <summary>
        /// Creates a facade on top of the specified IComposableDictionary that keeps tracks of changes and occasionally
        /// flushes them to the specified IComposableDictionary.
        /// </summary>
        public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(
            this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TInnerValue>, IDisposableDictionary<TKey, TInnerValue>> source,
            IMapper mapper = null)
        {
            return source.Select(x =>
            {
                return new DisposableReadOnlyDictionaryDecorator<TKey, TValue>(
                    x.WithMapping<TKey, TValue, TInnerValue>(mapper), x);
            }, x =>
            {
                return new DisposableDictionaryDecorator<TKey, TValue>(
                    x.WithMapping<TKey, TValue, TInnerValue>(mapper), x);
            });
        }

        /// <summary>
        /// Creates a facade on top of the specified IComposableDictionary that keeps tracks of changes and occasionally
        /// flushes them to the specified IComposableDictionary.
        /// </summary>
        public static IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(
            this IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TInnerValue>> source,
            IMapper mapper = null)
        {
            return source.Select(x => new DisposableReadOnlyDictionaryDecorator<TKey, TValue>(
                x.WithMapping<TKey, TValue, TInnerValue>(mapper), x));
        }
        
        #region Extension methods that use IPreserveReferencesState and deal with single dictionaries
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping.
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, IPreserveReferencesState preserveReferencesState, Func<TValue, TInnerValue> innerValueFactory, Func<TInnerValue, TValue> valueFactory)
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
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, IPreserveReferencesState preserveReferencesState, Func<TValue, TInnerValue> innerValueFactory) where TValue : new()
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
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, IPreserveReferencesState preserveReferencesState, Func<TInnerValue, TValue> valueFactory) where TInnerValue : new()
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
        public static IComposableDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableDictionary<TKey, TInnerValue> source, IPreserveReferencesState preserveReferencesState) where TValue : new() where TInnerValue : new()
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
        
        #endregion
        
        #region Read-only mapper extension methods that use IPreserveReferencesState and deal with single dictionaries
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping.
        /// </summary>
        public static IComposableReadOnlyDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableReadOnlyDictionary<TKey, TInnerValue> source, IPreserveReferencesState preserveReferencesState) where TValue : new()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TInnerValue, TValue>()
                    .ConstructUsing(preserveReferencesState, _ => new TValue());
            });
            var mapper = mapperConfig.CreateMapper();

            return new AnonymousReadOnlyMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static IComposableReadOnlyDictionary<TKey, TValue> WithMapping<TKey, TValue, TInnerValue>(this IComposableReadOnlyDictionary<TKey, TInnerValue> source, IPreserveReferencesState preserveReferencesState, Func<TInnerValue, TValue> valueFactory)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TInnerValue, TValue>()
                    .ConstructUsing(preserveReferencesState, valueFactory);
            });
            var mapper = mapperConfig.CreateMapper();

            return new AnonymousReadOnlyMapDictionary<TKey, TValue, TInnerValue>(source, (id, value) => mapper.Map<TInnerValue, TValue>(value));
        }
        
        #endregion
        
        #region Transaction extension methods that use IPreserveReferencesState and deal with single dictionaries
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping.
        /// </summary>
        public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TInnerValue>, IDisposableDictionary<TKey, TInnerValue>> source, IPreserveReferencesState preserveReferencesState, Func<TValue, TInnerValue> innerValueFactory, Func<TInnerValue, TValue> valueFactory)
        {
            return source.Select(x =>
                new DisposableReadOnlyDictionaryDecorator<TKey, TValue>(
                    x.WithMapping(preserveReferencesState,
                        valueFactory), x), x =>
                    new DisposableDictionaryDecorator<TKey, TValue>(
                        x.WithMapping<TKey, TValue, TInnerValue>(preserveReferencesState, innerValueFactory,
                            valueFactory), x));
        }
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TInnerValue>, IDisposableDictionary<TKey, TInnerValue>> source, IPreserveReferencesState preserveReferencesState, Func<TValue, TInnerValue> innerValueFactory) where TValue : new()
        {
            return source.WithMapping(preserveReferencesState, innerValueFactory,
                _ => new TValue());
        }
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TInnerValue>, IDisposableDictionary<TKey, TInnerValue>> source, IPreserveReferencesState preserveReferencesState, Func<TInnerValue, TValue> valueFactory) where TInnerValue : new()
        {
            return source.WithMapping(preserveReferencesState, _ => new TInnerValue(),
                valueFactory);
        }
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TInnerValue>, IDisposableDictionary<TKey, TInnerValue>> source, IPreserveReferencesState preserveReferencesState) where TValue : new() where TInnerValue : new()
        {
            return source.WithMapping(preserveReferencesState, _ => new TInnerValue(),
                _ => new TValue());
        }
        
        #endregion
        
        #region Transactional read-only mapper extension methods that use IPreserveReferencesState and deal with single dictionaries
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping.
        /// </summary>
        public static IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(this IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TInnerValue>> source, IPreserveReferencesState preserveReferencesState) where TValue : new()
        {
            return source.WithMapping<TKey, TValue, TInnerValue>(preserveReferencesState, _ => new TValue());
        }
        
        /// <summary>
        /// A facade that converts a dictionary with one type of key and value to the same type of key but a different value.
        /// This particular extension method also avoids recreating values using a cache that can be shared between multiple
        /// calls to WithMapping. 
        /// </summary>
        public static IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>> WithMapping<TKey, TValue, TInnerValue>(this IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TInnerValue>> source, IPreserveReferencesState preserveReferencesState, Func<TInnerValue, TValue> valueFactory)
        {
            return source.Select(values =>
                new DisposableReadOnlyDictionaryDecorator<TKey, TValue>(values.WithMapping<TKey, TValue, TInnerValue>(preserveReferencesState, valueFactory),
                    values));
        }
        
        #endregion
    }
}