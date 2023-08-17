//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class UIFade : MonoBehaviour {

	[Header ("Fade UI Elements")]
	CanvasGroup Canvas_Group;
	public bool TextIn;
	public bool TextOut;

	void Start()
	{
		Canvas_Group = this.GetComponent<CanvasGroup>();
	}

	void Update()
	{
		if(TextIn)
		{
			Canvas_Group.alpha += Time.deltaTime;
		}

		if(TextOut)
		{
			Canvas_Group.alpha -= Time.deltaTime;	
		}
    }

    public void FadeIn()
	{
		TextIn = true;
		TextOut = false;
	}

	public void FadeOut()
	{
		TextOut = true;
		TextIn = false;
	}    
}