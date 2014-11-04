using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;

namespace Nmap.Testing.Internal
{
	[TestClass]
	public class TypeMapCollectionTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Add_MapIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMapCollection().Add(null);
		}

		[TestMethod]
		public void Add_GoodValues_Succeeds()
		{
			//Arrange
			var typeMapCollection = new TypeMapCollection();
			var typeMap = new TypeMap(typeof(Type), typeof(Type));

			//Act
			typeMapCollection.Add(typeMap);

			//Assert
			Assert.AreEqual<TypeMapBase>(typeMap, typeMapCollection.Get(typeof(Type), typeof(Type)));
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Get_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMapCollection().Get(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Get_DestinationTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMapCollection().Get(typeof(Type), null);
		}

		[TestMethod]
		public void Get_GoodValues_ReturnsTypeMap()
		{
			//Arrange
			var typeMapCollection = new TypeMapCollection();
			var typeMap = new TypeMap(typeof(Type), typeof(Type));
			typeMapCollection.Add(typeMap);

			//Act
			var actual = typeMapCollection.Get(typeof(Type), typeof(Type));
			var value = typeMapCollection.Get(typeof(Type), typeof(string));

			//Assert
			Assert.AreEqual<TypeMapBase>(typeMap, actual);
			Assert.IsNull(value);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Remove_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMapCollection().Remove(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Remove_DestinationTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new TypeMapCollection().Get(typeof(Type), null);
		}

		[TestMethod]
		public void Remove_GoodValues_Succeeds()
		{
			//arrange
			var typeMapCollection = new TypeMapCollection();

			typeMapCollection.Add(new TypeMap(typeof(Type), typeof(Type)));

			//Act
			typeMapCollection.Remove(typeof(Type), typeof(Type));
			typeMapCollection.Remove(typeof(Type), typeof(string));
			typeMapCollection.Remove(typeof(string), typeof(string));

			//Assert
			Assert.IsNull(typeMapCollection.Get(typeof(Type), typeof(Type)));
			Assert.IsNull(typeMapCollection.Get(typeof(Type), typeof(string)));
			Assert.IsNull(typeMapCollection.Get(typeof(string), typeof(string)));
		}

		[TestMethod]
		public void Clear_Always_Succeeds()
		{
			//Arrange
			var typeMapCollection = new TypeMapCollection();

			typeMapCollection.Add(new TypeMap(typeof(Type), typeof(Type)));

			//Act
			typeMapCollection.Clear();

			//Assert
			Assert.IsNull(typeMapCollection.Get(typeof(Type), typeof(Type)));
		}
	}
}
