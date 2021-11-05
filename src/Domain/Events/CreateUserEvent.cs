using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;

namespace Events
{
    public sealed class CreateUserEvent: EventBase
    {
        public static CreateUserEvent GetRandomCreateUserEvent()
        {
            var randomizerFirstName = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());
            var randomizerLastName = RandomizerFactory.GetRandomizer(new FieldOptionsLastName());

            return new CreateUserEvent
            {
                FirstName = randomizerFirstName.Generate(),
                LastName = randomizerLastName.Generate()
            };
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
