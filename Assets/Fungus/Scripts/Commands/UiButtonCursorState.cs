using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum EnableDisable
{
    Enable,
    Disable
}
namespace Fungus
{
    /// <summary>
    /// Mouse cursor state when hovering ui buttons.
    /// </summary>
    [CommandInfo("UI",
                "OnMouseCursorHoverButton",
                "Change cursor state when entering a button")]
    [AddComponentMenu("")]
    public class UiButtonCursorState : Command
    {
        [System.Serializable]
        public class UiButtonMouseChange
        {
            public EnableDisable enable = EnableDisable.Disable;
            public Button UiButton;
            public Texture2D mouseTexture;
            public Vector2 hotSpot = Vector2.zero;
            public Flowchart flowchart;
            public string ExecBlock;
            [HideInInspector] public bool eventAdded = false;
        }
        [Tooltip("Assign UI buttons.")]
        [SerializeField] private UiButtonMouseChange[] buttons;
        [Tooltip("Optional, if the game already has a custom cursor set. If null, the default system cursor will be used.")]
        [SerializeField] private Texture2D defaultMouseCursor;
        [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
        public virtual bool EnableCursorStateOverButton{get;set;}
        private bool custMouse = false;
       
        private void MouseActions(UiButtonMouseChange uis)
        {
            if (uis != null && uis.enable == EnableDisable.Enable)
            {
                if (uis.mouseTexture == null)
                    return;

                custMouse = true;
                Cursor.SetCursor(uis.mouseTexture, uis.hotSpot, cursorMode);

                if(uis.flowchart != null)
                {
                    if (uis.ExecBlock != string.Empty && uis.flowchart.HasBlock(uis.ExecBlock))
                    {
                        if(uis.eventAdded == false)
                        {
                            uis.UiButton.onClick.AddListener(() => uis.flowchart.ExecuteBlock(uis.ExecBlock));
                            uis.eventAdded = true;
                        }
                    }
                }
            }
        }
        public virtual void SetDefaultMouseCursor()
        {
            if (custMouse)
            {
                if(defaultMouseCursor == null)
                    Cursor.SetCursor(null, Vector2.zero, cursorMode);
                else
                    Cursor.SetCursor(defaultMouseCursor, Vector2.zero, cursorMode);

                custMouse = false;
            }
        }
        public virtual void DisableUiButtonCustomState()
        {
            EnableCursorStateOverButton = false;
        }
        // Update is called once per frame
        void Update()
        {
            if(EnableCursorStateOverButton)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    void EventLoops()
                    {
                        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                        List<RaycastResult> results = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

                        for (int h = 0; h < results.Count; h++)
                        {
                            for (int j = 0; j < buttons.Length; j++)
                            {
                                if (results[h].gameObject.GetInstanceID() == buttons[j].UiButton.gameObject.GetInstanceID())
                                {
                                    MouseActions(buttons[j]);
                                    return;
                                }
                            }
                        }
                    }
                    EventLoops();
                }
                else
                {
                    if (custMouse)
                        SetDefaultMouseCursor();
                }
            }
        }
        public override void OnEnter()
        {
            EnableCursorStateOverButton = true;
            Continue();
        }
        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }
    }
}
