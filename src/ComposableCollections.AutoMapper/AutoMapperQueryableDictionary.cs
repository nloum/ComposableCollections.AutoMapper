using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Base;
using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Write;
using SimpleMonads;

namespace ComposableCollections
{
    public class AutoMapperQueryableDictionary<TKey1, TValue1, TKey2, TValue2> : DictionaryBase<TKey2, TValue2>, IQueryableDictionary<TKey2, TValue2>
    {
        private readonly IQueryable<TValue2> _queryable;
        private readonly IQueryableDictionary<TKey1, TValue1> _innerValues;
        private readonly Expression<Func<TValue2, TKey2>> _getKey;
        private readonly IMapper _mapper;
        private readonly Func<TKey2, Expression<Func<TValue2, bool>>> _compareKey;
        private readonly Expression<Func<TValue2, IKeyValue<TKey2, TValue2>>> _getKeyValue;

        public AutoMapperQueryableDictionary(IQueryableDictionary<TKey1, TValue1> innerValues, Expression<Func<TValue2, TKey2>> getKey, IMapper mapper)
        {
            _queryable = mapper.ProjectTo<TValue2>(innerValues.Values);
            _innerValues = innerValues;
            _getKey = getKey;
            _mapper = mapper;

            var memberExpression = getKey.Body as MemberExpression;
            _compareKey = key =>
            {
                var parameter = Expression.Parameter(typeof(TValue2), "p1");
                var equality = Expression.Equal(Expression.MakeMemberAccess(parameter, memberExpression.Member), Expression.Constant(key, typeof(TKey2)));
                var result = Expression.Lambda<Func<TValue2, bool>>(equality, parameter);
                return result;
            };

            var valueParameter = Expression.Parameter(typeof(TValue2), "p1");
            var body = Expression.New(typeof(KeyValue<TKey2, TValue2>).GetConstructor(new[] {typeof(TKey2), typeof(TValue2)}),
                Expression.MakeMemberAccess(valueParameter, memberExpression.Member),
                valueParameter);
            _getKeyValue = Expression.Lambda<Func<TValue2, IKeyValue<TKey2, TValue2>>>(body, valueParameter);
        }

        protected virtual IKeyValue<TKey2, TValue2> Convert(TKey1 key, TValue1 value)
        {
            return new KeyValue<TKey2, TValue2>(_mapper.Map<TKey1, TKey2>(key), _mapper.Map<TValue1, TValue2>(value));
        }

        protected virtual IKeyValue<TKey1, TValue1> Convert(TKey2 key, TValue2 value)
        {
            return new KeyValue<TKey1, TValue1>(_mapper.Map<TKey2, TKey1>(key), _mapper.Map<TValue2, TValue1>(value));
        }

        protected virtual TKey1 ConvertToKey1(TKey2 key)
        {
            return _mapper.Map<TKey2, TKey1>(key);
        }

        protected virtual TKey2 ConvertToKey2(TKey1 key)
        {
            return _mapper.Map<TKey1, TKey2>(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override IEnumerator<IKeyValue<TKey2, TValue2>> GetEnumerator()
        {
            return _queryable.Select(_getKeyValue).AsEnumerable().GetEnumerator();
        }

        public override int Count => _queryable.Count();
        public override IEqualityComparer<TKey2> Comparer => EqualityComparer<TKey2>.Default;

        public override IEnumerable<TKey2> Keys => _queryable.Select(_getKey);

        IEnumerable<TValue2> IComposableReadOnlyDictionary<TKey2, TValue2>.Values => _queryable;
        public override IEnumerable<TValue2> Values => _queryable;

        public override bool ContainsKey(TKey2 key)
        {
            return _queryable.Where(_compareKey(key)).FirstOrDefault() != null;
        }

        public override bool TryGetValue(TKey2 key, out TValue2 value)
        {
            value = _queryable.Where(_compareKey(key)).FirstOrDefault();
            return value != null;
        }

        IQueryable<TValue2> IQueryableReadOnlyDictionary<TKey2, TValue2>.Values => _queryable;
        
        public override void Write(IEnumerable<DictionaryWrite<TKey2, TValue2>> writes,
            out IReadOnlyList<DictionaryWriteResult<TKey2, TValue2>> results)
        {
            _innerValues.Write(writes.Select(write =>
            {
                Func<TValue1> valueIfAdding = () =>
                {
                    var result = write.ValueIfAdding.Value();
                    return Convert(write.Key, result).Value;
                };
                Func<TValue1, TValue1> valueIfUpdating = previousValue =>
                {
                    var result = write.ValueIfAdding.Value();
                    return Convert(write.Key, result).Value;
                };
                return new DictionaryWrite<TKey1, TValue1>(write.Type, ConvertToKey1(write.Key),
                    valueIfAdding.ToMaybe(),
                    valueIfUpdating.ToMaybe());
            }), out var innerResults);

            results = innerResults.Select(innerResult =>
            {
                if (innerResult.Type == DictionaryWriteType.Add || innerResult.Type == DictionaryWriteType.TryAdd)
                {
                    return DictionaryWriteResult<TKey2, TValue2>.CreateAdd(ConvertToKey2(innerResult.Key),
                        innerResult.Add.Value.Added,
                        innerResult.Add.Value.ExistingValue.Select(value => Convert(innerResult.Key, value).Value),
                        innerResult.Add.Value.NewValue.Select(value => Convert(innerResult.Key, value).Value));
                }
                else if (innerResult.Type == DictionaryWriteType.Remove ||
                         innerResult.Type == DictionaryWriteType.TryRemove)
                {
                    return DictionaryWriteResult<TKey2, TValue2>.CreateRemove(ConvertToKey2(innerResult.Key),
                        innerResult.Remove.Value.Select(value => Convert(innerResult.Key, value).Value));
                }
                else if (innerResult.Type == DictionaryWriteType.Update ||
                         innerResult.Type == DictionaryWriteType.TryUpdate)
                {
                    return DictionaryWriteResult<TKey2, TValue2>.CreateUpdate(ConvertToKey2(innerResult.Key),
                        innerResult.Update.Value.Updated,
                        innerResult.Update.Value.ExistingValue.Select(value => Convert(innerResult.Key, value).Value),
                        innerResult.Update.Value.NewValue.Select(value => Convert(innerResult.Key, value).Value));
                }
                else if (innerResult.Type == DictionaryWriteType.AddOrUpdate)
                {
                    return DictionaryWriteResult<TKey2, TValue2>.CreateAddOrUpdate(ConvertToKey2(innerResult.Key),
                        innerResult.AddOrUpdate.Value.Result,
                        innerResult.AddOrUpdate.Value.ExistingValue.Select(value =>
                            Convert(innerResult.Key, value).Value),
                        Convert(innerResult.Key, innerResult.AddOrUpdate.Value.NewValue).Value);
                }
                else
                {
                    throw new InvalidOperationException("Unknown dictionary write type");
                }
            }).ToList();
        }
    }
}