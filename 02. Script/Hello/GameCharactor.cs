using UnityEngine;

public class GameCharactor : MonoBehaviour
{
    public string charName;
    public float health;
    public float maxHealth;

    private Animator animator;
    private bool isDead;
    public int power;

    void Start()
    {
        /*
        animator = GetComponent<Animator>();
        isDead = false;*/
    }



    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        //animator.SetTrigger("Die");
        //CombatProgress.Instance.OnCharacterDeath(this);
    }
}
