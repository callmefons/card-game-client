using UnityEngine;
using System.Collections;

public class MonoWithUpdateDelegate<T> : MonoBehaviour where T : class
{

	protected delegate void UpdateDelegate ();

	protected UpdateDelegate updateDelegate;

	protected virtual void Update ()
	{
		if (updateDelegate != null) {
			updateDelegate ();
		}
	}

	public bool isIgnoreTimeScale = false;

	public float deltaTime {
		get { 
			return isIgnoreTimeScale ? ToolIgnoreTimeScale.Shared.realTimeDelta : Time.deltaTime; 
		}
	}
}
