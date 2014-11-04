using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Common;

namespace Nmap.Testing
{
	[TestClass]
	public class ObjectFactoryTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void CreateTargetObject_FromIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ObjectFactory().CreateTargetObject(null, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void CreateTargetObject_GoodValues_Succeeds()
		{
			//Arrange
			//Act
			object value = new ObjectFactory().CreateTargetObject(null, null, null);

			//Assert
			Assert.IsNotNull(value);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void CreateTargetObject_TargetTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ObjectFactory().CreateTargetObject(string.Empty, null, null);
		}

		[TestMethod]
		public void CreateTargetObject_TargetTypeIsComplexObject_ReturnsComplexObject()
		{
			//Arrange
			var objectFactory = new ObjectFactory();

			//Act
			object obj = objectFactory.CreateTargetObject(new MainEntity(), typeof(MainEntity), null);

			//Assert
			Assert.AreEqual<Type>(typeof(MainEntity), obj.GetType());
		}

		[TestMethod]
		public void CreateTargetObject_TargetTypeIsArray_ReturnsArray()
		{
			//Arrange
			var objectFactory = new ObjectFactory();

			List<MainEntity> list = new List<MainEntity>
			{
				new MainEntity(),
				new MainEntity()
			};

			//Act
			object obj = objectFactory.CreateTargetObject(list, typeof(MainEntity[]), null);

			//Assert
			Assert.AreEqual<System.Type>(typeof(MainEntity[]), obj.GetType());
			Assert.AreEqual<int>(list.Count, ((System.Array)obj).Length);

			foreach (object current in (System.Array)obj)
			{
				Assert.IsNull(current);
			}
		}

		[TestMethod]
		public void CreateTargetObject_TargetTypeIsCollection_ReturnsList()
		{
			//Arrange
			var objectFactory = new ObjectFactory();

			List<MainEntity> list = new List<MainEntity>
			{
				new MainEntity(),
				new MainEntity()
			};

			//Act
			object obj = objectFactory.CreateTargetObject(list, typeof(IEnumerable<MainEntity>), null);

			//Assert
			Assert.AreEqual<System.Type>(typeof(List<MainEntity>), obj.GetType());
			Assert.AreEqual<int>(0, ((List<MainEntity>)obj).Count);
		}
	}
}
