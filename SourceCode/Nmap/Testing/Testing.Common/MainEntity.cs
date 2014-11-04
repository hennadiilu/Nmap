using System.Collections.Generic;

namespace Testing.Common
{
	public class MainEntity : Entity
	{
		public SubEntity SubEntity
		{
			get;
			set;
		}

		public SubEntity[] SubEntityArrayToArray
		{
			get;
			set;
		}

		public List<SubEntity> SubEntityListToList
		{
			get;
			set;
		}

		public SubEntity[] SubEntityArrayToList
		{
			get;
			set;
		}

		public List<SubEntity> SubEntityListToArray
		{
			get;
			set;
		}
	}
}
