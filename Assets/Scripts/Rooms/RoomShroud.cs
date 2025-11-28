using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomShroud : MonoBehaviour
{
    [Header("Shroud Settings")]
    [SerializeField] private float _transitionDuration;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("References")]
    [SerializeField] private SpriteRenderer _shroudSprite;
    [SerializeField] private Room _room;

    private enum ShroudState { Idle, FadingToBlack, FadingFromBlack}
    private ShroudState _state = ShroudState.Idle;

    private float _fadeProgress;

    void Update()
    {
        // Do nothing in idle state
        if (_state == ShroudState.Idle)
            return;

        // Increment fade progress
        _fadeProgress += Time.deltaTime / _transitionDuration;
        _fadeProgress = Mathf.Clamp01(_fadeProgress);

        // If fading TO black
        if (_state == ShroudState.FadingToBlack)
        {
            // Smoothly fade alpha value
            float a = _fadeCurve.Evaluate(_fadeProgress);
            _shroudSprite.color = new Color(0, 0, 0, a);

            // Disable room sprites and return to idle if fade is finished
            if (_fadeProgress >= 1)
            {
                _state = ShroudState.Idle;
                DisableRoomSprites();
            }
        }

        // If fading FROM black
        else if (_state == ShroudState.FadingFromBlack)
        {
            // Enable room sprite
            EnableRoomSprites();

            // Smoothly fade alpha value
            float a = 1 - _fadeCurve.Evaluate(_fadeProgress);
            _shroudSprite.color = new Color(0, 0, 0, a);

            // Return to idle if fade is finished
            if (_fadeProgress >= 1)
            {
                _state = ShroudState.Idle;
                _room.FinishedFadingFromBlack();
            }
        }
    }

    private void EnableRoomSprites()
    {
        // Get all SpriteRenderer (except the shroud) and TilemapRenderers in the room
        List<SpriteRenderer> spriteRenderers = _room.GetComponentsInChildren<SpriteRenderer>().ToList();
        spriteRenderers.Remove(_shroudSprite);
        List<TilemapRenderer> tilemapRenderers = _room.GetComponentsInChildren<TilemapRenderer>().ToList();

        // Enable each renderer
        if (spriteRenderers.Count > 0)
            foreach (var renderer in spriteRenderers)
                renderer.enabled = true;
        if (tilemapRenderers.Count > 0)
            foreach (var renderer in tilemapRenderers)
                renderer.enabled = true;
    }
    private void DisableRoomSprites()
    {
        // Get all SpriteRenderer (except the shroud) and TilemapRenderers in the room
        List<SpriteRenderer> spriteRenderers = _room.GetComponentsInChildren<SpriteRenderer>().ToList();
        spriteRenderers.Remove(_shroudSprite);
        List<TilemapRenderer> tilemapRenderers = _room.GetComponentsInChildren<TilemapRenderer>().ToList();

        // Disable each renderer
        foreach (var renderer in spriteRenderers)
            renderer.enabled = false;
        foreach (var renderer in tilemapRenderers)
            renderer.enabled = false;
    }

    [ContextMenu("Fade to Black")]
    public void FadeToBlack()
    {
        _state = ShroudState.FadingToBlack;
        _fadeProgress = 0;
    }
    [ContextMenu("Fade from Black")]
    public void FadeFromBlack()
    {
        _state = ShroudState.FadingFromBlack;
        _fadeProgress = 0;
    }
}