using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    public static PlayerController Instance;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void OnDestroy()
    {
        // Singleton
        if (Instance == this)
            Instance = null;
    }

    public void KillPlayer()
    {
        Debug.Log("Player died");

        // Halt movement
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.canMove = false;
            PlayerMovement.Instance.rb.linearVelocity = Vector2.zero;
        } 
        
        // Play death animation
        animator.SetBool("isAlive", false);
    }
}