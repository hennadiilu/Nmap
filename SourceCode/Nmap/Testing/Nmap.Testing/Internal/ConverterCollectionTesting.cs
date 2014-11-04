using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;

namespace Nmap.Testing.Internal
{
	[TestClass]
	public class ConverterCollectionTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Add_ConverterIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Add<string, string>(null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Add_TSourceIsNotSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Add<System.Type, System.Type>((System.Type source, TypeMappingContext context) => null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Add_TDestinationIsNotSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Add<string, System.Type>((string source, TypeMappingContext context) => null);
		}

		[TestMethod]
		public void Add_GoodValues_Succeeds()
		{
			//Arrange
			var converterCollection = new ConverterCollection();

			//Act
			converterCollection.Add<string, string>((string source, TypeMappingContext context) => null);

			//Assert
			Assert.IsNotNull(converterCollection.Get(typeof(string), typeof(string)));
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Get_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Get(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Get_DestinationTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Get(typeof(string), null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Get_SourceTypeIsNotSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Get(typeof(Type), typeof(string));
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Get_DestinationTypeIsNotSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Get(typeof(string), typeof(Type));
		}

		[TestMethod]
		public void Get_GoodValues_ReturnsConverter()
		{
			//Arrange
			var converterCollection = new ConverterCollection();
			converterCollection.Add<string, string>((string source, TypeMappingContext context) => null);

			//Act
			var value = converterCollection.Get(typeof(string), typeof(string));

			//Assert
			Assert.IsNotNull(value);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Remove_TSourceIsNotSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Remove<System.Type, System.Type>();
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Remove_TDestinationIsNotSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			new ConverterCollection().Remove<string, System.Type>();
		}

		[TestMethod]
		public void Remove_GoodValues_Succeeds()
		{
			//Arrange
			var converterCollection = new ConverterCollection();
			converterCollection.Add<string, string>((string source, TypeMappingContext context) => null);

			//Act
			converterCollection.Remove<string, string>();

			//Assert
			Assert.IsNull(converterCollection.Get(typeof(string), typeof(string)));
		}
	}
}
