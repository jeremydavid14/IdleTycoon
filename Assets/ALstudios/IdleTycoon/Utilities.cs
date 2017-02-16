/*
 * Utilities
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using System.Collections.Generic;
using System.Linq;

namespace ALStudios.IdleTycoon
{
    
    public static class Utilities
    {

        #region Utilities

        public static IEnumerable<T> GetEnumValues<T>()
        {
            return System.Enum.GetValues(typeof(T)).Cast<T>();
        }

        #endregion
    
    }
    
}