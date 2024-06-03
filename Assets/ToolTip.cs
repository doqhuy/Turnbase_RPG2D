using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    public TextMeshProUGUI headerF;
    public TextMeshProUGUI contentF;
    public LayoutElement layoutElement;
    public RectTransform backgroundRectTransform;
    public int characterWrapLimit;
    // Start is called before the first frame update
    public void SetText(string content, string header ="")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerF.gameObject.SetActive(false);
        }
        else
        {
            headerF.gameObject.SetActive(true);
            headerF.text = header;
        }
        contentF.text = content;

        int headerLength = headerF.text.Length;
        int contentLength = contentF.text.Length;
        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;

        // Convert screen coordinates to local coordinates
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent.GetComponent<RectTransform>(),
            mousePosition,
            null,
            out Vector2 localPoint
        );

        // Set the tooltip position
        transform.localPosition = localPoint;

        // Shift the tooltip to the top right of the mouse
        Vector2 tooltipSize = backgroundRectTransform.sizeDelta;
        transform.localPosition += new Vector3(tooltipSize.x / 2f, tooltipSize.y / 2f, 0f);


    }
}
