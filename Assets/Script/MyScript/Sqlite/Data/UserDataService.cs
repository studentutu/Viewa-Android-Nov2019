using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System;

public class UserDataService : DatabaseService {

		#region UserInfo
		
		/// <summary>
		/// Gets the user info table values.
		/// </summary>
		/// <returns>The user info.</returns>
		public IEnumerable<UserInfo> GetUserInfo(){
			return _connection.Table<UserInfo>();
		}
		
		/// <summary>
		/// To console with new line.
		/// </summary>
		/// <param name="userList">User list.</param>
		public void ToConsole(IEnumerable<UserInfo> userList){
			foreach (var user in userList ) {
			ToConsole(user.ToString());
			}
		}
		
		/// <summary>
		/// To console with space.
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


	public void CreateUser(string _viewaUser_id, string _address1, string _address2, string _country, string _dob, string _email, string _firstName, string _lastName, string _gender, string _mobile,
		string _postCode, string _relationshipStatus, string _state, string _suburb, string _facebookId, string _googleId, string _linkedIn, string _password, string _passwordSalt, long _regionId, string _loginInfo, int _isLoggedIn, string _dateCreated, string _interest){
			var user = new UserInfo {
			ViewaUser_id = _viewaUser_id,
			Address1 = _address1,
			Address2 = _address2,
			Country = _country,
			DateOfBirth = _dob,
			Email = _email,
			FirstName = _firstName,
			LastName = _lastName,
			Gender = _gender,
			Mobile = _mobile,
			PostCode = _postCode,
			RelationshipStatus = _relationshipStatus,
			State = _state,
			Suburb = _suburb,
			FacebookId = _facebookId,
			GoogleId = _googleId,
			LinkedinId = _linkedIn,
			Password = _password,
			PasswordSalt = _passwordSalt,
			RegionId = _regionId,
			LoginInfo = _loginInfo,
			isLoggedIn = _isLoggedIn,
			DateCreated = _dateCreated,
			interest = _interest 
		};
		_connection.Insert (user);
	}


	public void CreateOrReplaceUser( string _viewaUser_id, string _address1, string _address2, string _country, string _dob, string _email, string _firstName, string _lastName, string _gender, string _mobile,
		string _postCode, string _relationshipStatus, string _state, string _suburb, string _facebookId, string _googleId, string _linkedIn, string _password, string _passwordSalt, long _regionId, string _loginInfo, int _isLoggedIn, string _dateCreated, string _interest){
		var user = new UserInfo {
			ViewaUser_id = _viewaUser_id,
			Address1 = _address1,
			Address2 = _address2,
			Country = _country,
			DateOfBirth = _dob,
			Email = _email,
			FirstName = _firstName,
			LastName = _lastName,
			Gender = _gender,
			Mobile = _mobile,
			PostCode = _postCode,
			RelationshipStatus = _relationshipStatus,
			State = _state,
			Suburb = _suburb,
			FacebookId = _facebookId,
			GoogleId = _googleId,
			LinkedinId = _linkedIn,
			Password = _password,
			PasswordSalt = _passwordSalt,
			RegionId = _regionId,
			LoginInfo = _loginInfo,
			isLoggedIn = _isLoggedIn,
			DateCreated = _dateCreated,
			interest = _interest 
		};
		_connection.InsertOrReplace (user);
	}
		
		/// <summary>
		/// Gets all users with name from UserInfo table.
		/// </summary>
		/// <returns>The all users having name userName.</returns>
		/// <param name="userName">User name.</param>
		public IEnumerable<UserInfo> GetUser(string userName){
			return _connection.Table<UserInfo>().Where(x => x.FirstName == userName);
		}

		/// <summary>
		/// return a single user from the UserInfo table.
		/// </summary>
		/// <returns>The all users having name userName.</returns>
		/// <param name="userName">User name.</param>
		public UserInfo GetSingleUser(string userName){
			return _connection.Table<UserInfo>().Where(x => x.FirstName == userName).FirstOrDefault();
		}
		
		public UserInfo GetUser(){
			return _connection.Table<UserInfo> ().Where (x => x.Email != null).FirstOrDefault ();
		}

		public int DeleteAll(){

		_connection.DeleteAll<UserInfo> ();
//		if (_connection.Table<UserInfo> ().Select (x => x.Email).Count > 0) {
//				int returnVal = _connection.DeleteAll<UserInfo> ();
//				return returnVal;
//			}
		return 0;
		}
		
		public int UserExist(string email){

			int userCount =  _connection.Table<UserInfo> ().Where (x => x.Email == email).Count();
			return userCount;
		}
		#endregion
}


[System.Serializable]
public class UserInfo {

	[PrimaryKey]
	public string ViewaUser_id { get; set; }
	public string Address1 { get; set; }
	public string Address2 { get; set; }
	public string Country { get; set; }
	public string DateOfBirth { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Gender { get; set; }
	public string Mobile { get; set; }
	public string PostCode { get; set;}
	public string RelationshipStatus { get; set;}
	public string State { get; set; }
	public string Suburb { get; set;}
	public string FacebookId { get; set;}
	public string GoogleId { get; set;}
	public string LinkedinId { get; set;}
	public string Password { get; set; }
	public string PasswordSalt { get; set;}
	public long RegionId { get; set; }
	public string LoginInfo { get; set;}
	public int isLoggedIn { get; set;}
	public string interest { get; set;}
	public string DateCreated { get; set;}


	public override string ToString ()
	{
		return string.Format ("[UserInfo: Address1 ={1}, Address2 = {3} , Country = {4}, DateOfBirth = {5}, Email = {6}, FirstName={7},  LastName={8}, Gender={9}, Mobile ={10}" +
			", PostCode ={11}, RelationshipStatus = {12}, State = {13}, Suburb = {14}, FacebookId = {15}, GoogleId = {16}, Mobile={17}, Password={18}, LoginInfo ={19}, isLoggedIn{20}, isLoggedIn{21}]",  Address1, Address2, Country, DateOfBirth, Email,  FirstName, LastName, Gender, Mobile, PostCode, RelationshipStatus, State, Suburb, 
			FacebookId, GoogleId, LinkedinId, Password, PasswordSalt, RegionId, LoginInfo, isLoggedIn, interest);  //ViewaUser_id={0},ViewaUser_id,
	}
}