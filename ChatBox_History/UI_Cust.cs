using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.ChatBox;
using ABI_RC.Systems.UI.UILib;
using ABI_RC.Systems.UI.UILib.UIObjects;
using ABI_RC.Systems.UI.UILib.UIObjects.Components;
using MelonLoader;
using Mono.WebBrowser.DOM;
using Semver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ChatBox_History
{
    public class CustomUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("ChatBox_History.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "CBhist_history", Assembly.GetExecutingAssembly().GetManifestResourceStream("ChatBox_History.Icons.ChatBox_History.png"));
            QuickMenuAPI.PrepareIcon(ModName, "CBhist_Delete", Assembly.GetExecutingAssembly().GetManifestResourceStream("ChatBox_History.Icons.Delete.png"));
            QuickMenuAPI.PrepareIcon(ModName, "CBhist_Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("ChatBox_History.Icons.Reset.png"));
        }

        public static string ModName = "NirvBTKUI";

        public static Category mainCat;
        public static Page pageChatHistRoot;
        public static Dictionary<GameObject, Vector3> wingDict = new Dictionary<GameObject, Vector3>();

        public static bool movePrec = true;

        public static string CBhist_Page, lastQMPage;

        public static void InitUi()
        {
            
            if (!Main.useNirvMiscPage.Value)
            {
                ModName = "chatbox_HistoryMod";
            }

            loadAssets();

            var page = pageChatHistRoot;
            if (Main.useNirvMiscPage.Value)
            {
                Page pageNirv = Page.GetOrCreatePage(ModName, "Nirv Misc Page", true, "NirvMisc");
                pageNirv.MenuTitle = "Nirv Misc Page";
                pageNirv.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";

                var catNirv = pageNirv.AddCategory("ChatBox History");
                page = new Page(ModName, "ChatBox History", false);

                QuickMenuAPI.AddRootPage(page);
                catNirv.AddButton("Open ChatBox History", "CBhist_history", "Opens the page for ChatBox History").OnPress += () =>
                {
                    page.OpenPage();
                };
            }
            else
            {
                page = new Page(ModName, "ChatBox History", true, "CBhist_history");
                page.MenuTitle= "ChatBox History";
                page.MenuSubtitle = "Displays all ChatBox messages you have received this session";
            }
            CBhist_Page = page.ElementID;
            pageChatHistRoot = page;

            //GenerateHistoryPage();

            QuickMenuAPI.OnOpenedPage += OnPageChange;
            QuickMenuAPI.OnBackAction += OnPageChange;
        }

        public static void OnPageChange(string targetPage, string lastPage)
        {
            lastQMPage = targetPage;
            GenerateHistoryPage();
        }
        public static void QMtoggle(bool __0)
        {
            if (__0)
            {
                GenerateHistoryPage(); ;
            }
        }

        private static void GenerateHistoryPage()
        {
            if (lastQMPage == CBhist_Page)
            {
                var page = pageChatHistRoot;
                page.ClearChildren();


                var titleCat = page.AddCategory("ChatBox History Settings");
                titleCat.AddButton($"Refresh", "CBhist_Reset", $"Refresh page").OnPress += () =>
                {
                    GenerateHistoryPage();
                };
                titleCat.AddButton($"Clear History", "CBhist_Delete", $"Clear History").OnPress += () =>
                {
                    Main.chatboxMessages.Clear();
                    GenerateHistoryPage();
                };
                //Main.Logger.Msg(Main.chatboxMessages.Count);
                if (Main.chatboxMessages.Count > 0)
                {
                    var msgCatTitle = page.AddCategory("Messages - Name | Time | Distance", true, false);
                    foreach (var msg in Main.chatboxMessages.Reverse<(ChatBoxAPI.ChatBoxMessage, string, string, string)>())
                    {//Message, Username, Time, Distance
                        //var msgCat = page.AddCategory($"{msg.Item2} | {msg.Item3} | {msg.Item4}m", true, false);
                        //msgCat.AddTextBlock(msg.Item1.Message);
                        msgCatTitle.AddTextBlock($"-- {msg.Item2} | {msg.Item3} | {msg.Item4}m");
                        msgCatTitle.AddTextBlock(SplitNewLines(msg.Item1.Message));
                        msgCatTitle.AddTextBlock("--------");
                    }
                }
                else
                {
                    var msgCatTitle = page.AddCategory("--- No Messages ---", true, false);
                }
            } 
        }

        private static string SplitNewLines(string text)
        { //https://stackoverflow.com/a/5573207 - Starting
            int lineLength = 40;

            string[] words = text.Split(' ');
            var sb = new StringBuilder();
            int currLength = 0;

            foreach (string raw in words)
            {
                if (string.IsNullOrEmpty(raw))
                    continue;
                string word = raw;

                if (currLength > 0) //If at start of line
                {
                    int remaining = lineLength - currLength;

                    if (1 + word.Length > remaining) // Does word fit in line, or make newline?
                    {
                        sb.Append(Environment.NewLine);
                        currLength = 0;
                    }
                    else
                    {
                        sb.Append(' ');
                        currLength += 1;
                    }
                }

                // Split long words
                while (word.Length > lineLength)
                {
                    int take = lineLength - 1;
                    sb.Append(word, 0, take)
                      .Append('-')
                      .Append(Environment.NewLine);
                    word = word.Substring(take);
                    currLength = 0;
                }

                sb.Append(word);
                currLength += word.Length;
            }

            return sb.ToString();
        }

    }
}