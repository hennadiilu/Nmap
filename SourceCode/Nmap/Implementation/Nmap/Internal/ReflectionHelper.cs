using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nmap.Internal
{
	internal static class ReflectionHelper
	{
		#region Fields

		private const string AddItemMethodName = "Add";

		#endregion

		#region Public Methods

		public static bool IsSimple(Type type)
		{
			return type != null && (ReflectionHelper.IsAssignable(typeof(ValueType), type)
				|| ReflectionHelper.IsAssignable(typeof(string), type));
		}

		public static bool IsComplexEnumerable(Type type)
		{
			return ReflectionHelper.IsAssignable(typeof(IEnumerable), type)
				&& !ReflectionHelper.IsSimple(ReflectionHelper.GetEnumerableItemType(type));
		}

		public static bool IsSimpleEnumerable(Type type)
		{
			return ReflectionHelper.IsAssignable(typeof(IEnumerable), type)
				&& ReflectionHelper.IsSimple(ReflectionHelper.GetEnumerableItemType(type));
		}

		public static bool IsComplex(Type type)
		{
			return type != null && !ReflectionHelper.IsSimple(type)
				&& !ReflectionHelper.IsSimpleEnumerable(type) && !ReflectionHelper.IsComplexEnumerable(type);
		}

		public static bool IsAssignable(Type baseType, Type derivedType)
		{
			return baseType != null && derivedType != null && baseType.IsAssignableFrom(derivedType);
		}

		public static string GetMemberPath<T, TMember>(Expression<Func<T, TMember>> memberExpression)
		{
			if (memberExpression == null)
			{
				return string.Empty;
			}
			else
			{
				return string.Join(".", memberExpression.ToString().Split(new char[] { '.' }).Skip(1))
					.TrimStart(new char[] { '<', '(' }).TrimEnd(new char[] { '>', ')' });
			}
		}

		public static MemberInfo GetMemberInfo<T, TMember>(Expression<Func<T, TMember>> memberExpression)
		{
			return ReflectionHelper.GetMemberInfo(typeof(T), ReflectionHelper.GetMemberPath<T, TMember>(memberExpression));
		}

		public static MemberInfo GetMemberInfo(Type type, string memberPath)
		{
			if (type == null || string.IsNullOrEmpty(memberPath))
			{
				return null;
			}
			else
			{
				string[] array = memberPath.Split(new char[] { '.' });

				if (array.Length > 1)
				{
					string path = array.Skip(1).Aggregate((string a, string i) => a + "." + i);

					var memberInfo = type.GetMember(array[0]).FirstOrDefault<MemberInfo>();

					if (memberInfo is FieldInfo)
					{
						return ReflectionHelper.GetMemberInfo(((FieldInfo)memberInfo).FieldType, path);
					}
					else
					{
						if (memberInfo is PropertyInfo)
						{
							return ReflectionHelper.GetMemberInfo(((PropertyInfo)memberInfo).PropertyType, path);
						}

						return null;
					}
				}
				else
				{
					return type.GetMember(memberPath).FirstOrDefault<MemberInfo>();
				}
			}
		}

		public static object CreateArray(Type itemType, int size)
		{
			return Array.CreateInstance(itemType, size);
		}

		public static object CreateList(Type itemType)
		{
			return typeof(List<>).MakeGenericType(new Type[]
			{
				itemType
			}).GetConstructor(Type.EmptyTypes).Invoke(null);
		}

		public static Type GetEnumerableItemType(Type enumerableType)
		{
			if (enumerableType.IsArray)
			{
				return enumerableType.GetElementType();
			}
			else
			{
				if (ReflectionHelper.IsAssignable(typeof(IEnumerable), enumerableType))
				{
					return enumerableType.GetGenericArguments()[0];
				}
			}

			return null;
		}

		public static Action<object, object, int, MethodInfo> GetAddItemMethod(Type enumerableType)
		{
			if (enumerableType.IsArray)
			{
				return delegate(object collection, object item, int index, MethodInfo addMethod)
				{
					((Array)collection).SetValue(item, index);
				};
			}
			else
			{
				return delegate(object collection, object item, int index, MethodInfo addMethod)
				{
					addMethod.Invoke(collection, new object[]
					{
						item
					});
				};
			}
		}

		public static MethodInfo GetAddItemMethodInfo(Type enumerableType)
		{
			return enumerableType.GetMethod("Add");
		}

		public static Func<object, object> CreateGetter(Type type, PropertyInfo pi)
		{
			var parameterExpression = Expression.Parameter(typeof(object));

			var lambdaExpression = Expression.Lambda(Expression.Convert(Expression.Property(
				Expression.Convert(parameterExpression, type), pi.Name), typeof(object)), new ParameterExpression[]
			{
				parameterExpression
			});

			return (Func<object, object>)lambdaExpression.Compile();
		}

		public static Action<object, object> CreateSetter(Type type, PropertyInfo pi)
		{
			var setMethod = pi.GetSetMethod();
			var parameterExpression = Expression.Parameter(typeof(object));
			var parameterExpression2 = Expression.Parameter(typeof(object));

			var body = Expression.Call(Expression.Convert(parameterExpression, type), setMethod, new Expression[]
			{
				Expression.Convert(parameterExpression2, pi.PropertyType)
			});

			var expression = Expression.Lambda<Action<object, object>>(body, new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			});

			return expression.Compile();
		}

		#endregion
	}
}
