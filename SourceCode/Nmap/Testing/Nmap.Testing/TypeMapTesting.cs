using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nmap.Testing
{
	[TestClass]
	public class TypeMapTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void ReversiveTypeMap_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ReversiveTypeMap(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void ReversiveTypeMap_DestinationTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ReversiveTypeMap(typeof(string), null);
		}

		[TestMethod]
		public void ReversiveTypeMap_GoodValues_Succeeds()
		{
			//Arrange
			//Act
			var reversiveTypeMap = new ReversiveTypeMap(typeof(string), typeof(int));

			//Assert
			Assert.AreEqual<Type>(typeof(string), reversiveTypeMap.SourceType);
			Assert.AreEqual<Type>(typeof(int), reversiveTypeMap.DestinationType);
		}

		[TestMethod]
		public void ToString_Always_Succeeds()
		{
			//Arrange
			var typeMap = new TypeMap(typeof(string), typeof(int));

			//Act
			string actual = typeMap.ToString();

			//Assert
			Assert.AreEqual<string>(string.Format("{{{0}, {1}}}", typeof(string), typeof(int)), actual);
		}

		[TestMethod]
		public void PropertyMaps_Initially_IsEmpty()
		{
			//Arrange
			var reversiveTypeMap = new ReversiveTypeMap(typeof(string), typeof(int));

			//Act
			var propertyMaps = reversiveTypeMap.PropertyMaps;

			//Assert
			Assert.AreEqual<int>(0, propertyMaps.Count);
		}
	}
}
