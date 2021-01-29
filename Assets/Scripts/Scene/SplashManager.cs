using UnityEngine;
using System.Collections;
using System;

public class SplashManager : MonoBehaviour {

	public TweenAlpha splashAlpha;

	private void Awake()
	{
		unsafe { StarManager._INSTANCE.OnReceiveData    = DataCallback; }
		StarManager._INSTANCE.OnStatusChange            = StatusChangeCallback;
		StarManager._INSTANCE.OnStarError               = ErrorCallback;

		EventDelegate.Add(splashAlpha.onFinished, OnSplashFinished, true);
	}


	private void OnSplashFinished()
	{
		StartCoroutine(Wait(0.5f, delegate()
		{	
			CardGameManager._INSTANCE.FadeScreen(delegate() 
				{
					CardGame.LoadScene (SceneName.Login); 
				});
			
		}));
	}

	private IEnumerator Wait(float sec, Action action)
	{
		while (StarManager._INSTANCE.ClientStatus != SCStatus.READY)
		{
			yield return null;
		}
		yield return new WaitForSeconds(sec);
		if (action != null)
		{
			action();
		}
	}

	private unsafe void DataCallback(int idx, void* buffer, uint size)
	{
		print("SplashManager DataCallback");
	}

	private void StatusChangeCallback(SCStatus changed)
	{
		print("SplashManager StatusChangeCallback");
		if (changed == SCStatus.READY)
		{
			print("READY");
			splashAlpha.PlayForward();
		}
	}

	private void ErrorCallback(SCError error, string msg)
	{
		print("SplashManager ErrorCallback");
	}
}
