
using UnityEngine;

public class Operations : DoorPanelController
{
	public const int c_welcomePageIndex = 0;
	public const int c_noticesPageIndex = 1;
	public const int c_evaluationPageIndex = 2;

	public const int c_welcomeNoticesButtonIndex = 0;
	public const int c_welcomeEvaluationButtonIndex = 1;
	public const int c_welcomeExitButtonIndex = 2;

	public const int c_noticesMoreButtonIndex = 3;
	public const int c_noticesPreviousButtonIndex = 4;
	public const int c_noticesNextButtonIndex = 5;
	public const int c_noticesQuitButtonIndex = 6;

	public const int c_evaluationMoreButtonIndex = 7;
	public const int c_evaluationPreviousButtonIndex = 8;
	public const int c_evaluationNextButtonIndex = 9;
	public const int c_evaluationQuitButtonIndex = 10;

	// notice screen components
	[SerializeField] Notices m_notices;

	protected override void Initialize()
	{
	}

	protected override void Restart()
	{
		ShowWelcomePage( false );
	}

	void Update()
	{
		switch ( m_currentPageIndex )
		{
			case c_noticesPageIndex:
				m_notices.Update();
				break;
		}
	}

	void ShowWelcomePage( bool makeNoise = true )
	{
		SetCurrentPage( c_welcomePageIndex, makeNoise, true );
		SetCurrentButton( c_welcomeNoticesButtonIndex, false );
	}

	void ShowNoticesPage()
	{
		SetCurrentPage( c_noticesPageIndex );

		m_notices.Initialize( this );
	}

	void ShowEvaluationPage()
	{
		SetCurrentPage( c_evaluationPageIndex );

		// temporary
		EnableButton( c_evaluationMoreButtonIndex, false );
		EnableButton( c_evaluationPreviousButtonIndex, false );
		EnableButton( c_evaluationNextButtonIndex, false );
		SetCurrentButton( c_evaluationQuitButtonIndex, false );
	}

	public override void OnFire()
	{
		base.OnFire();

		switch ( m_currentButtonIndex )
		{
			case c_welcomeNoticesButtonIndex:
				ShowNoticesPage();
				break;

			case c_welcomeEvaluationButtonIndex:
				ShowEvaluationPage();
				break;

			case c_welcomeExitButtonIndex:
				Exit();
				break;

			case c_noticesMoreButtonIndex:
				m_notices.ShowNextLine();
				break;

			case c_noticesPreviousButtonIndex:
				m_notices.ShowPreviousNotice();
				break;

			case c_noticesNextButtonIndex:
				m_notices.ShowNextNotice();
				break;

			case c_noticesQuitButtonIndex:
				ShowWelcomePage();
				break;

			case c_evaluationQuitButtonIndex:
				ShowWelcomePage();
				break;
		}
	}

	public override void OnCancel()
	{
		base.OnCancel();

		switch ( m_currentButtonIndex )
		{
			case c_welcomeNoticesButtonIndex:
			case c_welcomeEvaluationButtonIndex:
			case c_welcomeExitButtonIndex:
				Exit();
				break;

			case c_noticesMoreButtonIndex:
			case c_noticesPreviousButtonIndex:
			case c_noticesNextButtonIndex:
			case c_noticesQuitButtonIndex:
			case c_evaluationQuitButtonIndex:
				ShowWelcomePage();
				break;
		}
	}
}
