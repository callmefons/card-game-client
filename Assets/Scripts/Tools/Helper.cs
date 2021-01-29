using UnityEngine;
using System.Collections;

///<sumary>
/// Generic Resources.Load() that errors on null
/// </sumary>
static class Helper {
	
	public static T Load<T>(string path) where T : Object {
		T thing = (T) Resources.Load(path);
		if (thing == null) 
			Debug.LogError("Couldn't load resource '"+typeof(T)+"' with path '"+path+"'");
			return thing;
	}

	public static void Shuffle<T>(T[] array)
	{
		int n = array.Length;
		for (int i = 0; i < n; i++)
		{
			// NextDouble returns a random number between 0 and 1.
			// ... It is equivalent to Math.random() in Java.
			int r = i + (int)(Random.Range(0,1f) * (n - i));
			T t = array[r];
			array[r] = array[i];
			array[i] = t;
		}
	}
}
