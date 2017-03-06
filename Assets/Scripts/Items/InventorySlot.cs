using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image _itemImage;
    private Item _item;

    private bool _tooltipEnabled;

    private static string TooltipName
    {
        set { UIController.Tooltip.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = value; }
    }

    private static string TooltipDescription
    {
        set { UIController.Tooltip.transform.GetChild(1).GetComponent<Text>().text = value; }
    }

    private void Awake()
    {
        _itemImage = transform.GetComponent<Image>();
    }

    private void Update()
    {
        if (!_tooltipEnabled) return;
        var scaleFactor = GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
        var xPos = Input.mousePosition.x + UIController.Tooltip.GetComponent<RectTransform>().sizeDelta.x / 2 * scaleFactor;
        var yPos = Input.mousePosition.y - UIController.Tooltip.GetComponent<RectTransform>().sizeDelta.y / 2 * scaleFactor;
        UIController.Tooltip.transform.position = new Vector2(xPos, yPos);
    }


    public void ToggleTooltip()
    {
        UIController.Tooltip.SetActive(_tooltipEnabled);
        if (!_tooltipEnabled) return;
        TooltipName = _item.Name;
        TooltipDescription = _item.Description.Replace("\\n","\n");
    }

    public void SetUnsetItem(Item i, int index = -1)
    {
        _item = i;
        var enable = _item != null;
        _itemImage.enabled = enable;
        if (!enable) return;
        if (index >= 0 && index < InventoryController.ItemsPerRow)
            _itemImage.raycastTarget = false;
        _itemImage.sprite = _item.GetSprite();
    }

    public Item GetItem()
    {
        return _item;
    }

    public void SetItem(Item i)
    {
        _item = i;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltipEnabled = true;
        ToggleTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipEnabled = false;
        ToggleTooltip();
    }
}
