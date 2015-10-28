using UnityEngine;
using System.Collections;
/** Accessing the GPS Data over JNI calls
 *  Please check the Java sources under Plugins/Android
 * */
public class GPSManager : MonoBehaviour {
	/**
	* This class is the bridge between the Unity plugins and the JAVA plugin that does the Android functions.
	* The functions in this class are used by the other Plugins.
	* @see DatabaseConnect.cs
	* @see DatabaseDownload.cs
	* @see DatabaseFunctions.cs
	*/
	
	static string userLocation; /**< String that holds the user location in text.*/
	AndroidJavaClass gpsActivityJavaClass; /**< AndroidJavaClass that will access the .jar for android functions. */
	static float 	xValue; /**< Float that stores x value for the user's bearing.*/
	static float	yValue; /**< Float that stores y value for the user's bearing.*/
	static float	zValue; /**< Float that stores z value for the user's bearing.*/
	static float degrees; /**< Float that stores the bearing of the user in degrees (0-360).*/
	
#if !UNITY_EDITOR
	void Start () {
		/**
		* The start method that attaches the .jar plugin to the unity thread to access the java functions.
		*/
		AndroidJNI.AttachCurrentThread();
		gpsActivityJavaClass = new AndroidJavaClass("com.LuminAR.Project.GPSLocation");
	}
	void Update() {
		/**
		* The update functions in Unity is called multiple times a second.
		* This function is used to get the bearing of the user and display it updates with the update function call in unity.
		* This function also grabs the user location and displays that to the user. It also updates accordingly.
		**/
		GUIText setText;
		
		userLocation = gpsActivityJavaClass.CallStatic<string>("getLocation");

		xValue = gpsActivityJavaClass.CallStatic<float>("getX");
		yValue = gpsActivityJavaClass.CallStatic<float>("getY");
		zValue = gpsActivityJavaClass.CallStatic<float>("getZ");
		degrees = gpsActivityJavaClass.CallStatic<float>("getDegrees");

		
		if(userLocation!="Unknown"){
			setText = GameObject.Find("gps_user_location").GetComponent<GUIText>();
			setText.text = userLocation;
		} else {
			setText = GameObject.Find("gps_user_location").GetComponent<GUIText>();
			setText.text = "No Location.";
		}
		//Debug.Log("Compasses are " + xValue.ToString() + "," + yValue.ToString() + "," + zValue.ToString());
		setText = GameObject.Find("user_bearing").GetComponent<GUIText>();
		setText.text = "" + degrees;
	
	}


#endif
}