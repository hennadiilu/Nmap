using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;

namespace Nmap.Testing
{
	[TestClass]
	public class PropertyMapTesting
	{
		[ExpectedException(typeof(ArgumentNullException)), TestMethod]
		public void PropertyMap_SourcePropertyInfoIsNull_ThrowsArgumentNullException()
		{
			//Arrange
			//Act
			//Assert
			new PropertyMap(null);
		}

		[TestMethod]
		public void PropertyMap_GoodValues_Succeeds()
		{
			//Arrange
			var propertyInfo = ReflectionHelper.GetMemberInfo<string, int>((string s) => s.Length) as PropertyInfo;

			//Act
			var propertyMap = new PropertyMap(propertyInfo);

			//Assert
			Assert.AreEqual<string>("Length", propertyMap.SourcePropertyInfo.Name);
		}

		[TestMethod]
		public void ToString_Always_Succeeds()
		{
			//Arrange
			var propertyInfo = ReflectionHelper.GetMemberInfo<string, int>((string s) => s.Length) as PropertyInfo;

			//Act
			var propertyMap = new PropertyMap(propertyInfo);

			//Assert
			string actual = propertyMap.ToString();
			Assert.AreEqual<string>(string.Format("{{{0}, {1}}}", propertyInfo.Name, propertyInfo.ReflectedType), actual);
		}
	}
}
