using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public GameConstants.RoadType RoadType;
    public FixedJoint FixedJoint;

    public Transform StartPosition;
    public Transform EndPosition;
    public Transform PolePosition;
}
