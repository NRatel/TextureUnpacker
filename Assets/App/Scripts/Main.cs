using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRatel.TextureUnpacker
{
    public class Main : MonoBehaviour
    {
        void Start()
        {
            Screen.SetResolution(1000, 1000, false);

            string plistFilePath = @"C:\Users\Administrator\Desktop\p\res_bundle.plist";
            string pngFilePath = @"C:\Users\Administrator\Desktop\p\res_bundle.png";

            Loader loader = new Loader();
            Plist plist = loader.LoadPlist(plistFilePath);
            Texture2D texture2D = loader.LoadTexture(pngFilePath, plist.metadata);

            new ImageSpliter().Split(texture2D, plist.frames, true);
        }
    }

}

