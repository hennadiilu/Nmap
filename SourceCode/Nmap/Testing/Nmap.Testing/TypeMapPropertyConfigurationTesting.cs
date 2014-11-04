using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;
using Testing.Common;

namespace Nmap.Testing
{
	[TestClass]
	public class TypeMapPropertyConfigurationTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void TypeMapPropertyConfiguration_MapIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(null);
		}

		[TestMethod]
		public void TypeMapPropertyConfiguration_GoodValues_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			//Act
			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Assert
			Assert.AreEqual<Type>(typeof(MainEntity), typeMapPropertyConfiguration.Map.SourceType);
			Assert.AreEqual<Type>(typeof(MainEntityModel), typeMapPropertyConfiguration.Map.DestinationType);
			Assert.AreEqual<int>(0, typeMapPropertyConfiguration.Map.PropertyMaps.Count);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Action_SourcePropertyIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			typeMapPropertyConfiguration.MapProperty(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Action_MapperIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			typeMapPropertyConfiguration.MapProperty((MainEntity o) => (object)o.Simple, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void MapProperty_Expression_Action_SourcePropertyIsNotOneLevelMemberExpression_ThrowsArgumentException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			typeMapPropertyConfiguration.MapProperty((MainEntity o) => (object)o.SubEntity.Simple,
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				});
		}

		[TestMethod]
		public void MapProperty_Expression_Action_GoodValues_ReturnsTypeMapPropertyConfiguration()
		{
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			var typeMapPropertyConfiguration2 = typeMapPropertyConfiguration.MapProperty((MainEntity o)
				=> (object)o.Simple, delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				});

			//Assert
			Assert.AreEqual<TypeMap>(map, typeMapPropertyConfiguration2.Map);
			Assert.IsNull(typeMapPropertyConfiguration2.Map.Mapper);
			Assert.AreEqual<Type>(map.SourceType, typeMapPropertyConfiguration2.Map.SourceType);
			Assert.AreEqual<Type>(map.DestinationType, typeMapPropertyConfiguration2.Map.DestinationType);
			Assert.AreEqual<int>(1, typeMapPropertyConfiguration2.Map.PropertyMaps.Count);
			Assert.AreEqual<MemberInfo>(ReflectionHelper.GetMemberInfo<MainEntity, int>((MainEntity o) => o.Simple),
				typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().SourcePropertyInfo);
			Assert.IsNotNull(typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().Mapper);
			Assert.IsNull(typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().DestinationPropertyInfo);
			Assert.AreEqual<int>(0, typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().InheritanceMaps.Count);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_SourcePropertyIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			typeMapPropertyConfiguration.MapProperty(null, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_DestinationPropertyIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			typeMapPropertyConfiguration.MapProperty((MainEntity o) => o.SubEntity, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_InheritanceMapsIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			typeMapPropertyConfiguration.MapProperty((MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_SourcePropertyIsNotOneLevelMemberExpression_ThrowsArgumentException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			typeMapPropertyConfiguration.MapProperty((MainEntity o) =>
				(object)o.SubEntity.Simple, (MainEntityModel o) => (object)o.Simple, new TypeMap[]
				{
					MapBuilder.Instance.CreateMap<SubEntity, SubSubEntityModel>().Map
				});
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_DestinationPropertyIsNotOneLevelMemberExpression_ThrowsArgumentException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			typeMapPropertyConfiguration.MapProperty((MainEntity o) => (object)o.Simple,
				(MainEntityModel o) => (object)o.SubEntity.Simple, new TypeMap[]
				{
					MapBuilder.Instance.CreateMap<SubEntity, SubSubEntityModel>().Map
				});
		}

		[TestMethod]
		public void MapProperty_Expression_Expression_IEnumerable_GoodValues_ReturnsTypeMapPropertyConfiguration()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapPropertyConfiguration = new TypeMapPropertyConfiguration<MainEntity, MainEntityModel>(map);

			var map2 = MapBuilder.Instance.CreateMap<SubEntity, SubSubEntityModel>().Map;

			//Act
			var typeMapPropertyConfiguration2 = typeMapPropertyConfiguration.MapProperty(
				(MainEntity o) => o.SubEntity, (MainEntityModel o) => o.SubEntity, new TypeMap[]
				{
					map2
				});

			//Assert
			Assert.AreEqual<TypeMap>(map, typeMapPropertyConfiguration2.Map);
			Assert.IsNull(typeMapPropertyConfiguration2.Map.Mapper);
			Assert.AreEqual<Type>(map.SourceType, typeMapPropertyConfiguration2.Map.SourceType);
			Assert.AreEqual<Type>(map.DestinationType, typeMapPropertyConfiguration2.Map.DestinationType);
			Assert.AreEqual<int>(1, typeMapPropertyConfiguration2.Map.PropertyMaps.Count);
			Assert.AreEqual<MemberInfo>(ReflectionHelper.GetMemberInfo<MainEntity, SubEntity>((MainEntity o) => o.SubEntity), typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().SourcePropertyInfo);
			Assert.IsNull(typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().Mapper);
			Assert.AreEqual<MemberInfo>(ReflectionHelper.GetMemberInfo<MainEntityModel, SubEntityModel>((MainEntityModel o) => o.SubEntity), typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().DestinationPropertyInfo);
			Assert.IsNull(typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().InheritanceMaps.Single<TypeMap>().Mapper);
			Assert.AreEqual<int>(0, typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().InheritanceMaps.Single<TypeMap>().PropertyMaps.Count);
			Assert.AreEqual<Type>(map2.SourceType, typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().InheritanceMaps.Single<TypeMap>().SourceType);
			Assert.AreEqual<Type>(map2.DestinationType, typeMapPropertyConfiguration2.Map.PropertyMaps.Single<PropertyMap>().InheritanceMaps.Single<TypeMap>().DestinationType);
		}
	}
}
