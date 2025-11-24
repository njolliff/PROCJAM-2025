using System;
using UnityEngine;

public class RoomShroud : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float _transitionDuration;

    public Action onFadeFromBlackFinished;

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
            _sprite.color = new Color(0, 0, 0, a);

            // Return to idle if fade is finished
            if (_fadeProgress >= 1)
                _state = ShroudState.Idle;
        }

        // If fading FROM black
        else if (_state == ShroudState.FadingFromBlack)
        {
            // Smoothly fade alpha value
            float a = 1 - _fadeCurve.Evaluate(_fadeProgress);
            _sprite.color = new Color(0, 0, 0, a);

            // Return to idle if fade is finished
            if (_fadeProgress >= 1)
            {
                _state = ShroudState.Idle;
                onFadeFromBlackFinished?.Invoke();
            }
        }
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