using System.Reflection;
using log4net;

namespace Game.Logic.Actions;

public class CheckPVEGameStateAction : IAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_time;

	private bool m_isFinished;

	public CheckPVEGameStateAction(int delay)
	{
		m_time = TickHelper.GetTickCount() + delay;
		m_isFinished = false;
	}

	public void Execute(BaseGame game, long tick)
	{
		if (m_time > tick || game.GetWaitTimer() >= tick)
		{
			return;
		}
		if (game is PVEGame pVEGame)
		{
			switch (pVEGame.GameState)
			{
			case eGameState.Inited:
				pVEGame.Prepare();
				break;
			case eGameState.Prepared:
				pVEGame.PrepareNewSession();
				break;
			case eGameState.Loading:
				if (pVEGame.IsAllComplete())
				{
					pVEGame.StartGame();
				}
				else
				{
					game.WaitTime(1000);
				}
				break;
			case eGameState.GameStartMovie:
				if (game.CurrentActionCount > 1)
				{
					pVEGame.StartGameMovie();
				}
				else
				{
					pVEGame.StartGame();
				}
				break;
			case eGameState.GameStart:
				pVEGame.PrepareNewGame();
				break;
			case eGameState.Playing:
				if ((pVEGame.CurrentLiving != null && pVEGame.CurrentLiving.IsAttacking) || game.CurrentActionCount > 1)
				{
					break;
				}
				if (pVEGame.CanGameOver())
				{
					if (pVEGame.IsLabyrinth() && pVEGame.CanEnterGate)
					{
						pVEGame.GameOverMovie();
					}
					else
					{
						pVEGame.GameOver();
					}
				}
				else
				{
					pVEGame.NextTurn();
				}
				break;
			case eGameState.GameOver:
				if (pVEGame.HasNextSession())
				{
					pVEGame.PrepareNewSession();
				}
				else
				{
					pVEGame.GameOverAllSession();
				}
				break;
			case eGameState.SessionPrepared:
				if (pVEGame.CanStartNewSession())
				{
					pVEGame.StartLoading();
				}
				else
				{
					game.WaitTime(1000);
				}
				break;
			case eGameState.ALLSessionStopped:
				if (pVEGame.PlayerCount == 0 || pVEGame.WantTryAgain == 0)
				{
					pVEGame.Stop();
				}
				else if (pVEGame.WantTryAgain == 1)
				{
					pVEGame.ShowDragonLairCard();
					pVEGame.PrepareNewSession();
				}
				else if (pVEGame.WantTryAgain == 2)
				{
					pVEGame.SessionId--;
					pVEGame.PrepareNewSession();
				}
				else
				{
					game.WaitTime(1000);
				}
				break;
			}
		}
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}
