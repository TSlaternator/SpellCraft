using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScroll : MonoBehaviour, IScrollController
{
    [SerializeField] private int damageType; //damage type of the scroll
    [SerializeField] private float damage; //how much damage the scroll deals
    [SerializeField] private GameObject VFX; //VFX of the scroll
    private Transform enemiesList; //list of enemies
    private int VFXDuration = 10; //how long the VFX is played for


    //What to do when the scroll is cast
    public void OnCast() {
        enemiesList = GameObject.Find("EnemiesList").transform;
        StartCoroutine(VisualFX());
    }

    //Plays the VFX of the scroll
    private IEnumerator VisualFX() {
        int i;
        for (i = 0; i < VFXDuration / 2; i++) {
            yield return new WaitForFixedUpdate();
            VFX.transform.localScale = new Vector3(VFX.transform.localScale.x + 5f, VFX.transform.localScale.y + 5f, VFX.transform.localScale.z + 5f);
        }

        EnemyStatController[] enemies = enemiesList.GetComponentsInChildren<EnemyStatController>();
        for (int j = 0; j < enemies.Length; j++) enemies[j].ApplyDamage(damage, damageType, false, false);

        for (; i < VFXDuration; i++) {
            yield return new WaitForFixedUpdate();
            VFX.transform.localScale = new Vector3(VFX.transform.localScale.x + 5f, VFX.transform.localScale.y + 5f, VFX.transform.localScale.z + 5f);
        }
        Destroy(this.gameObject);
    }
}
