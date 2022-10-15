using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NpcScan.Controller
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject tooltip;
        public string title;
        public TextMeshProUGUI characterName;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Enter");
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = characterName.text + "\n" + title;
            var position = Input.mousePosition;
            tooltip.GetComponent<RectTransform>().pivot = new Vector2(position.x / Screen.width, position.y / Screen.height);
            tooltip.transform.localPosition = position - new Vector3(Screen.width/2, Screen.height/2, 0);
            tooltip.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Exit");
            tooltip.gameObject.SetActive(false);
        }
    }
}
