using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFollowPlayer : MonoBehaviour
{
    public Transform playerTransform;
    public float forwardOffset = 50f; // Khoảng cách đi trước Player

    void Update()
    {
        if (playerTransform == null) return;

        // Cập nhật position X chạy theo Player, giữ nguyên Y và Z
        Vector3 newPosition = transform.position;
        newPosition.x = playerTransform.position.x + forwardOffset;
        
        transform.position = newPosition;
    }
}