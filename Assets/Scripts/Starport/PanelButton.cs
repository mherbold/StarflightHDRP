
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PanelButton
{
	[SerializeField] public Image m_buttonImage;
	[SerializeField] public TextMeshProUGUI m_buttonTMP;

	[SerializeField] public int m_leftButtonIndex = -1;
	[SerializeField] public int m_rightButtonIndex = -1;
	[SerializeField] public int m_upButtonIndex = -1;
	[SerializeField] public int m_downButtonIndex = -1;

	[SerializeField] public int m_index = 0;
	[SerializeField] public bool m_enabled = true;
	[SerializeField] public bool m_fireOnSelect = false;
}
