using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEnemies : MonoBehaviour
{
    public List<GameObject> enemies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 GetEnemie(Vector3 pos)
    {
        for(int i = 0; i < enemies.Count;i++)
        {
            if(enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
        }
        Vector3 position =  Vector3.zero;
        float distance = 100;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.Distance(enemies[i].transform.position,pos) < distance)
            {
                distance = Vector3.Distance(enemies[i].transform.position, pos);
                position = new Vector3(enemies[i].transform.position.x, pos.y, enemies[i].transform.position.z);
            }

        }



        return position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(!enemies.Contains(other.gameObject))
            {
                enemies.Add(other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemies.Contains(other.gameObject))
            {
                enemies.Remove(other.gameObject);
            }

        }
    }
}
