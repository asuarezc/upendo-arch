using System;

namespace upendo.Entities
{
    public abstract class EntityBase
    {
        public DateTime UTCCreatedAt { get; private set; }

        protected EntityBase()
        {
            UTCCreatedAt = DateTime.UtcNow;
        }
    }
}
