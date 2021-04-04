
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Command : MonoBehaviour
{
	enum State
	{
		Off,
		Selected,
		Active,
		None
	};

	[SerializeField] Image m_off;
	[SerializeField] Image m_selected;
	[SerializeField] Image m_active;
	[SerializeField] TextMeshProUGUI m_text;

	Action m_action;

	public void SetAction( Action action )
	{
		m_action = action;

		var label = m_action.GetLabel();

		m_text.text = label;

		UpdateState( State.Off );
	}

	public Action GetAction()
	{
		return m_action;
	}

	public bool HasAction()
	{
		return m_action != null;
	}

	public void ClearAction()
	{
		m_action = null;

		m_text.text = "";

		UpdateState( State.None );
	}

	public void SetOff()
	{
		UpdateState( State.Off );
	}

	public void SetSelected()
	{
		UpdateState( State.Selected );
	}

	public void SetActive()
	{
		UpdateState( State.Active );
	}

	void UpdateState( State state )
	{
		m_off.gameObject.SetActive( false );
		m_selected.gameObject.SetActive( false );
		m_active.gameObject.SetActive( false );

		switch ( state )
		{
			case State.Off:
				m_off.gameObject.SetActive( true );
				break;

			case State.Selected:
				m_selected.gameObject.SetActive( true );
				break;

			case State.Active:
				m_active.gameObject.SetActive( true );
				break;
		}
	}
}
