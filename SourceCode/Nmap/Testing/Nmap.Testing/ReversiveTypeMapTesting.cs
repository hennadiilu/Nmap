using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nmap.Testing
{
	[TestClass]
	public class ReversiveTypeMapTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void TypeMap_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMap(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void TypeMap_DestinationTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMap(typeof(string), null);
		}

		[TestMethod]
		public void TypeMap_GoodValues_Succeeds()
		{
			//Act
			var typeMap = new TypeMap(typeof(string), typeof(int));

			//Assert
			Assert.AreEqual<Type>(typeof(string), typeMap.SourceType);
			Assert.AreEqual<Type>(typeof(int), typeMap.DestinationType);
		}

		[TestMethod]
		public void ToString_Always_Succeeds()
		{
			//Arrange
			TypeMap typeMap = new TypeMap(typeof(string), typeof(int));

			//Act
			string actual = typeMap.ToString();

			//Assert
			Assert.AreEqual<string>(string.Format("{{{0}, {1}}}", typeof(string), typeof(int)), actual);
		}

		[TestMethod]
		public void PropertyMaps_Initially_IsEmpty()
		{
			//Arrange
			TypeMap typeMap = new TypeMap(typeof(string), typeof(int));

			//Act
			var propertyMaps = typeMap.PropertyMaps;

			//Assert
			Assert.AreEqual<int>(0, propertyMaps.Count);
		}
	}
}
