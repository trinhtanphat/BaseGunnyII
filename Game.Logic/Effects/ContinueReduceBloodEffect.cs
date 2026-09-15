using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness.Managers;
using Game.Logic.Spells;
using Bussiness;


namespace Game.Logic.Effects
{
    public class ContinueReduceBloodEffect : AbstractEffect
    {
        private int m_count;
        private int m_blood;
        private Living m_liv;
        private bool m_sourceAware;
        public ContinueReduceBloodEffect(int count,int blood)
            : base(eEffectType.ContinueReduceBloodEffect)
        {
            m_count = count;
            m_blood = blood;
        }

        public ContinueReduceBloodEffect(int count, int blood, Living liv)
            : base(eEffectType.ContinueReduceBloodEffect)
        {
            m_count = count;
            m_blood = blood;
            m_liv = liv;
            m_sourceAware = true;
        }

        public override bool Start(Living living)
        {
            ContinueReduceBloodEffect effect = living.EffectList.GetOfType(eEffectType.ContinueDamageEffect) as ContinueReduceBloodEffect;
            if (effect != null)
            {
                effect.m_count = m_count;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(player_BeginFitting);
            living.Game.SendPlayerPicture(living, 2, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            living.Game.SendPlayerPicture(living, 2, false);
        }

        void player_BeginFitting(Living living)
        {
            if (m_sourceAware)
            {
                m_count--;
                if (m_count < 0)
                {
                    Stop();
                    return;
                }
                living.AddBlood(-m_blood, 1);
                if (living.Blood <= 0)
                {
                    living.Die();
                    if (m_liv is Player)
                    {
                        (m_liv as Player).PlayerDetail.OnKillingLiving(m_liv.Game, 2, living.Id, living.IsLiving, m_blood);
                    }
                }
                return;
            }

            m_count--;
            if (living is Player)
            {
                Player p = (living as Player);

                if (p.Blood < Math.Abs(m_blood))
                {
                    p.AddBlood(-p.Blood + 2);
                }
                else
                {
                    p.AddBlood(m_blood);
                }
            }
            if (m_count < 0)
            {
                Stop();
            }
        }

    }
}
