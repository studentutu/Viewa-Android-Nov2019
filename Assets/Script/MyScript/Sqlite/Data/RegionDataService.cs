using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System;

public class RegionDataService : DatabaseService {

	#region UserInfo

	/// <summary>
	/// Gets the region data.
	/// </summary>
	/// <returns>The region data.</returns>
	public IEnumerable<RegionData> GetRegionData(){
		return _connection.Table<RegionData>();
	}

	/// <summary>
	/// Tos the console.
	/// </summary>
	/// <param name="regionList">Region list.</param>
	public void ToConsole(IEnumerable<RegionData> regionList){
		foreach (var region in regionList ) {
			ToConsole(region.ToString());
		}
	}

	/// <summary>
	/// Tos the console with space.
	/// </summary>
	/// <param name="msg">Message.</param>
	public void ToConsoleWithSpace(string msg){
		Debug.Log (" " +msg+ " ");
	}

	/// <summary>
	/// To console with space.
	/// </summary>
	/// <param name="msg">Message.</param>
	public void ToConsole(string msg){
		Debug.Log (System.Environment.NewLine +msg);
	}

	/// <summary>
	/// Creates the region.
	/// </summary>
	/// <param name="_id">Identifier.</param>
	/// <param name="_name">Name.</param>
	/// <param name="_client_access_key">Client access key.</param>
	/// <param name="_client_secret_key">Client secret key.</param>
	/// <param name="_image">Image.</param>
	/// <param name="_autoDetect">Auto detect.</param>
	/// <param name="_countryCode">Country code.</param>
	public void CreateRegion(long _id, string _name, string _client_access_key, string _client_secret_key, string _image, int _autoDetect, string _countryCode) {
		var region = new RegionData {
			id = _id,
			name = _name,
			client_access_key = _client_access_key,
			client_secret_key = _client_secret_key,
			image = _image,
			autoDetect = _autoDetect,
			countryCode = _countryCode,
		};
		_connection.Insert(region);
	}

	/// <summary>
	/// Creates the or replace region.
	/// </summary>
	/// <param name="_name">Name.</param>
	/// <param name="_client_access_key">Client access key.</param>
	/// <param name="_client_secret_key">Client secret key.</param>
	/// <param name="_id">Identifier.</param>
	/// <param name="_image">Image.</param>
	/// <param name="_autoDetect">If set to <c>true</c> auto detect.</param>
	/// <param name="_countryCode">Country code.</param>
	public void CreateOrReplaceRegion(long _id, string _name, string _client_access_key, string _client_secret_key, string _image, int _autoDetect, string _countryCode){
		var region = new RegionData {
			id = _id,
			name = _name,
			client_access_key = _client_access_key,
			client_secret_key = _client_secret_key,
			image = _image,
			autoDetect = _autoDetect,
			countryCode = _countryCode,
		};
		_connection.InsertOrReplace (region);
	}

	/// <summary>
	/// Gets the region.
	/// </summary>
	/// <returns>The region.</returns>
	/// <param name="regionName">Region name.</param>
	public IEnumerable<RegionData> GetRegion(string regionName){
		return _connection.Table<RegionData>().Where(x => x.name == regionName);
	}

	/// <summary>
	/// Gets the single region.
	/// </summary>
	/// <returns>The single region.</returns>
	/// <param name="regionName">Region name.</param>
	public RegionData GetSingleRegion(string regionName){
		return _connection.Table<RegionData>().Where(x => x.name == regionName).FirstOrDefault();
	}

	/// <summary>
	/// Gets the region.
	/// </summary>
	/// <returns>The region.</returns>
	public RegionData GetRegion(){
		return _connection.Table<RegionData> ().Where (x => x.name != null).FirstOrDefault ();
	}

	/// <summary>
	/// Deletes all.
	/// </summary>
	/// <returns>The all.</returns>
	public int DeleteAll(){
		return _connection.DeleteAll<RegionData> ();
	}

	public int GetRegionCount(){
		return _connection.Table<RegionData> ().Where (x => x.name != null).Count();
	}
	#endregion
}


[System.Serializable]
public class RegionData {

	[PrimaryKey]
	public long id { get; set;}
	public string name { get; set;}
	public string image { get; set;}
	public string client_access_key { get; set;}
	public string client_secret_key { get; set;}
	public int autoDetect { get; set;}
	public string countryCode { get; set;}

	public override string ToString ()
	{
		return string.Format ("[RegionData: id ={1}, name = {2} , image = {3}, client_access_key = {4}, client_secret_key = {4}, autoDetect = {5}, countryCode = {6}]", id, name, image, client_access_key, client_secret_key, autoDetect, countryCode);  
	}
}