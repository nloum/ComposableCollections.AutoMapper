using AutoMapper;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Sources;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComposableCollections.AutoMapper.Tests
{
    [TestClass]
    public class AutoMapperExtensionsTests
    {
        public class Type1
        {
            public string Name { get; set; }
        }

        public class Type2
        {
            public string Name { get; set; }
        }

        [TestMethod]
        public void AutoMapperQueryableDictionaryShouldAccessIndividualItemsCorrectly()
        {
            
        }
        
        [TestMethod]
        public void ShouldShareReferencesAcrossMultipleMappings()
        {
            var cache = new PreserveReferencesState();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Type1, Type2>()
                    .ConstructUsing(cache)
                    .ReverseMap()
                    .ConstructUsing(cache);
            });
            var mapper = mapperConfig.CreateMapper();
            
            var backend1 = new ComposableDictionary<string, Type1>();
            var backend2 = new ComposableDictionary<string, Type1>();
            var item1 = new Type1() { Name = "item1" };
            backend1.Add("item1", item1);
            backend2.Add("item1", item1);
            
            var frontend1 = backend1.WithAutoMapping<string, Type1, Type2>(mapper);
            var frontend2 = backend2.WithAutoMapping<string, Type1, Type2>(mapper);

            frontend1.TryGetValue("item1", out var item1_frontend1).Should().BeTrue();
            item1_frontend1.Name.Should().Be("item1");
            frontend1.ContainsKey("item1").Should().BeTrue();

            frontend2.TryGetValue("item1", out var item1_frontend2).Should().BeTrue();
            item1_frontend2.Name.Should().Be("item1");
            frontend2.ContainsKey("item1").Should().BeTrue();

            ReferenceEquals(item1_frontend1, item1_frontend2).Should().BeTrue();
        }
        
        [TestMethod]
        public void ShouldShareInnerReferencesAcrossMultipleMappings()
        {
            var cache = new PreserveReferencesState();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Type1, Type2>()
                    .ConstructUsing(cache)
                    .ReverseMap()
                    .ConstructUsing(cache);
            });
            var mapper = mapperConfig.CreateMapper();

            cache.Initialize<Type1, Type2>();
            cache.Initialize<Type2, Type1>();
            
            var backend1 = new ComposableDictionary<string, Type1>();
            var backend2 = new ComposableDictionary<string, Type1>();
            
            var frontend1 = backend1.WithAutoMapping<string, Type1, Type2>(mapper);
            var frontend2 = backend2.WithAutoMapping<string, Type1, Type2>(mapper);

            var item1 = new Type2() { Name = "item1" };
            frontend1.Add("item1", item1);
            frontend2.Add("item1", item1);

            backend1.TryGetValue("item1", out var item1_backend1).Should().BeTrue();
            item1_backend1.Name.Should().Be("item1");
            backend1.ContainsKey("item1").Should().BeTrue();

            backend2.TryGetValue("item1", out var item1_backend2).Should().BeTrue();
            item1_backend2.Name.Should().Be("item1");
            backend2.ContainsKey("item1").Should().BeTrue();

            ReferenceEquals(item1_backend1, item1_backend2).Should().BeTrue();
        }
    }
}
