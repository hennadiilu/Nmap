using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;

namespace Nmap.Testing
{
	[TestClass]
	public class ReversivePropertyMapTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void ReversivePropertyMap_SourcePropertyInfoIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new ReversivePropertyMap(null);
		}

		[TestMethod]
		public void ReversivePropertyMap_GoodValues_Succeeds()
		{
			//Arrange
			var propertyInfo = ReflectionHelper.GetMemberInfo<string, int>((string s) => s.Length) as PropertyInfo;

			//Act
			var reversivePropertyMap = new ReversivePropertyMap(propertyInfo);

			//Assert
			Assert.AreEqual<string>("Length", reversivePropertyMap.SourcePropertyInfo.Name);
		}

		[TestMethod]
		public void ToString_Always_Succeeds()
		{

			//Arrange
			var propertyInfo = ReflectionHelper.GetMemberInfo<string, int>((string s) => s.Length) as PropertyInfo;

			//Act
			var reversivePropertyMap = new ReversivePropertyMap(propertyInfo);

			//Assert
			var actual = reversivePropertyMap.ToString();
			Assert.AreEqual<string>(string.Format("{{{0}, {1}}}", propertyInfo.Name, propertyInfo.ReflectedType), actual);
		}
	}
}
