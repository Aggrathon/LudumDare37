using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : MonoBehaviour {

	public bool startInMenu = true;

	void Start()
	{
		SetMenuActive(startInMenu);
	}

	public static void SetMenuActive(bool state)
	{
		if(state)
		{
			Time.timeScale = 0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
		{
			Time.timeScale = 1f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void OnDestroy()
	{
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
