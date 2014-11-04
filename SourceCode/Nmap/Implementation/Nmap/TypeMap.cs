using System;
using System.Collections.Generic;

namespace Nmap
{
	/// <summary>
	/// Represents the type map.
	/// </summary>
	public sealed class TypeMap : TypeMapBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.TypeMap" /> class.
		/// </summary>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="destinationType">Type of the destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourceType" />
		/// or <paramref name="destinationType" /> is null.</exception>
		public TypeMap(Type sourceType, Type destinationType)
			: base(sourceType, destinationType)
		{
			PropertyMaps = new List<PropertyMap>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the property maps.
		/// </summary>
		public ICollection<PropertyMap> PropertyMaps
		{
			get;
			private set;
		}

		#endregion
	}
}
