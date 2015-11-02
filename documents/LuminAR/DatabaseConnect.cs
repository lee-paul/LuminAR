using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

namespace LuminAR.Project{

	public class DatabaseConnect {

		[Header("Database Name")]
		public string dbName = "gpsnodes.sqlite"; /**< String which holds the database name, Can be set in the Unity Inspector */

		[Header("Table Name")]
		public string tableName = "nodes"; /**< String which holds the table name, Can be set in the Unity Inspector */

		[Header("Table Name")]
		// Can set a location to load the database from elsewhere.
		public string fileLocation;  /**< String which holds the path of the database file, Can be set in the Unity Inspector */

		[Header("ID of Location")]
		// You can set this ID if you know what ID the location is in the Database
		public int locationID = 0; /**< Int of the ID of a location (admin use only) that is the target of edit/delete, Can be set in the Unity Inspector */

		private int gps_id = 0; /**< Int of the id of the gps. Can be set in the Unity Inspector */
		private float gps_lat = 0f;/**< Float that holds the value of the GPS LATITUDE Coordinate that is to be changed to. Can be set in the Unity Inspector */
		private float gps_long = 0f;/**< Float that holds the value of the GPS LONGITUDE Coordinate that is to be changed to. Can be set in the Unity Inspector */
		private string gps_desc = "";/**< String The description of the GPS NODE that is set by the user, to give meaning. Can be set in the Unity Inspector */

		//This value is the max distance (in meters) around the user that nodes will show
		public float maxDistance = 500f; /**< Float that is set which holds a value, it is the MAX DISTANCE away from the user in which nodes will show. Can be set in the Unity Inspector */

		IDbConnection dbconn; /**< The database connection variable*/
		IDbCommand dbcmd; /**< The database command variable*/
		IDataReader reader; /**<  The database reader variable*/

		AndroidJavaClass gpsActivityJavaClass; /**< AndroidJavaClass this is to make a connection with the .jar file that handles the Android GPS functions */
		//static float locationArray = 0f;
		static string distanceArray = "";

		DatabaseDownload dbDownload;
		Boolean dbConnection = false;

		// Open the database for access
		public void openDB () {
			/**
			* Method which finds the database that has been downloaded to make a connection.
			* This method also opens the .jar file for the Android GPS Functions, it also runs a select query for the database to read all the nodes in the database.
			* 
			**/
			string conn = "URI=file:" + Application.persistentDataPath + "/" + dbName; //Path to database.

			dbconn = (IDbConnection)new SqliteConnection (conn);
			dbConnection = true;
			dbconn.Open (); //Open connection to the database.
			Debug.Log("Connection Success!");

			#if !UNITY_EDITOR
			//Make a connection to the Java (.jar) file that is created.
			AndroidJNI.AttachCurrentThread();
			gpsActivityJavaClass = new AndroidJavaClass("com.LuminAR.Project.GPSLocation"); //The path to the .jar file
			
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
			#endif		
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
			/**
			* A function which takes the latitude, longitude and description values that have been set to insert in to the database.
			* @param gps_lat The float variable that holds the GPS LATITUDE.
			* @param gps_long The float variable that holds the GPS LONGITUDE.
			* @param gps_desc The string variable that holds the GPS DESCRIPTION.
			**/
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
			/**
			* A function which retrieves the nodes from the database and runs a function to get the distance the user is away from the nodes in the database.
			**/
		}

		public void setMax(float distance){
			//Check if the database is open
			if (dbconn != null && dbconn.State == ConnectionState.Open) {
				
				maxDistance = distance;
				Debug.Log("The max distance is now " + maxDistance);
			} else {
				Debug.Log ("Error - Could not connect to Database! - Trying to connect...");
			}
			/**
			* A function which retrieves the nodes from the database and runs a function to get the distance the user is away from the nodes in the database.
			**/
		}

		public void getAllNodes(){
			GUIText setText;
			//Check if the database is open
			if (dbconn != null && dbconn.State == ConnectionState.Open) {
				
				distanceArray = gpsActivityJavaClass.CallStatic<string>("getAllNodes");				
				
				setText = GameObject.Find ("gps_locations").GetComponent<GUIText>();
				setText.text = (distanceArray);
				Debug.Log("Retrieved the nodes");
			} else {
				Debug.Log ("Error - Could not connect to Database! - Trying to connect...");
			}
			/**
			* A function which retrieves the nodes from the database and runs a function to get the distance the user is away from the nodes in the database.
			**/
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
			/**
			* A function which takes the latitude, longitude and description values that have been set by the user and uses those values to edit an existing node in the database.
			* @param gps_id The int variable that has been set by the user (set in the Unity Inspector) which specifies which node in the database they would like to edit.
			* @param gps_lat The float variable that holds the GPS LATITUDE.
			* @param gps_long The float variable that holds the GPS LONGITUDE.
			* @param gps_desc The string variable that holds the GPS DESCRIPTION.
			**/
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
			/**
			* A function which takes an ID variable that has been set by the user and deletes the corresponding node in the database.
			* @param gps_id The int variable (set in the Unity Inspector]) that specifies which node to delete from the database.
			**/
		}
				
		public void closeConnections(){
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			dbconn.Close ();
			dbconn = null;
			/**
			* The function which closes the database and reader connections
			**/
		}
	}
}
