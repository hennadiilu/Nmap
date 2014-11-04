namespace Nmap
{
	/// <summary>
	/// Represents the type map creating.
	/// </summary>
	public class TypeMapCreating
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.TypeMapCreating" /> class.
		/// </summary>
		internal TypeMapCreating()
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates the map.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <typeparam name="TDestination">The type of the destination.</typeparam>
		/// <returns>The type map configuration.</returns>
		public TypeMapConfiguration<TSource, TDestination> CreateMap<TSource, TDestination>()
			where TSource : class
			where TDestination : class
		{
			return new TypeMapConfiguration<TSource, TDestination>(new TypeMap(typeof(TSource), typeof(TDestination)));
		}

		/// <summary>
		/// Creates the reversive map.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <typeparam name="TDestination">The type of the destination.</typeparam>
		/// <returns>The reversive type map configuration.</returns>
		public ReversiveTypeMapConfiguration<TSource, TDestination> CreateReversiveMap<TSource, TDestination>()
			where TSource : class
			where TDestination : class
		{
			return new ReversiveTypeMapConfiguration<TSource, TDestination>(
				new ReversiveTypeMap(typeof(TSource), typeof(TDestination)));
		}

		#endregion
	}
}
