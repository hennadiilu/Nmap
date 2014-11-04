using System;
using System.Collections.Generic;

namespace Nmap
{
	/// <summary>
	/// Represents the reversive type map.
	/// </summary>
	public sealed class ReversiveTypeMap : TypeMapBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.ReversiveTypeMap" /> class.
		/// </summary>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="destinationType">Type of the destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourceType" />
		/// or <paramref name="destinationType" /> is null.</exception>
		public ReversiveTypeMap(Type sourceType, Type destinationType)
			: base(sourceType, destinationType)
		{
			PropertyMaps = new List<ReversivePropertyMap>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the property maps.
		/// </summary>
		public ICollection<ReversivePropertyMap> PropertyMaps
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets or sets the un mapper.
		/// </summary>
		public Action<object, object, TypeMappingContext> UnMapper
		{
			get;
			set;
		}

		#endregion
	}
}
