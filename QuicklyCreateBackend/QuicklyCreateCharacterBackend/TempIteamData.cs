using System.Collections;
using System.Collections.Generic;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Item;

namespace QuicklyCreateCharacterBackend;

public class TempIteamData
{
	public List<ItemBase> itemList = new List<ItemBase>();

	public SkillBook lifeSkillBook;

	public SkillBook combatSkillBook;

	public List<int> combatSkillBookPageTypes;

	public int Count;

	public TempIteamData(Inventory inventory)
	{
		if (inventory == null)
		{
			return;
		}
		foreach (ItemKey key in inventory.Items.Keys)
		{
			ItemBase baseItem = DomainManager.Item.GetBaseItem(key);
			if (baseItem.GetItemSubType() == 1000)
			{
				lifeSkillBook = (SkillBook)baseItem;
			}
			if (baseItem.GetItemSubType() == 1001)
			{
				combatSkillBook = (SkillBook)baseItem;
			}
			itemList.Add(baseItem);
		}
		Count = itemList.Count;
		if (combatSkillBook != null)
		{
			combatSkillBookPageTypes = new List<int>();
			byte pageTypes = combatSkillBook.GetPageTypes();
			BitArray bitArray = new BitArray(pageTypes);
			combatSkillBookPageTypes.Add(((pageTypes & 1) == 1) ? 1 : 0);
			combatSkillBookPageTypes.Add(((pageTypes & 2) == 2) ? 1 : 0);
			combatSkillBookPageTypes.Add(((pageTypes & 4) == 4) ? 1 : 0);
			combatSkillBookPageTypes.Add(((pageTypes & 8) == 8) ? 1 : 0);
			combatSkillBookPageTypes.Add(((pageTypes & 0x10) == 16) ? 1 : 0);
			combatSkillBookPageTypes.Add(((pageTypes & 0x20) == 32) ? 1 : 0);
			combatSkillBookPageTypes.Add(((pageTypes & 0x40) == 64) ? 1 : 0);
			combatSkillBookPageTypes.Add(((pageTypes & 0x80) == 128) ? 1 : 0);
		}
	}
}
