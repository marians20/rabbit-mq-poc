using System;
using System.Text.Json;

namespace Events
{
    public abstract class EventBase
    {
        protected EventBase()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public static CreateUserEvent Deserialize(string value)
        {
            return JsonSerializer.Deserialize<CreateUserEvent>(value);
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, GetType());
        }
    }
}
