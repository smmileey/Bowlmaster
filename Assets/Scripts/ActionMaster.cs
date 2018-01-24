using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Consts;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts
{
    public class ActionMaster
    {
        private const int LastFrameFirstThrow = 19;
        private const int LastFrameSecondThrow = 20;
        private const int MaxThrowCount = 21;

        private int CurrentThrowNumber { get; set; }

        public AfterStrikeAction NextAction(List<int> throws)
        {
            if (throws == null) throw new ArgumentNullException(nameof(throws));
            if (throws.Count < 1 || throws.Count > 21) throw new ArgumentOutOfRangeException(nameof(throws));

            CurrentThrowNumber = throws.Count;
            ValidateScoreInTurn(throws);

            switch (CurrentThrowNumber)
            {
                case MaxThrowCount:
                    return AfterStrikeAction.EndGame;
                case LastFrameSecondThrow:
                    if (!IsAdditionalThrowAwarded(throws)) return AfterStrikeAction.EndGame;

                    return ArePinsKnockedDown(throws) ? AfterStrikeAction.Reset : AfterStrikeAction.Tidy;
            }

            if (throws[CurrentThrowNumber - 1] == Specification.MaxPinsCount)
            {
                bool isLastFrameFirstThrow = CurrentThrowNumber == LastFrameFirstThrow;
                return isLastFrameFirstThrow ? AfterStrikeAction.Reset : AfterStrikeAction.EndTurn;
            }

            if (IsFirstThrowInFrame(throws)) return AfterStrikeAction.Tidy;

            return AfterStrikeAction.EndTurn;
        }

        private void ValidateScoreInTurn(List<int> throws)
        {
            bool strikeLastTurn = CurrentThrowNumber - 2 >= 0 && throws[CurrentThrowNumber - 2] == Specification.MaxPinsCount;
            if (IsFirstThrowInFrame(throws) || strikeLastTurn) return;

            bool maxPinCountExceededThisTurn = throws[CurrentThrowNumber - 2] + throws[CurrentThrowNumber - 1] > Specification.MaxPinsCount;
            if (maxPinCountExceededThisTurn) throw new UnityException("Sum of last two throws exceeded 10!");
        }

        private bool IsAdditionalThrowAwarded(List<int> throws)
        {
            return GetLastFrameScore(throws) >= Specification.MaxPinsCount;
        }

        private bool ArePinsKnockedDown(List<int> throws)
        {
            bool strikeThisTurn = throws[CurrentThrowNumber - 1] == Specification.MaxPinsCount;
            bool spareThisTurn = GetLastFrameScore(throws) == Specification.MaxPinsCount && throws[CurrentThrowNumber - 1] != 0;
            return strikeThisTurn || spareThisTurn;
        }

        private bool IsFirstThrowInFrame(List<int> throws)
        {
            int sum = throws
                .Select((element, index) => CalculateTotalThrowsNumber(throws, element, index))
                .Sum();
                
            return sum % 2 != 0;
        }

        private static int CalculateTotalThrowsNumber(List<int> throws, int element, int index)
        {
            if (element != Specification.MaxPinsCount) return 1;

            bool nextThrowExist = index + 2 > throws.Count;
            bool isOddPosition = index % 2 == 0;

            if (isOddPosition && nextThrowExist) return 1;
            if (!isOddPosition && throws[index - 1] == 0) return 1;

            return nextThrowExist ? 1 : 2;
        }

        private int GetLastFrameScore(List<int> throws)
        {
            return throws[LastFrameFirstThrow - 1] + throws[LastFrameFirstThrow];
        }
    }
}
