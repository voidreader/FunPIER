using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
/// <summary>
/// This class is for managing Unity ADS system. To use Unity ADS, you need to import Unity ADS from Window/Services/Ads.
/// </summary>
public class ADS : MonoBehaviour {
    public int everyXGameShowAdMin; // Ad show minimum random value
                                                              //------>>> If you want to set fixed values, you can set like this: Minimum is 3 and Maximum is 4. This will produce only 3.
    public int everyXGameShowAdMax; // Ad show maximum random value
    private int adRate; //This holds produced random value by using everyXGameShowAdMin and everyXGameShowAdMax.
    private int counter=0; //This counts deaths in game.
    void Awake()
    {
        adRate = Random.Range(everyXGameShowAdMin, everyXGameShowAdMax); //Produce a random integer value.
    }
	public void ShowAd() //This method is called in RestartButtonInput method. If you are not going to use Unity ADS, you can comment 29th line to not get warning debug log.
    {
        counter++;
        if (adRate <=counter) {
#if UNITY_ADS
            Advertisement.Show();
#else
        Debug.LogWarning("WARNING----Unity ADS not implemented. Showing Ad method has aborted\nIf you want to use Unity ADS, you need to activate Unity ADS from Window/Services/Ads----WARNING");
#endif
            adRate = Random.Range(everyXGameShowAdMin, everyXGameShowAdMax);
            counter = 0;
        }
    }
}
