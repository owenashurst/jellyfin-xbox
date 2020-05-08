﻿using System;
using System.Linq;
using Jellyfin.Core;
using Jellyfin.Models.ServiceModels;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON Item to tv show.
    /// </summary>
    public class TvShowAdapter : ItemMediaElementBaseAdapter, IAdapter<TvShowItem, TvShow>
    {
        #region Additional methods

        public TvShow Convert(TvShowItem source)
        {
            TvShow t = new TvShow();
            ConvertBase(source, t);

           if (source.BackdropImageTags != null)
            {
                t.BackdropImageId = source.BackdropImageTags.FirstOrDefault();
            }

            t.PlaybackPosition = new TimeSpan(source.UserData.PlaybackPositionTicks);
            t.IsPlayed = source.UserData.Played;

            return t;
        }

        #endregion
    }
}