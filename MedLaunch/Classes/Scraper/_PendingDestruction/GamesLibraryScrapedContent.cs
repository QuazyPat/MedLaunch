﻿namespace MedLaunch.Classes
{
    /*
    // class for scraped sidebar data - instantiated once on main page then passed around (loads masterscraper data from json file once)
    public class GamesLibraryScrapedContent
    {
        // properties
        public string BaseContentDirectory { get; set; }
        public List<ScraperMaster> MasterPlatformList { get; set; }

        // constructor
        public GamesLibraryScrapedContent()
        {
            // set base content dir
            BaseContentDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Data\Games";

            // load the master json file
            string masterPath = AppDomain.CurrentDomain.BaseDirectory + @"Data\System\MasterGames.json";
            string json = File.ReadAllText(masterPath);
            MasterPlatformList = JsonConvert.DeserializeObject<List<ScraperMaster>>(json);

            // ensure initial directory structure is created
            Directory.CreateDirectory(BaseContentDirectory);
        }

       

        public void SaveJson(ScrapedGameObjectWeb o)
        {
            string gPath = AppDomain.CurrentDomain.BaseDirectory + @"Data\Games\" + o.GdbId.ToString() + @"\" + o.GdbId.ToString() + ".json";
            string json = JsonConvert.SerializeObject(o, Formatting.Indented);
            File.WriteAllText(gPath, json);
        }

        // looks up and returns scrapeddataobject based on Internal GameId (not gamesdb id)
        public ScrapedGameObject GetScrapedGameObject(int GameId, int GdbId)
        {
            //Game link = Game.GetGame(GameId);

            // we have a link record - proceed and generate object
            ScrapedGameObject sgo = new ScrapedGameObject();
            if (GdbId < 1)
                return null;
            sgo.GdbId = GdbId;

            // attempt to load game data json
            string gPath = AppDomain.CurrentDomain.BaseDirectory + @"Data\Games\" + sgo.GdbId.ToString() + @"\" + sgo.GdbId.ToString() + ".json";
            
            if (File.Exists(gPath))
            {
                ScrapedGameObjectWeb sgoweb = new ScrapedGameObjectWeb();
                ScrapedGameData sg = new ScrapedGameData();
                string jsonString = File.ReadAllText(gPath);
                try
                {
                    sgoweb = JsonConvert.DeserializeObject<ScrapedGameObjectWeb>(jsonString);
                }
                catch (Exception e)
                {
                    // there was a problem with the file - do nothing
                }
                finally
                {
                    sgo.Data = sgoweb.Data;
                }                
            }
            else { sgo.Data = new ScrapedGameData(); }

            // populate lists in object
            string baseGameDir = AppDomain.CurrentDomain.BaseDirectory + @"Data\Games\" + sgo.GdbId.ToString();

            sgo.BackCovers = GetAllFolderFiles(baseGameDir + @"\BackCover");
            sgo.Banners = GetAllFolderFiles(baseGameDir + @"\Banners");
            sgo.FanArts = GetAllFolderFiles(baseGameDir + @"\FanArt");
            sgo.FrontCovers = GetAllFolderFiles(baseGameDir + @"\FrontCover");
            sgo.Manuals = GetAllFolderFiles(baseGameDir + @"\Manual");
            sgo.Medias = GetAllFolderFiles(baseGameDir + @"\Media");
            sgo.Screenshots = GetAllFolderFiles(baseGameDir + @"\Screenshots");

            // return object
            return sgo;
        }

        public static List<string> GetAllFolderFiles(string folderPath)
        {
            List<string> list = new List<string>();

            // check folder exists
            if (!Directory.Exists(folderPath))
                return list;

            // enumerate all files in folder (non-recursive)
            string[] fileEntries = Directory.GetFiles(folderPath);
            foreach (string s in fileEntries) { list.Add(s); }
            return list;
        }
        

        public void CreateFolderStructure(int gdbId)
        {
            string basePath = BaseContentDirectory + @"\" + gdbId.ToString() + @"\";
            // boxart
            System.IO.Directory.CreateDirectory(basePath + "FrontCover");
            System.IO.Directory.CreateDirectory(basePath + "BackCover");
            System.IO.Directory.CreateDirectory(basePath + "Media");
            System.IO.Directory.CreateDirectory(basePath + "Banners");
            System.IO.Directory.CreateDirectory(basePath + "Screenshots");
            System.IO.Directory.CreateDirectory(basePath + "PromoArt");
            System.IO.Directory.CreateDirectory(basePath + "FanArt");
            System.IO.Directory.CreateDirectory(basePath + "Manual");
        }

        public void ReloadMasterObject()
        {
            // reload the master json file
            string masterPath = AppDomain.CurrentDomain.BaseDirectory + @"Data\System\MasterGames.json";
            string json = File.ReadAllText(masterPath);
            MasterPlatformList = JsonConvert.DeserializeObject<List<ScraperMaster>>(json);
        }
    }
    */
}
