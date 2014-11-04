using System.Collections.Generic;
using System.Reflection;

namespace Nmap
{
	/// <summary>
	/// Represents the property map.
	/// </summary>
	public sealed class PropertyMap : PropertyMapBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.PropertyMap" /> class.
		/// </summary>
		/// <param name="sourcePropertyInfo">The source property information.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourcePropertyInfo" /> is null.</exception>
		public PropertyMap(PropertyInfo sourcePropertyInfo)
			: base(sourcePropertyInfo)
		{
			InheritanceMaps = new List<TypeMap>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the inheritance maps.
		/// </summary>
		public ICollection<TypeMap> InheritanceMaps
		{
			get;
			private set;
		}

		#endregion
	}
}
