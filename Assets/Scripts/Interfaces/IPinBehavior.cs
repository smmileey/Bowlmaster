using Assets.Scripts.Enums;

namespace Assets.Scripts.Interfaces
{
    public interface IPinBehavior
    {
        void HandlePinBehavior(Pin[] pins, PinBehavior pinBehavior, float offsetMultiplier);
    }
}
