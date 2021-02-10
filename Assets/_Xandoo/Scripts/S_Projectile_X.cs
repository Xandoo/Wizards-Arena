using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Projectile_X : MonoBehaviour
{
    public SOBJ_Spell_X spellSettings;
    private SphereCollider sCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        sCollider = GetComponent<SphereCollider>();
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, sCollider.radius);
    }
}
