using Fungus.SaveSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Helps test the Save Menu UI without requiring user input to progress the menus along.
/// </summary>
public class ButtonTestFuncs : MonoBehaviour
{
    private SaveSlotManager slotManager;
    private EventSystem eventSystem;

    private void Awake()
    {
        slotManager = FindObjectOfType<SaveSlotManager>();
    }

    private void Start()
    {
        eventSystem = EventSystem.current;
    }

    public void SelectButton(Button button)
    {
        var pointerEventData = new PointerEventData(eventSystem);
        button.OnPointerDown(pointerEventData);
    }

    public void InvokeClick(Button button)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        button.OnPointerClick(eventData);
    }

    public void ClickSaveSlot(int indexNumber)
    {
        slotManager.SelectSlot(indexNumber);
        PointerEventData eventData = new PointerEventData(eventSystem);
        Button slotButton = slotManager.selectedSlot.GetComponent<Button>();
        eventSystem.SetSelectedGameObject(slotButton.gameObject);

        slotButton.OnPointerClick(eventData);
    }

    public void SelectSaveSlot(int indexNumber)
    {
        slotManager.SelectSlot(indexNumber);
        PointerEventData eventData = new PointerEventData(eventSystem);
        Button slotButton = slotManager.selectedSlot.GetComponent<Button>();
        eventSystem.SetSelectedGameObject(slotButton.gameObject);
    }
}