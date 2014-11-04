using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;
using Testing.Common;

namespace Nmap.Testing
{
	[TestClass]
	public class MapValidatorTesting
	{
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapDuplicated_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;
			var array = new TypeMapBase[]
			{
				map,
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapIsNotForComplexTypes_ForSimpleTypes_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<string, string>().Map;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapIsNotForComplexTypes_ForEnumerableTypes_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity[], MainEntityModel[]>().Map;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapperOrTypeUnMapperIsNotDefined_OnlyMapper_ThrowsMapValidationException()
		{
			//Arrange
			var reversiveTypeMap = new ReversiveTypeMap(typeof(MainEntity), typeof(MainEntityModel));

			reversiveTypeMap.Mapper = delegate(object DataSourceAttribute, object dest, TypeMappingContext context)
			{
			};

			var array = new TypeMapBase[]
			{
				reversiveTypeMap
			};

			//Act
			//Assert
			new MapValidator().Validate(reversiveTypeMap, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapperOrTypeUnMapperIsNotDefined_OnlyUnMapper_ThrowsMapValidationException()
		{
			//Arrange
			var reversiveTypeMap = new ReversiveTypeMap(typeof(MainEntity), typeof(MainEntityModel));

			reversiveTypeMap.UnMapper = delegate(object source, object dest, TypeMappingContext context)
			{
			};

			var array = new TypeMapBase[]
			{
				reversiveTypeMap
			};

			//Act
			//Assert
			new MapValidator().Validate(reversiveTypeMap, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapHasMapperAndPropertyMaps_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().MapProperty(
				(MainEntity o) => o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}).Map;

			map.Mapper = delegate(object source, object dest, TypeMappingContext context)
			{
			};

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapDuplicated_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) =>
				o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				}).MapProperty((MainEntity o) => o.SubEntity,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				}).Map;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapperOrPropertyUnMapperIsNotDefined_OnlyMapper_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				},
				delegate(MainEntityModel source, MainEntity dest, TypeMappingContext context)
				{
				}).Map;

			map.PropertyMaps.First<ReversivePropertyMap>().UnMapper = null;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapperOrPropertyUnMapperIsNotDefined_OnlyUnMapper_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				},
				delegate(MainEntityModel source, MainEntity dest, TypeMappingContext context)
				{
				}).Map;

			map.PropertyMaps.First<ReversivePropertyMap>().Mapper = null;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapHasMapperAndInheritanceMapsOrNothing_HasBoth_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntity,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				}).Map;

			map.PropertyMaps.First<PropertyMap>().InheritanceMaps.Add(
				MapBuilder.Instance.CreateMap<SubEntity, SubSubEntityModel>().Map);

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapHasMapperAndInheritanceMapsOrNothing_HasNothing_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntity,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				}).Map;

			map.PropertyMaps.First<PropertyMap>().Mapper = null;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapIsForBothEnumerableOrComplexTypes_SourcePropertyIsEnumerable_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntityArrayToArray, (MainEntityModel o) => o.SubEntity, new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, SubSubEntityModel>().Map
			}).Map;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapIsForBothEnumerableOrComplexTypes_DestinationPropertyIsEnumerable_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntityArrayToArray, new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, SubSubEntityModel>().Map
			}).Map;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapIsForBothEnumerableOrComplexTypes_SourcePropertyIsSimple_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => (object)o.Simple, (MainEntityModel o) => o.SubEntity, new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, SubSubEntityModel>().Map
			}).Map;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapIsForBothEnumerableOrComplexTypes_DestinationPropertyIsSimple_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => (object)o.Simple, new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, SubEntityModel>().Map
			}).Map;

			var array = new TypeMapBase[]
			{
				map
			};

			//Act
			//Assert
			new MapValidator().Validate(map, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfInheritanceMapDuplicated_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<SubEntity, SubEntityModel>().Map;
			var map2 = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntityArrayToArray, new TypeMap[]
			{
				map,
				map
			}).Map;

			var array = new TypeMapBase[]
			{
				map2
			};

			//Act
			//Assert
			new MapValidator().Validate(map2, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfInheritanceMapIsNotForDerivedTypes_BadSourceType_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<SubEntity, SubSubEntityModel>().Map;
			var map2 = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntityArrayToArray, (MainEntityModel o) => o.SubEntityArrayToArray, new TypeMap[]
			{
				map
			}).Map;

			var array = new TypeMapBase[]
			{
				map2
			};

			//Act
			//Assert
			new MapValidator().Validate(map2, array);
		}

		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfInheritanceMapIsNotForDerivedTypes_BadDestinationType_ThrowsMapValidationException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<SubEntity, System.Type>().Map;
			var map2 = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity, new TypeMap[]
			{
				map
			}).Map;

			var array = new TypeMapBase[]
			{
				map2
			};

			//Act
			//Assert
			new MapValidator().Validate(map2, array);
		}
	}
}
