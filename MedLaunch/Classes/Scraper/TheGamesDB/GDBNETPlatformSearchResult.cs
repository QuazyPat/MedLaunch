using System;

namespace MedLaunch.Classes.TheGamesDB
{
    /// <summary>
    /// Represents a search result when listing games.
    /// </summary>
    public class GDBNETPlatformSearchResult
    {
        /// <summary>
        /// Unique database ID.
        /// </summary>
        public int ID;

        /// <summary>
        /// Name of the platform.
        /// </summary>
        public String Name;

        /// <summary>
        /// URL alias
        /// </summary>
        public String Alias;
    }
}
