using System;
using System.Collections.Generic;

namespace Nmap.Internal
{
	internal sealed class ConverterCollection
	{
		#region Fields

		private readonly IDictionary<Type, IDictionary<Type, Func<object, TypeMappingContext, object>>> converters;

		#endregion

		#region Constructors

		public ConverterCollection()
		{
			converters = new Dictionary<Type, IDictionary<Type, Func<object, TypeMappingContext, object>>>();
		}

		#endregion

		#region Public Methods

		public Func<object, TypeMappingContext, object> Get(Type sourceType, Type destinationType)
		{
			Error.ArgumentNullException_IfNull(sourceType, "sourceType");
			Error.ArgumentNullException_IfNull(destinationType, "destinationType");
			Error.ArgumentException_IfNotSimple(sourceType, "sourceType");
			Error.ArgumentException_IfNotSimple(destinationType, "destinationType");

			if (converters.ContainsKey(sourceType))
			{
				if (converters[sourceType].ContainsKey(destinationType))
				{
					return converters[sourceType][destinationType];
				}
			}

			return null;
		}

		public void Add<TSource, TDestination>(Func<TSource, TypeMappingContext, TDestination> converter)
		{
			Error.ArgumentNullException_IfNull(converter, "converter");
			Error.ArgumentException_IfNotSimple(typeof(TSource), "TSource");
			Error.ArgumentException_IfNotSimple(typeof(TDestination), "TDestination");

			Remove<TSource, TDestination>();

			if (!converters.ContainsKey(typeof(TSource)))
			{
				converters.Add(typeof(TSource), new Dictionary<Type, Func<object, TypeMappingContext, object>>());
			}

			converters[typeof(TSource)].Add(typeof(TDestination), (object sourceProperty, TypeMappingContext context)
				=> converter((sourceProperty == null) ? default(TSource) : ((TSource)((object)sourceProperty)), context));
		}

		public void Remove<TSource, TDestination>()
		{
			Error.ArgumentException_IfNotSimple(typeof(TSource), "TSource");
			Error.ArgumentException_IfNotSimple(typeof(TDestination), "TDestination");

			if (converters.ContainsKey(typeof(TSource)) && converters[typeof(TSource)].ContainsKey(typeof(TDestination)))
			{
				converters[typeof(TSource)].Remove(typeof(TDestination));
			}
		}

		public void Clear()
		{
			converters.Clear();
		}

		#endregion
	}
}
