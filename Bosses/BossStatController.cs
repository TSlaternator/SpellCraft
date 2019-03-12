using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossStatController : MonoBehaviour
{
    [SerializeField] private float maxHP; //maximum HP of the boss
    [SerializeField] private float fireResistance; //fire resistance of the boss
    [SerializeField] private float waterResistance; //water resistance of the boss
    [SerializeField] private float airResistance; //air resistance of the boss
    [SerializeField] private float earthResistance; //earth resistance of the boss
    [SerializeField] private float orderResistance; //order resistance of the boss
    [SerializeField] private float chaosResistance; //chaos resistance of the boss
    [SerializeField] private GameObject[] FloatingDamageText; //damage numbers used when bosses take damage
    [SerializeField] private Transform textList; //clearnup list to spawn the text under
    [SerializeField] private Transform floatingTextSpawn; //spawn point of the floating text
    [SerializeField] private float textSpawnOffset; //random horizontal offset of the texts spawn point
    [SerializeField] private GameObject manaDrop; //mana orb dropped upon death
    [SerializeField] private GameObject goldDrop; //gold orb dropped upon death
    [SerializeField] private int goldDropped; //how much gold to drop upon death
    [SerializeField] private Animator animator; //animator for the boss' sprite
    [SerializeField] private float phase2Threshold; //the threshold health percentage phase2 will start at
    private bool phase2 = false; //if the boss is in its second phase or not
    private float[] resistances; //holds the resistances of the boss
    private Transform gameController; //transform always facing forwards (used to spawn damage text)
    private float currentHP; //the current HP of the boss
    private float damagedHP; //lags slightly behind current hp to create a visual effect on the healthbar
    private bool marked; //if the enemy is marked or not (marked enemies take more damage from certain elements)
    private int markType; //the element that marked the enemy (will take more damage from all elements except this one)
    private float markMultiplier; //the multiplier applied to mark damage
    private GameObject healthbarObject; //holds the bosses healthbar
    private Image healthbar; //displays the bosses health
    private Image damagedHealthbar; //dispalys recent damage done to the bosses health
    [SerializeField] private float lerpSpeed; //how fast to lerp the damagedhealth bar 


    //initialises variables and references
    void Start() {
        currentHP = maxHP;
        damagedHP = maxHP;
        resistances = new float[] { fireResistance, waterResistance, airResistance, earthResistance, orderResistance, chaosResistance, 0f };
        textList = GameObject.Find("DamageNumbersList").transform;
        gameController = GameObject.Find("GameController").transform;
        healthbarObject = gameController.GetComponent<GameController>().getHealthBar();
        healthbar = gameController.GetComponent<GameController>().getHealthUI();
        damagedHealthbar = gameController.GetComponent<GameController>().getDamagedHealthUI();
        healthbarObject.SetActive(true);
    }

    //updates the bosses time controlled effects
    private void Update() {
        damagedHP = Mathf.Lerp(damagedHP, currentHP, Time.deltaTime * lerpSpeed);
        damagedHealthbar.fillAmount = damagedHP / maxHP;
    }

    //applies damage to the boss
    public void ApplyDamage(float damage, int damageType, bool isCrit, bool isDoT) {
        float realDamage;
        if (marked && damageType != markType) realDamage = damage * markMultiplier * (1 - (resistances[damageType] / 100));
        else realDamage = damage * (1 - (resistances[damageType] / 100));
        realDamage = Mathf.Round(realDamage);
        currentHP -= realDamage;
        if (isDoT) DamageText(realDamage, 2);
        else if (isCrit) DamageText(realDamage, 1);
        else DamageText(realDamage, 0);

        healthbar.fillAmount = currentHP / maxHP;
        if (currentHP <= 0) Die();
        else if (!phase2 && currentHP / maxHP <= phase2Threshold) StartPhase2();
    }

    //kills the boss, dropping rewards
    private void Die() {
        animator.SetBool("IsDead", true);
        gameObject.GetComponent<BossController>().Die();
        DropPickups((int)(goldDropped), goldDrop);
        gameObject.transform.SetParent(null);
        StartCoroutine(CorpseTime());
        healthbarObject.SetActive(false);
    }

    //changes the boss into phase2
    private void StartPhase2() {
        phase2 = true;
        GetComponent<BossController>().setPhase2();
    }

    //How long the boss will stay visible as a corpse before despawning
    private IEnumerator CorpseTime() {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    //drops pickups upon death
    private void DropPickups(int amount, GameObject pickup) {
        for (int i = 0; i < amount; i++) {
            Instantiate(pickup, transform.position + GenerateOffset(), transform.rotation);
        }
    }


    //drains mana from the enemy
    public void ManaDrain() {
        Instantiate(manaDrop, transform.position + GenerateOffset(), transform.rotation);
    }

    //generates a random offset to spawn pickups at
    private Vector3 GenerateOffset() {
        float offsetX = Random.Range(-0.5f, 0.5f);
        float offsetZ = Random.Range(-0.5f, 0.5f);
        return new Vector3(offsetX, 0f, offsetZ);
    }

    //spawns the damage number text
    private void DamageText(float damageNumber, int textType) {
        GameObject text;
        float offsetX = Random.Range(-textSpawnOffset, textSpawnOffset);
        Vector3 textSpawnPosition = new Vector3(floatingTextSpawn.position.x + offsetX, floatingTextSpawn.position.y, floatingTextSpawn.position.z);
        text = Instantiate(FloatingDamageText[textType], textSpawnPosition, gameController.rotation, textList);
        text.GetComponent<DamageTextController>().SetText(damageNumber.ToString());
    }

    //marks the enemy with an element, buffing all other elements' damage
    public void Mark(int type, float multiplier) {
        StopCoroutine(MarkEffect(type, multiplier));
        StartCoroutine(MarkEffect(type, multiplier));
    }

    //Coroutine to control mark duration
    public IEnumerator MarkEffect(int type, float multiplier) {
        marked = true;
        markType = type;
        markMultiplier = multiplier;
        yield return new WaitForSeconds(5f);
        marked = false;
    }

    //returns the health percentage
    public float GetHealthPercent() {
        return currentHP / maxHP;
    }

    //returns the current health
    public float GetHealth() {
        return currentHP;
    }

    //Applies a DoT to the enemy
    public void ApplyDoT(int damageType, float damage) {
        StopCoroutine(DoT(damageType, damage, 1f));
        StartCoroutine(DoT(damageType, damage, 1f));
    }

    //Applies the DoT's effects
    private IEnumerator DoT(int damageType, float damage, float interval) {
        float duration = Time.time + 5.5f;
        while (Time.time < duration) {
            yield return new WaitForSeconds(interval);
            ApplyDamage(damage, damageType, false, true);
        }
    }
}
