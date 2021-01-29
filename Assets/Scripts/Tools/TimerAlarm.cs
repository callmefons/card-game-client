using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TimerAlarm : ToolMonoSinglaton<TimerAlarm>
{


	private List<TimerObj> timerObjs = new List<TimerObj> ();

	public void AddTimerObj (TimerObj node)
	{
		timerObjs.Add (node);
	}

	private List<TimerObj> timerObjToDelete = new List<TimerObj> ();

	private void UpdateTimerObjs ()
	{
		timerObjToDelete.Clear ();		
		for (int i = 0; i < timerObjs.Count; ++i) {
			timerObjs [i].UpdateTimer ();	
			if (timerObjs [i].isExpire) {
				timerObjToDelete.Add (timerObjs [i]);
			}
		}

		for (int i = 0; i < timerObjToDelete.Count; ++i) {
			timerObjs.Remove (timerObjToDelete [i]);
		}
	}

	public static TimerObj LazyTimer (float duration, TimerObj.FinishTimerDelegate _finishTimerDelegate)
	{
		return new TimerObj (duration, _finishTimerDelegate);	
	}

	public static TimerObj LazyScheduleTimer (float duration, TimerObj.FinishTimerDelegate _finishTimerDelegate)
	{
		TimerObj timer = new TimerObj (duration, _finishTimerDelegate);
		timer.isSchedule = true;
		return timer;
	}

	private  void SetupDelegate ()
	{
		updateDelegate = UpdateTimerObjs;
	}

	protected void Awake ()
	{
		SetupDelegate ();
	}
}

public class TimerObj
{

	public TimerObj (float _duration)
	{
		duration = _duration;
		TimerAlarm.Shared.AddTimerObj (this);
	}

	public TimerObj (float _duration, FinishTimerDelegate _finishTimerDelegate)
	{		
		finishTimerDelegate = _finishTimerDelegate;
		duration = _duration;
		TimerAlarm.Shared.AddTimerObj (this);
	}

	public void StopTimer ()
	{
		isExpire = true;	
	}

	private float timePos = 0;
	private float duration = 0;

	public delegate void FinishTimerDelegate ();

	public FinishTimerDelegate finishTimerDelegate;
	public bool isIgnoreTimeScale = false;

	public bool isSchedule = false;

	public bool isExpire = false;

	public void UpdateTimer ()
	{
		if (isExpire) {
			return;
		}
		timePos += isIgnoreTimeScale ? ToolIgnoreTimeScale.Shared.realTimeDelta : Time.deltaTime;
		if (timePos >= duration) {
			timePos = 0;

			if (finishTimerDelegate != null) {
				finishTimerDelegate ();					
			}

			if (!isSchedule) {
				isExpire = true;
			}
		}
	}
}
