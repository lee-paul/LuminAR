using UnityEngine;
using System.Collections;

namespace LuminAR.Project{

	public class DatabaseFunctions : MonoBehaviour {

		[Header("Edit Specific Location Information")]
		public int edit_gps_id = 0; /**< Int that holds the value of the GPS ID to edit, Can be set in the Unity Inspector */
		public float edit_gps_lat = -36f; /**< Float that holds the value of the GPS LATITUDE to edit, Can be set in the Unity Inspector */
		public float edit_gps_long = 174f; /**< Int that holds the value of the GPS LONGITUDE to edit, Can be set in the Unity Inspector */
		public string edit_gps_desc = "GPS DESCRIPTION"; /**< String that holds the DESCRIPTION of the gps node to edit, Can be set in the Unity Inspector */

		[Header("Insert new gps Location values")]
		public string gps_id = "0";
		public string gps_lat = "GPS LATITUDE"; /**< String which is the value (converted to float) of the LATITUDE to be inserted to the database, Can be set in the Unity Inspector */
		public string gps_long = "GPS LONGITUDE"; /**< String which is the value (converted to float) of the LONGITUDE to be inserted to the database, Can be set in the Unity Inspector */
		public string gps_desc = "GPS DESCRIPTION"; /**< String which holds the DESCRIPTION to be inserted to the database, Can be set in the Unity Inspector */

		[Header("Delete Location node at:")]
		public int delete_edit_gps_id = 0; /**< Int which is set that specifies which node to delete from the database, Can be set in the Unity Inspector */

		DatabaseConnect db;

		void Awake(){
			/** 
			* When the class is active, start the database connection and open the database.
			* @see DatabaseConnect.cs
			*/
			db = new DatabaseConnect();
			db.openDB();
		}

		void OnGUI() {
			/**
			* On the UNITY GUI display fields and buttons that can be interacted with by the user to to the GPS Functions.
			* - Add a new GPS NODE.
			* - EDIT a GPS NODE.
			* - DELETE a GPS NODE.
			* - Retrieve the distances away of the stored gps nodes from the user.
			* @see DatabaseConnect.cs
			* @see DatabaseDownload.cs
			*/
			float w = Screen.width;
			float h = Screen.height;
			if (w > h) {
				w/=10; 
				h/=5;	
			} else {
				w/=5; 
				h/=10;	
			}

			GUIStyle customButtons = new GUIStyle("button");
			customButtons.fontSize = 30;
			GUIStyle customTextfields = new GUIStyle("textfield");
			customTextfields.fontSize = 30;

			gps_id = GUI.TextField(new Rect(Screen.width / 2 - (Screen.width/4), Screen.height / 2 - 30, Screen.width/2, 50), gps_id,customTextfields);
			gps_lat = GUI.TextField(new Rect(Screen.width / 2 - (Screen.width/4), Screen.height / 2 + 25, Screen.width/2, 50), gps_lat,customTextfields);
			gps_long = GUI.TextField(new Rect(Screen.width / 2 - (Screen.width/4), Screen.height / 2 + 80 , Screen.width/2, 50), gps_long,customTextfields);
			gps_desc = GUI.TextField(new Rect(Screen.width / 2 - (Screen.width/4), Screen.height / 2 + 135, Screen.width/2, 50), gps_desc,customTextfields);
				
			if (GUI.Button(new Rect(Screen.width /2 - (Screen.width/4), Screen.height / 2 + 200, Screen.width/2, 50), "Insert Location to Database",customButtons)){
				db.insertToDB(System.Convert.ToSingle(gps_lat), System.Convert.ToSingle(gps_long), gps_desc);
			}
			if (GUI.Button(new Rect(Screen.width /2 - (Screen.width/4), Screen.height / 2 + 260, Screen.width/2, 50),"Get Nearby Node Distances",customButtons)){
				db.getDistance();
			}
			if (GUI.Button(new Rect(Screen.width / 2 - (Screen.width/4), Screen.height / 2 + 320, Screen.width/2, 50),"Edit Location",customButtons)){
				db.editLocation(int.Parse(gps_id), System.Convert.ToSingle(gps_lat), System.Convert.ToSingle(gps_long), gps_desc);
			}
			if (GUI.Button(new Rect(Screen.width / 2 - (Screen.width/4), Screen.height / 2 + 380, Screen.width/2, 50),"Delete Location",customButtons)){
				db.deleteLocation(int.Parse(gps_id));
			}
			if (GUI.Button(new Rect(Screen.width / 2 - (Screen.width/4), Screen.height / 2 + 440, Screen.width/2, 50),"Get All Locations",customButtons)){
				db.getAllNodes();
			}
		}
	}
}
