using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Common;

namespace Nmap.Testing
{
	[TestClass]
	public class ReversiveTypeMapConfigurationTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void ReversiveTypeMapConfiguration_MapIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ReversiveTypeMapConfiguration<MainEntity, MainEntityModel>(null);
		}

		[TestMethod]
		public void ReversiveTypeMapConfiguration_GoodValues_Succeeds()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			//Act
			var reversiveTypeMapConfiguration = new ReversiveTypeMapConfiguration<MainEntity, MainEntityModel>(map);

			Assert.AreEqual<Type>(typeof(MainEntity), reversiveTypeMapConfiguration.Map.SourceType);
			Assert.AreEqual<Type>(typeof(MainEntityModel), reversiveTypeMapConfiguration.Map.DestinationType);
			Assert.AreEqual<int>(0, reversiveTypeMapConfiguration.Map.PropertyMaps.Count);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void As_MapperIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			var reversiveTypeMapConfiguration = new ReversiveTypeMapConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapConfiguration.As(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void As_UnMapperIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;
			var reversiveTypeMapConfiguration = new ReversiveTypeMapConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			//Assert
			reversiveTypeMapConfiguration.As(
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				}, null);
		}

		[TestMethod]
		public void As_GoodValues_SetsMapperAndReturnsReversiveTypeMap()
		{
			//Arrange
			var map = MapBuilder.Instance.CreateReversiveMap<MainEntity, MainEntityModel>().Map;

			var reversiveTypeMapConfiguration = new ReversiveTypeMapConfiguration<MainEntity, MainEntityModel>(map);

			//Act
			var reversiveTypeMap = reversiveTypeMapConfiguration.As(
				delegate(MainEntity source, MainEntityModel dest, TypeMappingContext context)
				{
				}, delegate(MainEntityModel source, MainEntity dest, TypeMappingContext context)
				{
				});

			//Assert
			Assert.AreEqual<Type>(map.SourceType, reversiveTypeMap.SourceType);
			Assert.AreEqual<Type>(map.DestinationType, reversiveTypeMap.DestinationType);
			Assert.AreEqual<int>(0, reversiveTypeMap.PropertyMaps.Count);
			Assert.IsNotNull(reversiveTypeMap.Mapper);
			Assert.IsNotNull(reversiveTypeMap.UnMapper);
		}
	}
}
