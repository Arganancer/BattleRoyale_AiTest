using Playmode.Npc;
using UnityEngine;
using UnityEngine.UI;

namespace Playmode.Interface.VisualInterface
{
	public class UiNpcCardController : MonoBehaviour
	{
		private NpcController npcController;

		private Text behaviorText;
		private Text healthText;
		private Text stateText;
		private Text positionText;

		private void Awake()
		{
			var texts = GetComponentsInChildren<Text>();
			behaviorText = texts[0];
			healthText = texts[1];
			stateText = texts[2];
			positionText = texts[3];
		}

		private void Update()
		{
			if (npcController == null)
			{
				//BEN_CORRECTION : SerializeFields.
				healthText.text = "0";
				stateText.text = "dead";
				stateText.color = Color.red;
				positionText.text = "";
			}
			else
			{
				healthText.text = npcController.GetHealth().ToString();
				stateText.text = npcController.GetStateString();
				positionText.text = "x: " + (int)npcController.transform.position.x + ", y: " +
				                    (int)npcController.transform.position.y;
			}
		}

		public void Configure(NpcController npcControllerToConfigure)
		{
			this.npcController = npcControllerToConfigure;
			behaviorText.text = npcControllerToConfigure.GetStrategyString();
		}
	}
}