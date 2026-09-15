using Bussiness.Managers;
using SqlDataProvider.Data;

namespace Fighting.Server.GameObjects;

public class ProxyPlayerInfo
{
	public int ServerId { get; set; }

	public double BaseAttack { get; set; }

	public double BaseDefence { get; set; }

	public double BaseAgility { get; set; }

	public double BaseBlood { get; set; }

	public int TemplateId { get; set; }

	public bool CanUserProp { get; set; }

	public bool CanX2Exp { get; set; }

	public bool CanX3Exp { get; set; }

	public int SecondWeapon { get; set; }

	public int StrengthLevel { get; set; }

	public int Healstone { get; set; }

	public int HealstoneCount { get; set; }

	public double GPAddPlus { get; set; }

	public float GMExperienceRate { get; set; }

	public float AuncherExperienceRate { get; set; }

	public double OfferAddPlus { get; set; }

	public float GMOfferRate { get; set; }

	public float AuncherOfferRate { get; set; }

	public float GMRichesRate { get; set; }

	public float AuncherRichesRate { get; set; }

	public double AntiAddictionRate { get; set; }

	public string FightFootballStyle { get; set; }

	public ItemTemplateInfo GetItemTemplateInfo()
	{
		return ItemMgr.FindItemTemplate(TemplateId);
	}

	public ItemInfo GetItemInfo()
	{
		ItemInfo itemInfo = null;
		if (SecondWeapon != 0)
		{
			itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(SecondWeapon), 1, 1);
			itemInfo.StrengthenLevel = StrengthLevel;
		}
		return itemInfo;
	}

	public ItemInfo GetHealstone()
	{
		ItemInfo itemInfo = null;
		if (Healstone != 0)
		{
			itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(Healstone), 1, 1);
			itemInfo.Count = HealstoneCount;
		}
		return itemInfo;
	}
}
