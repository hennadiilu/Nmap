using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Common;

namespace Nmap.Testing
{
	[TestClass]
	public class TypeMapConfigurationTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void TypeMapConfiguration_MapIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMapConfiguration<MainEntity, MainEntityModel>(null);
		}

		[TestMethod]
		public void TypeMapConfiguration_GoodValues_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			//Act
			var typeMapConfiguration = new TypeMapConfiguration<MainEntity, MainEntityModel>(map);

			//Assert
			Assert.AreEqual<Type>(typeof(MainEntity), typeMapConfiguration.Map.SourceType);
			Assert.AreEqual<Type>(typeof(MainEntityModel), typeMapConfiguration.Map.DestinationType);
			Assert.AreEqual<int>(0, typeMapConfiguration.Map.PropertyMaps.Count);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void As_MapperIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			//Act
			var typeMapConfiguration = new TypeMapConfiguration<MainEntity, MainEntityModel>(map);

			//Assert
			typeMapConfiguration.As(null);
		}

		[TestMethod]
		public void As_GoodValues_SetsMapperAndReturnsTypeMap()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateMap<MainEntity, MainEntityModel>().Map;

			var typeMapConfiguration = new TypeMapConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			var typeMap = typeMapConfiguration.As(delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
			{
			});

			//Assert
			Assert.AreEqual<Type>(map.SourceType, typeMap.SourceType);
			Assert.AreEqual<Type>(map.DestinationType, typeMap.DestinationType);
			Assert.AreEqual<int>(0, typeMap.PropertyMaps.Count);
			Assert.IsNotNull(typeMap.Mapper);
		}
	}
}
