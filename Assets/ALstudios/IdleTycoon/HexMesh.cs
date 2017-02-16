/*
 * HexMesh
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using System.Collections.Generic;
using UnityEngine;

namespace ALStudios.IdleTycoon
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class HexMesh : MonoBehaviour
    {

        #region Variables

        private Mesh _mesh;

        private static readonly List<Vector3> Vertices = new List<Vector3>();
        private static readonly List<int> Triangles = new List<int>();
        private static readonly List<Color> Colors = new List<Color>();

        #endregion

        #region MonoBehaviour

        protected void Awake()
        {
            _mesh = new Mesh
            {
                name = "Hex Mesh"
            };
        }

        #endregion

        #region HexMesh

        public void Triangulate(HexCell[] cells)
        {
            _mesh.Clear();
            Vertices.Clear();
            Triangles.Clear();
            Colors.Clear();

            foreach (HexCell hexCell in cells)
                Triangulate(hexCell);

            _mesh.vertices = Vertices.ToArray();
            _mesh.triangles = Triangles.ToArray();
            _mesh.colors = Colors.ToArray();
            _mesh.RecalculateNormals();

            GetComponent<MeshCollider>().sharedMesh = _mesh;
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        private void Triangulate(HexCell cell)
        {
            foreach (HexDirection direction in Utilities.GetEnumValues<HexDirection>())
                Triangulate(direction, cell);
        }

        private void Triangulate(HexDirection direction, HexCell cell)
        {
            Vector3 center = cell.Position;
            HexEdgeVertices e = new HexEdgeVertices(
                center + HexMetrics.GetFirstSolidCorner(direction),
                center + HexMetrics.GetSecondSolidCorner(direction)
            );

            TriangulateEdgeFan(center, e, cell.Color);

            if (direction <= HexDirection.SOUTH_EAST)
                TriangulateConnection(direction, cell, e);
        }

        private void TriangulateConnection(HexDirection direction, HexCell cell, HexEdgeVertices e1)
        {
            HexCell neighbor = cell.GetNeighbor(direction);

            if (neighbor == null)
                return;

            Vector3 bridge = HexMetrics.GetBridge(direction);
            bridge.y = neighbor.Position.y - cell.Position.y;

            HexEdgeVertices e2 = new HexEdgeVertices(
                e1.V1 + bridge,
                e1.V4 + bridge
            );

            if (cell.GetEdgeType(direction) == HexEdgeType.SLOPE)
                TriangulateEdgeTerraces(e1, cell, e2, neighbor);
            else
                TriangulateEdgeStrip(e1, cell.Color, e2, neighbor.Color);

            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());

            if (direction > HexDirection.EAST || nextNeighbor == null)
                return;

            Vector3 v5 = e1.V4 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Position.y;

            if (cell.Elevation <= neighbor.Elevation)
            {
                if (cell.Elevation <= nextNeighbor.Elevation)
                    TriangulateCorner(e1.V4, cell, e2.V4, neighbor, v5, nextNeighbor);
                else
                    TriangulateCorner(v5, nextNeighbor, e1.V4, cell, e2.V4, neighbor);
            }
            else if (neighbor.Elevation <= nextNeighbor.Elevation)
                TriangulateCorner(e2.V4, neighbor, v5, nextNeighbor, e1.V4, cell);
            else
                TriangulateCorner(v5, nextNeighbor, e1.V4, cell, e2.V4, neighbor);
        }

        private void TriangulateEdgeTerraces(HexEdgeVertices begin, HexCell beginCell, HexEdgeVertices end, HexCell endCell)
        {
            HexEdgeVertices e2 = HexEdgeVertices.TerraceLerp(begin, end, 1);
            Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

            TriangulateEdgeStrip(begin, beginCell.Color, e2, c2);

            for (int i = 2; i < HexMetrics.TERRACE_STEPS; i++)
            {
                HexEdgeVertices e1 = e2;
                Color c1 = c2;

                e2 = HexEdgeVertices.TerraceLerp(begin, end, i);
                c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);

                TriangulateEdgeStrip(e2, c2, end, endCell.Color);
            }

            TriangulateEdgeStrip(e2, c2, end, endCell.Color);
        }

        private void TriangulateEdgeFan(Vector3 center, HexEdgeVertices edge, Color color)
        {
            AddTriangle(center, edge.V1, edge.V2);
            AddTriangleColor(color);

            AddTriangle(center, edge.V2, edge.V3);
            AddTriangleColor(color);

            AddTriangle(center, edge.V3, edge.V4);
            AddTriangleColor(color);
        }

        private void TriangulateEdgeStrip(HexEdgeVertices e1, Color c1, HexEdgeVertices e2, Color c2)
        {
            AddQuad(e1.V1, e1.V2, e2.V1, e2.V2);
            AddQuadColor(c1, c2);

            AddQuad(e1.V2, e1.V3, e2.V2, e2.V3);
            AddQuadColor(c1, c2);

            AddQuad(e1.V3, e1.V4, e2.V3, e2.V4);
            AddQuadColor(c1, c2);
        }

        private void TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
            HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

            if (leftEdgeType == HexEdgeType.SLOPE)
            {
                switch (rightEdgeType)
                {
                    case HexEdgeType.SLOPE:
                        TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                        break;
                    case HexEdgeType.FLAT:
                        TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
                        break;
                    case HexEdgeType.CLIFF:
                        TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
                        break;
                    default:
                        TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
                        break;
                }
            }
            else if (rightEdgeType == HexEdgeType.SLOPE)
            {
                if (leftEdgeType == HexEdgeType.FLAT)
                    TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                else
                    TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
            }
            else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.SLOPE)
            {
                if (leftCell.Elevation < rightCell.Elevation)
                    TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                else
                    TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
            }
            else
            {
                AddTriangle(bottom, left, right);
                AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
            }
        }

        private void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
            Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
            Color c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
            Color c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);

            AddTriangle(begin, v3, v4);
            AddTriangleColor(beginCell.Color, c3, c4);

            for (int i = 2; i < HexMetrics.TERRACE_STEPS; i++)
            {
                Vector3 v1 = v3;
                Vector3 v2 = v4;
                Color c1 = c3;
                Color c2 = c4;

                v3 = HexMetrics.TerraceLerp(begin, left, i);
                v4 = HexMetrics.TerraceLerp(begin, right, i);
                c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
                c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);

                AddQuad(v1, v2, v3, v4);
                AddQuadColor(c1, c2, c3, c4);
            }

            AddQuad(v3, v4, left, right);
            AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
        }

        private void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            float b = 1F / (rightCell.Elevation - beginCell.Elevation);

            if (b < 0)
                b = -b;

            Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(right), b);
            Color boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, b);

            TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.SLOPE)
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            else
            {
                AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
                AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }

        private void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            float b = 1F / (leftCell.Elevation - beginCell.Elevation);

            if (b < 0)
                b = -b;

            Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(left), b);
            Color boundaryColor = Color.Lerp(beginCell.Color, leftCell.Color, b);

            TriangulateBoundaryTriangle(right, rightCell, begin, beginCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.SLOPE)
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            else
            {
                AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
                AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }

        private void TriangulateBoundaryTriangle(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell,
            Vector3 boundary, Color boundaryColor)
        {
            Vector3 v2 = Perturb(HexMetrics.TerraceLerp(begin, left, 1));
            Color c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

            AddTriangleUnperturbed(Perturb(begin), v2, boundary);
            AddTriangleColor(beginCell.Color, c2, boundaryColor);

            for (int i = 2; i < HexMetrics.TERRACE_STEPS; i++)
            {
                Vector3 v1 = v2;
                Color c1 = c2;

                v2 = Perturb(HexMetrics.TerraceLerp(begin, left, i));
                c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);

                AddTriangleUnperturbed(v1, v2, boundary);
                AddTriangleColor(c1, c2, boundaryColor);
            }

            AddTriangleUnperturbed(v2, Perturb(left), boundary);
            AddTriangleColor(c2, leftCell.Color, boundaryColor);
        }

        private Vector3 Perturb(Vector3 position)
        {
            Vector4 sample = HexMetrics.SampleNoise(position);
            position.x += (sample.x * 2F - 1F) * HexMetrics.CELL_PERTURB_STRENGTH;
            position.z += (sample.z * 2F - 1F) * HexMetrics.CELL_PERTURB_STRENGTH;
            return position;
        }

        private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertIndex = Vertices.Count;

            Vertices.Add(Perturb(v1));
            Vertices.Add(Perturb(v2));
            Vertices.Add(Perturb(v3));

            Triangles.Add(vertIndex + 0);
            Triangles.Add(vertIndex + 1);
            Triangles.Add(vertIndex + 2);
        }

        private void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertIndex = Vertices.Count;

            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);

            Triangles.Add(vertIndex + 0);
            Triangles.Add(vertIndex + 1);
            Triangles.Add(vertIndex + 2);
        }

        private void AddTriangleColor(Color color)
        {
            Colors.Add(color);
            Colors.Add(color);
            Colors.Add(color);
        }

        private void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            Colors.Add(c1);
            Colors.Add(c2);
            Colors.Add(c3);
        }

        private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertIndex = Vertices.Count;

            Vertices.Add(Perturb(v1));
            Vertices.Add(Perturb(v2));
            Vertices.Add(Perturb(v3));
            Vertices.Add(Perturb(v4));

            Triangles.Add(vertIndex + 0);
            Triangles.Add(vertIndex + 2);
            Triangles.Add(vertIndex + 1);

            Triangles.Add(vertIndex + 1);
            Triangles.Add(vertIndex + 2);
            Triangles.Add(vertIndex + 3);
        }

        private void AddQuadColor(Color c1, Color c2)
        {
            Colors.Add(c1);
            Colors.Add(c1);
            Colors.Add(c2);
            Colors.Add(c2);
        }

        private void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            Colors.Add(c1);
            Colors.Add(c2);
            Colors.Add(c3);
            Colors.Add(c4);
        }

        #endregion

    }
    
}