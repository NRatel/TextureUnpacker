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
                //SplitWithOffset(srcTexture, frame);
                SplitWithoutOffset(srcTexture, frame);
            }
        }

        private void SplitWithoutOffset(Texture2D srcTexture, Frame frame)
        {
            //小图大小。
            int width = frame.spriteSize.width;
            int height = frame.spriteSize.height;

            //实际在大图中展示的大小
            int actualWidth = width;
            int actualHeight = height;

            //创建一张图
            Texture2D destTexture = new Texture2D(width, height);

            //旋转时, 实际展示的宽高调换
            if (frame.textureRotated)
            {
                actualWidth = height;
                actualHeight = width;
            }

            //起始位置。注意，plist中坐标的Y轴方向和GetPixels()的Y轴方向相反、起点相反（左上和左下），需要转换。
            int startPosX = frame.textureRect.x;
            int startPosY = srcTexture.height - (frame.textureRect.y + actualHeight);

            //采集像素
            Color[] colors = srcTexture.GetPixels(startPosX, startPosY, actualWidth, actualHeight);

            //设置像素
            for (int w = 0; w < actualWidth; w++)
            {
                for (int h = 0; h < actualHeight; h++)
                {
                    if (frame.textureRotated)
                    {
                        //逆时针旋转90度赋值
                        destTexture.SetPixel(actualHeight - 1 - h, w, colors[h * actualWidth + w]);
                    }
                    else
                    {
                        destTexture.SetPixel(w, h, colors[h * actualWidth + w]);
                    }    
                }
            }

            destTexture.Apply();

            //保存打扫
            byte[] bytes = destTexture.EncodeToPNG();
            Save(frame.textureName, bytes);
            Texture2D.DestroyImmediate(destTexture);
            destTexture = null;
        }

        private void SplitWithOffset(Texture2D srcTexture, Frame frame)
        {
            if (!frame.textureRotated)
            {
                //大图中小图的实际有效大小。
                int validWidth = frame.spriteSize.width;
                int validHeight = frame.spriteSize.height;
                //将要还原的小图的目标大小。
                int destWidth = frame.spriteSourceSize.width;
                int destHeight = frame.spriteSourceSize.height;
                //起始位置。注意，plist中坐标的Y轴方向和GetPixels()的Y轴方向相反、起点相反（左上和左下），需要转换。
                int startPosX = frame.textureRect.x;
                int startPosY = srcTexture.height - (frame.textureRect.y + validHeight);
                //偏移值(按中点偏移)，（可以画图理解）
                int offsetX = destWidth / 2 + frame.spriteOffset.x - validWidth / 2;
                int offsetY = destHeight / 2 - frame.spriteOffset.y - validHeight / 2;

                //创建一张透明底图
                Texture2D destTexture = new Texture2D(destWidth, destHeight);
                for (int w = 0; w < destWidth; w++)
                {
                    for (int h = 0; h < destHeight; h++)
                    {
                        destTexture.SetPixel(w, h, new Color(0, 0, 0, 0));
                    }
                }

                Color[] colors = srcTexture.GetPixels(startPosX, startPosY, validWidth, validHeight);
                destTexture.SetPixels(offsetX, offsetY, validWidth, validHeight, colors);
                destTexture.Apply();

                byte[] bytes = destTexture.EncodeToPNG();
                Save(frame.textureName, bytes);
                Texture2D.DestroyImmediate(destTexture);
                destTexture = null;
            }
            else
            {
                //int validWidth = frame.spriteSize.width;
                //int validHeight = frame.spriteSize.height;
                //int destWidth = frame.spriteSourceSize.width;
                //int destHeight = frame.spriteSourceSize.height;

                //int offsetX = destWidth / 2 + frame.spriteOffset.x - validWidth / 2;
                //int offsetY = destHeight / 2 - frame.spriteOffset.y - validHeight / 2;

                //int temp = validWidth;
                //validWidth = validHeight;
                //validHeight = temp;

                //temp = destWidth;
                //destWidth = destHeight;
                //destHeight = temp;

                //temp = offsetX;
                //offsetX = offsetY;
                //offsetY = temp;

                //int startPosX = frame.textureRect.x;
                //int startPosY = srcTexture.height - (frame.textureRect.y + validHeight);

                //Texture2D destTexture = new Texture2D(destWidth, destHeight);
                //Color[] colors = srcTexture.GetPixels(startPosX, startPosY, validWidth, validHeight);
                //destTexture.SetPixels(offsetX, offsetY, validWidth, validHeight, colors);
                //destTexture.Apply();

                //byte[] bytes = destTexture.EncodeToPNG();
                //Save(frame.textureName, bytes);
                //Texture2D.DestroyImmediate(destTexture);
                //destTexture = null;
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