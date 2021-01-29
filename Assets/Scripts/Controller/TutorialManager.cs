using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {
	public UITexture pileCard;
	public GameObject hangObj;
	public Transform handPos_1;
	public Transform handPos_2;
	// Use this for initialization
	public UIButton btn_1;
	public UIButton btn_2;
	public UIButton btn_back;
	public UILabel  label_1;
	public UILabel  label_2;
	public UITexture character_1;
	public UITexture character_2;

	void Awake(){
		SoundControl.shared.PlayBackgroundSound("TutorialSound");
		EventDelegate.Add(btn_1.onClick, delegate() { Rule1(); });
		EventDelegate.Add(btn_2.onClick, delegate() { Rule2(); });
		EventDelegate.Add(btn_back.onClick, delegate() { 
			CardGameManager._INSTANCE.FadeScreen(delegate()
			{
				CardGame.LoadScene(SceneName.Lobby); 
			});
		}); 
	}

	public void Rule1 () {
		SoundControl.shared.PlayEffectSound ("Click");
		pileCard.mainTexture = Resources.Load ("Cards/" + 0 + "/"+ 0) as Texture;
		hangObj.transform.position = handPos_2.position;
		label_1.gameObject.SetActive(false);
		character_1.gameObject.SetActive(false);
		label_2.gameObject.SetActive(true);
		character_2.gameObject.SetActive(true);
	}
	public void Rule2 () {
		SoundControl.shared.PlayEffectSound ("Click");
		pileCard.mainTexture = Resources.Load ("Cards/" + 0 + "/"+ 79) as Texture;
		hangObj.transform.position = handPos_1.position;
		label_1.gameObject.SetActive(true);
		character_1.gameObject.SetActive(true);
		label_2.gameObject.SetActive(false);
		character_2.gameObject.SetActive(false);
	}
}
