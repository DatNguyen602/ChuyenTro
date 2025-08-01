using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DOTweenPathCreator : MonoBehaviour
{
    [Serializable]
    public class Waypoint
    {
        public Vector3 position = Vector3.zero;
        public ObjectRan pauseDuration = ObjectRan.zero;
    }

    [Header("Waypoints (position + pause time)")]
    public List<Waypoint> waypoints = new List<Waypoint>()
    {
        new Waypoint { position = Vector3.zero, pauseDuration = ObjectRan.zero },
        new Waypoint { position = Vector3.right, pauseDuration = ObjectRan.zero }
    };

    [Header("Gizmos Settings")]
    public Color lineColor = Color.cyan;
    public float pointHandleSize = 0.1f;
    public float pointPickSize = 0.2f;
}

[Serializable]
public class ObjectRan
{
    public float from;
    public float to;
    public static ObjectRan zero => new ObjectRan(0f, 0f);
    public ObjectRan(float from, float to)
    {
        this.from = from;
        this.to = to;
    }
}