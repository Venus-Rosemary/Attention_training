using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("角色移动速度")]
    public float moveSpeed = 5f;
    
    [Header("移动范围限制")]
    [Tooltip("X轴最小值")]
    public float minX = -10f;
    [Tooltip("X轴最大值")]
    public float maxX = 10f;
    [Tooltip("Y轴最小值")]
    public float minY = -10f;
    [Tooltip("Y轴最大值")]
    public float maxY = 10f;

    // 目标位置
    public Vector3 targetPosition;

    //玩家初始位置
    public Vector3 delauftPos;
    // 是否正在移动
    private bool isMoving = false;

    private void Start()
    {
        targetPosition = transform.position;
        delauftPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 获取鼠标点击的屏幕位置
            Vector3 mouseScreenPosition = Input.mousePosition;
            
            // 将鼠标屏幕位置转换为世界坐标
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 
                -Camera.main.transform.position.z + transform.position.z));
            
            // 限制目标位置在指定范围内
            worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
            worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);
            worldPosition.z = transform.position.z;
            
            targetPosition = worldPosition;
            isMoving = true;
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            CollectItem(other.gameObject);
        }
    }
    
    private void CollectItem(GameObject item)
    {
        
        CollectibleManager manager = FindObjectOfType<CollectibleManager>();
        if (manager != null)
        {
            manager.RemoveCollectible(item);
        }

        Destroy(item);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}