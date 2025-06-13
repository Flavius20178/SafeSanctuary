using System;
using UnityEngine;

namespace sc.terrain.proceduralpainter
{
    [Serializable]
    public class Height : Modifier
    {
        public float min;
        [Min(0.001f)] public float minFalloff = 1;

        public float max = 2000;
        [Min(0.001f)] public float maxFalloff = 1;

        public void OnEnable()
        {
            passIndex = FilterPass.Height;
        }

        public override void Configure(Material material)
        {
            base.Configure(material);

            material.SetVector("_MinMaxHeight", new Vector4(min, max, minFalloff, maxFalloff));
        }
    }
}