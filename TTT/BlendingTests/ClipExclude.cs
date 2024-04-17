#if UNITY_EDITOR
using UnityEngine;
using static Unity.Mathematics.math;

namespace net.rs64.TTTDebuggingPlayground.TTT
{

    internal class ClipExclude : BlendingReversEngrailingBase
    {
        public override float Render(float x, float y)
        {
            return lerp(min(x, y), 1 - min(x, y), max(x, y));
        }
    }
}
#endif
