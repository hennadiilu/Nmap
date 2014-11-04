using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nmap.Internal
{
	internal sealed class MapCompiler
	{
		#region Fields

		private readonly ConverterCollection converters;

		private readonly ObjectFactory objectFactory;

		private readonly IDictionary<PropertyInfo, Func<object, object>> getters;

		private readonly IDictionary<PropertyInfo, Action<object, object>> setters;

		private readonly IDictionary<Type, Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>> rootCompilers;

		private readonly IDictionary<PropertyInfo, Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>> nodeCompilers;

		#endregion

		#region Constructors

		public MapCompiler(ConverterCollection converters, ObjectFactory objectFactory)
		{
			this.converters = converters;
			this.objectFactory = objectFactory;
			getters = new Dictionary<PropertyInfo, Func<object, object>>();
			setters = new Dictionary<PropertyInfo, Action<object, object>>();
			rootCompilers = new Dictionary<Type, Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>>();
			nodeCompilers = new Dictionary<PropertyInfo, Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>>();
			rootCompilers.Add(typeof(TypeMap), new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(TypeMap));
			rootCompilers.Add(typeof(ReversiveTypeMap), new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(ReversiveTypeMap));
			nodeCompilers.Add(ReflectionHelper.GetMemberInfo<TypeMap, Action<object, object, TypeMappingContext>>((TypeMap o) => o.Mapper) as PropertyInfo, new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(MapWithMapper));
			nodeCompilers.Add(ReflectionHelper.GetMemberInfo<TypeMap, ICollection<PropertyMap>>((TypeMap o) => o.PropertyMaps) as PropertyInfo, new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(TypeMapWithPropertyMaps));
			nodeCompilers.Add(ReflectionHelper.GetMemberInfo<PropertyMap, Action<object, object, TypeMappingContext>>((PropertyMap o) => o.Mapper) as PropertyInfo, new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(MapWithMapper));
			nodeCompilers.Add(ReflectionHelper.GetMemberInfo<PropertyMap, ICollection<TypeMap>>((PropertyMap o) => o.InheritanceMaps) as PropertyInfo, new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(PropertyMapWithInheritanceMaps));
			nodeCompilers.Add(ReflectionHelper.GetMemberInfo<ReversiveTypeMap, Action<object, object, TypeMappingContext>>((ReversiveTypeMap o) => o.Mapper) as PropertyInfo, new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(ReversiveTypeMapWithMapper));
			nodeCompilers.Add(ReflectionHelper.GetMemberInfo<ReversiveTypeMap, ICollection<ReversivePropertyMap>>((ReversiveTypeMap o) => o.PropertyMaps) as PropertyInfo, new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(ReversiveTypeMapWithPropertyMaps));
			nodeCompilers.Add(ReflectionHelper.GetMemberInfo<ReversivePropertyMap, Action<object, object, TypeMappingContext>>((ReversivePropertyMap o) => o.Mapper) as PropertyInfo, new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(ReversivePropertyMapWithMapper));
			nodeCompilers.Add(ReflectionHelper.GetMemberInfo<ReversivePropertyMap, ICollection<ReversiveTypeMap>>((ReversivePropertyMap o) => o.InheritanceMaps) as PropertyInfo, new Func<MapCompiler.CompilationContext, MapCompiler.CompilationResult>(ReversivePropertyMapWithInheritanceMaps));
		}

		#endregion

		#region Public Methods

		public MapCompiler.CompilationResult Compile(TypeMapBase map, string contextualName)
		{
			if (!rootCompilers.ContainsKey(map.GetType()))
			{
				Error.MapValidationException_TypeMapIsNotSupported(map);
			}

			var arg = new MapCompiler.CompilationContext
			{
				Map = map,
				CurrentNode = map,
				ParentNode = null,
				ContextualName = contextualName
			};

			return rootCompilers[map.GetType()](arg);
		}

		#endregion

		#region Private Methods

		private MapCompiler.CompilationResult TypeMap(MapCompiler.CompilationContext context)
		{
			var compilationResult = FirstNode(context);

			if (context.Map.Mapper != null)
			{
				return compilationResult;
			}
			else
			{
				TypeMap typeMap = context.Map as TypeMap;
				var list = new List<Action<object, object, TypeMappingContext>>();

				var properties = typeMap.SourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

				for (int i = 0; i < properties.Length; i++)
				{
					var sourcePropertyInfo = properties[i];

					if (ReflectionHelper.IsSimple(sourcePropertyInfo.PropertyType)
						|| ReflectionHelper.IsSimpleEnumerable(sourcePropertyInfo.PropertyType))
					{
						if (!typeMap.PropertyMaps.Any((PropertyMap pm) => pm.SourcePropertyInfo == sourcePropertyInfo))
						{
							PropertyInfo propertyInfo = FindDestintationProperty(typeMap.DestinationType,
								sourcePropertyInfo, context.ContextualName);

							if (!(propertyInfo == null))
							{
								var getter = GetGetter(typeMap.SourceType, sourcePropertyInfo);
								var setter = GetSetter(typeMap.DestinationType, propertyInfo);

								if (ReflectionHelper.IsSimple(sourcePropertyInfo.PropertyType)
									&& ReflectionHelper.IsSimple(propertyInfo.PropertyType))
								{
									list.Add(Mappers.GetSimplePropertiesMapper(
										sourcePropertyInfo, propertyInfo, converters, getter, setter));
								}
								else
								{
									if (ReflectionHelper.IsSimpleEnumerable(sourcePropertyInfo.PropertyType)
										&& ReflectionHelper.IsSimpleEnumerable(propertyInfo.PropertyType))
									{
										list.Add(Mappers.GetSimpleEnumerablePropertiesMapper(
											sourcePropertyInfo, propertyInfo, objectFactory, converters, getter, setter));
									}
								}
							}
						}
					}
				}

				compilationResult = compilationResult ?? new MapCompiler.CompilationResult();

				var mapper = compilationResult.Mapper;

				compilationResult.Mapper = Mappers.GetMapperWithSimplePropertyMappers(mapper, list);

				return compilationResult;
			}
		}

		private MapCompiler.CompilationResult MapWithMapper(MapCompiler.CompilationContext context)
		{
			var mapper = context.CurrentNode as Action<object, object, TypeMappingContext>;

			return new MapCompiler.CompilationResult
			{
				Mapper = mapper
			};
		}

		private MapCompiler.CompilationResult TypeMapWithPropertyMaps(MapCompiler.CompilationContext context)
		{
			var enumerable = context.CurrentNode as IEnumerable<PropertyMapBase>;

			if (enumerable.Count<PropertyMapBase>() == 0)
			{
				return null;
			}
			else
			{
				var list = new List<Action<object, object, TypeMappingContext>>();

				foreach (PropertyMapBase current in enumerable)
				{
					var compilationResult = FirstNode(new MapCompiler.CompilationContext
					{
						Map = context.Map,
						ContextualName = context.ContextualName,
						ParentNode = enumerable,
						CurrentNode = current
					});
					list.Add(compilationResult.Mapper);
				}

				return new MapCompiler.CompilationResult
				{
					Mapper = Mappers.GetMapperWithPropertyMappers(list)
				};
			}
		}

		private MapCompiler.CompilationResult PropertyMapWithInheritanceMaps(MapCompiler.CompilationContext context)
		{
			var enumerable = context.CurrentNode as IEnumerable<TypeMapBase>;

			if (enumerable == null || enumerable.Count<TypeMapBase>() == 0)
			{
				return null;
			}
			else
			{
				var propertyMapBase = context.ParentNode as PropertyMapBase;
				var mapperCollection = new MapperCollection();
				string text = context.ContextualName;

				if (propertyMapBase.DestinationPropertyInfo == null)
				{
					text += propertyMapBase.SourcePropertyInfo.Name;
				}

				foreach (TypeMapBase current in enumerable)
				{
					mapperCollection.Add(current.SourceType, current.DestinationType, Compile(current, text).Mapper);
				}

				var compilationResult = new MapCompiler.CompilationResult();
				var getter = GetGetter(context.Map.SourceType, propertyMapBase.SourcePropertyInfo);

				if (propertyMapBase.DestinationPropertyInfo == null)
				{
					compilationResult.Mapper = Mappers.GetMapperToRootType(getter, mapperCollection);

					return compilationResult;
				}
				else
				{
					var setter = GetSetter(context.Map.DestinationType, propertyMapBase.DestinationPropertyInfo);

					if (ReflectionHelper.IsComplex(propertyMapBase.SourcePropertyInfo.PropertyType)
						&& ReflectionHelper.IsComplex(propertyMapBase.DestinationPropertyInfo.PropertyType))
					{
						compilationResult.Mapper = Mappers.GetComplexPropertiesMapper(getter, setter, mapperCollection, objectFactory);

						return compilationResult;
					}
					else
					{
						if (ReflectionHelper.IsComplexEnumerable(propertyMapBase.SourcePropertyInfo.PropertyType)
							&& ReflectionHelper.IsComplexEnumerable(propertyMapBase.DestinationPropertyInfo.PropertyType))
						{
							compilationResult.Mapper = Mappers.GetComplexEnumerablePropertiesMapper(propertyMapBase
								.DestinationPropertyInfo.PropertyType, getter, setter, objectFactory, mapperCollection);

							return compilationResult;
						}

						return null;
					}
				}
			}
		}

		private MapCompiler.CompilationResult ReversiveTypeMap(MapCompiler.CompilationContext context)
		{
			var compilationResult = FirstNode(context);
			var reversiveTypeMap = context.Map as ReversiveTypeMap;

			if (reversiveTypeMap.Mapper != null && reversiveTypeMap.UnMapper != null)
			{
				return compilationResult;
			}
			else
			{
				var mappers = new List<Action<object, object, TypeMappingContext>>();
				var unmappers = new List<Action<object, object, TypeMappingContext>>();
				var properties = reversiveTypeMap.SourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

				for (int i = 0; i < properties.Length; i++)
				{
					var sourcePropertyInfo = properties[i];

					if (ReflectionHelper.IsSimple(sourcePropertyInfo.PropertyType)
						|| ReflectionHelper.IsSimpleEnumerable(sourcePropertyInfo.PropertyType))
					{
						if (!reversiveTypeMap.PropertyMaps.Any((ReversivePropertyMap pm) => pm.SourcePropertyInfo == sourcePropertyInfo))
						{
							var propertyInfo = FindDestintationProperty(reversiveTypeMap.DestinationType,
								sourcePropertyInfo, context.ContextualName);

							if (!(propertyInfo == null))
							{
								var sourceGetter = GetGetter(reversiveTypeMap.SourceType, sourcePropertyInfo);
								var sourceSetter = GetSetter(reversiveTypeMap.SourceType, sourcePropertyInfo);

								var destinationGetter = GetGetter(reversiveTypeMap.DestinationType, propertyInfo);
								var destinationSetter = GetSetter(reversiveTypeMap.DestinationType, propertyInfo);

								if (ReflectionHelper.IsSimple(sourcePropertyInfo.PropertyType)
									&& ReflectionHelper.IsSimple(propertyInfo.PropertyType))
								{
									mappers.Add(Mappers.GetSimplePropertiesMapper(sourcePropertyInfo,
										propertyInfo, converters, sourceGetter, destinationSetter));

									unmappers.Add(Mappers.GetSimplePropertiesMapper(propertyInfo,
										sourcePropertyInfo, converters, destinationGetter, sourceSetter));
								}
								else
								{
									if (ReflectionHelper.IsSimpleEnumerable(sourcePropertyInfo.PropertyType)
										&& ReflectionHelper.IsSimpleEnumerable(propertyInfo.PropertyType))
									{
										mappers.Add(Mappers.GetSimpleEnumerablePropertiesMapper(sourcePropertyInfo,
											propertyInfo, objectFactory, converters, sourceGetter, destinationSetter));

										unmappers.Add(Mappers.GetSimpleEnumerablePropertiesMapper(propertyInfo,
											sourcePropertyInfo, objectFactory, converters, destinationGetter, sourceSetter));
									}
								}
							}
						}
					}
				}

				compilationResult = (compilationResult ?? new MapCompiler.CompilationResult());

				var mapper = compilationResult.Mapper;
				var unMapper = compilationResult.UnMapper;

				compilationResult.Mapper = Mappers.GetMapperWithSimplePropertyMappers(mapper, mappers);

				compilationResult.UnMapper = Mappers.GetMapperWithSimplePropertyMappers(unMapper, unmappers);

				return compilationResult;
			}
		}

		private MapCompiler.CompilationResult ReversiveTypeMapWithMapper(MapCompiler.CompilationContext context)
		{
			var compilationResult = MapWithMapper(context);

			compilationResult.UnMapper = ((ReversiveTypeMap)context.ParentNode).UnMapper;

			return compilationResult;
		}

		private MapCompiler.CompilationResult ReversiveTypeMapWithPropertyMaps(MapCompiler.CompilationContext context)
		{
			var enumerable = context.CurrentNode as IEnumerable<PropertyMapBase>;

			if (enumerable.Count<PropertyMapBase>() == 0)
			{
				return null;
			}
			else
			{
				var mappers = new List<Action<object, object, TypeMappingContext>>();
				var unmappers = new List<Action<object, object, TypeMappingContext>>();

				foreach (PropertyMapBase current in enumerable)
				{
					MapCompiler.CompilationResult compilationResult = FirstNode(new MapCompiler.CompilationContext
					{
						Map = context.Map,
						ContextualName = context.ContextualName,
						ParentNode = enumerable,
						CurrentNode = current
					});

					mappers.Add(compilationResult.Mapper);
					unmappers.Add(compilationResult.UnMapper);
				}

				return new MapCompiler.CompilationResult
				{
					Mapper = Mappers.GetMapperWithPropertyMappers(mappers),
					UnMapper = Mappers.GetMapperWithPropertyMappers(unmappers)
				};
			}
		}

		private MapCompiler.CompilationResult ReversivePropertyMapWithMapper(MapCompiler.CompilationContext context)
		{
			var compilationResult = MapWithMapper(context);

			compilationResult.UnMapper = ((ReversivePropertyMap)context.ParentNode).UnMapper;

			return compilationResult;
		}

		private MapCompiler.CompilationResult ReversivePropertyMapWithInheritanceMaps(MapCompiler.CompilationContext context)
		{
			var enumerable = context.CurrentNode as IEnumerable<TypeMapBase>;

			if (enumerable == null || enumerable.Count<TypeMapBase>() == 0)
			{
				return null;
			}
			else
			{
				var propertyMapBase = context.ParentNode as PropertyMapBase;

				var mapperCollection = new MapperCollection();

				var unmapperCollection = new MapperCollection();

				string text = context.ContextualName;

				if (propertyMapBase.DestinationPropertyInfo == null)
				{
					text += propertyMapBase.SourcePropertyInfo.Name;
				}

				foreach (TypeMapBase current in enumerable)
				{
					var compilationResult = Compile(current, text);

					mapperCollection.Add(current.SourceType, current.DestinationType, compilationResult.Mapper);

					unmapperCollection.Add(current.DestinationType, current.SourceType, compilationResult.UnMapper);
				}

				var compilationResult2 = new MapCompiler.CompilationResult();
				var sourceGetter = GetGetter(context.Map.SourceType, propertyMapBase.SourcePropertyInfo);
				var sourceSetter = GetSetter(context.Map.SourceType, propertyMapBase.SourcePropertyInfo);

				if (propertyMapBase.DestinationPropertyInfo == null)
				{
					compilationResult2.Mapper = Mappers.GetMapperToRootType(sourceGetter, mapperCollection);

					compilationResult2.UnMapper = Mappers.GetMapperFromRootType(propertyMapBase.SourcePropertyInfo.PropertyType,
						sourceSetter, unmapperCollection, objectFactory);

					return compilationResult2;
				}
				else
				{
					var destinationGetter = GetGetter(context.Map.DestinationType, propertyMapBase.DestinationPropertyInfo);

					var destinationSetter = GetSetter(context.Map.DestinationType, propertyMapBase.DestinationPropertyInfo);

					if (ReflectionHelper.IsComplex(propertyMapBase.SourcePropertyInfo.PropertyType)
						&& ReflectionHelper.IsComplex(propertyMapBase.DestinationPropertyInfo.PropertyType))
					{
						compilationResult2.Mapper = Mappers.GetComplexPropertiesMapper(sourceGetter,
							destinationSetter, mapperCollection, objectFactory);
						compilationResult2.UnMapper = Mappers.GetComplexPropertiesMapper(destinationGetter,
							sourceSetter, unmapperCollection, objectFactory);

						return compilationResult2;
					}
					else
					{
						if (ReflectionHelper.IsComplexEnumerable(propertyMapBase.SourcePropertyInfo.PropertyType)
							&& ReflectionHelper.IsComplexEnumerable(propertyMapBase.DestinationPropertyInfo.PropertyType))
						{
							compilationResult2.Mapper = Mappers.GetComplexEnumerablePropertiesMapper(propertyMapBase.DestinationPropertyInfo
								.PropertyType, sourceGetter, destinationSetter, objectFactory, mapperCollection);
							compilationResult2.UnMapper = Mappers.GetComplexEnumerablePropertiesMapper(propertyMapBase.SourcePropertyInfo
								.PropertyType, destinationGetter, sourceSetter, objectFactory, unmapperCollection);

							return compilationResult2;
						}
						else
						{
							return null;
						}
					}
				}
			}
		}

		private MapCompiler.CompilationResult FirstNode(MapCompiler.CompilationContext currentNodeContext)
		{
			var properties = currentNodeContext.CurrentNode.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < properties.Length; i++)
			{
				var propertyInfo = properties[i];

				if (nodeCompilers.ContainsKey(propertyInfo))
				{
					var value = propertyInfo.GetValue(currentNodeContext.CurrentNode, null);

					if (value != null)
					{
						var compilationResult = nodeCompilers[propertyInfo](new MapCompiler.CompilationContext
						{
							Map = currentNodeContext.Map,
							ParentNode = currentNodeContext.CurrentNode,
							CurrentNode = value,
							ContextualName = currentNodeContext.ContextualName
						});

						if (compilationResult != null)
						{
							return compilationResult;
						}
					}
				}
			}

			return null;
		}

		private PropertyInfo FindDestintationProperty(Type destinationType, PropertyInfo sourcePropertyInfo, string contextualName)
		{
			var property = destinationType.GetProperty(string.Format("{0}{1}{2}", sourcePropertyInfo.DeclaringType.Name,
				contextualName, sourcePropertyInfo.Name));

			if (property == null)
			{
				property = destinationType.GetProperty(string.Format("{0}{1}", contextualName, sourcePropertyInfo.Name));
			}

			return property;
		}

		private Func<object, object> GetGetter(Type type, PropertyInfo pi)
		{
			if (!getters.ContainsKey(pi))
			{
				getters.Add(pi, ReflectionHelper.CreateGetter(type, pi));
			}

			return getters[pi];
		}

		private Action<object, object> GetSetter(Type type, PropertyInfo pi)
		{
			if (!setters.ContainsKey(pi))
			{
				setters.Add(pi, ReflectionHelper.CreateSetter(type, pi));
			}

			return setters[pi];
		}

		#endregion

		#region Classes

		internal sealed class CompilationResult
		{
			#region Public Properties

			public Action<object, object, TypeMappingContext> Mapper
			{
				get;
				set;
			}

			public Action<object, object, TypeMappingContext> UnMapper
			{
				get;
				set;
			}

			#endregion
		}

		private sealed class CompilationContext
		{
			#region Public Properties

			public TypeMapBase Map
			{
				get;
				set;
			}

			public object ParentNode
			{
				get;
				set;
			}

			public object CurrentNode
			{
				get;
				set;
			}

			public string ContextualName
			{
				get;
				set;
			}

			#endregion
		}

		#endregion
	}
}
