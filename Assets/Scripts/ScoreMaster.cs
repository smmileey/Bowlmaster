
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class ScoreMaster
    {
        public List<int> GetFrameScores(List<int> throws)
        {
            if (throws == null) throw new ArgumentNullException(nameof(throws));
            if (throws.Count < 1 || throws.Count > 21) throw new ArgumentOutOfRangeException(nameof(throws));
            if (throws.Any(IsScoreInvalid)) throw new ArgumentOutOfRangeException(nameof(throws));

            var frameScores = new List<int>();
            int throwNumber = 1;

            if (throws.Count < throwNumber) return frameScores;

            do
            {
                int firstThrow = throws[throwNumber - 1];
                if (firstThrow == 10)
                {
                    if (!CanStrikeBeCalculated(throws, throwNumber)) return frameScores;
                    frameScores.Add(Strike(throws, throwNumber));
                    throwNumber += 2;
                }
                else
                {
                    if (throws.Count < throwNumber + 1) return frameScores;
                    int secondThrow = throws[throwNumber];
                    if (IsSpare(firstThrow, secondThrow))
                    {
                        if (!CanSpareBeCalculated(throws, throwNumber)) return frameScores;
                        frameScores.Add(Spare(throws, throwNumber));
                        throwNumber += 2;
                    }
                    else
                    {
                        bool isNextThrowAvailable = throws.Count >= throwNumber + 1; 
                        if (!isNextThrowAvailable) return frameScores;
                        frameScores.Add(GetSimpleScore(firstThrow, secondThrow));
                        throwNumber += 2;
                    }
                }
            } while (throwNumber <= throws.Count);

            return frameScores;
        }

        private int GetSimpleScore(int firstThrow, int secondThrow)
        {
            if (firstThrow + secondThrow <= 10) return firstThrow + secondThrow;
            throw new ArgumentOutOfRangeException();
        }

        private bool IsSpare(int firstThrow, int secondThrow)
        {
            return firstThrow + secondThrow == 10;
        }

        private int Strike(List<int> throws, int throwNumber)
        {
            return throws[throwNumber - 1] + throws[throwNumber] + throws[throwNumber + 1];
        }

        private int Spare(List<int> throws, int throwNumber)
        {
            return throws[throwNumber - 1] + throws[throwNumber] + throws[throwNumber + 1];
        }

        private bool CanStrikeBeCalculated(List<int> throws, int throwNumber)
        {
            return throws.Count >= throwNumber + 2;
        }

        private bool CanSpareBeCalculated(List<int> throws, int throwNumber)
        {
            return throws.Count >= throwNumber + 2;
        }

        private bool IsScoreInvalid(int score)
        {
            return score < 0 || score > 10;
        }
    }
}
