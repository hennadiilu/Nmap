using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Nmap.Internal;

namespace Nmap
{
	/// <summary>
	/// Represents the reversive type map property configuration.
	/// </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <typeparam name="TDestination">The type of the destination.</typeparam>
	public class ReversiveTypeMapPropertyConfiguration<TSource, TDestination> : TypeMapCreating
		where TSource : class
		where TDestination : class
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.ReversiveTypeMapPropertyConfiguration`2" /> class.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="map" /> is null.</exception>
		internal ReversiveTypeMapPropertyConfiguration(ReversiveTypeMap map)
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
		/// <param name="destinationProperty">The destination property.</param>
		/// <param name="mapper">The mapper.</param>
		/// <param name="unMapper">The un mapper.</param>
		/// <returns>The reversive type map property configuration.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourceProperty" />
		/// or <paramref name="mapper" /> or <paramref name="unMapper" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="sourceProperty" />
		/// or <paramref name="destinationProperty" /> is not one level property expression.</exception>
		public ReversiveTypeMapPropertyConfiguration<TSource, TDestination> MapProperty(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDestination, object>> destinationProperty, Action<TSource, TDestination, TypeMappingContext> mapper, Action<TDestination, TSource, TypeMappingContext> unMapper)
		{
			Error.ArgumentNullException_IfNull(sourceProperty, "sourceProperty");
			Error.ArgumentNullException_IfNull(mapper, "mapper");
			Error.ArgumentNullException_IfNull(unMapper, "unMapper");
			Error.ArgumentException_IfNotOneLevelMemberExpression(sourceProperty);
			if (destinationProperty != null)
			{
				Error.ArgumentException_IfNotOneLevelMemberExpression(destinationProperty);
			}
			ReversivePropertyMap reversivePropertyMap = new ReversivePropertyMap((PropertyInfo)ReflectionHelper.GetMemberInfo<TSource, object>(sourceProperty));
			reversivePropertyMap.DestinationPropertyInfo = ((destinationProperty != null) ? ((PropertyInfo)ReflectionHelper.GetMemberInfo<TDestination, object>(destinationProperty)) : null);
			reversivePropertyMap.Mapper = delegate(object source, object destination, TypeMappingContext context)
			{
				mapper(source as TSource, destination as TDestination, context);
			};
			reversivePropertyMap.UnMapper = delegate(object destination, object source, TypeMappingContext context)
			{
				unMapper(destination as TDestination, source as TSource, context);
			};
			Map.PropertyMaps.Add(reversivePropertyMap);
			return this;
		}

		/// <summary>
		/// Maps the property.
		/// </summary>
		/// <param name="sourceProperty">The source property.</param>
		/// <param name="destinationProperty">The destination property.</param>
		/// <param name="inheritanceMaps">The inheritance maps.</param>
		/// <returns>The reversive type map property configuration.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourceProperty" />
		/// or <paramref name="inheritanceMaps" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="sourceProperty" />
		/// or <paramref name="destinationProperty" /> is not one level property expression.</exception>
		public ReversiveTypeMapPropertyConfiguration<TSource, TDestination> MapProperty(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDestination, object>> destinationProperty, IEnumerable<ReversiveTypeMap> inheritanceMaps)
		{
			Error.ArgumentNullException_IfNull(sourceProperty, "sourceProperty");
			Error.ArgumentNullException_IfNull(inheritanceMaps, "inheritanceMaps");
			Error.ArgumentException_IfNotOneLevelMemberExpression(sourceProperty);
			if (destinationProperty != null)
			{
				Error.ArgumentException_IfNotOneLevelMemberExpression(destinationProperty);
			}
			ReversivePropertyMap reversivePropertyMap = new ReversivePropertyMap((PropertyInfo)ReflectionHelper.GetMemberInfo<TSource, object>(sourceProperty));
			reversivePropertyMap.DestinationPropertyInfo = ((destinationProperty != null) ? ((PropertyInfo)ReflectionHelper.GetMemberInfo<TDestination, object>(destinationProperty)) : null);
			reversivePropertyMap.DestinationPropertyInfo = ((destinationProperty != null) ? (ReflectionHelper.GetMemberInfo<TDestination, object>(destinationProperty) as PropertyInfo) : null);
			foreach (ReversiveTypeMap current in inheritanceMaps)
			{
				reversivePropertyMap.InheritanceMaps.Add(current);
			}
			Map.PropertyMaps.Add(reversivePropertyMap);
			return this;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the map.
		/// </summary>
		public ReversiveTypeMap Map
		{
			get;
			private set;
		}

		#endregion
	}
}
