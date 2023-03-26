using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WorldPropListMod
{
    class SaveLoad
    {
        //guid,name - Per line
        public static Dictionary<string, string> blockedProps;
        public static string blockedPropsPath = "UserData/blockedProps.txt";

        public static void InitFileListOrLoad()
        {
            if (!File.Exists(blockedPropsPath)) 
            {
                try
                {
                    File.WriteAllText(blockedPropsPath, "", Encoding.UTF8);
                } catch (Exception ex) { Main.Logger.Error("Error creating initial blocked props file \n" + ex.ToString()); }
            }
            else
            {
                try
                {
                    blockedProps = new Dictionary<string, string>(File.ReadAllLines(blockedPropsPath).Select(s => s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Where(x => x.Length == 2).ToDictionary(p => p[0].Trim(), p => p[1].Trim()));
                } catch (Exception ex) { Main.Logger.Error("Error reading prop blocklist \n" + ex.ToString()); }
            }
        }

        public static void SaveListFiles()
        {
            try
            {
                File.WriteAllLines(blockedPropsPath, blockedProps.Select(p => string.Format("{0}, {1}", p.Key, p.Value)), Encoding.UTF8);
                
            }
            catch (Exception ex) { Main.Logger.Error("Error writing prop blocklist \n" + ex.ToString()); }
        }
    }
}
