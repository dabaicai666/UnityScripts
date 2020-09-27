using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCheck : MonoBehaviour
{
    private int Maxnum = 10;            //内存中对象的最大数量
    private int CHECKTIME = 8;          //内存检测的间隔时间
    private Hashtable pool;             //对象池字典
    private ArrayList indexList;        //对象存放顺序集合
    public GameObject prefab;           
    void Start()
    {
        pool = new Hashtable();
        indexList = new ArrayList();
        StartCoroutine("CheckPool");
  
    }
    //增加内存对象数量
    void Additem(string name,GameObject obj) {
        pool[name] = obj;
        indexList.Add(name);
    }
    void Update()
    {
        
    }
    //检测内存
    IEnumerator CheckPool() {
        while (true) {
            yield return new WaitForSeconds(CHECKTIME);
            int num = pool.Count;
            int a = pool.Count;
            Debug.Log("数量" + num + indexList.Count);
            if (num > Maxnum) {
                for (int i = 0; i < a; i++)
                {                   
                    string str = indexList[0].ToString();
                    indexList.RemoveAt(0);
                    Destroy((GameObject)pool[str]);
                    pool.Remove(str);
                    Debug.Log(pool.Count.ToString());
                    num--;
                    Debug.Log(num.ToString());
                    if (num <= Maxnum) {
                        Debug.Log("jinru" + num);
                        break;
                    };                     
                }
            }
        }
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,100),"")) {
            prefab.transform.position = new Vector3(UnityEngine.Random.Range(-10, 10), 0, 0);
            GameObject OBJ = Instantiate(prefab);
            int a =UnityEngine.Random.Range(0,6);
            switch (a) {
                case 0:
                    OBJ.GetComponent<Rigidbody>().AddForce(Vector3.forward * 500f);
                    break;
                case 1:
                    OBJ.GetComponent<Rigidbody>().AddForce(Vector3.up * 500f);
                    break;
                case 2:
                    OBJ.GetComponent<Rigidbody>().AddForce(Vector3.back * 500f);
                    break;
                case 3:
                    OBJ.GetComponent<Rigidbody>().AddForce(Vector3.left * 500f);
                    break;
                case 4:
                    OBJ.GetComponent<Rigidbody>().AddForce(Vector3.right * 500f);
                    break;
                case 5:
                    OBJ.GetComponent<Rigidbody>().AddForce(Vector3.down * 500f);
                    break;                  
            }           
            Additem(GetCurTime(),OBJ);
        }       
    }
    private static string GetCurTime()
    {
        return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()
            + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond;
    }
}
