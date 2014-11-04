using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nmap.Testing
{
	public partial class MapperTesting
	{
		[TestMethod]
		public void GetMap_IfMapExists_ReturnsMap()
		{
			//Arrange
			var typeMap = new TypeMap(typeof(Type), typeof(Type));
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(typeMap);

			//Act
			var map = Mapper<MapperTesting.MapperTester>.Instance.GetMap<Type, Type>();

			//Assert
			Assert.AreEqual<TypeMapBase>(typeMap, map);
		}

		[TestMethod]
		public void GetMap_IfMapNotExists_ReturnsNull()
		{
			//Arrange
			var typeMap = new TypeMap(typeof(Type), typeof(Type));

			Mapper<MapperTesting.MapperTester>.Instance.AddMap(typeMap);

			//Act
			var map = Mapper<MapperTesting.MapperTester>.Instance.GetMap<Type, string>();
			var map2 = Mapper<MapperTesting.MapperTester>.Instance.GetMap<string, string>();

			//Assert
			Assert.IsNull(map);
			Assert.IsNull(map2);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void AddMap_MapIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(null);
		}

		[TestMethod]
		public void AddMap_GoodValues_Succeeds()
		{
			//Arrange
			//Act
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(new TypeMap(typeof(Type), typeof(Type)));
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(new ReversiveTypeMap(typeof(Type), typeof(string)));

			//Assert
			Assert.IsNotNull(Mapper<MapperTesting.MapperTester>.Instance.GetMap<Type, Type>());
			Assert.IsNotNull(Mapper<MapperTesting.MapperTester>.Instance.GetMap<Type, string>());
		}

		[TestMethod]
		public void RemoveMap_GoodValues_Succeeds()
		{
			//Arrange
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(new TypeMap(typeof(Type), typeof(Type)));

			//Act
			Mapper<MapperTesting.MapperTester>.Instance.RemoveMap<Type, Type>();
			Mapper<MapperTesting.MapperTester>.Instance.RemoveMap<Type, string>();
			Mapper<MapperTesting.MapperTester>.Instance.RemoveMap<string, Type>();

			//Assert
			Assert.IsNull(Mapper<MapperTesting.MapperTester>.Instance.GetMap<Type, Type>());
			Assert.IsNull(Mapper<MapperTesting.MapperTester>.Instance.GetMap<Type, string>());
			Assert.IsNull(Mapper<MapperTesting.MapperTester>.Instance.GetMap<string, Type>());
		}

		[TestMethod]
		public void ClearMaps_Always_Succeeds()
		{
			//Arrange
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(new TypeMap(typeof(Type), typeof(Type)));
			Mapper<MapperTesting.MapperTester>.Instance.AddMap(new TypeMap(typeof(Type), typeof(string)));

			//Act
			Mapper<MapperTesting.MapperTester>.Instance.ClearMaps();

			//Assert
			Assert.IsNull(Mapper<MapperTesting.MapperTester>.Instance.GetMap<Type, Type>());
			Assert.IsNull(Mapper<MapperTesting.MapperTester>.Instance.GetMap<Type, string>());
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void SetObjectFactory_FactoryIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.SetObjectFactory(null);
		}

		[TestMethod]
		public void SetObjectFactory_GoodValues_Succeeds()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.SetObjectFactory(new ObjectFactory());
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void AddConverter_ConverterIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.AddConverter<string, string>(null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void AddConverter_TSourcePropertyIsComplex_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.AddConverter<Type, string>((Type source, TypeMappingContext dest) => null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void AddConverter_TSourcePropertyIsEnumerable_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.AddConverter<List<string>, string>(
				(List<string> source, TypeMappingContext dest) => null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void AddConverter_TDestinationPropertyIsComplex_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.AddConverter<string, Type>((string source, TypeMappingContext dest) => null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void AddConverter_TDestinationPropertyIsEnumerable_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.AddConverter<string, List<string>>((string source, TypeMappingContext dest) => null);
		}

		[TestMethod]
		public void AddConverter_GoodValues_Succeeds()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.AddConverter<string, string>((string source, TypeMappingContext context) => null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void RemoveConverter_TSourcePropertyIsComplex_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.RemoveConverter<Type, string>();
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void RemoveConverter_TSourcePropertyIsEnumerable_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.RemoveConverter<List<string>, string>();
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void RemoveConverter_TDestinationPropertyIsComplex_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.RemoveConverter<string, Type>();
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void RemoveConverter_TDestinationPropertyIsEnumerable_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.RemoveConverter<string, List<string>>();
		}

		[TestMethod]
		public void ClearConverters_Always_Succeeds()
		{
			//Arrange
			Mapper<MapperTesting.MapperTester>.Instance.AddConverter<string, int>((string source, TypeMappingContext dest) => 0);

			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.ClearConverters();
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Map_Object_MappingContext_SourceIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map<Type>(null, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Map_Object_MappingContext_SourceIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map<Type>(string.Empty, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Map_Object_MappingContext_TDestinationIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map<string>(new Type[0], null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Map_Object_Type_MappingContext_SourceIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map(null, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Map_Object_Type_MappingContext_DestinationTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map(new Type[0], null, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Map_Object_Type_MappingContext_SourceIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map(string.Empty, typeof(Type[]), null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Map_Object_Type_MappingContext_DestinationTypeIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map(new Type[0], typeof(int), null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Map_Object_TDestination_MappingContext_SourceIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map<Type[]>(null, new Type[0], null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Map_Object_TDestination_MappingContext_DestinationIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map<Type[]>(new Type[0], null, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Map_Object_TDestination_MappingContext_SourceIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map<Type[]>(string.Empty, new Type[0], null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Map_Object_TDestination_MappingContext_DestinationIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Map<string>(typeof(Type), string.Empty, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Unmap_Object_MappingContext_DestinationIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap<Type>(null, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Unmap_Object_MappingContext_DestinationIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap<Type>(string.Empty, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Unmap_Object_MappingContext_TSourceIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap<string>(new Type[0], null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Unmap_Object_Type_MappingContext_DestinationIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap(null, null, null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Unmap_Object_Type_MappingContext_SourceTypeIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap(new Type[0], null, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Unmap_Object_Type_MappingContext_DestinationIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap(string.Empty, typeof(Type[]), null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Unmap_Object_Type_MappingContext_SourceTypeIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap(new Type[0], typeof(int), null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Unmap_Object_TSource_MappingContext_DestinationIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap<Type[]>(null, new Type[0], null);
		}

		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void Unmap_Object_TSource_MappingContext_SourceIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap<Type[]>(new Type[0], null, null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Unmap_Object_TSource_MappingContext_DestinationIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap<Type[]>(string.Empty, new Type[0], null);
		}

		[ExpectedException(typeof(ArgumentException)), TestMethod]
		public void Unmap_Object_TSource_MappingContext_SourceIsSimple_ThrowsArgumentException()
		{
			//Arrange
			//Act
			//Assert
			Mapper<MapperTesting.MapperTester>.Instance.Unmap<string>(typeof(Type), string.Empty, null);
		}
	}
}
