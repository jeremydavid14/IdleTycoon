/*
 * HexMapCamera
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using System;
using UnityEngine;

namespace ALStudios.IdleTycoon
{
    
    public class HexMapCamera : MonoBehaviour
    {

        #region Variables

        public HexGrid Grid;

        public float StickMinZoom = -250;
        public float StickMaxZoom = -45;

        public float SwivelMinZoom = 90;
        public float SwivelMaxZoom = 45;

        public float MoveSpeedMinZoom = 400;
        public float MoveSpeedMaxZoom = 100;

        public float RotationSpeed = 180;

        private Transform _swivel;
        private Transform _stick;

        private float _zoom = 1F;
        private float _rotationAngle;

        #endregion

        #region MonoBehaviour

        protected void Awake()
        {
            _swivel = transform.GetChild(0);
            _stick = _swivel.GetChild(0);
        }

        protected void Update()
        {
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel");

            if (Math.Abs(zoomDelta) >= 0.01F)
                AdjustZoom(zoomDelta);

            float rotationDelta = Input.GetAxis("Rotation");

            if (Math.Abs(rotationDelta) >= 0.01F)
                AdjustRotation(rotationDelta);

            float xDelta = Input.GetAxis("Horizontal");
            float zDelta = Input.GetAxis("Vertical");

            if (Math.Abs(xDelta) >= 0.01F || Math.Abs(zDelta) >= 0.01F)
                AdjustPosition(xDelta, zDelta);
        }

        #endregion

        #region HexMapCamera

        private void AdjustZoom(float delta)
        {
            _zoom = Mathf.Clamp01(_zoom + delta);

            float distance = Mathf.Lerp(StickMinZoom, StickMaxZoom, _zoom);
            _stick.localPosition = new Vector3(0, 0, distance);

            float angle = Mathf.Lerp(SwivelMinZoom, SwivelMaxZoom, _zoom);
            _swivel.localRotation = Quaternion.Euler(angle, 0, 0);
        }

        private void AdjustPosition(float xDelta, float zDelta)
        {
            Vector3 direction = transform.localRotation * new Vector3(xDelta, 0, zDelta).normalized;
            float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            float distance = Mathf.Lerp(MoveSpeedMinZoom, MoveSpeedMaxZoom, _zoom) * damping * Time.deltaTime;

            Vector3 position = transform.localPosition;
            position += direction * distance;
            transform.localPosition = ClampPosition(position);
        }

        private void AdjustRotation(float delta)
        {
            _rotationAngle += delta * RotationSpeed * Time.deltaTime;

            if (_rotationAngle < 0F)
                _rotationAngle += 360F;

            if (_rotationAngle >= 360F)
                _rotationAngle -= 360F;

            transform.localRotation = Quaternion.Euler(0, _rotationAngle, 0);
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            float xMax = (Grid.ChunkCountX * HexMetrics.CHUNK_SIZE_X - 0.5F) * (2F * HexMetrics.INNER_RADIUS);
            position.x = Mathf.Clamp(position.x, 0F, xMax);

            float zMax = (Grid.ChunkCountZ * HexMetrics.CHUNK_SIZE_Z - 1) * (1.5F * HexMetrics.OUTER_RADIUS);
            position.z = Mathf.Clamp(position.z, 0F, zMax);

            return position;
        }

        #endregion

    }
    
}