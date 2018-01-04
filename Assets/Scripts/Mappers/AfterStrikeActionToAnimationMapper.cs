using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Mappers
{
    public class AfterStrikeActionToAnimationMapper
    {
        private static Dictionary<AfterStrikeAction, Triggers> _mappings = InitializeMappings();

        private static Dictionary<AfterStrikeAction, Triggers> InitializeMappings()
        {
            return new Dictionary<AfterStrikeAction, Triggers>()
            {
                {AfterStrikeAction.Tidy, Triggers.TidyPins },
                {AfterStrikeAction.Reset, Triggers.ResetPins },
                {AfterStrikeAction.EndTurn, Triggers.ResetPins },
                {AfterStrikeAction.EndGame, Triggers.ResetPins }
            };
        }

        public static Triggers Map(AfterStrikeAction afterStrikeAction)
        {
            Triggers trigger;
            if (!_mappings.TryGetValue(afterStrikeAction, out trigger)) throw new ArgumentOutOfRangeException(nameof(afterStrikeAction));

            return trigger;
        }
    }
}
