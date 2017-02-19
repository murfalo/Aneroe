using System;
using System.Collections.Generic;

public class EntityController : BaseController
{
    protected Dictionary<string, Action<Entity>> actionResponses;

    public virtual void RespondToEntityAction(Entity e, string actionName)
    {
        if (actionResponses.ContainsKey(actionName))
            actionResponses[actionName](e);
    }
}

