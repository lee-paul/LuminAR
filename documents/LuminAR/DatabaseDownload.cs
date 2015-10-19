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
		public string fileLocation; //Application.dataPath

		[Header("Sever to download db from")]
		public string url = "http://www.nzwheelsonline.com/AHCI/gpsnodes.sqlite";

		WWW www;

		IEnumerator Start(){
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