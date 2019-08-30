using System;
    
public interface IUnitySegment
{
    event Action<String> OnSegmentRecieved;

    void CreateSegment();
}
