using System;

namespace GameActions
{
    [Serializable]
    public abstract class Action
    {
        public abstract void Invoke();
    }
}
