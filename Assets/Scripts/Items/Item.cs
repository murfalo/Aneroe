using System.Collections;
using SaveData;
using UnityEngine;

public class Item : MonoBehaviour, ISavable<ItemSaveData>
{
    // Components
    protected SpriteRenderer SRend;
    protected Collider2D PickupCollider;

    // Properties
    public string PrefabName;

    protected Entity Owner;

    // If true, item can be put into deep pocket
    public bool SmallItem;

    // Number of units of the item
    public int Count;

    /// <summary>The name that appears in this item's tooltip.</summary>
    public string Name;

    /// <summary>The description that appears in this item's tooltip.</summary>
    public string Description;

    public virtual void Setup()
    {
        SRend = GetComponent<SpriteRenderer>();
        PickupCollider = GetComponent<Collider2D>();
    }

    public virtual void PickupItem(Entity e)
    {
        Owner = e;
        transform.parent = e.transform;
        transform.localPosition = new Vector3(0, 0, 0);
        SRend.enabled = false;
        PickupCollider.enabled = false;
    }

    public virtual void DropItem(Vector3 pos)
    {
        Owner = null;
        transform.parent = GameObject.Find("Items").transform;
        transform.position = pos;
        SRend.enabled = true;
        PickupCollider.enabled = true;
        StartCoroutine(WaitToDespawn());
    }

    private IEnumerator WaitToDespawn()
    {
        yield return new WaitForSeconds(30);
        if (Owner == null) Destroy(gameObject);
    }

    public virtual void EquipItem(bool equip)
    {
        SRend.enabled = false;
    }

    // Returns entity that holds item
    public Entity GetEntity()
    {
        return Owner;
    }

    public void SetEntity(Entity e)
    {
        Owner = e;
    }

    public Sprite GetSprite()
    {
        return SRend.sprite;
    }

    public virtual ItemSaveData Save(ItemSaveData baseObj)
    {
        var isd = baseObj != default(ItemSaveData) ? baseObj : new ItemSaveData();
        isd.count = Count;
        isd.smallItem = SmallItem;
        isd.prefabName = PrefabName;
        return isd;
    }

    public virtual void Load(ItemSaveData isd)
    {
        Setup();
        Count = isd.count;
        SmallItem = isd.smallItem;
        // No need to laod prefabName as this prefab already has it 
    }
}