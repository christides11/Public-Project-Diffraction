namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;
    
    public class OutOfBoundsArrow : MonoBehaviour
    {
        private MatchManager _matchManager;
        private RectTransform _rect;
    
        [SerializeField]
        private RectTransform _topPos;
        [SerializeField]
        private RectTransform _sidePos;
    
        [SerializeField]
        private GameObject _arrowObj;
        [SerializeField]
        private GameObject _trigger;
    
        [SerializeField]
        private List<TextMeshProUGUI> _tags;
        [SerializeField]
        private Image _arrowImage;
        [SerializeField]
        private RectTransform _arrow;
    
        [SerializeField]
        private int _id;
    
        private Camera _cam;
        [SerializeField]
        private RectTransform _canvasRect;
    
        void Start()
        {
            _matchManager = FindObjectOfType<MatchManager>();
            if (_id > _matchManager.fighters.Count - 1)
            {
                gameObject.SetActive(false);
                return;
            }
            _rect = GetComponent<RectTransform>();
            _cam = Camera.main;
            foreach (var tag in _tags)
                tag.text = _matchManager.fighters[_id].controlling.playerTag;
            _tags[0].color = (_matchManager.fighters[_id].controlling.playerColor + Color.white) / 2;
            _arrowImage.color = (_matchManager.fighters[_id].controlling.playerColor + Color.white) / 2;
            if (_matchManager.fighters[_id].controlling.playerColor.r < 0.5 && _matchManager.fighters[_id].controlling.playerColor.g < 0.5 && _matchManager.fighters[_id].controlling.playerColor.b < 0.5)
            {
                _arrowImage.color = Color.white;
                _tags[0].color = Color.white;
            }
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            var point = _matchManager.fighters[_id].transform.position;
            var fighterScreenPos = Vector3.Scale(_cam.WorldToViewportPoint(point), new Vector3(1, 1, 0));
            fighterScreenPos -= new Vector3(0.5f, 0.5f, 0);
            if ((Mathf.Abs(fighterScreenPos.x) < 0.5f && Mathf.Abs(fighterScreenPos.y) < 0.5f) || !_matchManager.fighters[_id].entity.enabled)
            {
                _trigger.SetActive(false);
                _arrowObj.SetActive(false);
                return;
            }
            _trigger.SetActive(true);
            _arrowObj.SetActive(true);
    
            fighterScreenPos.Normalize();
    
            Vector2 viewportPosition = _cam.WorldToViewportPoint(point);
            Vector2 worldObjectScreenPosition = new Vector2(((viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f)));
            Vector2 lookDir = worldObjectScreenPosition - viewportPosition;
    
            var rot = Quaternion.identity;
            if (Mathf.Abs(fighterScreenPos.x) > 0.9f)
            {
                _rect.anchoredPosition = _sidePos.anchoredPosition * Mathf.Sign(fighterScreenPos.x) + worldObjectScreenPosition * Vector2.up;
                _arrow.anchoredPosition = _tags[0].rectTransform.sizeDelta * 0.4f * Vector2.right * Mathf.Sign(fighterScreenPos.x);
                _arrow.rotation = Quaternion.Euler(0, 0, -90 * Mathf.Sign(fighterScreenPos.x));
            }
            else if (Mathf.Abs(fighterScreenPos.x) > 0.6f)
            {
                _rect.anchoredPosition = _sidePos.anchoredPosition * Mathf.Sign(fighterScreenPos.x) * Vector2.right + _topPos.anchoredPosition * Mathf.Sign(fighterScreenPos.y) * Vector2.up;
                _arrow.anchoredPosition = new Vector2(_tags[0].rectTransform.sizeDelta.x * 0.45f * Mathf.Sign(fighterScreenPos.x), _tags[0].rectTransform.sizeDelta.y * 0.35f * Mathf.Sign(fighterScreenPos.y));
                _arrow.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-lookDir.normalized.x, lookDir.normalized.y) * Mathf.Rad2Deg);
            }
            else
            {
                _rect.anchoredPosition = _topPos.anchoredPosition * Mathf.Sign(fighterScreenPos.y) * Vector2.up + worldObjectScreenPosition * Vector2.right;
                _arrow.anchoredPosition = _tags[0].rectTransform.sizeDelta * 0.4f * Vector2.up * Mathf.Sign(fighterScreenPos.y);
                _arrow.rotation = Quaternion.Euler(0, 0, fighterScreenPos.normalized.y > 0 ? 0 : 180);
            }
        }
    }
}
