using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    // створюємо сінглтон як єдину точку доступу до методів скрипта
    public static WindowsManager Layout;

    [SerializeField] private GameObject [] windows;

    private void Awake()
    {
        // ініціалізація сінглтону
        Layout = this;

        foreach(GameObject window in windows)
        {
            window.SetActive(false);
        }
    }

    private void Start()
    {
        OpenLayout("Panel_Loading");
    }

    public void OpenLayout(string name)
    {
        foreach(GameObject window in windows)
        {
            if(window.name == name)
            {
                window.SetActive(true);
            }
            else
            {
                window.SetActive(false);
            }
        }
    }
}
