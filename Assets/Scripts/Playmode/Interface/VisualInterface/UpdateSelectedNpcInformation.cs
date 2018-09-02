using System.Collections;
using System.Collections.Generic;
using Playmode.Npc;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSelectedNpcInformation : MonoBehaviour
{
	private NpcController npcSelected;
	private Text healthText;
	private string baseText;

	private void Awake()
	{
//		healthText = GameObject.Find("NpcInformation").GetComponentInChildren<Text>();
//		baseText = healthText.text;
	}

	public void RegisterNpc(NpcController npcSelected)
	{
//		this.npcSelected = npcSelected;
	}
	
	// Update is called once per frame
	void Update () {
//		if (npcSelected != null)
//		{
//			int nbHealth = npcSelected.GetHealth();
//			healthText.text = baseText + " " + nbHealth;
//		}
//		else
//		{
//			healthText.text = baseText;
//		}
	}
}
