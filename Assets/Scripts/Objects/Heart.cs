using UnityEngine;

public class Heart : MonoBehaviour
{
    public float healAmount;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Only get picked up if the player is hurt
        if (PlayerCombat.Instance.hp < PlayerCombat.Instance.maxHP)
        {
            PlayerCombat.Instance.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}