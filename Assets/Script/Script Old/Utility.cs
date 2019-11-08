using System;
using UnityEngine;
using UnityEngine.UI;

namespace ACP
{
	public class Utility
	{
		public Utility ()
		{
		}

		public static string Md5Sum (string strToEncrypt)
		{
			System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding ();
			byte[] bytes = ue.GetBytes (strToEncrypt);
			
			// encrypt bytes
			System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
			byte[] hashBytes = md5.ComputeHash (bytes);
			
			// Convert the encrypted bytes back to a string (base 16)
			string hashString = "";
			
			for (int i = 0; i < hashBytes.Length; i++) {
				hashString += System.Convert.ToString (hashBytes[i], 16).PadLeft (2, '0');
			}
			
			return hashString.PadLeft (32, '0');
		}
		
		public static string GetTempFilePath () 
        { 
			return Application.temporaryCachePath;
//#if UNITY_IPHONE
//                // Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
//                // Application.dataPath returns              
//                // /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
//                // Strip "/Data" from path 
//                string path = Application.dataPath.Substring (0, Application.dataPath.Length - 5); 
//                // Strip application name 
//                path = path.Substring(0, path.LastIndexOf('/'));  
//                return path + "/Library/Caches/"; 
//#elif UNITY_ANDROID
//			return "/sdcard/"		
//#endif
        }

		public static Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r,g,b, 255);
		}

	}
}

