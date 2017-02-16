/*
 * HexEdgeVertices
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using UnityEngine;

namespace ALStudios.IdleTycoon
{
    
    public struct HexEdgeVertices
    {

        #region Variables

        public Vector3 V1;
        public Vector3 V2;
        public Vector3 V3;
        public Vector3 V4;

        #endregion

        #region Constructors

        public HexEdgeVertices(Vector3 corner1, Vector3 corner2)
        {
            V1 = corner1;
            V2 = Vector3.Lerp(corner1, corner2, 1F / 3F);
            V3 = Vector3.Lerp(corner1, corner2, 2F / 3F);
            V4 = corner2;
        }

        #endregion

        #region HexEdgeVertices

        public static HexEdgeVertices TerraceLerp(HexEdgeVertices a, HexEdgeVertices b, int step)
        {
            HexEdgeVertices result;
            result.V1 = HexMetrics.TerraceLerp(a.V1, b.V1, step);
            result.V2 = HexMetrics.TerraceLerp(a.V2, b.V2, step);
            result.V3 = HexMetrics.TerraceLerp(a.V3, b.V3, step);
            result.V4 = HexMetrics.TerraceLerp(a.V4, b.V4, step);

            return result;
        }

        #endregion

    }
    
}