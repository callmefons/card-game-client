using UnityEngine;
using System.Collections;

public class Options : MonoBehaviour
{	
	
	public UIToggle musicToggle;
	public UIButton musicBtn;
	public UILabel musicText;
	public UIToggle sfxToggle;
	public UIButton sfxBtn;
	public UILabel sfxText;

	void Awake ()
	{	
		musicToggle.value = PrefManager._INSTANCE.GetMusic ();
		sfxToggle.value = PrefManager._INSTANCE.GetSfx();

		musicText.text = musicToggle.value?"On":"Off";
		sfxText.text = sfxToggle.value?"On":"Off";

		EventDelegate.Add (musicBtn.onClick, delegate() {
			SetMusic ();
		});

		EventDelegate.Add (sfxBtn.onClick, delegate() {
			SetSfx ();
		});

	}

	public void SetMusic ()
	{	
		PrefManager._INSTANCE.SetMusic (musicToggle.value);
		musicText.text = musicToggle.value?"On":"Off"; 
		SoundControl.shared.SetMusic (musicToggle.value);
	}

	public void SetSfx ()
	{		
		PrefManager._INSTANCE.SetSfx (sfxToggle.value); 
		sfxText.text = sfxToggle.value?"On":"Off";
		SoundControl.shared.SetSfx (sfxToggle.value);
	}

}
