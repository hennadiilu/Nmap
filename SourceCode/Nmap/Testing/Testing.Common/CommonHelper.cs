using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Nmap.Internal;

namespace Testing.Common
{
	public static class CommonHelper
	{
		public static TEntity CreateEntityWithSimpleProperties<TEntity>() where TEntity : Entity, new()
		{
			return CommonHelper.CreateEntityWithSimpleProperties(typeof(TEntity)) as TEntity;
		}

		public static MainEntity CreateMainEntityWithSimpleProperties()
		{
			return CommonHelper.CreateEntityWithSimpleProperties(typeof(MainEntity)) as MainEntity;
		}

		public static DerivedMainEntity CreateDerivedMainEntityWithSimpleProperties()
		{
			return CommonHelper.CreateEntityWithSimpleProperties(typeof(DerivedMainEntity)) as DerivedMainEntity;
		}

		public static MainEntity CreateMainEntityWithAllProperties(bool createDerived)
		{
			MainEntity mainEntity = CommonHelper.CreateMainEntityWithSimpleProperties();

			CommonHelper.FillEntityWithNestedProperties(mainEntity, createDerived);

			return mainEntity;
		}

		private static void FillEntityWithNestedProperties(Entity entity, bool createDerived)
		{
			var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < properties.Length; i++)
			{
				var propertyInfo = properties[i];

				if (ReflectionHelper.IsComplex(propertyInfo.PropertyType))
				{
					Type type = propertyInfo.PropertyType;

					if (createDerived)
					{
						type = entity.GetType().Assembly.GetType(type.FullName.Replace(type.Name, "Derived" + type.Name));
					}

					Entity entity2 = CommonHelper.CreateEntityWithSimpleProperties(type);

					CommonHelper.FillEntityWithNestedProperties(entity2, createDerived);

					propertyInfo.SetValue(entity, entity2, null);
				}
				else
				{
					if (ReflectionHelper.IsComplexEnumerable(propertyInfo.PropertyType))
					{
						var addItemMethod = ReflectionHelper.GetAddItemMethod(propertyInfo.PropertyType);

						MethodInfo addItemMethodInfo = ReflectionHelper.GetAddItemMethodInfo(propertyInfo.PropertyType);

						Type enumerableItemType = ReflectionHelper.GetEnumerableItemType(propertyInfo.PropertyType);

						object obj = null;

						if (ReflectionHelper.IsAssignable(typeof(Array), propertyInfo.PropertyType))
						{
							obj = ReflectionHelper.CreateArray(enumerableItemType, 3);
						}
						else
						{
							if (ReflectionHelper.IsAssignable(typeof(IList), propertyInfo.PropertyType))
							{
								obj = ReflectionHelper.CreateList(enumerableItemType);
							}
						}

						for (int j = 0; j < 3; j++)
						{
							Type type2 = enumerableItemType;

							if (createDerived && j % 2 == 0)
							{
								type2 = entity.GetType().Assembly.GetType(type2.FullName.Replace(type2.Name, "Derived" + type2.Name));
							}

							Entity entity3 = CommonHelper.CreateEntityWithSimpleProperties(type2);

							CommonHelper.FillEntityWithNestedProperties(entity3, createDerived);

							addItemMethod(obj, entity3, j, addItemMethodInfo);
						}

						propertyInfo.SetValue(entity, obj, null);
					}
				}
			}
		}
		private static Entity CreateEntityWithSimpleProperties(Type entityType)
		{
			Entity entity = Activator.CreateInstance(entityType) as Entity;
			int num = 1;
			int num2 = 1;

			var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];

				if (propertyInfo.PropertyType == typeof(int))
				{
					propertyInfo.SetValue(entity, num, null);
					num *= 2;
				}
				else
				{
					if (propertyInfo.PropertyType == typeof(int[]))
					{
						propertyInfo.SetValue(entity, new int[]
						{
							num2++,
							num2++,
							num2++
						}, null);
					}
					else
					{
						if (propertyInfo.PropertyType == typeof(List<int>))
						{
							propertyInfo.SetValue(entity, new List<int>
							{
								num2++,
								num2++,
								num2++
							}, null);
						}
					}
				}
			}

			return entity;
		}
	}
}
