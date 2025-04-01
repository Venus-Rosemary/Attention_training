using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private Vector3 screenPoint;//物体的屏幕坐标
    private Vector3 offset;//鼠标点击位置和拖拽物体中心偏移量
    private GameObject Object;//要拖拽的物体
    private bool isDrag;//判断是否正在拖拽
    void Update()
    {
        //按下左键开始发出射线
        if (Input.GetMouseButtonDown(0))
        {
            //相机发出射线射线鼠标
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //射线撞击点
            RaycastHit hit;
            //如果射线撞击到碰撞体，且碰撞体的标签是我们设置需要拖拽的物体，进行拖拽
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Drag")
            {
                Object = hit.collider.gameObject;
                MouseDown();
                isDrag = true;
            }
        }

        //一直按着左键表示拖拽过程
        if (Input.GetMouseButton(0))
        {
            if (isDrag)
            {
                MouseDrag();
            }
        }

        //松开左键，拖拽结束
        if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
        }
    }

    /// <summary>
    /// 鼠标点击
    /// </summary>
    /// <param name="game">要拖拽的物体</param>
    void MouseDown()
    {
        //记录下物体的屏幕坐标
        screenPoint = Camera.main.WorldToScreenPoint(Object.transform.position);
        //鼠标点中物体的时候计算出一个偏移量
        offset = Object.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    /// <summary>
    /// 鼠标拖拽
    /// </summary>
    /// <param name="game">要拖拽的物体</param>
    void MouseDrag()
    {
        //记录当前的鼠标位置，传入原先保存的物体屏幕坐标Z轴，保持物体Z轴不变
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        //将位置赋值给物体，加上偏移量就可以保证点击物体任何位置都可以进行拖拽
        Object.transform.position = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
    }

}

