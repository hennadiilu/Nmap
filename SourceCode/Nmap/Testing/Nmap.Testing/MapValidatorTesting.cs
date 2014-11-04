using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;
using System;
using System.Linq;
using Testing.Common;
namespace Nmap.Testing
{
	[TestClass]
	public class MapValidatorTesting
	{
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapDuplicated_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map,
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapIsNotForComplexTypes_ForSimpleTypes_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<string, string>().get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapIsNotForComplexTypes_ForEnumerableTypes_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity[], MainEntityModel[]>().get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapperOrTypeUnMapperIsNotDefined_OnlyMapper_ThrowsMapValidationException()
		{
			ReversiveTypeMap reversiveTypeMap = new ReversiveTypeMap(typeof(MainEntity), typeof(MainEntityModel));
			reversiveTypeMap.set_Mapper(delegate(object DataSourceAttribute, object dest, TypeMappingContext context)
			{
			});
			TypeMapBase[] array = new TypeMapBase[]
			{
				reversiveTypeMap
			};
			new MapValidator().Validate(reversiveTypeMap, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapperOrTypeUnMapperIsNotDefined_OnlyUnMapper_ThrowsMapValidationException()
		{
			ReversiveTypeMap reversiveTypeMap = new ReversiveTypeMap(typeof(MainEntity), typeof(MainEntityModel));
			reversiveTypeMap.set_UnMapper(delegate(object source, object dest, TypeMappingContext context)
			{
			});
			TypeMapBase[] array = new TypeMapBase[]
			{
				reversiveTypeMap
			};
			new MapValidator().Validate(reversiveTypeMap, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfTypeMapHasMapperAndPropertyMaps_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}).get_Map();
			map.set_Mapper(delegate(object source, object dest, TypeMappingContext context)
			{
			});
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapDuplicated_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}).MapProperty((MainEntity o) => o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}).get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapperOrPropertyUnMapperIsNotDefined_OnlyMapper_ThrowsMapValidationException()
		{
			ReversiveTypeMap map = MapBuilder.get_Instance().CreateReversiveMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}, delegate(MainEntityModel source, MainEntity dest, TypeMappingContext context)
			{
			}).get_Map();
			map.get_PropertyMaps().First<ReversivePropertyMap>().set_UnMapper(null);
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapperOrPropertyUnMapperIsNotDefined_OnlyUnMapper_ThrowsMapValidationException()
		{
			ReversiveTypeMap map = MapBuilder.get_Instance().CreateReversiveMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}, delegate(MainEntityModel source, MainEntity dest, TypeMappingContext context)
			{
			}).get_Map();
			map.get_PropertyMaps().First<ReversivePropertyMap>().set_Mapper(null);
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapHasMapperAndInheritanceMapsOrNothing_HasBoth_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}).get_Map();
			map.get_PropertyMaps().First<PropertyMap>().get_InheritanceMaps().Add(MapBuilder.get_Instance().CreateMap<SubEntity, SubSubEntityModel>().get_Map());
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapHasMapperAndInheritanceMapsOrNothing_HasNothing_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}).get_Map();
			map.get_PropertyMaps().First<PropertyMap>().set_Mapper(null);
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapIsForBothEnumerableOrComplexTypes_SourcePropertyIsEnumerable_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntityArrayToArray, (MainEntityModel o) => o.SubEntity, new TypeMap[]
			{
				MapBuilder.get_Instance().CreateMap<SubEntity, SubSubEntityModel>().get_Map()
			}).get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapIsForBothEnumerableOrComplexTypes_DestinationPropertyIsEnumerable_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntityArrayToArray, new TypeMap[]
			{
				MapBuilder.get_Instance().CreateMap<SubEntity, SubSubEntityModel>().get_Map()
			}).get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapIsForBothEnumerableOrComplexTypes_SourcePropertyIsSimple_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => (object)o.Simple, (MainEntityModel o) => o.SubEntity, new TypeMap[]
			{
				MapBuilder.get_Instance().CreateMap<SubEntity, SubSubEntityModel>().get_Map()
			}).get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfPropertyMapIsForBothEnumerableOrComplexTypes_DestinationPropertyIsSimple_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => (object)o.Simple, new TypeMap[]
			{
				MapBuilder.get_Instance().CreateMap<SubEntity, SubEntityModel>().get_Map()
			}).get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map
			};
			new MapValidator().Validate(map, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfInheritanceMapDuplicated_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<SubEntity, SubEntityModel>().get_Map();
			TypeMap map2 = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntityArrayToArray, new TypeMap[]
			{
				map,
				map
			}).get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map2
			};
			new MapValidator().Validate(map2, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfInheritanceMapIsNotForDerivedTypes_BadSourceType_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<SubEntity, SubSubEntityModel>().get_Map();
			TypeMap map2 = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntityArrayToArray, (MainEntityModel o) => o.SubEntityArrayToArray, new TypeMap[]
			{
				map
			}).get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map2
			};
			new MapValidator().Validate(map2, array);
		}
		[ExpectedException(typeof(MapValidationException)), TestMethod]
		public void Validate_IfInheritanceMapIsNotForDerivedTypes_BadDestinationType_ThrowsMapValidationException()
		{
			TypeMap map = MapBuilder.get_Instance().CreateMap<SubEntity, System.Type>().get_Map();
			TypeMap map2 = MapBuilder.get_Instance().CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity, new TypeMap[]
			{
				map
			}).get_Map();
			TypeMapBase[] array = new TypeMapBase[]
			{
				map2
			};
			new MapValidator().Validate(map2, array);
		}
	}
}
