using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleManager : MonoBehaviour
{
    [Header("收集物品设置")]
    [Tooltip("收集物品预制体")]
    public GameObject collectiblePrefab;
    
    [Tooltip("最小生成数量")]
    public int minSpawnCount = 3;
    
    [Tooltip("最大生成数量")]
    public int maxSpawnCount = 6;
    
    [Header("引导设置")]
    [Tooltip("引导物体预制体")]
    public GameObject guidePrefab;
    
    [Tooltip("引导物体移动速度")]
    public float guideMoveSpeed = 5f;
    
    [Header("生成区域设置")]
    [Tooltip("X轴最小值")]
    public float minX = -9f;
    
    [Tooltip("X轴最大值")]
    public float maxX = 9f;
    
    [Tooltip("Y轴最小值")]
    public float minY = -4f;
    
    [Tooltip("Y轴最大值")]
    public float maxY = 4f;
    
    [Header("游戏时间设置")]
    [Tooltip("游戏总时间(秒)")]
    public float gameTime = 180f;
    
    [Tooltip("时间显示文本")]
    public TMP_Text timeText;
    
    [Tooltip("结束提示文本")]
    public TMP_Text gameOverText;

    // 当前生成的收集物品列表
    private List<GameObject> currentCollectibles = new List<GameObject>();

    // 剩余游戏时间
    private float remainingTime;

    // 游戏是否结束
    private bool isGameOver = false;

    // 当前应该收集的物品索引
    private int currentCollectIndex = 1;
    
    // 引导物体
    private GameObject guideObject;
    
    // 是否正在引导
    private bool isGuiding = false;
    
    // 玩家控制器引用
    private PlayerController playerController;

    private bool isAllCorrect = false;
    // 玩家得分
    private int score = 0;
    
    [Header("UI设置")]
    [Tooltip("得分显示文本")]
    public TMP_Text scoreText;

    public GameObject endUI;

    [Header("关卡设置")]
    [Tooltip("设置不同关卡的物体生成和得分逻辑")]
    public bool isSecondPass = false;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.gameObject.SetActive(false);
        remainingTime = gameTime;
        isGameOver = true;
    }

    void GameInitialize()
    {
        remainingTime = gameTime;

        score = 0;

        isGameOver = true;

        isGuiding = false;

        isAllCorrect = false;

        playerController.transform.position = playerController.delauftPos;

        playerController.enabled = true;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        UpdateScoreUI();
    }

    public void StartGame(bool isPass)
    {
        GameInitialize();
        isGameOver = false;
        playerController.gameObject.SetActive(true);
        isSecondPass = isPass;
        if (isSecondPass)
        {
            SpawnCollectiblesSecond();
        }
        else
        {
            SpawnCollectibles();
        }
    }

    void Update()
    {
        if (isGameOver)
            return;
            
        remainingTime -= Time.deltaTime;
        
        // 更新UI显示
        UpdateTimeUI();
        
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            UpdateTimeUI();
            EndGame();
            return;
        }

        if (isSecondPass)
        {
            if (currentCollectibles.Count != 0 && !CheckIfAnyIsTrue())
            {
                foreach (GameObject collectible in currentCollectibles)
                {
                    if (collectible != null)
                    {
                        Destroy(collectible);
                    }
                }
                if (isAllCorrect)
                {
                    score += 10;
                    UpdateScoreUI();
                }
                SpawnCollectiblesSecond();
            }
        }
        else
        {
            if (currentCollectibles.Count == 0 && !isGuiding)
            {
                if (isAllCorrect)
                {
                    score += 10;
                    UpdateScoreUI();
                }
                SpawnCollectibles();
            }
        }

    }

    //第二关检测list中是否还存在isRight为true的物体
    public bool CheckIfAnyIsTrue()
    {
        foreach (var item in currentCollectibles)
        {
            CollectItemsController cc = item.GetComponent<CollectItemsController>();
            if (cc!=null && cc.isRight)
            {
                return true;
            }
        }
        return false;
    }

    #region 第一关生成
    void SpawnCollectibles()
    {
        isAllCorrect = true;

        currentCollectibles.Clear();
        
        currentCollectIndex = 1;
        
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
        
        List<Vector3> spawnPositions = new List<Vector3>();
        
        for (int i = 0; i < spawnCount; i++)
        {
            // 随机位置
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX),Random.Range(minY, maxY),0);

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            float distance = 0;
            bool tooCloseToOthers = false;
            do
            {
                tooCloseToOthers = false;
                distance = Vector3.Distance(spawnPosition, player.transform.position);
                foreach (Vector3 existingPos in spawnPositions)
                {
                    if (Vector3.Distance(spawnPosition, existingPos) < 1f)
                    {
                        tooCloseToOthers = true;
                    }
                }
                if (distance>=3f && !tooCloseToOthers)
                {
                    break;
                }
                if (distance < 3f)
                {
                    spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
                }
                else if (tooCloseToOthers)
                {
                    spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
                }
            } while (true);


            spawnPositions.Add(spawnPosition);
            
            GameObject collectible = Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);
            
            currentCollectibles.Add(collectible);
            
            
            CollectItemsController indexController = collectible.GetComponent<CollectItemsController>();
            if (indexController == null)
            {
                indexController = collectible.AddComponent<CollectItemsController>();
            }
            indexController.index = i + 1;
            
            //TMP_Text indexText = collectible.GetComponentInChildren<TMP_Text>();
            //if (indexText != null)
            //{
            //    indexText.text = (i + 1).ToString();
            //}
        }
        
        // 禁用玩家控制
        if (playerController != null)
        {
            playerController.targetPosition = playerController.transform.position;
            playerController.enabled = false;
        }
        
        StartCoroutine(StartGuideProcess(spawnPositions));
    }
    
    // 引导过程协程
    private IEnumerator StartGuideProcess(List<Vector3> positions)
    {
        isGuiding = true;
        

        Vector3 playerPosition = playerController.transform.position;
        guideObject = Instantiate(guidePrefab, playerPosition, Quaternion.identity);
        
        // 按顺序移动到每个收集物位置
        foreach (Vector3 position in positions)
        {
            while (Vector3.Distance(guideObject.transform.position, position) > 0.1f)
            {
                guideObject.transform.position = Vector3.MoveTowards(
                    guideObject.transform.position, 
                    position, 
                    guideMoveSpeed * Time.deltaTime
                );
                yield return null;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        
        Destroy(guideObject);
        guideObject = null;
        
        // 启用玩家控制
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        isGuiding = false;
    }
    #endregion

    #region 第二关生成
    void SpawnCollectiblesSecond()
    {
        isAllCorrect = true;

        currentCollectibles.Clear();

        int correctCount = Random.Range(2, 5);

        List<Vector3> spawnPositions = new List<Vector3>();

        // 生成正确的物体
        for (int i = 0; i < correctCount; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
            spawnPositions.Add(spawnPosition);

            GameObject collectible = Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);

            collectible.GetComponent<Collider>().isTrigger = false;

            currentCollectibles.Add(collectible);

            CollectItemsController indexController = collectible.GetComponent<CollectItemsController>();
            if (indexController == null)
            {
                indexController = collectible.AddComponent<CollectItemsController>();
            }
            indexController.isRight = true;

            indexController.canMove = true;
            indexController.moveSpeed = Random.Range(2f, 4f);
            indexController.minX = minX;
            indexController.maxX = maxX;
            indexController.minY = minY;
            indexController.maxY = maxY;
        }

        if (playerController != null)
        {
            playerController.targetPosition = playerController.transform.position;
            playerController.enabled = false;
        }

        StartCoroutine(SpawnWrongCollectibles());
    }

    // 生成错误的物体
    IEnumerator SpawnWrongCollectibles()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 6; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);

            GameObject collectible = Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);

            collectible.GetComponent<Collider>().isTrigger = false;

            currentCollectibles.Add(collectible);

            CollectItemsController indexController = collectible.GetComponent<CollectItemsController>();
            if (indexController == null)
            {
                indexController = collectible.AddComponent<CollectItemsController>();
            }
            indexController.isRight = false;

            indexController.canMove = true;
            indexController.moveSpeed = Random.Range(2f, 4f);
            indexController.minX = minX;
            indexController.maxX = maxX;
            indexController.minY = minY;
            indexController.maxY = maxY;
        }

        StartCoroutine(StopAllMovementAndEnablePlayer());
    }

    // 停止所有物体移动并启用玩家控制
    IEnumerator StopAllMovementAndEnablePlayer()
    {
        yield return new WaitForSeconds(3f);

        foreach (GameObject collectible in currentCollectibles)
        {
            if (collectible != null)
            {
                collectible.GetComponent<Collider>().isTrigger = true;
                CollectItemsController controller = collectible.GetComponent<CollectItemsController>();
                if (controller != null)
                {
                    controller.canMove = false;
                }
            }
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isGuiding = false;
    }
    #endregion

    #region 移除物体操作
    // 从列表中移除收集物品
    public void RemoveCollectible(GameObject collectible)
    {
        if (currentCollectibles.Contains(collectible))
        {
            if (isSecondPass)
            {
                // 第二关模式：只有收集正确的物体才得分
                CollectItemsController indexController = collectible.GetComponent<CollectItemsController>();
                if (indexController != null)
                {
                    if (indexController.isRight)
                    {
                        Debug.Log("收集到正确的物体！");
                    }
                    else
                    {
                        isAllCorrect = false;
                        Debug.Log("收集到错误的物体！");
                    }
                }
            }
            else
            {
                // 第一关模式：按顺序收集
                CollectItemsController indexController = collectible.GetComponent<CollectItemsController>();
                if (indexController != null)
                {
                    if (indexController.index == currentCollectIndex)
                    {
                        Debug.Log("收集到正确顺序的物体！");
                        currentCollectIndex++;
                    }
                    else
                    {
                        Debug.Log("收集到错误顺序的物体！");
                        isAllCorrect = false;
                    }
                }
            }

            currentCollectibles.Remove(collectible);
        }
    }
    #endregion

    #region UI更新
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "得分: " + score;
        }
    }
    

    void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timeText.text = string.Format("时间: {0:00}:{1:00}", minutes, seconds);
        }
    }
    #endregion

    // 结束游戏
    void EndGame()
    {
        isGameOver = true;

        playerController.gameObject.SetActive(false);

        endUI.SetActive(true);

        StopAllCoroutines();

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }

        if (guideObject != null)
        {
            Destroy(guideObject);
        }

        foreach (GameObject collectible in currentCollectibles)
        {
            if (collectible != null)
            {
                Destroy(collectible);
            }
        }
        
        currentCollectibles.Clear();
    }

    public void ExitClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region Scene窗口绘制范围区域
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
    #endregion
}