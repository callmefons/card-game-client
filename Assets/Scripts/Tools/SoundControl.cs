using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SoundControl : MonoBehaviour
{
	public static bool soundEnable = true;
	//----------------------------------------------------------------------
	// Singlaton
	//----------------------------------------------------------------------
	private static SoundControl sharedInstance;

	public static SoundControl shared {
		get {
			if (sharedInstance == null) {
				GameObject go = new GameObject ("Sound Manager");
				sharedInstance = (SoundControl)go.AddComponent<SoundControl> ();
				AudioSource loopAudioSource = (AudioSource)go.AddComponent<AudioSource> ();
				loopAudioSource.playOnAwake = false;
				loopAudioSource.loop = true;
				loopAudioSource.mute = PrefManager._INSTANCE.GetMusic ()?false:true;
				GameObject oneShot = new GameObject ("One Shot");
				sharedInstance.soundOneShotTransform = oneShot.transform;
				sharedInstance.soundOneShotTransform.transform.parent = go.transform;
				sharedInstance = (SoundControl)go.GetComponent<SoundControl> ();
				sharedInstance.Initialize ();
			}
			return 	sharedInstance;
		}
	}

	//----------------------------------------------------------------------
	//
	//----------------------------------------------------------------------
	public Transform soundOneShotTransform;
	private AudioSource audioOne;
	private Dictionary<string,AudioClip> sfxDictionary = new Dictionary<string, AudioClip> ();

	public void Initialize ()
	{
		audioOne = (AudioSource)soundOneShotTransform.gameObject.AddComponent<AudioSource> ();
		audioOne.playOnAwake = false;
		audioOne.loop = false;
		Object[] soundResources = Resources.LoadAll ("Sounds");
		for (int i = 0; i < soundResources.Length; ++i) {
			sfxDictionary.Add (soundResources [i].name, (AudioClip)soundResources [i]);				
		}
	}


	public void PlayBackgroundSound (string key)
	{
		GetComponent<AudioSource> ().Stop ();
		GetComponent<AudioSource> ().clip = sfxDictionary [key];
		GetComponent<AudioSource> ().Play ();
		GetComponent<AudioSource>().volume = 1.0f;
	}


	public void PlayEffectSound (string key)
	{
		audioOne.PlayOneShot (sfxDictionary [key]);
	}


	public void SetMusic (bool mute)
	{
		GetComponent<AudioSource> ().mute = mute?false:true;
	}

	public void SetSfx (bool mute)
	{
		audioOne.mute = mute?false:true; 
	}

	public void SetVolume (float volume)
	{
		GetComponent<AudioSource> ().volume = volume;
	}
}
