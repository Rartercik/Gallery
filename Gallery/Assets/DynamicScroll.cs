using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class DynamicScroll : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewTransform;
        [SerializeField] private ScrollRect _view;
        [SerializeField] private FlexibleGridLayout _grid;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;
        [SerializeField] private ContentItem[] _items;

        private Action<ContentItem, int> _onContentRefreshed;
        private int _contentLength;
        private int _emptyContentLength;
        private int _count;
        private bool _skipped = false;

        private bool ContentBeginning => _count <= 0;
        private bool ContentFinished => _count >= _contentLength - _items.Length;

        private void Update()
        {
            if (_skipped == false)
            {
                _skipped = true;
                return;
            }

            var reordered = false;
            var offset = new Vector2(0, _content.rect.size.y - _grid.padding.top - _grid.padding.bottom + _grid.Spacing.y);
            var oldContent = new List<ContentItem>();
            var latestOldContent = -1;
            for (int i = 0; i < _items.Length; i++)
            {
                if (GetDisplacementFromCenter(i).y > _content.rect.height / 2f)
                {
                    if (ContentFinished == false)
                    {
                        _items[i].RectTransform.anchoredPosition -= offset;
                        _count++;
                        var index = _count + _items.Length - 1 < _contentLength - _emptyContentLength ? _count + _items.Length - 1 : -1;
                        _onContentRefreshed?.Invoke(_items[i], index);
                        reordered = true;
                    }
                }
                else if (GetDisplacementFromCenter(i).y < _content.rect.height / -2f)
                {
                    if (ContentBeginning == false)
                    {
                        _items[i].RectTransform.anchoredPosition += offset;
                        _count--;
                        if (latestOldContent == -1)
                        {
                            latestOldContent = _count;
                        }
                        oldContent.Add(_items[i]);
                        reordered = true;
                    }
                }
            }

            if (latestOldContent != -1)
            {
                oldContent = oldContent
                    .OrderBy(content => content.RectTransform.position.y)
                    .ThenByDescending(content => content.RectTransform.position.x).ToList();

                for (int i = 0; i < oldContent.Count; i++)
                {
                    _onContentRefreshed?.Invoke(oldContent[i], latestOldContent - i);
                }
            }

            if (reordered)
            {
                OrderItems();
                return;
            }

            var bottomConstraint = -_viewport.rect.height * _viewport.pivot.y + _grid.padding.bottom + _grid.CellSize.y / 2;
            var topConstraint = _viewport.rect.height * (1f - _viewport.pivot.y) - _grid.padding.top - _grid.CellSize.y / 2;
            
            var columnCount = 0;
            for (int i = 0; i < _items.Length; i++)
            {
                if (ContentFinished)
                {
                    if (columnCount >= _grid.Columns)
                    {
                        bottomConstraint += _grid.CellSize.y + _grid.Spacing.y;
                        columnCount = 0;
                    }

                    var index = _items.Length - 1 - i;
                    ClampItem(_items[index], bottomConstraint, false);
                    columnCount++;
                }
                else if (ContentBeginning)
                {
                    if (columnCount >= _grid.Columns)
                    {
                        topConstraint -= _grid.CellSize.y + _grid.Spacing.y;
                        columnCount = 0;
                    }

                    ClampItem(_items[i], topConstraint, true);
                    columnCount++;
                }
            }
        }

        public void Initialize(Action<ContentItem, int> onContentRefreshed, int contentLength)
        {
            SetColumnsOverWidth();
            _count = 0;
            _emptyContentLength = 0;
            _view.verticalNormalizedPosition = 1f;
            _onContentRefreshed = onContentRefreshed;
            _contentLength = contentLength;
            var extraColumns = _contentLength % _grid.Columns;
            if (extraColumns != 0)
            {
                _emptyContentLength = _grid.Columns - extraColumns;
                _contentLength += _emptyContentLength;
            }
            OrderItems();

            for (int i = 0; i < _items.Length; i++)
            {
                _onContentRefreshed?.Invoke(_items[i], i);
            }
        }

        private void SetColumnsOverWidth()
        {
            if ((float)Screen.width / Screen.height > 0.62f)
            {
                _grid.Columns = 3;
            }
            else
            {
                _grid.Columns = 2;
            }
        }

        private void ClampItem(ContentItem item, float targetY, bool clampDown)
        {
            float currentY = GetItemViewportLocalY(item);
            if (clampDown && currentY >= targetY) return;
            if (!clampDown && currentY <= targetY) return;

            Vector3 viewportLocal = new Vector3(0, targetY, 0);
            Vector3 world = _viewport.TransformPoint(viewportLocal);

            Vector2 screen = RectTransformUtility.WorldToScreenPoint(Camera.main, world);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _content,
                screen,
                Camera.main,
                out Vector2 contentLocal
            );

            item.RectTransform.anchoredPosition =
                new Vector2(item.RectTransform.anchoredPosition.x, contentLocal.y);
        }

        private float GetItemViewportLocalY(ContentItem item)
        {
            Vector3 world = item.RectTransform.TransformPoint(item.RectTransform.rect.center);

            Vector2 screen = RectTransformUtility.WorldToScreenPoint(Camera.main, world);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _viewport,
                screen,
                Camera.main,
                out Vector2 local
            );

            return local.y;
        }

        private Vector2 GetDisplacementFromCenter(int index)
        {
            return _items[index].RectTransform.anchoredPosition + _content.anchoredPosition - new Vector2(_viewport.rect.width * (0.5f - _content.anchorMin.x), _viewport.rect.height * (0.5f - _content.anchorMin.y));
        }

        private void OrderItems()
        {
            _items = _items.OrderByDescending(item => item.RectTransform.position.y).ToArray();
        }
    }
}
