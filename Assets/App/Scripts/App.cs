using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRatel.Win32;
using System.IO;

namespace NRatel.TextureUnpacker
{
    public class App
    {
        private enum UnpackMode
        {
            JustSplit = 0,
            Restore = 1,
            All = 2
        }
        private UnpackMode currentUnpackMode;

        private Main main;
        private AppUI appUI;
        private string plistFilePath = "";
        private string pngFilePath = "";
        private bool isExecuting = false;
        private Loader loader;
        private Plist plist;
        private Texture2D bigTexture;
        private Core core;

        public App(Main main)
        {
            this.main = main;
            appUI = main.GetComponent<AppUI>().Init();
            currentUnpackMode = (UnpackMode)appUI.m_Dropdown_SelectMode.value;

            //编辑器下测试用
#if UNITY_EDITOR
            plistFilePath = @"C:\Users\Administrator\Desktop\image\res_bundle.plist";
            pngFilePath = @"C:\Users\Administrator\Desktop\image\res_bundle.png";
            main.StartCoroutine(LoadFiles());
#endif

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            main.GetComponent<FilesOrFolderDragInto>().AddEventListener((List<string> aPathNames) =>
            {
                if (isExecuting)
                {
                    appUI.SetTip("正在执行\n请等待结束");
                    return;
                }

                if (aPathNames.Count > 1)
                {
                    appUI.SetTip("只可拖入一个文件");
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
                            appUI.SetTip("不存在与当前plist文件同名的png文件");
                            return;
                        }
                    }
                    else if (path.EndsWith(".png"))
                    {
                        pngFilePath = path;
                        plistFilePath = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + ".plist";
                        if (!File.Exists(plistFilePath))
                        {
                            appUI.SetTip("不存在与当前png文件同名的plist文件");
                            return;
                        }
                    }
                    else
                    {
                        appUI.SetTip("请放入 plist或png 文件");
                        return;
                    }

                    main.StartCoroutine(LoadFiles());
                }
            });

            appUI.m_Btn_Excute.onClick.AddListener(() =>
            {
                if (isExecuting)
                {
                    appUI.SetTip("正在执行\n请等待结束");
                    return;
                }

                if (loader == null || plist == null)
                {
                    appUI.SetTip("没有指定可执行的plist&png");
                    return;
                }

                isExecuting = true;
                core = new Core(this);

                main.StartCoroutine(Unpack());
            });

            appUI.m_Dropdown_SelectMode.onValueChanged.AddListener((value) =>
            {
                currentUnpackMode = (UnpackMode)value;
            });
        }

        private IEnumerator LoadFiles()
        {
            try
            {
                loader = Loader.LookingForLoader(plistFilePath);
                if (loader != null)
                {
                    plist = loader.LoadPlist(plistFilePath);
                    bigTexture = loader.LoadTexture(pngFilePath, plist.metadata);
                    appUI.SetImage(bigTexture);
                    appUI.SetTip("名称: " + plist.metadata.textureFileName + "\n类型: format_" + plist.metadata.format + "\n大小: " + plist.metadata.size.width + "*" + plist.metadata.size.height, false);
                }
                else
                {
                    appUI.SetTip("无法识别的plist类型!!!\n请联系作者");
                }
            }
            catch
            {
                appUI.SetTip("出错了!!!\n请联系作者\n↓");
            }
            yield return null;
        }

        private IEnumerator Unpack()
        {
            try
            {
                int total = plist.frames.Count;
                int count = 0;
                foreach (var frame in plist.frames)
                {
                    if (currentUnpackMode == UnpackMode.JustSplit)
                    {
                        core.JustSplit(bigTexture, frame);
                    }
                    else if (currentUnpackMode == UnpackMode.Restore)
                    {
                        core.Restore(bigTexture, frame);
                    }
                    else if (currentUnpackMode == UnpackMode.All)
                    {
                        core.JustSplit(bigTexture, frame);
                        core.Restore(bigTexture, frame);
                    }
                    count += 1;
                    appUI.SetTip("进度：" + count + "/" + total + (count >= total ? "\n已完成！" : ""), false);
                }
                isExecuting = false;
            }
            catch
            {
                appUI.SetTip("出错了!!!\n请联系作者\n↓");
            }
            yield return null;
        }

        public string GetSaveDir()
        {
            string s = Path.GetFileNameWithoutExtension(plistFilePath);
            foreach (char invalidChar in Path.GetInvalidPathChars())
            {
                s = s.Replace(invalidChar, '_');
            }
            return Path.GetDirectoryName(plistFilePath) + @"\NRatel_" + s;
        }
    }
}

