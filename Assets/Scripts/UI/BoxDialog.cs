using UnityEngine;
using System.Collections;
using System;

public class BoxDialog : MonoBehaviour
{

	private static BoxDialog instance;

	public static BoxDialog _INSTANCE {
		get {
			return instance;
		}
		private set {
			if (instance == null) {
				lock ("instance BoxDialog lock") {
					if (instance == null) {
						instance = value;
						instance.name = "BoxDialog Insatnce";
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

	public GameObject dialogPanelPrefab;

	public GameObject messagDialogPrefab;

	private const int DIALOG_PANEL_DEPTH = 20;
	private const string DIALOG_PANEL_NAME = "DialogPanel";

	private GameObject uiRoot;
	private GameObject dialogPanel;
	private GameObject blocker;

	private int totalShow = 0;

	private GameObject GetDialogPanel {
		get {
			if (dialogPanel == null) {
				uiRoot = GameObject.Find ("UI Root");
				dialogPanel = NGUITools.AddChild (uiRoot, dialogPanelPrefab);
				dialogPanel.name = DIALOG_PANEL_NAME;
				dialogPanel.GetComponent<UIPanel> ().depth = DIALOG_PANEL_DEPTH;
				blocker = dialogPanel.transform.GetChild (0).gameObject;
			}
			return dialogPanel;
		}
	}

	private void Awake ()
	{
		_INSTANCE = this;
	}

	private Action ManageSelf (Action okCallback)
	{
		return delegate() {
			if (okCallback != null)
				okCallback ();
			--totalShow;
			if (totalShow <= 0) {
				blocker.SetActive (false);
			}
		};
	}

	public MessageDialog ShowMessageDialog (string title, string text_1, string text_2, string text_3, string okBtnLabel, Action okCallback)
	{
		MessageDialog tmp = NGUITools.AddChild (GetDialogPanel, messagDialogPrefab).GetComponent<MessageDialog> ();
		tmp.Panel.depth = DIALOG_PANEL_DEPTH + ++totalShow;
		tmp.Setup (title, text_1, text_2, text_3, okBtnLabel, ManageSelf (okCallback));
		blocker.SetActive (true);
		return tmp;
	}


}
