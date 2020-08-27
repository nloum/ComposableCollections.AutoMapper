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
    }
}