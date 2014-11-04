using System.Collections.Generic;

namespace Testing.Common
{
	public class MainEntityModel : EntityModel
	{
		public SubEntityModel SubEntity
		{
			get;
			set;
		}
		public SubEntityModel[] SubEntityArrayToArray
		{
			get;
			set;
		}
		public List<SubEntityModel> SubEntityListToList
		{
			get;
			set;
		}
		public List<SubEntityModel> SubEntityArrayToList
		{
			get;
			set;
		}
		public SubEntityModel[] SubEntityListToArray
		{
			get;
			set;
		}
	}
}
