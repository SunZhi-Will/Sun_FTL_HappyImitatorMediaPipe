using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Beatmap", menuName = "Create Beatmap", order = 1)]
public class Beatmap : ScriptableObject
{
    const int UP = 1, RIGHT = 2, DOWN = 3, LEFT = 4, EMPTY = 0;

    public float bpm = 120f;
    public int measure = 4;
    public AudioClip clip;
    public List<string> beatlines = new List<string>();


}
