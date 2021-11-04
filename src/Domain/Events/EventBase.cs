using System.Text.Json;

namespace Events
{
    public abstract class EventBase
    {
        public static CreateUserEvent Deserialize(string value)
        {
            return JsonSerializer.Deserialize<CreateUserEvent>(value);
        }
    }
}
