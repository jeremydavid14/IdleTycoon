/*
 * HexGridChunk
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using UnityEngine;

namespace ALStudios.IdleTycoon
{
    
    public class HexGridChunk : MonoBehaviour
    {

        #region Variables

        private HexCell[] _cells;

        private HexMesh _mesh;
        private Canvas _canvas;

        #endregion

        #region MonoBehaviour

        protected void Awake()
        {
            _canvas = GetComponentInChildren<Canvas>();
            _mesh = GetComponentInChildren<HexMesh>();

            _cells = new HexCell[HexMetrics.CHUNK_SIZE_X * HexMetrics.CHUNK_SIZE_Z];
            ShowUi(false);
        }

        protected void LateUpdate()
        {
            _mesh.Triangulate(_cells);
            enabled = false;
        }

        #endregion

        #region MonoBehaviour

        public void Refresh()
        {
            enabled = true;
        }

        public void ShowUi(bool visible)
        {
            _canvas.gameObject.SetActive(visible);
        }

        public void AddCell(int index, HexCell cell)
        {
            _cells[index] = cell;
            cell.Chunk = this;
            cell.transform.SetParent(transform, false);
            cell.UiRect.SetParent(_canvas.transform, false);
        }

        #endregion

    }
    
}