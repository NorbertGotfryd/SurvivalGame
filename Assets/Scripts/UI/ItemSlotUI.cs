using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quantityText;
    public int index;
    public bool isEquipped;

    private ItemSlot currentSlot;
    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = isEquipped;
    }

    public void Set (ItemSlot slot)
    {
        currentSlot = slot;

        icon.gameObject.SetActive(true);
        icon.sprite = slot.item.icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;

        if (outline != null)
            outline.enabled = isEquipped;
    }

    public void Clear()
    {
        currentSlot = null;

        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    public void OnButtonClick()
    {
        Inventory.instance.SelectItem(index);
    }
}
