using UnityEngine;
using System;
using System.Collections;

namespace RPG.AntiVariable
{
    public class AntiManager : MonoBehaviour
    {
        Action SpeedHackDetected = null;
        public float Speed = 1.0f;

        static System.Random rand = new System.Random(DateTime.Now.Millisecond);

        static AntiManager _instance = null;
        public static AntiManager Instance
        {
            get
            {
                if (_instance == null)
                {     
                    _instance = GameObject.FindObjectOfType(typeof(AntiManager)) as AntiManager;
                    if (!_instance)
                        _instance = new GameObject("AntiManager").AddComponent<AntiManager>();
                    //_instance = new AntiManager();
                }
                return _instance;
            }
        }

        static public int random(int min, int max)
        {
            return rand.Next(min, max);
        }

        public void StartDetection(Action action)
        {
            SpeedHackDetected = action;
        }

        void Awake()
        {
            if (_instance == null)
                _instance = this;
            DontDestroyOnLoad(this);
        }

        long Tick;
        void Start()
        {
            Tick = DateTime.Now.Ticks;
            StartCoroutine("CheckSpeedHack");
        }

        IEnumerator CheckSpeedHack()
        {            
            while(true)
            {
                Tick = DateTime.Now.Ticks;
                yield return new WaitForSeconds(1.0f);
                long diff = DateTime.Now.Ticks - Tick;
                //Debug.Log("diff = " + diff);
                Speed = Mathf.Round(10000000.0f / diff);
                if (Speed > 1.1f)
                {
                    if (SpeedHackDetected != null)
                    {
                        SpeedHackDetected();
                        SpeedHackDetected = null;                                
                    }
                }
            }
        }



    }
}