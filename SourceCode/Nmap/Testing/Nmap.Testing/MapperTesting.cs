using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nmap.Internal;
using Testing.Common;

namespace Nmap.Testing
{
	[TestClass]
	public partial class MapperTesting
	{
		private sealed class MapperTester : Mapper<MapperTesting.MapperTester>
		{
		}

		private sealed class CustomMappingContext : MappingContext
		{
			public string Data
			{
				get;
				set;
			}
		}

		private MainEntityModel[] ActMapping(MainEntity entity, MappingContext context)
		{
			var mainEntityModel = Mapper<MapperTesting.MapperTester>.Instance.Map<MainEntityModel>(entity, context);

			var mainEntityModel2 = Mapper<MapperTesting.MapperTester>.Instance.Map<MainEntityModel>(entity, new MainEntityModel(), context);

			var mainEntityModel3 = Mapper<MapperTesting.MapperTester>.Instance.Map(entity, typeof(MainEntityModel), context) as MainEntityModel;

			return new MainEntityModel[]
			{
				mainEntityModel,
				mainEntityModel2,
				mainEntityModel3
			};
		}

		private IEnumerable<MainEntityModel>[] ActEnumerableMapping(MainEntity[] entities, MappingContext context)
		{
			var enumerable = Mapper<MapperTesting.MapperTester>.Instance.Map<IEnumerable<MainEntityModel>>(entities, context);

			var array = Mapper<MapperTesting.MapperTester>.Instance.Map<MainEntityModel[]>(entities, context);

			var list = Mapper<MapperTesting.MapperTester>.Instance.Map<List<MainEntityModel>>(entities, new List<MainEntityModel>(), context);

			var array2 = Mapper<MapperTesting.MapperTester>.Instance.Map<MainEntityModel[]>(entities, new MainEntityModel[2], context);

			var enumerable2 = Mapper<MapperTesting.MapperTester>.Instance.Map(entities, typeof(List<MainEntityModel>), context) as IEnumerable<MainEntityModel>;

			var enumerable3 = Mapper<MapperTesting.MapperTester>.Instance.Map(entities, typeof(MainEntityModel[]), context) as IEnumerable<MainEntityModel>;

			return new IEnumerable<MainEntityModel>[]
			{
				enumerable,
				array,
				list,
				array2,
				enumerable2,
				enumerable3
			};
		}

		private MainEntity[] ActUnmapping(MainEntity entity, MappingContext context)
		{
			var mainEntity = Mapper<MapperTesting.MapperTester>.Instance.Unmap<MainEntity>(
				Mapper<MapperTesting.MapperTester>.Instance.Map<MainEntityModel>(entity, context), context);

			var mainEntity2 = Mapper<MapperTesting.MapperTester>.Instance.Unmap<MainEntity>(
				Mapper<MapperTesting.MapperTester>.Instance.Map<MainEntityModel>(entity,
				new MainEntityModel(), context), new MainEntity(), context);

			var mainEntity3 = Mapper<MapperTesting.MapperTester>.Instance.Unmap(
				Mapper<MapperTesting.MapperTester>.Instance.Map(entity, typeof(MainEntityModel), context),
				typeof(MainEntity), context) as MainEntity;

			return new MainEntity[]
			{
				mainEntity,
				mainEntity2,
				mainEntity3
			};
		}

		private IEnumerable<MainEntity>[] ActEnumerableUnmapping(MainEntity[] entities, MappingContext context)
		{
			var array = Mapper<MapperTesting.MapperTester>.Instance.Unmap<MainEntity[]>(
				Mapper<MapperTesting.MapperTester>.Instance.Map<IEnumerable<MainEntityModel>>(entities, context), context);
			var array2 = Mapper<MapperTesting.MapperTester>.Instance.Unmap<MainEntity[]>(
				Mapper<MapperTesting.MapperTester>.Instance.Map<MainEntityModel[]>(entities, context), context);
			var array3 = Mapper<MapperTesting.MapperTester>.Instance.Unmap<MainEntity[]>(
				Mapper<MapperTesting.MapperTester>.Instance.Map<List<MainEntityModel>>(entities,
				new List<MainEntityModel>(), context), new MainEntity[2], context);
			var array4 = Mapper<MapperTesting.MapperTester>.Instance.Unmap<MainEntity[]>(
				Mapper<MapperTesting.MapperTester>.Instance.Map<MainEntityModel[]>(
				entities, new MainEntityModel[2], context), new MainEntity[2], context);
			var array5 = Mapper<MapperTesting.MapperTester>.Instance.Unmap(
				Mapper<MapperTesting.MapperTester>.Instance.Map(entities, typeof(List<MainEntityModel>), context),
				typeof(MainEntity[]), context) as MainEntity[];
			var array6 = Mapper<MapperTesting.MapperTester>.Instance.Unmap(Mapper<MapperTesting.MapperTester>
				.Instance.Map(entities, typeof(MainEntityModel[]), context), typeof(MainEntity[]), context) as MainEntity[];

			return new IEnumerable<MainEntity>[]
			{
				array,
				array2,
				array3,
				array4,
				array5,
				array6
			};
		}

		private void AssertEntity(Entity expected, Entity actual)
		{
			var xmlSerializer = new XmlSerializer(expected.GetType(), new Type[]
			{
				typeof(DerivedMainEntity),
				typeof(DerivedSubEntity),
				typeof(DerivedSubSubEntity)
			});

			var memoryStream = new MemoryStream();

			xmlSerializer.Serialize(memoryStream, expected);

			memoryStream.Seek(0L, SeekOrigin.Begin);

			string expected2 = new StreamReader(memoryStream).ReadToEnd();

			memoryStream.Close();

			xmlSerializer = new XmlSerializer(actual.GetType(), new Type[]
			{
				typeof(DerivedMainEntity),
				typeof(DerivedSubEntity),
				typeof(DerivedSubSubEntity)
			});

			memoryStream = new MemoryStream();

			xmlSerializer.Serialize(memoryStream, actual);

			memoryStream.Seek(0L, SeekOrigin.Begin);

			string actual2 = new StreamReader(memoryStream).ReadToEnd();

			memoryStream.Close();

			Assert.AreEqual<string>(expected2, actual2);
		}

		private void AssertEntityModel(Entity entity, EntityModel model)
		{
			Type type = model.GetType();

			var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < properties.Length; i++)
			{
				var propertyInfo = properties[i];

				var property = type.GetProperty("Entity" + propertyInfo.Name);

				if (property == null)
				{
					property = type.GetProperty(propertyInfo.Name, BindingFlags.Instance | BindingFlags.Public);
				}

				if (propertyInfo.GetValue(entity, null) == null)
				{
					Assert.IsNull(property.GetValue(model, null));
				}
				else
				{
					if (ReflectionHelper.IsSimple(propertyInfo.PropertyType))
					{
						Assert.AreEqual<string>(propertyInfo.GetValue(entity, null).ToString(), property.GetValue(model, null).ToString());
					}
					else
					{
						if (ReflectionHelper.IsSimpleEnumerable(propertyInfo.PropertyType))
						{
							object[] array = ((IEnumerable)propertyInfo.GetValue(entity, null)).Cast<object>().ToArray<object>();
							object[] array2 = ((IEnumerable)property.GetValue(model, null)).Cast<object>().ToArray<object>();

							Assert.AreEqual<int>(array.Length, array2.Length);

							for (int j = 0; j < array.Length; j++)
							{
								Assert.AreEqual<string>(array[j].ToString(), array2[j].ToString());
							}
						}
						else
						{
							if (ReflectionHelper.IsComplex(propertyInfo.PropertyType))
							{
								AssertEntityModel(propertyInfo.GetValue(entity, null) as Entity,
									property.GetValue(model, null) as EntityModel);
							}
							else
							{
								if (ReflectionHelper.IsComplexEnumerable(propertyInfo.PropertyType))
								{
									var list = ((IEnumerable)propertyInfo.GetValue(entity, null)).Cast<object>().ToList<object>();
									var list2 = ((IEnumerable)property.GetValue(model, null)).Cast<object>().ToList<object>();

									Assert.AreEqual<int>(list.Count, list2.Count);

									for (int j = 0; j < list.Count; j++)
									{
										if (list[j] == null)
										{
											Assert.IsNull(list2[j]);
										}

										AssertEntityModel(list[j] as Entity, list2[j] as EntityModel);
									}
								}
							}
						}
					}
				}
			}
		}

		private void AssertSimpleFlattenedEntityModel(Entity entity, object model, string contextualName)
		{
			Type type = model.GetType();

			var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < properties.Length; i++)
			{
				var propertyInfo = properties[i];
				var property = type.GetProperty("Entity" + contextualName + propertyInfo.Name);

				if (property == null)
				{
					property = type.GetProperty(contextualName + propertyInfo.Name, BindingFlags.Instance | BindingFlags.Public);
				}

				if (property != null && propertyInfo.GetValue(entity, null) == null)
				{
					Assert.IsNull(property.GetValue(model, null));
				}
				else
				{
					if (ReflectionHelper.IsSimple(propertyInfo.PropertyType))
					{
						Assert.AreEqual<string>(propertyInfo.GetValue(entity, null).ToString(), property.GetValue(model, null).ToString());
					}
					else
					{
						if (ReflectionHelper.IsSimpleEnumerable(propertyInfo.PropertyType))
						{
							object[] array = ((IEnumerable)propertyInfo.GetValue(entity, null)).Cast<object>().ToArray<object>();
							object[] array2 = ((IEnumerable)property.GetValue(model, null)).Cast<object>().ToArray<object>();

							Assert.AreEqual<int>(array.Length, array2.Length);

							for (int j = 0; j < array.Length; j++)
							{
								Assert.AreEqual<string>(array[j].ToString(), array2[j].ToString());
							}
						}
					}
				}
			}
		}

		[TestInitialize]
		public void TestInitialize()
		{
			Mapper<MapperTesting.MapperTester>.Instance.ClearMaps();
			Mapper<MapperTesting.MapperTester>.Instance.ClearConverters();
			Mapper<MapperTesting.MapperTester>.Instance.SetObjectFactory(new ObjectFactory());
			Mapper<MapperTesting.MapperTester>.Instance.Setup();
		}
	}
}
