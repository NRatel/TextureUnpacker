using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

namespace NRatel.TextureUnpacker
{
    public class ImageSpliter
    {
        public void Split(Texture2D srcTexture, List<Frame> frames, bool isUseOffset)
        {
            foreach (var frame in frames)
            {
                if (isUseOffset)
                {
                    SplitWithOffset(srcTexture, frame);
                }
                else
                {
                    SplitWithoutOffset(srcTexture, frame);
                }
            }
        }

        private void SplitWithOffset(Texture2D srcTexture, Frame frame)
        {
            if (!frame.textureRotated)
            {
                return;
            }

            //小图宽高。
            int width = frame.spriteSize.width;
            int height = frame.spriteSize.height;

            //小图原始的宽高，也是考虑offset时要被还原的目标宽高。
            int destWidth = frame.spriteSourceSize.width;
            int destHeight = frame.spriteSourceSize.height;

            //按目标宽高创建一张目标底图（透明）
            Texture2D destTexture = new Texture2D(destWidth, destHeight);
            for (int w = 0; w < destWidth; w++)
            {
                for (int h = 0; h < destHeight; h++)
                {
                    destTexture.SetPixel(w, h, new Color(0, 0, 0, 0));
                }
            }

            //采样宽高 (在大图中展示的宽高)
            int sampleWidth = width;
            int sampleHeight = height;

            int aaaXXX = destWidth;
            int aaaYYY = destHeight;

            //旋转时, 采样宽高调换
            if (frame.textureRotated)
            {
                sampleWidth = height;
                sampleHeight = width;
                aaaXXX = destHeight;
                aaaYYY = destWidth;
            }

            //换算起始位置。注意，plist中坐标的Y轴方向和GetPixels()的Y轴方向相反、起点相反（左上和左下），需要转换。
            int startPosX = frame.textureRect.x;
            int startPosY = srcTexture.height - (frame.textureRect.y + sampleHeight);

            //采集像素
            Color[] colors = srcTexture.GetPixels(startPosX, startPosY, sampleWidth, sampleHeight);

            //换算偏移值(将“从左上开始、按图片裁剪前后的中点计算的偏移值”，转为“从左下开始、按左下边界计算的偏移值”)，（因为SetPixel是从左下开始）
            int offsetX = aaaXXX / 2 + frame.spriteOffset.x - sampleWidth / 2;
            int offsetY = aaaYYY / 2 - frame.spriteOffset.y - sampleHeight / 2;

            //设置像素（将采样像素放到目标图中去）
            for (int w = 0; w < sampleWidth; w++)
            {
                for (int h = 0; h < sampleHeight; h++)
                {
                    if (frame.textureRotated)
                    {
                        //逆时针旋转90度赋值
                        destTexture.SetPixel(aaaYYY - 1 - h + offsetX, w + offsetY, colors[h * sampleWidth + w]);
                    }
                    else
                    {
                        destTexture.SetPixel(w + offsetX, h + offsetY, colors[h * sampleWidth + w]);
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

        private void SplitWithoutOffset(Texture2D srcTexture, Frame frame)
        {
            //小图宽高。也是忽略offset时要被还原的目标宽高。
            int destWidth = frame.spriteSize.width;
            int destHeight = frame.spriteSize.height;

            //按目标宽高创建一张目标底图
            Texture2D destTexture = new Texture2D(destWidth, destHeight);

            //采样宽高 (在大图中展示的宽高)
            int sampleWidth = destWidth;
            int sampleHeight = destHeight;

            //旋转时, 采样宽高调换
            if (frame.textureRotated)
            {
                sampleWidth = destHeight;
                sampleHeight = destWidth;
            }

            //换算起始位置。注意，plist中坐标的Y轴方向和GetPixels()的Y轴方向相反、起点相反（左上和左下），需要转换。
            int startPosX = frame.textureRect.x;
            int startPosY = srcTexture.height - (frame.textureRect.y + sampleHeight);

            //采集像素
            Color[] colors = srcTexture.GetPixels(startPosX, startPosY, sampleWidth, sampleHeight);

            //设置像素（将采样像素放到目标图中去）
            for (int w = 0; w < sampleWidth; w++)
            {
                for (int h = 0; h < sampleHeight; h++)
                {
                    if (frame.textureRotated)
                    {
                        //逆时针旋转90度赋值
                        destTexture.SetPixel(sampleHeight - 1 - h, w, colors[h * sampleWidth + w]);
                    }
                    else
                    {
                        destTexture.SetPixel(w, h, colors[h * sampleWidth + w]);
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