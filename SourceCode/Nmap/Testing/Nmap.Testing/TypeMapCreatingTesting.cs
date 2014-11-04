using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Common;

namespace Nmap.Testing
{
	[TestClass]
	public class TypeMapCreatingTesting
	{
		[TestMethod]
		public void CreateMap_Always_ReturnsTypeMapConfiguration()
		{
			//Arrange
			var typeMapCreating = new TypeMapCreating();

			//Act
			var typeMapConfiguration = typeMapCreating.CreateMap<MainEntity, MainEntityModel>();

			//Assert
			Assert.AreEqual<Type>(typeof(MainEntity), typeMapConfiguration.Map.SourceType);
			Assert.AreEqual<Type>(typeof(MainEntityModel), typeMapConfiguration.Map.DestinationType);
			Assert.AreEqual<int>(0, typeMapConfiguration.Map.PropertyMaps.Count);
			Assert.IsNull(typeMapConfiguration.Map.Mapper);
		}

		[TestMethod]
		public void CreateReversiveMap_Always_Returns_ReversiveTypeMapConfiguration()
		{
			//Arrange
			var typeMapCreating = new TypeMapCreating();

			//Act
			var reversiveTypeMapConfiguration = typeMapCreating.CreateReversiveMap<MainEntity, MainEntityModel>();

			//Assert
			Assert.AreEqual<Type>(typeof(MainEntity), reversiveTypeMapConfiguration.Map.SourceType);
			Assert.AreEqual<Type>(typeof(MainEntityModel), reversiveTypeMapConfiguration.Map.DestinationType);
			Assert.AreEqual<int>(0, reversiveTypeMapConfiguration.Map.PropertyMaps.Count);
			Assert.IsNull(reversiveTypeMapConfiguration.Map.Mapper);
			Assert.IsNull(reversiveTypeMapConfiguration.Map.UnMapper);
		}
	}
}
