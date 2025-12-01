using UnityEngine;
using UnityEngine.InputSystem;

public class Crosshair : MonoBehaviour
{
    public Transform playerTransform;
    public PlayerInput playerInput;
    public SpriteRenderer sprite;
    public float radius = 1;

    void Update()
    {
        // Only show crosshair when using a controller, aiming, and able to move
        if (playerInput.currentControlScheme == "Gamepad" && PlayerCombat.Instance.aimDirection != Vector2.zero && PlayerMovement.Instance.canMove)
        {
            Vector2 crosshairOffset = PlayerCombat.Instance.aimDirection * radius;
            transform.localPosition = crosshairOffset;

            sprite.enabled = true;
        }
        else
            sprite.enabled = false;
    }
}