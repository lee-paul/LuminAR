using UnityEngine;
using System.Collections;
/** Accessing the GPS Data over JNI calls
 *  Please check the Java sources under Plugins/Android
 * */
public class GPSManager : MonoBehaviour {
	static string userLocation;
	AndroidJavaClass gpsActivityJavaClass;
	static float 	xValue;
	static float	yValue;
	static float	zValue;
	static float degrees;
	
#if !UNITY_EDITOR
	void Start () {
		AndroidJNI.AttachCurrentThread();
		gpsActivityJavaClass = new AndroidJavaClass("com.LuminAR.Project.GPSLocation");
	}
	void Update() {
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

	void OnGUI() {
		GUI.Label(new Rect(Screen.width / 2 -200, Screen.height / 2, 400,100), "xmag = " + xValue.ToString() + " ymag = " + yValue.ToString() + " zmag = " + zValue.ToString());
	}
#endif
}