using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System;

public class CategoriesDataService : DatabaseService {

	#region UserInfo

	/// <summary>
	/// Gets the region data.
	/// </summary>
	/// <returns>The region data.</returns>
	public IEnumerable<Categories> GetCategoryData(){
		return _connection.Table<Categories>();
	}

	/// <summary>
	/// Tos the console.
	/// </summary>
	/// <param name="regionList">Region list.</param>
	public void ToConsole(IEnumerable<Categories> catList){
		foreach (var category in catList ) {
			ToConsole(category.ToString());
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
	/// <param name="_image">Image.</param>
	public void CreateCategory(long _id, string _name, string _image) {
		var category = new Categories {
			id = _id,
			name = _name,
			image = _image,
		};
		_connection.Insert(category);
	}

	/// <summary>
	/// Creates the or replace category.
	/// </summary>
	/// <param name="_id">Identifier.</param>
	/// <param name="_name">Name.</param>
	/// <param name="_image">Image.</param>
	public void CreateOrReplaceCategory(long _id, string _name, string _image){
		var category = new Categories {
			id = _id,
			name = _name,
			image = _image,
		};
		_connection.InsertOrReplace (category);
	}

	/// <summary>
	/// Gets the category.
	/// </summary>
	/// <returns>The category.</returns>
	/// <param name="catName">Cat name.</param>
	public IEnumerable<Categories> GetCategoriesWithName(string catName){
		return _connection.Table<Categories>().Where(x => x.name == catName);
	}

	/// <summary>
	/// Gets the single Category.
	/// </summary>
	/// <returns>The single categories.</returns>
	/// <param name="catName">Cat name.</param>
	public Categories GetSingleCategory(string catName){
		return _connection.Table<Categories>().Where(x => x.name == catName).FirstOrDefault();
	}

	/// <summary>
	/// Gets the category.
	/// </summary>
	/// <returns>The category.</returns>
	public IEnumerable<Categories> GetAllCategory(){
		return _connection.Table<Categories> ().Where (x => x.name != null).OrderBy(x => x.name);
	}

	/// <summary>
	/// Deletes all.
	/// </summary>
	/// <returns>The all.</returns>
	public int DeleteAll(){
		return _connection.DeleteAll<Categories> ();
	}

	public int GetCategoryCount(){
		return _connection.Table<Categories> ().Where (x => x.name != null).Count();
	}

	#endregion
}


[System.Serializable]
public class Categories {

	[PrimaryKey]
	public long id { get; set;}
	public string name { get; set;}
    public string desc { get; set; }
	public string image { get; set;}

	public override string ToString ()
	{
        return string.Format ("[RegionData: id ={1}, name = {2} , name = {3}, image = {4}", id, name, desc, image);  
	}
}
