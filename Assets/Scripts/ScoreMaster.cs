using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Consts;

namespace Assets.Scripts
{
    public class ScoreMaster
    {
        private const int MinScoresCount = 1;
        private const int MinScoreValue = 0;
        private const int MaxScoresCount = 21;
        private const int MaxScoreValue = 10;
        private int _throwNumber;

        public List<int> GetFrameScores(List<int> throws)
        {
            if (throws == null) throw new ArgumentNullException(nameof(throws));
            if (throws.Count < MinScoresCount || throws.Count > MaxScoresCount) throw new ArgumentOutOfRangeException(nameof(throws));
            if (throws.Any(IsScoreInvalid)) throw new ArgumentOutOfRangeException(nameof(throws));

            var frameScores = new List<int>();
            _throwNumber = 1;

            while (_throwNumber < throws.Count)
            {
                int firstThrow = throws[_throwNumber - 1];
                int secondThrow = throws[_throwNumber];
                bool isLastFrame = frameScores.Count == Specification.MaxPinsCount - 1;
                bool strikeOrSpare = IsStrike(firstThrow) || IsSpare(firstThrow, secondThrow);

                if (isLastFrame && strikeOrSpare)
                {
                    if (IsStrikeOrSpareCalculable(throws)) frameScores.Add(ResolveStrikeOrSpareScore(throws, _throwNumber));
                    return frameScores;
                }

                if (strikeOrSpare)
                {
                    if (!IsStrikeOrSpareCalculable(throws)) return frameScores;
                    frameScores.Add(ResolveStrikeOrSpareScore(throws, _throwNumber));
                    _throwNumber += IsStrike(firstThrow) ? 1 : 2;
                }
                else
                {
                    frameScores.Add(ValidateAndResolveOpenScore(firstThrow, secondThrow));
                    _throwNumber += 2;
                }
            }
            return frameScores;
        }

        public int GetCurrentScore(List<int> throws)
        {
            List<int> frameScores = GetFrameScores(throws);
            return frameScores.Aggregate((first, second) => first + second);
        }

        private bool IsStrike(int firstThrow)
        {
            return firstThrow == Specification.MaxPinsCount;
        }

        private bool IsSpare(int firstThrow, int secondThrow)
        {
            return firstThrow + secondThrow == Specification.MaxPinsCount;
        }

        private bool IsStrikeOrSpareCalculable(List<int> throws)
        {
            return throws.Count >= _throwNumber + 2;
        }

        private int ResolveStrikeOrSpareScore(List<int> throws, int throwNumber)
        {
            return throws[throwNumber - 1] + throws[throwNumber] + throws[throwNumber + 1];
        }

        private int ValidateAndResolveOpenScore(int firstThrow, int secondThrow)
        {
            if (firstThrow + secondThrow <= Specification.MaxPinsCount) return firstThrow + secondThrow;
            throw new ArgumentOutOfRangeException();
        }

        private bool IsScoreInvalid(int score)
        {
            return score < MinScoreValue || score > MaxScoreValue;
        }
    }
}
