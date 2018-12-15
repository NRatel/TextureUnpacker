using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

namespace NRatel.TextureUnpacker
{
    public class TextureUnpacker
    {
        private App app;
        public TextureUnpacker(App app)
        {
            this.app = app;
        }

        //仅从大图中裁剪出小图
        public void JustSplit(Texture2D bigTexture, Frame frame)
        {
            int sampleWidth = frame.size.width;
            int sampleHeight = frame.size.height;
            int destWidth = sampleWidth;
            int destHeight = sampleHeight;
            
            Texture2D destTexture = new Texture2D(destWidth, destHeight);

            //旋转时, 采样宽高调换
            if (frame.isRotated)
            {
                sampleWidth = frame.size.height;
                sampleHeight = frame.size.width;
            }

            //起始位置（Y轴需变换，且受旋转影响）。
            int startPosX = frame.startPos.x;
            int startPosY = bigTexture.height - (frame.startPos.y + sampleHeight);
            
            Color[] colors = bigTexture.GetPixels(startPosX, startPosY, sampleWidth, sampleHeight);

            //设置像素（将采样像素放到目标图中去）
            for (int w = 0; w < destWidth; w++)
            {
                for (int h = 0; h < destHeight; h++)
                {
                    if (frame.isRotated)
                    {
                        //旋转时，目标图中的坐标(w, h),对应的采样区坐标为(h, sampleHeight - w - 1)
                        int index = (sampleHeight - w - 1) * sampleWidth + h;
                        destTexture.SetPixel(w, h, colors[index]);
                    }
                    else
                    {
                        //没有旋转时，目标图中的坐标(w, h),对应的采样区坐标为(w, h)
                        int index = h * sampleWidth + w;
                        destTexture.SetPixel(w, h, colors[index]);
                    }
                }
            }
            destTexture.Apply();
            byte[] bytes = destTexture.EncodeToPNG();
            Save("SplitedPngs", frame.textureName, bytes);
            Texture2D.Destroy(destTexture);
            destTexture = null;
        }

        //从大图中裁剪出小图，并还原到原始大小（恢复其四周被裁剪的透明像素）
        public void Restore(Texture2D bigTexture, Frame frame)
        {
            int sampleWidth = frame.size.width;
            int sampleHeight = frame.size.height;
            int destWidth = frame.sourceSize.width;
            int destHeight = frame.sourceSize.height;

            //换算偏移值（不受旋转影响）
            int offsetLX = frame.offset.x + frame.sourceSize.width / 2 - frame.size.width / 2;
            int offsetBY = -(-frame.offset.y + frame.size.height / 2 - frame.sourceSize.height / 2);
            
            Texture2D destTexture = new Texture2D(destWidth, destHeight);
            
            if (frame.isRotated)
            {
                sampleWidth = frame.size.height;
                sampleHeight = frame.size.width;
            }

            //起始位置（Y轴需变换，且受旋转影响）。
            int startPosX = frame.startPos.x;
            int startPosY = bigTexture.height - (frame.startPos.y + sampleHeight);
            
            Color[] colors = bigTexture.GetPixels(startPosX, startPosY, sampleWidth, sampleHeight);

            //设置像素（将采样像素放到目标图中去）
            for (int w = 0; w < destWidth; w++)
            {
                for (int h = 0; h < destHeight; h++)
                {
                    if (w >= offsetLX && w < frame.size.width + offsetLX && h >= offsetBY && h < frame.size.height + offsetBY)
                    {
                        if (frame.isRotated)
                        {
                            //旋转时，目标图中的坐标(w, h),对应的采样区坐标为(h-offsetTY, sampleHeight-(w-offsetLX)-1)
                            int index = (sampleHeight - (w - offsetLX) - 1) * sampleWidth + (h - offsetBY);
                            destTexture.SetPixel(w, h, colors[index]);
                        }
                        else
                        {
                            //没有旋转时，目标图中的坐标(w, h),对应的采样区坐标为（w-offsetLX, h-offsetTY）
                            int index = (h - offsetBY) * sampleWidth + (w - offsetLX);
                            destTexture.SetPixel(w, h, colors[index]);
                        }
                    }
                    else
                    {
                        //四周颜色（透明）
                        destTexture.SetPixel(w, h, new Color(0, 0, 0, 0));
                    }
                }
            }

            destTexture.Apply();
            byte[] bytes = destTexture.EncodeToPNG();
            Save("RestoredPngs", frame.textureName, bytes);
            Texture2D.Destroy(destTexture);
            destTexture = null;
        }

        private void Save(string subDir, string textureName, byte[] bytes)
        {
            string dir = this.app.GetSaveDir() + @"\" + subDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!textureName.EndsWith(".png"))
            {
                textureName = textureName + ".png";
            }

            FileStream file = File.Open(dir + @"\" + textureName, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
        }
    }
}