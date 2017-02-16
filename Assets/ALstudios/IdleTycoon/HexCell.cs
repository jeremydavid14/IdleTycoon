/*
 * HexCell
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using UnityEngine;

namespace ALStudios.IdleTycoon
{
    
    public class HexCell : MonoBehaviour
    {

        #region Variables

        public HexGridChunk Chunk;
        public HexCoordinates Coordinates;

        public Vector3 Position
        {
            get { return transform.localPosition; }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color == value)
                    return;

                _color = value;
                Refresh();
            }
        }

        public int Elevation
        {
            get { return _elevation; }
            set
            {
                if (_elevation == value)
                    return;

                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.ELEVATION_STEP;
                position.y += (HexMetrics.SampleNoise(position).y * 2F - 1F) * HexMetrics.ELEVATION_PERTURB_STRENGTH;
                transform.localPosition = position;

                Vector3 uiPosition = UiRect.localPosition;
                uiPosition.z = -position.y;
                UiRect.localPosition = uiPosition;

                _elevation = value;
                Refresh();
            }
        }

        private Color _color;
        private int _elevation;


        public RectTransform UiRect;

        [SerializeField] private HexCell[] _neighbors;

        #endregion

        #region HexCell

        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(Elevation, _neighbors[(int) direction].Elevation);
        }

        public HexEdgeType GetEdgeType(HexCell cell)
        {
            return HexMetrics.GetEdgeType(Elevation, cell.Elevation);
        }

        public HexCell GetNeighbor(HexDirection direction)
        {
            return _neighbors[(int) direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell neighbor)
        {
            _neighbors[(int) direction] = neighbor;
            neighbor._neighbors[(int) direction.Opposite()] = this;
        }

        private void Refresh()
        {
            if (Chunk == null)
                return;

            Chunk.Refresh();

            foreach (HexCell neighbor in _neighbors)
            {
                if (neighbor != null && neighbor.Chunk != null)
                    neighbor.Chunk.Refresh();
            }
        }

        #endregion

    }
    
}