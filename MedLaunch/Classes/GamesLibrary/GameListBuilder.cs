﻿using MedLaunch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MedLaunch.Classes.GamesLibrary
{
    public class GameListBuilder : INotifyPropertyChanged
    {
        // properties
        private ObservableCollection<DataGridGamesView> filteredSet;
        public ObservableCollection<DataGridGamesView> FilteredSet
        {
            get
            {
                return filteredSet;
            }
            set
            {
                if (filteredSet != value)
                {
                    filteredSet = value;
                    OnPropertyChanged("FilteredSet");

                }
            }
        }


        public List<DataGridGamesView> AllGames { get; set; }
        public int SystemId { get; set; }
        public string SearchString { get; set; }
        public bool UpdateRequired { get; set; }

        // constructors

        public GameListBuilder()
        {
            // instantiate new AllGames object for the first time
            AllGames = new List<DataGridGamesView>();

            // populate the object
            AllGames = GamesLibraryDataGridRefresh.Update(AllGames);
            
            UpdateRequired = false;
        }

        protected void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public static List<DataGridGamesView> Filter(int systemId, string search)
        {
            List<DataGridGamesView> results = new List<DataGridGamesView>();
            App _App = ((App)Application.Current);

            if (_App.GamesList.UpdateRequired == true)
                GamesLibraryDataGridRefresh.Update();

            // system id selector
            switch (systemId)
            {
                case -1:        // favorites
                    results = (from g in _App.GamesList.AllGames
                               where g.Favorite == true
                             select g).ToList();
                    break;
                case 0:         // all games
                    results = _App.GamesList.AllGames;
                    break;
                case -100:      // unscraped games
                    // ignore for now
                    results = _App.GamesList.AllGames;
                    break;
                default:        // based on actual system id
                    results = (from g in _App.GamesList.AllGames
                               where GSystem.GetSystemId(g.System) == systemId
                             select g).ToList();
                    break;
            }

            // now we have results based on the system filter - process file search
            List<DataGridGamesView> srch = DoSearch(results, search);

            _App.GamesList.FilteredSet = new ObservableCollection<DataGridGamesView>(srch);

            return srch;
        }

        public static List<DataGridGamesView> DoSearch(List<DataGridGamesView> list, string sStr)
        {
            // check whether we need to search the gdb columns
            GlobalSettings gs = GlobalSettings.GetGlobals();

            List<DataGridGamesView> search = new List<DataGridGamesView>();

            search = (from g in list
                     where g.Game.ToLower().Contains(sStr.ToLower())
                     select g).ToList();

            return search;
        }

        /// <summary>
        /// Set the update flag so that on the next local operation the data is refreshed from the database
        /// </summary>
        public static void UpdateFlag()
        {
            App _App = ((App)Application.Current);
            _App.GamesList.UpdateRequired = true;
        }
             
      
        

        /*
        public GameListBuilder(int systemId)
        {
            List<Game> games = new List<Game>();
            SearchString = "";
            FilteredSet = new List<DataGridGamesView>();

            // system id selector
            switch (systemId)
            {
                case -1:        // favorites
                    games = (from g in Game.GetGames()
                                       where g.isFavorite == true
                                       select g).ToList();
                    break;
                case 0:         // all games
                    games = Game.GetGames();
                    break;
                case -100:      // unscraped games
                    games = (from g in Game.GetGames()
                             where g.isScraped != true
                             select g).ToList();
                    break;
                default:        // based on actual system id
                    games = (from g in Game.GetGames()
                             where g.systemId == systemId
                             select g).ToList();
                    break;
            }

            // build filtered set
            foreach (var g in games)
            {
                DataGridGamesView dgv = new DataGridGamesView();

                // standard entries from GAME table in database
                dgv.ID = g.gameId;
                dgv.Game = g.gameName;
                dgv.System = GSystem.GetSystemName(g.systemId);
                dgv.Favorite = g.isFavorite;
                string lp;
                if (g.gameLastPlayed.ToString("yyyy-MM-dd HH:mm:ss") == "0001-01-01 00:00:00")
                {
                    lp = "NEVER";
                }
                else
                {
                    lp = g.gameLastPlayed.ToString("yyyy-MM-dd HH:mm:ss");
                }
                dgv.LastPlayed = lp;

                // add looked up entries from scraping link
                var l = GDBLink.GetRecord(dgv.ID);
                if (l != null)
                {
                    if (l.GdbId != null)
                    {
                        int gdbid = l.GdbId.Value;
                        LibraryDataGDBLink lnk = LibraryDataGDBLink.GetLibraryData(gdbid);
                        if (lnk != null)
                        {
                            dgv.Coop = lnk.Coop;
                            dgv.Developer = lnk.Developer;
                            dgv.ESRB = lnk.ESRB;
                            dgv.Players = lnk.Players;
                            dgv.Publisher = lnk.Publisher;
                            dgv.Year = lnk.Year;
                        }
                    }                   
                }                

                // add to filtered list
                FilteredSet.Add(dgv);
            }
        }

        */
        /*
        public List<DataGridGamesView> Search (string SearchString)
        {

        }
        */
    }
}
