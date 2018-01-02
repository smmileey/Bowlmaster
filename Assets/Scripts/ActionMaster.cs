using System;

namespace Assets.Scripts
{
    public class ActionMaster
    {
        private const int LastFrameFirstThrow = 19;
        private const int LastFrameSecondThrow = 20;
        private const int MaxThrowCount = 21;
        private const int MaxPinsCount = 10;

        private readonly int[] _scores = new int[MaxThrowCount];
        private int _currentThrowNumber = 1;

        public AfterStrikeAction Bowl(int pinsHitCount)
        {
            if (pinsHitCount < 0 || pinsHitCount > 10) throw new ArgumentOutOfRangeException(nameof(pinsHitCount));

            _scores[_currentThrowNumber - 1] = pinsHitCount;

            if (_currentThrowNumber == MaxThrowCount) return AfterStrikeAction.EndGame;

            if (_currentThrowNumber >= LastFrameFirstThrow && IsAdditionalThrowAwarded())
            {
                _currentThrowNumber++;
                return AfterStrikeAction.Reset;
            }

            if (_currentThrowNumber == LastFrameSecondThrow) return AfterStrikeAction.EndGame;

            if (pinsHitCount == MaxPinsCount)
            {
                _currentThrowNumber += 2;
                return AfterStrikeAction.EndTurn;
            }

            if (IsFirstThrowInFrame())
            {
                _currentThrowNumber++;
                return AfterStrikeAction.Tidy;
            }

            _currentThrowNumber++;
            return AfterStrikeAction.EndTurn;
        }

        private bool IsAdditionalThrowAwarded()
        {
            return _scores[LastFrameFirstThrow - 1] + _scores[LastFrameFirstThrow] >= MaxPinsCount;
        }

        private bool IsFirstThrowInFrame()
        {
            return _currentThrowNumber % 2 != 0;
        }
    }
}
