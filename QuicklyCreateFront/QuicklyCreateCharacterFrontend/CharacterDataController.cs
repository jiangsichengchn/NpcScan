using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using Config;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Item;
using GameData.GameDataBridge;
using HarmonyLib;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace QuicklyCreateCharacterFrontend;

public class CharacterDataController : MonoBehaviour
{
	public UI_NewGame UI_NewGame_Member;

	public ProtagonistCreationInfo protagonistCreationInfo = new ProtagonistCreationInfo();

	public List<string> characterDataList = new List<string>();

	public Dictionary<CharacterDataType, List<string>> characterDataDict = new Dictionary<CharacterDataType, List<string>>();

	public Dictionary<CharacterDataType, List<short>> characterDataShortDict = new Dictionary<CharacterDataType, List<short>>();

	public Dictionary<CharacterDataType, List<string>> characterDataNameDict = new Dictionary<CharacterDataType, List<string>>();

	public Dictionary<CharacterDataType, List<string>> characterDataColorDict = new Dictionary<CharacterDataType, List<string>>();

	private bool _bool_IsSendCreationInfo = false;

	private bool _bool_IsGetCharacterData = true;

	public event Action updateDataEvent;

	public void DoRollCharacterData()
	{
		UpdateTheCreationInfo();
		if (_bool_IsGetCharacterData)
		{
			SendTheCreationInfo();
			Invoke("GetCharacterData", 0.5f);
		}
	}

	public void UpdateTheCreationInfo()
	{
		Refers refers = UI_NewGame_Member.CGet<Refers>("SettingView");
		Refers refers2 = UI_NewGame_Member.CGet<Refers>("NameView");
		Refers refers3 = UI_NewGame_Member.CGet<Refers>("FaceView");
		Refers refers4 = UI_NewGame_Member.CGet<Refers>("HomeView");
		string text = UI_NewGame_Member.CGet<Refers>("NameView").CGet<TextMeshProUGUI>("Surname").text;
		string text2 = UI_NewGame_Member.CGet<Refers>("NameView").CGet<TextMeshProUGUI>("Name").text;
		ProtagonistCreationInfo protagonistCreationInfo = new ProtagonistCreationInfo();
		protagonistCreationInfo.Surname = text;
		protagonistCreationInfo.GivenName = text2;
		protagonistCreationInfo.Morality = (short)refers2.CGet<CSlider>("GoodnessSlider").value;
		protagonistCreationInfo.Gender = (sbyte)Traverse.Create(UI_NewGame_Member).Field("_gender").GetValue();
		protagonistCreationInfo.Age = (short)refers3.CGet<CSlider>("AgeSlider").value;
		protagonistCreationInfo.BirthMonth = (sbyte)refers3.CGet<CSlider>("BirthdaySlider").value;
		protagonistCreationInfo.Avatar = (AvatarData)Traverse.Create(UI_NewGame_Member).Field("_avatarData").GetValue();
		protagonistCreationInfo.Avatar.FormatDisabledElements();
		List<ProtagonistFeatureItem> list = (List<ProtagonistFeatureItem>)Traverse.Create(UI_NewGame_Member).Field("_selectedAbilities").GetValue();
		protagonistCreationInfo.ProtagonistFeatureIds = list.ConvertAll((ProtagonistFeatureItem e) => e.TemplateId);
		protagonistCreationInfo.TaiwuVillageStateTemplateId = (sbyte)refers4.CGet<CToggleGroup>("MapCells").GetActive().Key;
		protagonistCreationInfo.InscribedChar = null;
		protagonistCreationInfo.ClothingTemplateId = ItemTemplateHelper.GetClothingTemplateIdByDisplayId((byte)protagonistCreationInfo.Avatar.ClothDisplayId);
		this.protagonistCreationInfo = protagonistCreationInfo;
	}

	public void SendTheCreationInfo()
	{
		GameDataBridge.AddMethodCall(-1, 4, 0, protagonistCreationInfo);
		_bool_IsSendCreationInfo = true;
		_bool_IsGetCharacterData = false;
	}

	public void GetCharacterData()
	{
		string @string;
		using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.OpenExisting("QuicklyCreateCharacterData"))
		{
			using MemoryMappedViewAccessor memoryMappedViewAccessor = memoryMappedFile.CreateViewAccessor();
			byte[] array = new byte[memoryMappedViewAccessor.Capacity];
			memoryMappedViewAccessor.ReadArray(0L, array, 0, array.Length);
			@string = Encoding.Unicode.GetString(array);
		}
		characterDataList = JsonConvert.DeserializeObject<List<string>>(@string);
		characterDataDict = CharacterDataTool.CharacterDataListToDataDict(characterDataList);
		characterDataColorDict = CharacterDataTool.CharacterDataDictToColorDict(characterDataDict);
		characterDataNameDict = CharacterDataTool.CharacterDataDictToNameDict(characterDataDict);
		characterDataShortDict = CharacterDataTool.CharacterDataDictToShortDataDict(characterDataDict);
		_bool_IsGetCharacterData = true;
		DoUpdate();
	}

	public void DoUpdate()
	{
		if (this.updateDataEvent != null)
		{
			this.updateDataEvent();
		}
	}
}
