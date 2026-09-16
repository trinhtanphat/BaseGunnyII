using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Actions
{
    public class CheckPVPGameStateAction:IAction
    {
        private long m_tick;
        private bool m_isFinished;
        private eGameState m_scheduledState;

        public CheckPVPGameStateAction(int delay, eGameState scheduledState)
        {
            m_isFinished = false;
            m_scheduledState = scheduledState;
            m_tick += TickHelper.GetTickCount() + delay;
        }

        public void Execute(BaseGame game, long tick)
        {
            
            if (m_tick <= tick)
            {
                PVPGame pvp = game as PVPGame;
                if (pvp != null)
                {
                    if (game.GameState != m_scheduledState)
                    {
                        m_isFinished = true;
                        return;
                    }

                    switch (game.GameState)
                    {
                        case eGameState.Inited:
                            pvp.Prepare();
                            break;
                        case eGameState.Prepared:
                            pvp.StartLoading();
                            break;
                        case eGameState.Loading:
                            if (pvp.IsAllComplete())
                            {
                                pvp.StartGame();
                            }
                            break;
                        case eGameState.Playing:
                            if (pvp.CanGameOver())
                            {
                                pvp.GameOver();
                            }
                            else if (pvp.CurrentPlayer == null || pvp.CurrentPlayer.IsAttacking == false)
                            {
                                pvp.NextTurn();
                            }
                            break;
                        case eGameState.GameOver:
                            if (m_scheduledState == eGameState.GameOver)
                            {
                                pvp.Stop();
                            }
                            break;
                    }
                }
                m_isFinished = true;
            }
        }

        public bool IsFinished(long tick)
        {
            return m_isFinished;
        }
    }
}
