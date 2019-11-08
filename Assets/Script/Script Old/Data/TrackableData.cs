using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class TrackableData
	{
		public ulong id;
		public string tracking_id;
		
		public string cloud_id;
		
		public string channel_tracking_id;
		
		public string channel_share_title;
		public string channel_share_text;
		public string channel_share_link;
		
		public int min_app_version;
		
		public string share_url;
		public string share_text;
		public string share_title;
		
		public bool sticky = false;
		public bool record_history = false;
        public bool is_love = false;
		
		//public PackageData packageData;
		
		public List<AreaData> areas;
		public string imageUrl;
		public Vector3 size;
		
		public TrackableData ()
		{
			areas = new List<AreaData>();
		}
		
		public static TrackableData Create (JSONObject trackableJson) //, PackageData package)
		{
			TrackableData trackable = new TrackableData ();
			//trackable.id = ulong.Parse(trackableJson ["id"].n);
			
			JSONObject temp = trackableJson ["tracking_id"];
			if (temp != null)
				trackable.tracking_id = temp.str;
			//trackable.packageData = package;
			
			temp = trackableJson ["share_link"];
			if (temp != null) 
				trackable.share_url = temp.str;
			
			temp = trackableJson ["share_title"];
			if (temp != null) 
				trackable.share_title = temp.str;
			
			temp = trackableJson ["share_text"];
			if (temp != null) 
				trackable.share_text = temp.str;
			
			temp = trackableJson ["sticky"];
			if (temp != null) {
				trackable.sticky = temp.b;
			}
			
			temp = trackableJson ["min_app_version"];
			if (temp != null) {
				trackable.min_app_version = int.Parse (temp.str);
			}
			
			trackable.cloud_id = trackableJson.StringOrEmpty ("cloud_id");
			
			trackable.channel_tracking_id = trackableJson.StringOrEmpty ("channel_tracking_id");
			
			trackable.channel_share_text = trackableJson.StringOrEmpty ("channel_share_text");
			trackable.channel_share_title = trackableJson.StringOrEmpty ("channel_share_title");
			trackable.channel_share_link = trackableJson.StringOrEmpty ("channel_share_link");

			temp = trackableJson ["image_url"];
			if (temp != null && temp.str != "") {
				trackable.imageUrl = temp.str;
			}
			JSONObject s = trackableJson ["size"];
			if (s != null) {
				if (s.Count == 2) {
					trackable.size = new Vector3 ((float)s [0].n, 1.0f, (float)s [1].n);
				} else {
					trackable.size = new Vector3 ((float)s [0].n, (float)s [1].n, (float)s [2].n);
				}
			}
			
			// Load the areas data
			JSONObject areasJson = trackableJson ["areas"];
			
			if (areasJson != null) {
				//Debug.Log("Found " + areasJson.Count + " areas");
				
				//Debug.Log ("Found " + areasJson.Count + " areas.");
				for (int ei = 0; ei < areasJson.Count; ei++) {
					AreaData area = AreaData.Create (areasJson [ei], trackable);
					if (area != null) {
						//Debug.Log ("Adding area with id: " + area.id);
						trackable.areas.Add (area);
					}
				}
			} else {
				Debug.Log ("areas is null");
			}
			//Debug.Log("Creating trackable data with id: " + trackable.id);
			
			
			// If record history is there, use that to determine if we can record history
			temp = trackableJson["record_history"];
			if (temp != null)
			{
				trackable.record_history = temp.b;
			}
			else
			{
				// Otherwise if its not there, use the value of sticky
				trackable.record_history = trackable.sticky;
			}
            temp = trackableJson["is_love"];
            if(temp != null){
                trackable.is_love = temp.b;
            }
			
			return trackable;
		}
	}
}

