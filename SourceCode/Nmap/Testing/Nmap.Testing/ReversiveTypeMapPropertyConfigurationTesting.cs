using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;
using Testing.Common;

namespace Nmap.Testing
{
	[TestClass]
	public class ReversiveTypeMapPropertyConfigurationTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void ReversiveTypeMapPropertyConfiguration_MapIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(null);
		}

		[TestMethod]
		public void ReversiveTypeMapPropertyConfiguration_GoodValues_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			//Act
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Assert
			Assert.AreEqual<System.Type>(typeof(MainEntity), reversiveTypeMapPropertyConfiguration.Map.SourceType);
			Assert.AreEqual<System.Type>(typeof(MainEntityModel), reversiveTypeMapPropertyConfiguration.Map.DestinationType);
			Assert.AreEqual<int>(0, reversiveTypeMapPropertyConfiguration.Map.PropertyMaps.Count);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_Action_Action_SourcePropertyIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty(null, null, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_Action_Action_DestinationPropertyIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty(
				(MainEntity o) => (object)o.Simple, null, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_Action_Action_MapperIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty((MainEntity o)
				=> (object)o.Simple, (MainEntityModel o) => (object)o.Simple, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_Action_Action_UnMapperIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty((MainEntity o) => (object)o.Simple, (MainEntityModel o)
				=> (object)o.Simple, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				}, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void MapProperty_Expression_Expression_Action_Action_SourcePropertyIsNotOneLevelMemberExpression_ThrowsArgumentException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty((MainEntity o) => (object)o.SubEntity.Simple,
				(MainEntityModel o) => (object)o.Simple, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				},
				delegate(MainEntityModel source, MainEntity dest, TypeMappingContext context)
				{
				});
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void MapProperty_Expression_Expression_Action_Action_DestinationPropertyIsNotOneLevelMemberExpression_ThrowsArgumentException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty((MainEntity o) => (object)o.Simple, (MainEntityModel o) => (object)o.SubEntity.Simple, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			}, delegate(MainEntityModel source, MainEntity dest, TypeMappingContext context)
			{
			});
		}

		[TestMethod]
		public void MapProperty_Expression_Expression_Action_Action_GoodValues_ReturnsReversiveTypeMapPropertyConfiguration()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			var reversiveTypeMapPropertyConfiguration2 = reversiveTypeMapPropertyConfiguration.MapProperty(
				(MainEntity o) => (object)o.Simple, (MainEntityModel o) => (object)o.Simple,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				},
				delegate(MainEntityModel source, MainEntity dest, TypeMappingContext context)
				{
				});

			//Assert
			Assert.AreEqual<ReversiveTypeMap>(map, reversiveTypeMapPropertyConfiguration2.Map);
			Assert.IsNull(reversiveTypeMapPropertyConfiguration2.Map.Mapper);
			Assert.IsNull(reversiveTypeMapPropertyConfiguration2.Map.UnMapper);
			Assert.AreEqual<System.Type>(map.SourceType, reversiveTypeMapPropertyConfiguration2.Map.SourceType);
			Assert.AreEqual<System.Type>(map.DestinationType, reversiveTypeMapPropertyConfiguration2.Map.DestinationType);
			Assert.AreEqual<int>(1, reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Count);
			Assert.AreEqual<System.Reflection.MemberInfo>(ReflectionHelper.GetMemberInfo<MainEntity, int>((MainEntity o) => o.Simple), reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().SourcePropertyInfo);
			Assert.IsNotNull(reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().Mapper);
			Assert.IsNotNull(reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().UnMapper);
			Assert.AreEqual<System.Reflection.MemberInfo>(ReflectionHelper.GetMemberInfo<MainEntityModel, int>((MainEntityModel o) => o.Simple), reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().DestinationPropertyInfo);
			Assert.AreEqual<int>(0, reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().InheritanceMaps.Count);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_SourcePropertyIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty(null, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_DestinationPropertyIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty((MainEntity o) => o.SubEntity, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_InheritanceMapsIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_SourcePropertyIsNotOneLevelMemberExpression_ThrowsArgumentException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty((MainEntity o) => (object)o.SubEntity.Simple,
				(MainEntityModel o) => (object)o.Simple, new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubEntity, SubSubEntityModel>().Map
			});
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_DestinationPropertyIsNotOneLevelMemberExpression_ThrowsArgumentException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapPropertyConfiguration.MapProperty((MainEntity o) => (object)o.Simple, (MainEntityModel o) => (object)o.SubEntity.Simple, new ReversiveTypeMap[]
			{
				MapBuilder.Instance.CreateReversiveMap<SubEntity, SubSubEntityModel>().Map
			});
		}

		[TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_GoodValues_ReturnsReversiveTypeMapPropertyConfiguration()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			var reversiveTypeMapPropertyConfiguration = new ReversiveTypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			var map2 = MapBuilder.Instance.CreateReversiveMap<SubEntity, SubSubEntityModel>().Map;

			//Act
			var reversiveTypeMapPropertyConfiguration2 = reversiveTypeMapPropertyConfiguration.MapProperty(
				(MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity, new ReversiveTypeMap[]
				{
					map2
				});

			//Assert
			Assert.AreEqual<ReversiveTypeMap>(map, reversiveTypeMapPropertyConfiguration2.Map);
			Assert.IsNull(reversiveTypeMapPropertyConfiguration2.Map.Mapper);
			Assert.IsNull(reversiveTypeMapPropertyConfiguration2.Map.UnMapper);
			Assert.AreEqual<System.Type>(map.SourceType, reversiveTypeMapPropertyConfiguration2.Map.SourceType);
			Assert.AreEqual<System.Type>(map.DestinationType, reversiveTypeMapPropertyConfiguration2.Map.DestinationType);
			Assert.AreEqual<int>(1, reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Count);
			Assert.AreEqual<System.Reflection.MemberInfo>(ReflectionHelper.GetMemberInfo<MainEntity, SubEntity>((MainEntity o) => o.SubEntity), reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().SourcePropertyInfo);
			Assert.IsNull(reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().Mapper);
			Assert.IsNull(reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().UnMapper);
			Assert.AreEqual<System.Reflection.MemberInfo>(ReflectionHelper.GetMemberInfo<MainEntityModel, SubEntityModel>((MainEntityModel o) => o.SubEntity), reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().DestinationPropertyInfo);
			Assert.IsNull(reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().InheritanceMaps.Single<ReversiveTypeMap>().Mapper);
			Assert.IsNull(reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().InheritanceMaps.Single<ReversiveTypeMap>().UnMapper);
			Assert.AreEqual<int>(0, reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().InheritanceMaps.Single<ReversiveTypeMap>().PropertyMaps.Count);
			Assert.AreEqual<System.Type>(map2.SourceType, reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().InheritanceMaps.Single<ReversiveTypeMap>().SourceType);
			Assert.AreEqual<System.Type>(map2.DestinationType, reversiveTypeMapPropertyConfiguration2.Map.PropertyMaps.Single<ReversivePropertyMap>().InheritanceMaps.Single<ReversiveTypeMap>().DestinationType);
		}
	}
}
