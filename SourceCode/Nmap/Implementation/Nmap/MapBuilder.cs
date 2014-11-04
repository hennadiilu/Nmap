using System;

namespace Nmap
{
	/// <summary>
	/// Provides functionality for building maps.
	/// </summary>
	public sealed class MapBuilder : TypeMapCreating
	{
		#region Constructors

		/// <summary>
		/// Prevents a default instance of the <see cref="T:Nmap.MapBuilder" /> class from being created.
		/// </summary>
		private MapBuilder()
		{
		}

		/// <summary>
		/// Initializes the <see cref="T:Nmap.MapBuilder" /> class.
		/// </summary>
		static MapBuilder()
		{
			MapBuilder.Instance = new MapBuilder();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static MapBuilder Instance
		{
			get;
			private set;
		}

		#endregion
	}
}
