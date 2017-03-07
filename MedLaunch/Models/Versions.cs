﻿using MedLaunch.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MedLaunch.Models
{
    public class Versions
    {
        public int versionId { get; set; }
        public string dbVersion { get; set; }
        public string CurrentMednafenVersion { get; set; }

        // constructors
        public Versions()
        {
            
        }
        /*
        // return compatible mednafen version branch
        public static string CompatibleMednafenBranch()
        {
            string branch = "0.9.43.x";
            return branch;
        }
        */

        /*
         *  Mednafen version compatibility 
         */

        public static List<MednafenChangeHistory> GetMednafenCompatibilityMatrix()
        {
            return
                new List<MednafenChangeHistory>
                {
                    // 0.9.43
                    new MednafenChangeHistory
                    {
                        Version = "0.9.43",
                        Changes = new List<VersionChange>
                        {

                        }
                    },

                    // 0.9.42
                    new MednafenChangeHistory
                    {
                        Version = "0.9.42",
                        Changes = new List<VersionChange>
                        {
                            new VersionChange { Description = "ss multitap 1", ChangeMethod = ChangeType.ToRemove, Item = "ss.input.sport1.multitap" },
                            new VersionChange { Description = "ss multitap 2", ChangeMethod = ChangeType.ToRemove, Item = "ss.input.sport2.multitap" },
                        }
                    },

                    // 0.9.41
                    new MednafenChangeHistory
                    {
                        Version = "0.9.41",
                        Changes = new List<VersionChange>
                        {
                            new VersionChange { Description = "snes_faust multitap 1", ChangeMethod = ChangeType.ToRemove, Item = "snes_faust.input.sport1.multitap" },
                            new VersionChange { Description = "snes_faust multitap 2", ChangeMethod = ChangeType.ToRemove, Item = "snes_faust.input.sport2.multitap" },
                        }
                    },

                    // 0.9.39
                    new MednafenChangeHistory
                    {
                        Version = "0.9.39",
                        Changes = new List<VersionChange>
                        {
                            new VersionChange { Description = "rename pixshader", ChangeMethod = ChangeType.ToRename, Item = ".shader", ChangeItem = ".pixshader" },
                            new VersionChange { Description = "goat shader", ChangeMethod = ChangeType.ToRemoveCompletely, Item = "goat" },
                            new VersionChange { Description = "analogmode CT compare", ChangeMethod = ChangeType.ToRemove, Item = "psx.input.analog_mode_ct.compare" },
                            new VersionChange { Description = "analogmode CT", ChangeMethod = ChangeType.ToRemove, Item = "psx.input.analog_mode_ct" },
                            new VersionChange { Description = "snes.h_blend", ChangeMethod = ChangeType.ToRemove, Item = "snes.h_blend" },
                            new VersionChange { Description = "ss.h_blend", ChangeMethod = ChangeType.ToRemove, Item = "ss.h_blend" },
                            new VersionChange { Description = "ss.h_overscan", ChangeMethod = ChangeType.ToRemove, Item = "ss.h_overscan" },
                            new VersionChange { Description = "ss.correct_aspect", ChangeMethod = ChangeType.ToRemove, Item = "ss.correct_aspect" },
                            new VersionChange { Description = "sms.slstart", ChangeMethod = ChangeType.ToRemove, Item = "sms.slstart" },
                            new VersionChange { Description = "sms.slend", ChangeMethod = ChangeType.ToRemove, Item = "sms.slend" },
                            new VersionChange { Description = "sms.slstartp", ChangeMethod = ChangeType.ToRemove, Item = "sms.slstartp" },
                            new VersionChange { Description = "sms.slendp", ChangeMethod = ChangeType.ToRemove, Item = "sms.slendp" },
                            new VersionChange { Description = "sms.slstart", ChangeMethod = ChangeType.ToRemove, Item = "sms.slstart" },
                        }
                    }
                };

        }

        public static string GetCompatLaunchString(string launchParams)
        {
            Versions VC = new Versions();
            string working = launchParams;

            bool isVersionValid = Versions.MednafenVersionCheck(false);
            if (isVersionValid == false)
            {
                // skip processing
                return working;
            }

            // iterate through version changes
            foreach (MednafenChangeHistory c in GetMednafenCompatibilityMatrix())
            {
                // process changes
                foreach (var change in c.Changes)
                {
                    StringBuilder sb = new StringBuilder();
                    switch (change.ChangeMethod)
                    {
                        case ChangeType.ToRemove:               // explicitly remove the entire command
                            string[] arr = working.Split('-');
                            foreach (string s in arr)
                            {
                                if (!s.Contains(change.Item))
                                    sb.Append("-" + s);
                            }
                            working = sb.ToString();
                            break;

                        case ChangeType.ToRemoveCompletely:
                            string[] arr2 = working.Split('-');
                            foreach (string s in arr2)
                            {
                                if (!s.Contains(change.Item))
                                    sb.Append("-" + s);
                            }
                            working = sb.ToString();
                            break;

                        case ChangeType.ToRename:
                            string[] arr3 = working.Split('-');
                            foreach (string s in arr3)
                            {
                                if (!s.Contains(change.Item))
                                    sb.Append("-" + s);
                                else
                                {
                                    sb.Append("-" + s.Replace(change.Item, change.ChangeItem));
                                }
                            }
                            working = sb.ToString();
                            break;

                        case ChangeType.ToAdd:
                            // currently not used
                            break;
                    }
                }

                working = working.TrimStart('-');

                if (VC.CurrentMednafenVersion == c.Version)
                {
                    // we have reached the targeted version and all transformations should have been applied
                    break;
                }
            }
            return working;
        }

        /*
         *      Misc Version methods
         */ 

        // compare mednafen versions
        /*
        public static bool IsMednafenVersionValid()
        {
            string localbranch = LogParser.GetMednafenVersion();
            string requiredbranch = Versions.CompatibleMednafenBranch();

            string[] lb = localbranch.Trim().Split('.');
            string[] rb = requiredbranch.Trim().Split('.');

            bool isValid = true;
            for (int i = 0; i < 3; i++)
            {
                if (lb[i] != rb[i])
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }
        */

        // get application version
        public static string ReturnApplicationVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string versionMajor = fvi.ProductMajorPart.ToString();
            string versionMinor = fvi.ProductMinorPart.ToString();
            string versionBuild = fvi.ProductBuildPart.ToString();
            string versionPrivate = fvi.ProductPrivatePart.ToString();

            string fVersion = fvi.FileVersion;
            return versionMajor + "." + versionMinor + "." + versionBuild + "." + versionPrivate;
        }

        // get defaults (for initial seed)
        public static Versions GetVersionDefaults()
        {
            Versions v = new Versions
            {
                versionId = 1,
                dbVersion = ReturnApplicationVersion()
            };
            return v;
        }

        // get the database version
        public static string GetVersionString()
        {
            string vStr = GetVersions().dbVersion;
            return vStr;
        }

        // return Versions entry from database
        public static Versions GetVersions()
        {
            Versions v = new Versions();
            using (var context = new MyDbContext())
            {
                var query = from s in context.Versions
                            where s.versionId == 1
                            select s;
                v = query.FirstOrDefault();
            }
            return v;
        }        

        public static bool MednafenVersionCheck(bool showDialog)
        {
            // mednafen version check     
            Paths pa = Paths.GetPaths();
            string medFolderPath = pa.mednafenExe;
            string medPath = medFolderPath + @"\mednafen.exe";

            if (!File.Exists(medPath))
            {
                if (showDialog)
                    MessageBox.Show("Path to Mednafen is NOT valid\nPlease set this on the Settings tab", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string version = LogParser.GetMednafenVersion();

            if (version == null || version == "")
            {
                if (showDialog)
                    MessageBox.Show("There was a problem retreiving the Mednafen version.\nPlease check your paths", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string[] compArrMax = GetMednafenCompatibilityMatrix().First().Version.Split('.');
            string[] compArrMin = GetMednafenCompatibilityMatrix().Last().Version.Split('.');
            string[] versionArr = version.Split('.');

            // mednafen version we are targeting MUST be within the MIN and MAX mednafen versions supported
            for (int i = 0; i < 3; i++)
            {
                // convert to ints
                int compMin = Convert.ToInt32(compArrMin[i]);
                int compMax = Convert.ToInt32(compArrMax[i]);
                int medVer = Convert.ToInt32(versionArr[i]);

                if (medVer < compMin || medVer > compMax)
                {
                    if (showDialog)
                    {
                        // version doesnt match
                        StringBuilder sb = new StringBuilder();
                        sb.Append("The version of Mednafen you are trying to launch is potentially NOT compatible with this version of MedLaunch\n\nMednafen version installed: ");
                        sb.Append(version);
                        sb.Append("\nMednafen version required: ");
                        sb.Append(GetMednafenCompatibilityMatrix().Last().Version + " - " + GetMednafenCompatibilityMatrix().First().Version);
                        sb.Append("\n\nPlease ensure you are targeting a MedLaunch supported version of Mednafen.");
                        sb.Append("\n\nPress OK to return to the Games Library");
                        sb.Append("\nPress CANCEL to ignore these very important messages and try to launch the game (which probably will NOT work anyway)");

                        MessageBoxResult result = MessageBox.Show(sb.ToString(), "Mednafen Version Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                        if (result == MessageBoxResult.OK)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                        
                }
            }

            // if we have gotten this far, the versions seem to match - return true
            return true;
            
        }
        
    }

    public class MednafenChangeHistory
    {
        public string Version { get; set; }
        public List<VersionChange> Changes { get; set; }
    }

    public class VersionChange
    {
        public string Description { get; set; }
        public ChangeType ChangeMethod { get; set; }
        public string Item { get; set; }
        public string ChangeItem { get; set; }
    }

    public enum ChangeType
    {
        ToRename,               // rename a specific string
        ToRemove,               // remove an explicit command line option
        ToRemoveCompletely,     // remove entire option where string is matched
        ToAdd                   // add in a command that was previous removed (not currently needed)
    }
}
