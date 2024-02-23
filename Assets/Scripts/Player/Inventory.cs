using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots; //this modify UI elements
    public ItemSlot[] slots; //this hold all the data
    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private ItemSlot selectedItem;
    private int selectedItemIndex;
    private int currentEquipIndex;

    //components
    private PlayerController controller;
    private PlayerNeeds needs;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    //singelton
    public static Inventory instance;

    private void Awake()
    {
        instance = this;

        controller = GetComponent<PlayerController>();
        needs = GetComponent<PlayerNeeds>();
    }

    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        //initialize slots
        for (int x = 0; x < slots.Length; x++)
        {
            slots[x] = new ItemSlot();
            uiSlots[x].index = x;
            uiSlots[x].Clear();
        }

        ClearSelectedItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Toggle();
    }

    public void Toggle()
    {
        if (inventoryWindow.activeInHierarchy)
        {
            inventoryWindow.SetActive(false);
            onCloseInventory.Invoke();
            controller.ToggleCursor(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory.Invoke();
            ClearSelectedItemWindow();
            controller.ToggleCursor(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)
    {
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);

            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }

        ThrowItem(item);
    }

    private void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360.0f));
    }

    private void UpdateUI()
    {
        for (int x = 0; x < slots.Length; x++)
        {
            if (slots[x].item != null)
                uiSlots[x].Set(slots[x]);
            else
                uiSlots[x].Clear();
        }
    }

    private ItemSlot GetItemStack(ItemData item)
    {
        for (int x = 0; x < slots.Length; x++)
        {
            if (slots[x].item == item && slots[x].quantity < item.maxStackAmount)
                return slots[x];
        }

        return null;
    }

    private ItemSlot GetEmptySlot()
    {
        for (int x = 0; x < slots.Length; x++)
        {
            if (slots[x].item == null)
                return slots[x];
        }

        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        //set stats value and stat name
        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        for (int x = 0; x < selectedItem.item.consumables.Length; x++)
        {
            selectedItemStatNames.text += selectedItem.item.consumables[x].type.ToString() + "\n";
            selectedItemStatValues.text += selectedItem.item.consumables[x].value.ToString() + "\n";
        }

        //set buttons
        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].isEquipped);
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].isEquipped);
        dropButton.SetActive(true);
    }

    private void ClearSelectedItemWindow()
    {
        //clear the text elements
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        //disable buttons
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if (selectedItem.item.type == ItemType.Consumable)
        {
            for (int x = 0; x < selectedItem.item.consumables.Length; x++)
            {
                switch (selectedItem.item.consumables[x].type)
                {
                    case ConsumableType.Hunger:
                        needs.Eat(selectedItem.item.consumables[x].value);
                        break;
                    case ConsumableType.Thirst:
                        needs.Drink(selectedItem.item.consumables[x].value);
                        break;
                    case ConsumableType.Health:
                        needs.Heal(selectedItem.item.consumables[x].value);
                        break;
                    case ConsumableType.Sleep:
                        needs.Sleep(selectedItem.item.consumables[x].value);
                        break;
                    default:
                        break;
                }
            }
        }
        RemoveSelectedItem();
    }

    public void OnEquipButton()
    {
        if (uiSlots[currentEquipIndex].isEquipped)
            UnEquip(currentEquipIndex);

        uiSlots[selectedItemIndex].isEquipped = true;
        currentEquipIndex = selectedItemIndex;
        EquipManager.instance.EquipNew(selectedItem.item);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    private void UnEquip(int index)
    {
        uiSlots[index].isEquipped = false;
        EquipManager.instance.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index)
            SelectItem(selectedItemIndex);
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity == 0)
        {
            if (uiSlots[selectedItemIndex].isEquipped == true)
                UnEquip(selectedItemIndex);

            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    public void RemoveItem(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].quantity--;

                if (slots[i].quantity == 0)
                {
                    if (uiSlots[i].isEquipped == true)
                        UnEquip(i);

                    slots[i].item = null;
                    ClearSelectedItemWindow();
                }

                UpdateUI();
                return;
            }
        }
    }

    public bool HasItems(ItemData item, int quantity)
    {
        int amount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
                amount += slots[i].quantity;

            if (amount >= quantity)
                return true;
        }
        return false;
    }
}

public class ItemSlot
{
    public ItemData item;
    public int quantity;
}
