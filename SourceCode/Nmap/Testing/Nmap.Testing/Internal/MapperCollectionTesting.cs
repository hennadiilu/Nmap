using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;
using Testing.Common;

namespace Nmap.Testing.Internal
{
	[TestClass]
	public class MapperCollectionTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Add_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new MapperCollection().Add(null, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Add_DestinationTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new MapperCollection().Add(typeof(Type), null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Add_MapperIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new MapperCollection().Add(typeof(Type), typeof(Type), null);
		}

		[TestMethod]
		public void Add_GoodValues_Succeeds()
		{
			//Arrange
			var mapperCollection = new MapperCollection();

			//Act
			mapperCollection.Add(typeof(Type), typeof(Type), delegate(object source, object dest, TypeMappingContext context)
			{
			});

			//Assert
			Assert.IsNotNull(mapperCollection.Get(typeof(Type), typeof(Type)));
			Assert.IsNull(mapperCollection.Get(typeof(string), typeof(Type)));
			Assert.IsNull(mapperCollection.Get(typeof(string), typeof(string)));
		}

		[TestMethod]
		public void Clear_Always_Succeeds()
		{
			//Arrange
			var mapperCollection = new MapperCollection();

			mapperCollection.Add(typeof(Type), typeof(Type), delegate(object source, object dest, TypeMappingContext context)
			{
			});

			//Act
			mapperCollection.Clear();

			//Assert
			Assert.IsNull(mapperCollection.Get(typeof(Type), typeof(Type)));
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Get_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new MapperCollection().Get(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Get_DestinationTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new MapperCollection().Get(typeof(Type), null);
		}

		[TestMethod]
		public void Get_GoodValues_ReturnsMapper()
		{
			//Arrange
			var mapperCollection = new MapperCollection();
			mapperCollection.Add(typeof(Type), typeof(Type),
				delegate(object source, object dest, TypeMappingContext context) { });

			//Act
			var value = mapperCollection.Get(typeof(Type), typeof(Type));
			var value2 = mapperCollection.Get(typeof(string), typeof(Type));
			var value3 = mapperCollection.Get(typeof(string), typeof(string));

			//Assert
			Assert.IsNotNull(value);
			Assert.IsNull(value2);
			Assert.IsNull(value3);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void GetBySourceType_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Type type = null;

			new MapperCollection().GetBySourceType(null, out type);
		}

		[TestMethod]
		public void GetBySourceType_GoodValues_ReturnsMapper()
		{
			//Arrange
			Type actual = null;

			var mapperCollection = new MapperCollection();

			mapperCollection.Add(typeof(MainEntity), typeof(MainEntityModel),
				delegate(object source, object dest, TypeMappingContext context)
				{
				});

			//Act
			var bySourceType = mapperCollection.GetBySourceType(typeof(MainEntity), out actual);

			//Assert
			Assert.IsNotNull(bySourceType);
			Assert.AreEqual<System.Type>(typeof(MainEntityModel), actual);
		}
	}
}
