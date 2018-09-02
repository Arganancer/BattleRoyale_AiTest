using System.Collections;
using System.Collections.Generic;
using Playmode.Npc;
using UnityEngine;

public class MouseClickHandler : MonoBehaviour
{
	private const string UPDATER_OBJECT_NAME = "NpcInformationUpdater";
	private const string NPC_MASK_NAME = "NPC";
	private UpdateSelectedNpcInformation updateSelectedNpcInformation;
	private RaycastHit2D hit;
	private CircleCollider2D npcClickCollider2D;
	private Vector3 mouse;
	private LayerMask npcLayerMask;
	private void Awake()
	{
		//updateSelectedNpcInformation = GameObject.Find(UPDATER_OBJECT_NAME).GetComponent<UpdateSelectedNpcInformation>();
		npcClickCollider2D = GetComponent<CircleCollider2D>();
		mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouse.z = 0;
		npcLayerMask = LayerMask.GetMask(NPC_MASK_NAME);
	}

	private void OnMouseDown()
	{
		updateSelectedNpcInformation.RegisterNpc(transform.root.GetComponent<NpcController>());
	}

	private void Update()
	{
		mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		MouseEvents();
	}

	private void MouseEvents()
	{
//		if (Input.GetMouseButtonDown(0))
//		{
//			hit = Physics2D.Raycast(mouse,Vector2.zero,0,npcLayerMask);
//			if (hit)
//			{
//				updateSelectedNpcInformation.RegisterNpc(hit.transform.gameObject.transform.root.GetComponentInChildren<NpcController>());
//			}
//		}
		
	}
}
