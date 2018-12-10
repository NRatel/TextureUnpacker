using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

namespace NRatel.TextureUnpacker
{
    public class ImageSpliter
    {
        public void Split(Texture2D srcTexture, List<Frame> frames)
        {
            foreach (var frame in frames)
            {
                Split(srcTexture, frame);
            }
        }

        private void Split(Texture2D srcTexture, Frame frame)
        {
            if (!frame.textureRotated)
            {
                //int validWidth = frame.spriteSize.width;
                //int validHeight = frame.spriteSize.height;
                //int destWidth = frame.spriteSourceSize.width;
                //int destHeight = frame.spriteSourceSize.height;
                //int startPosX = frame.textureRect.x;
                //int startPosY = srcTexture.height - (frame.textureRect.y + validHeight);

                //int offsetX = destWidth / 2 + frame.spriteOffset.x - validWidth / 2;
                //int offsetY = destHeight / 2 - frame.spriteOffset.y - validHeight / 2;

                //Debug.Log(frame.textureName + ", " + offsetX);

                //Texture2D destTexture = new Texture2D(destWidth, destHeight);
                //Color[] colors = srcTexture.GetPixels(startPosX, startPosY, validWidth, validHeight);
                //destTexture.SetPixels(offsetX, 0, validWidth, validHeight, colors);

                //byte[] bytes = destTexture.EncodeToPNG();
                //Save(frame.textureName, bytes);
                //Texture2D.DestroyImmediate(destTexture);
                //destTexture = null;
            }
            else
            {
                int validWidth = frame.spriteSize.width;
                int validHeight = frame.spriteSize.height;
                int destWidth = frame.spriteSourceSize.width;
                int destHeight = frame.spriteSourceSize.height;

                int offsetX = destWidth / 2 + frame.spriteOffset.x - validWidth / 2;
                int offsetY = destHeight / 2 - frame.spriteOffset.y - validHeight / 2;

                int temp = validWidth;
                validWidth = validHeight;
                validHeight = temp;

                temp = destWidth;
                destWidth = destHeight;
                destHeight = temp;

                temp = offsetX;
                offsetX = offsetY;
                offsetY = temp;

                int startPosX = frame.textureRect.x;
                int startPosY = srcTexture.height - (frame.textureRect.y + validHeight);

                Texture2D destTexture = new Texture2D(destWidth, destHeight);
                Color[] colors = srcTexture.GetPixels(startPosX, startPosY, validWidth, validHeight);
                destTexture.SetPixels(offsetX, offsetY, validWidth, validHeight, colors);

                byte[] bytes = destTexture.EncodeToPNG();
                Save(frame.textureName, bytes);
                Texture2D.DestroyImmediate(destTexture);
                destTexture = null;
            }
        }

        private void Save(string textureName, byte[] bytes)
        {
            string folder = @"C:\Users\Administrator\Desktop\p\NRatelOutput";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            FileStream file = File.Open(folder + "/" + textureName, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
        }
    }
}