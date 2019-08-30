using System;
using UnityEngine;

public class IronSourceSegmentAndroid : AndroidJavaProxy, IUnitySegment
    {
    private AndroidJavaObject segmentObject;
    public event Action<string> OnSegmentRecieved = delegate { };

    //implements UnitySegmentListener java interface
    public IronSourceSegmentAndroid():base(IronSourceConstants.segmentBridgeListenerClass)
        {
        try
        {
            
            using (var pluginClass = new AndroidJavaClass(IronSourceConstants.bridgeClass))
            {
                segmentObject = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            segmentObject.Call("setUnitySegmentListener", this);
        }
       catch(Exception e)
       {
            Debug.LogError("setUnitySegmentListener method doesn't exist, error: " + e.Message);
        }

    }
    
    //implement Segment callback in the bridge
    public void CreateSegment()
    {
        try
        {
            segmentObject.Call("createSegment");
        }
        catch(Exception e)
        {
            Debug.LogError("createSegment method doesn't exist, error: " + e.Message);
        }
    }

    public void onSegmentRecieved(string segmentName){
        if( OnSegmentRecieved != null)
        {
            OnSegmentRecieved(segmentName);
        }
    }

}
