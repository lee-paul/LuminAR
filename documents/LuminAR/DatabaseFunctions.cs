using UnityEngine;
using System.Collections;

namespace LuminAR.Project{

	public class DatabaseFunctions : MonoBehaviour {

		private bool hidden = true;

		[Header("Edit Specific Location Information")]
		public int edit_gps_id = 0;
		public float edit_gps_lat = -36f;
		public float edit_gps_long = 174f;
		public string edit_gps_desc = "GPS DESCRIPTION";

		[Header("Insert new gps Location values")]
		public string gps_lat = "GPS LATITUDE";
		public string gps_long = "GPS LONGITUDE";
		public string gps_desc = "GPS DESCRIPTION";

		[Header("Delete Location node at:")]
		public int delete_edit_gps_id = 0;

		DatabaseConnect db;

		void Awake(){
			db = new DatabaseConnect();
			db.openDB();
		}

		void OnGUI() {
			float w = Screen.width;
			float h = Screen.height;
			if (w > h) {
				w/=10; 
				h/=5;	
			} else {
				w/=5; 
				h/=10;	
			}
			
			
			if (!hidden) {
				gps_lat = GUI.TextField(new Rect(Screen.width / 2 - 70, Screen.height / 2 + 25, Screen.width / 3, 25), gps_lat);
				gps_long = GUI.TextField(new Rect(Screen.width / 2 - 70, Screen.height / 2 + 60, Screen.width / 3, 25), gps_long);
				gps_desc = GUI.TextField(new Rect(Screen.width / 2 - 70, Screen.height / 2 + 95, Screen.width / 3, 25), gps_desc);
				if (GUI.Button(new Rect(Screen.width - w -20, Screen.height / 2, w, 40), "INSERT TO DB")){
					db.insertToDB(System.Convert.ToSingle(gps_lat), System.Convert.ToSingle(gps_long), gps_desc);
					hide(true);
				}
			} else {
				if (GUI.Button(new Rect(Screen.width - w -20, Screen.height / 2, w, 40), "Show")) {
					hide(false);
				}
			}

			if (GUI.Button(new Rect(Screen.width - w -20, Screen.height / 2 + 80, w, 40),"Get Distance")){
				db.getDistance();
			}
			if (GUI.Button(new Rect(Screen.width - w -20, Screen.height / 2 + 160, w, 40),"Edit a Location")){
				db.editLocation(edit_gps_id,edit_gps_lat,edit_gps_long,edit_gps_desc);
			}
			if (GUI.Button(new Rect(Screen.width - w -20, Screen.height / 2 + 240, w, 40),"Delete a Location")){
				db.deleteLocation(delete_edit_gps_id);
			}
		}
		
		public void hide(bool shouldHide)
		{
			hidden = shouldHide;
		}
	}
}
