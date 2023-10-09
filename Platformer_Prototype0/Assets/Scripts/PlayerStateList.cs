using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool jumping = false;
    public bool dashing = false;
    public bool recoilingX, recoilingY;
    public bool lookingRight = true; //this is the starting direction
    public bool invincible;
    public bool healing;
    public bool casting;
    public bool cutscene = false;
}
