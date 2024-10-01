using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using HarmonyLib;
using UnityEngine;

namespace QuicklyCreateCharacterFrontend;

internal static class UITool
{
	private static Dictionary<UIElement, GameObject> _uiElementPrefabDict = new Dictionary<UIElement, GameObject>();

	public static List<GameObject> GetUIElementPrefabs(List<UIElement> uiElementList)
	{
		string path = (string)AccessTools.Field(typeof(UIElement), "rootPrefabPath").GetValue(null);
		GameObject[] returnGoArray = new GameObject[uiElementList.Count];
		Dictionary<int, UIElement> dictionary = new Dictionary<int, UIElement>();
		for (int i = 0; i < uiElementList.Count; i++)
		{
			UIElement uIElement = uiElementList[i];
			if (_uiElementPrefabDict.ContainsKey(uIElement))
			{
				returnGoArray[i] = _uiElementPrefabDict[uIElement];
			}
			else
			{
				dictionary.Add(i, uIElement);
			}
		}
		if (dictionary.Count > 0)
		{
			CountdownEvent countdownEvent = new CountdownEvent(dictionary.Count);
			UIElement uiElement;
			foreach (int index in dictionary.Keys)
			{
				try
				{
					uiElement = dictionary[index];
					string path2 = (string)AccessTools.Field(typeof(UIElement), "_path").GetValue(uiElement);
					ResLoader.Load(Path.Combine(path, path2), delegate(GameObject newGameObject)
					{
						_uiElementPrefabDict.Add(uiElement, newGameObject);
						returnGoArray[index] = newGameObject;
						countdownEvent.Signal();
					});
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					countdownEvent.Signal();
				}
			}
			countdownEvent.Wait();
		}
		return new List<GameObject>(returnGoArray);
	}

	public static Canvas GetRootCanvas(RectTransform rectTransform)
	{
		Canvas componentInParent = rectTransform.GetComponentInParent<Canvas>();
		if (componentInParent != null)
		{
			return componentInParent.isRootCanvas ? componentInParent : componentInParent.rootCanvas;
		}
		return null;
	}
}
