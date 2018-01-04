namespace Assets.Scripts.Enums
{
    public class Triggers
    {
        public string TriggerName { get; }

        private Triggers(string triggerName)
        {
            TriggerName = triggerName;
        }

        public static Triggers TidyPins = new Triggers("tidyPins");
        public static Triggers ResetPins = new Triggers("resetPins");
    }
}
