using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Bussiness;

namespace Game.Server.Quests
{
    /// <summary>
    /// 18、公会人数/空/具体人数
    /// 触发条件：客户端当前公会有新加入用户时触发、登陆
    /// </summary>
    public class OwnConsortiaCondition:BaseCondition
    {
        private GamePlayer m_player;

        public OwnConsortiaCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(GamePlayer player)
        {
            m_player = player;
            RefreshValue(player);
            player.GuildChanged += new GamePlayer.PlayerOwnConsortiaEventHandle(player_OwnConsortia);
        }

        void player_OwnConsortia()
        {
            if (m_player != null)
            {
                RefreshValue(m_player);
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.GuildChanged -= new GamePlayer.PlayerOwnConsortiaEventHandle(player_OwnConsortia);
            if (object.ReferenceEquals(m_player, player))
            {
                m_player = null;
            }
        }

        public override void Reset(GamePlayer player)
        {
            base.Reset(player);
            RefreshValue(player);
        }

        private bool RefreshValue(GamePlayer player)
        {
            int required = Math.Max(1, m_info.Para2);
            if (player == null || !player.PlayerCharacter.IsConsortia || player.PlayerCharacter.ConsortiaID <= 0)
            {
                Value = required;
                return false;
            }

            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaInfo info = db.GetConsortiaSingle(player.PlayerCharacter.ConsortiaID);
                ConsortiaUserInfo membership = db.GetConsortiaUsersByUserID(player.PlayerCharacter.ID);
                if (info == null || membership == null || membership.ConsortiaID != player.PlayerCharacter.ConsortiaID)
                {
                    Value = required;
                    return false;
                }

                int current = 0;
                switch (m_info.Para1)
                {
                    case 0:
                        current = info.Count;
                        break;
                    case 1:
                        current = player.PlayerCharacter.RichesOffer + player.PlayerCharacter.RichesRob;
                        break;
                    case 2:
                        current = info.SmithLevel;
                        break;
                    case 3:
                        current = info.ShopLevel;
                        break;
                    case 4:
                        current = info.StoreLevel;
                        break;
                }

                int remaining = m_info.Para2 - current;
                Value = remaining > 0 ? remaining : 0;
                return Value <= 0;
            }
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return RefreshValue(player);
        }
    }
}
