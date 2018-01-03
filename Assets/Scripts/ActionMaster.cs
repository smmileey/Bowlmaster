using System;
using Assets.Scripts.Enums;

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

            switch (_currentThrowNumber)
            {
                case MaxThrowCount:
                    Reset();
                    return AfterStrikeAction.EndGame;
                case LastFrameSecondThrow:
                    if (!IsAdditionalThrowAwarded())
                    {
                        Reset();
                        return AfterStrikeAction.EndGame;
                    }

                    _currentThrowNumber++;
                    return ArePinsKnockedDown(pinsHitCount) ? AfterStrikeAction.Reset : AfterStrikeAction.Tidy;
            }

            if (pinsHitCount == MaxPinsCount)
            {
                bool isLastFrameFirstThrow = _currentThrowNumber == LastFrameFirstThrow;
                _currentThrowNumber += isLastFrameFirstThrow ? 1 : 2;
                return isLastFrameFirstThrow ? AfterStrikeAction.Reset : AfterStrikeAction.EndTurn;
            }

            if (IsFirstThrowInFrame())
            {
                _currentThrowNumber++;
                return AfterStrikeAction.Tidy;
            }

            _currentThrowNumber++;
            return AfterStrikeAction.EndTurn;
        }

        private void Reset()
        {
            _currentThrowNumber = 1;
        }

        private bool IsAdditionalThrowAwarded()
        {
            return GetLastFrameScore() >= MaxPinsCount;
        }

        private bool ArePinsKnockedDown(int pinsHitCount)
        {
            bool strikeThisTurn = pinsHitCount == MaxPinsCount;
            bool spareAndNotGutterBallThisTurn = GetLastFrameScore() == MaxPinsCount && pinsHitCount != 0;
            return strikeThisTurn || spareAndNotGutterBallThisTurn;
        }

        private int GetLastFrameScore()
        {
            return _scores[LastFrameFirstThrow - 1] + _scores[LastFrameFirstThrow];
        }

        private bool IsFirstThrowInFrame()
        {
            return _currentThrowNumber % 2 != 0;
        }
    }
}
