using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontSleepScreen : MonoBehaviour
{
    void Start()
    {
        // Ngăn không cho màn hình tắt
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnDestroy()
    {
        // Khôi phục lại chế độ tắt màn hình mặc định
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
