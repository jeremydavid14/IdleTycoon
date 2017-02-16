/*
 * HexMetrics
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

// http://catlikecoding.com/unity/tutorials/hex-map/part-6/

using UnityEngine;

namespace ALStudios.IdleTycoon
{
    
    public static class HexMetrics
    {

        #region Variables

        public const int CHUNK_SIZE_X = 5;
        public const int CHUNK_SIZE_Z = 5;

        public const float ELEVATION_STEP = 3F;

        public const int TERRACES_PER_SLOPE = 2;
        public const int TERRACE_STEPS = TERRACES_PER_SLOPE * 2 + 1;

        public const float VERTICAL_TERRACE_STEPS = 1F / (TERRACES_PER_SLOPE + 1);
        public const float HORIZONTAL_TERRACE_STEPS = 1F / TERRACE_STEPS;

        public const float OUTER_RADIUS = 10F;
        public const float INNER_RADIUS = OUTER_RADIUS * 0.866025404F;

        public const float SOLID_FACTOR = 0.8F;
        public const float BLEND_FACTOR = 1F - SOLID_FACTOR;

        public const float NOISE_SCALE = 0.003F;
        public const float CELL_PERTURB_STRENGTH = 4F;
        public const float ELEVATION_PERTURB_STRENGTH = 1.5F;

        public static Texture2D NoiseSource;

        private static readonly Vector3[] Corners =
        {
            new Vector3(0F, 0F, OUTER_RADIUS),
            new Vector3(INNER_RADIUS, 0F, 0.5F * OUTER_RADIUS),
            new Vector3(INNER_RADIUS, 0F, -0.5F * OUTER_RADIUS),
            new Vector3(0F, 0F, -OUTER_RADIUS),
            new Vector3(-INNER_RADIUS, 0F, -0.5F * OUTER_RADIUS),
            new Vector3(-INNER_RADIUS, 0F, 0.5F * OUTER_RADIUS),
            new Vector3(0F, 0F, OUTER_RADIUS)
        };

        #endregion

        #region HexMetrics

        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return Corners[(int) direction];
        }

        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return GetFirstCorner(direction) * SOLID_FACTOR;
        }

        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return Corners[(int) direction + 1];
        }

        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return GetSecondCorner(direction) * SOLID_FACTOR;
        }

        public static Vector3 GetBridge(HexDirection direction)
        {
            return (GetFirstCorner(direction) + GetSecondCorner(direction)) * BLEND_FACTOR;
        }

        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
                return HexEdgeType.FLAT;

            int delta = elevation2 - elevation1;

            return Mathf.Abs(delta) == 1 ? HexEdgeType.SLOPE : HexEdgeType.CLIFF;
        }

        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            float h = step * HORIZONTAL_TERRACE_STEPS;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;

            float v = ((step + 1) / 2) * VERTICAL_TERRACE_STEPS;
            a.y += (b.y - a.y) * v;

            return a;
        }

        public static Color TerraceLerp(Color a, Color b, int step)
        {
            return Color.Lerp(a, b, step * HORIZONTAL_TERRACE_STEPS);
        }

        public static Vector4 SampleNoise(Vector3 position)
        {
            return NoiseSource.GetPixelBilinear(position.x * NOISE_SCALE, position.z * NOISE_SCALE);
        }

        #endregion

    }
    
}