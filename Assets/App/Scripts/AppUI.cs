using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRatel.Win32;
using System.IO;
using UnityEngine.UI;

namespace NRatel.TextureUnpacker
{
    public class AppUI : MonoBehaviour
    {
        public Image m_Image_BigImage;
        public Text m_Text_Tip;
        public Button m_Btn_Excute;

        public void SetTip(string str, bool isError = true)
        {
            string prefix = "<color=red>";
            string postfix = "</color>";
            if (isError == false)
            {
                prefix = "<color=green>";
            }
            this.m_Text_Tip.text = prefix + str + postfix;
        }

        public void SetImage(Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            this.m_Image_BigImage.sprite = sprite;

            float width, height;
            float aspectRatio = 1.0f * texture.width / texture.height;

            int max = Mathf.Max(texture.width, texture.height);
            max = max < 600 ? max : 600;

            if (aspectRatio == 1)
            {
                width = max;
                height = max;
            }
            else if (aspectRatio > 1)
            {
                width = max;
                height = width / aspectRatio;
            }
            else
            {
                height = max;
                width = height * aspectRatio;
            }

            this.m_Image_BigImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
    }
}

