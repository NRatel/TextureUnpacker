using UnityEngine;

namespace NRatel.TextureUnpacker
{
    //入口文件，兼职处理App的协程
    public class Main : MonoBehaviour
    {
        void Start()
        {
            Screen.SetResolution(800, 600, false);
            new App(this);
        }
    }
}