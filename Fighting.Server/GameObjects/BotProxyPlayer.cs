using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Fighting.Server.GameObjects
{
    public class BotProxyPlayer : IGamePlayer, IBotGamePlayer
    {
        private readonly PlayerInfo m_character;
        private readonly ItemTemplateInfo m_weapon;
        private readonly double m_baseAttack;
        private readonly double m_baseDefence;
        private readonly double m_baseBlood;
        private int m_gamePlayerId;
        private int m_serverId;
        private bool m_canUseProp;
        private List<int> m_equipEffect;

        public BotProxyPlayer(IGamePlayer source, int botId)
        {
            PlayerInfo src = source.PlayerCharacter;
            m_character = new PlayerInfo();
            m_character.ID = botId;
            m_character.UserName = "bot_" + Math.Abs(botId);
            m_character.NickName = "NPC " + src.NickName;
            m_character.Sex = src.Sex;
            m_character.Hide = src.Hide;
            m_character.Style = src.Style;
            m_character.Colors = src.Colors;
            m_character.Skin = src.Skin;
            m_character.GP = src.GP;
            m_character.Grade = src.Grade;
            m_character.Repute = src.Repute;
            m_character.Attack = src.Attack;
            m_character.Defence = src.Defence;
            m_character.Agility = src.Agility;
            m_character.Luck = src.Luck;
            m_character.FightPower = src.FightPower;
            m_character.Nimbus = src.Nimbus;
            m_character.Win = 0;
            m_character.Total = 0;
            m_character.Offer = 0;
            m_character.ConsortiaID = 0;
            m_character.ConsortiaName = string.Empty;
            CopyWeakGuildProgress(src, m_character);
            m_weapon = source.MainWeapon;
            m_baseAttack = source.GetBaseAttack();
            m_baseDefence = source.GetBaseDefence();
            m_baseBlood = source.GetBaseBlood();
            m_equipEffect = new List<int>();
        }
        private static void CopyWeakGuildProgress(PlayerInfo source, PlayerInfo target)
        {
            var property = typeof(PlayerInfo).GetProperty("weaklessGuildProgress");
            if (property == null || property.PropertyType != typeof(byte[]) || !property.CanRead || !property.CanWrite)
                return;

            byte[] progress = property.GetValue(source, null) as byte[];
            int length = progress == null ? 2 : Math.Max(2, progress.Length);
            byte[] clone = new byte[length];
            if (progress != null)
                Array.Copy(progress, clone, progress.Length);
            property.SetValue(target, clone, null);
        }

        public bool IsBot { get { return true; } }
        public PlayerInfo PlayerCharacter { get { return m_character; } }
        public ItemTemplateInfo MainWeapon { get { return m_weapon; } }
        public ItemInfo SecondWeapon { get { return null; } }
        public bool CanUseProp { get { return m_canUseProp; } set { m_canUseProp = value; } }
        public int GamePlayerId { get { return m_gamePlayerId; } set { m_gamePlayerId = value; } }
        public int ServerID { get { return m_serverId; } set { m_serverId = value; } }
        public List<int> EquipEffect { get { return m_equipEffect; } set { m_equipEffect = value ?? new List<int>(); } }

        public double GetBaseBlood() { return m_baseBlood; }
        public double GetGoldBlood() { return 0; }
        public double GetBaseAttack() { return m_baseAttack; }
        public double GetBaseDefence() { return m_baseDefence; }

        public int AddGP(int value) { return 0; }
        public int RemoveGP(int value) { return 0; }
        public int AddGold(int value) { return 0; }
        public int RemoveGold(int value) { return 0; }
        public int AddMoney(int value) { return 0; }
        public int RemoveMoney(int value) { return 0; }
        public int AddGiftToken(int value) { return 0; }
        public int RemoveGiftToken(int value) { return 0; }
        public int AddMedal(int value) { return 0; }
        public int RemoveMedal(int value) { return 0; }
        public int AddOffer(int value) { return 0; }
        public int RemoveOffer(int value) { return 0; }
        public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count) { return false; }
        public bool ClearTempBag() { return true; }
        public bool ClearFightBag() { return true; }
        public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving) { return false; }
        public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int damage) { }
        public void OnGameOver(AbstractGame game, bool isWin, int gainXp) { }
        public void OnMissionOver(AbstractGame game, bool isWin, int missionId, int turnNum) { }
        public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players,
            eRoomType roomType, eGameType gameClass, int totalKillHealth, int count) { return 0; }
        public void SendConsortiaFight(int consortiaID, int riches, string msg) { }
        public bool SetPvePermission(int missionId, eHardLevel hardLevel) { return true; }
        public bool IsPvePermission(int missionId, eHardLevel hardLevel) { return true; }
        public void Disconnect() { }
        public void SendInsufficientMoney(int type) { }
        public void SendMessage(string msg) { }
        public void SendTCP(GSPacketIn pkg) { }
        public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int money, int spareMoney) { }

        private static Player FindBestTarget(PVPGame game, Player player)
        {
            Player target = null;
            double minDistance = double.MaxValue;
            foreach (Player candidate in game.GetAllFightPlayers())
            {
                if (!candidate.IsLiving || candidate.Blood <= 0 || candidate.Team == player.Team)
                    continue;

                double distance = candidate.Distance(player.X, player.Y);
                if (target == null || distance < minDistance - 0.01 ||
                    (Math.Abs(distance - minDistance) <= 0.01 && candidate.Blood < target.Blood))
                {
                    minDistance = distance;
                    target = candidate;
                }
            }
            return target;
        }

        private static bool IsTrajectoryViable(Player player, Player target,
            int force, int angle)
        {
            BallInfo ball = BallMgr.FindBall(player.CurrentBall.ID);
            if (ball == null || player.Game == null || player.Game.Map == null)
                return false;

            List<Rectangle> targetBounds = target.GetDirectBoudRect();

            var map = player.Game.Map;
            Point shootPoint = player.GetShootPoint();
            return BotAimTrajectory.IsViable(shootPoint.X, shootPoint.Y, force, angle, ball.Mass,
                map.airResistance * ball.DragIndex,
                map.gravity * ball.Weight * ball.Mass, map.wind * ball.Wind,
                targetBounds, ball.Radii, map.Bound.Width, map.Bound.Height,
                delegate(Rectangle rect) { return map.IsRectangleEmpty(rect); },
                delegate(int impactX, int impactY)
                {
                    return target.Distance(new Point(impactX, impactY));
                });
        }
        private static bool TryFindAccurateShot(Player player, Player target,
            out int aimX, out int aimY, out int force, out int angle)
        {
            float[] timeSeeds = { 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.1f };
            int[] yOffsets = { 0, -8, 8, -16, 16 };
            foreach (int yOffset in yOffsets)
            {
                foreach (float timeSeed in timeSeeds)
                {
                    int candidateX = target.X;
                    int candidateY = target.Y + yOffset;
                    int candidateForce = 0;
                    int candidateAngle = 0;
                    player.GetShootForceAndAngle(ref candidateX, ref candidateY, player.CurrentBall.ID,
                        1, 5, 1, timeSeed, ref candidateForce, ref candidateAngle);
                    if (candidateForce > 0 && IsTrajectoryViable(player, target,
                        candidateForce, candidateAngle))
                    {
                        aimX = candidateX;
                        aimY = candidateY;
                        force = candidateForce;
                        angle = candidateAngle;
                        return true;
                    }
                }
            }

            aimX = 0;
            aimY = 0;
            force = 0;
            angle = 0;
            return false;
        }

        public void TakeTurn(PVPGame game, Player player)
        {
            Player target = FindBestTarget(game, player);
            if (target == null)
            {
                player.Skip(0);
                return;
            }

            player.Direction = target.X >= player.X ? 1 : -1;
            int aimX;
            int aimY;
            int force;
            int angle;
            if (!TryFindAccurateShot(player, target, out aimX, out aimY, out force, out angle))
            {
                player.Skip(0);
                return;
            }
            Point shootPoint = player.GetShootPoint();
            player.Shoot(shootPoint.X, shootPoint.Y, force, angle);
        }
    }
}
