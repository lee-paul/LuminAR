using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;

namespace LuminAR.Project{

	public class DatabaseDownload : MonoBehaviour {

		[Header("File Save destination")]
		// This can be used to download the file elsewhere.
		public string fileLocation; /**< String which holds the path of where to save the downloaded database, Can be set in the Unity Inspector */

		[Header("Sever to download db from")]
		public string url = "http://www.nzwheelsonline.com/AHCI/gpsnodes.sqlite"; /**< String that holds the URL address which specifies where to download the database file from, Can be set in the Unity Inspector */

		WWW www;

		public IEnumerator Start(){ 
		/**
		* The function that starts the download of the database.
		* fileLocation The location path of where to save the downloaded database.
		* url The URL of where to download the database from.
		* www The WWW class used to retieve content from URLs.
		* @see DatabaseConnect.cs
		**/
			www = new WWW (url);

			while (!www.isDone) {
				Debug.Log ("downloaded " + (www.progress*100).ToString() + "%...");
				yield return null;
			}
			Debug.Log ("Download Complete");
			string fullPath = Application.persistentDataPath + "/gpsnodes.sqlite";
			Debug.Log (Application.persistentDataPath);
			File.WriteAllBytes (fullPath, www.bytes);
		}

	}
}