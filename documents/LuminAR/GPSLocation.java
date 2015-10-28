package com.LuminAR.Project;
import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Iterator;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Set;
import java.util.TreeSet;

import com.unity3d.player.UnityPlayerActivity;

import android.content.ContentValues;
import android.content.Context;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.util.Config;
import android.util.Log;

public class GPSLocation extends UnityPlayerActivity {
    private static final String TAG = "GPS_Unity";
    private static final String TAGCOMPASS = "Compass";
    
    /** Stores the current location */
    public static Location currentLocation;
    public static LocationManager myLocationManager;
    
    /** Listeners for the gps and network location */
    static LocationListener networkLocationListener;
    static LocationListener gpsLocationListener;
    static SensorEventListener mListener;
    
    /**Stuff for magnetic bearing**/
    public static SensorManager mSensorManager;
    public static Sensor mSensor;

    public static float xmag;
    public static float ymag;
    public static float zmag;
    public static float degrees;
    
    /**Array for created Locations**/
    static ArrayList<Location> arrayOfLocations = new ArrayList<Location>();
    static ArrayList<Location> oldArray = new ArrayList<Location>();
    
    public static void startEventListeners(){
		/**
		* This function starts the event listeners.
		* The event listeners are used to find the bearing of the user.
		* The even listen uses the sensors in the device, in this case to get the bearing it uses the magnetometer. To find magnetic NORTH, SOUTH etc.
		*/
	    mListener = new SensorEventListener() {
	    		public void onSensorChanged(SensorEvent event) {
	    		if (Config.DEBUG) Log.d(TAGCOMPASS, "sensorChanged (" + event.values[0] + ", " + event.values[1] + ", " + event.values[2] + ")");
		    		xmag = event.values[0];
		    		ymag = event.values[1];
		    		zmag = event.values[2];
		    		float degree = Math.round(event.values[0]);
		    		degrees = degree;
	    		}
	
	    		public void onAccuracyChanged(Sensor sensor, int accuracy) {
	    		}
	    };
    }
    
    /** Starts the GPS stuff */
    public static void startLocationListeners() {
        /**
         * This function starts the location listener.
		 * The location listenser is used to find the location of the user.
		 * The location is found by accessing the device's GPS hardware and tracks a location.		
         */
        gpsLocationListener = new LocationListener() {
            public void onLocationChanged(Location location) {
                currentLocation = location;
                
                double latitude = currentLocation.getLatitude();
                double longitude = currentLocation.getLongitude();
                
                Log.i("Geo_Location", "Latitude: " + latitude + ", Longitude: " + longitude);
            }
            public void onProviderDisabled(String provider) {
            }
            public void onProviderEnabled(String provider) {
            }
            public void onStatusChanged(String provider, int status,
                    Bundle extras) {
            }
        };
        /**
         * The function also uses a network location listener.
		 * The network location listener is used to find the device's location using the network of the device.
		 * The location is set by the network and is not as accurate as the device's GPS sensor hardware.
         */
        networkLocationListener = new LocationListener() {
            public void onLocationChanged(Location location) {
                currentLocation = location;
                
                double latitude = currentLocation.getLatitude();
                double longitude = currentLocation.getLongitude();
                
                Log.i("Geo_Location", "Latitude: " + latitude + ", Longitude: " + longitude);
            }
            public void onProviderDisabled(String provider) {
            }
            public void onProviderEnabled(String provider) {
            }
            public void onStatusChanged(String provider, int status,
                    Bundle extras) {
            }
        };
        myLocationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER,0, 0,
                networkLocationListener);
        myLocationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 0, 0,
                gpsLocationListener);
    }
    @Override
    protected void onCreate(Bundle myBundle) {
		/**
		* This function assigns the services to be started and starts the location and event listeners.
		* This is started when the plugin is loaded.
		*/
        super.onCreate(myBundle);
        myLocationManager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);
        mSensorManager = (SensorManager)getSystemService(Context.SENSOR_SERVICE);
        mSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION);
        // Starts the listeners
        startLocationListeners();
        startEventListeners();
    }
    @Override
    protected void onResume() {
		/**
		* If there is a connection loss then when the application comes back it will resume the listeners.
		*/
        if (Config.DEBUG)
            Log.d(TAG, "onResume");
        super.onResume();
        mSensorManager.registerListener(mListener, mSensor, SensorManager.SENSOR_DELAY_GAME);
        startLocationListeners();
        startEventListeners();
    }
    @Override
    protected void onPause()
    {
		/**
		* When the conneciton is paused the listeners should be paused to.
		* This makes sure there is an effecient use of the listeners.
		*/
        myLocationManager.removeUpdates(networkLocationListener);
        myLocationManager.removeUpdates(gpsLocationListener);
        super.onPause();
        mSensorManager.unregisterListener(mListener);
    }
    @Override
    protected void onStop() {
		/**
		* When the application is stopped the listeners must be stopped too. For obvious reasons.
		*/
        if (Config.DEBUG)
            Log.d(TAG, "onStop");
        myLocationManager.removeUpdates(networkLocationListener);
        myLocationManager.removeUpdates(gpsLocationListener);
        mSensorManager.unregisterListener(mListener);
        super.onStop();
    }
    /** Returns the users location **/
    public static String getLocation(){
		/**
		* This function uses the location listeners to retrieve the location of the user's device.
		*/
        if(currentLocation!=null){
            return "Latitude: " + currentLocation.getLatitude() + "\nLongitude: " + currentLocation.getLongitude();
        } else {
            return "Unknown";
        }
    }
    
    /**Creates a new Location node and store in array of created Location Nodes**/
    public static void createLocation(float gps_lat, float gps_long, String gps_desc){
		/**
		* This function is called by the Unity Plugins to create a new location.
		* The location parameters are passed through the Unity Plugins.
		* An array is created to store the locations.
		* @param gps_lat This is the value of the GPS LATITUDE to be stored, passed in from the unity plugin.
		* @param gps_long This is the value of the GPS LONGITUDE to be stored, passed in from the unity plugin.
		* @param gps_desc This is the value of the GPS DESCRIPTION to be stored, passed in from the unity plugin.
		* @see DatabaseConnect.cs
		* @see DatabaseDownload.cs
		* @see DatabaseFunctions.cs
		*/
    	Location newLocation;
    	newLocation = new Location(gps_desc);
    	newLocation.setLatitude(gps_lat);
    	newLocation.setLongitude(gps_long);
    	
    	storeLocation(newLocation);

    }
    
    public static void storeLocation(Location location){
		/**
		* This function is called to store the inserted gps coordinates in to an array.
		*/
    	arrayOfLocations.add(location);
    }
    
    
    public static void editLocation(int id, float gps_lat, float gps_long, String gps_desc){
    	/**
		* This function is called by the Unity Plugins to edit a location.
		* The location parameters are passed through the Unity Plugins.
		* @param id This is the value of the GPS ID that will be edited, passed in from the unity plugin.
		* @param gps_lat This is the value of the GPS LATITUDE to be stored, passed in from the unity plugin.
		* @param gps_long This is the value of the GPS LONGITUDE to be stored, passed in from the unity plugin.
		* @param gps_desc This is the value of the GPS DESCRIPTION to be stored, passed in from the unity plugin.
		* @see DatabaseFunctions.cs
		*/
		
		Location editLocation = new Location(gps_desc);
    	editLocation.setLatitude(gps_lat);
    	editLocation.setLongitude(gps_long);
    	arrayOfLocations.set(id, editLocation);
    }
	
	public static void deleteLocation(int id){
		/**
		* This function is used to delete a gps node at a given ID.
		* @param id This value is the id of the gps node to delete in the array.
		*/
		arrayOfLocations.remove(id);
	}
    
    public static String getLocationArray(){
		/**
		* This function is used to retrieve the array of nodes.
		*/
    	return arrayOfLocations.toString();
    }
    
    public static Location getLocationAt(int id){
		/**
		* This funciton is used to get a location at the id that is specified.
		* @param id This value is set and called from another function.
		*/
    	Location findLocation;
    	
    	findLocation = arrayOfLocations.get(id);
    	
    	return findLocation;
    	
    }
    
    public static String getDistanceTo(float setDistance){
		/**
		* This function is used to get the distances of the user from the stored gps nodes.
		* @param setDistance This value is set from the Unity Plugin to set the maximum distance of nodes to show.
		*/
    	DecimalFormat df = new DecimalFormat("#.00");
    	StringBuilder distances = new StringBuilder();
    	float bearing;
    	for (int i = 0;i<arrayOfLocations.size();i++){
    		Location dest = getLocationAt(i);
    		float distanceInMeters = currentLocation.distanceTo(dest);
    		bearing = getBearing(currentLocation, dest);
    		if (distanceInMeters < setDistance){
    			distances.append("Node ID: " + i + " is " + df.format(currentLocation.distanceTo(dest)) + "m away."
    					+ "\nAt a bearing: " + bearing + " east of north.\n");
    		}
    	}
    	return distances.toString();
    }
    
   public static String getAllNodes(){
	   /**
	    * This function is used to get all the nodes that are in the database.
	    */
	   float bearing;
    	StringBuilder nodes = new StringBuilder();
    	for (int i = 0;i<arrayOfLocations.size();i++){
    		Location locationNode = getLocationAt(i);
    		bearing = getBearing(currentLocation, locationNode);
    		nodes.append("GPS ID = " + i + "\nGPS Latitude = " + locationNode.getLatitude() 
    					+ "\nGPS Longitude = " + locationNode.getLongitude()
    					+ "\nBearing = " + bearing + "\n");
    	}
    	return nodes.toString();
    }
   
   public static float getBearing(Location myLocation, Location destination){
	   return myLocation.bearingTo(destination);
   }
    
    
    /**Bearing Stuff**/
    public static float getX() {
    	return xmag;
	}
    /**Bearing Stuff**/
	public static float getY() {
    	return ymag;
	}
	/**Bearing Stuff**/
	public static float getZ() {
    	return zmag;
	}
	/**Bearing Stuff**/
	public static float getDegrees(){
		return degrees;
	}
}