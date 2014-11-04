using System.Collections.Generic;

namespace Testing.Common
{
	public class SubEntity : Entity
	{
		public SubSubEntity SubSubEntity
		{
			get;
			set;
		}

		public SubSubEntity[] SubSubEntityArrayToArray
		{
			get;
			set;
		}

		public List<SubSubEntity> SubSubEntityListToList
		{
			get;
			set;
		}

		public SubSubEntity[] SubSubEntityArrayToList
		{
			get;
			set;
		}

		public List<SubSubEntity> SubSubEntityListToArray
		{
			get;
			set;
		}
	}
}
