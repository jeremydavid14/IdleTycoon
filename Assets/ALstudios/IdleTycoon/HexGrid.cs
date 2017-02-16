/*
 * HexGrid
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using UnityEngine;
using UnityEngine.UI;

namespace ALStudios.IdleTycoon
{
    
    public class HexGrid : MonoBehaviour
    {

        #region Variables

        public Texture2D NoiseSource;

        public HexGridChunk ChunkPrefab;
        public HexCell CellPrefab;
        public Text LabelPrefab;

        public Color DefaultColor = Color.white;

        public int ChunkCountX = 4;
        public int ChunkCountZ = 3;

        private int _cellCountX;
        private int _cellCountZ;

        private HexGridChunk[] _chunks;
        private HexCell[] _cells;

        #endregion

        #region MonoBehaviour

        protected void Awake()
        {
            HexMetrics.NoiseSource = NoiseSource;

            _cellCountX = ChunkCountX * HexMetrics.CHUNK_SIZE_X;
            _cellCountZ = ChunkCountZ * HexMetrics.CHUNK_SIZE_Z;

            CreateChunks();
            CreateCells();
        }

        protected void OnEnable()
        {
            HexMetrics.NoiseSource = NoiseSource;
        }

        #endregion

        #region HexGrid

        public HexCell GetCell(HexCoordinates coordinates)
        {
            int z = coordinates.Z;

            if (z < 0 || z >= _cellCountZ)
                return null;

            int x = coordinates.X + z / 2;

            if (x < 0 || x >= _cellCountX)
                return null;

            return _cells[x + z * _cellCountX];
        }

        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);

            int index = coordinates.X + coordinates.Z * _cellCountX + coordinates.Z / 2;
            return _cells[index];
        }

        public void ShowUi(bool visible)
        {
            foreach (HexGridChunk chunk in _chunks)
                chunk.ShowUi(visible);
        }

        private void CreateCells()
        {
            _cells = new HexCell[_cellCountZ * _cellCountX];

            for (int z = 0, index = 0; z < _cellCountZ; z++)
                for (int x = 0; x < _cellCountX; x++)
                    CreateCell(x, z, index++);
        }

        private void CreateChunks()
        {
            _chunks = new HexGridChunk[ChunkCountZ * ChunkCountX];

            for (int z = 0, index = 0; z < ChunkCountZ; z++)
                for (int x = 0; x < ChunkCountX; x++)
                {
                    HexGridChunk chunk = _chunks[index++] = Instantiate(ChunkPrefab);
                    chunk.transform.SetParent(transform);
                }
        }

        private void CreateCell(int x, int z, int index)
        {
            Vector3 position;
            position.x = (x + z * 0.5F - z / 2) * (HexMetrics.INNER_RADIUS * 2F);
            position.y = 0F;
            position.z = z * (HexMetrics.OUTER_RADIUS * 1.5F);

            HexCell cell = _cells[index] = Instantiate(CellPrefab);
            cell.transform.localPosition = position;

            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color = DefaultColor;

            if (x > 0)
                cell.SetNeighbor(HexDirection.WEST, _cells[index - 1]);

            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SOUTH_EAST, _cells[index - _cellCountX]);

                    if (x > 0)
                        cell.SetNeighbor(HexDirection.SOUTH_WEST, _cells[index - _cellCountX - 1]);
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SOUTH_WEST, _cells[index - _cellCountX]);

                    if (x < _cellCountX - 1)
                        cell.SetNeighbor(HexDirection.SOUTH_EAST, _cells[index - _cellCountX + 1]);
                }
            }

            Text label = Instantiate<Text>(LabelPrefab);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.Coordinates.ToStringDivided();

            cell.UiRect = label.rectTransform;
            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }

        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            int chunkX = x / HexMetrics.CHUNK_SIZE_X;
            int chunkZ = z / HexMetrics.CHUNK_SIZE_Z;
            HexGridChunk chunk = _chunks[chunkX + chunkZ * ChunkCountX];

            int localX = x - chunkX * HexMetrics.CHUNK_SIZE_X;
            int localZ = z - chunkZ * HexMetrics.CHUNK_SIZE_Z;
            chunk.AddCell(localX + localZ * HexMetrics.CHUNK_SIZE_X, cell);
        }

        #endregion

    }
    
}