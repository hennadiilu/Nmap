using System;
using Nmap.Internal;

namespace Nmap
{
	/// <summary>
	/// Represents the base class for mappers.
	/// </summary>
	/// <typeparam name="TMapper">The type of the mapper.</typeparam>
	public abstract class Mapper<TMapper> where TMapper : Mapper<TMapper>, new()
	{
		#region Fields

		private readonly TypeMapCollection maps;
		private MapperCollection mappers;
		private MapperCollection unMappers;
		private ConverterCollection converters;
		private ObjectFactory objectFactory;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.Mapper`1" /> class.
		/// </summary>
		protected Mapper()
		{
			maps = new TypeMapCollection();
			mappers = new MapperCollection();
			unMappers = new MapperCollection();
			converters = new ConverterCollection();
			objectFactory = new ObjectFactory();
		}

		/// <summary>
		/// Initializes the <see cref="T:Nmap.Mapper`1" /> class.
		/// </summary>
		static Mapper()
		{
			Mapper<TMapper>.Instance = Activator.CreateInstance<TMapper>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the map.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <typeparam name="TDestination">The type of the destination.</typeparam>
		/// <returns>The map.</returns>
		public TypeMapBase GetMap<TSource, TDestination>()
			where TSource : class
			where TDestination : class
		{
			return maps.Get(typeof(TSource), typeof(TDestination));
		}

		/// <summary>
		/// Adds the map.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="map" /> is null.</exception>
		public void AddMap(TypeMapBase map)
		{
			maps.Add(map);
		}

		/// <summary>
		/// Removes the map.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <typeparam name="TDestination">The type of the destination.</typeparam>
		public void RemoveMap<TSource, TDestination>()
			where TSource : class
			where TDestination : class
		{
			maps.Remove(typeof(TSource), typeof(TDestination));
		}

		/// <summary>
		/// Clears the maps.
		/// </summary>
		public void ClearMaps()
		{
			maps.Clear();
		}

		/// <summary>
		/// Sets the object factory.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="factory" /> is null.</exception>
		public void SetObjectFactory(ObjectFactory factory)
		{
			Error.ArgumentNullException_IfNull(factory, "factory");
			objectFactory = factory;
		}

		/// <summary>
		/// Adds the converter.
		/// </summary>
		/// <typeparam name="TSourceProperty">The type of the source property.</typeparam>
		/// <typeparam name="TDestinationProperty">The type of the destination property.</typeparam>
		/// <param name="converter">The converter.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="converter" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <typeparamref name="TSourceProperty" />
		/// or <typeparamref name="TDestinationProperty" />is not a simple type.</exception>
		public void AddConverter<TSourceProperty, TDestinationProperty>(Func<TSourceProperty, TypeMappingContext, TDestinationProperty> converter)
		{
			converters.Add<TSourceProperty, TDestinationProperty>(converter);
		}

		/// <summary>
		/// Removes the converter.
		/// </summary>
		/// <typeparam name="TSourceProperty">The type of the source property.</typeparam>
		/// <typeparam name="TDestinationProperty">The type of the destination property.</typeparam>
		/// <exception cref="T:System.ArgumentException">The <typeparamref name="TSourceProperty" />
		/// or <typeparamref name="TDestinationProperty" />is not a simple type.</exception>
		public void RemoveConverter<TSourceProperty, TDestinationProperty>()
		{
			converters.Remove<TSourceProperty, TDestinationProperty>();
		}

		/// <summary>
		/// Clears the converters.
		/// </summary>
		public void ClearConverters()
		{
			converters.Clear();
		}

		/// <summary>
		/// Setups this mapper.
		/// </summary>
		/// <exception cref="T:Nmap.MapValidationException">Some maps are not valid.</exception>
		public void Setup()
		{
			mappers.Clear();
			unMappers.Clear();
			ValidateMaps(maps);

			foreach (TypeMapBase current in maps)
			{
				var mapCompiler = new MapCompiler(converters, objectFactory);

				if (current is ReversiveTypeMap)
				{
					MapCompiler.CompilationResult compilationResult = mapCompiler.Compile(current, string.Empty);
					mappers.Add(current.SourceType, current.DestinationType, compilationResult.Mapper);
					unMappers.Add(current.DestinationType, current.SourceType, compilationResult.UnMapper);
				}
				else
				{
					if (current is TypeMap)
					{
						mappers.Add(current.SourceType, current.DestinationType,
							mapCompiler.Compile(current, string.Empty).Mapper);
					}
				}
			}
		}

		/// <summary>
		/// Maps the specified source.
		/// </summary>
		/// <typeparam name="TDestination">The type of the destination.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="context">The context.</param>
		/// <returns>The destination object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="source" />
		/// or <typeparamref name="TDestination" /> is simple.</exception>
		/// <exception cref="T:Nmap.MappingException">A mapping error occured.</exception>
		public TDestination Map<TDestination>(object source, MappingContext context) where TDestination : class
		{
			Error.ArgumentNullException_IfNull(source, "source");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(source.GetType(), "source");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(typeof(TDestination), "TDestination");

			var typeMappingContext = new TypeMappingContext
			{
				MappingContext = context,
				From = source
			};

			typeMappingContext.To = objectFactory.CreateTargetObject(source, typeof(TDestination), context);

			return Map(mappers, typeMappingContext) as TDestination;
		}

		/// <summary>
		/// Maps the specified source.
		/// </summary>
		/// <typeparam name="TDestination">The type of the destination.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="context">The context.</param>
		/// <returns>The destination object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" />
		/// or <paramref name="destination" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="source" />
		/// or <paramref name="destination" /> is simple.</exception>
		/// <exception cref="T:Nmap.MappingException">A mapping error occured.</exception>
		public TDestination Map<TDestination>(object source,
			TDestination destination, MappingContext context) where TDestination : class
		{
			Error.ArgumentNullException_IfNull(source, "source");
			Error.ArgumentNullException_IfNull(destination, "destination");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(source.GetType(), "source");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(typeof(TDestination), "destination");

			var ctx = new TypeMappingContext
			{
				MappingContext = context,
				From = source,
				To = destination
			};

			return Map(mappers, ctx) as TDestination;
		}

		/// <summary>
		/// Maps the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="destinationType">Type of the destination.</param>
		/// <param name="context">The context.</param>
		/// <returns>The destination object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> or
		/// <paramref name="destinationType" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="source" />
		/// or <paramref name="destinationType" /> is simple.</exception>
		/// <exception cref="T:Nmap.MappingException">A mapping error occured.</exception>
		public object Map(object source, Type destinationType, MappingContext context)
		{
			Error.ArgumentNullException_IfNull(source, "source");
			Error.ArgumentNullException_IfNull(destinationType, "destinationType");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(source.GetType(), "source");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(destinationType, "destinationType");

			var typeMappingContext = new TypeMappingContext
			{
				MappingContext = context,
				From = source
			};

			typeMappingContext.To = objectFactory.CreateTargetObject(source, destinationType, context);

			return Map(mappers, typeMappingContext);
		}

		/// <summary>
		/// Unmaps the specified destination.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <param name="destination">The destination.</param>
		/// <param name="context">The context.</param>
		/// <returns>The source object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="destination" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="destination" />
		/// or <typeparamref name="TSource" /> is simple.</exception>
		/// <exception cref="T:Nmap.MappingException">An unmapping error occured.</exception>
		public TSource Unmap<TSource>(object destination, MappingContext context) where TSource : class
		{
			Error.ArgumentNullException_IfNull(destination, "destination");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(destination.GetType(), "destination");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(typeof(TSource), "TSource");

			var typeMappingContext = new TypeMappingContext
			{
				MappingContext = context,
				From = destination
			};

			typeMappingContext.To = objectFactory.CreateTargetObject(destination, typeof(TSource), context);

			return Map(unMappers, typeMappingContext) as TSource;
		}

		/// <summary>
		/// Unmaps the specified destination.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <param name="destination">The destination.</param>
		/// <param name="source">The source.</param>
		/// <param name="context">The context.</param>
		/// <returns>The source object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="destination" />
		/// or <paramref name="source" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="destination" />
		/// or <paramref name="source" /> is simple.</exception>
		/// <exception cref="T:Nmap.MappingException">An unmapping error occured.</exception>
		public TSource Unmap<TSource>(object destination, TSource source, MappingContext context) where TSource : class
		{
			Error.ArgumentNullException_IfNull(destination, "destination");
			Error.ArgumentNullException_IfNull(source, "source");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(destination.GetType(), "destination");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(source.GetType(), "source");

			var ctx = new TypeMappingContext
			{
				MappingContext = context,
				From = destination,
				To = source
			};

			return Map(unMappers, ctx) as TSource;
		}

		/// <summary>
		/// Unmaps the specified destination.
		/// </summary>
		/// <param name="destination">The destination.</param>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="context">The context.</param>
		/// <returns>The source object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="destination" />
		/// or <paramref name="sourceType" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="destination" />
		/// or <paramref name="sourceType" /> is simple.</exception>
		/// <exception cref="T:Nmap.MappingException">An unmapping error occured.</exception>
		public object Unmap(object destination, Type sourceType, MappingContext context)
		{
			Error.ArgumentNullException_IfNull(destination, "destination");
			Error.ArgumentNullException_IfNull(sourceType, "sourceType");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(destination.GetType(), "destination");
			Error.ArgumentException_IfSimpleOrSimpleEnumerable(sourceType, "sourceType");

			var typeMappingContext = new TypeMappingContext
			{
				MappingContext = context,
				From = destination
			};

			typeMappingContext.To = objectFactory.CreateTargetObject(destination, sourceType, context);

			return Map(unMappers, typeMappingContext);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Maps the specified context.
		/// </summary>
		/// <param name="relatedMappers">Related mappers collection.</param>
		/// <param name="context">The context.</param>
		/// <returns>The mapped object.</returns>
		/// <exception cref="T:Nmap.MappingException">A mapping error occured.</exception>
		private object Map(MapperCollection relatedMappers, TypeMappingContext context)
		{
			try
			{
				if (ReflectionHelper.IsComplexEnumerable(context.From.GetType())
					&& ReflectionHelper.IsComplexEnumerable(context.To.GetType()))
				{
					Mappers.MapComplexEnumerables(context, relatedMappers, objectFactory);
				}
				else
				{
					if (ReflectionHelper.IsComplex(context.From.GetType())
						&& ReflectionHelper.IsComplex(context.To.GetType()))
					{
						var action = relatedMappers.Get(context.From.GetType(), context.To.GetType());

						if (action != null)
						{
							action(context.From, context.To, context);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new MappingException(ex.Message, ex);
			}

			return context.To;
		}

		/// <summary>
		/// Validates the maps.
		/// </summary>
		/// <param name="maps">The maps.</param>
		/// <exception cref="T:Nmap.MapValidationException">Some maps are not valid.</exception>
		private void ValidateMaps(TypeMapCollection maps)
		{
			var mapValidator = new MapValidator();

			foreach (TypeMapBase current in maps)
			{
				mapValidator.Validate(current, maps);
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static TMapper Instance
		{
			get;
			private set;
		}

		#endregion
	}
}
