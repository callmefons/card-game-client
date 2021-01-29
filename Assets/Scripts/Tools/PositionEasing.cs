using UnityEngine;
using System.Collections;

public class PositionEasing : ParentEasing
{

	private Vector3 beginPos = Vector3.zero;
	private Vector3 endPos = Vector3.zero;

	public void Easing (Vector3 _beginPos, Vector3 _endPos, float _duration, EasingType _easingType)
	{
		beginPos = _beginPos;
		endPos = _endPos;
		duration = _duration;
		easingType = _easingType;
		DoEasing ();
		updateDelegate += PositionUpdate;
		transform.localPosition = _beginPos;
	}

	private void PositionUpdate ()
	{
		transform.localPosition = Vector3.Lerp (beginPos, endPos, lerpPos);
	}

}