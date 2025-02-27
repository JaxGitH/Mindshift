﻿using Mindshift;
using UnityEngine;

namespace Mindshift
{
    public class B_Draggable : B_DragAndDrop
    {
        [Header("Draggable Settings")]
        [SerializeField] private bool isResettable = true;
        private Rigidbody rb;
        public bool isDragging = false;
        private Vector3 offset;
        private Collider draggableCollider;

        protected override void Start()
        {
            rb = GetComponent<Rigidbody>();
            draggableCollider = GetComponent<Collider>();

            if (rb == null)
            {
                Debug.LogError($"B_Draggable: No Rigidbody found on {gameObject.name}");
                enabled = false;
            }
        }

        protected override void Update()
        {
            if (isDragging)
            {
                DragObject();
            }
        }

        private void OnMouseDown()
        {
            if (!isPlayerContact)
            {
                StartDragging();
            }
        }

        private void OnMouseUp()
        {
            StopDragging();
        }

        private void StartDragging()
        {
            if (GameStateManager.Instance.IsMovementInputActive()) return;

            if (rb != null)
            {
                isDragging = true;
                GameStateManager.Instance.SetDraggingState(true);
                rb.isKinematic = true;

                if (draggableCollider != null)
                {
                    draggableCollider.enabled = false;
                }

                offset = transform.position - GetMouseWorldPosition();
            }
        }

        private void StopDragging()
        {
            if (rb != null)
            {
                isDragging = false;
                GameStateManager.Instance.SetDraggingState(false);
                rb.isKinematic = false;

                if (draggableCollider != null)
                {
                    draggableCollider.enabled = true;
                }
            }
        }

        private void DragObject()
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            return Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }
}
