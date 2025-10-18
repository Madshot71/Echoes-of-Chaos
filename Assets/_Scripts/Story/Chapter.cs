using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "Chapter", order = 0)]
public class Chapter : ScriptableObject {
    [Scene] public string[] scenes;
}