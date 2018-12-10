using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRatel.PlistParser;
using System.Text.RegularExpressions;
using System.IO;

namespace NRatel.TextureUnpacker
{
    public class Loader
    {
        public Plist LoadPlist(string path)
        {
            PlistDocument pd = new PlistDocument();
            pd.ReadFromFile(path);

            string version = pd.version;
            PlistElementDict root = pd.root;

            Metadata metadata = CreateMetadata(root["metadata"].AsDict());
            List<Frame> frames = new List<Frame>();
            foreach (var kvPair in root["frames"].AsDict().values)
            {
                frames.Add(CreateFrame(kvPair.Key, kvPair.Value.AsDict()));
            }

            return new Plist(version, metadata, frames);
        }

        public Texture2D LoadTexture(string path, Metadata metadata)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;

            Texture2D texture2D = new Texture2D(metadata.size.width, metadata.size.height);
            texture2D.LoadImage(bytes);
            return texture2D;
        }

        private Frame CreateFrame(string textureName, PlistElementDict pd)
        {
            List<string> aliases = new List<string>();
            foreach (var a in pd["aliases"].AsArray().values)
            {
                aliases.Add(a.AsString());
            }

            Vector2Int spriteOffset = Util.StringToVector2Int(pd["spriteOffset"].AsString());
            SizeInt spriteSize = Util.StringToSizeInt(pd["spriteSize"].AsString());
            SizeInt spriteSourceSize = Util.StringToSizeInt(pd["spriteSourceSize"].AsString());
            RectInt textureRect = Util.StringToRectInt(pd["textureRect"].AsString());
            bool textureRotated = pd["textureRotated"].AsBoolean();

            return new Frame(textureName, aliases, spriteOffset, spriteSize, spriteSourceSize, textureRect, textureRotated);
        }

        private Metadata CreateMetadata(PlistElementDict pd)
        {
            int format = pd["format"].AsInteger();
            string pixelFormat = pd["pixelFormat"].AsString();
            bool premultiplyAlpha = pd["premultiplyAlpha"].AsBoolean();
            string realTextureFileName = pd["realTextureFileName"].AsString();
            SizeInt size = Util.StringToSizeInt(pd["size"].AsString());
            string smartupdate = pd["smartupdate"].AsString();
            string textureFileName = pd["textureFileName"].AsString();

            return new Metadata(format, pixelFormat, premultiplyAlpha, realTextureFileName, size, smartupdate, textureFileName);
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
