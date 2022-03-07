using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SnappableElement : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private RectTransform _viewPort;
    [SerializeField] private RectTransform _element;
    [SerializeField] private ScrollRect _scroll;

    private LayoutGroup _contentGroup;

    private int _elementIndex;

    private float _elementSizeVertical;
    private float _viewPortSize;
    private float _contentPositionVertical;
    private float _elementPositionVertical;
    private float _topSnapPosition;
    private float _botSnapPosition;

    private float _topPadding;
    private float _botPadding;

    private float _topDifference;
    private float _botDifference;

    private void Awake()
    {
        _contentGroup = _content.GetComponent<LayoutGroup>();
        _viewPortSize = _viewPort.rect.size.y;
        StartCoroutine(GetElementPosition());
        GetContentPosition();
        _elementSizeVertical = _element.rect.size.y;
        _elementIndex = _element.GetSiblingIndex();
        _topPadding = _contentGroup.padding.top;
        _botPadding = _contentGroup.padding.bottom;
    }

    private void Start()
    {
        StartCoroutine(PrepareVerticalGrid());

    }

    private void OnEnable()
    {
        _scroll.onValueChanged.AddListener
            ((value) =>
            {
                TrySnap();
            });
    }

    private void OnDisable()
    {
        _scroll.onValueChanged.RemoveListener
            ((value) =>
             {
                 TrySnap();
             });
    }

    private void TrySnap()
    {
        GetContentPosition();
        CalculateAnchors();
        CalculateDifferences();

        if (!IsOutOfVision())
        {
            IgnoreLayout(false);
            return;
        }

        IgnoreLayout(true);

        if (_botDifference <= 0)
        {
            SnapTo(_botSnapPosition);
        }
        else if (_topDifference >= 0)
        {
            SnapTo(_topSnapPosition);
        }
    }

    private void SnapTo(float verticalPos)
    {
        _element.anchoredPosition = new Vector2(_element.anchoredPosition.x, -verticalPos);
    }

    private void IgnoreLayout(bool state)
    {
        _contentGroup.enabled = !state;

        if (state)
        {
            _element.SetAsLastSibling();
        }
        else
        {
            _element.SetSiblingIndex(_elementIndex);
        }

    }

    private void CalculateAnchors()
    {
        _topSnapPosition = _contentPositionVertical + _elementSizeVertical / 2 + _topPadding;
        _botSnapPosition = _contentPositionVertical + _viewPortSize - _elementSizeVertical / 2 - _botPadding;
    }

    private void CalculateDifferences()
    {
        _topDifference = Mathf.Abs(_topSnapPosition) - Mathf.Abs(_elementPositionVertical);
        _botDifference = Mathf.Abs(_botSnapPosition) - Mathf.Abs(_elementPositionVertical);
    }

    private bool IsOutOfVision()
    {
        if (_botDifference <= 0 || _topDifference >= 0)
        {
            return true;
        }

        return false;
    }

    private void GetContentPosition()
    {
        _contentPositionVertical = _content.anchoredPosition.y;
    }

    private IEnumerator GetElementPosition()
    {
        yield return new WaitForEndOfFrame();
        _elementPositionVertical = _element.anchoredPosition.y;
    }

    private IEnumerator PrepareVerticalGrid()
    {
        yield return new WaitForEndOfFrame();
        TrySnap();

    }
}
