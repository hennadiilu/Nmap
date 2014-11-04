using Nmap.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Nmap
{
	/// <summary>
	/// Represents the type map property configuration.
	/// </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <typeparam name="TDestination">The type of the destination.</typeparam>
	public class TypeMapPropertyConfiguration<TSource, TDestination>
		where TSource : class
		where TDestination : class
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.TypeMapPropertyConfiguration`2" /> class.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="map" /> is null.</exception>
		internal TypeMapPropertyConfiguration(TypeMap map)
		{
			Error.ArgumentNullException_IfNull(map, "map");
			Map = map;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Maps the property.
		/// </summary>
		/// <param name="sourceProperty">The source property.</param>
		/// <param name="mapper">The mapper.</param>
		/// <returns>The type map property configuration.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourceProperty" />
		/// or <paramref name="mapper" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="sourceProperty" />
		/// is not one level property expression.</exception>
		public TypeMapPropertyConfiguration<TSource, TDestination> MapProperty(Expression<Func<TSource, object>> sourceProperty, Action<TSource, TDestination, TypeMappingContext> mapper)
		{
			Error.ArgumentNullException_IfNull(sourceProperty, "sourceProperty");
			Error.ArgumentNullException_IfNull(mapper, "mapper");
			Error.ArgumentException_IfNotOneLevelMemberExpression(sourceProperty);

			var propertyMap = new PropertyMap((PropertyInfo)ReflectionHelper.GetMemberInfo<TSource, object>(sourceProperty));

			propertyMap.Mapper = delegate(object source, object destination, TypeMappingContext context)
			{
				mapper(source as TSource, destination as TDestination, context);
			};

			Map.PropertyMaps.Add(propertyMap);

			return this;
		}

		/// <summary>
		/// Maps the property.
		/// </summary>
		/// <param name="sourceProperty">The source property.</param>
		/// <param name="destinationProperty">The destination property.</param>
		/// <param name="inheritanceMaps">The inheritance maps.</param>
		/// <returns>The type map property configuration.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourceProperty" />
		/// or <paramref name="inheritanceMaps" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="sourceProperty" />
		/// or <paramref name="destinationProperty" /> is not one level property expression.</exception>
		public TypeMapPropertyConfiguration<TSource, TDestination> MapProperty(
			Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDestination, object>> destinationProperty,
			IEnumerable<TypeMap> inheritanceMaps)
		{
			Error.ArgumentNullException_IfNull(sourceProperty, "sourceProperty");
			Error.ArgumentNullException_IfNull(inheritanceMaps, "inheritanceMaps");
			Error.ArgumentException_IfNotOneLevelMemberExpression(sourceProperty);

			if (destinationProperty != null)
			{
				Error.ArgumentException_IfNotOneLevelMemberExpression(destinationProperty);
			}

			var propertyMap = new PropertyMap((PropertyInfo)ReflectionHelper.GetMemberInfo<TSource, object>(sourceProperty));

			propertyMap.DestinationPropertyInfo =
				(destinationProperty != null)
				? ((PropertyInfo)ReflectionHelper.GetMemberInfo<TDestination, object>(destinationProperty))
				: null;

			foreach (TypeMap current in inheritanceMaps)
			{
				propertyMap.InheritanceMaps.Add(current);
			}

			Map.PropertyMaps.Add(propertyMap);

			return this;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the map.
		/// </summary>
		public TypeMap Map
		{
			get;
			private set;
		}

		#endregion

	}
}
