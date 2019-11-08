using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	public partial class ExternalAuthenticationCredentials
	{
		public class Android
		{
			#region Constants
			private		const	string 		kServerAuthCodeKey	= "server-auth-code";
			private		const	string 		kIDToken	= "id-token";
			#endregion

			public string ServerAuthCode
			{
				get; 
				private set;
			}

			public string IDToken
			{
				get; 
				private set;
			}

			public void Load(IDictionary _jsonDict)
			{
				string _authCodeEncoded = _jsonDict.GetIfAvailable<string>(kServerAuthCodeKey);
				string _idToken 		= _jsonDict.GetIfAvailable<string>(kIDToken);
				ServerAuthCode 	= _authCodeEncoded.FromBase64();
				IDToken			= _idToken.FromBase64();
			}
		}
	}
}