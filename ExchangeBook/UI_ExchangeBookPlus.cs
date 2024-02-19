using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

namespace ExchangeBook;

public class UI_ExchangeBookPlus : UIBase
{
	private static UIElement element;

	private int _organizationId;

	private int _taiwuId;

	private readonly List<CharacterDisplayData> _npcDatas = new List<CharacterDisplayData>();

	private ValueTuple<int, int> _taiwuLoads;

	private int _totalAuthority;

	private sbyte _approveHighestGrave;

	private readonly Dictionary<int, int> _authorities = new Dictionary<int, int>();

	private readonly Dictionary<int, ValueTuple<sbyte[], sbyte[]>> _pagesDatas = new Dictionary<int, ValueTuple<sbyte[], sbyte[]>>();

	private readonly Dictionary<int, List<ItemDisplayData>> _npcItems = new Dictionary<int, List<ItemDisplayData>>();

	private List<ItemDisplayData> _currItems = new List<ItemDisplayData>();

	private List<ItemDisplayData> _exchangeList = new List<ItemDisplayData>();

	private ItemScrollView _npcItemScroll;

	private ItemScrollView _exchangeNpcScroll;

	private List<short> _learnedBooks = new List<short>();

	private RectTransform _exchangeNpcBase;

	private CToggleGroup _gradeTogGroup;

	private CToggleGroup _skillTypeTogGroup;

	private CToggleGroup _firstPageTogGroup;

	private CToggleGroup _directPageTogGroup;

	private CToggleGroup _reversePageTogGroup;

	private CToggleGroup _completePageTogGroup;

	private CButton _confirm;

	private bool _isCombatSkill;

	private int Grade => _gradeTogGroup.GetActive().Key;

	private int SkillType => _skillTypeTogGroup.GetActive().Key;

	private int FirstPageIdx => _firstPageTogGroup.GetActive().Key;

	private bool ShouldShowItem(ItemDisplayData item)
	{
		SkillBookItem skillBookItem = SkillBook.Instance[item.Key.TemplateId];
		if (_isCombatSkill)
		{
			var (pageInfo, pageState2) = _pagesDatas[item.Key.Id];
			if (skillBookItem.Grade != Grade || skillBookItem.CombatSkillType != SkillType || (FirstPageIdx > 0 && FirstPageIdx - 1 != pageInfo[0]))
			{
				return false;
			}
			return _directPageTogGroup.GetAllActive().TrueForAll((CToggle toggle) => pageInfo[toggle.Key] == 0) && _reversePageTogGroup.GetAllActive().TrueForAll((CToggle toggle) => pageInfo[toggle.Key] == 1) && _completePageTogGroup.GetAllActive().TrueForAll((CToggle toggle) => pageState2[toggle.Key] == 0);
		}
		if (skillBookItem.Grade != Grade || skillBookItem.LifeSkillType != SkillType)
		{
			return false;
		}
		sbyte[] pageState = _pagesDatas[item.Key.Id].Item2;
		return _completePageTogGroup.GetAllActive().TrueForAll((CToggle toggle) => pageState[toggle.Key - 1] == 0);
	}

	private void UpdateCurrList()
	{
		_currItems.Clear();
		foreach (KeyValuePair<int, List<ItemDisplayData>> npcItem in _npcItems)
		{
			foreach (ItemDisplayData item in npcItem.Value)
			{
				if (ShouldShowItem(item))
				{
					_currItems.Add(item);
				}
			}
		}
		UpdateShopDisplay();
	}

	private void UpdateShopDisplay()
	{
		_npcItemScroll.SetItemList(ref _currItems);
		RectTransform content = _npcItemScroll.GetComponent<CScrollRect>().Content;
		content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, content.sizeDelta.y + 30f);
	}

	public static UIElement GetUI()
	{
		if (element != null && element.UiBase != null)
		{
			return element;
		}
		element = new UIElement();
		Traverse.Create(element).Field("_path").SetValue("UI_ExchangeBookPlus");
		GameObject gameObject = UIBuilder.BuildMainUI("UI_ExchangeBookPlus");
		UI_ExchangeBookPlus uI_ExchangeBookPlus = gameObject.AddComponent<UI_ExchangeBookPlus>();
		uI_ExchangeBookPlus.UiType = UILayer.LayerPopUp;
		uI_ExchangeBookPlus.Element = element;
		uI_ExchangeBookPlus.RelativeAtlases = new SpriteAtlas[0];
		uI_ExchangeBookPlus.Init(gameObject);
		element.UiBase = uI_ExchangeBookPlus;
		element.UiBase.name = element.Name;
		UIManager.Instance.PlaceUI(element.UiBase);
		uI_ExchangeBookPlus.NeedDataListenerId = true;
		return element;
	}

	private void Init(GameObject obj)
	{
		AnimIn = obj.transform.Find("MoveIn").GetComponent<DOTweenAnimation>();
		AnimOut = obj.transform.Find("MoveOut").GetComponent<DOTweenAnimation>();
		AnimIn.hasOnPlay = true;
		AnimIn.onPlay = new UnityEvent();
		AnimOut.hasOnPlay = true;
		AnimOut.onPlay = new UnityEvent();
		_exchangeNpcBase = obj.transform.Find("MainWindow/ExchangeArea/Npc/").gameObject.GetComponent<RectTransform>();
		_confirm = obj.transform.Find("MainWindow/ExchangeArea/ConfirmFrame/Confirm").gameObject.GetComponent<CButton>();
		AddMono(obj.transform.Find("MainWindow/NpcBooks/ImgTitle36/Title").gameObject.GetComponent<TextMeshProUGUI>(), "NpcTitle");
		_npcItemScroll = obj.transform.Find("MainWindow/NpcBooks/NpcItemScroll/").gameObject.GetComponent<ItemScrollView>();
		AddMono(obj.transform.Find("MainWindow/ExchangeArea/SelfPrestige/").gameObject.GetComponent<RectTransform>(), "SelfAuthority");
		AddMono(obj.transform.Find("MainWindow/ExchangeArea/Load/").gameObject.GetComponent<RectTransform>(), "Load");
		_confirm.ClearAndAddListener(delegate
		{
			OnClick(_confirm);
		});
		CButton close = obj.transform.Find("MainWindow/Close").GetComponent<CButton>();
		close.ClearAndAddListener(delegate
		{
			OnClick(close);
		});
		CButton component = obj.transform.Find("MainWindow/ExchangeArea/Reset").GetComponent<CButton>();
		close.ClearAndAddListener(delegate
		{
			OnClick(close);
		});
		_skillTypeTogGroup = obj.transform.Find("MainWindow/NpcBooks/LifeSkillTypeTogGroup/").gameObject.GetComponent<CToggleGroup>();
		_gradeTogGroup = obj.transform.Find("MainWindow/NpcBooks/GradeToggleGroup/").gameObject.GetComponent<CToggleGroup>();
		_skillTypeTogGroup.InitPreOnToggle();
		_gradeTogGroup.InitPreOnToggle();
		_skillTypeTogGroup.OnActiveToggleChange = delegate
		{
			UpdateCurrList();
		};
		_gradeTogGroup.OnActiveToggleChange = delegate
		{
			UpdateCurrList();
		};
		_firstPageTogGroup = _npcItemScroll.transform.Find("ItemSortAndFilter/FirstPageFilter/").gameObject.GetComponent<CToggleGroup>();
		_directPageTogGroup = _npcItemScroll.transform.Find("ItemSortAndFilter/DirectPageFilter/").gameObject.GetComponent<CToggleGroup>();
		_reversePageTogGroup = _npcItemScroll.transform.Find("ItemSortAndFilter/ReversePageFilter/").gameObject.GetComponent<CToggleGroup>();
		_completePageTogGroup = _npcItemScroll.transform.Find("ItemSortAndFilter/CompletePageFilter/").gameObject.GetComponent<CToggleGroup>();
		_firstPageTogGroup.InitPreOnToggle();
		_firstPageTogGroup.OnActiveToggleChange = delegate
		{
			UpdateCurrList();
		};
		_directPageTogGroup.InitPreOnToggle();
		_reversePageTogGroup.InitPreOnToggle();
		_completePageTogGroup.InitPreOnToggle();
		_directPageTogGroup.OnActiveToggleChange = delegate(CToggle togNew, CToggle togOld)
		{
			if (togNew != null)
			{
				_reversePageTogGroup.SetWithoutNotify(togNew.Key, value: false);
			}
			UpdateCurrList();
		};
		_reversePageTogGroup.OnActiveToggleChange = delegate(CToggle togNew, CToggle togOld)
		{
			if (togNew != null)
			{
				_directPageTogGroup.SetWithoutNotify(togNew.Key, value: false);
			}
			UpdateCurrList();
		};
		_completePageTogGroup.OnActiveToggleChange = delegate
		{
			UpdateCurrList();
		};
		obj.SetActive(value: false);
	}

	public override void OnInit(ArgumentBox argsBox)
	{
		_npcDatas.Clear();
		_taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		argsBox.Get("OrganizationId", out _organizationId);
		argsBox.Get("OrganizationName", out string arg);
		argsBox.Get("IsCombatSkill", out _isCombatSkill);
		CGet<TextMeshProUGUI>("NpcTitle").text = arg + "的藏书";
		_npcItems.Clear();
		_pagesDatas.Clear();
		_currItems.Clear();
		_taiwuLoads = new ValueTuple<int, int>(0, 0);
		_learnedBooks.Clear();
		_authorities.Clear();
		_exchangeNpcScroll = _exchangeNpcBase.Find("TradeItemScroll").GetComponent<ItemScrollView>();
		_exchangeNpcScroll.Init();
		_exchangeNpcScroll.SetItemList(ref _exchangeList, reset: true, "exchange_book_npc1", _exchangeNpcScroll.SortAndFilter.IsDetailView, OnRenderTradeNpcItem);
		if (_isCombatSkill)
		{
			for (int i = 1; i < _skillTypeTogGroup.transform.childCount; i++)
			{
				_skillTypeTogGroup.transform.GetChild(i).gameObject.SetActive(i <= 14);
			}
			for (int j = 0; j < 14; j++)
			{
				Refers component = _skillTypeTogGroup.Get(j).GetComponent<Refers>();
				component.CGet<TextMeshProUGUI>("Label").text = CombatSkillType.Instance[j].Name;
				string filterCombatSkillTypeIcon = CommonUtils.GetFilterCombatSkillTypeIcon(j);
				component.CGet<CImage>("Icon").SetSprite(filterCombatSkillTypeIcon);
				component.CGet<CImage>("Icon").SetSprite(filterCombatSkillTypeIcon);
                component.CGet<GameObject>("BookCountBack").SetActive(false);
            }
			if (SkillType > 14)
			{
				_skillTypeTogGroup.Set(1);
			}
		}
		else
		{
			for (int k = 1; k < _skillTypeTogGroup.transform.childCount; k++)
			{
				_skillTypeTogGroup.transform.GetChild(k).gameObject.SetActive(k <= 16);
			}
			for (int l = 0; l < 16; l++)
			{
				Refers component2 = _skillTypeTogGroup.Get(l).GetComponent<Refers>();
				component2.CGet<TextMeshProUGUI>("Label").text = Config.LifeSkillType.Instance[l].Name;
				string filterLifeSkillTypeIcon = CommonUtils.GetFilterLifeSkillTypeIcon(l);
				component2.CGet<CImage>("Icon").SetSprite(filterLifeSkillTypeIcon);
                component2.CGet<GameObject>("BookCountBack").SetActive(false);
            }
		}
		_confirm.interactable = false;
		_npcItemScroll.Init();
		_npcItemScroll.SetItemList(ref _currItems, reset: true, "exchange_book_npc", _npcItemScroll.SortAndFilter.IsDetailView, OnRenderNpcItem);
		_firstPageTogGroup.SetInteractable(_isCombatSkill);
		_directPageTogGroup.SetInteractable(_isCombatSkill);
		_reversePageTogGroup.SetInteractable(_isCombatSkill);
		UpdateCurrList();
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Set("ShowBlackMask", arg: false);
		argumentBox.Set("ShowWaitAnimation", arg: true);
		UIElement.FullScreenMask.SetOnInitArgs(argumentBox);
		UIElement.FullScreenMask.Show();
		UIElement uIElement = Element;
		uIElement.OnListenerIdReady = (Action)Delegate.Combine(uIElement.OnListenerIdReady, new Action(OnListenerIdReady));
	}

	private void OnListenerIdReady()
	{
		AsyncMethodCall(3, 2, _organizationId, delegate(int offset, RawDataPool dataPool)
		{
			List<CharacterDisplayData> item = null;
			Serializer.Deserialize(dataPool, offset, ref item);
			_npcDatas.AddRange(item);
			if (item.Count > 0)
			{
				AsyncMethodCall(4, 96, _npcDatas[0].CharacterId, delegate(int offsetOrg, RawDataPool dataPoolOrg)
				{
					Serializer.Deserialize(dataPoolOrg, offsetOrg, ref _approveHighestGrave);
				});
				StartCoroutine(CoroutineGetBook());
			}
			for (int i = 0; i < _npcDatas.Count; i++)
			{
				MonitorFields.Add(new MonitorDataField(4, 0, (ulong)_npcDatas[i].CharacterId, new uint[1] { 34u }));
			}
			Element.MonitorData();
		});
		MonitorFields.Add(new MonitorDataField(4, 0, (ulong)_taiwuId, new uint[4] { 104u, 105u, 34u, 60u }));
	}

	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper notification2 in notifications)
		{
			Notification notification = notification2.Notification;
			if (notification.Type != 0)
			{
				continue;
			}
			DataUid uid = notification.Uid;
			if (uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == _taiwuId && (uid.SubId1 == 105 || uid.SubId1 == 104))
			{
				int item = 0;
				Serializer.Deserialize(notification2.DataPool, notification.ValueOffset, ref item);
				if (uid.SubId1 == 105)
				{
					_taiwuLoads.Item2 = item;
				}
				else
				{
					_taiwuLoads.Item1 = item;
				}
			}
			else if (uid.DomainId == 4 && uid.DataId == 0 && uid.SubId1 == 34)
			{
				ResourceInts item2 = default(ResourceInts);
				Serializer.Deserialize(notification2.DataPool, notification.ValueOffset, ref item2);
				if (_authorities.ContainsKey((int)uid.SubId0))
				{
					_authorities[(int)uid.SubId0] = item2.Get(7);
				}
				else
				{
					_authorities.Add((int)uid.SubId0, item2.Get(7));
				}
			}
			else if (uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == _taiwuId && uid.SubId1 == 60)
			{
				Serializer.Deserialize(notification2.DataPool, notification.ValueOffset, ref _learnedBooks);
				_npcItemScroll.ReRender();
			}
		}
		UpdateAuthorityAndLoad();
	}

	private void UpdateAuthorityAndLoad()
	{
		_totalAuthority = 0;
		int weight = 0;
		RectTransform rectTransform = CGet<RectTransform>("SelfAuthority");
		RectTransform rectTransform2 = CGet<RectTransform>("Load");
		_exchangeList.ForEach(delegate(ItemDisplayData element)
		{
			weight += element.Weight;
			_totalAuthority += element.Price;
		});
		float num = (float)(_taiwuLoads.Item2 + weight) / 100f;
		string text = $"{num:F1}";
		string text2 = ((_totalAuthority > 0) ? $"-{_totalAuthority}".SetColor("brightred") : "");
		rectTransform2.Find("CurValue").GetComponent<TextMeshProUGUI>().text = ((_taiwuLoads.Item2 + weight > _taiwuLoads.Item1) ? text.SetColor("brightred") : text);
		rectTransform2.Find("MaxValue").GetComponent<TextMeshProUGUI>().text = $" / {(float)((double)_taiwuLoads.Item1 / 100.0):F1}";
		rectTransform.Find("Value").GetComponent<TextMeshProUGUI>().text = $"{_authorities[_taiwuId]}";
		rectTransform.Find("Delta").GetComponent<TextMeshProUGUI>().text = text2;
	}

	private void OnRenderNpcItem(ItemDisplayData itemDisplayData, ItemView itemView)
	{
		MouseTipDisplayer mouseTipDisplayer = itemView.CGet<MouseTipDisplayer>("Tip");
		bool isLock = false;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
        CharacterDisplayData characterDisplayData = _npcDatas[itemDisplayData.SpecialArg];

		if (_isCombatSkill)
		{
			if (IsLockByApproveEnough(itemDisplayData))
			{
				stringBuilder.Append(LocalStringManager.Get("LK_ExchangeBook_Tip_1"));
				stringBuilder2.Append(LocalStringManager.Get("LK_ExchangeBook_Tag_1"));
				isLock = true;
			}
			if (!IsTaiwuLearned(itemDisplayData))
			{
				string text = LocalStringManager.Get("LK_ExchangeBook_Tip_2");
				stringBuilder.Append(isLock ? ("\n" + text) : (text ?? ""));
				stringBuilder2.Append(isLock ? "" : LocalStringManager.Get("LK_ExchangeBook_Tag_2"));
				isLock = true;
			}
		}
		else
		{
			stringBuilder.Append("请找拥有者进行换书");
			isLock = true;
		}
		if (isLock)
		{
			itemView.SetLocked(locked: true);
			itemView.GetComponent<PointerTrigger>().enabled = false;
			mouseTipDisplayer.PresetParam = new string[1] { stringBuilder.ToString().ColorReplace() };
			mouseTipDisplayer.Type = TipType.SingleDesc;
			mouseTipDisplayer.RuntimeParam = null;
			mouseTipDisplayer.transform.Find("Lable").GetComponent<TextMeshProUGUI>().text = stringBuilder2.ToString();
			mouseTipDisplayer.gameObject.SetActive(value: true);
			itemView.SetClickEvent(delegate
			{
			});
		}
		else
		{
			itemView.SetLocked(locked: false);
			itemView.GetComponent<PointerTrigger>().enabled = true;
			mouseTipDisplayer.gameObject.SetActive(value: false);
			itemView.SetClickEvent(delegate
			{
				PutShopItemToTrade(itemView);
				UpdateAuthorityAndLoad();
				UpdateConfirmButton();
			});
		}

		string arg = NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, isTaiwu: false).SetGradeColor(characterDisplayData.OrgInfo.Grade);
		itemView.CGet<TextMeshProUGUI>("Price").text = string.Format($"{itemDisplayData.Price} {arg}");
	}

	private bool IsTaiwuLearned(ItemDisplayData itemDisplayData)
	{
		return _learnedBooks.Contains((short)(itemDisplayData.Key.TemplateId - 144));
	}

	private bool IsLockByApproveEnough(ItemDisplayData item)
	{
		return _approveHighestGrave < ItemTemplateHelper.GetGrade(item.Key.ItemType, item.Key.TemplateId);
	}

    private void OnRenderTradeNpcItem(ItemDisplayData itemDisplayData, ItemView itemView)
	{
		itemView.SetClickEvent(delegate
		{
			PutBookBack(itemView);
			UpdateAuthorityAndLoad();
			UpdateConfirmButton();
		});
		CharacterDisplayData characterDisplayData = _npcDatas[itemDisplayData.SpecialArg];
		string arg = NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, isTaiwu: false).SetGradeColor(characterDisplayData.OrgInfo.Grade);
		itemView.CGet<TextMeshProUGUI>("Price").text = string.Format($"{itemDisplayData.Price} {arg}");
	}

	private void UpdateConfirmButton()
	{
		if (_totalAuthority == 0)
		{
			_confirm.interactable = false;
		}
		else
		{
			_confirm.interactable = _authorities[_taiwuId] >= _totalAuthority;
		}
	}

	private void PutBookBack(ItemView itemView)
	{
		if (!itemView.IsLocked)
		{
			ItemDisplayData data = itemView.Data;
			if (ShouldShowItem(data))
			{
				_currItems.Add(data);
				UpdateShopDisplay();
			}
			_exchangeList.Remove(data);
			UpdateExchangeAreaDisplay();
		}
	}

	private void PutShopItemToTrade(ItemView itemView)
	{
		if (!itemView.IsLocked)
		{
			ItemDisplayData data = itemView.Data;
			_exchangeList.Add(data);
			_currItems.Remove(data);
			UpdateShopDisplay();
			UpdateExchangeAreaDisplay();
		}
	}

	private void UpdateExchangeAreaDisplay()
	{
		_exchangeNpcScroll.SetItemList(ref _exchangeList);
		_exchangeNpcBase.Find("NoPick").gameObject.SetActive(_exchangeList.Count <= 0);
		RectTransform content = _exchangeNpcScroll.GetComponent<CScrollRect>().Content;
		content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, content.sizeDelta.y + 30f);
	}

	private void SetBookAuthority(ItemDisplayData itemData)
	{
		sbyte behaviorType = _npcDatas[itemData.SpecialArg].BehaviorType;
		int num = 100;
		switch (behaviorType)
		{
		case 0:
			num = 200;
			break;
		case 1:
			num = 100;
			break;
		case 2:
			num = 150;
			break;
		case 3:
			num = 100;
			break;
		case 4:
			num = 200;
			break;
		}
		itemData.Price = (int)((double)itemData.Price * (0.5 + 0.5 * (double)itemData.Durability / (double)itemData.MaxDurability) / 10.0 * (double)num / 100.0);
	}

	public override void QuickHide()
	{
		ResetPage();
		foreach (KeyValuePair<int, List<ItemDisplayData>> npcItem in _npcItems)
		{
			List<ItemKey> arg = npcItem.Value.ConvertAll((ItemDisplayData element) => element.Key);
			GameDataBridge.AddMethodCall(Element.GameDataListenerId, 14, 4, npcItem.Key, arg, !_isCombatSkill);
		}
		AudioManager.Instance.PlaySound("ui_default_cancel");
		base.QuickHide();
	}

	protected override void OnClick(CButton btn)
	{
		if (btn.name == "Close")
		{
			QuickHide();
		}
		else if (btn.name == "Confirm")
		{
			ChangeBook();
		}
		else if (btn.name == "Reset")
		{
			ResetPage();
		}
	}

	private void ResetPage()
	{
		_exchangeList.Clear();
		UpdateCurrList();
		UpdateAuthorityAndLoad();
		UpdateExchangeAreaDisplay();
		_confirm.interactable = false;
	}

	private void ChangeBook()
	{
		_exchangeList.Sort((ItemDisplayData a, ItemDisplayData b) => a.SpecialArg.CompareTo(a.SpecialArg));
		Inventory inventory = new Inventory();
		int num = _authorities[_taiwuId];
		int num2 = 0;
		for (int i = 0; i < _exchangeList.Count; i++)
		{
			inventory.Items.Add(_exchangeList[i].Key, _exchangeList[i].Amount);
			num2 += _exchangeList[i].Price;
			int characterId = _npcDatas[_exchangeList[i].SpecialArg].CharacterId;
			_npcItems[characterId].Remove(_exchangeList[i]);
			if (i == _exchangeList.Count - 1 || _exchangeList[i].SpecialArg != _exchangeList[i + 1].SpecialArg)
			{
				num -= num2;
				int arg = _authorities[characterId] + num2;
                //GameDataBridge.AddMethodCall<int, int, string, string>(-1, 12, 46, _taiwuId, characterId, "换书", "换书");
                GameDataBridge.AddMethodCall<int, Inventory, Inventory, int, int>(-1, 14, 5, characterId, inventory, null, num, arg);
				num2 = 0;
				inventory.Items.Clear();
			}
		}
		_exchangeList.Clear();
		UpdateExchangeAreaDisplay();
		_confirm.interactable = false;
	}

	private void GetBook(int index)
	{
		if (_isCombatSkill)
		{
			AsyncMethodCall(14, 9, _npcDatas[index].CharacterId, arg2: false, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> item2 = new List<ItemDisplayData>();
				Serializer.Deserialize(dataPool, offset, ref item2);
				_npcItems.Add(_npcDatas[index].CharacterId, item2);
				foreach (ItemDisplayData item3 in item2)
				{
					item3.SpecialArg = index;
					SetBookAuthority(item3);
					AsyncMethodCall(6, 14, item3.Key, OnGetPageInfo);
				}
			});
			return;
		}
		AsyncMethodCall(14, 9, _npcDatas[index].CharacterId, arg2: true, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> item = new List<ItemDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref item);
			_npcItems.Add(_npcDatas[index].CharacterId, item);
			foreach (ItemDisplayData item4 in item)
			{
				if (SkillBook.Instance[item4.Key.TemplateId].ItemSubType == 1000)
				{
					item4.SpecialArg = index;
					SetBookAuthority(item4);
					AsyncMethodCall(6, 14, item4.Key, OnGetPageInfo);
				}
			}
		});
	}

	private IEnumerator CoroutineGetBook()
	{
		for (int i = 0; i < _npcDatas.Count; i++)
		{
			GetBook(i);
		}
		while (_npcItems.Count < _npcDatas.Count)
		{
			yield return null;
		}
		int totalBooks = 0;
		foreach (List<ItemDisplayData> list in _npcItems.Values)
		{
			totalBooks = ((!_isCombatSkill) ? (totalBooks + list.Count((ItemDisplayData data) => SkillBook.Instance[data.Key.TemplateId].ItemSubType == 1000)) : (totalBooks + list.Count));
		}
		while (_pagesDatas.Count < totalBooks)
		{
			yield return null;
		}
		UpdateCurrList();
		Element.ShowAfterRefresh();
		UIElement.FullScreenMask.Hide();
	}

	private void OnGetPageInfo(int offset, RawDataPool dataPool)
	{
		SkillBookPageDisplayData item = null;
		Serializer.Deserialize(dataPool, offset, ref item);
		_pagesDatas.Add(item.ItemKey.Id, new ValueTuple<sbyte[], sbyte[]>(item.Type, item.State));
	}
}
