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

        public void SplitWithOffset(Texture2D bigTexture, Frame frame)
        {
            //TODO
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