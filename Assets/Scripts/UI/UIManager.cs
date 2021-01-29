using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
	
	public GameManager gameManager;

	//Reference from GameManager
	private GameSeat[] seats;
	private UIButton[] buttons;
	private UITexture pileCard;

	//Major Panels
	public GameObject UIGame;
	public GameObject UILobby;
	public GameObject UIPostMatch;

	public UITexture BackgroundTexture;

	//Lobby State
	public UILabel gameState;

	//Post Match State
	private TweenAlpha fadeTween;

	//Effects
	public Transform smokePrefab;
	public GameObject plusFx;
	public GameObject wrongFx;
	public UI2DSprite _fx_trueborder;

	public GameObject[] buttonsFx = new GameObject[5];
	public Transform[] startPosition = new Transform[5];
	public Transform[] seatsPos = new Transform[4];
	public Transform[] materialSeatPos = new Transform[4];
	public Transform[] seatsEndPos = new Transform[4];
	public Transform coinsPos;
	public GameObject coin;
	private float speed = .5f;

	static Random random = new Random ();

	void Awake ()
	{	
		//game manger
		gameManager = gameManager.GetComponent<GameManager> ();
		seats = gameManager.seats; 
		buttons = gameManager.buttons; 
		pileCard = gameManager.pileCard;

		//all panel
		SetActive (UIGame, false);
		SetActive (UILobby, true);
		SetActive (UIPostMatch, false);
		SetActive (BackgroundTexture.gameObject, false);

	}

	public void SettingLayout (sbyte layout)
	{
		BackgroundTexture.mainTexture = Helper.Load<Texture> ("Layouts/" + layout.ToString ()); 

		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].GetComponent<UITexture> ().mainTexture = Helper.Load<Texture> ("Cards/Material_" + layout + "/" + i.ToString ()); 
			buttonsFx [i].GetComponent<UITexture> ().mainTexture = Helper.Load<Texture> ("Cards/Fx_" + layout + "/" + i.ToString ());
		}
	}

	public void OnInitGame ()
	{	
		CardGameManager._INSTANCE.FadeScreen (delegate() {

			SetActive (UIGame, true);
			SetActive (UILobby, false);
			SetActive (UIPostMatch, false);
			SetActive (BackgroundTexture.gameObject, true);
			for (int i = 0; i < seats.Length; i++) {
				seats [i].transform.position = seatsPos [i].position;
				seats [i].totalCardSrite.gameObject.SetActive (true);
			}

		});
			

		TimerAlarm.LazyTimer (.5f, delegate() {
			for (sbyte i = 0; i < buttons.Length; i++) {
				buttons [i].gameObject.SetActive (true);
				buttons [i].GetComponent<UIButton> ().enabled = false;
				//buttons [i].transform.position = startPosition [Random.Range (0, startPosition.Length)].position;
				//MoveToBorn (i, buttons [i].transform.rotation);
			}

			Helper.Shuffle (startPosition); 
			for (sbyte i = 0; i < buttons.Length; i++) {
				buttons [i].transform.SetParent (startPosition [i].transform);  
				buttons [i].transform.localPosition = new Vector2 (0f, 0f);  
				Debug.Log ("startPosition [r]  " + startPosition [i].name);  
			} 

		});
	}

	private void MoveToBorn (sbyte index, Quaternion rot)
	{
		

		/*iTween.MoveTo (buttons [index].gameObject, iTween.Hash (
			"position", startPosition [index].transform,   
			"time", speed,
			"onCompleteTarget", this.gameObject,
			"onComplete", "onCompleteInit",
			"oncompleteparams", index,
			"easetype", iTween.EaseType.easeInOutBack
		));

		iTween.ScaleTo (buttons [index].gameObject, iTween.Hash (
			"scale", new Vector3 (1f, 1f, 1f), 
			"speed", speed, 
			"easetype", iTween.EaseType.easeInOutBack
		));

		iTween.RotateTo (buttons [index].gameObject, iTween.Hash (
			"x", this.transform.eulerAngles.z - rot.x,
			"y", this.transform.eulerAngles.z - rot.y,
			"z", this.transform.eulerAngles.z - rot.z,
			"time", .5,
			"easetype", iTween.EaseType.easeOutSine
		));*/
	}

	private void onCompleteInit (sbyte index)
	{	
		iTween.RotateTo (buttonsFx [index].gameObject, iTween.Hash (
			"x", this.transform.eulerAngles.x + 0,
			"y", this.transform.eulerAngles.y + 0,
			"z", this.transform.eulerAngles.z + 0,
			"time", .5,
			"easetype", iTween.EaseType.easeOutSine
		));
	}

	bool initGame = true;

	public void ItweenFoldCard ()
	{
		if (initGame) {
			gameManager.onCompleteDrawCard = true;
			initGame = false;
		} else {
			pileCard.mainTexture = Helper.Load<Texture> ("Cards/" + 80); 
			iTween.RotateBy (pileCard.gameObject, 
				iTween.Hash ("y", 0.25f,
					"time", 0.25,
					"onCompleteTarget", this.gameObject,
					"onComplete", "onCompleteFromFold"
				)
			);	
		}
	}

	void onCompleteFromFold ()
	{
		Debug.Log ("onCompleteFromFold");
		pileCard.mainTexture = Helper.Load<Texture> ("Cards/" + 80); 
		iTween.RotateBy (pileCard.gameObject,
			iTween.Hash ("y", -0.25f,
				"time", 0.25,
				"onCompleteTarget", this.gameObject,
				"onComplete", "onCompleteFromDrawCard"
			)
		);

		gameManager.onCompleteDrawCard = true;
	}

	public  void ItweenDrawCard ()
	{
		iTween.RotateBy (pileCard.gameObject, 
			iTween.Hash ("y", 0.5f,
				"time", 1
			)
		);
	}

	public bool nextRound = false;

	public void Move (int btn, int seat)
	{	
		//Debug.Log ("Move Button[" + btn + "] to seat " + seat);
		buttonsFx [btn].gameObject.SetActive (true);

		Instantiate (smokePrefab, buttons [btn].transform.position, Quaternion.identity); 


		iTween.MoveTo (buttonsFx [btn].gameObject, iTween.Hash (
			"position", materialSeatPos [seat].transform,   
			"time", speed,
			"onCompleteTarget", this.gameObject,
			"onComplete", "onCompleteMove",
			"oncompleteparams", btn,
			"easetype", iTween.EaseType.easeInOutBack
		));


	}

	public void MoveTrue (int index, int listener)
	{    
		Debug.Log ("index " + index + " listener " + listener);
		MoveToCoin ();
		for (int i = 0; i < seats.Length; i++) {
			if (seats [i].userId == listener) {
				Move (index, i);
				seats [i].correctIcon.gameObject.SetActive (true);
				NGUITools.AddChild (seats [i].gameObject, plusFx);
				seats [i].wrongIcon.gameObject.SetActive (false);
				TimerAlarm.LazyTimer (.1f, delegate() {
					SoundControl.shared.PlayEffectSound ("CorrectAnswer");
				});

			} else {
				seats [i].wrongIcon.gameObject.SetActive (true);
				NGUITools.AddChild (seats [i].gameObject, wrongFx);

			}
		}
	}

	public void MoveFalse (int index, int listener)
	{    
		Debug.Log ("index " + index + " listener " + listener);

		for (int i = 0; i < seats.Length; i++) {
			if (seats [i].userId == listener) { 
				Move (index, i);
				seats [i].wrongIcon.gameObject.SetActive (true);
				NGUITools.AddChild (seats [i].gameObject, wrongFx);
				TimerAlarm.LazyTimer (.1f, delegate() {
					SoundControl.shared.PlayEffectSound ("WrongAnswer");
				});
			} else {

				if (seats [i].wrongIcon.gameObject.activeSelf) { 
					seats [i].wrongIcon.gameObject.SetActive (true);
				} else {
					seats [i].wrongIcon.gameObject.SetActive (false);
				}
			}
		}
	}

	private void onCompleteMove (int _index)
	{	
		buttonsFx [_index].gameObject.GetComponent<UITexture> ().alpha = 0.5f;  

	}


	public void ResetMove ()
	{
		for (int i = 0; i < seats.Length; i++) {
			seats [i].wrongIcon.gameObject.SetActive (false);
			seats [i].correctIcon.gameObject.SetActive (false);
		}

		for (int i = 0; i < buttonsFx.Length; i++) {
			buttonsFx [i].transform.position = startPosition [i].transform.position;
			buttonsFx [i].gameObject.GetComponent<UITexture> ().alpha = 1f; 
			buttonsFx [i].gameObject.SetActive (false);
		}
	}

	public void MoveToCoin ()
	{	
		coin.gameObject.SetActive (true); 
		coin.gameObject.transform.position = pileCard.transform.position;

		TimerAlarm.LazyTimer (.1f, delegate() {
			SoundControl.shared.PlayEffectSound ("CoinRoomSound");
		});

		iTween.MoveTo (coin.gameObject, iTween.Hash (
			"position", coinsPos.transform,   
			"time", speed,
			"onCompleteTarget", this.gameObject,
			"onComplete", "onCompleteMoveToCoin",
			"easetype", iTween.EaseType.easeInOutBack
		));

		iTween.ScaleTo (coin.gameObject, iTween.Hash (
			"scale", new Vector3 (.6f, .6f, .6f), 
			"speed", speed, 
			"easetype", iTween.EaseType.easeInOutBack
		));

	}

	private void onCompleteMoveToCoin ()
	{	
		coin.transform.localScale = Vector3.one;
		coin.gameObject.SetActive (false);
		nextRound = false;

	}

	public void ShowResult ()
	{
		SetActive (UIGame, false);
		SetActive (UILobby, false);
		SetActive (UIPostMatch, true);
		OnShowResult ();
	}

	public void SetRank (sbyte i, sbyte rank)
	{	
		if (i == 0)
			seats [i].gameObject.transform.GetChild (0).gameObject.SetActive (false);

		seats [i].gameObject.transform.position = seatsEndPos [rank].position;
		//seats [i].uname.gameObject.transform.localPosition = new Vector3 (120f, 38f, 0f);
		seats [i].xp.gameObject.SetActive (true);
		seats [i].coins.gameObject.SetActive (true);
		seats [i].totalCardSrite.gameObject.SetActive (false);
	}

	public void OnPostMatch ()
	{
		
	}

	public  void OnShowResult ()
	{
		
	}

	public void SetActive (GameObject obj, bool active)
	{
		obj.SetActive (active);
	}
}
