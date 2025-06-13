using UnityEngine;

namespace sc.terrain.proceduralpainter
{
    public class Attributes
    {
        public class ResolutionDropdown : PropertyAttribute
        {
            public int max;
            public int min;

            public ResolutionDropdown(int min, int max)
            {
                this.min = min;
                this.max = max;
            }
        }

        public class MinMaxSlider : PropertyAttribute
        {
            public float max;
            public float min;

            public MinMaxSlider(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }

        public class ChannelPicker : PropertyAttribute
        {
        }
    }
}