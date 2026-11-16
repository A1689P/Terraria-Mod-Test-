using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyMod.MyProjectiles
{
    /// <summary>
    /// 记录龙头走过的轨迹点，体节按「沿轨迹的弧长」取样，
    /// 这样每节都精确落在龙头经过的位置上，间距固定，转弯也不会切角。
    /// </summary>
    public static class DragonTrail
    {
        private const int MaxPoints = 400;
        private static readonly Dictionary<int, List<Vector2>> Trails = new();

        public static void Reset(int owner)
        {
            if (Trails.TryGetValue(owner, out var list))
                list.Clear();
        }

        public static void Push(int owner, Vector2 center)
        {
            if (!Trails.TryGetValue(owner, out var list))
                Trails[owner] = list = new List<Vector2>();

            // 只记录真正移动过的点，避免原地不动时堆一堆重复点
            if (list.Count == 0 || (list[0] - center).LengthSquared() > 0.25f)
            {
                list.Insert(0, center);
                if (list.Count > MaxPoints)
                    list.RemoveAt(list.Count - 1);
            }
        }

        /// <summary>
        /// 从最新点往回走 distance 像素，插值出该处的坐标。
        /// 轨迹长度不足时返回 false。
        /// </summary>
        public static bool Sample(int owner, float distance, out Vector2 result)
        {
            result = default;
            if (!Trails.TryGetValue(owner, out var list) || list.Count < 2)
                return false;

            float acc = 0f;
            for (int i = 0; i < list.Count - 1; i++)
            {
                float seg = Vector2.Distance(list[i], list[i + 1]);
                if (seg < 0.0001f)
                    continue;

                if (acc + seg >= distance)
                {
                    float t = (distance - acc) / seg;
                    result = Vector2.Lerp(list[i], list[i + 1], t);
                    return true;
                }

                acc += seg;
            }

            return false;
        }
    }
}
