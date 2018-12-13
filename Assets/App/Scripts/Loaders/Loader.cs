using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using NRatel.TextureUnpacker.PlistParser;

namespace NRatel.TextureUnpacker
{
    abstract public class Loader
    {
        static public Loader LookingForLoader(string path)
        {
            Loader loader = null;
            PlistDocument pd = new PlistDocument();
            pd.ReadFromFile(path);

            try
            {
                int format = pd.root["metadata"].AsDict()["format"].AsInteger();
                if (format == 2)
                {
                    loader = new Loader_Format2();
                }
                else if (format == 3)
                {
                    loader = new Loader_Format3();
                }
                else
                {
                    Debug.LogWarning("找到format,但是尚未处理");
                }
            }
            catch
            {
                Debug.LogWarning("找不到format");
            }

            return loader;
        }

        //为 format 2+ 的plist加载提供模板方法。
        public virtual Plist LoadPlist(string path)
        {
            PlistDocument pd = new PlistDocument();
            pd.ReadFromFile(path);

            Metadata metadata = CreateMetadata(pd.root["metadata"].AsDict());
            List<Frame> frames = new List<Frame>();
            foreach (var kvPair in pd.root["frames"].AsDict().values)
            {
                frames.Add(CreateFrame(kvPair.Key, kvPair.Value.AsDict()));
            }
            return new Plist(pd.version, metadata, frames);
        }

        protected abstract Metadata CreateMetadata(PlistElementDict pd);

        protected abstract Frame CreateFrame(string textureName, PlistElementDict pd);

        public Texture2D LoadTexture(string path, Metadata metadata)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;

            Texture2D texture = new Texture2D(metadata.size.width, metadata.size.height);
            texture.LoadImage(bytes);
            return texture;
        }
    }

    static public class Util
    {
        static public Vector2Int StringToVector2Int(string s)
        {
            s = s.Replace(" ", "");
            s = s.Substring(1, s.Length - 2);
            string[] v2is = s.Split(',');
            return new Vector2Int(int.Parse(v2is[0]), int.Parse(v2is[1]));
        }

        static public SizeInt StringToSizeInt(string s)
        {
            return new SizeInt(StringToVector2Int(s));
        }

        static public RectInt StringToRectInt(string s)
        {
            s = s.Replace(" ", "");
            s = s.Substring(1, s.Length - 2);
            string[] ri = Regex.Split(s, "},{");
            return new RectInt(StringToVector2Int(ri[0] + "}"), StringToVector2Int("{" + ri[1]));
        }
    }
}
