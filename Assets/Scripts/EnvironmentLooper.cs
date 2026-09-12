using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentLooper : MonoBehaviour
{
    public Transform player;
    public Transform env1; // Tấm 1 (Pos X nhỏ hơn)
    public Transform env2; // Tấm 2 (Pos X lớn hơn, đè 1 phần lên Tấm 1)

    private float offset;       // Khoảng cách giữa 2 tấm
    private float nextTriggerX; // Ngưỡng X của Player để kích hoạt nhảy
    private bool isEnv2Ahead;   // Trạng thái tấm nào đang ở phía trước

    void Start()
    {
        if (env1 == null || env2 == null || player == null) return;

        // Tính độ lệch X gốc giữ nguyên phần đè/chồng nhau
        offset = env2.position.x - env1.position.x;

        isEnv2Ahead = true;

        // Đặt mốc kích hoạt nhảy đầu tiên: 
        // Phải là vị trí hiện tại của Player CỘNG THÊM khoảng cách offset
        // Đảm bảo không bị nhảy ngay frame đầu tiên
        nextTriggerX = player.position.x + offset;
    }

    void Update()
    {
        if (player == null || env1 == null || env2 == null) return;

        // Chỉ di chuyển khi Player di chuyển vượt qua mốc nextTriggerX
        if (player.position.x >= nextTriggerX)
        {
            if (isEnv2Ahead)
            {
                // Dịch Tấm 1 (đằng sau) lên trước Tấm 2
                Vector3 newPos = env1.position;
                newPos.x = env2.position.x + offset;
                env1.position = newPos;

                // Cập nhật mốc kích hoạt tiếp theo
                nextTriggerX += offset;
                isEnv2Ahead = false;
            }
            else
            {
                // Dịch Tấm 2 (đằng sau) lên trước Tấm 1
                Vector3 newPos = env2.position;
                newPos.x = env1.position.x + offset;
                env2.position = newPos;

                // Cập nhật mốc kích hoạt tiếp theo
                nextTriggerX += offset;
                isEnv2Ahead = true;
            }
        }
    }
}