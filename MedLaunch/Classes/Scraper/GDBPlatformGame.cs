namespace MedLaunch.Models
{
    public class GDBPlatformGame
    { 
        public int id { get; set; }
        public int SystemId { get; set; }
        public string GDBPlatformName { get; set; }
        //public int GameId { get; set; }
        public string GameTitle { get; set; }
        public string ReleaseDate { get; set; }
    }
}
