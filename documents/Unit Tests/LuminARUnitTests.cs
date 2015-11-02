using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace LuminAR.Project{
	[TestFixture]
		public class LuminARUnitTests	{
			DatabaseConnect dbConn;
			GPSManager gpsMan;

			[Test]
			public void userLocationTest(){
				gpsMan = new GPSManager();
				gpsMan.setUserLocation("500");
				string userLocation = gpsMan.getUserLocation();
				
				Assert.AreEqual("500", userLocation);
			}

			[Test]
			public void OpenDBTest(){

			dbConn = new DatabaseConnect();
			Boolean connection = true;
			dbConn.openDB();

			Assert.AreEqual(true, connection);
			}

			[Test]
			public void InsertToDBErrorTest(){
			dbConn = new DatabaseConnect();

			float gps_lat = 25f;
			float gps_long = 30f;
			string gps_desc = "Test Location";
			string tableName = "nodes";

			string sqlQuery = "INSERT INTO " + tableName + " (gps_lat, gps_long, gps_desc) VALUES (" + gps_lat + ", " 
				+ gps_long + ", '" + gps_desc + "')";
			
			dbConn.insertToDB(gps_lat, gps_long, gps_desc);
			Assert.AreNotEqual(sqlQuery, "INSERT INTO nodesx (gps_lat, gps_long, gps_desc) VALUES (" + gps_lat + ", " 
			                + gps_long + ", '" + gps_desc + "')");
			}

			[Test]
			public void InsertToDBTest(){
				dbConn = new DatabaseConnect();
				
				float gps_lat = 25f;
				float gps_long = 30f;
				string gps_desc = "Test Location";
				string tableName = "nodes";
				
				string sqlQuery = "INSERT INTO " + tableName + " (gps_lat, gps_long, gps_desc) VALUES (" + gps_lat + ", " 
					+ gps_long + ", '" + gps_desc + "')";
				
				dbConn.insertToDB(gps_lat, gps_long, gps_desc);
				Assert.AreEqual(sqlQuery, "INSERT INTO nodes (gps_lat, gps_long, gps_desc) VALUES (" + gps_lat + ", " 
				                   + gps_long + ", '" + gps_desc + "')");
			}

			[Test]
			public void getMaxDistanceTest(){
				dbConn = new DatabaseConnect();
				
					float distanceNode = 500f;

					Assert.AreEqual(distanceNode, dbConn.maxDistance);


			}
			[Test]
			public void deleteFromDBErrorTest(){
				dbConn = new DatabaseConnect();
				
				int gps_id = 2;
				
				string sqlQuery = "DELETE FROM nodes WHERE _id=" + gps_id;
				
				dbConn.deleteLocation(gps_id);
				Assert.AreNotEqual(sqlQuery, "DELETE FROM nodes WHERE _id=" + 3);
			}

			[Test]
			public void deleteFromDBTest(){
				dbConn = new DatabaseConnect();
				
				int gps_id = 2;
				
				string sqlQuery = "DELETE FROM nodes WHERE _id=" + gps_id;
				
				dbConn.deleteLocation(gps_id);
				Assert.AreEqual(sqlQuery, "DELETE FROM nodes WHERE _id=" + 2);
			}
		
	}

	[TestFixture]
		public class DatabaseTests{

		DatabaseDownload dbDown;

		[Test]
		public void didDatabaseDownloadTest(){
			dbDown = new DatabaseDownload();

			Assert.IsNotNull(dbDown.Start());
		}

		[Test]
		public void didDatabaseDownloadErrorTest(){
			dbDown = null;
			Assert.IsNull(dbDown);
		}

		[Test]
		public void getDatabasePathErrorTest(){
			string fullPath = "http://thisisafailingtest.com";
			string serverURL = dbDown.url;

			Assert.AreNotSame(fullPath,serverURL);
		}
		[Test]
		public void getDatabasePathTest(){
			string fullPath = "http://www.nzwheelsonline.com/AHCI/gpsnodes.sqlite";
			string serverURL = dbDown.url;
			
			Assert.AreSame(fullPath,serverURL);
		}
	}
}