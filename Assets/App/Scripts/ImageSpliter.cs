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

        //仅从大图中裁剪出小图
        public void JustSplit(Texture2D bigTexture, Frame frame)
        {
            //小图采样宽高 (在大图中展示的宽高)
            int sampleWidth = frame.size.width;
            int sampleHeight = frame.size.height;

            //要裁剪出来的目标宽高。
            int destWidth = sampleWidth;
            int destHeight = sampleHeight;

            //按目标宽高创建一张目标底图
            Texture2D destTexture = new Texture2D(destWidth, destHeight);

            //旋转时, 采样宽高调换
            if (frame.isRotated)
            {
                sampleWidth = frame.size.height;
                sampleHeight = frame.size.width;
            }

            //换算起始位置。注意，plist中坐标的Y轴方向和GetPixels()的Y轴方向相反、起点相反（左上和左下），需要转换。
            int startPosX = frame.startPos.x;
            int startPosY = bigTexture.height - (frame.startPos.y + sampleHeight);

            //采集像素
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

            //保存打扫
            byte[] bytes = destTexture.EncodeToPNG();
            Save("JustSplit", frame.textureName, bytes);
            Texture2D.DestroyImmediate(destTexture);
            destTexture = null;
        }

        //从大图中裁剪出小图，并还原到原始大小（恢复其四周被裁剪的透明像素）
        public void Restore(Texture2D bigTexture, Frame frame)
        {
            //小图采样宽高 (在大图中展示的宽高)
            int sampleWidth = frame.size.width;
            int sampleHeight = frame.size.height;

            //小图原始的宽高，也是要还原出来的目标宽高。
            int destWidth = frame.sourceSize.width;
            int destHeight = frame.sourceSize.height;

            //换算偏移值（不受旋转影响）
            int offsetLX = frame.sourceSize.width / 2 - frame.size.width / 2 - frame.offset.x;
            int offsetTY = frame.sourceSize.height / 2 - frame.size.height / 2 - frame.offset.y;

            //按目标宽高创建一张目标底图
            Texture2D destTexture = new Texture2D(destWidth, destHeight);

            //旋转时，调换采样宽高
            if (frame.isRotated)
            {
                sampleWidth = frame.size.height;
                sampleHeight = frame.size.width;
            }

            //起始位置（Y轴需变换，且受旋转影响）。
            int startPosX = frame.startPos.x;
            int startPosY = bigTexture.height - (frame.startPos.y + sampleHeight);

            //采集像素（受旋转影响）
            Color[] colors = bigTexture.GetPixels(startPosX, startPosY, sampleWidth, sampleHeight);

            //Debug.Log("offsetLX: " + offsetLX);
            //Debug.Log("offsetTY: " + offsetTY);

            //设置像素（将采样像素放到目标图中去）
            for (int w = 0; w < destWidth; w++)
            {
                for (int h = 0; h < destHeight; h++)
                {
                    if (w >= offsetLX && w < frame.size.width + offsetLX && h >= offsetTY && h < frame.size.height + offsetTY)
                    {
                        //找到目标区域坐标( w，h)对应的采样区域坐标
                        if (frame.isRotated)
                        {
                            //旋转时，目标图中的坐标(w, h),对应的采样区坐标为(h-offsetTY, sampleHeight-(w-offsetLX)-1)
                            int index = (sampleHeight - (w - offsetLX) - 1) * sampleWidth + (h - offsetTY);
                            destTexture.SetPixel(w, h, colors[index]);
                        }
                        else
                        {
                            //没有旋转时，目标图中的坐标(w, h),对应的采样区坐标为（w-offsetLX, h-offsetTY）
                            int index = (h - offsetTY) * sampleWidth + (w - offsetLX);
                            destTexture.SetPixel(w, h, colors[index]);
                        }
                    }
                    else
                    {
                        //四周颜色
                        destTexture.SetPixel(w, h, new Color(0, 0, 0, 0));
                    }
                }
            }

            destTexture.Apply();

            //保存打扫
            byte[] bytes = destTexture.EncodeToPNG();
            Save("Restore", frame.textureName, bytes);
            Texture2D.DestroyImmediate(destTexture);
            destTexture = null;
        }

        private void Save(string subDir, string textureName, byte[] bytes)
        {
            string dir = this.main.GetSaveDir() + @"\" + subDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            FileStream file = File.Open(dir + @"\" + textureName, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
        }
    }
}