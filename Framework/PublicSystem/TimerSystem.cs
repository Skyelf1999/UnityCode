using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectUtil
{
    /*
        计时器系统
            持有一个计时器Manager用于存储计时器
            自动更新计时器
    */

    public class TimerSystem : MonoBehaviour
    {
        public static TimerSystem instance;
        public TimerManager timerManager;
        
        private void Awake() {
            instance = this;
            timerManager = new TimerManager();
        }


        private void Update() {
            timerManager.UpdateAllTimer();
        }
    }

}
