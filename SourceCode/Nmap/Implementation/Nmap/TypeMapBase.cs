using System;
using Nmap.Internal;

namespace Nmap
{
	/// <summary>
	/// Represents the base class for type maps.
	/// </summary>
	public abstract class TypeMapBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.TypeMapBase" /> class.
		/// </summary>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="destinationType">Type of the destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourceType" />
		/// or <paramref name="destinationType" /> is null.</exception>
		internal TypeMapBase(Type sourceType, Type destinationType)
		{
			Error.ArgumentNullException_IfNull(sourceType, "sourceType");
			Error.ArgumentNullException_IfNull(destinationType, "destinationType");

			SourceType = sourceType;
			DestinationType = destinationType;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the type of the source.
		/// </summary>
		public Type SourceType
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the type of the destination.
		/// </summary>
		public Type DestinationType
		{
			get;
			private set;
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
		/// Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{{{0}, {1}}}", SourceType.FullName, DestinationType.FullName);
		}

		#endregion
	}
}
