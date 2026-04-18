using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public GameObject bulletPrefab;         // префаб місця влучання після пострілу
    public Animator gunAnimator;
    public GameObject shootFX;              // ефект пострілу
    public Transform muzzle;                // точка спавну ефекта пострілу

    public abstract override void Use();
}
