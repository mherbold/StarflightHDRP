
using TMPro;
using UnityEngine;

[RequireComponent( typeof( Collider ) )]
public class StarportDoorTrigger : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI m_doorNameTMP;
	[SerializeField] string m_doorName = "";
	[SerializeField] DoorPanelController m_doorPanelController;

	void OnTriggerEnter( Collider other )
	{
		m_doorNameTMP.text = m_doorName;

		m_doorPanelController.SetCurrentDoorPanel();
	}

	void OnTriggerExit( Collider other )
	{
		m_doorNameTMP.text = "";

		m_doorPanelController.UnsetCurrentDoorPanel();
	}
}
