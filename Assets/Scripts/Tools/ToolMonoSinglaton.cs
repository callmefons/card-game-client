using UnityEngine;
using System;
using System.Collections;

public class ToolMonoSinglaton<T>:MonoWithUpdateDelegate<T>  where T : class
{

	#region Parent Object

	public const string GameObjectName = "Tool - Ultimate Stone";
	private static GameObject _parent;

	private static GameObject Parent {
		get {
			_parent = GameObject.Find (GameObjectName);
			if (_parent == null) {
				_parent = new GameObject (GameObjectName);
				DontDestroyOnLoad (_parent);
			}
			return _parent;
		}
	}

	#endregion

	private static bool isSaperateParent = true;

	private static T _instance = null;

	public static T Shared {
		get {

			if (_instance == null || _instance.Equals (default(T))) {
				GameObject go = isSaperateParent ? new GameObject ("Tool - " + typeof(T).Name) : Parent;
				_instance = go.AddComponent (typeof(T)) as T;				
			}
			return _instance;
		}
	}


	public static T GetInstance (GameObject go)
	{
		T instance = go.GetComponent (typeof(T)) as T;  
		if (instance == null) {
			instance = go.AddComponent (typeof(T)) as T;   
		}		
		return instance;
	}
}
