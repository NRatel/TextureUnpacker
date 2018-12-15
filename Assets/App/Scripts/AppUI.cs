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
        public GameObject m_Go_BigImageBg;
        public Image m_Image_BigImage;
        public Text m_Text_Tip;
        public Button m_Btn_Excute;
        public Dropdown m_Dropdown_SelectMode;
        public Button m_Btn_ContactMe;

        public AppUI Init()
        {
            this.m_Go_BigImageBg.gameObject.SetActive(false);
            this.m_Text_Tip.gameObject.SetActive(false);
            this.RegisterEvents();
            return this;
        }

        private void RegisterEvents()
        {
            this.m_Btn_ContactMe.onClick.AddListener(()=> {
                Application.OpenURL("https://blog.csdn.net/NRatel/article/details/85009462");
            });
        }

        public void SetTip(string str, bool isError = true)
        {
            this.m_Text_Tip.gameObject.SetActive(true);

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
            this.m_Go_BigImageBg.gameObject.SetActive(true);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            this.m_Image_BigImage.sprite = sprite;
            RectTransform rt = this.m_Image_BigImage.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(texture.width, texture.height);

            //缩放至屏幕可直接显示的大小
            float minRate = Mathf.Min(600.0f / texture.width, 600.0f/ texture.height);
            if (minRate < 1)
            {
                rt.localScale = new Vector2(minRate, minRate);
            }
            else {
                rt.localScale = Vector2.one;
            }
        }
    }
}

