using UnityEngine;
using System.Collections;

public enum EasingType
{
	LinearTween = 0,
	EaseInQuad = 1,
	EaseOutQuad = 2,
	EaseInOutQuad = 3,
	EaseOutInQuad = 4,
	EaseInCubic = 5,
	EaseOutCubic = 6,
	EaseInOutCubic = 7,
	EaseOutInCubic = 8,
	EaseInQuart = 9,
	EaseOutQuart = 10,
	EaseInOutQuart = 11,
	EaseOutInQuart = 12,
	EaseInQuint = 13,
	EaseOutQuint = 14,
	EaseInOutQuint = 15,
	EaseOutInQuint = 16,
	EaseInSine = 17,
	EaseOutSine = 18,
	EaseInOutSine = 19,
	EaseOutInSine = 20,
	EaseInExpo = 21,
	EaseOutExpo = 22,
	EaseInOutExpo = 23,
	EaseOutInExpo = 24,
	EaseInCirc = 25,
	EaseOutCirc = 26,
	EaseInOutCirc = 27,
	EaseOutInCirc = 28,
	EaseInElastic = 29,
	EaseOutElastic = 30,
	EaseInOutElastic = 31,
	EaseOutInElastic = 32,
	EaseInBack = 33,
	EaseOutBack = 34,
	EaseInOutBack = 35,
	EaseOutInBack = 36,
	EaseInBounce = 37,
	EaseOutBounce = 38,
	EaseInOutBounce = 39,
	EaseOutInBounce = 40
}

public class EasingCalculator
{

	public static float Calculate (EasingType easingType, float t, float b, float c, float d)
	{	

		return Equations.ChangeFloat (t, b, c, d, (Ease)((int)easingType));		
	}
}

public class ParentEasing : MonoWithUpdateDelegate<ParentEasing>
{

	void Awake ()
	{
		isIgnoreTimeScale = true;	
	}

	public delegate void FinishEasingDelegate ();

	public FinishEasingDelegate finishEasingDelegate;

	public void StopEasing ()
	{
		timePos = duration;	
		updateDelegate = null;		
		finishEasingDelegate = null;
		lerpPos = 1;
	}

	public void ForceEndEasing ()
	{
		timePos = duration;	
		if (updateDelegate != null) {
			updateDelegate ();	
		}			
	}

	public bool isClearDelegateAfterFinish = true;
	protected EasingType easingType = EasingType.LinearTween;
	protected float timePos = 0;
	protected float duration = 1;
	protected float lerpPos = 0;

	protected void DoEasing ()
	{
		timePos = 0;
		updateDelegate = ActionUpdate;
	}

	public void DoEasing (float _duration)
	{
		duration = _duration;
		timePos = 0;
		updateDelegate = ActionUpdate;
	}

	protected void ActionUpdate ()
	{

		timePos += deltaTime;		
		if (timePos >= duration) {
			timePos = duration;	
			updateDelegate = null;
			if (finishEasingDelegate != null) {
				finishEasingDelegate ();
				if (isClearDelegateAfterFinish) {
					finishEasingDelegate = null;
				}
			}
		}
		lerpPos = EasingCalculator.Calculate (easingType, timePos, 0f, 1f, duration);
	}

}
