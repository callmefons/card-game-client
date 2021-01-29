using UnityEngine;
using System.Collections;

public class ToolIgnoreTimeScale : MonoBehaviour
{


	private static ToolIgnoreTimeScale instanceOfBonIgnoreTimeScale;

	public static ToolIgnoreTimeScale Shared {
		get {
			if (instanceOfBonIgnoreTimeScale == null) {
				GameObject go = new GameObject ("Tool - IgnoreTimeScale");
				instanceOfBonIgnoreTimeScale = (ToolIgnoreTimeScale)go.AddComponent<ToolIgnoreTimeScale> ();
			}
			return instanceOfBonIgnoreTimeScale;
		}
	}


	float mTimeStart = 0f;
	float mTimeDelta = 0f;
	float mActual = 0f;
	bool mTimeStarted = false;

	/// <summary>
	/// Equivalent of Time.deltaTime not affected by timeScale, provided that UpdateRealTimeDelta() was called in the Update().
	/// </summary>

	public float realTimeDelta { get { return mTimeDelta; } }

	/// <summary>
	/// Clear the started flag;
	/// </summary>

	protected virtual void OnEnable ()
	{
		mTimeStarted = true;
		mTimeDelta = 0f;
		mTimeStart = Time.realtimeSinceStartup;
	}

	void Update ()
	{
		UpdateRealTimeDelta ();
	}

	/// <summary>
	/// Update the 'realTimeDelta' parameter. Should be called once per frame.
	/// </summary>

	protected void UpdateRealTimeDelta ()
	{
		if (mTimeStarted) {
			float time = Time.realtimeSinceStartup;
			float delta = time - mTimeStart;
			mActual += Mathf.Max (0f, delta);
			mTimeDelta = 0.001f * Mathf.Round (mActual * 1000f);
			mActual -= mTimeDelta;
			if (mTimeDelta > 1f)
				mTimeDelta = 1f;
			mTimeStart = time;
		} else {
			mTimeStarted = true;
			mTimeStart = Time.realtimeSinceStartup;
			mTimeDelta = 0f;
		}
	}


}
