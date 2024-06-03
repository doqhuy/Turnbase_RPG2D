using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    public GameObject skillbutton;

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject Header = skillbutton.transform.Find("Header").gameObject;
        GameObject Content = skillbutton.transform.Find("Content").gameObject;
        TMP_Text HeaderText = Header.GetComponentInChildren<TMP_Text>();
        TMP_Text contentText = Content.GetComponentInChildren<TMP_Text>();


        ToolTipSystem.Show(contentText.text.ToString(), HeaderText.text.ToString());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.Hide();
    }

    // Update is called once per frame

}
