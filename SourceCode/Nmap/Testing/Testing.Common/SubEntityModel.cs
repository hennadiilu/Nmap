using System.Collections.Generic;

namespace Testing.Common
{
	public class SubEntityModel : EntityModel
	{
		public SubSubEntityModel SubSubEntity
		{
			get;
			set;
		}

		public SubSubEntityModel[] SubSubEntityArrayToArray
		{
			get;
			set;
		}

		public List<SubSubEntityModel> SubSubEntityListToList
		{
			get;
			set;
		}

		public List<SubSubEntityModel> SubSubEntityArrayToList
		{
			get;
			set;
		}

		public SubSubEntityModel[] SubSubEntityListToArray
		{
			get;
			set;
		}
	}
}
