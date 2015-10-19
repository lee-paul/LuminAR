package com.LuminAR.Project;
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
         * Gps location listener.
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
         * Network location listener.
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
        myLocationManager.removeUpdates(networkLocationListener);
        myLocationManager.removeUpdates(gpsLocationListener);
        super.onPause();
        mSensorManager.unregisterListener(mListener);
    }
    @Override
    protected void onStop() {
        if (Config.DEBUG)
            Log.d(TAG, "onStop");
        myLocationManager.removeUpdates(networkLocationListener);
        myLocationManager.removeUpdates(gpsLocationListener);
        mSensorManager.unregisterListener(mListener);
        super.onStop();
    }
    /** Returns the users location **/
    public static String getLocation(){
        if(currentLocation!=null){
            return "Latitude: " + currentLocation.getLatitude() + "\nLongitude: " + currentLocation.getLongitude();
        } else {
            return "Unknown";
        }
    }
    
    /**Creates a new Location node and store in array of created Location Nodes**/
    public static void createLocation(float gps_lat, float gps_long, String gps_desc){
    	Location newLocation;
    	newLocation = new Location(gps_desc);
    	newLocation.setLatitude(gps_lat);
    	newLocation.setLongitude(gps_long);
    	
    	storeLocation(newLocation);

    }
    
    public static void storeLocation(Location location){
    	arrayOfLocations.add(location);
    }
    
    public static void editLocation(int id, float gps_lat, float gps_long, String gps_desc){
    	Location editLocation = new Location(gps_desc);
    	editLocation.setLatitude(gps_lat);
    	editLocation.setLongitude(gps_long);
    	arrayOfLocations.set(id, editLocation);
    }
	
	public static void deleteLocation(int id){
		arrayOfLocations.remove(id);
	}
    
    public static String getLocationArray(){
    	return arrayOfLocations.toString();
    }
    
    public static Location getLocationAt(int id){
    	Location findLocation;
    	
    	findLocation = arrayOfLocations.get(id);
    	
    	return findLocation;
    	
    }
    
    public static String getDistanceTo(float setDistance){
    	float distances[] = new float[arrayOfLocations.size()];
    	for (int i = 0;i<arrayOfLocations.size();i++){
    		Location dest = getLocationAt(i);
    		float distanceInMeters = currentLocation.distanceTo(dest);
    		if (distanceInMeters < setDistance){
    			distances[i] = currentLocation.distanceTo(dest);
    		}
    	}
    	return Arrays.toString(distances);
    }
        
    
    /**Bearing Stuff**/
    public static float getX() {
    	return xmag;
	}

	public static float getY() {
    	return ymag;
	}

	public static float getZ() {
    	return zmag;
	}
	public static float getDegrees(){
		return degrees;
	}
}