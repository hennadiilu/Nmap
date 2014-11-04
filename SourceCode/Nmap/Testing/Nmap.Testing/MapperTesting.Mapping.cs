using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Common;

namespace Nmap.Testing
{
	public partial class MapperTesting
	{
		[TestMethod]
		public void Map_WithMapper_Succeeds()
		{
			//Arrange
			var typeMap = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().As(
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext ctxt)
				{
					dest.Simple = source.Simple;
					dest.SimpleConverter = source.SimpleConverter.ToString();
				});

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();
			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(typeMap);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActMapping(mainEntity, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				MainEntityModel mainEntityModel = array[i];
				Assert.AreEqual<int>(mainEntity.Simple, mainEntityModel.Simple);
				Assert.AreEqual<string>(mainEntity.SimpleConverter.ToString(), mainEntityModel.SimpleConverter);
			}
		}

		[TestMethod]
		public void Map_EnumerableWithMapper_Succeeds()
		{
			//Arrange
			var typeMap = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().As(
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext ctxt)
				{
					dest.Simple = source.Simple;
					dest.SimpleConverter = source.SimpleConverter.ToString();
				});

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();

			var entities = new MainEntity[]
			{
				mainEntity,
				mainEntity
			};

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(typeMap);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActEnumerableMapping(entities, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				var enumerable = array[i];

				foreach (MainEntityModel current in enumerable)
				{
					Assert.AreEqual<int>(mainEntity.Simple, current.Simple);
					Assert.AreEqual<string>(mainEntity.SimpleConverter.ToString(), current.SimpleConverter);
				}
			}
		}

		[TestMethod]
		public void Map_EnumerableWithDerivedWithMappers_Succeeds()
		{
			//Arrange
			var typeMap = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().As(
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext ctxt)
				{
					dest.Simple = source.Simple;
					dest.SimpleConverter = source.SimpleConverter.ToString();
				});

			var typeMap2 = MapBuilder.Instance.CreateMap<DerivedMainEntity, DerivedMainEntityModel>().As(
				delegate(DerivedMainEntity source, DerivedMainEntityModel dest, TypeMappingContext ctxt)
				{
					dest.Simple = source.Simple;
					dest.SimpleConverter = source.SimpleConverter.ToString();
					dest.DerivedSimple = source.DerivedSimple;
				});

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();

			var derivedMainEntity = CommonHelper.CreateDerivedMainEntityWithSimpleProperties();

			var entities = new MainEntity[]
			{
				mainEntity,
				derivedMainEntity
			};

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(typeMap);
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(typeMap2);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActEnumerableMapping(entities, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				var enumerable = array[i];

				foreach (MainEntityModel current in enumerable)
				{
					if (current is DerivedMainEntityModel)
					{
						Assert.AreEqual<int>(derivedMainEntity.Simple, current.Simple);
						Assert.AreEqual<string>(derivedMainEntity.SimpleConverter.ToString(), current.SimpleConverter);
						Assert.AreEqual<int>(derivedMainEntity.DerivedSimple, ((DerivedMainEntityModel)current).DerivedSimple);
					}
					else
					{
						Assert.AreEqual<int>(mainEntity.Simple, current.Simple);
						Assert.AreEqual<string>(mainEntity.SimpleConverter.ToString(), current.SimpleConverter);
					}
				}
			}
		}

		[TestMethod]
		public void Map_WithSimple_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var entity = CommonHelper.CreateMainEntityWithSimpleProperties();

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			MainEntityModel[] array = ActMapping(entity, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				MainEntityModel model = array[i];
				AssertEntityModel(entity, model);
			}
		}

		[TestMethod]
		public void Map_EnumerableWithSimple_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();

			var entities = new MainEntity[]
			{
				mainEntity,
				mainEntity
			};

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActEnumerableMapping(entities, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				var enumerable = array[i];

				foreach (MainEntityModel current in enumerable)
				{
					AssertEntityModel(mainEntity, current);
				}
			}
		}

		[TestMethod]
		public void Map_EnumerableWithDerivedWithSimple_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;
			var map2 = MapBuilder.Instance.CreateMap<DerivedMainEntity, DerivedMainEntityModel>().Map;
			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();
			var derivedMainEntity = CommonHelper.CreateDerivedMainEntityWithSimpleProperties();

			var entities = new MainEntity[]
			{
				mainEntity,
				derivedMainEntity
			};

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map2);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActEnumerableMapping(entities, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				var enumerable = array[i];

				foreach (MainEntityModel current in enumerable)
				{
					if (current is DerivedMainEntityModel)
					{
						AssertEntityModel(derivedMainEntity, current);
					}
					else
					{
						AssertEntityModel(mainEntity, current);
					}
				}
			}
		}

		[TestMethod]
		public void Map_NestedWithPropertyMapsWithMappers_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity p) => p.SubEntity,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext ctxt)
				{
					dest.SubEntity = new SubEntityModel();
					dest.SubEntity.Simple = source.SubEntity.Simple;
					dest.SubEntity.EntitySimpleNaming = source.SubEntity.SimpleNaming;
					dest.SubEntity.EntitySimpleConverterNaming = source.SubEntity.SimpleConverterNaming.ToString();
				}).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithAllProperties(false);

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			MainEntityModel[] array = ActMapping(mainEntity, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				var mainEntityModel = array[i];

				Assert.AreEqual<int>(mainEntity.SubEntity.Simple, mainEntityModel.SubEntity.Simple);
				Assert.AreEqual<int>(mainEntity.SubEntity.SimpleNaming, mainEntityModel.SubEntity.EntitySimpleNaming);
				Assert.AreEqual<string>(mainEntity.SubEntity.SimpleConverterNaming.ToString(), mainEntityModel.SubEntity.EntitySimpleConverterNaming.ToString());
			}
		}

		[TestMethod]
		public void Map_NestedWithPropertyMapsWithSimple_Succeeds()
		{
			//Arrange
			var array = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubSubEntity, SubSubEntityModel>().Map
			};

			var array2 = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, SubEntityModel>().MapProperty((SubEntity p) =>
					p.SubSubEntity, (SubEntityModel p) => p.SubSubEntity, array).MapProperty((SubEntity p) =>
						p.SubSubEntityArrayToArray, (SubEntityModel p) => p.SubSubEntityArrayToArray, array)
						.MapProperty((SubEntity p) => p.SubSubEntityArrayToList, (SubEntityModel p) =>
							p.SubSubEntityArrayToList, array).MapProperty((SubEntity p) => p.SubSubEntityListToArray,
							(SubEntityModel p) => p.SubSubEntityListToArray, array).MapProperty((SubEntity p) =>
								p.SubSubEntityListToList, (SubEntityModel p) => p.SubSubEntityListToList, array).Map
			};

			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity p) =>
				p.SubEntity, (MainEntityModel p) => p.SubEntity, array2).MapProperty((MainEntity p) =>
					p.SubEntityArrayToArray, (MainEntityModel p) => p.SubEntityArrayToArray, array2)
					.MapProperty((MainEntity p) => p.SubEntityArrayToList, (MainEntityModel p) =>
						p.SubEntityArrayToList, array2).MapProperty((MainEntity p) => p.SubEntityListToArray,
						(MainEntityModel p) => p.SubEntityListToArray, array2).MapProperty((MainEntity p) =>
							p.SubEntityListToList, (MainEntityModel p) => p.SubEntityListToList, array2).Map;

			var entity = CommonHelper.CreateMainEntityWithAllProperties(false);

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			MainEntityModel[] array3 = ActMapping(entity, context);

			//Asserts
			for (int i = 0; i < array3.Length; i++)
			{
				MainEntityModel model = array3[i];
				AssertEntityModel(entity, model);
			}
		}

		[TestMethod]
		public void Map_NestedWithPropertyMapsWithDerivedInheritanceMaps_Succeeds()
		{
			//Arrange
			var array = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubSubEntity, SubSubEntityModel>().Map,
				MapBuilder.Instance.CreateMap<DerivedSubSubEntity, DerivedSubSubEntityModel>().Map
			};

			var array2 = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, SubEntityModel>().MapProperty((SubEntity p) => p.SubSubEntity,
				(SubEntityModel p) => p.SubSubEntity, array).MapProperty((SubEntity p) => p.SubSubEntityArrayToArray,
				(SubEntityModel p) => p.SubSubEntityArrayToArray, array).MapProperty((SubEntity p) => p.SubSubEntityArrayToList,
				(SubEntityModel p) => p.SubSubEntityArrayToList, array).MapProperty((SubEntity p) => p.SubSubEntityListToArray,
				(SubEntityModel p) => p.SubSubEntityListToArray, array).MapProperty((SubEntity p) => p.SubSubEntityListToList,
				(SubEntityModel p) => p.SubSubEntityListToList, array).Map,
				MapBuilder.Instance.CreateMap<DerivedSubEntity, DerivedSubEntityModel>().MapProperty((DerivedSubEntity p) =>
					p.SubSubEntity, (DerivedSubEntityModel p) => p.SubSubEntity, array).MapProperty((DerivedSubEntity p) =>
						p.SubSubEntityArrayToArray, (DerivedSubEntityModel p) => p.SubSubEntityArrayToArray, array)
						.MapProperty((DerivedSubEntity p) => p.SubSubEntityArrayToList, (DerivedSubEntityModel p) =>
							p.SubSubEntityArrayToList, array).MapProperty((DerivedSubEntity p) => p.SubSubEntityListToArray,
							(DerivedSubEntityModel p) => p.SubSubEntityListToArray, array).MapProperty((DerivedSubEntity p) =>
								p.SubSubEntityListToList, (DerivedSubEntityModel p) => p.SubSubEntityListToList, array).Map
			};

			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().MapProperty((MainEntity p) => p.SubEntity,
				(MainEntityModel p) => p.SubEntity, array2).MapProperty((MainEntity p) => p.SubEntityArrayToArray,
				(MainEntityModel p) => p.SubEntityArrayToArray, array2).MapProperty((MainEntity p) => p.SubEntityArrayToList,
				(MainEntityModel p) => p.SubEntityArrayToList, array2).MapProperty((MainEntity p) => p.SubEntityListToArray,
				(MainEntityModel p) => p.SubEntityListToArray, array2).MapProperty((MainEntity p) => p.SubEntityListToList,
				(MainEntityModel p) => p.SubEntityListToList, array2).Map;

			var entity = CommonHelper.CreateMainEntityWithAllProperties(true);

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			MainEntityModel[] array3 = ActMapping(entity, context);

			//Assert
			for (int i = 0; i < array3.Length; i++)
			{
				MainEntityModel model = array3[i];

				AssertEntityModel(entity, model);
			}
		}

		[TestMethod]
		public void Map_SecondToFirstLevelFlattening_Succeeds()
		{
			var array = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, FlattenedSecondLevelModel>().Map
			};

			var map = MapBuilder.Instance.CreateMap<MainEntity, FlattenedSecondLevelModel>()
				.MapProperty((MainEntity p) => p.SubEntity, null, array).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithAllProperties(false);

			var customMappingContext = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var flattenedSecondLevelModel = Mapper<MapperTesting.MapperTester>.Instance.Map<FlattenedSecondLevelModel>(
				mainEntity, customMappingContext);

			var flattenedSecondLevelModel2 = Mapper<MapperTesting.MapperTester>.Instance.Map<FlattenedSecondLevelModel>(
				mainEntity, new FlattenedSecondLevelModel(), customMappingContext);

			var flattenedSecondLevelModel3 = Mapper<MapperTesting.MapperTester>.Instance.Map(
				mainEntity, typeof(FlattenedSecondLevelModel), customMappingContext) as FlattenedSecondLevelModel;

			var array2 = new FlattenedSecondLevelModel[]
			{
				flattenedSecondLevelModel,
				flattenedSecondLevelModel2,
				flattenedSecondLevelModel3
			};

			//Assert
			for (int i = 0; i < array2.Length; i++)
			{
				FlattenedSecondLevelModel model = array2[i];
				AssertSimpleFlattenedEntityModel(mainEntity.SubEntity, model, "SubEntity");
			}
		}

		[TestMethod]
		public void Map_ThirdToFirstLevelFlattening_Succeeds()
		{
			//Arrange
			var array = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubSubEntity, FlattenedThirdLevelModel>().Map
			};

			var array2 = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, FlattenedThirdLevelModel>().MapProperty(
				(SubEntity p) => p.SubSubEntity, null, array).Map
			};

			var map = MapBuilder.Instance.CreateMap<MainEntity, FlattenedThirdLevelModel>().MapProperty(
				(MainEntity p) => p.SubEntity, null, array2).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithAllProperties(false);

			var customMappingContext = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var flattenedThirdLevelModel = Mapper<MapperTesting.MapperTester>.Instance
				.Map<FlattenedThirdLevelModel>(mainEntity, customMappingContext);

			var flattenedThirdLevelModel2 = Mapper<MapperTesting.MapperTester>.Instance
				.Map<FlattenedThirdLevelModel>(mainEntity, new FlattenedThirdLevelModel(), customMappingContext);

			var flattenedThirdLevelModel3 = Mapper<MapperTesting.MapperTester>.Instance
				.Map(mainEntity, typeof(FlattenedThirdLevelModel), customMappingContext) as FlattenedThirdLevelModel;

			//Assert
			var array3 = new FlattenedThirdLevelModel[]
			{
				flattenedThirdLevelModel,
				flattenedThirdLevelModel2,
				flattenedThirdLevelModel3
			};

			for (int i = 0; i < array3.Length; i++)
			{
				var model = array3[i];

				AssertSimpleFlattenedEntityModel(mainEntity.SubEntity.SubSubEntity, model, "SubEntitySubSubEntity");
			}
		}

		[TestMethod]
		public void Map_ThirdToSecondLevelFlattening_Succeeds()
		{
			//Arrange
			var array = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubSubEntity, SubFlattenedThirdToSecondLevelModel>().Map
			};

			var array2 = new TypeMap[]
			{
				MapBuilder.Instance.CreateMap<SubEntity, SubFlattenedThirdToSecondLevelModel>()
				.MapProperty((SubEntity p) => p.SubSubEntity, null, array).Map
			};

			var map = MapBuilder.Instance.CreateMap<MainEntity, FlattenedThirdToSecondLevelModel>()
				.MapProperty((MainEntity p) => p.SubEntity, (FlattenedThirdToSecondLevelModel p) => p.SubEntityModel, array2).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithAllProperties(false);

			var customMappingContext = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var flattenedThirdToSecondLevelModel = Mapper<MapperTesting.MapperTester>.Instance
				.Map<FlattenedThirdToSecondLevelModel>(mainEntity, customMappingContext);
			var flattenedThirdToSecondLevelModel2 = Mapper<MapperTesting.MapperTester>.Instance
				.Map<FlattenedThirdToSecondLevelModel>(mainEntity, new FlattenedThirdToSecondLevelModel(), customMappingContext);
			var flattenedThirdToSecondLevelModel3 = Mapper<MapperTesting.MapperTester>.Instance
				.Map(mainEntity, typeof(FlattenedThirdToSecondLevelModel), customMappingContext) as FlattenedThirdToSecondLevelModel;

			var array3 = new FlattenedThirdToSecondLevelModel[]
			{
				flattenedThirdToSecondLevelModel,
				flattenedThirdToSecondLevelModel2,
				flattenedThirdToSecondLevelModel3
			};

			//Assert
			for (int i = 0; i < array3.Length; i++)
			{
				var flattenedThirdToSecondLevelModel4 = array3[i];

				AssertSimpleFlattenedEntityModel(mainEntity.SubEntity.SubSubEntity,
					flattenedThirdToSecondLevelModel4.SubEntityModel, "SubSubEntity");
			}
		}
	}
}
