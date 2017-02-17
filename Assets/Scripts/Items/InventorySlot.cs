using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image _itemImage;
    private Item _item;

    private void Awake()
    {
        _itemImage = transform.GetComponent<Image>();
    }

    public void SetUnsetItem(Item i, int index)
    {
        _item = i;
        var enable = _item != null;
        _itemImage.enabled = enable;
        if (!enable) return;
        if (index < InventoryController.ItemsPerRow)
            _itemImage.raycastTarget = false;
        _itemImage.sprite = _item.GetSprite();
    }

    public Item GetItem()
    {
        return _item;
    }
}
