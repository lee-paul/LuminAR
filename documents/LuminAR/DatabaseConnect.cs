using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

namespace LuminAR.Project{

	public class DatabaseConnect {

		[Header("Database Name")]
		public string dbName = "gpsnodes.sqlite";

		[Header("Table Name")]
		public string tableName = "nodes";

		[Header("Table Name")]
		// Can set a location to load the database from elsewhere.
		public string fileLocation; //Application.dataPath

		[Header("ID of Location")]
		// You can set this ID if you know what ID the location is in the Database
		public int locationID = 0;

		private int gps_id = 0;
		private float gps_lat = 0f;
		private float gps_long = 0f;
		private string gps_desc = "";

		//This value is the max distance (in meters) around the user that nodes will show
		public float maxDistance = 300f;

		IDbConnection dbconn;
		IDbCommand dbcmd;
		IDataReader reader;

		AndroidJavaClass gpsActivityJavaClass;
		//static float locationArray = 0f;
		static string distanceArray = "";

		DatabaseDownload dbDownload;

		// Open the database for access
		public void openDB () {
			string conn = "URI=file:" + Application.persistentDataPath + "/" + dbName; //Path to database.

			dbconn = (IDbConnection)new SqliteConnection (conn);
			dbconn.Open (); //Open connection to the database.
			Debug.Log("Connection Success!");

			AndroidJNI.AttachCurrentThread();
			gpsActivityJavaClass = new AndroidJavaClass("com.LuminAR.Project.GPSLocation");
			
			if (dbconn != null && dbconn.State == ConnectionState.Open) {
				
				dbcmd = dbconn.CreateCommand ();
				string sqlQuery = "SELECT * FROM " + tableName;
				dbcmd.CommandText = sqlQuery;
				reader = dbcmd.ExecuteReader ();
				
				//Read the contents of the database.
				while (reader.Read()) {
					gps_id = reader.GetInt32 (0);
					gps_lat = reader.GetFloat (1);
					gps_long = reader.GetFloat (2);
					gps_desc = reader.GetString (3);
					
					gpsActivityJavaClass.CallStatic("createLocation",gps_lat,gps_long,gps_desc);
					
				}
			} else {
				Debug.Log ("Error - Could not connect to Database!");
			}
		
		}
		// Called to insert in to the database
		public void insertToDB (float gps_lat, float gps_long, string gps_desc) {
			//Check if the database is open
			if (dbconn != null && dbconn.State == ConnectionState.Open) {
				
				dbcmd = dbconn.CreateCommand ();
				string sqlQuery = "INSERT INTO " + tableName + " (gps_lat, gps_long, gps_desc) VALUES (" + gps_lat + ", " 
													+ gps_long + ", '" + gps_desc + "')";
				dbcmd.CommandText = sqlQuery;
				reader = dbcmd.ExecuteReader ();
				Debug.Log("Inserted in to database!");

				gpsActivityJavaClass.CallStatic("createLocation",gps_lat,gps_long,gps_desc);
			
			} else {
				Debug.Log ("Error - Could not connect to Database! - Trying to connect...");
			}
		}

		public void getDistance(){
			GUIText setText;
			//Check if the database is open
			if (dbconn != null && dbconn.State == ConnectionState.Open) {
				
				distanceArray = gpsActivityJavaClass.CallStatic<string>("getDistanceTo",maxDistance);				
				
				setText = GameObject.Find ("gps_locations").GetComponent<GUIText>();
				setText.text = (distanceArray);
				Debug.Log("Retrieved the distances");
			} else {
				Debug.Log ("Error - Could not connect to Database! - Trying to connect...");
			}
		}

		public void editLocation(int gps_id, float gps_lat, float gps_long, string gps_desc){
			//Check if the database is open
			if (dbconn != null && dbconn.State == ConnectionState.Open) {

				gpsActivityJavaClass.CallStatic("editLocation",gps_id, gps_lat, gps_long, gps_desc);

				dbcmd = dbconn.CreateCommand ();
				string sqlQuery = "UPDATE nodes SET gps_lat="+ gps_lat + ", gps_long=" + gps_long + ",gps_desc=\""+ gps_desc + "\" WHERE _id=" + gps_id;
				dbcmd.CommandText = sqlQuery;
				reader = dbcmd.ExecuteReader ();
				Debug.Log("Updated Location");
				
			} else {
				Debug.Log ("Error - Could not connect to Database! - Trying to connect...");
			}
		}
		public void deleteLocation(int gps_id){
			//Check if the database is open
			if (dbconn != null && dbconn.State == ConnectionState.Open) {

				gpsActivityJavaClass.CallStatic("deleteLocation",gps_id);	
				
				dbcmd = dbconn.CreateCommand ();
				string sqlQuery = "DELETE FROM nodes WHERE _id=" + gps_id;
				dbcmd.CommandText = sqlQuery;
				reader = dbcmd.ExecuteReader ();
				Debug.Log("Deleted Location");
				
			} else {
				Debug.Log ("Error - Could not connect to Database! - Trying to connect...");
			}
		}
				
				public void closeConnections(){
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			dbconn.Close ();
			dbconn = null;
		}
	}
}
