using System;
using Nmap.Internal;

namespace Nmap
{
	/// <summary>
	/// Represents the reversive type map configuration.
	/// </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <typeparam name="TDestination">The type of the destination.</typeparam>
	public sealed class ReversiveTypeMapConfiguration<TSource, TDestination> : ReversiveTypeMapPropertyConfiguration<TSource, TDestination>
		where TSource : class
		where TDestination : class
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.ReversiveTypeMapConfiguration`2" /> class.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="map" /> is null.</exception>
		internal ReversiveTypeMapConfiguration(ReversiveTypeMap map)
			: base(map)
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the specified mapper.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="unMapper">The un mapper.</param>
		/// <returns>The reversive type map.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="mapper" />
		/// or <paramref name="unMapper" /> is null.</exception>
		public ReversiveTypeMap As(Action<TSource, TDestination, TypeMappingContext> mapper, Action<TDestination, TSource, TypeMappingContext> unMapper)
		{
			Error.ArgumentNullException_IfNull(mapper, "mapper");
			Error.ArgumentNullException_IfNull(unMapper, "unMapper");
			base.Map.Mapper = delegate(object source, object destination, TypeMappingContext context)
			{
				mapper(source as TSource, destination as TDestination, context);
			};
			base.Map.UnMapper = delegate(object destination, object source, TypeMappingContext context)
			{
				unMapper(destination as TDestination, source as TSource, context);
			};
			return base.Map;
		}

		#endregion
	}
}
