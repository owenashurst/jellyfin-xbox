﻿using System;

namespace Jellyfin.Models
{
    /// <summary>
    /// Movie model representation
    /// </summary>
    [Obsolete]
    public class MovieDetail : Movie
    {
        #region Properties



        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name}";
        }

        #endregion
    }
}