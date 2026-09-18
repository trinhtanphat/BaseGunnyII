using System;
using System.Linq;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.VIP_RENEWAL, "VIP renewal")]
    public class OpenVipHandler : IPacketHandler
    {
        private const int VipTemplateId = 11992;
        private const int GoldPerXu = 1000;
        private const byte PayWithXu = 0;
        private const byte PayWithGold = 1;

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string requestedNickName = packet.ReadString();
            int renewalDays = packet.ReadInt();
            byte paymentMode = packet.DataLeft > 0 ? packet.ReadByte() : PayWithXu;

            if (!String.Equals(requestedNickName, client.Player.PlayerCharacter.NickName,
                StringComparison.OrdinalIgnoreCase))
            {
                client.Out.SendMessage(eMessageType.Normal, "Yêu cầu VIP không hợp lệ.");
                return 0;
            }

            if (paymentMode != PayWithXu && paymentMode != PayWithGold)
            {
                client.Out.SendMessage(eMessageType.Normal, "Phương thức thanh toán VIP không hợp lệ.");
                return 0;
            }

            ShopItemInfo shopItem = Bussiness.Managers.ShopMgr
                .FindShopbyTemplatID(VipTemplateId)
                .FirstOrDefault(item => item.IsContinue);

            int xuPrice = GetRenewalPrice(shopItem, renewalDays);
            if (xuPrice <= 0)
            {
                client.Out.SendMessage(eMessageType.Normal, "Thời hạn VIP không hợp lệ.");
                return 0;
            }

            int charge;
            if (paymentMode == PayWithGold)
            {
                long goldPrice = (long)xuPrice * GoldPerXu;
                if (goldPrice > Int32.MaxValue)
                {
                    client.Out.SendMessage(eMessageType.Normal, "Giá VIP vượt giới hạn.");
                    return 0;
                }
                charge = (int)goldPrice;
                if (client.Player.PlayerCharacter.Gold < charge)
                {
                    client.Out.SendMessage(eMessageType.Normal, "Không đủ Vàng.");
                    return 0;
                }
            }
            else
            {
                charge = xuPrice;
                if (client.Player.PlayerCharacter.Money < charge)
                {
                    client.Out.SendMessage(eMessageType.Normal,
                        LanguageMgr.GetTranslation("UserBuyItemHandler.Money"));
                    return 0;
                }
            }

            int removed = paymentMode == PayWithGold
                ? client.Player.RemoveGold(charge)
                : client.Player.RemoveMoney(charge);
            if (removed != charge)
                return 0;

            int result;
            DateTime expireDay;
            using (PlayerBussiness db = new PlayerBussiness())
            {
                result = db.VIPRenewal(client.Player.PlayerCharacter.ID, renewalDays, out expireDay);
                if (result == 1)
                {
                    PlayerInfo player = client.Player.PlayerCharacter;
                    player.typeVIP = player.typeVIP < 1 ? (byte)1 : player.typeVIP;
                    player.VIPLevel = Math.Max(1, Math.Min(PlayerInfo.MaxVipLevel, player.VIPLevel));
                    player.VIPExpireDay = expireDay;
                    player.LastVIPPackTime = DateTime.Now;
                    player.VIPLastDate = DateTime.Now;
                    player.CanTakeVipReward = true;
                    player.EnsureVipExpFloor();
                    player.VIPNextLevelDaysNeeded = player.DaysNeeded(player.VIPLevel);
                    db.UpdateVIPInfo(player);
                }
            }

            if (result != 1)
            {
                if (paymentMode == PayWithGold)
                    client.Player.AddGold(charge);
                else
                    client.Player.AddMoney(charge);
                client.Out.SendMessage(eMessageType.Normal, "Không thể gia hạn VIP, giao dịch đã được hoàn lại.");
                return 0;
            }

            client.Out.SendOpenVIP(client.Player);
            return result;
        }

        private static int GetRenewalPrice(ShopItemInfo shopItem, int renewalDays)
        {
            int oneMonth = 569;
            int threeMonths = 1707;
            int sixMonths = 3000;

            if (shopItem != null)
            {
                if (shopItem.AValue1 > 0) oneMonth = shopItem.AValue1;
                if (shopItem.BValue1 > 0) threeMonths = shopItem.BValue1;
                if (shopItem.CValue1 > 0) sixMonths = shopItem.CValue1;
            }

            int months;
            if (renewalDays == 365)
                months = 12;
            else if (renewalDays > 0 && renewalDays % 31 == 0)
                months = renewalDays / 31;
            else
                return 0;

            if (months < 1 || months > 24)
                return 0;

            int price = 0;
            while (months >= 6)
            {
                price += sixMonths;
                months -= 6;
            }
            while (months >= 3)
            {
                price += threeMonths;
                months -= 3;
            }
            price += months * oneMonth;
            return price;
        }
    }
}
