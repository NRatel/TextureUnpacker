using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRatel.TextureUnpacker
{
    public class SizeInt
    {
        public int width;
        public int height;
        public SizeInt(Vector2Int v2i)
        {
            this.width = v2i.x;
            this.height = v2i.y;
        }
    }

    public class Metadata
    {
        public int format;
        public string pixelFormat;
        public bool premultiplyAlpha;
        public string realTextureFileName;
        public SizeInt size;
        public string smartupdate;
        public string textureFileName;

        public Metadata(int format, string pixelFormat, bool premultiplyAlpha, string realTextureFileName, SizeInt size, string smartupdate, string textureFileName)
        {
            this.format = format;
            this.pixelFormat = pixelFormat;
            this.premultiplyAlpha = premultiplyAlpha;
            this.realTextureFileName = realTextureFileName;
            this.size = size;
            this.smartupdate = smartupdate;
            this.textureFileName = textureFileName;
        }
    }

    public class Frame
    {
        public string textureName;
        public List<string> aliases;
        public Vector2Int spriteOffset;
        public SizeInt spriteSize;
        public SizeInt spriteSourceSize;
        public RectInt textureRect;
        public bool textureRotated;

        public Frame(string textureName, List<string> aliases, Vector2Int spriteOffset, SizeInt spriteSize, SizeInt spriteSourceSize, RectInt textureRect, bool textureRotated)
        {
            this.textureName = textureName;
            this.aliases = aliases;
            this.spriteOffset = spriteOffset;
            this.spriteSize = spriteSize;
            this.spriteSourceSize = spriteSourceSize;
            this.textureRect = textureRect;
            this.textureRotated = textureRotated;
        }
    }

    public class Plist
    {
        public string version;
        public Metadata metadata;
        public List<Frame> frames;

        public Plist(string version, Metadata metadata, List<Frame> frames)
        {
            this.metadata = metadata;
            this.frames = frames;
        }
    }
}

