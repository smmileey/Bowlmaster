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
        private int _frameNumber;

        private bool LastFrame => _frameNumber == Specification.MaxFrame;

        public List<int> GetFrameScores(List<int> throws)
        {
            if (throws == null) throw new ArgumentNullException(nameof(throws));
            if (throws.Count < MinScoresCount || throws.Count > MaxScoresCount) throw new ArgumentOutOfRangeException(nameof(throws));
            if (throws.Any(IsScoreInvalid)) throw new ArgumentOutOfRangeException(nameof(throws));

            var frameScores = new List<int>();
            _throwNumber = 1;
            _frameNumber = 1;

            while (_throwNumber <= throws.Count)
            {
                if (throws.Count < _throwNumber + 1) return frameScores;

                int firstThrow = throws[_throwNumber - 1];
                int secondThrow = throws[_throwNumber];
                bool strikeOrSpare = IsStrike(firstThrow) || IsSpare(firstThrow, secondThrow);

                if (LastFrame && strikeOrSpare)
                {
                    if (IsStrikeOrSpareCalculable(frameScores, throws)) frameScores.Add(ResolveStrikeOrSpare(throws, _throwNumber));
                    return frameScores;
                }

                if (strikeOrSpare)
                {
                    if (!IsStrikeOrSpareCalculable(frameScores, throws)) return frameScores;
                    frameScores.Add(ResolveStrikeOrSpare(throws, _throwNumber));
                    _throwNumber += IsStrike(firstThrow) ? 1 : 2;
                }
                else
                {
                    frameScores.Add(ValidateAndResolveScore(firstThrow, secondThrow));
                    _throwNumber += 2;
                }
                _frameNumber++;
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

        private bool IsStrikeOrSpareCalculable(List<int> frameScores, List<int> throws)
        {
            return throws.Count >= _throwNumber + 2;
        }

        private int ResolveStrikeOrSpare(List<int> throws, int throwNumber)
        {
            return throws[throwNumber - 1] + throws[throwNumber] + throws[throwNumber + 1];
        }

        private int ValidateAndResolveScore(int firstThrow, int secondThrow)
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
