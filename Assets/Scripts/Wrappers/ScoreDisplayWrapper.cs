using UnityEngine.UI;

namespace Assets.Scripts.Wrappers
{
    public class ScoreDisplayWrapper
    {
        public int FrameIndex { get; set; }

        public Text FirstScore { get; set; }

        public Text SecondScore { get; set; }

        public Text ThirdScore { get; set; }

        public bool Completed { get; set; }
    }
}
