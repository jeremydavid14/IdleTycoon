/*
 * HexDirection
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

namespace ALStudios.IdleTycoon
{
    
    public enum HexDirection
    {

        #region HexDirection

        NORTH_EAST,
        EAST,
        SOUTH_EAST,
        SOUTH_WEST,
        WEST,
        NORTH_WEST

        #endregion

    }

    public static class HexDirectionExtensions
    {

        #region HexDirectionExtensions

        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int) direction < 3 ? (direction + 3) : (direction - 3);
        }

        public static HexDirection Previous(this HexDirection direction)
        {
            return direction == HexDirection.NORTH_EAST ? HexDirection.NORTH_WEST : direction - 1;
        }

        public static HexDirection Next(this HexDirection direction)
        {
            return direction == HexDirection.NORTH_WEST ? HexDirection.NORTH_EAST : direction + 1;
        }

        #endregion

    }
    
}