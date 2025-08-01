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
        public float pauseDuration = 0.5f;
    }

    [Header("Waypoints (position + pause time)")]
    public List<Waypoint> waypoints = new List<Waypoint>()
    {
        new Waypoint { position = Vector3.zero, pauseDuration = 0.5f },
        new Waypoint { position = Vector3.right, pauseDuration = 0.5f }
    };

    [Header("Gizmos Settings")]
    public Color lineColor = Color.cyan;
    public float pointHandleSize = 0.1f;
    public float pointPickSize = 0.2f;
}
