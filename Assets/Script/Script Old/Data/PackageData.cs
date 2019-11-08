using System;
using System.Collections.Generic;

namespace ACP
{
	public class PackageData
	{
		public int id;
		public string date;
		public string description;
		public int minAppVersion;
		public string qcarConfigUrl;
		
		public string tracking_id;
		public string channel_tracking_id;
		
		public List<TrackableData> trackables;
		
		public PackageData ()
		{
			trackables = new List<TrackableData>();
		}
		
	}
	

}

