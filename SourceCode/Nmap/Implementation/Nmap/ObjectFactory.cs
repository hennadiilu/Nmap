using System;
using System.Collections;
using System.Linq;
using Nmap.Internal;

namespace Nmap
{
	/// <summary>
	/// The factory for creation source and destination objects.
	/// </summary>
	public class ObjectFactory
	{
		#region Public Methods

		/// <summary>
		/// Creates the target object.
		/// </summary>
		/// <param name="from">The value from which the mapping is executed.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="context">The context.</param>
		/// <returns>The target object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="from" />
		/// or <paramref name="targetType" /> is null.</exception>
		public virtual object CreateTargetObject(object from, Type targetType, MappingContext context)
		{
			Error.ArgumentNullException_IfNull(from, "from");
			Error.ArgumentNullException_IfNull(targetType, "targetType");

			if (ReflectionHelper.IsAssignable(typeof(Array), targetType))
			{
				int size = from.GetType().IsArray ? ((Array)from).Length : ((IEnumerable)from).Cast<object>().Count<object>();

				return ReflectionHelper.CreateArray(targetType.GetElementType(), size);
			}
			else
			{
				if (ReflectionHelper.IsComplexEnumerable(targetType))
				{
					return ReflectionHelper.CreateList(targetType.GetGenericArguments()[0]);
				}
				else
				{
					return Activator.CreateInstance(targetType);
				}
			}
		}

		#endregion
	}
}
