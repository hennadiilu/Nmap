using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nmap.Internal
{
	internal sealed class TypeMapCollection : IEnumerable<TypeMapBase>, IEnumerable
	{
		#region Fields

		private IList<TypeMapBase> maps;

		#endregion

		#region Constructors

		public TypeMapCollection()
		{
			maps = new List<TypeMapBase>();
		}

		#endregion

		#region Public Methods

		public TypeMapBase Get(Type sourceType, Type destinationType)
		{
			Error.ArgumentNullException_IfNull(sourceType, "sourceType");
			Error.ArgumentNullException_IfNull(destinationType, "destinationType");

			return maps.FirstOrDefault((TypeMapBase m) => m.SourceType
				== sourceType && m.DestinationType == destinationType);
		}

		public void Add(TypeMapBase map)
		{
			Error.ArgumentNullException_IfNull(map, "map");
			maps.Add(map);
		}

		public void Remove(Type sourceType, Type destinationType)
		{
			Error.ArgumentNullException_IfNull(sourceType, "sourceType");
			Error.ArgumentNullException_IfNull(destinationType, "destinationType");

			TypeMapBase item = maps.FirstOrDefault((TypeMapBase m) =>
				m.SourceType == sourceType && m.DestinationType == destinationType);

			maps.Remove(item);
		}

		public void Clear()
		{
			maps.Clear();
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<TypeMapBase> GetEnumerator()
		{
			return maps.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
