using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private Vector3 screenPoint;//�������Ļ����
    private Vector3 offset;//�����λ�ú���ק��������ƫ����
    private GameObject Object;//Ҫ��ק������
    private bool isDrag;//�ж��Ƿ�������ק
    void Update()
    {
        //���������ʼ��������
        if (Input.GetMouseButtonDown(0))
        {
            //������������������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //����ײ����
            RaycastHit hit;
            //�������ײ������ײ�壬����ײ��ı�ǩ������������Ҫ��ק�����壬������ק
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Drag")
            {
                Object = hit.collider.gameObject;
                MouseDown();
                isDrag = true;
            }
        }

        //һֱ���������ʾ��ק����
        if (Input.GetMouseButton(0))
        {
            if (isDrag)
            {
                MouseDrag();
            }
        }

        //�ɿ��������ק����
        if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
        }
    }

    /// <summary>
    /// �����
    /// </summary>
    /// <param name="game">Ҫ��ק������</param>
    void MouseDown()
    {
        //��¼���������Ļ����
        screenPoint = Camera.main.WorldToScreenPoint(Object.transform.position);
        //�����������ʱ������һ��ƫ����
        offset = Object.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    /// <summary>
    /// �����ק
    /// </summary>
    /// <param name="game">Ҫ��ק������</param>
    void MouseDrag()
    {
        //��¼��ǰ�����λ�ã�����ԭ�ȱ����������Ļ����Z�ᣬ��������Z�᲻��
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        //��λ�ø�ֵ�����壬����ƫ�����Ϳ��Ա�֤��������κ�λ�ö����Խ�����ק
        Object.transform.position = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
    }

}

