using UnityEngine;
using System.Collections;
using System;

public class CardGameManager : MonoBehaviour
{

	private static CardGameManager instance;

	public static CardGameManager _INSTANCE {
		get {
			return instance;
		}
		private set {
			if (instance == null) {
				lock ("instance CardGameManager lock") {
					if (instance == null) {
						instance = value;
						instance.name = "CardGameManager Insatnce";
						DontDestroyOnLoad (value.gameObject);

						StarManager._INSTANCE.InitStar (ConnectionType.TCP, new StarClientVersion (1, 8, 0));
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

	public CharactersData[] charactersData;
	public GameObject overlayPrefab;
	private const int OVERLAY_DEPTH = 50;
	private const string OVERLAY_NAME = "Overlay";
	private GameObject overlay;
	private GameObject uiRoot;
	private TweenAlpha cacheFade;

	public TweenAlpha fade {
		get {
			if (cacheFade == null) {
				uiRoot = GameObject.Find ("UI Root");
				overlay = NGUITools.AddChild (uiRoot, overlayPrefab);
				overlay.name = OVERLAY_NAME;
				overlay.GetComponent<UIPanel> ().depth = OVERLAY_DEPTH;
				cacheFade = overlay.transform.FindChild ("Fade").GetComponent<TweenAlpha> ();
			}
			return cacheFade;
		}
	}

	private void Awake ()
	{
		_INSTANCE = this;
	}

	public void Connect ()
	{
		if (StarManager._INSTANCE.ClientStatus == SCStatus.READY) {
			StarManager._INSTANCE.Connect (CardGame.IP, CardGame.PORT);
		}
	}

	public void InitFade ()
	{
		fade.gameObject.SetActive (true);
		EventDelegate.Add (fade.onFinished, delegate() {
			fade.gameObject.SetActive (false);
		}, true);
	}

	public void FadeFirstOpen ()
	{
		fade.enabled = true;
		fade.PlayForward ();
	}

	public void FadeScreen (Action action)
	{
		fade.gameObject.SetActive (true);
		EventDelegate.Add (fade.onFinished, delegate() {
			if (action != null)
				action ();
			EventDelegate.Add (fade.onFinished, delegate() {
				fade.gameObject.SetActive (false);
			}, true);

			fade.PlayForward ();
		}, true);

		fade.PlayReverse ();
	}

	private void Update ()
	{
		if (Input.GetKeyUp (KeyCode.Escape)) {
			Application.Quit ();
		}
	}
}
