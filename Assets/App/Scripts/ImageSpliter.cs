using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

namespace NRatel.TextureUnpacker
{
    public class ImageSpliter
    {
        Main main;
        public ImageSpliter(Main main)
        {
            this.main = main;
        }

        public void SplitWithoutOffset(Texture2D bigTexture, Frame frame)
        {
            //小图宽高。也是忽略offset时要被还原的目标宽高。
            int destWidth = frame.size.width;
            int destHeight = frame.size.height;

            //按目标宽高创建一张目标底图
            Texture2D destTexture = new Texture2D(destWidth, destHeight);

            //采样宽高 (在大图中展示的宽高)
            int sampleWidth = destWidth;
            int sampleHeight = destHeight;

            //旋转时, 采样宽高调换
            if (frame.isRotated)
            {
                sampleWidth = destHeight;
                sampleHeight = destWidth;
            }

            //换算起始位置。注意，plist中坐标的Y轴方向和GetPixels()的Y轴方向相反、起点相反（左上和左下），需要转换。
            int startPosX = frame.startPos.x;
            int startPosY = bigTexture.height - (frame.startPos.y + sampleHeight);

            //采集像素
            Color[] colors = bigTexture.GetPixels(startPosX, startPosY, sampleWidth, sampleHeight);

            //设置像素（将采样像素放到目标图中去）
            for (int w = 0; w < sampleWidth; w++)
            {
                for (int h = 0; h < sampleHeight; h++)
                {
                    if (frame.isRotated)
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

        public void SplitWithOffset(Texture2D bigTexture, Frame frame)
        {
            if (frame.isRotated)
            {
                return;
            }

            //小图宽高。
            int width = frame.size.width;
            int height = frame.size.height;

            //小图原始的宽高，也是考虑offset时要被还原的目标宽高。
            int destWidth = frame.sourceSize.width;
            int destHeight = frame.sourceSize.height;

            //按目标宽高创建一张目标底图（透明）
            Texture2D destTexture = new Texture2D(destWidth, destHeight);
            //for (int w = 0; w < destWidth; w++)
            //{
            //    for (int h = 0; h < destHeight; h++)
            //    {
            //        destTexture.SetPixel(w, h, new Color(0, 0, 0, 0));
            //    }
            //}

            //采样宽高 (在大图中展示的宽高)
            int sampleWidth = width;
            int sampleHeight = height;

            //换算起始位置。注意，plist中坐标的Y轴方向和GetPixels()的Y轴方向相反、起点相反（左上和左下），需要转换。
            int startPosX = frame.startPos.x;
            int startPosY = bigTexture.height - (frame.startPos.y + sampleHeight);

            //采集像素
            Color[] colors = bigTexture.GetPixels(startPosX, startPosY, sampleWidth, sampleHeight);

            //换算偏移值(将“从左上开始、按图片裁剪前后的中点计算的偏移值”，转为“从左下开始、按左下边界计算的偏移值”)，（因为SetPixel是从左下开始）
            int offsetX = destWidth / 2 - sampleWidth / 2 - frame.offset.x;
            int offsetY = destHeight / 2 - sampleHeight / 2 - frame.offset.y;

            Debug.Log("offsetX: " + offsetX);
            Debug.Log("offsetY: " + offsetY);

            //设置像素（将采样像素放到目标图中去）
            destTexture.SetPixels(offsetX, offsetY, sampleWidth, sampleHeight, colors);

            destTexture.Apply();
            //保存打扫
            byte[] bytes = destTexture.EncodeToPNG();
            Save(frame.textureName, bytes);
            Texture2D.DestroyImmediate(destTexture);
            destTexture = null;
        }
        
        private void Save(string textureName, byte[] bytes)
        {
            string dir = this.main.GetSaveDir();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            FileStream file = File.Open(dir + "/" + textureName, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
        }
    }
}