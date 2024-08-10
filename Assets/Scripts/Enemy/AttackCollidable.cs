using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackCollidable : MonoBehaviour
{
    public abstract void AttackColliderRelay(bool initialHit);
}
