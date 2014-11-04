using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Nmap.Internal
{
	internal static class Mappers
	{
		#region Delegates

		public delegate Action<object, object, TypeMappingContext> GetMapperByFromType(Type fromType, out Type toType);

		#endregion

		#region Public Methods

		public static void MapComplexEnumerables(TypeMappingContext context, MapperCollection mappers, ObjectFactory objectFactory)
		{
			Type fromType = context.From.GetType();
			Type toType = context.To.GetType();

			var addItemMethod = ReflectionHelper.GetAddItemMethod(toType);

			var addItemMethodInfo = ReflectionHelper.GetAddItemMethodInfo(toType);

			int num = 0;

			foreach (object current in context.From as IEnumerable)
			{
				if (current == null)
				{
					addItemMethod(context.To, null, num, addItemMethodInfo);
					num++;
				}
				else
				{
					Type targetType = null;
					Type currentType = current.GetType();
					var bySourceType = mappers.GetBySourceType(currentType, out targetType);

					Error.MappingException_IfMapperIsNull(bySourceType, currentType);

					object obj = objectFactory.CreateTargetObject(current, targetType, context.MappingContext);

					var arg = new TypeMappingContext
					{
						From = current,
						To = obj,
						MappingContext = context.MappingContext
					};

					bySourceType(current, obj, arg);

					addItemMethod(context.To, obj, num, addItemMethodInfo);

					num++;
				}
			}
		}

		public static Action<object, object, TypeMappingContext> GetComplexPropertiesMapper(
			Func<object, object> fromPropertyGetter, Action<object, object> toPropertySetter,
			MapperCollection mappers, ObjectFactory objectFactory)
		{
			return delegate(object from, object to, TypeMappingContext context)
			{
				object obj = fromPropertyGetter(from);

				if (obj != null)
				{
					var type = obj.GetType();
					Type targetType = null;
					var bySourceType = mappers.GetBySourceType(type, out targetType);

					Error.MappingException_IfMapperIsNull(bySourceType, type);

					object toObject = objectFactory.CreateTargetObject(obj, targetType, context.MappingContext);

					var arg = new TypeMappingContext
					{
						From = obj,
						To = toObject,
						MappingContext = context.MappingContext
					};

					bySourceType(obj, toObject, arg);

					toPropertySetter(to, toObject);
				}
			};
		}

		public static Action<object, object, TypeMappingContext> GetComplexEnumerablePropertiesMapper(
			Type toEnumerableType, Func<object, object> fromEnumerableGetter,
			Action<object, object> toEnumerableSetter, ObjectFactory objectFactory, MapperCollection mappers)
		{
			return delegate(object from, object to, TypeMappingContext context)
			{
				object obj = fromEnumerableGetter(from);

				if (obj != null)
				{
					object targetObject = objectFactory.CreateTargetObject(obj, toEnumerableType, context.MappingContext);

					var ctx = new TypeMappingContext
					{
						From = obj,
						To = targetObject,
						MappingContext = context.MappingContext
					};

					Mappers.MapComplexEnumerables(ctx, mappers, objectFactory);

					toEnumerableSetter(to, targetObject);
				}
			};
		}

		public static Action<object, object, TypeMappingContext> GetSimplePropertiesMapper(
			PropertyInfo fromPropertyInfo, PropertyInfo toPropertyInfo, ConverterCollection converters,
			Func<object, object> fromPropertyGetter, Action<object, object> toPropertySetter)
		{
			var converter = converters.Get(fromPropertyInfo.PropertyType, toPropertyInfo.PropertyType);

			if (converter == null)
			{
				converter = ((object fromProperty, TypeMappingContext context) =>
					Convert.ChangeType(fromProperty, toPropertyInfo.PropertyType));
			}

			return delegate(object from, object to, TypeMappingContext context)
			{
				toPropertySetter(to, converter(fromPropertyGetter(from), context));
			};
		}

		public static Action<object, object, TypeMappingContext> GetSimpleEnumerablePropertiesMapper(
			PropertyInfo fromPropertyInfo, PropertyInfo toPropertyInfo, ObjectFactory objectFactory,
			ConverterCollection converters, Func<object, object> fromPropertyGetter, Action<object, object> toPropertySetter)
		{
			if (fromPropertyInfo.PropertyType == toPropertyInfo.PropertyType)
			{
				return delegate(object from, object to, TypeMappingContext ctxt)
				{
					toPropertySetter(to, fromPropertyGetter(from));
				};
			}
			else
			{
				Type enumerableItemType = ReflectionHelper.GetEnumerableItemType(fromPropertyInfo.PropertyType);
				Type toItemType = ReflectionHelper.GetEnumerableItemType(toPropertyInfo.PropertyType);
				var addMethod = ReflectionHelper.GetAddItemMethod(toPropertyInfo.PropertyType);
				var addMethodInfo = ReflectionHelper.GetAddItemMethodInfo(toPropertyInfo.PropertyType);
				var converter = converters.Get(enumerableItemType, toItemType);

				if (converter == null)
				{
					converter = ((object fromProperty, TypeMappingContext context) =>
						Convert.ChangeType(fromProperty, toItemType));
				}

				return delegate(object from, object to, TypeMappingContext context)
				{
					object obj = fromPropertyGetter(from);
					if (obj != null)
					{
						object targetObject = objectFactory.CreateTargetObject(
							obj, toPropertyInfo.PropertyType, context.MappingContext);

						int num = 0;

						foreach (object current in obj as IEnumerable)
						{
							addMethod(targetObject, converter(current, context), num, addMethodInfo);
							num++;
						}

						toPropertySetter(to, targetObject);
					}
				};
			}
		}

		public static Action<object, object, TypeMappingContext> GetMapperWithPropertyMappers(
			List<Action<object, object, TypeMappingContext>> propertyMappers)
		{
			return delegate(object from, object to, TypeMappingContext context)
			{
				propertyMappers.ForEach(delegate(Action<object, object, TypeMappingContext> m)
				{
					m(from, to, context);
				});
			};
		}

		public static Action<object, object, TypeMappingContext> GetMapperWithSimplePropertyMappers(
			Action<object, object, TypeMappingContext> propertyMapper, List<Action<object, object,
			TypeMappingContext>> simplePropertyMappers)
		{
			return delegate(object from, object to, TypeMappingContext context)
			{
				simplePropertyMappers.ForEach(delegate(Action<object, object, TypeMappingContext> spm)
				{
					spm(from, to, context);
				});
				if (propertyMapper != null)
				{
					propertyMapper(from, to, context);
				}
			};
		}

		public static Action<object, object, TypeMappingContext> GetMapperToRootType(
			Func<object, object> fromPropertyGetter, MapperCollection mappers)
		{
			return delegate(object from, object to, TypeMappingContext context)
			{
				object obj = fromPropertyGetter(from);

				if (obj != null)
				{
					var action = mappers.Get(obj.GetType(), context.To.GetType());

					Error.MappingException_IfMapperIsNull(action, obj.GetType());

					var arg = new TypeMappingContext
					{
						From = obj,
						To = to,
						MappingContext = context.MappingContext
					};

					action(obj, to, arg);
				}
			};
		}

		public static Action<object, object, TypeMappingContext> GetMapperFromRootType(
			Type toPropertyType, Action<object, object> toPropertySetter,
			MapperCollection mappers, ObjectFactory objectFactory)
		{
			return delegate(object from, object to, TypeMappingContext context)
			{
				var action = mappers.Get(from.GetType(), toPropertyType);

				Error.MappingException_IfMapperIsNull(action, from.GetType());

				object obj = objectFactory.CreateTargetObject(from, toPropertyType, context.MappingContext);

				var arg = new TypeMappingContext
				{
					From = from,
					To = obj,
					MappingContext = context.MappingContext
				};

				action(from, obj, arg);

				toPropertySetter(to, obj);
			};
		}

		#endregion
	}
}
