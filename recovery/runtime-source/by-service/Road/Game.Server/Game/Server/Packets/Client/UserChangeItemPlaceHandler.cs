using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(49, "改变物品位置")]
public class UserChangeItemPlaceHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eBageType eBageType2 = (eBageType)packet.ReadByte();
		int num = packet.ReadInt();
		eBageType eBageType3 = (eBageType)packet.ReadByte();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		packet.ReadBoolean();
		PlayerInventory inventory = client.Player.GetInventory(eBageType2);
		PlayerInventory inventory2 = client.Player.GetInventory(eBageType3);
		ItemInfo itemAt = inventory.GetItemAt(num);
		if (inventory == null || itemAt == null)
		{
			return 0;
		}
		if (num3 < 0 || num3 > itemAt.Count || num3 > itemAt.Template.MaxCount)
		{
			client.Disconnect();
			return 0;
		}
		inventory.BeginChanges();
		inventory2.BeginChanges();
		try
		{
			if (eBageType3 == eBageType.Consortia)
			{
				ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
				if (consortiaInfo != null)
				{
					inventory2.Capalility = consortiaInfo.StoreLevel * 10;
				}
			}
			if (num2 == -1)
			{
				bool flag = false;
				if (eBageType2 == eBageType.CaddyBag && eBageType3 == eBageType.BeadBag)
				{
					num2 = inventory2.FindFirstEmptySlot();
					if (inventory2.AddItemTo(itemAt, num2))
					{
						inventory.TakeOutItem(itemAt);
					}
					else
					{
						flag = true;
					}
				}
				else if (eBageType2 == eBageType3 && eBageType3 == eBageType.MainBag)
				{
					num2 = inventory2.FindFirstEmptySlot();
					if (!inventory.MoveItem(num, num2, num3))
					{
						flag = true;
					}
				}
				else if (inventory2.StackItemToAnother(itemAt) || inventory2.AddItem(itemAt))
				{
					inventory.TakeOutItem(itemAt);
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"));
				}
			}
			else if (eBageType2 == eBageType3)
			{
				ItemInfo itemAt2 = inventory.GetItemAt(num2);
				int num4 = 0;
				if (eBageType2 == eBageType.MainBag)
				{
					num4 = 31;
				}
				if (itemAt2 != null && num2 >= num4)
				{
					num2 = inventory.FindFirstEmptySlot();
					inventory.MoveItem(num, num2, num3);
				}
				else
				{
					inventory.MoveItem(num, num2, num3);
				}
				client.Player.OnNewGearEvent(itemAt.Template.CategoryID);
			}
			else
			{
				switch (eBageType2)
				{
				case eBageType.Store:
					MoveFromStore(client, inventory, itemAt, num2, inventory2, num3);
					break;
				case eBageType.Consortia:
				{
					ItemInfo itemAt4 = inventory2.GetItemAt(num2);
					if (itemAt4 != null)
					{
						num2 = inventory2.FindFirstEmptySlot();
						MoveFromBank(client, num, num2, inventory, inventory2, itemAt);
					}
					else
					{
						MoveFromBank(client, num, num2, inventory, inventory2, itemAt);
					}
					break;
				}
				default:
					switch (eBageType3)
					{
					case eBageType.Store:
						if (itemAt.IsAdvanceDate())
						{
							itemAt.StrengthenExp = 0;
							itemAt.AdvanceDate = DateTime.Now;
						}
						MoveToStore(client, inventory, itemAt, num2, inventory2, num3);
						break;
					case eBageType.Consortia:
					{
						ItemInfo itemAt3 = inventory2.GetItemAt(num2);
						if (itemAt3 != null)
						{
							num2 = inventory2.FindFirstEmptySlot();
							MoveToBank(num, num2, inventory, inventory2, itemAt);
						}
						else
						{
							MoveToBank(num, num2, inventory, inventory2, itemAt);
						}
						break;
					}
					default:
						if (inventory2.AddItemTo(itemAt, num2))
						{
							inventory.TakeOutItem(itemAt);
						}
						break;
					}
					break;
				}
			}
		}
		finally
		{
			inventory.CommitChanges();
			inventory2.CommitChanges();
		}
		return 0;
	}

	public void MoveFromStore(GameClient client, PlayerInventory storeBag, ItemInfo item, int toSlot, PlayerInventory bag, int count)
	{
		if (client.Player == null || item == null || storeBag == null || bag == null || item.Template.BagType != (eBageType)bag.BagType)
		{
			return;
		}
		if (toSlot < bag.BeginSlot || toSlot > bag.Capalility)
		{
			if (bag.StackItemToAnother(item))
			{
				storeBag.RemoveItem(item, eItemRemoveType.Stack);
				return;
			}
			string key = $"temp_place_{item.ItemID}";
			if (client.Player.TempProperties.ContainsKey(key))
			{
				toSlot = (int)storeBag.Player.TempProperties[key];
				storeBag.Player.TempProperties.Remove(key);
			}
			else
			{
				toSlot = bag.FindFirstEmptySlot();
			}
		}
		if (bag.StackItemToAnother(item) || bag.AddItemTo(item, toSlot))
		{
			storeBag.TakeOutItem(item);
			return;
		}
		toSlot = bag.FindFirstEmptySlot();
		if (bag.AddItemTo(item, toSlot))
		{
			storeBag.TakeOutItem(item);
			return;
		}
		storeBag.TakeOutItem(item);
		client.Player.SendItemToMail(item, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), eMailType.ItemOverdue);
		client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
	}

	public void MoveToStore(GameClient client, PlayerInventory bag, ItemInfo item, int toSlot, PlayerInventory storeBag, int count)
	{
		if (client.Player == null || bag == null || item == null || storeBag == null)
		{
			return;
		}
		int place = item.Place;
		ItemInfo itemAt = storeBag.GetItemAt(toSlot);
		if (itemAt != null)
		{
			if (item.Count == 1 && item.BagType == itemAt.BagType)
			{
				bag.TakeOutItem(item);
				storeBag.TakeOutItem(itemAt);
				bag.AddItemTo(itemAt, place);
				storeBag.AddItemTo(item, toSlot);
				return;
			}
			string key = $"temp_place_{itemAt.ItemID}";
			PlayerInventory itemInventory = client.Player.GetItemInventory(itemAt.Template);
			if (client.Player.TempProperties.ContainsKey(key) && itemInventory.BagType == 0)
			{
				int place2 = (int)client.Player.TempProperties[key];
				client.Player.TempProperties.Remove(key);
				if (itemInventory.AddItemTo(itemAt, place2))
				{
					storeBag.TakeOutItem(itemAt);
				}
			}
			else if (itemInventory.StackItemToAnother(itemAt))
			{
				storeBag.RemoveItem(itemAt, eItemRemoveType.Stack);
			}
			else if (itemInventory.AddItem(itemAt))
			{
				storeBag.TakeOutItem(itemAt);
			}
			else
			{
				client.Player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"));
			}
		}
		if (!storeBag.IsEmpty(toSlot))
		{
			return;
		}
		if (item.Count == 1)
		{
			if (!storeBag.AddItemTo(item, toSlot))
			{
				return;
			}
			bag.TakeOutItem(item);
			if (item.Template.BagType == eBageType.MainBag && place < 31)
			{
				string key = $"temp_place_{item.ItemID}";
				if (client.Player.TempProperties.ContainsKey(key))
				{
					client.Player.TempProperties[key] = place;
				}
				else
				{
					client.Player.TempProperties.Add(key, place);
				}
			}
		}
		else
		{
			ItemInfo itemInfo = item.Clone();
			itemInfo.Count = count;
			if (bag.RemoveCountFromStack(item, count, eItemRemoveType.Stack) && !storeBag.AddItemTo(itemInfo, toSlot))
			{
				bag.AddCountToStack(item, count);
			}
		}
	}

	private static void MoveToBank(int place, int toplace, PlayerInventory bag, PlayerInventory bank, ItemInfo item)
	{
		if (bag == null || item == null || bag == null)
		{
			return;
		}
		ItemInfo itemAt = bank.GetItemAt(toplace);
		if (itemAt != null)
		{
			if (item.CanStackedTo(itemAt) && item.Count + itemAt.Count <= item.Template.MaxCount)
			{
				if (bank.AddCountToStack(itemAt, item.Count))
				{
					bag.RemoveCountFromStack(item, item.Count);
				}
			}
			else if (itemAt.Template.BagType == (eBageType)bag.BagType)
			{
				bag.TakeOutItem(item);
				bank.TakeOutItem(itemAt);
				bag.AddItemTo(itemAt, place);
				bank.AddItemTo(item, toplace);
			}
		}
		else if (bank.AddItemTo(item, toplace))
		{
			bag.TakeOutItem(item);
		}
	}

	private static void MoveFromBank(GameClient client, int place, int toplace, PlayerInventory bag, PlayerInventory tobag, ItemInfo item)
	{
		if (item == null)
		{
			return;
		}
		PlayerInventory itemInventory = client.Player.GetItemInventory(item.Template);
		if (itemInventory == tobag)
		{
			ItemInfo itemAt = itemInventory.GetItemAt(toplace);
			if (itemAt == null)
			{
				if (itemInventory.AddItemTo(item, toplace))
				{
					bag.TakeOutItem(item);
				}
			}
			else if (!item.CanStackedTo(itemAt) || item.Count + itemAt.Count > item.Template.MaxCount)
			{
				itemInventory.TakeOutItem(itemAt);
				bag.TakeOutItem(item);
				itemInventory.AddItemTo(item, toplace);
				bag.AddItemTo(itemAt, place);
			}
			else if (itemInventory.AddCountToStack(itemAt, item.Count))
			{
				bag.RemoveCountFromStack(item, item.Count);
			}
		}
		else if (itemInventory.AddItem(item))
		{
			bag.TakeOutItem(item);
		}
	}
}
