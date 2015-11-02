using UnityEngine;
using System.Collections;

public class MyLog : MonoBehaviour
{
	static string myLog;
	static Queue myLogQueue = new Queue();
	public string output = "";
	public string stack = "";
	private bool hidden = true;
	private Vector2 scrollPos;
	public int maxLines = 30;
	
	void OnEnable()
	{
		Application.RegisterLogCallback(HandleLog);
	}
	
	void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}
	
	void HandleLog(string logString, string stackTrace, LogType type)
	{
		output = logString;
		stack = stackTrace;
		string newString = "\n [" + type + "] : " + output;
		myLogQueue.Enqueue(newString);
		if (type == LogType.Exception)
		{
			newString = "\n" + stackTrace;
			myLogQueue.Enqueue(newString);
		}
		
		while (myLogQueue.Count > maxLines)
		{
			myLogQueue.Dequeue();
		}
		
		myLog = string.Empty;
		foreach (string s in myLogQueue)
		{
			myLog += s;
		}
	}
	
	void OnGUI()
	{
		
		
		float w = Screen.width;
		float h = Screen.height;
		if (w > h) {
			w/=10; 
			h/=5;	
		} else {
			w/=5; 
			h/=10;	
		}

		GUIStyle customstyle = new GUIStyle("textarea");
		customstyle.fontSize = 40;

		GUIStyle customstyle2 = new GUIStyle("button");
		customstyle2.fontSize = 35;
		customstyle2.alignment = TextAnchor.MiddleCenter;
		
		if (!hidden)
		{
			GUI.skin.textArea.wordWrap = true;
			GUI.TextArea(new Rect(0, h + 20, Screen.width, Screen.height), myLog,customstyle);
			if (GUI.Button(new Rect(Screen.width - w -20, 10, w, h), "Hide Log", customstyle2))
			{
				hide(true);
			}
		}
		else
		{
			if (GUI.Button(new Rect(Screen.width - w -20, 10, w, h), "Show Log", customstyle2))
			{
				hide(false);
			}
		}
	}
	
	public void hide(bool shouldHide)
	{
		hidden = shouldHide;
	}
}