using UnityEngine;
using System.Collections;
using System;
public class LifesAddCounter : MonoBehaviour {


	public UILabel lifesTimer;
	public UILabel lifes;

	static float TimeLeft;
	float TotalTimeForRestLife = 10f * 60;  
	bool startTimer;
	DateTime templateTime;

	bool CheckPassedTime(){
		print (PrefManager.DateOfExit);
		if (PrefManager.DateOfExit == "" || PrefManager.DateOfExit == default(DateTime).ToString()) {
			PrefManager.DateOfExit = DateTime.Now.ToString ();
		}

		DateTime dateOfExit = DateTime.Parse (PrefManager.DateOfExit);

		if (DateTime.Now.Subtract (dateOfExit).TotalSeconds > TotalTimeForRestLife * (PrefManager.CapOfLife - PrefManager.Lifes)) {
			PrefManager._INSTANCE.RestoreLifes ();
			PrefManager.RestLifeTimer = 0;
			return false; //don't need lifes
		} else {
			TimeCount ((float)DateTime.Now.Subtract(dateOfExit).TotalSeconds);
			return true;
		}
	}


	void TimeCount(float trick){
		if (PrefManager.RestLifeTimer <= 0) ResetTimer ();

		PrefManager.RestLifeTimer -= trick;
		if (PrefManager.RestLifeTimer <= 1 && PrefManager.Lifes < PrefManager.CapOfLife) {
			PrefManager._INSTANCE.AddLife (1);
			ResetTimer ();
		}
	}

	void ResetTimer(){
		PrefManager.RestLifeTimer = TotalTimeForRestLife;
	}

	// Update is called once per frame
	void Update () {
		if (!startTimer && DateTime.Now.Subtract(DateTime.Now).Days == 0) {
			PrefManager.DateofRestLife = DateTime.Now;
			if (PrefManager.Lifes < PrefManager.CapOfLife) {
				if (CheckPassedTime()) startTimer = true;
				
			}
		}

		if (startTimer) TimeCount (Time.deltaTime);

		if (gameObject.activeSelf) {
			if (PrefManager.Lifes < PrefManager.CapOfLife) {
				int minutes = Mathf.FloorToInt (PrefManager.RestLifeTimer / 60f);
				int seconds = Mathf.FloorToInt (PrefManager.RestLifeTimer - minutes * 60);

				lifesTimer.enabled = true;
				lifesTimer.text = "" + string.Format ("{0:00}:{1:00}", minutes, seconds);
				PrefManager.timeForReps = lifesTimer.text;
			} else {
				lifesTimer.text = "FULL";

			}
		}

		lifes.text = "" + PrefManager._INSTANCE.GetLife();

	}

	void OnApplicationPause(bool pauseStatus) {
		if(pauseStatus){ 
			PrefManager.DateOfExit = DateTime.Now.ToString();
		}
		else{
			startTimer = false;
		}
	}

	void OnEnable(){
		startTimer = false;
	}

	void OnDisable(){
		PrefManager.DateOfExit = DateTime.Now.ToString();
	}
}
