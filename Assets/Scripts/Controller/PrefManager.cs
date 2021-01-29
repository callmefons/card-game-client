using UnityEngine;
using System.Collections;
using System;

public class PrefManager : MonoBehaviour
{
	private static PrefManager instance;

	public static PrefManager _INSTANCE {
		get {
			return instance;
		}
		private set {
			if (instance == null) {
				lock ("instance CardGameManager lock") {
					if (instance == null) {
						instance = value;
						instance.name = "PrefManager Insatnce";
						DontDestroyOnLoad (value.gameObject);
					} else {
						Debug.Log ("instance already set");
						Destroy (value.gameObject);
					}
				}
			} else {
				Debug.Log ("instance already set");
				Destroy (value.gameObject);
			}
		}
	}

	public static string KEY_VOLUME = "VOLUMNE";
	public static string KEY_MUSIC = "MUSIC";
	public static string KEY_SFX = "SFX";

	public static bool Lauched;
	public static bool FirstTime;
	public static int Lifes;
	public static int CapOfLife = 5;
	public static float RestLifeTimer;
	public static string DateOfExit;
	public static DateTime today;
	public static DateTime DateofRestLife;
	public static string timeForReps;

	private void Awake ()
	{
		_INSTANCE = this;

		RestLifeTimer = PlayerPrefs.GetFloat ("RestLifeTimer");
		DateOfExit = PlayerPrefs.GetString ("DateOfExit", "");
		Lifes = PlayerPrefs.GetInt ("Lifes");
		if (PlayerPrefs.GetInt ("Lauched") == 0) {   
			FirstTime = true;
			Lifes = CapOfLife;
			PlayerPrefs.SetInt ("Lifes", Lifes);
			PlayerPrefs.SetInt ("Lauched", 1); 
			PlayerPrefs.Save ();
		}

	}

	public void SetVolume (float volume)
	{
		PlayerPrefs.SetFloat (KEY_VOLUME, volume);
	}

	public float GetVolume ()
	{
		return PlayerPrefs.GetFloat (KEY_VOLUME); 
	}

	public void SetMusic (bool music)
	{
		PlayerPrefs.SetInt (KEY_MUSIC, music?1:0);
	}

	public bool GetMusic ()
	{
		return PlayerPrefs.GetInt (KEY_MUSIC)==1?true:false; 
	}

	public void SetSfx (bool sfx)
	{
		PlayerPrefs.SetInt (KEY_SFX, sfx?1:0);
	}

	public bool GetSfx ()
	{
		return PlayerPrefs.GetInt (KEY_SFX)==1?true:false; 
	}

	public void RestoreLifes ()
	{
		Lifes = CapOfLife;
		PlayerPrefs.SetInt ("Lifes", Lifes);
		PlayerPrefs.Save ();
	}

	public void AddLife (int count)
	{
		Lifes += count;
		if (Lifes > CapOfLife)
			Lifes = CapOfLife;
		PlayerPrefs.SetInt ("Lifes", Lifes);
		PlayerPrefs.Save ();
	}

	public int GetLife ()
	{
		if (Lifes > CapOfLife) {
			Lifes = CapOfLife;
			PlayerPrefs.SetInt ("Lifes", Lifes);
			PlayerPrefs.Save ();
		}
		return Lifes;
	}

	public void SpendLife (int count)
	{
		if (Lifes > 0) {
			Lifes -= count;
			PlayerPrefs.SetInt ("Lifes", Lifes);
			PlayerPrefs.Save ();
		}
	}

	void OnApplicationPause (bool pauseStatus)
	{
		if (pauseStatus) {
			if (RestLifeTimer > 0) {
				PlayerPrefs.SetFloat ("RestLifeTimer", RestLifeTimer);
			}
			PlayerPrefs.SetInt ("Lifes", Lifes);
			PlayerPrefs.SetString ("DateOfExit", DateTime.Now.ToString ());
			PlayerPrefs.Save ();
		}
	}

	void OnDisable ()
	{
		PlayerPrefs.SetFloat ("RestLifeTimer", RestLifeTimer);
		PlayerPrefs.SetInt ("Lifes", Lifes);
		if (Application.loadedLevel != 3)
			PlayerPrefs.SetString ("DateOfExit", DateTime.Now.ToString ());
		PlayerPrefs.Save ();
	}

}

