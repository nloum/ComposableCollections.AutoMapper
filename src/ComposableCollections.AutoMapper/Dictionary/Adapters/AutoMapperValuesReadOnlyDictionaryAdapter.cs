using AutoMapper;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;

namespace ComposableCollections.AutoMapper.Dictionary.Adapters
{
    public class AutoMapperValuesReadOnlyDictionaryAdapter<TKey, TSourceValue, TValue> : MappingValuesReadOnlyDictionaryAdapter<TKey, TSourceValue, TValue>
    {
        public AutoMapperValuesReadOnlyDictionaryAdapter(IComposableReadOnlyDictionary<TKey, TSourceValue> innerValues, IMapper mapper) : base(innerValues, mapper.Map<TSourceValue, TValue>)
        {
        }
    }
}