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
        // Only show crosshair when using a controller
        if (playerInput.currentControlScheme == "Gamepad")
        {
            Vector2 crosshairOffset = PlayerCombat.Instance.aimDirection * radius;

            // Disable the crosshair if the player is not aiming
            if (crosshairOffset == Vector2.zero)
                sprite.enabled = false;

            // Otherwise, enable the sprite and move it to the aimed position
            else
            {
                sprite.enabled = true;
                transform.position = (Vector2)playerTransform.position + crosshairOffset;
            }
        }
        else
            sprite.enabled = false;
    }
}