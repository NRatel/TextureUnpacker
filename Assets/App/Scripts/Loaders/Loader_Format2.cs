using UnityEngine;
using NRatel.TextureUnpacker.PlistParser;

namespace NRatel.TextureUnpacker
{
    public class Loader_Format2 : Loader
    {
        protected override Frame CreateFrame(string textureName, PlistElementDict pd)
        {
            Vector2Int startPos = Util.StringToRectInt(pd["frame"].AsString()).position;
            SizeInt size = new SizeInt(Util.StringToRectInt(pd["sourceColorRect"].AsString()).size);
            SizeInt sourceSize = Util.StringToSizeInt(pd["sourceSize"].AsString());
            bool isRotated = pd["rotated"].AsBoolean();
            Vector2Int offset = Util.StringToVector2Int(pd["offset"].AsString());

            return new Frame(textureName, startPos, size, sourceSize, isRotated, offset);
        }

        protected override Metadata CreateMetadata(PlistElementDict pd)
        {
            int format = pd["format"].AsInteger();
            SizeInt size = Util.StringToSizeInt(pd["size"].AsString());
            string textureFileName = pd["textureFileName"].AsString();

            return new Metadata(format, size, textureFileName);
        }
    }
}
