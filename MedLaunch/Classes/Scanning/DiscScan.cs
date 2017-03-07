﻿using MahApps.Metro.Controls.Dialogs;
using MedLaunch.Classes.GamesLibrary;
using MedLaunch.Classes.IO;
using MedLaunch.Classes.Scraper.DAT.Models;
using MedLaunch.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MedLaunch.Classes.Scanning
{
    public class DiscScan : GameScanner
    {
        // Start Disc scan and import process for specific system
        public void BeginDiscImport(int systemId, ProgressDialogController dialog)
        {            
            // get path to ROM folder
            string discFolderPath = GetPath(systemId);
            //MessageBoxResult result2 = MessageBox.Show(romFolderPath);
            // get allowed file types for this particular system
            HashSet<string> exts = GSystem.GetAllowedFileExtensions(systemId);

            // get a list of games for this system currently already in the database
            List<Game> presentGames = (from g in Games
                                       where g.systemId == systemId
                                       select g).ToList();

            /* disc games at the moment MUST reside in 1st level subfolders within the system folder */

            // get all subfolders within the system folder
            List<string> subs = Directory.GetDirectories(discFolderPath).ToList();
            if (subs.Count == 0)
                return;

            int foldersFound = subs.Count;
            int foldersEmpty = 0;
            int gamesFound = 0;
            string strBase = "Scanning: ";

            // iterate through each sub-directory (should be one game in each)
            for (int i = 0; i < subs.Count; i++)
            {
                //string uiUpdate = strBase + "\nGames Found: " + gamesFound;
                //dialog.SetMessage(uiUpdate);

                List<DiscGameFile> game = DetermineDiscFileFromSubFolder(subs[i], systemId);

                // if none found skip
                if (game.Count == 0)
                    continue;

                // if a single sheet file in the List, add this to library
                if (game.Count == 1)
                {
                    InsertOrUpdateDisk(game.First(), systemId);
                    continue;
                }

                // if multiple, create m3u file
                if (game.Count > 0)
                {
                    CreateM3uPlaylist(game.OrderBy(a => a.FileName).ToList(), game.First().FolderPath + "\\" + game.First().GameName + ".m3u", true);
                    // create a new discgamefile for the m3u and add it to library
                    InsertOrUpdateDisk(new DiscGameFile(game.First().FolderPath + "\\" + game.First().GameName, systemId), systemId);
                    continue;
                }

                

            }

            GameListBuilder.UpdateFlag();

        }

        /// <summary>
        /// Manually choose a disc game and import into database/library
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public DiscGameFile SelectGameFile(int sysId)
        {
            // get allowed file types for this system
            List<string> exts = (GSystem.GetAllowedFileExtensions(sysId)).ToList();

            // convert allowed types to filter string
            string filter = "";
            string fStart = "Allowed Types (";
            string fEnd = "";
            foreach (string i in exts)
            {
                if (i == "") { continue; }

                fStart += "*" + i + ",";
                fEnd += "*" + i + ";";
            }
            char comma = ',';
            char semi = ';';
            filter = (fStart.TrimEnd(comma)) + ")|" + (fEnd.TrimEnd(semi));
            //MessageBox.Show(filter);

            // open the file dialog showing only allowed file types - multi-select enabled
            OpenFileDialog filePath = new OpenFileDialog();
            filePath.Multiselect = true;
            filePath.Filter = filter;
            filePath.Title = "Select a single (or multiple in the same directory) .cue or .ccd or .toc file(s)";
            filePath.ShowDialog();

            if (filePath.FileNames.Length > 0)
            {
                // file(s) have been selected
                List<string> files = filePath.FileNames.ToList();

                // single or multiple files?
                if (files.Count > 1)
                {

                    // Create a list of GameFile Objects to process
                    List<DiscGameFile> games = new List<DiscGameFile>();

                    // iterate through each game
                    foreach (string game in files)
                    {
                        // Create a new DiskGameFile instance with all path details
                        DiscGameFile g = new DiscGameFile(game, sysId);
                        // add to list
                        games.Add(g);
                    }

                    // process the list and create an m3u playlist file - all selected files have to be in the same directory
                    List<DiscGameFile> ordered = (from a in games
                                                  select a).OrderBy(a => a.FileName).ToList();

                    // check whether an m3u playlist file already exists
                    var firstEntry = (from a in ordered
                                      select a).First();

                    // create string for the new m3u path
                    string m3uPath = firstEntry.FolderPath + "\\" + firstEntry.GameName + ".m3u";
                    //MessageBox.Show(m3uPath);

                    // create GameFIle object for the m3u playlist
                    DiscGameFile mf = new DiscGameFile(m3uPath, sysId);

                    // Attempt M3U creation
                    bool create = CreateM3uPlaylist(ordered, m3uPath, false);

                    if (create == false)
                    {
                        // user cancelled import, return null
                        return null;
                    }
                    else
                    {
                        // method returned true - begin to import m3u to games library
                        return mf;
                    }
                }
                else
                {
                    // single file selected - create GameFile object and return it
                    DiscGameFile g = new DiscGameFile(files[0], sysId, true);
                    return g;
                }
            }
            else
            {
                // no files selected - return empty string
                return null;
            }
        }

        public void BeginManualImport(int sysId)
        {
            // Start manual import process for a game based on sysId
            DiscGameFile gameFile = SelectGameFile(sysId);
            if (gameFile == null)
            {
                MessageBox.Show("No valid file was selected", "MedLaunch: Error");
                return;
            }
            else
            {
                // Add or update the returned GameFile to the database
                InsertOrUpdateDisk(gameFile, sysId);
                SaveToDatabase();
                MessageBox.Show("Game: " + gameFile.FileName + " has added to (or updated in) the library", "MedLaunch: Import or Update Completed");
                GameListBuilder.UpdateFlag();
            }

        }

        public void InsertOrUpdateDisk(DiscGameFile f, int sysId)
        {
            // check whether game already exists (by gameName and systemId)
            Game chkGame = (from g in Games
                            where g.systemId == sysId && g.gameName == f.GameName
                            select g).SingleOrDefault();

            // create new Game object for import
            Game newGame = new Game();

            string hash = string.Empty;

            // calculate MD5 checksum hashes            
            if (f.Extension.Contains("m3u"))
            {
                // this is an m3u playlist - inspect and get relevant cue files
                string[] lines = File.ReadAllLines(f.FullPath);

                if (lines.Length > 0)
                {
                    // get hash for first cue/toc/ccd
                    hash = Crypto.checkMD5(f.FolderPath + "\\" + lines[0]);
                }
            }
            else
            {
                hash = Crypto.checkMD5(f.FullPath);
            }



            // lookup md5 in master DAT
            List<DATMerge> lookup = (from i in DAT
                                     where i.SystemId == sysId && i.Roms.Any(l => l.MD5.ToUpper().Trim() == hash)
                                     select i).ToList();

            if (chkGame == null)
            {
                // does not already exist - create new game
                newGame.configId = 1;
                newGame.gameName = f.GameName;
                newGame.gamePath = f.FullPath;
                newGame.hidden = false;
                newGame.isDiskBased = true;
                newGame.isFavorite = false;
                newGame.systemId = sysId;

                if (lookup != null && lookup.Count > 0)
                {
                    newGame.gameNameFromDAT = lookup.First().GameName;
                    newGame.Publisher = lookup.First().Publisher;
                    newGame.Year = lookup.First().Year;

                    // get rom we are interested in
                    var rom = (from ro in lookup.First().Roms
                               where ro.MD5.ToUpper().Trim() == hash.ToUpper().Trim()
                               select ro).First();
                    newGame.romNameFromDAT = rom.RomName;
                    newGame.Copyright = rom.Copyright;
                    newGame.Country = rom.Country;
                    newGame.DevelopmentStatus = rom.DevelopmentStatus;
                    newGame.Language = rom.Language;
                    newGame.OtherFlags = rom.OtherFlags;

                    if (rom.Year != null && rom.Year != "")
                    {
                        newGame.Year = rom.Year;
                    }
                    if (rom.Publisher != null && rom.Publisher != "")
                    {
                        newGame.Publisher = rom.Publisher;
                    }

                }

                // add to finaGames list
                DisksToAdd.Add(newGame);
                // increment the added counter
                AddedStats++;
                GameListBuilder.UpdateFlag();
            }
            else
            {
                // matching game found - update it
                if ((chkGame.gamePath == f.FullPath || chkGame.gamePath == f.FullPath) && chkGame.hidden == false)
                {
                    //nothing to update - Path is either identical either absoultely or relatively (against the systemPath set in the database)
                    UntouchedStats++;
                }
                else
                {
                    newGame = chkGame;
                    // update path in case it has changed location
                    newGame.gamePath = f.FullPath;
                    // mark as not hidden
                    newGame.hidden = false;
                    newGame.isDiskBased = true;

                    if (lookup != null && lookup.Count > 0)
                    {
                        newGame.gameNameFromDAT = lookup.First().GameName;
                        newGame.Publisher = lookup.First().Publisher;
                        newGame.Year = lookup.First().Year;

                        // get rom we are interested in
                        var rom = (from ro in lookup.First().Roms
                                   where ro.MD5.ToUpper().Trim() == hash.ToUpper().Trim()
                                   select ro).First();
                        newGame.romNameFromDAT = rom.RomName;
                        newGame.Copyright = rom.Copyright;
                        newGame.Country = rom.Country;
                        newGame.DevelopmentStatus = rom.DevelopmentStatus;
                        newGame.Language = rom.Language;
                        newGame.OtherFlags = rom.OtherFlags;

                        if (rom.Year != null && rom.Year != "")
                        {
                            newGame.Year = rom.Year;
                        }
                        if (rom.Publisher != null && rom.Publisher != "")
                        {
                            newGame.Publisher = rom.Publisher;
                        }

                    }

                    // add to finalGames list
                    DisksToUpdate.Add(newGame);
                    // increment updated counter
                    UpdatedStats++;
                    GameListBuilder.UpdateFlag();
                }

            }
        }

        /// <summary>
        /// Examine a disc game folder and return a DiscGameFile List ( may contain singles or multiples depending on logic)
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static List<DiscGameFile> DetermineDiscFileFromSubFolder(string folderPath, int sysId)
        {
            List<DiscGameFile> list = new List<DiscGameFile>();
            DiscGameFile gdf = new DiscGameFile();
            
            // get all files in the folder (that have extensions we are interested in)
            List<string> cueFiles = ( from a in FileAndFolder.GetFiles(folderPath, false)
                                             where a.ToLower().Contains(".m3u") ||
                                             a.ToLower().Contains(".cue") ||
                                             a.ToLower().Contains(".ccd") ||
                                             a.ToLower().Contains(".toc")
                                             select a).ToList();

            // count file types
            int m3uCount = cueFiles.Where(a => a.ToLower().Contains(".m3u")).ToList().Count();
            int cueCount = cueFiles.Where(a => a.ToLower().Contains(".cue")).ToList().Count();
            int ccdCount = cueFiles.Where(a => a.ToLower().Contains(".ccd")).ToList().Count();
            int tocCount = cueFiles.Where(a => a.ToLower().Contains(".toc")).ToList().Count();

            // check for m3u first and return this (first one) if found
            if (m3uCount > 0)
            {
                string m3uFile = cueFiles.Where(a => a.ToLower().Contains(".m3u")).ToList().First();
                gdf = new DiscGameFile(m3uFile, sysId);
                list.Add(gdf);
                return list;
            }

            // if we have got this far then no m3u was detected - now check for SINGLE cue files (denoting one game)
            if (cueCount == 1 && ccdCount == 0 && tocCount == 0)
            {
                string cueFile = cueFiles.Where(a => a.ToLower().Contains(".cue")).ToList().First();
                gdf = new DiscGameFile(cueFile, sysId);
                list.Add(gdf);
                return list;
            }

            // now check for single ccd
            if (ccdCount == 1 && cueCount == 0 && tocCount == 0)
            {
                string ccdFile = cueFiles.Where(a => a.ToLower().Contains(".ccd")).ToList().First();
                gdf = new DiscGameFile(ccdFile, sysId);
                list.Add(gdf);
                return list;
            }

            // now check for single toc
            if (tocCount == 1 && cueCount == 0 && ccdCount == 0)
            {
                string tocFile = cueFiles.Where(a => a.ToLower().Contains(".toc")).ToList().First();
                list.Add(gdf);
                return list;
            }

            /* done with m3us and single sheet files - now comes multiples logic */

            // get ALL sheet files (not including m3us)
            List<string> sheets = (from a in cueFiles
                                   where !a.ToLower().Contains(".m3u")
                                   select a).ToList();

            // instantiate working set
            List<DiscGameFile> working = new List<DiscGameFile>();

            List<string> disc1 = new List<string>();
            List<string> disc2 = new List<string>();
            List<string> disc3 = new List<string>();
            List<string> disc4 = new List<string>();
            List<string> disc5 = new List<string>();
            List<string> disc6 = new List<string>();                      

            // lookup based on disc number
            for (int sheet = 0; sheet < sheets.Count; sheet++)
            {
                if (sheets[sheet].ToLower().Contains("disc " + (sheet + 1)) ||
                    sheets[sheet].ToLower().Contains("cd " + (sheet + 1)))
                {
                    switch (sheet + 1)
                    {
                        case 1: disc1.Add(sheets[sheet]); break;
                        case 2: disc2.Add(sheets[sheet]); break;
                        case 3: disc3.Add(sheets[sheet]); break;
                        case 4: disc4.Add(sheets[sheet]); break;
                        case 5: disc5.Add(sheets[sheet]); break;
                        case 6: disc6.Add(sheets[sheet]); break;
                    }
                }
            }
            List<List<string>> combined = new List<List<string>>
            {
                disc1, disc2, disc3, disc4, disc5, disc6
            };

            // now loop through the combined object
            for (int disc = 0; disc < 6; disc++)
            {
                // if disc 1 is not present, then dont go any further
                if (combined[0].Count == 0)
                    break;

                // if there is a single disc only per disc list then add it to working
                if (combined[disc].Count == 1)
                    working.Add(new DiscGameFile(combined[disc].First(), sysId));

                // if there are multiple discs per disc list then we have to loop through each of them and check whether they are valid
                if (combined[disc].Count > 1)
                {
                    foreach (string s in combined[disc])
                    {
                        string test = null;
                        switch (Path.GetExtension(s).ToLower())
                        {
                            case ".cue":
                                test = ParseNonM3UTrackSheetString(s, CueType.cue, sysId);
                                break;

                            case ".ccd":
                                test = ParseNonM3UTrackSheetString(s, CueType.ccd, sysId);
                                break;

                            case ".toc":
                                test = ParseNonM3UTrackSheetString(s, CueType.toc, sysId);
                                break;
                        }

                        // continue if nothing returned
                        if (test == null || test == "")
                            continue;

                        // prefer cue
                        if (Path.GetExtension(test).ToLower() == ".cue")
                        {
                            working.Add(new DiscGameFile(s, sysId));
                            break;
                        }
                        // then ccd
                        if (Path.GetExtension(test).ToLower() == ".ccd")
                        {
                            working.Add(new DiscGameFile(s, sysId));
                            break;
                        }
                        // then toc
                        if (Path.GetExtension(test).ToLower() == ".toc")
                        {
                            working.Add(new DiscGameFile(s, sysId));
                            break;
                        }
                    }
                }
            }

            // now - working should contain all disc(s) for this particular game
            return working;

        }

        /// <summary>
        /// checks the contents of a sheet file (m3u, cue, etc) - returns true if files it is pointing to exist
        /// </summary>
        /// <param name="sheetPath"></param>
        /// <returns></returns>
        public static bool IsSheetValid(string sheetPath)
        {
            DiscGameFile dgf = new DiscGameFile();
            switch (Path.GetExtension(sheetPath).ToLower())
            {
                case ".m3u":
                    break;

                case ".cue":
                    break;

                case ".ccd":
                    break;
            }

            return false;
        }

        public static string ParseNonM3UTrackSheetString(string trackSheet, CueType sheetType, int systemId)
        {
            List<DiscGameFile> r = ParseTrackSheet(new DiscGameFile(trackSheet, systemId), sheetType, systemId);
            if (r.Count == 0)
                return null;
            else
            {
                return r.First().FullPath;
            }
        }

        /// <summary>
        /// Takes a cue, m3u, ccd or toc and returns a List<DiscGameFile> object containing all the referenced (or implied) files
        /// </summary>
        /// <param name="trackSheet"></param>
        /// <param name="sheetType"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static List<DiscGameFile> ParseTrackSheet(DiscGameFile trackSheet, CueType sheetType, int systemId)
        {
            List<DiscGameFile> working = new List<DiscGameFile>();

            switch (sheetType)
            {
                case CueType.cue:
                    // get referenced image from cue file
                    string[] cFile = File.ReadAllLines(trackSheet.FullPath);
                    foreach (string l in cFile)
                    {
                        if (l == "" || l == " ")
                            continue;

                        if (l.Contains("FILE") || l.ToLower().Contains("file"))
                        {
                            // this is the line we are interested in
                            string filename = l.Replace("File ", "")
                                .Replace("FILE ", "")
                                .Replace("file ", "")
                                .Replace("BINARY", "")
                                .Replace("Binary", "")
                                .Replace("binary", "")
                                .Trim()
                                .Trim('"');

                            if (File.Exists(trackSheet.FolderPath + "\\" + filename))
                            {
                                DiscGameFile dgfc = new DiscGameFile(trackSheet.FolderPath + "\\" + filename, systemId);
                                working.Add(dgfc);
                            }                            
                            break;
                        }
                    }
                    break;

                case CueType.ccd:
                    // ccd files dont appear to have any reference to the image filename - im going to assume they just have to be named the same
                    string ccdFilePath = trackSheet.FullPath;
                    string imgFilePath = ccdFilePath.ToLower().Replace(".ccd", ".img");
                    
                    // check whether this file actuall exists
                    if (File.Exists(imgFilePath))
                    {
                        DiscGameFile dgfi = new DiscGameFile(imgFilePath, systemId);
                        working.Add(dgfi);
                    }
                    break;

                case CueType.toc:
                    // not implemented at the moment
                    break;

                case CueType.m3u:
                    // get all referenced files from m3u
                    string[] mFiles = File.ReadAllLines(trackSheet.FullPath);
                    foreach (string l in mFiles)
                    {
                        if (l == "" || l == " ")
                            continue;

                        // create a discgamefile
                        DiscGameFile dgf = new DiscGameFile(trackSheet.FolderPath + "\\" + l, systemId);
                        // add it to working list
                        working.Add(dgf);
                        break;
                    }
                    break;
            }

            return working;
        }

        public static bool CreateM3uPlaylist(List<DiscGameFile> files, string m3uPath, bool overwrite)
        {
            // does the file already exist
            if (!File.Exists(m3uPath))
            {
                // file does not exist - create file and populate
                using (StreamWriter sw = File.CreateText(m3uPath))
                {
                    foreach (var f in files)
                    {
                        sw.WriteLine(f.FileName);
                    }
                }
                return true;
            }
            else
            {
                if (overwrite == false)
                {
                    // file already exists - check whether we want to overwrite or not
                    MessageBoxResult result = MessageBox.Show("File Name: " + Path.GetFileName(m3uPath) + "\n\nDo you want to replace this file?\n(Yes overwrites, No uses existing file, Cancel aborts the import process)", "M3U Playlist File Already Exists", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        // dont overwrite, just return
                        return true;
                    }
                    if (result == MessageBoxResult.Cancel)
                    {
                        // cancel import process
                        return false;
                    }
                    if (result == MessageBoxResult.Yes)
                    {
                        // overwrite

                        // clear file first
                        File.Create(m3uPath).Close();

                        // create and populate file
                        using (StreamWriter sw = File.CreateText(m3uPath))
                        {
                            foreach (var f in files)
                            {
                                sw.WriteLine(f.FileName);
                            }
                        }
                        return true;
                    }
                }

                else
                {
                    // clear file first
                    File.Create(m3uPath).Close();

                    // create and populate file
                    using (StreamWriter sw = File.CreateText(m3uPath))
                    {
                        foreach (var f in files)
                        {
                            sw.WriteLine(f.FileName);
                        }
                    }
                    return true;
                }

                return false;
            }
        }
    }

    public enum CueType
    {
        cue,
        ccd,
        toc,
        m3u
    }
}
