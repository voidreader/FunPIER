using UnityEngine;
using System.Collections;
using RPG.AntiVariable;

public class Test_Performance : MonoBehaviour
{

    System.DateTime intTest1;
    System.DateTime intTest2;

    System.DateTime Test1()
    {
        int a = 0;
        long start = System.DateTime.Now.Ticks;
        for (int i = 0; i < 10000000; i++)
        {
            a++;
        }
        long end = System.DateTime.Now.Ticks;
        System.DateTime DT = new System.DateTime(end - start);
        return DT;

    }

    System.DateTime Test2()
    {
        HInt32 a = 0;
        long start = System.DateTime.Now.Ticks;
        for (int i = 0; i < 10000000; i++)
        {
            a++;
        }
        long end = System.DateTime.Now.Ticks;
        System.DateTime DT = new System.DateTime(end - start);
        return DT;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.width - 20));
        {
            //if (intTest1)
                GUILayout.Label("int Test Result : " + intTest1.Second + "." + intTest1.Millisecond + " Sec");
            if (GUILayout.Button("int Test(10000000 count)", GUILayout.ExpandWidth(false)))
            {
                intTest1 = Test1();
            }

            //if (intTest2)
                GUILayout.Label("HInt32 Test Result : " + intTest2.Second + "." + intTest2.Millisecond + " Sec");
            if (GUILayout.Button("HInt32 Test(10000000 count)", GUILayout.ExpandWidth(false)))
            {
                intTest2 = Test2();
            }
        }
        GUILayout.EndArea();
    }

}
