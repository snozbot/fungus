using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CGTUnity.Fungus.SaveSystem;

/// <summary>
/// Helps test the Save Menu UI without requiring user input to progress the menus along.
/// </summary>
public class ButtonTestFuncs : MonoBehaviour
{
    SaveSlotManager slotManager;
    EventSystem eventSystem;

    void Awake()
    {
        slotManager = FindObjectOfType<SaveSlotManager>();
        
    }

    void Start()
    {
        eventSystem =                       EventSystem.current;
    }

    public void SelectButton(Button button)
    {
        var pointerEventData = new PointerEventData(eventSystem);
        button.OnPointerDown(pointerEventData);
    }

    public void InvokeClick(Button button)
    {
        PointerEventData eventData =        new PointerEventData(eventSystem);
        button.OnPointerClick(eventData);
    }

    public void ClickSaveSlot(int indexNumber)
    {
        slotManager.SelectSlot(indexNumber);
        PointerEventData eventData =        new PointerEventData(eventSystem);
        Button slotButton =                 slotManager.selectedSlot.GetComponent<Button>();
        eventSystem.SetSelectedGameObject(slotButton.gameObject);

        slotButton.OnPointerClick(eventData);
    }

    public void SelectSaveSlot(int indexNumber)
    {
        slotManager.SelectSlot(indexNumber);
        PointerEventData eventData =        new PointerEventData(eventSystem);
        Button slotButton =                 slotManager.selectedSlot.GetComponent<Button>();
        eventSystem.SetSelectedGameObject(slotButton.gameObject);
    }

    
}
