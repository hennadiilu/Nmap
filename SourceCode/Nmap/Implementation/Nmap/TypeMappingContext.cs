namespace Nmap
{
	/// <summary>
	/// Represents the type mapping context.
	/// </summary>
	public sealed class TypeMappingContext
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nmap.TypeMappingContext" /> class.
		/// </summary>
		internal TypeMappingContext()
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the mapping context.
		/// </summary>
		public MappingContext MappingContext
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the object is mapping from.
		/// </summary>
		public object From
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the object is mapping to.
		/// </summary>
		public object To
		{
			get;
			internal set;
		}

		#endregion
	}
}
