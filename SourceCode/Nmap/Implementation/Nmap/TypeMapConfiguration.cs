using System;
using Nmap.Internal;

namespace Nmap
{
	/// <summary>
	/// Represents the type map configuration.
	/// </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <typeparam name="TDestination">The type of the destination.</typeparam>
	public sealed class TypeMapConfiguration<TSource, TDestination> : TypeMapPropertyConfiguration<TSource, TDestination>
		where TSource : class
		where TDestination : class
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.TypeMapConfiguration`2" /> class.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="map" /> is null.</exception>
		internal TypeMapConfiguration(TypeMap map)
			: base(map)
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the specified mapper.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <returns>The type map.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="mapper" /> is null.</exception>
		public TypeMap As(Action<TSource, TDestination, TypeMappingContext> mapper)
		{
			Error.ArgumentNullException_IfNull(mapper, "mapper");

			base.Map.Mapper = delegate(object source, object destination, TypeMappingContext context)
			{
				mapper(source as TSource, destination as TDestination, context);
			};

			return base.Map;
		}

		#endregion
	}
}
