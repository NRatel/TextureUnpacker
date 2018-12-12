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
        public SizeInt size;
        public string textureFileName;

        public Metadata(int format, SizeInt size, string textureFileName)
        {
            this.format = format;
            this.size = size;
            this.textureFileName = textureFileName;
        }
    }

    public class Frame
    {
        public string textureName;
        public Vector2Int startPos;
        public SizeInt size;
        public SizeInt sourceSize;
        public bool isRotated;
        public Vector2Int offset;

        public Frame(string textureName, Vector2Int startPos, SizeInt size, SizeInt sourceSize, bool isRotated, Vector2Int offset)
        {
            this.textureName = textureName;
            this.startPos = startPos;
            this.size = size;
            this.sourceSize = sourceSize;
            this.isRotated = isRotated;
            this.offset = offset;
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

