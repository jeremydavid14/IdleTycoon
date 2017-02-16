/*
 * HexCoordinates
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using UnityEngine;

namespace ALStudios.IdleTycoon
{

    [System.Serializable]
    public struct HexCoordinates
    {

        #region Variables

        [SerializeField] private int _x;
        [SerializeField] private int _z;

        public int X
        {
            get { return _x; }
            private set { _x = value; }
        }

        public int Z
        {
            get { return _z; }
            private set { _z = value; }
        }

        public int Y
        {
            get { return 0 - X - Z; }
        }

        #endregion

        #region Constructors

        public HexCoordinates(int x, int z)
        {
            _x = x;
            _z = z;
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + + Z + ")";
        }

        #endregion

        #region HexCoordinates

        public string ToStringDivided()
        {
            return X + System.Environment.NewLine + Y + System.Environment.NewLine + Z;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        public static HexCoordinates FromPosition(Vector3 position)
        {
            float x = position.x / (HexMetrics.INNER_RADIUS * 2F);
            float y = -x;

            float offset = position.z / (HexMetrics.OUTER_RADIUS * 3F);
            x -= offset;
            y -= offset;

            int ix = Mathf.RoundToInt(x);
            int iy = Mathf.RoundToInt(y);
            int iz = Mathf.RoundToInt(0 - x - y);

            if (ix + iy + iz == 0)
                return new HexCoordinates(ix, iz);

            float dx = Mathf.Abs(x - ix);
            float dy = Mathf.Abs(y - iy);
            float dz = Mathf.Abs(0 - x - y - iz);

            if (dx > dy && dx > dz)
                ix = 0 - iy - iz;
            else if (dz > dy)
                iz = 0 - ix - iy;

            return new HexCoordinates(ix, iz);
        }

        #endregion

    }
    
}