using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "TimelineDatabase", menuName = "Game/TimelineDatabase")]
public class TimelineDatabase : ScriptableObject
{
    public List<TimelineEntry> timelines;

    public TimelineAsset GetTimeline(string name)
    {
        return timelines.Find(t => t.name == name).timeline;
    }
}
