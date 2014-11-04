using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Common;

namespace Nmap.Testing
{
	public partial class MapperTesting
	{
		[TestMethod]
		public void Unmap_WithMapper_Succeeds()
		{
			//Arrange
			var reversiveTypeMap = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>()
				.As(delegate(MainEntity source, MainEntityModel dest, TypeMappingContext ctxt)
			{
				dest.Simple = source.Simple;
				dest.SimpleConverter = source.SimpleConverter.ToString();
			}, delegate(MainEntityModel dest, MainEntity source, TypeMappingContext ctxt)
			{
				source.Simple = dest.Simple;
				source.SimpleConverter = int.Parse(dest.SimpleConverter);
			});

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(reversiveTypeMap);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActUnmapping(mainEntity, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				Assert.AreEqual<int>(mainEntity.Simple, array[i].Simple);
				Assert.AreEqual<int>(mainEntity.SimpleConverter, array[i].SimpleConverter);
			}
		}

		[TestMethod]
		public void Unmap_EnumerableWithMapper_Succeeds()
		{
			//Arrange
			var reversiveTypeMap = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>()
				.As(delegate(MainEntity source, MainEntityModel dest, TypeMappingContext ctxt)
			{
				dest.Simple = source.Simple;
				dest.SimpleConverter = source.SimpleConverter.ToString();
			}, delegate(MainEntityModel dest, MainEntity source, TypeMappingContext ctxt)
			{
				source.Simple = dest.Simple;
				source.SimpleConverter = int.Parse(dest.SimpleConverter);
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

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(reversiveTypeMap);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActEnumerableUnmapping(entities, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MainEntity current in array[i])
				{
					Assert.AreEqual<int>(mainEntity.Simple, current.Simple);
					Assert.AreEqual<int>(mainEntity.SimpleConverter, current.SimpleConverter);
				}
			}
		}

		[TestMethod]
		public void Unmap_EnumerableWithDerivedWithMappers_Succeeds()
		{
			//Arrange
			var reversiveTypeMap = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>()
				.As(delegate(MainEntity source, MainEntityModel dest, TypeMappingContext ctxt)
			{
				dest.Simple = source.Simple;
				dest.SimpleConverter = source.SimpleConverter.ToString();
			}, delegate(MainEntityModel dest, MainEntity source, TypeMappingContext ctxt)
			{
				source.Simple = dest.Simple;
				source.SimpleConverter = int.Parse(dest.SimpleConverter);
			});

			var reversiveTypeMap2 = MapBuilder.Instance.CreateReversiveMap<DerivedMainEntity, DerivedMainEntityModel>()
				.As(delegate(DerivedMainEntity source, DerivedMainEntityModel dest, TypeMappingContext ctxt)
			{
				dest.Simple = source.Simple;
				dest.SimpleConverter = source.SimpleConverter.ToString();
				dest.DerivedSimple = source.DerivedSimple;
			}, delegate(DerivedMainEntityModel dest, DerivedMainEntity source, TypeMappingContext ctxt)
			{
				source.Simple = dest.Simple;
				source.SimpleConverter = int.Parse(dest.SimpleConverter);
				source.DerivedSimple = dest.DerivedSimple;
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

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(reversiveTypeMap);

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(reversiveTypeMap2);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActEnumerableUnmapping(entities, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MainEntity current in array[i])
				{
					if (current is DerivedMainEntity)
					{
						Assert.AreEqual<int>(derivedMainEntity.Simple, current.Simple);
						Assert.AreEqual<int>(derivedMainEntity.SimpleConverter, current.SimpleConverter);
						Assert.AreEqual<int>(derivedMainEntity.DerivedSimple, ((DerivedMainEntity)current).DerivedSimple);
					}
					else
					{
						Assert.AreEqual<int>(mainEntity.Simple, current.Simple);
						Assert.AreEqual<int>(mainEntity.SimpleConverter, current.SimpleConverter);
					}
				}
			}
		}

		[TestMethod]
		public void Unmap_WithSimple_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);

			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActUnmapping(mainEntity, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				AssertEntity(mainEntity, array[i]);
			}
		}

		[TestMethod]
		public void Unmap_EnumerableWithSimple_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

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
			var array = ActEnumerableUnmapping(entities, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MainEntity current in array[i])
				{
					AssertEntity(mainEntity, current);
				}
			}
		}

		[TestMethod]
		public void Unmap_EnumerableWithDerivedWithSimple_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var map2 = MapBuilder.Instance.CreateReversiveMap<DerivedMainEntity, DerivedMainEntityModel>().Map;
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
			var array = ActEnumerableUnmapping(entities, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MainEntity current in array[i])
				{
					if (current is DerivedMainEntity)
					{
						AssertEntity(derivedMainEntity, current);
					}
					else
					{
						AssertEntity(mainEntity, current);
					}
				}
			}
		}

		[TestMethod]
		public void Unmap_NestedWithPropertyMapsWithMappers_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity p) => p.SubEntity, (MainEntityModel p) => p.SubEntity,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext ctxt)
			{
				dest.SubEntity = new SubEntityModel();
				dest.SubEntity.Simple = source.SubEntity.Simple;
				dest.SubEntity.EntitySimpleNaming = source.SubEntity.SimpleNaming;
				dest.SubEntity.EntitySimpleConverterNaming = source.SubEntity.SimpleConverterNaming.ToString();
			}, delegate(MainEntityModel dest, MainEntity source, TypeMappingContext ctxt)
			{
				source.SubEntity = new SubEntity();
				source.SubEntity.Simple = dest.SubEntity.Simple;
				source.SubEntity.SimpleNaming = dest.SubEntity.EntitySimpleNaming;
				source.SubEntity.SimpleConverterNaming = int.Parse(dest.SubEntity.EntitySimpleConverterNaming);
			}).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithAllProperties(false);
			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var array = ActUnmapping(mainEntity, context);

			//Assert
			for (int i = 0; i < array.Length; i++)
			{
				Assert.AreEqual<int>(mainEntity.SubEntity.Simple, array[i].SubEntity.Simple);
				Assert.AreEqual<int>(mainEntity.SubEntity.SimpleNaming, array[i].SubEntity.SimpleNaming);
				Assert.AreEqual<int>(mainEntity.SubEntity.SimpleConverterNaming, array[i].SubEntity.SimpleConverterNaming);
			}
		}

		[TestMethod]
		public void Unmap_NestedWithPropertyMapsWithSimple_Succeeds()
		{
			//Arrange
			var array = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubSubEntity, SubSubEntityModel>().Map
			};
			var array2 = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubEntity, SubEntityModel>()
				.MapProperty((SubEntity p) => p.SubSubEntity, (SubEntityModel p) => p.SubSubEntity, array)
				.MapProperty((SubEntity p) => p.SubSubEntityArrayToArray,
				(SubEntityModel p) => p.SubSubEntityArrayToArray, array)
				.MapProperty((SubEntity p) => p.SubSubEntityArrayToList,
				(SubEntityModel p) => p.SubSubEntityArrayToList, array)
				.MapProperty((SubEntity p) => p.SubSubEntityListToArray,
				(SubEntityModel p) => p.SubSubEntityListToArray, array)
				.MapProperty((SubEntity p) => p.SubSubEntityListToList,
				(SubEntityModel p) => p.SubSubEntityListToList, array).Map
			};

			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity p) => p.SubEntity, (MainEntityModel p) => p.SubEntity, array2)
				.MapProperty((MainEntity p) => p.SubEntityArrayToArray,
				(MainEntityModel p) => p.SubEntityArrayToArray, array2)
				.MapProperty((MainEntity p) => p.SubEntityArrayToList,
				(MainEntityModel p) => p.SubEntityArrayToList, array2)
				.MapProperty((MainEntity p) => p.SubEntityListToArray,
				(MainEntityModel p) => p.SubEntityListToArray, array2)
				.MapProperty((MainEntity p) => p.SubEntityListToList,
				(MainEntityModel p) => p.SubEntityListToList, array2).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithAllProperties(false);

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var result = ActUnmapping(mainEntity, context);

			//Assert
			for (int i = 0; i < result.Length; i++)
			{
				MainEntity actual = result[i];
				AssertEntity(mainEntity, actual);
			}
		}

		[TestMethod]
		public void Unmap_NestedWithPropertyMapsWithDerivedInheritanceMaps_Succeeds()
		{
			//Arrange
			var array = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubSubEntity, SubSubEntityModel>().Map,
				MapBuilder.Instance.CreateReversiveMap<DerivedSubSubEntity, DerivedSubSubEntityModel>().Map
			};

			var array2 = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubEntity, SubEntityModel>()
				.MapProperty((SubEntity p) => p.SubSubEntity, (SubEntityModel p) => p.SubSubEntity, array)
				.MapProperty((SubEntity p) => p.SubSubEntityArrayToArray,
				(SubEntityModel p) => p.SubSubEntityArrayToArray, array)
				.MapProperty((SubEntity p) => p.SubSubEntityArrayToList,
				(SubEntityModel p) => p.SubSubEntityArrayToList, array)
				.MapProperty((SubEntity p) => p.SubSubEntityListToArray,
				(SubEntityModel p) => p.SubSubEntityListToArray, array)
				.MapProperty((SubEntity p) => p.SubSubEntityListToList,
				(SubEntityModel p) => p.SubSubEntityListToList, array).Map,
				MapBuilder.Instance.CreateReversiveMap<DerivedSubEntity, DerivedSubEntityModel>()
				.MapProperty((DerivedSubEntity p) => p.SubSubEntity,
				(DerivedSubEntityModel p) => p.SubSubEntity, array)
				.MapProperty((DerivedSubEntity p) => p.SubSubEntityArrayToArray,
				(DerivedSubEntityModel p) => p.SubSubEntityArrayToArray, array)
				.MapProperty((DerivedSubEntity p) => p.SubSubEntityArrayToList,
				(DerivedSubEntityModel p) => p.SubSubEntityArrayToList, array)
				.MapProperty((DerivedSubEntity p) => p.SubSubEntityListToArray,
				(DerivedSubEntityModel p) => p.SubSubEntityListToArray, array)
				.MapProperty((DerivedSubEntity p) => p.SubSubEntityListToList,
				(DerivedSubEntityModel p) => p.SubSubEntityListToList, array).Map
			};

			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>()
				.MapProperty((MainEntity p) => p.SubEntity, (MainEntityModel p) => p.SubEntity, array2)
				.MapProperty((MainEntity p) => p.SubEntityArrayToArray,
				(MainEntityModel p) => p.SubEntityArrayToArray, array2)
				.MapProperty((MainEntity p) => p.SubEntityArrayToList,
				(MainEntityModel p) => p.SubEntityArrayToList, array2)
				.MapProperty((MainEntity p) => p.SubEntityListToArray,
				(MainEntityModel p) => p.SubEntityListToArray, array2)
				.MapProperty((MainEntity p) => p.SubEntityListToList,
				(MainEntityModel p) => p.SubEntityListToList, array2).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithAllProperties(true);

			var context = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var result = ActUnmapping(mainEntity, context);

			//Assert
			for (int i = 0; i < result.Length; i++)
			{
				MainEntity actual = result[i];
				AssertEntity(mainEntity, actual);
			}
		}

		[TestMethod]
		public void Unmap_SecondToFirstLevelFlattening_Succeeds()
		{
			//Arrange
			var array = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubEntity, FlattenedSecondLevelModel>().Map
			};
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, FlattenedSecondLevelModel>()
				.MapProperty((MainEntity p) => p.SubEntity, null, array).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();

			mainEntity.SubEntity = CommonHelper.CreateEntityWithSimpleProperties<SubEntity>();

			var customMappingContext = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var mainEntity2 = Mapper<MapperTesting.MapperTester>.Instance
				.Unmap<MainEntity>(Mapper<MapperTesting.MapperTester>.Instance
				.Map<FlattenedSecondLevelModel>(mainEntity, customMappingContext), customMappingContext);
			var mainEntity3 = Mapper<MapperTesting.MapperTester>.Instance
				.Unmap<MainEntity>(Mapper<MapperTesting.MapperTester>.Instance
				.Map<FlattenedSecondLevelModel>(mainEntity, new FlattenedSecondLevelModel(),
				customMappingContext), new MainEntity(), customMappingContext);
			var mainEntity4 = Mapper<MapperTesting.MapperTester>.Instance
				.Unmap(Mapper<MapperTesting.MapperTester>.Instance
				.Map(mainEntity, typeof(FlattenedSecondLevelModel),
				customMappingContext), typeof(MainEntity), customMappingContext) as MainEntity;

			//Assert
			var array2 = new MainEntity[]
			{
				mainEntity2,
				mainEntity3,
				mainEntity4
			};

			for (int i = 0; i < array2.Length; i++)
			{
				MainEntity mainEntity5 = array2[i];
				AssertEntity(mainEntity.SubEntity, mainEntity5.SubEntity);
			}
		}

		[TestMethod]
		public void Unmap_ThirdToFirstLevelFlattening_Succeeds()
		{
			//Arrange
			var array = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubSubEntity, FlattenedThirdLevelModel>().Map
			};
			var array2 = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubEntity, FlattenedThirdLevelModel>()
				.MapProperty((SubEntity p) => p.SubSubEntity, null, array).Map
			};

			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, FlattenedThirdLevelModel>()
				.MapProperty((MainEntity p) => p.SubEntity, null, array2).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();

			mainEntity.SubEntity = CommonHelper.CreateEntityWithSimpleProperties<SubEntity>();
			mainEntity.SubEntity.SubSubEntity = CommonHelper.CreateEntityWithSimpleProperties<SubSubEntity>();

			var customMappingContext = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var mainEntity2 = Mapper<MapperTesting.MapperTester>.Instance.Unmap<MainEntity>(Mapper<MapperTesting.MapperTester>.Instance.Map<FlattenedThirdLevelModel>(mainEntity, customMappingContext), customMappingContext);
			var mainEntity3 = Mapper<MapperTesting.MapperTester>.Instance.Unmap<MainEntity>(Mapper<MapperTesting.MapperTester>.Instance.Map<FlattenedThirdLevelModel>(mainEntity, new FlattenedThirdLevelModel(), customMappingContext), new MainEntity(), customMappingContext);
			var mainEntity4 = Mapper<MapperTesting.MapperTester>.Instance.Unmap(Mapper<MapperTesting.MapperTester>.Instance.Map(mainEntity, typeof(FlattenedThirdLevelModel), customMappingContext), typeof(MainEntity), customMappingContext) as MainEntity;

			//Assert
			var array3 = new MainEntity[]
			{
				mainEntity2,
				mainEntity3,
				mainEntity4
			};

			for (int i = 0; i < array3.Length; i++)
			{
				MainEntity mainEntity5 = array3[i];
				AssertEntity(mainEntity.SubEntity.SubSubEntity, mainEntity5.SubEntity.SubSubEntity);
			}
		}

		[TestMethod]
		public void Unmap_ThirdToSecondLevelFlattening_Succeeds()
		{
			//Arrange
			var array = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubSubEntity, SubFlattenedThirdToSecondLevelModel>().Map
			};

			var array2 = new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubEntity, SubFlattenedThirdToSecondLevelModel>()
				.MapProperty((SubEntity p) => p.SubSubEntity, null, array).Map
			};

			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, FlattenedThirdToSecondLevelModel>()
				.MapProperty((MainEntity p) => p.SubEntity, (FlattenedThirdToSecondLevelModel p) => p.SubEntityModel, array2).Map;

			var mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();
			mainEntity.SubEntity = CommonHelper.CreateEntityWithSimpleProperties<SubEntity>();
			mainEntity.SubEntity.SubSubEntity = CommonHelper.CreateEntityWithSimpleProperties<SubSubEntity>();

			var customMappingContext = new MapperTesting.CustomMappingContext
			{
				Data = "data1"
			};

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(map);
			Mapper<MapperTesting.MapperTester>.Instance.Setup();

			//Act
			var mainEntity2 = Mapper<MapperTesting.MapperTester>.Instance
				.Unmap<MainEntity>(Mapper<MapperTesting.MapperTester>.Instance
				.Map<FlattenedThirdToSecondLevelModel>(mainEntity, customMappingContext), customMappingContext);
			var mainEntity3 = Mapper<MapperTesting.MapperTester>.Instance
				.Unmap<MainEntity>(Mapper<MapperTesting.MapperTester>.Instance
				.Map<FlattenedThirdToSecondLevelModel>(mainEntity, new FlattenedThirdToSecondLevelModel(),
				customMappingContext), new MainEntity(), customMappingContext);
			var mainEntity4 = Mapper<MapperTesting.MapperTester>.Instance
				.Unmap(Mapper<MapperTesting.MapperTester>.Instance
				.Map(mainEntity, typeof(FlattenedThirdToSecondLevelModel),
				customMappingContext), typeof(MainEntity), customMappingContext) as MainEntity;

			//Assert
			var array3 = new MainEntity[]
			{
				mainEntity2,
				mainEntity3,
				mainEntity4
			};

			for (int i = 0; i < array3.Length; i++)
			{
				AssertEntity(mainEntity.SubEntity.SubSubEntity, array3[i].SubEntity.SubSubEntity);
			}
		}
	}
}
