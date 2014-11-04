using System;
using System.Reflection;
using Nmap.Internal;

namespace Nmap
{
	/// <summary>
	/// Represents the base class for property maps.
	/// </summary>
	public abstract class PropertyMapBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.PropertyMapBase" /> class.
		/// </summary>
		/// <param name="sourcePropertyInfo">The source property information.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourcePropertyInfo" /> is null.</exception>
		internal PropertyMapBase(PropertyInfo sourcePropertyInfo)
		{
			Error.ArgumentNullException_IfNull(sourcePropertyInfo, "sourcePropertyInfo");
			SourcePropertyInfo = sourcePropertyInfo;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the source property information.
		/// </summary>
		public PropertyInfo SourcePropertyInfo
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the destination property information.
		/// </summary>
		public PropertyInfo DestinationPropertyInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the mapper.
		/// </summary>
		public Action<object, object, TypeMappingContext> Mapper
		{
			get;
			set;
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			PropertyMapBase propertyMapBase = obj as PropertyMapBase;
			return propertyMapBase != null && propertyMapBase.SourcePropertyInfo.Equals(SourcePropertyInfo);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return SourcePropertyInfo.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{{{0}, {1}}}", SourcePropertyInfo.Name, SourcePropertyInfo.ReflectedType);
		}

		#endregion
	}
}
