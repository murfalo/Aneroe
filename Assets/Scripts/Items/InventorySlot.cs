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
        var xPos = Input.mousePosition.x + UIController.Tooltip.GetComponent<RectTransform>().sizeDelta.x / 2;
        var yPos = Input.mousePosition.y - UIController.Tooltip.GetComponent<RectTransform>().sizeDelta.y / 2;
        UIController.Tooltip.transform.position = new Vector2(xPos, yPos);
    }


    public void ToggleTooltip()
    {
        _tooltipEnabled = !UIController.Tooltip.activeSelf;
        UIController.Tooltip.SetActive(_tooltipEnabled);
        if (!_tooltipEnabled) return;
        TooltipName = _item.Name;
        TooltipDescription = _item.Description;
    }

    public void SetUnsetItem(Item i, int index = -1)
    {
        _item = i;
        var enable = _item != null;
        _itemImage.enabled = enable;
        if (!enable) return;
        if (index < InventoryController.ItemsPerRow && index > 0)
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
        ToggleTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToggleTooltip();
    }
}
