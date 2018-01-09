using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class ScoreMaster
    {
        private int _throwNumber;

        public List<int> GetFrameScores(List<int> throws)
        {
            if (throws == null) throw new ArgumentNullException(nameof(throws));
            if (throws.Count < 1 || throws.Count > 21) throw new ArgumentOutOfRangeException(nameof(throws));
            if (throws.Any(IsScoreInvalid)) throw new ArgumentOutOfRangeException(nameof(throws));

            var frameScores = new List<int>();
            _throwNumber = 1;

            while (_throwNumber <= throws.Count)
            {
                if (throws.Count < _throwNumber + 1) return frameScores;

                int firstThrow = throws[_throwNumber - 1];
                int secondThrow = throws[_throwNumber];

                bool strikeOrSpare = IsStrike(firstThrow) || IsSpare(firstThrow, secondThrow);
                if (strikeOrSpare && !IsStrikeOrSpareCalculable(frameScores, throws)) return frameScores;

                bool isNextThrowAvailable = throws.Count >= _throwNumber + 1;
                if (!isNextThrowAvailable) return frameScores;

                frameScores.Add(ValidateAndResolveScore(firstThrow, secondThrow));
                _throwNumber += 2;
            }
            return frameScores;
        }

        private bool IsStrike(int firstThrow)
        {
            return firstThrow == ActionMaster.MaxPinsCount;
        }

        private bool IsSpare(int firstThrow, int secondThrow)
        {
            return firstThrow + secondThrow == 10;
        }

        private bool IsStrikeOrSpareCalculable(List<int> frameScores, List<int> throws)
        {
            if (!CanScoreBeCalculated(throws, _throwNumber)) return false;

            frameScores.Add(ResolveStrikeOrSpare(throws, _throwNumber));
            _throwNumber += 2;
            return true;
        }

        private int ResolveStrikeOrSpare(List<int> throws, int throwNumber)
        {
            return throws[throwNumber - 1] + throws[throwNumber] + throws[throwNumber + 1];
        }

        private bool CanScoreBeCalculated(List<int> throws, int throwNumber)
        {
            return throws.Count >= throwNumber + 2;
        }

        private int ValidateAndResolveScore(int firstThrow, int secondThrow)
        {
            if (firstThrow + secondThrow <= 10) return firstThrow + secondThrow;
            throw new ArgumentOutOfRangeException();
        }

        private bool IsScoreInvalid(int score)
        {
            return score < 0 || score > 10;
        }
    }
}
