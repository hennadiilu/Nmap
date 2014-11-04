using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nmap.Internal
{
	internal sealed class MapValidator
	{
		#region Fields

		private readonly IDictionary<PropertyInfo, Action<MapValidator.ValidationContext>> nodeValidators;

		private readonly IDictionary<Type, Action<MapValidator.ValidationContext>> rootValidators;

		#endregion

		#region Constructors

		public MapValidator()
		{
			rootValidators = new Dictionary<Type, Action<MapValidator.ValidationContext>>();
			nodeValidators = new Dictionary<PropertyInfo, Action<MapValidator.ValidationContext>>();
			rootValidators.Add(typeof(TypeMap), new Action<MapValidator.ValidationContext>(TypeMap));
			rootValidators.Add(typeof(ReversiveTypeMap), new Action<MapValidator.ValidationContext>(ReversiveTypeMap));
			nodeValidators.Add(ReflectionHelper.GetMemberInfo<TypeMap, ICollection<PropertyMap>>((TypeMap o) => o.PropertyMaps) as PropertyInfo, new Action<MapValidator.ValidationContext>(PropertyMaps));
			nodeValidators.Add(ReflectionHelper.GetMemberInfo<ReversiveTypeMap, ICollection<ReversivePropertyMap>>((ReversiveTypeMap o) => o.PropertyMaps) as PropertyInfo, new Action<MapValidator.ValidationContext>(ReversivePropertyMaps));
			nodeValidators.Add(ReflectionHelper.GetMemberInfo<PropertyMap, ICollection<TypeMap>>((PropertyMap o) => o.InheritanceMaps) as PropertyInfo, new Action<MapValidator.ValidationContext>(InheritanceMaps));
			nodeValidators.Add(ReflectionHelper.GetMemberInfo<ReversivePropertyMap, ICollection<ReversiveTypeMap>>((ReversivePropertyMap o) => o.InheritanceMaps) as PropertyInfo, new Action<MapValidator.ValidationContext>(InheritanceMaps));
		}

		#endregion

		#region Public Methods

		public void Validate(TypeMapBase map, IEnumerable<TypeMapBase> maps)
		{
			if (!rootValidators.ContainsKey(map.GetType()))
			{
				Error.MapValidationException_TypeMapIsNotSupported(map);
			}

			var obj = new MapValidator.ValidationContext
			{
				Map = map,
				CurrentNode = map,
				ParentNode = maps
			};

			rootValidators[map.GetType()](obj);
		}

		#endregion

		#region Private Methods

		private void TypeMap(MapValidator.ValidationContext context)
		{
			Error.MapValidationException_IfTypeMapDuplicated(context.Map, context.ParentNode as IEnumerable<TypeMapBase>);
			Error.MapValidationException_IfTypeMapIsNotForComplexTypes(context.Map);

			AllNodes(context);
		}

		private void ReversiveTypeMap(MapValidator.ValidationContext context)
		{
			Error.MapValidationException_IfTypeMapperOrTypeUnMapperIsNotDefined(context.Map as ReversiveTypeMap);

			TypeMap(context);
		}

		private void PropertyMaps(MapValidator.ValidationContext context)
		{
			var enumerable = context.CurrentNode as IEnumerable<PropertyMapBase>;

			Error.MapValidationException_IfTypeMapHasMapperAndPropertyMaps(context.Map, context.Map.Mapper, enumerable);

			foreach (PropertyMapBase current in enumerable)
			{
				Error.MapValidationException_IfPropertyMapDuplicated(context.Map, current, enumerable);

				AllNodes(new MapValidator.ValidationContext
				{
					Map = context.Map,
					ParentNode = enumerable,
					CurrentNode = current
				});
			}
		}

		private void ReversivePropertyMaps(MapValidator.ValidationContext context)
		{
			foreach (PropertyMapBase current in context.CurrentNode as IEnumerable<PropertyMapBase>)
			{
				Error.MapValidationException_IfPropertyMapperOrPropertyUnMapperIsNotDefined(context.Map, current as ReversivePropertyMap);
			}

			PropertyMaps(context);
		}

		private void InheritanceMaps(MapValidator.ValidationContext context)
		{
			var enumerable = context.CurrentNode as IEnumerable<TypeMapBase>;
			var propertyMapBase = context.ParentNode as PropertyMapBase;

			Error.MapValidationException_IfPropertyMapHasMapperAndInheritanceMapsOrNothing(
				context.Map, propertyMapBase, propertyMapBase.Mapper, enumerable);
			Error.MapValidationException_IfPropertyMapIsNotForBothComplexEnumerableOrComplexTypes(context.Map, propertyMapBase);

			foreach (TypeMapBase current in enumerable)
			{
				Error.MapValidationException_IfInheritanceMapDuplicated(current, enumerable);
				Error.MapValidationException_IfInheritanceMapIsNotForDerivedTypes(context.Map, propertyMapBase, current);

				Validate(current, enumerable);
			}
		}

		private void AllNodes(MapValidator.ValidationContext currentNodeContext)
		{
			var properties = currentNodeContext.CurrentNode.GetType()
				.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < properties.Length; i++)
			{
				var propertyInfo = properties[i];
				object value = propertyInfo.GetValue(currentNodeContext.CurrentNode, null);

				if (value != null && nodeValidators.ContainsKey(propertyInfo))
				{
					nodeValidators[propertyInfo](new MapValidator.ValidationContext
					{
						Map = currentNodeContext.Map,
						CurrentNode = value,
						ParentNode = currentNodeContext.CurrentNode
					});
				}
			}
		}

		#endregion

		#region Classes

		private sealed class ValidationContext
		{
			#region Properties

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

			#endregion
		}

		#endregion
	}
}
