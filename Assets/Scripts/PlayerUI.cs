using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	[Header("Sprint")]
	public Image sprintbar;
	public Text sprintText;
	public float sprintFadeSpeed = 1f;
	float sprintLerp;


	FPSController player;

	void Start()
	{
		player = GameState.instance.player;
	}

	void Update()
	{
		if(player.sprintAmount < 1f)
		{
			sprintLerp += Time.deltaTime * sprintFadeSpeed;
			if (sprintLerp > 1f) sprintLerp = 1f;
			Color c = Color.white;
			c.a *= sprintLerp;
			sprintbar.color = c;
			sprintText.color = c;
			sprintbar.fillAmount = player.sprintAmount;
		}
		else
		{
			sprintLerp -= Time.deltaTime * sprintFadeSpeed;
			if (sprintLerp < 0f) sprintLerp = 0f;
			Color c = Color.white;
			c.a *= sprintLerp;
			sprintbar.color = c;
			sprintText.color = c;
		}
	}
}
