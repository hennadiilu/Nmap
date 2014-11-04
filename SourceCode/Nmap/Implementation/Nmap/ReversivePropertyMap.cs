using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nmap
{
	/// <summary>
	/// Represents th reversive property map.
	/// </summary>
	public sealed class ReversivePropertyMap : PropertyMapBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.ReversivePropertyMap" /> class.
		/// </summary>
		/// <param name="sourcePropertyInfo">The source property information.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourcePropertyInfo" /> is null.</exception>
		public ReversivePropertyMap(PropertyInfo sourcePropertyInfo)
			: base(sourcePropertyInfo)
		{
			InheritanceMaps = new List<ReversiveTypeMap>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the inheritance maps.
		/// </summary>
		public ICollection<ReversiveTypeMap> InheritanceMaps
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
