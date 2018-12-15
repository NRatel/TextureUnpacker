using UnityEngine;
using NRatel.TextureUnpacker.PlistParser;

namespace NRatel.TextureUnpacker
{
    public class Loader_Format3 : Loader
    {
        protected override Frame CreateFrame(string textureName, PlistElementDict pd)
        {
            Vector2Int startPos = Util.StringToRectInt(pd["textureRect"].AsString()).position;
            SizeInt size = Util.StringToSizeInt(pd["spriteSize"].AsString());   //也可使用textureRect中的数据
            SizeInt sourceSize = Util.StringToSizeInt(pd["spriteSourceSize"].AsString());
            bool isRotated = pd["textureRotated"].AsBoolean();
            Vector2Int offset = Util.StringToVector2Int(pd["spriteOffset"].AsString());

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
