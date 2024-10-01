using UnityEngine;

namespace QuicklyCreateCharacterFrontend;

public class UIController : MonoBehaviour
{
	public CharacterDataController dataController;

	private bool _bool_WindowShow;

	private Rect _winRect;

	private Rect _buttonRect;

	private void Awake()
	{
		_winRect = new Rect(new Vector2(50f, 400f), new Vector2(300f, 600f));
		UpdateButtonRect();
	}

	private void UpdateButtonRect()
	{
		_buttonRect = new Rect(new Vector2(_winRect.xMax - 150f - 50f, _winRect.yMax + 10f), new Vector2(100f, 40f));
	}

	private void OnGUI()
	{
		if (_bool_WindowShow)
		{
			_winRect = GUILayout.Window(1, _winRect, WindowFunc, "<b><color=#ffffffff><size=16>随机人物属性</size></color></b>");
			UpdateButtonRect();
			if (GUI.Button(_buttonRect, "<b><size=16>随机属性</size></b>"))
			{
				DoClickRoll();
			}
		}
	}

	private void WindowFunc(int winId)
	{
		GUI.DragWindow();
		if (dataController.characterDataList.Count > 0)
		{
			string text = dataController.characterDataDict[CharacterDataType.LifeSkillGrowthType][0];
			string text2 = dataController.characterDataNameDict[CharacterDataType.LifeSkillGrowthType][0];
			string text3 = dataController.characterDataColorDict[CharacterDataType.LifeSkillGrowthType][0];
			GUILayout.Space(10f);
			GUILayout.BeginVertical(GUI.skin.box);
			GUILayout.Label("<b><color=" + text3 + "><size=16>技艺资质（" + text2 + "）</size></color></b>");
			for (int i = 0; i < 16; i += 4)
			{
				GUILayout.BeginHorizontal();
				for (int j = 0; j < 4 && i + j < 16; j++)
				{
					string text4 = dataController.characterDataDict[CharacterDataType.LifeSkillQualification][i + j];
					string text5 = dataController.characterDataNameDict[CharacterDataType.LifeSkillQualification][i + j];
					string text6 = dataController.characterDataColorDict[CharacterDataType.LifeSkillQualification][i + j];
					GUILayout.Label("<b><color=" + text6 + "><size=14>" + text5 + "  " + text4 + "</size></color></b>");
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			string text7 = dataController.characterDataDict[CharacterDataType.CombatSkillGrowthType][0];
			string text8 = dataController.characterDataNameDict[CharacterDataType.CombatSkillGrowthType][0];
			string text9 = dataController.characterDataColorDict[CharacterDataType.CombatSkillGrowthType][0];
			GUILayout.BeginVertical(GUI.skin.box);
			GUILayout.Label("<b><color=" + text9 + "><size=16>功法资质（" + text8 + "）</size></color></b>");
			for (int k = 0; k < 14; k += 4)
			{
				GUILayout.BeginHorizontal();
				for (int l = 0; l < 4 && k + l < 14; l++)
				{
					string text10 = dataController.characterDataDict[CharacterDataType.CombatSkillQualification][k + l];
					string text11 = dataController.characterDataNameDict[CharacterDataType.CombatSkillQualification][k + l];
					string text12 = dataController.characterDataColorDict[CharacterDataType.CombatSkillQualification][k + l];
					GUILayout.Label("<b><color=" + text12 + "><size=14>" + text11 + "  " + text10 + "</size></color></b>");
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			GUILayout.BeginVertical(GUI.skin.box);
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b><size=16>特性</size></b>");
			for (int m = 0; m < 3; m++)
			{
				string text13 = dataController.characterDataDict[CharacterDataType.FeatureMedalValue][m];
				string text14 = dataController.characterDataNameDict[CharacterDataType.FeatureMedalValue][m];
				string text15 = dataController.characterDataColorDict[CharacterDataType.FeatureMedalValue][m];
				GUILayout.Label("<b><color=" + text15 + "><size=14>" + text14 + "×" + text13.TrimStart('-') + "</size></color></b>");
			}
			GUILayout.EndHorizontal();
			int num;
			for (num = 0; num < dataController.characterDataNameDict[CharacterDataType.FeatureIds].Count; num++)
			{
				GUILayout.BeginHorizontal();
				for (int n = 0; n < 4 && num + n < dataController.characterDataNameDict[CharacterDataType.FeatureIds].Count; n++)
				{
					string text16 = dataController.characterDataNameDict[CharacterDataType.FeatureIds][num + n];
					string text17 = dataController.characterDataColorDict[CharacterDataType.FeatureIds][num + n];
					GUILayout.Label("<b><color=" + text17 + "><size=14>" + text16 + "</size></color></b>");
				}
				GUILayout.EndHorizontal();
				num += 4;
			}
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			GUILayout.BeginVertical(GUI.skin.box);
			GUILayout.Label("<b><size=16>主属性</size></b>");
			for (int num2 = 0; num2 < 6; num2 += 3)
			{
				GUILayout.BeginHorizontal();
				for (int num3 = 0; num3 < 3 && num2 + num3 < 6; num3++)
				{
					string text18 = dataController.characterDataDict[CharacterDataType.MainAttribute][num2 + num3];
					string text19 = dataController.characterDataNameDict[CharacterDataType.MainAttribute][num2 + num3];
					string text20 = dataController.characterDataColorDict[CharacterDataType.MainAttribute][num2 + num3];
					GUILayout.Label("<b><color=" + text20 + "><size=14>" + text19 + " " + text18 + "</size></color></b>");
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			if (dataController.characterDataDict.ContainsKey(CharacterDataType.LifeSkillBookName))
			{
				string text21 = dataController.characterDataNameDict[CharacterDataType.LifeSkillBookName][0];
				string text22 = dataController.characterDataColorDict[CharacterDataType.LifeSkillBookName][0];
				string text23 = dataController.characterDataNameDict[CharacterDataType.LifeSkillBookType][0];
				string text24 = dataController.characterDataNameDict[CharacterDataType.CombatSkillBookName][0];
				string text25 = dataController.characterDataColorDict[CharacterDataType.CombatSkillBookName][0];
				GUILayout.Space(10f);
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label("<b><size=16>古冢遗刻</size></b>");
				GUILayout.BeginHorizontal();
				GUILayout.Label("<b><color=" + text22 + "><size=16>" + text21 + "</size></color></b>");
				GUILayout.Label("<b><size=14>" + text23 + "</size></b>");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("<b><color=" + text25 + "><size=16>" + text24 + "</size></color></b>");
				for (int num4 = 0; num4 < dataController.characterDataNameDict[CharacterDataType.CombatSkillBookPageType].Count; num4++)
				{
					string text26 = dataController.characterDataNameDict[CharacterDataType.CombatSkillBookPageType][num4];
					string text27 = ((num4 > 0) ? dataController.characterDataColorDict[CharacterDataType.CombatSkillBookPageType][num4] : CharacterDataTool.GetColorWhite(""));
					GUILayout.Label("<b><color=" + text27 + "><size=14>" + text26 + "</size></color></b>");
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
		}
		else
		{
			GUILayout.Label("<b><size=30>Loading</size></b>");
		}
	}

	public void ShowUI()
	{
		_bool_WindowShow = true;
	}

	public void CloseUI()
	{
		_bool_WindowShow = false;
	}

	public void DoClickRoll()
	{
		dataController.DoRollCharacterData();
	}
}
