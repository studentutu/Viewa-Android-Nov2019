using UnityEngine;
using System.Collections;
using SimpleJSON;
using OTPL.modal;

/// <summary>
/// The Geo data for a user.
/// 
/// http://ip-api.com/docs/api:json
/// 
/// <code>
/// {
/// 	"status": "success",
/// 	"country": "COUNTRY",
/// 	"countryCode": "COUNTRY CODE",
/// 	"region": "REGION CODE",
/// 	"regionName": "REGION NAME",
/// 	"city": "CITY",
/// 	"zip": "ZIP CODE",
/// 	"lat": LATITUDE,
/// 	"lon": LONGITUDE,
/// 	"timezone": "TIME ZONE",
/// 	"isp": "ISP NAME",
/// 	"org": "ORGANIZATION NAME",
/// 	"as": "AS NUMBER / NAME",
/// 	"query": "IP ADDRESS USED FOR QUERY"
/// }
/// </code>
/// 
/// </summary>
public class GeoData 
{
	/// <summary>
	/// The status that is returned if the response was successful.
	/// </summary>
	public const string SuccessResult = "success";

	public string Status { get; set; }
	public string Country { get; set; }
	public string IpAddress { get; set; }
}

public class GeoCountry : MonoBehaviour {

	// Use this for initialization
	void Start () {

		WebService.Instnace.Get ("http://ip-api.com/json", GeoCallback);
//		HTTP.Request someRequest = new HTTP.Request( "get", "http://ip-api.com/json" );
//		someRequest.Send( ( request ) => {
//
//			// Get Geo data
//			GeoData data = null;
//			try {
//				data = request.response.Get<GeoData>();
//			} 
//			catch (System.Exception ex) {
//
//				// TODO: Hook into an auto retry case
//
//				Debug.LogError("Could not get geo data: " + ex.ToString());
//				return;
//			}
//
//			// Ensure successful
//			if (data.Status != GeoData.SuccessResult) {
//
//				// TODO: Hook into an auto retry case
//
//				Debug.LogError("Unsuccessful geo data request: " + request.response.Text);
//				return;
//			}
//
//			Debug.Log ("User's Country: \"" + data.Country + "\"; Query: \"" + data.IpAddress + "\"");
//		});
	}
	void GeoCallback(string response) {

		Debug.Log ("GeoData: "+ response);
		var res = JSON.Parse(response);

		if (res != null) {
			
			GeoLocation responseModel = JsonUtility.FromJson<GeoLocation> (response); 
			GeoLocation location = responseModel;
            AppManager.Instnace.geoLocation.country = location.country;
            AppManager.Instnace.geoLocation.city = location.city;
            AppManager.Instnace.geoLocation.regionName = location.regionName;
            AppManager.Instnace.geoLocation.countryCode = location.countryCode;
            AppManager.Instnace.geoLocation.zip = location.zip;
            AppManager.Instnace.geoLocation.lat = location.lat;
            AppManager.Instnace.geoLocation.lon = location.lon;
            AppManager.Instnace.geoLocation.deviceId = location.deviceId;
            AppManager.Instnace.geoLocation.timezone = location.timezone;
            AppManager.Instnace.geoLocation.deviceName = location.deviceName;
		}
	}

}