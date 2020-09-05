using System.Collections.Generic;
using System.IO;
using System.Linq;
using IoFluently;
using ReactiveProcesses;

namespace ComposableCollections.AutoMapper.CodeGenerator
{
    class Program
    {
	    private static void GenerateWithMappingExtensionMethods(TextWriter textWriter)
	    {
		    var interfaces = new List<string>
		    {
			    "ICachedDictionary",
			    "ICachedDisposableDictionary",
			    "ICachedDisposableQueryableDictionary",
			    "ICachedQueryableDictionary",
			    "IComposableDictionary",
			    "IComposableReadOnlyDictionary",
			    "IDisposableDictionary",
			    "IDisposableQueryableDictionary",
			    "IDisposableQueryableReadOnlyDictionary",
			    "IDisposableReadOnlyDictionary",
			    "IQueryableDictionary",
			    "IQueryableReadOnlyDictionary"
		    };

		    var interfacesWithBuiltInKeys = interfaces.Select(x => $"{x}WithBuiltInKey").ToList();

		    textWriter.WriteLine("#region WithMapping - different key types");
		    
		    foreach (var iface in interfaces)
		    {
			    if (iface.Contains("Queryable"))
			    {
				    textWriter.WriteLine(
					    $"public static {iface}<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this {iface}<TKey1, TValue1> source, Expression<Func<TValue2, TKey2>> id, IMapper mapper) {{");
			    }
			    else
			    {
				    textWriter.WriteLine(
					    $"public static {iface}<TKey2, TValue2> WithMapping<TKey1, TValue1, TKey2, TValue2>(this {iface}<TKey1, TValue1> source, IMapper mapper) {{");
			    }
			    
			    textWriter.WriteLine("if (mapper == null) {");
			    textWriter.WriteLine("var configurationProvider = new MapperConfiguration(cfg => {");
			    textWriter.WriteLine("cfg.CreateMap<TValue1, TValue2>().ReverseMap();");
			    textWriter.WriteLine("cfg.CreateMap<TKey1, TKey2>().ReverseMap();");
			    textWriter.WriteLine("});");
			    textWriter.WriteLine("mapper = configurationProvider.CreateMapper();");
			    textWriter.WriteLine("}");
			    
			    if (iface.Contains("ReadOnly"))
			    {
				    if (iface.Contains("Queryable"))
				    {
					    textWriter.WriteLine("var mappedSource = new AutoMapperQueryableReadOnlyDictionary<TKey1, TValue1, TKey2, TValue2>(source, id, mapper);");
				    }
				    else
				    {
					    textWriter.WriteLine("var mappedSource = new MappingReadOnlyDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>(source,\n" +
					                         "(key, value) => new KeyValue<TKey2, TValue2>(mapper.Map<TKey1, TKey2>(key), mapper.Map<TValue1, TValue2>(value)),\n" +
					                         "key => mapper.Map<TKey1, TKey2>(key),\n" +
					                         "key => mapper.Map<TKey2, TKey1>(key));");
				    }
			    }
			    else
			    {
				    if (iface.Contains("Queryable"))
				    {
					    textWriter.WriteLine(
						    "var mappedSource = new AutoMapperQueryableDictionary<TKey1, TValue1, TKey2, TValue2>(source, id, mapper);");
				    } else {
					    textWriter.WriteLine(
						    "var mappedSource = new MappingDictionaryAdapter<TKey1, TValue1, TKey2, TValue2>(source, (key, value) => new KeyValue<TKey2, TValue2>(mapper.Map<TKey1, TKey2>(key), mapper.Map<TValue1, TValue2>(value)),\n" +
						    "(key, value) => new KeyValue<TKey1, TValue1>(mapper.Map<TKey2, TKey1>(key), mapper.Map<TValue2, TValue1>(value))," +
						    "key => mapper.Map<TKey1, TKey2>(key)," +
						    "key => mapper.Map<TKey2, TKey1>(key));");
				    }
			    }

			    if (iface.Contains("Cached"))
			    {
				    textWriter.WriteLine("var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey2, TValue2>(mappedSource);");
			    }
			    var arguments = new List<string>();

			    arguments.Add("mappedSource");
			    if (iface.Contains("Cached"))
			    {
				    arguments.Add("cachedMapSource.AsBypassCache");
				    arguments.Add("cachedMapSource.AsNeverFlush");
				    arguments.Add("() => {  cachedMapSource.FlushCache(); source.FlushCache(); }");
				    arguments.Add("cachedMapSource.GetWrites");
			    }

			    if (iface.Contains("Disposable"))
			    {
				    arguments.Add("source");
			    }
			    if (iface.Contains("Queryable"))
			    {
				    arguments.Add("mapper.ProjectTo<TValue2>(source.Values)");
			    }

			    if (iface.Contains("Composable"))
			    {
				    textWriter.WriteLine($"    return mappedSource;");
			    }
			    else
			    {
				    textWriter.WriteLine($"    return new {iface.Substring(1)}Adapter<TKey2, TValue2>({string.Join(", ", arguments)});");
			    }

			    textWriter.WriteLine("}");
		    }

 		    textWriter.WriteLine("#endregion\n");
		    textWriter.WriteLine("#region WithMapping - one key type");
		    
		    foreach (var iface in interfaces)
		    {
			    if (iface.Contains("Queryable"))
			    {
				    textWriter.WriteLine(
					    $"public static {iface}<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this {iface}<TKey, TValue1> source, Expression<Func<TValue2, TKey>> id, IMapper mapper) {{");
			    }
			    else
			    {
				    textWriter.WriteLine(
					    $"public static {iface}<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this {iface}<TKey, TValue1> source, IMapper mapper) {{");
			    }
			    
			    textWriter.WriteLine("if (mapper == null) {");
			    textWriter.WriteLine("var configurationProvider = new MapperConfiguration(cfg => {");
			    textWriter.WriteLine("cfg.CreateMap<TValue1, TValue2>().ReverseMap();");
			    textWriter.WriteLine("});");
			    textWriter.WriteLine("mapper = configurationProvider.CreateMapper();");
			    textWriter.WriteLine("}");
			    
			    if (iface.Contains("ReadOnly"))
			    {
				    if (iface.Contains("Queryable"))
				    {
					    textWriter.WriteLine("var mappedSource = new AutoMapperQueryableReadOnlyDictionary<TKey, TValue1, TKey, TValue2>(source, id, mapper);");
				    }
				    else
				    {
					    textWriter.WriteLine("var mappedSource = new MappingReadOnlyDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source,\n" +
					                         "(key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),\n" +
					                         "key => key,\n" +
					                         "key => key);");
				    }
			    }
			    else
			    {
				    if (iface.Contains("Queryable"))
				    {
					    textWriter.WriteLine(
						    "var mappedSource = new AutoMapperQueryableDictionary<TKey, TValue1, TKey, TValue2>(source, id, mapper);");
				    } else {
					    textWriter.WriteLine(
						    "var mappedSource = new MappingDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source, (key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),\n" +
						    "(key, value) => new KeyValue<TKey, TValue1>(key, mapper.Map<TValue2, TValue1>(value))," +
						    "key => key," +
						    "key => key);");
				    }
			    }

			    if (iface.Contains("Cached"))
			    {
				    textWriter.WriteLine("var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey, TValue2>(mappedSource);");
			    }
			    var arguments = new List<string>();

			    arguments.Add("mappedSource");
			    if (iface.Contains("Cached"))
			    {
				    arguments.Add("cachedMapSource.AsBypassCache");
				    arguments.Add("cachedMapSource.AsNeverFlush");
				    arguments.Add("() => {  cachedMapSource.FlushCache(); source.FlushCache(); }");
				    arguments.Add("cachedMapSource.GetWrites");
			    }

			    if (iface.Contains("Disposable"))
			    {
				    arguments.Add("source");
			    }
			    if (iface.Contains("Queryable"))
			    {
				    arguments.Add("mapper.ProjectTo<TValue2>(source.Values)");
			    }

			    if (iface.Contains("Composable"))
			    {
				    textWriter.WriteLine($"    return mappedSource;");
			    }
			    else
			    {
				    textWriter.WriteLine($"    return new {iface.Substring(1)}Adapter<TKey, TValue2>({string.Join(", ", arguments)});");
			    }

			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion\n");

		    textWriter.WriteLine("#region WithMapping - transactional different key types");
		    
		    foreach (var iface in interfaces)
		    {
			    if (!iface.Contains("Disposable") || iface.Contains("ReadOnly"))
			    {
				    continue;
			    }
			    
			    var parameters = new List<string>();
			    var readOnlyArguments = new List<string>();
			    var readWriteArguments = new List<string>();
			    if (iface.Contains("Queryable"))
			    {
				    parameters.Add("Expression<Func<TValue2, TKey2>> id");
				    readOnlyArguments.Add("id");
				    readWriteArguments.Add("id");
			    }
				parameters.Add("IMapper mapper");
			    readOnlyArguments.Add("mapper");
			    readWriteArguments.Add("mapper");

			    var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");
			    
			    textWriter.WriteLine(
				    $"public static IReadWriteFactory<{readOnlyInterface}<TKey2, TValue2>, {iface}<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IReadWriteFactory<{readOnlyInterface}<TKey1, TValue1>, {iface}<TKey1, TValue1>> source, {string.Join(", ", parameters)}) {{");

			    textWriter.WriteLine(
				    $"return new AnonymousReadWriteFactory<{readOnlyInterface}<TKey2, TValue2>, {iface}<TKey2, TValue2>>(");
			    textWriter.WriteLine(
				    $"() => source.CreateReader().WithMapping<TKey1, TValue1, TKey2, TValue2>({string.Join(", ", readOnlyArguments)}),");
			    textWriter.WriteLine(
				    $"() => source.CreateWriter().WithMapping<TKey1, TValue1, TKey2, TValue2>({string.Join(", ", readWriteArguments)}));");
			    
			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion");
		    
		    textWriter.WriteLine("#region WithMapping - transactional same key type");
		    
		    foreach (var iface in interfaces)
		    {
			    if (!iface.Contains("Disposable") || iface.Contains("ReadOnly"))
			    {
				    continue;
			    }
			    
			    var parameters = new List<string>();
			    var readOnlyArguments = new List<string>();
			    var readWriteArguments = new List<string>();
			    if (iface.Contains("Queryable"))
			    {
				    parameters.Add("Expression<Func<TValue2, TKey>> id");
				    readOnlyArguments.Add("id");
				    readWriteArguments.Add("id");
			    }
			    parameters.Add("IMapper mapper");
			    readOnlyArguments.Add("mapper");
			    readWriteArguments.Add("mapper");

			    var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");

			    textWriter.WriteLine(
				    $"public static IReadWriteFactory<{readOnlyInterface}<TKey, TValue2>, {iface}<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this IReadWriteFactory<{readOnlyInterface}<TKey, TValue1>, {iface}<TKey, TValue1>> source, {string.Join(", ", parameters)}) {{");

			    textWriter.WriteLine(
				    $"return new AnonymousReadWriteFactory<{readOnlyInterface}<TKey, TValue2>, {iface}<TKey, TValue2>>(");
			    textWriter.WriteLine(
				    $"() => source.CreateReader().WithMapping<TKey, TValue1, TValue2>({string.Join(", ", readOnlyArguments)}),");
			    textWriter.WriteLine(
				    $"() => source.CreateWriter().WithMapping<TKey, TValue1, TValue2>({string.Join(", ", readWriteArguments)}));");
			    
			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion");
		    
		    textWriter.WriteLine("#region WithMapping - read-only transactional different key types");
		    
		    foreach (var iface in interfaces)
		    {
			    if (!iface.Contains("Disposable") || iface.Contains("ReadOnly") || iface.Contains("Cached"))
			    {
				    continue;
			    }
			    
			    var parameters = new List<string>();
			    var readOnlyArguments = new List<string>();
			    var readWriteArguments = new List<string>();
			    if (iface.Contains("Queryable"))
			    {
				    parameters.Add("Expression<Func<TValue2, TKey2>> id");
				    readOnlyArguments.Add("id");
				    readWriteArguments.Add("id");
			    }
			    parameters.Add("IMapper mapper");
			    readOnlyArguments.Add("mapper");
			    readWriteArguments.Add("mapper");

			    var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");
			    
			    textWriter.WriteLine(
				    $"public static IReadOnlyFactory<{readOnlyInterface}<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IReadOnlyFactory<{readOnlyInterface}<TKey1, TValue1>> source, {string.Join(", ", parameters)}) {{");

			    textWriter.WriteLine(
				    $"return new AnonymousReadOnlyFactory<{readOnlyInterface}<TKey2, TValue2>>(");
			    textWriter.WriteLine(
				    $"() => source.CreateReader().WithMapping<TKey1, TValue1, TKey2, TValue2>({string.Join(", ", readOnlyArguments)}));");
			    
			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion");
		    
		    textWriter.WriteLine("#region WithMapping - read-only transactional same key type");
		    
		    foreach (var iface in interfaces)
		    {
			    if (!iface.Contains("Disposable") || iface.Contains("ReadOnly") || iface.Contains("Cached"))
			    {
				    continue;
			    }
			    
			    var parameters = new List<string>();
			    var readOnlyArguments = new List<string>();
			    var readWriteArguments = new List<string>();
			    if (iface.Contains("Queryable"))
			    {
				    parameters.Add("Expression<Func<TValue2, TKey>> id");
				    readOnlyArguments.Add("id");
				    readWriteArguments.Add("id");
			    }
			    parameters.Add("IMapper mapper");
			    readOnlyArguments.Add("mapper");
			    readWriteArguments.Add("mapper");

			    var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");

			    textWriter.WriteLine(
				    $"public static IReadOnlyFactory<{readOnlyInterface}<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this IReadOnlyFactory<{readOnlyInterface}<TKey, TValue1>> source, {string.Join(", ", parameters)}) {{");

			    textWriter.WriteLine(
				    $"return new AnonymousReadOnlyFactory<{readOnlyInterface}<TKey, TValue2>>(");
			    textWriter.WriteLine(
				    $"() => source.CreateReader().WithMapping<TKey, TValue1, TValue2>({string.Join(", ", readOnlyArguments)}));");
			    
			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion");
	    }
	    
	    static void Main(string[] args)
	    {
		    var ioService = new IoService(new ReactiveProcessFactory());
		    var repoRoot = ioService.CurrentDirectory.Ancestors().First(ancestor => (ancestor / ".git").IsFolder());

		    var dictionaryExtensionsFilePath = repoRoot / "src" / "ComposableCollections.AutoMapper" / "AutoMapperExtensions.g.cs";
		    using (var streamWriter = dictionaryExtensionsFilePath.OpenWriter())
		    {
			    streamWriter.WriteLine(@"using System;
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
        {");
			    GenerateWithMappingExtensionMethods(streamWriter);
			    streamWriter.WriteLine("}\n}");
		    }
	    }
    }
}