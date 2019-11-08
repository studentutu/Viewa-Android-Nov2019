using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;


namespace VoxelBusters.NativePlugins
{
	public partial class ExternalAuthenticationCredentials
	{
		public class iOS
		{
			#region Constants
			private		const 		string		kCredentialsPublicKeyUrl	= "public-key-url";
			private		const 		string		kCredentialsSignature		= "signature";
			private		const 		string		kCredentialsSalt			= "salt";
			private		const 		string		kCredentialsTimestamp		= "timestamp";
			#endregion

			public string PublicKeyURL
			{
				get; 
				private set;
			}

			public byte[] Signature
			{
				get; 
				private set;
			}

			public byte[] Salt
			{
				get; 
				private set;
			}

			public long Timestamp
			{
				get; 
				private set;
			}

			public void Load(IDictionary _jsonDict)
			{
				PublicKeyURL 	= _jsonDict.GetIfAvailable<string>(kCredentialsPublicKeyUrl);

				string _signature = _jsonDict.GetIfAvailable<string>(kCredentialsSignature);
				if(!string.IsNullOrEmpty(_signature))
				{
                    Signature 		= System.Convert.FromBase64String(_signature);
				}

				string _salt 	= _jsonDict.GetIfAvailable<string>(kCredentialsSalt);
				if(!string.IsNullOrEmpty(_salt))
				{
					Salt 		= System.Convert.FromBase64String(_salt);
                }

				string _timestamp = _jsonDict.GetIfAvailable<string>(kCredentialsTimestamp);

				if(!string.IsNullOrEmpty(_timestamp))
				{
					Timestamp 	= long.Parse(_timestamp);
				}
            }
		}
	}
}