using UnityEngine;
using System.Collections;

public class LobbyController : MonoBehaviour
{
	public GameObject BackDrop;
	//COINTANER
	public GameObject quickJoinContainer;
	public GameObject optionContainer;
	public GameObject instructionContainer;
	public GameObject shopContainer;
	public GameObject shopInAppContainer;
	public GameObject collectionCardContainer;
	public GameObject rankContainer;

	//CloseBtn
	public UIButton closequickJoin;
	public UIButton closeBtnOption;
	public UIButton closeBtnInstruction;
	public UIButton closeBtnShop;
	public UIButton closeBtnShopInApp;
	public UIButton closeBtnCollectionCard;

	public UIButton quickJoin;
	public UIButton ShopBtn;
	public UIButton btn_Practice;

	//Option Button
	public UIButton[] optionsBtn;
	//Bottom
	public UIButton coinsBtn;
	public UIButton gemBtn;
	public UIButton heartBtn;
	//Tab
	public GameObject[] tabview;
	public UIButton[] tabBtn;
	public UI2DSprite[] tabSprite;

	private GameObject temp;
	private bool onTab;
	private float speed = 2f;

	// Use this for initialization
	void Awake ()
	{	
		///------ practice ------///
		EventDelegate.Add(btn_Practice.onClick, delegate() { 
			CardGameManager._INSTANCE.FadeScreen(delegate()
			{
				CardGame.LoadScene(SceneName.Tutorial); 
			});
		}); 
		 
		///------ quick join ------///
		EventDelegate.Add (quickJoin.onClick, delegate() {
			OnOpen (quickJoinContainer);
		});

		EventDelegate.Add (closequickJoin.onClick, delegate() {
			OnClose (quickJoinContainer);
		});

		///------ option ------///
		EventDelegate.Add (optionsBtn [0].onClick, delegate() {
			OnOpen (optionContainer);
		});

		EventDelegate.Add (closeBtnOption.onClick, delegate() {
			OnClose (optionContainer);
		});

		///------ instruction ------///
		EventDelegate.Add (optionsBtn [3].onClick, delegate() {
			OnOpen (instructionContainer);
		});
		
		EventDelegate.Add (closeBtnInstruction.onClick, delegate() {
			OnClose (instructionContainer);
		});

		///------ shop ------///
		EventDelegate.Add (ShopBtn.onClick, delegate() {
			OnOpen (shopContainer);
			OnTab ();
		});

		EventDelegate.Add (closeBtnShop.onClick, delegate() {
			OnClose (shopContainer); 
			CardGameManager._INSTANCE.FadeScreen (delegate() {
				CardGame.LoadScene (SceneName.Lobby);
			}); 
		});

		///------ shop in app ------///
		EventDelegate.Add (tabBtn [0].onClick, delegate() {
			OnTab (0);
		});
		EventDelegate.Add (tabBtn [1].onClick, delegate() {
			OnTab (1);
		});
		EventDelegate.Add (coinsBtn.onClick, delegate() {
			OnOpen (shopInAppContainer);
			OnTab (0);
		});
		EventDelegate.Add (gemBtn.onClick, delegate() {
			OnOpen (shopInAppContainer);
			OnTab (1);
		});
		EventDelegate.Add (heartBtn.onClick, delegate() {
			OnOpen (shopInAppContainer);
		});
		EventDelegate.Add (closeBtnShopInApp.onClick, delegate() {
			OnClose (shopInAppContainer);
			OnCloseTab ();
		});

		///------ Collection of Cards ------///
		EventDelegate.Add (optionsBtn [2].onClick, delegate() {
			OnOpen (collectionCardContainer);
		});

		EventDelegate.Add (closeBtnCollectionCard.onClick, delegate() {
			OnClose (collectionCardContainer); 
		});


		ResizeCointainer (quickJoinContainer); 
		ResizeCointainer (optionContainer); 
		ResizeCointainer (instructionContainer); 
		ResizeCointainer (shopContainer); 
		ResizeCointainer (shopInAppContainer); 
		ResizeCointainer (collectionCardContainer); 
	}

	void ResizeCointainer (GameObject obj)
	{
		obj.transform.localScale = new Vector3 (.8f, .8f, .8f);
	}


	void OnOpen (GameObject obj)
	{	
		SoundControl.shared.PlayEffectSound ("Click");
		obj.SetActive (true);
		temp = obj;
		DISABLEDALL (obj);
		BackDrop.SetActive (true);
		PopupTweenOpen (obj);
	}

	void PopupTweenOpen (GameObject obj)
	{
		iTween.ScaleTo (obj, iTween.Hash (
			"scale", new Vector3 (1f, 1f, 1f), 
			"speed", speed, 
			"easetype", iTween.EaseType.easeInOutBack
		));
	}

	void OnClose (GameObject obj)
	{	
		SoundControl.shared.PlayEffectSound ("Click");
		PopupTweenClose (obj);
		BackDrop.SetActive (false);

		TimerAlarm.LazyTimer (.2f, delegate() {
			obj.SetActive (false);
			ENABLEDALL ();
		});
	}

	//Switch Tab
	void OnCloseTab ()
	{	
		TimerAlarm.LazyTimer (.2f, delegate() {
			for (int i = 0; i < tabview.Length; i++) {
				tabview [i].gameObject.SetActive (false);
				tabSprite [i].enabled = false;
				//onTab = false;
			}
		});

	}

	void PopupTweenClose (GameObject obj)
	{
		iTween.ScaleTo (obj, iTween.Hash (
			"scale", new Vector3 (.8f, .8f, .8f), 
			"speed", speed, 
			"easetype", iTween.EaseType.easeInOutBack
		));
	}

	void ENABLEDALL ()
	{
		temp.SetActive (false); 
		quickJoin.enabled = true;
		ShopBtn.enabled = true;
		coinsBtn.enabled = true;
		gemBtn.enabled = true;
		rankContainer.GetComponent < BoxCollider> ().enabled = true;

		foreach (var item in optionsBtn) {
			item.enabled = true;
		}
	}

	void DISABLEDALL (GameObject obj)
	{
		quickJoin.enabled = false;
		ShopBtn.enabled = false;
		coinsBtn.enabled = false;
		gemBtn.enabled = false;
		rankContainer.GetComponent < BoxCollider> ().enabled = false;

		foreach (var item in optionsBtn) {
			if (item != obj) {
				item.enabled = false;
			}
		}
	}

	//First Open Tab
	void OnTab ()
	{	
		onTab = true;

		for (int i = 0; i < tabview.Length; i++) {
			if (i != 0) {
				tabview [i].gameObject.SetActive (false);
				tabSprite [i].enabled = false;
			} else {
				tabview [i].gameObject.SetActive (true);
				tabSprite [i].enabled = true;
			}
		}
	}

	//Switch Tab
	void OnTab (sbyte tab)
	{	
		for (int i = 0; i < tabview.Length; i++) {

			if (tab == i) {
				tabview [i].gameObject.SetActive (true);
				tabSprite [i].enabled = true;
			} else {
				tabview [i].gameObject.SetActive (false);
				tabSprite [i].enabled = false;
			}	
		}
			
	}


}
