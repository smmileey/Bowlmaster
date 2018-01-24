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

        private int ConsecutiveThrowNumber { get; set; }

        public AfterStrikeAction NextAction(List<int> throws)
        {
            if (throws == null) throw new ArgumentNullException(nameof(throws));
            if (throws.Count < 1 || throws.Count > 21) throw new ArgumentOutOfRangeException(nameof(throws));

            ConsecutiveThrowNumber = throws.Count;
            int throwNumberWithStrikesIncluded = CalculateThrowNumberWithStrikesIncluded(throws);

            ValidateScoreInTurn(throws);

            switch (throwNumberWithStrikesIncluded)
            {
                case MaxThrowCount:
                    return AfterStrikeAction.EndGame;
                case LastFrameSecondThrow:
                    if (!IsAdditionalThrowAwarded(throws)) return AfterStrikeAction.EndGame;

                    return ArePinsKnockedDownLastTurn(throws) ? AfterStrikeAction.Reset : AfterStrikeAction.Tidy;
            }

            if (throws[ConsecutiveThrowNumber - 1] == Specification.MaxPinsCount)
            {
                bool isLastFrameFirstThrow = throwNumberWithStrikesIncluded == LastFrameFirstThrow;
                return isLastFrameFirstThrow ? AfterStrikeAction.Reset : AfterStrikeAction.EndTurn;
            }

            return IsFirstThrowInFrame(throws) ? AfterStrikeAction.Tidy : AfterStrikeAction.EndTurn;
        }

        private void ValidateScoreInTurn(List<int> throws)
        {
            bool strikeLastTurn = ConsecutiveThrowNumber - 2 >= 0 && throws[ConsecutiveThrowNumber - 2] == Specification.MaxPinsCount;
            if (IsFirstThrowInFrame(throws) || strikeLastTurn) return;

            bool maxPinCountExceededThisTurn = throws[ConsecutiveThrowNumber - 2] + throws[ConsecutiveThrowNumber - 1] > Specification.MaxPinsCount;
            if (maxPinCountExceededThisTurn) throw new UnityException("Sum of last two throws exceeded 10!");
        }

        private static int CalculateThrowNumberWithStrikesIncluded(List<int> throws)
        {
            int sum = 0;
            for (int index = 0; index < throws.Count; index++)
            {
                bool isLastRound = sum + 1 >= LastFrameFirstThrow;
                int element = throws[index];

                if (element != Specification.MaxPinsCount || isLastRound)
                {
                    sum++;
                    continue;
                }

                bool isLastThrow = index + 2 > throws.Count;
                bool isOddPosition = index % 2 == 0;

                if (isOddPosition && isLastThrow)
                {
                    sum++;
                    continue;
                }
                if (!isOddPosition && throws[index - 1] == 0)
                {
                    sum++;
                    continue;
                }

                sum += isLastThrow ? 1 : 2;
            }

            return sum;
        }

        private bool IsAdditionalThrowAwarded(List<int> throws)
        {
            return GetLastFrameScore(throws) >= Specification.MaxPinsCount;
        }

        private bool ArePinsKnockedDownLastTurn(List<int> throws)
        {
            bool isStrike = throws[ConsecutiveThrowNumber - 1] == Specification.MaxPinsCount;
            bool isSpare = GetLastFrameScore(throws) == Specification.MaxPinsCount && throws[ConsecutiveThrowNumber - 1] != 0;
            return isStrike || isSpare;
        }

        private bool IsFirstThrowInFrame(List<int> throws)
        {
            return CalculateThrowNumberWithStrikesIncluded(throws) % 2 != 0;
        }

        private int GetLastFrameScore(List<int> throws)
        {
            return throws.Skip(Math.Max(0, throws.Count() - 2)).Sum();
        }
    }
}
