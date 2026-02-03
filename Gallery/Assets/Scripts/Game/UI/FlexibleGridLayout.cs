using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class FlexibleGridLayout : LayoutGroup, ILayoutElement
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns
        }

        [SerializeField] private FitType _fitType = FitType.Uniform;
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        [SerializeField] private Vector2 _cellSize;
        [SerializeField] private Vector2 _spacing;
        [SerializeField] private bool _alwaysSquare;
        [SerializeField] private bool _fitX;
        [SerializeField] private bool _fitY;

        public int Rows
        {
            get
            {
                return _rows;
            }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Value should be more than 0");
                _rows = value;
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        public int Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Value should be more than 0");
                _columns = value;
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        public float TotalWidth => preferredWidth - padding.left - padding.right;
        public float TotalHeight => preferredHeight - padding.top - padding.bottom;
        public Vector2 CellSize => _cellSize;
        public Vector2 Spacing => _spacing;

        public override float preferredWidth => _cellSize.x * _columns + _spacing.x * (_columns - 1) + padding.left + padding.right;
        public override float preferredHeight => _cellSize.y * _rows + _spacing.y * (_rows - 1) + padding.top + padding.bottom;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_rows <= 0)
            {
                _rows = 1;
            }
            if (_columns <= 0)
            {
                _columns = 1;
            }
        }
#endif

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            SetLayout();
        }

        public override void CalculateLayoutInputVertical() { }

        public override void SetLayoutHorizontal() { }

        public override void SetLayoutVertical() { }

        private void SetLayout()
        {
            var fittingConstrained = CheckFittingConstrained(_fitX, _fitY, out _fitX, out _fitY);
            SetUnconstrainedGrid(fittingConstrained, ref _rows, ref _columns);
            SetGrid(_fitType, ref _rows, ref _columns);
            _cellSize = GetCellSizes(_cellSize, _fitX, _fitY);
            SetCells(_spacing, _cellSize);
        }

        private bool CheckFittingConstrained(bool defaultFitX, bool defaultFitY, out bool fitX, out bool fitY)
        {
            fitX = defaultFitX;
            fitY = defaultFitY;

            if (_fitType == FitType.Width || _fitType == FitType.Height || _fitType == FitType.Uniform)
            {
                switch (_fitType)
                {
                    case FitType.Width:
                        fitX = true;
                        fitY = false;
                        break;
                    case FitType.Height:
                        fitX = false;
                        fitY = true;
                        break;
                    case FitType.Uniform:
                        fitX = fitY = true;
                        break;
                }
                return false;
            }
            return true;
        }

        private void SetUnconstrainedGrid(bool fittingConstrained, ref int rows, ref int columns)
        {
            if (fittingConstrained == false)
            {
                float squareRoot = Mathf.Sqrt(transform.childCount);
                rows = columns = Mathf.CeilToInt(squareRoot);
            }
        }

        private void SetGrid(FitType fitType, ref int rows, ref int columns)
        {
            if (fitType == FitType.Width || fitType == FitType.FixedColumns)
            {
                rows = Mathf.CeilToInt(transform.childCount / (float)_columns);
            }
            if (fitType == FitType.Height || fitType == FitType.FixedRows)
            {
                columns = Mathf.CeilToInt(transform.childCount / (float)_rows);
            }
        }

        private Vector2 GetCellSizes(Vector2 defaultCellSize, bool fitX, bool fitY)
        {
            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float cellWidth = parentWidth / _columns - (_spacing.x / _columns * (_columns - 1))
                - (padding.left / (float)_columns) - (padding.right / (float)_columns);
            float cellHeight = parentHeight / _rows - (_spacing.y / _rows * (_rows - 1))
                - (padding.top / (float)_rows) - (padding.bottom / (float)_rows);

            var result = Vector2.zero;
            result.x = fitX ? cellWidth : defaultCellSize.x;
            result.y = fitY ? cellHeight : defaultCellSize.y;

            if (_alwaysSquare)
            {
                // FIX: Only make square cells when we're fitting both dimensions
                // or when we're not fitting any dimension
                if ((fitX && fitY) || (!fitX && !fitY))
                {
                    // When fitting both or neither, use the minimum to ensure it fits
                    var min = Mathf.Min(result.x, result.y);
                    result.x = result.y = min;
                }
                else if (fitX && !fitY)
                {
                    // When fitting only width, use width as the size (height will stretch)
                    result.y = result.x;
                }
                else if (!fitX && fitY)
                {
                    // When fitting only height, use height as the size (width will stretch)
                    result.x = result.y;
                }
            }
            return result;
        }

        private void SetCells(Vector2 spacing, Vector2 cellSize)
        {
            // Calculate total grid size
            float totalGridWidth = _columns * cellSize.x + (_columns - 1) * spacing.x;
            float totalGridHeight = _rows * cellSize.y + (_rows - 1) * spacing.y;

            // Calculate available space
            float availableWidth = rectTransform.rect.width - padding.left - padding.right;
            float availableHeight = rectTransform.rect.height - padding.top - padding.bottom;

            // Calculate starting position based on childAlignment
            float startX = padding.left;
            float startY = padding.top;

            // Horizontal alignment
            switch (childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft:
                    startX = padding.left;
                    break;

                case TextAnchor.UpperCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.LowerCenter:
                    startX = padding.left + (availableWidth - totalGridWidth) * 0.5f;
                    break;

                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    startX = rectTransform.rect.width - padding.right - totalGridWidth;
                    break;
            }

            // Vertical alignment
            switch (childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    startY = padding.top;
                    break;

                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    startY = padding.top + (availableHeight - totalGridHeight) * 0.5f;
                    break;

                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    startY = rectTransform.rect.height - padding.bottom - totalGridHeight;
                    break;
            }

            for (int i = 0; i < rectChildren.Count; i++)
            {
                int rowCount = i / _columns;
                int columnCount = i % _columns;

                var item = rectChildren[i];

                var xPosition = startX + (cellSize.x * columnCount) + (spacing.x * columnCount);
                var yPosition = startY + (cellSize.y * rowCount) + (spacing.y * rowCount);

                SetChildAlongAxis(item, 0, xPosition, cellSize.x);
                SetChildAlongAxis(item, 1, yPosition, cellSize.y);
            }
        }
    }
}