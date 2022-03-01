using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Resizes UI elements with the scrollwheel, and moves them with the mouse.
/// Zoom code, and template for this class, are taken from this thread: https://forum.unity.com/threads/zoom-in-out-on-scrollrect-content-image.284655/
/// </summary>
public class ResizeOnMouseWheel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Inspector fields
    [SerializeField] float startSize = 1;
    [SerializeField] float minSize = 0.5f;
    [SerializeField] float maxSize = 2;

    [SerializeField] private float zoomRate = 5;
    #endregion

    #region Private Variables
    private bool onObj = false;
    private bool dragStarted = false;
    private Vector3 MouseOrigin;
    private Vector3 ImageOrigin;
    #endregion



    #region Unity Methods
    private void Update()
    {
        float scrollWheel = -Input.GetAxis("Mouse ScrollWheel");

        if (onObj && scrollWheel != 0)
        {
            ChangeZoom(scrollWheel);
        }
        if (onObj && Input.GetMouseButtonDown(0))
        {
            MouseOrigin = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            ImageOrigin = transform.position;
            dragStarted = true;
        }

        if (dragStarted && Input.GetMouseButton(0))
        {
            Vector3 newPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.position = ImageOrigin + (newPos - MouseOrigin);
        }

        else
        {
            dragStarted = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // reset Map Position
            transform.position = Vector3.zero;
            SetZoom(1);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onObj = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onObj = false;
    }

    public void OnDisable()
    {
        onObj = false;
    }
    #endregion

    #region Private Methods
    private void ChangeZoom(float scrollWheel)
    {
        float rate = 1 + zoomRate * Time.unscaledDeltaTime;
        if (scrollWheel > 0)
        {
            SetZoom(Mathf.Clamp(transform.localScale.y / rate, minSize, maxSize));
        }
        else
        {
            SetZoom(Mathf.Clamp(transform.localScale.y * rate, minSize, maxSize));
        }
    }


    private void SetZoom(float targetSize)
    {
        transform.localScale = new Vector3(targetSize, targetSize, 1);
    }
    #endregion
}
