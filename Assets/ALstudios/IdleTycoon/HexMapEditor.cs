/*
 * HexMapEditor
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using UnityEngine;
using UnityEngine.EventSystems;

namespace ALStudios.IdleTycoon
{
    
    public class HexMapEditor : MonoBehaviour
    {

        #region Variables

        public HexGrid Grid;

        public Color[] Colors;

        private Color _color;
        private bool _applyColor;

        private int _elevation;
        private bool _applyElevation;

        private int _brushSize;

        #endregion

        #region MonoBehaviour

        protected void Awake()
        {
            SelectColor(-1);
        }

        protected void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButton(0))
                HandleInput();
        }

        #endregion

        #region HexMapEditor

        public void ShowUi(bool visible)
        {
            Grid.ShowUi(visible);
        }

        public void SelectColor(int index)
        {
            _applyColor = index >= 0;

            if (_applyColor)
                _color = Colors[index];
        }

        public void SetBrushSize(float size)
        {
            _brushSize = (int) size;
        }

        public void EnableElevation(bool apply)
        {
            _applyElevation = apply;
        }

        public void SetElevation(float elevation)
        {
            _elevation = (int) elevation;
        }

        private void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(inputRay, out hit))
                return;

            EditCells(Grid.GetCell(hit.point));
        }

        private void EditCells(HexCell center)
        {
            int cx = center.Coordinates.X;
            int cz = center.Coordinates.Z;

            for (int r = 0, z = cz - _brushSize; z <= cz; z++, r++)
                for (int x = cx - r; x <= cx + _brushSize; x++)
                    EditCell(Grid.GetCell(new HexCoordinates(x, z)));

            for (int r = 0, z = cz + _brushSize; z > cz; z--, r++)
                for (int x = cx - _brushSize; x <= cx + r; x++)
                    EditCell(Grid.GetCell(new HexCoordinates(x, z)));
        }

        private void EditCell(HexCell cell)
        {
            if (cell == null)
                return;

            if (_applyColor)
                cell.Color = _color;

            if (_applyElevation)
                cell.Elevation = _elevation;
        }

        #endregion

    }
    
}