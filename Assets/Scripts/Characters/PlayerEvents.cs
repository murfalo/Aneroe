using System;

namespace PlayerEvents
{
    public class PlayerHealthChangedEventArgs : EventArgs
    {
        public PlayerHealthChangedEventArgs(Entity e)
        {
            NewHealth = e.stats.GetStat("health");
        }

        public float NewHealth;
    }
}
