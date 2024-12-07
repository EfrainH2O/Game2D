using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Character", menuName = "Character", order = 0)]
public class Character : ScriptableObject {
    
    public string CharacterName;
    public RuntimeAnimatorController animator;
    public Sprite image;
}
