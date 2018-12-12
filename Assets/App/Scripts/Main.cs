using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRatel.Win32;
using System.IO;
using UnityEngine.UI;

namespace NRatel.TextureUnpacker
{
    public class Main : MonoBehaviour
    {
        static public Main main;
        public Image m_Image_BigImage;
        public Text m_Text_Tip;
        public Button m_Btn_Excute;

        public string plistFilePath = "";
        public string pngFilePath = "";

        private Loader loader;
        private Plist plist;
        private Texture2D bigTexture;

        void Start()
        {
            main = this;
            Screen.SetResolution(800, 600, false);

#if UNITY_EDITOR
            //测试用
            plistFilePath = @"C:\Users\Administrator\Desktop\p\s.plist";
            pngFilePath = @"C:\Users\Administrator\Desktop\p\s.png";
            LoadInputFiles();
#endif

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            this.GetComponent<FilesOrFolderDragInto>().AddEventListener((List<string> aPathNames) =>
            {
                if (aPathNames.Count > 1)
                {
                    Main.SetTip("只可拖入一个文件");
                    return;
                }
                else
                {
                    string path = aPathNames[0];
                    if (path.EndsWith(".plist"))
                    {
                        plistFilePath = path;
                        pngFilePath = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + ".png";
                        if (!File.Exists(pngFilePath))
                        {
                            Main.SetTip("不存在与当前plist文件同名的png文件");
                            return;
                        }
                    }
                    else if (path.EndsWith(".png"))
                    {
                        pngFilePath = path;
                        plistFilePath = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + ".plist";
                        if (!File.Exists(plistFilePath))
                        {
                            Main.SetTip("不存在与当前png文件同名的plist文件");
                            return;
                        }
                    }
                    else
                    {
                        Main.SetTip("请放入plist 或 png 文件");
                        return;
                    }
                    LoadInputFiles();
                }
            });

            this.m_Btn_Excute.onClick.AddListener(() =>
            {
                if (loader == null || plist == null)
                {
                    Main.SetTip("没有指定可执行的plist&png");
                    return;
                }
                StartCoroutine(new ImageSpliter().Split(bigTexture, plist.frames, false));
            });
        }

        private void LoadInputFiles()
        {
            this.loader = Loader.LookingForLoader(plistFilePath);
            this.plist = loader.LoadPlist(plistFilePath);
            this.bigTexture = loader.LoadTexture(pngFilePath, plist.metadata);
        }

        static public void SetTip(string str, bool isError = true)
        {
            string prefix = "<color=red>";
            string postfix = "</color>";
            if (isError == false)
            {
                prefix = "<color=green>";
            }
            main.m_Text_Tip.text = prefix + str + postfix;
        }

        static public void SetImage(Texture2D texture)
        {
            //放到协程里，避免图大时卡顿
            main.StartCoroutine(main.LoadImage(texture));
        }

        public IEnumerator LoadImage(Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            this.m_Image_BigImage.sprite = sprite;

            float width, height;
            float aspectRatio = 1.0f * texture.width / texture.height;

            int max = Mathf.Max(texture.width, texture.height);
            max = max < 600 ? max : 600;

            if (aspectRatio == 1)
            {
                width = max;
                height = max;
            }
            else if (aspectRatio > 1)
            {
                width = max;
                height = width / aspectRatio;
            }
            else
            {
                height = max;
                width = height * aspectRatio;
            }

            this.m_Image_BigImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            yield return null;
        }
    }
}

