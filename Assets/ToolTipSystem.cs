using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    public static ToolTipSystem current;
    public ToolTip tooltip;
    // Start is called before the first frame update

    private void Awake()
    {
        current = this; 
    }
    public static void Show(string content, string header = "")
    {
        current.tooltip.SetText(content, header);
        current.tooltip.gameObject.SetActive(true);
    }

    // Update is called once per frame
    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
