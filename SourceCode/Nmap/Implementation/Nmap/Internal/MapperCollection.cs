using System;
using System.Collections.Generic;
using System.Linq;

namespace Nmap.Internal
{
	internal sealed class MapperCollection
	{
		#region Fields

		private readonly IDictionary<Type, IDictionary<Type, Action<object, object, TypeMappingContext>>> mappers;

		#endregion

		#region Constructors

		public MapperCollection()
		{
			mappers = new Dictionary<Type, IDictionary<Type, Action<object, object, TypeMappingContext>>>();
		}

		#endregion

		#region Public Methods

		public void Add(Type sourceType, Type destinationType, Action<object, object, TypeMappingContext> mapper)
		{
			Error.ArgumentNullException_IfNull(sourceType, "sourceType");
			Error.ArgumentNullException_IfNull(destinationType, "destinationType");
			Error.ArgumentNullException_IfNull(mapper, "mapper");

			if (!mappers.ContainsKey(sourceType))
			{
				mappers.Add(sourceType, new Dictionary<Type, Action<object, object, TypeMappingContext>>());
			}

			mappers[sourceType].Add(destinationType, mapper);
		}

		public Action<object, object, TypeMappingContext> Get(Type sourceType, Type destinationType)
		{
			Error.ArgumentNullException_IfNull(sourceType, "sourceType");
			Error.ArgumentNullException_IfNull(destinationType, "destinationType");

			if (mappers.ContainsKey(sourceType))
			{
				if (mappers[sourceType].ContainsKey(destinationType))
				{
					return mappers[sourceType][destinationType];
				}
			}

			return null;
		}

		public Action<object, object, TypeMappingContext> GetBySourceType(Type sourceType, out Type destinationType)
		{
			Error.ArgumentNullException_IfNull(sourceType, "sourceType");

			if (mappers.ContainsKey(sourceType))
			{
				destinationType = mappers[sourceType].Keys.FirstOrDefault<Type>();

				if (destinationType != null)
				{
					return mappers[sourceType][destinationType];
				}
			}

			destinationType = null;

			return null;
		}

		public void Clear()
		{
			mappers.Clear();
		}

		#endregion
	}
}
