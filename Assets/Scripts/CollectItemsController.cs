using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItemsController : MonoBehaviour
{
    public int index;
    public bool isRight;
    
    // 移动相关参数
    public bool canMove = false;
    public float moveSpeed = 3f;
    private Vector2 moveDirection;
    
    // 边界限制
    public float minX = -9f;
    public float maxX = 9f;
    public float minY = -4f;
    public float maxY = 4f;
    
    // 碰撞检测层
    public LayerMask collectibleLayer;
    
    void Start()
    {
        // 初始化随机移动方向
        moveDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        if (canMove)
        {
            Move();
        }
    }
    
    void Move()
    {
        // 计算新位置
        Vector3 newPosition = transform.position + new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.deltaTime;
        
        if (newPosition.x < minX || newPosition.x > maxX)
        {
            moveDirection.x = -moveDirection.x;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        }
        
        if (newPosition.y < minY || newPosition.y > maxY)
        {
            moveDirection.y = -moveDirection.y;
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }
        

        transform.position = newPosition;
        
        // 检测与其他收集物的碰撞
        CheckCollisions();
    }
    
    void CheckCollisions()
    {
        // 检测周围的收集物
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.6f, collectibleLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {

                Vector2 direction = (transform.position - collider.transform.position).normalized;
                moveDirection = direction;
                break;
            }
        }
    }
    
    // 可视化碰撞范围（仅在编辑器中）
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}
