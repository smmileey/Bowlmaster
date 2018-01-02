using System;

namespace Assets.Scripts
{
    public class ActionMaster
    {
        public AfterStrikeAction Bowl(int pins)
        {
            if (pins < 0 || pins > 10) throw new ArgumentOutOfRangeException(nameof(pins));

            if (pins == 10) return AfterStrikeAction.EndTurn;
            throw new NotImplementedException();
        }
    }
}
