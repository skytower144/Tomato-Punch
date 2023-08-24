using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimator : MonoBehaviour
{
    [SerializeField] private Image _image;
    private List<Sprite> _bgSprites;
    private bool _isMovingBg;

    private int _currentFrame, _totalFrames, _fps;
    private float _frameRate, _timer;

    void Update()
    {
        Animate();
    }

    void OnDisable()
    {
        _bgSprites = new List<Sprite>();
    }

    public void SetBackground(List<Sprite> _bgSprites)
    {
        this._bgSprites = _bgSprites;
        _image.sprite = _bgSprites[0];

        InitAnimator();
        _isMovingBg = _bgSprites.Count > 1;
    }

    private void InitAnimator()
    {
        _totalFrames = _bgSprites.Count;
        _currentFrame = 0;
        _timer = 0f;

        if (_fps == 0) _frameRate = 0;
        else _frameRate = 1f / (float)_fps;
    }

    private void Animate()
    {
        if (_isMovingBg)
        {
            _timer += Time.deltaTime;

            if (_timer > _frameRate)
            {
                _currentFrame = (_currentFrame + 1) % _totalFrames;
                _image.sprite = _bgSprites[_currentFrame];
                _timer = 0f;
            }
        }
    }
}
