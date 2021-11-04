using System;
using System.Text.Json;

namespace Events
{
    public sealed class CreateUserEvent: EventBase
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
