using UnityEngine;
using System.Collections;
using Fungus;

[EventHandlerInfo("", "On Execute", "Executes on a default Unity event")]
public class ExecuteEventHandler : EventHandler
{
    public enum ExecuteHandlerContext
    {
        Self,
        Static
    }

    public ExecuteHandler.ExecuteMethod executeEvent;

    [Tooltip("Choose target for event listening")]
    public ExecuteHandlerContext context;

    [Tooltip("Target gameObject if context is Static")]
    [HideInInspector]
    public GameObjectData targetObject;

    void Awake()
    {
        if (context == ExecuteHandlerContext.Self)
        {
            AddEventHandler(this.gameObject, executeEvent);
        }
        else if (context == ExecuteHandlerContext.Static)
        {
            AddEventHandler(targetObject.Value, executeEvent);
        }
    }

    private void AddEventHandler(GameObject target, ExecuteHandler.ExecuteMethod method)
    {
        if (target != null)
        {
            var executeHandler = target.AddComponent<ExecuteHandler>();
            executeHandler.executeMethods = method;
            executeHandler.onExecute = (args) =>
            {
                this.ExecuteBlock();
            };
        }
    }
}