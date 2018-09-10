using Playmode.Util.Values;
using UnityEngine;
using UnityEngine.UI;

namespace MenuEvent
{
	//BEN_REVIEW : Le DropDown est toujours réinitialisé à 12 ? C'est vraiment ce que vous vouliez ?
	public class DropdownEventHandler : MonoBehaviour
	{
		private Dropdown dropdown;

		private void Start()
		{
			dropdown = GetComponent<Dropdown>();
			dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
			int.TryParse(dropdown.options[dropdown.value].text, out GameValues.NbOfEnemies);
		}

		private static void DropdownValueChanged(Dropdown change)
		{
			int.TryParse(change.options[change.value].text, out GameValues.NbOfEnemies);
		}
	}
}