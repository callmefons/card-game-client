using UnityEngine;
using System.Collections;

public class ItweenManager : MonoBehaviour {


	//PlayerSeats

	public GameObject UIGameplay;
	public GameObject[] seats;
	public Transform[] target;
	public float speed = 1f;
	private short max = 4;


	public UITexture pilecard;


	private static ItweenManager instance;
	
	public static ItweenManager _INSTANCE {
		get {
			return instance;
		}
		private set {
			if (instance == null) {
				lock ("instance ItweenManager lock") {
					if (instance == null) {
						instance = value;
						instance.name = "ItweenManager Insatnce";
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
	
	private void Awake ()
	{	
		UIGameplay.gameObject.SetActive (false);
		_INSTANCE = this;
	}

	void Start(){
		//ItweenMoveToTarget ();
	}
	public void ItweenMoveToTarget() {
		if (target==null){
			Debug.LogWarning("ConstantSpeedMove is missing a transform target");
		}else{
			for (int i = 0; i < max; i++) {
				iTween.MoveTo(seats[i],iTween.Hash(
					"position",target[i].position,
					"speed",speed,
					"onCompleteTarget", this.gameObject,
					"onComplete", "onCompleteFromiTween",
					"easetype",iTween.EaseType.easeInOutBack
					));
			}
		}
	}
	void onCompleteFromiTween () {
		UIGameplay.gameObject.SetActive (true);
		Debug.Log("ConstantSpeedMove done");
	}

	public void ItweenFoldCard(){
		iTween.RotateBy(pilecard.gameObject, 
			iTween.Hash("y",0.25f,
				"time",0.25,
				"onCompleteTarget", this.gameObject,
				"onComplete", "onCompleteFromFold"
			)
		);
	}

	void onCompleteFromFold(){
		Debug.Log("onCompleteFromFold");
		pilecard.mainTexture = Resources.Load ("Cards/" + 60) as Texture;
		iTween.RotateBy(pilecard.gameObject,
			iTween.Hash("y",-0.25f,
				"time",0.25,
				"onCompleteTarget", this.gameObject,
				"onComplete", "onCompleteFromDrawCard"
			)
		);
	}
	public  void ItweenDrawCard(){
		iTween.RotateBy(pilecard.gameObject, 
			iTween.Hash("y",0.5f,
				"time",1
			)
		);
	}

}
