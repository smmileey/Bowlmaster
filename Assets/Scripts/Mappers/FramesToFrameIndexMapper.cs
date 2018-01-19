using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Mappers
{
    public class FramesToFrameIndexMapper
    {
        private static readonly Dictionary<Frames, int> Mappings = InitialzeMappings();

        private static Dictionary<Frames, int> InitialzeMappings()
        {
            return new Dictionary<Frames, int>
            {
                {Frames.F1, 1},
                {Frames.F2, 2},
                {Frames.F3, 3},
                {Frames.F4, 4},
                {Frames.F5, 5},
                {Frames.F6, 6},
                {Frames.F7, 7},
                {Frames.F8, 8},
                {Frames.F9, 9},
                {Frames.F10, 10},
            };
        }

        public static int GetFrame(Frames frame)
        {
            int frameIndex;
            if(!Mappings.TryGetValue(frame, out frameIndex)) throw new ArgumentOutOfRangeException(nameof(frame));

            return frameIndex;
        }
    }
}
