using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using System.Collections.Generic;

namespace NearClipPlaneAdj
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "0001", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.n0001.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "001", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.n001.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "01", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.n01.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "05", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.n05.png"));

        }

        public static void InitUi()
        {
            loadAssets();

            var cat = QuickMenuAPI.MiscTabPage.AddCategory("Nearclip Plane Adjust", "NearClipPlaneAdj");
            
            var clipList = new float[] {
                .05f,
                .01f,
                .001f,
                .0001f
            };

            foreach (var clip in clipList)
            {
                var butt = cat.AddButton($"{clip}", clip.ToString().Replace("0.",""), $"Sets Nearclipping plane to {clip}");
                butt.OnPress += () =>
                {
                    Main.ChangeNearClipPlane(clip, true);
                };
            }
        }
    }
}