using UnityEngine.UI;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private RawImage _image;
    [SerializeField] private float _x, _y;

    void FixedUpdate()
    {
        _image.uvRect = new Rect(_image.uvRect.position + Time.deltaTime * new Vector2(_x,_y) , _image.uvRect.size);
    }

    void OnDisable()
    {
        _image.uvRect = new Rect(Vector2.zero, new Vector2(1, 1));
    }

    public void SetParallaxDirection(Vector2 direction)
    {
        _x = direction.x;
        _y = direction.y;
    }
}
