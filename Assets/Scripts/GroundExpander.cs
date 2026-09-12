using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterExpander : MonoBehaviour
{
    public Transform playerTransform;
    public float forwardOffset = 50f; // Luôn vượt trước Player 50 đơn vị

    private float startX; // Vị trí mép sau cố định của Water
    private float planeUnitSize = 10f; // 1 đơn vị Scale.x của Mesh Plane tương ứng 10 units trong World Space

    void Start()
    {
        // Tính vị trí mép sau cố định của Water dựa trên Position và Scale ban đầu
        startX = transform.position.x - (transform.localScale.x * planeUnitSize / 2f);
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Tính điểm đầu mới phía trước mặt Player
        float targetFrontX = playerTransform.position.x + forwardOffset;

        // Chỉ kéo dài khi Player đi vượt quá chiều dài hiện tại
        if (targetFrontX > startX + (transform.localScale.x * planeUnitSize))
        {
            // 1. Tính tổng chiều dài mới từ điểm bắt đầu đến targetFrontX
            float newTotalLength = targetFrontX - startX;

            // 2. Cập nhật Scale.x mới
            float newScaleX = newTotalLength / planeUnitSize;
            transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);

            // 3. Dời tâm Position.x về chính giữa để mép sau (startX) giữ nguyên cố định
            float newCenterX = startX + (newTotalLength / 2f);
            transform.position = new Vector3(newCenterX, transform.position.y, transform.position.z);
        }
    }
}