using System;

namespace Assets.Scripts
{
    public class ActionMaster
    {
        private readonly int[] _scores = new int[22];
        private int _throwNumber = 1;

        public AfterStrikeAction Bowl(int pins)
        {
            if (pins < 0 || pins > 10) throw new ArgumentOutOfRangeException(nameof(pins));
            _scores[_throwNumber] = pins;

            if (_throwNumber == 21)
            {
                return AfterStrikeAction.EndGame;
            }

            if (_throwNumber == 20)
            {
                _throwNumber++;
                return _scores[19] + pins >= 10
                    ? AfterStrikeAction.Reset
                    : AfterStrikeAction.EndGame;
            }

            if (pins == 10)
            {
                if (_throwNumber == 19)
                {
                    _throwNumber++;
                    return AfterStrikeAction.Reset;
                }

                _throwNumber += 2;
                return _throwNumber == 21
                    ? AfterStrikeAction.Reset 
                    : AfterStrikeAction.EndTurn;
            }

            if (_throwNumber % 2 != 0)
            {
                _throwNumber++;
                return AfterStrikeAction.Tidy;
            }
            else
            {
                _throwNumber++;
                return AfterStrikeAction.EndTurn;
            }
        }
    }
}
