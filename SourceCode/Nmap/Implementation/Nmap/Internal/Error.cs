using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nmap.Properties;

namespace Nmap.Internal
{
	internal static class Error
	{
		#region Public Methods

		public static void ArgumentNullException_IfNull(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(string.Format(Resources.ArgumentIsNull1, argumentName ?? string.Empty));
			}
		}

		public static void ArgumentException_IfNotSimple(Type argumentType, string argumentName)
		{
			if (argumentType == null || !ReflectionHelper.IsSimple(argumentType))
			{
				throw new ArgumentException(string.Format(Resources.NotSimple1, argumentName ?? string.Empty));
			}
		}

		public static void ArgumentException_IfSimpleOrSimpleEnumerable(Type argumentType, string argumentName)
		{
			if (argumentType == null || ReflectionHelper.IsSimple(argumentType) || ReflectionHelper.IsSimpleEnumerable(argumentType))
			{
				throw new ArgumentException(string.Format(Resources.SimpleOrSimpleEnumerable1, argumentName));
			}
		}

		public static void ArgumentException_IfNotOneLevelMemberExpression(Expression expression)
		{
			var lambdaExpression = expression as LambdaExpression;

			if (lambdaExpression == null)
			{
				throw new ArgumentException(Resources.NotOneLevelMemberExpression);
			}

			Expression body = lambdaExpression.Body;

			if (body.NodeType == ExpressionType.MemberAccess || body.NodeType == ExpressionType.Convert)
			{
				if (!(body.ToString().Count((char c) => c == '.') <= 1))
				{
					throw new ArgumentException(Resources.NotOneLevelMemberExpression);
				}
			}
			else
			{
				throw new ArgumentException(Resources.NotOneLevelMemberExpression);
			}
		}

		public static void MapValidationException_IfTypeMapDuplicated(TypeMapBase map, IEnumerable<TypeMapBase> maps)
		{
			if (maps.Count((TypeMapBase m) => m.Equals(map)) > 1)
			{
				throw new MapValidationException(string.Format(Resources.TypeMapDuplicated1,
					(map != null) ? map.ToString() : string.Empty), null);
			}
		}

		public static void MapValidationException_IfPropertyMapDuplicated(TypeMapBase typeMap, PropertyMapBase propertyMap,
			IEnumerable<PropertyMapBase> propertyMaps)
		{
			if (propertyMaps.Count((PropertyMapBase m) => m.SourcePropertyInfo == propertyMap.SourcePropertyInfo
				|| m.DestinationPropertyInfo == propertyMap.DestinationPropertyInfo) > 1)
			{
				throw new MapValidationException(string.Format(Resources.PropertyMapDuplicated2,
					(propertyMap != null) ? propertyMap.ToString() : string.Empty,
					(typeMap != null) ? typeMap.ToString() : string.Empty), null);
			}
		}

		public static void MapValidationException_IfTypeMapIsNotForComplexTypes(TypeMapBase map)
		{
			if (map == null || !ReflectionHelper.IsComplex(map.SourceType) || !ReflectionHelper.IsComplex(map.DestinationType))
			{
				throw new MapValidationException(string.Format(Resources.TypeMapIsNotForComplexTypes1,
					(map != null) ? map.ToString() : string.Empty), null);
			}
		}

		public static void MapValidationException_IfInheritanceMapIsNotForDerivedTypes(TypeMapBase map,
			PropertyMapBase propertyMap, TypeMapBase inheritanceMap)
		{
			Type baseType =
				ReflectionHelper.IsComplexEnumerable(propertyMap.SourcePropertyInfo.PropertyType)
				? ReflectionHelper.GetEnumerableItemType(propertyMap.SourcePropertyInfo.PropertyType)
				: propertyMap.SourcePropertyInfo.PropertyType;

			Type type =
				(propertyMap.DestinationPropertyInfo != null)
				? (ReflectionHelper.IsComplexEnumerable(propertyMap.DestinationPropertyInfo.PropertyType)
					? ReflectionHelper.GetEnumerableItemType(propertyMap.DestinationPropertyInfo.PropertyType)
					: propertyMap.DestinationPropertyInfo.PropertyType)
				: null;

			if (!ReflectionHelper.IsAssignable(baseType, inheritanceMap.SourceType)
				|| (type != null && !ReflectionHelper.IsAssignable(type, inheritanceMap.DestinationType))
				|| (type == null && map.DestinationType != inheritanceMap.DestinationType))
			{
				throw new MapValidationException(string.Format(Resources.InheritanceMapIsNotForDerivedTypes3,
					(inheritanceMap != null)
					? inheritanceMap.ToString()
					: string.Empty, (propertyMap != null)
						? propertyMap.ToString()
						: string.Empty, (map != null)
							? map.ToString()
							: string.Empty), null);
			}
		}

		public static void MapValidationException_IfInheritanceMapDuplicated(TypeMapBase map, IEnumerable<TypeMapBase> maps)
		{
			if (maps.Count((TypeMapBase m) => m.SourceType == map.SourceType || m.DestinationType == map.DestinationType) > 1)
			{
				throw new MapValidationException(string.Format(Resources.InheritanceMapDuplicated1,
					(map != null) ? map.ToString() : string.Empty), null);
			}
		}

		public static void MapValidationException_IfTypeMapHasMapperAndPropertyMaps(TypeMapBase map,
			Action<object, object, TypeMappingContext> mapper, IEnumerable<PropertyMapBase> propertyMaps)
		{
			if (mapper != null && propertyMaps != null && propertyMaps.Count<PropertyMapBase>() > 0)
			{
				throw new MapValidationException(string.Format(Resources.TypeMapHasMapperAndMaps1,
					(map != null) ? map.ToString() : string.Empty), null);
			}
		}

		public static void MapValidationException_IfPropertyMapHasMapperAndInheritanceMapsOrNothing(TypeMapBase typeMap,
			PropertyMapBase propertyMap, Action<object, object, TypeMappingContext> mapper, IEnumerable<TypeMapBase> inheritanceMaps)
		{
			if ((mapper != null && inheritanceMaps != null && inheritanceMaps.Count<TypeMapBase>() > 0)
				|| (mapper == null && (inheritanceMaps == null || inheritanceMaps.Count<TypeMapBase>() == 0)))
			{
				throw new MapValidationException(string.Format(Resources.PropertyMapHasMapperAndInheritanceMapsOrNothing2,
					(propertyMap != null) ? propertyMap.ToString()
					: string.Empty, (typeMap != null)
						? typeMap.ToString()
						: string.Empty), null);
			}
		}

		public static void MapValidationException_IfTypeMapperOrTypeUnMapperIsNotDefined(ReversiveTypeMap map)
		{
			if ((map.Mapper == null && map.UnMapper != null) || (map.Mapper != null && map.UnMapper == null))
			{
				throw new MapValidationException(string.Format(Resources.TypeMapperOrTypeUnMapperIsNotDefined1,
					(map != null) ? map.ToString() : string.Empty), null);
			}
		}

		public static void MapValidationException_IfPropertyMapperOrPropertyUnMapperIsNotDefined(
			TypeMapBase typeMap, ReversivePropertyMap propertyMap)
		{
			if ((propertyMap.Mapper == null && propertyMap.UnMapper != null)
				|| (propertyMap.Mapper != null && propertyMap.UnMapper == null))
			{
				throw new MapValidationException(string.Format(Resources.PropertyMapperOrPropertyUnMapperIsNotDefined2,
					(propertyMap != null)
					? propertyMap.ToString()
					: string.Empty, (typeMap != null)
						? typeMap.ToString()
						: string.Empty), null);
			}
		}

		public static void MapValidationException_IfPropertyMapIsNotForBothComplexEnumerableOrComplexTypes(
			TypeMapBase typeMap, PropertyMapBase propertyMap)
		{
			Type propertyType = propertyMap.SourcePropertyInfo.PropertyType;

			Type type = (propertyMap.DestinationPropertyInfo != null) ? propertyMap.DestinationPropertyInfo.PropertyType : null;

			if ((type == null && (ReflectionHelper.IsComplexEnumerable(propertyType)
				|| ReflectionHelper.IsSimple(propertyType))) || (type != null && ((ReflectionHelper.IsComplexEnumerable(propertyType) && ReflectionHelper.IsComplex(type)) || (ReflectionHelper.IsComplex(propertyType) && ReflectionHelper.IsComplexEnumerable(type)) || ReflectionHelper.IsSimple(propertyType) || ReflectionHelper.IsSimple(type))))
			{
				throw new MapValidationException(string.Format(Resources.PropertyMapIsNotForBothComplexEnumerableOrComplexTypes2,
					(propertyMap != null)
					? propertyMap.ToString()
					: string.Empty, (typeMap != null)
						? typeMap.ToString()
						: string.Empty), null);
			}
		}

		public static void MapValidationException_TypeMapIsNotSupported(TypeMapBase map)
		{
			throw new MapValidationException(string.Format(Resources.TypeMapIsNotSupported1,
				(map != null) ? map.ToString() : string.Empty), null);
		}

		public static void MapValidationException_PropertyMapIsNotSupported(PropertyMapBase map)
		{
			throw new MapValidationException(string.Format(Resources.PropertyMapIsNotSupported1,
				(map != null) ? map.ToString() : string.Empty), null);
		}

		public static void MappingException_IfMapperIsNull(Action<object, object, TypeMappingContext> mapper, Type fromType)
		{
			if (mapper == null)
			{
				throw new MappingException(string.Format(Resources.MapperNotFound1,
					(fromType != null) ? fromType.FullName : string.Empty), null);
			}
		}

		#endregion
	}
}
