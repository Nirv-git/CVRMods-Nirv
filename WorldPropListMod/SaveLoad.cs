﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WorldPropListMod
{
    class SaveLoad
    {
        public static string blockedPropsPath = "UserData/WorldPropsList/blockedProps.txt";
        public static string propNameCachePath = "UserData/WorldPropsList/cachedProps.txt";
        public static string playerNamesCachePath = "UserData/WorldPropsList/cachedUsers.txt";

        public static string dateFormatOut = "yyyy'-'MM'-'dd";
        public static string dateFormatIn = "yyyy-MM-dd";

        public static void InitFileListOrLoad()
        {
            if (!Directory.Exists("UserData/WorldPropsList")) Directory.CreateDirectory("UserData/WorldPropsList");
            //Blocked Props
            {
                var path = blockedPropsPath;
                if (!File.Exists(path))
                {
                    try { File.WriteAllText(path, "", Encoding.UTF8);
                    } catch (Exception ex) { Main.Logger.Error("Error creating initial blocked props file \n" + ex.ToString()); }
                }
                else
                {
                    try { Main.blockedProps = new Dictionary<string, string>(File.ReadAllLines(path).Select(s => s.Split(new char[] { '␟' }, StringSplitOptions.RemoveEmptyEntries))
                        .Where(x => x.Length == 2).ToDictionary(p => p[0].Trim(), p => p[1].Trim()));
                    } catch (Exception ex) { Main.Logger.Error("Error reading prop blocklist \n" + ex.ToString()); }
                }
            }
            //Prop Cache
            {
                var path = propNameCachePath;
                if (!File.Exists(path))
                {
                    try { File.WriteAllText(path, "", Encoding.UTF8); }
                    catch (Exception ex) { Main.Logger.Error("Error creating initial props cache file \n" + ex.ToString()); }
                }
                else
                {
                    try { Main.PropNamesCache = new Dictionary<string, (string, string, string, bool, string, string, string, DateTime)>(File.ReadAllLines(path).Select(s => s.Split(new char[] { '␟' }, StringSplitOptions.RemoveEmptyEntries))
                        .Where(x => x.Length == 9).Where(x => Utils.ParseDate(x[8].Trim(), dateFormatIn) >= DateTime.Now.AddDays(-7))
                        .ToDictionary(p => p[0].Trim(), p => (p[1].Trim(), p[2].Trim(), p[3].Trim(), Boolean.Parse(p[4].Trim()), p[5].Trim(), p[6].Trim(), p[7].Trim(), Utils.ParseDate(p[8].Trim(), dateFormatIn) )));
                    } catch (Exception ex) { Main.Logger.Error("Error reading prop cache file \n" + ex.ToString()); }
                }
            }
            //User Cache
            {
                var path = playerNamesCachePath;
                if (!File.Exists(path))
                {
                    try { File.WriteAllText(path, "", Encoding.UTF8); }
                    catch (Exception ex) { Main.Logger.Error("Error creating initial player cache file \n" + ex.ToString()); }
                }
                else
                {
                    try { Main.PlayerNamesCache = new Dictionary<string, (string, DateTime)>(File.ReadAllLines(path).Select(s => s.Split(new char[] { '␟' }, StringSplitOptions.RemoveEmptyEntries))
                        .Where(x => x.Length == 3).Where(x => Utils.ParseDate(x[2].Trim(), dateFormatIn) >= DateTime.Now.AddDays(-7))
                        .ToDictionary(p => p[0].Trim(), p => (p[1].Trim(), Utils.ParseDate(p[2].Trim(), dateFormatIn))));
                    } catch (Exception ex) { Main.Logger.Error("Error reading player cache file \n" + ex.ToString()); }
                }
            }
        }

        public static void SaveListFiles()
        {
            try { File.WriteAllLines(blockedPropsPath, Main.blockedProps.Select(p => string.Format("{0}␟ {1}", p.Key, p.Value)), Encoding.UTF8); 
            } catch (Exception ex) { Main.Logger.Error("Error writing prop blocklist \n" + ex.ToString()); }

            try { File.WriteAllLines(propNameCachePath, Main.PropNamesCache.Select(p => string.Format("{0}␟ {1}␟ {2}␟ {3}␟ {4}␟ {5}␟ {6}␟ {7}␟ {8}", p.Key, p.Value.Item1, p.Value.Item2, p.Value.Item3, p.Value.Item4,
                p.Value.Item5, p.Value.Item6, p.Value.Item7, p.Value.Item8.ToString(dateFormatOut))), Encoding.UTF8); 
            } catch (Exception ex) { Main.Logger.Error("Error writing prop cache file \n" + ex.ToString()); }

            try { File.WriteAllLines(playerNamesCachePath, Main.PlayerNamesCache.Select(p => string.Format("{0}␟ {1}␟ {2}", p.Key, p.Value.Item1, p.Value.Item2.ToString(dateFormatOut))), Encoding.UTF8); 
            } catch (Exception ex) { Main.Logger.Error("Error writing prop cache file \n" + ex.ToString()); }
        }


       
    }
}
