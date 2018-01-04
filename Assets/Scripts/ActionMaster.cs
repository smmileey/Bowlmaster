using System;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts
{
    public class ActionMaster
    {
        private const int LastFrameFirstThrow = 19;
        private const int LastFrameSecondThrow = 20;
        private const int MaxThrowCount = 21;
        private const int MaxPinsCount = 10;
        private readonly int[] _scores = new int[MaxThrowCount];

        private bool IsAdditionalThrowAwarded => GetLastFrameScore() >= MaxPinsCount;
        private bool IsFirstThrowInFrame => CurrentThrowNumber % 2 != 0;

        public ActionMaster()
        {
            CurrentThrowNumber = 1;
        }

        public int CurrentThrowNumber { get; set; }

        public AfterStrikeAction Bowl(int pinsHitCount)
        {
            if (pinsHitCount < 0 || pinsHitCount > 10) throw new ArgumentOutOfRangeException(nameof(pinsHitCount));
            ValidateScoreInTurn(pinsHitCount);

            _scores[CurrentThrowNumber - 1] = pinsHitCount;

            switch (CurrentThrowNumber)
            {
                case MaxThrowCount:
                    ResetActionMaster();
                    return AfterStrikeAction.EndGame;
                case LastFrameSecondThrow:
                    if (!IsAdditionalThrowAwarded)
                    {
                        ResetActionMaster();
                        return AfterStrikeAction.EndGame;
                    }

                    CurrentThrowNumber++;
                    return ArePinsKnockedDown(pinsHitCount) ? AfterStrikeAction.Reset : AfterStrikeAction.Tidy;
            }

            if (pinsHitCount == MaxPinsCount)
            {
                bool isLastFrameFirstThrow = CurrentThrowNumber == LastFrameFirstThrow;
                CurrentThrowNumber += isLastFrameFirstThrow || !IsFirstThrowInFrame ? 1 : 2;
                return isLastFrameFirstThrow ? AfterStrikeAction.Reset : AfterStrikeAction.EndTurn;
            }

            if (IsFirstThrowInFrame)
            {
                CurrentThrowNumber++;
                return AfterStrikeAction.Tidy;
            }

            CurrentThrowNumber++;
            return AfterStrikeAction.EndTurn;
        }

        private void ValidateScoreInTurn(int pinsHitCount)
        {
            bool strikeLastTurn = CurrentThrowNumber - 2 >= 0 && _scores[CurrentThrowNumber - 2] == MaxPinsCount;
            if (IsFirstThrowInFrame || strikeLastTurn) return;

            bool maxPinCountExceededThisTurn = _scores[CurrentThrowNumber - 2] + pinsHitCount > MaxPinsCount;
            if (maxPinCountExceededThisTurn) throw new UnityException("Sum of last two throws exceeded 10!");
        }

        private void ResetActionMaster()
        {
            CurrentThrowNumber = 1;
        }

        private bool ArePinsKnockedDown(int pinsHitCount)
        {
            bool strikeThisTurn = pinsHitCount == MaxPinsCount;
            bool spareThisTurn = GetLastFrameScore() == MaxPinsCount && pinsHitCount != 0;
            return strikeThisTurn || spareThisTurn;
        }

        private int GetLastFrameScore()
        {
            return _scores[LastFrameFirstThrow - 1] + _scores[LastFrameFirstThrow];
        }
    }
}
