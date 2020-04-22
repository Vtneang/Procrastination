using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public float y_mod = 2f;
    public float x_mod = 2f;
    public GameObject[] Enemies;
    public float SpawnDelay;
    public float InitialDelay = 0f;

    private Vector2 pos;
    private int num;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        StartCoroutine(Spawning());
        num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
    }

    //Helps infinelty spawn enemies in positions randomly after a set time
    IEnumerator Spawning()
    {
        while (true)
        {
            if (num == 0)
            {
                yield return new WaitForSeconds(InitialDelay);
                num += 1;
            }
            float xmin = pos.x - x_mod;
            float xmax = pos.x + x_mod;
            float ymin = pos.y - y_mod;
            float ymax = pos.y + y_mod;
            Vector3 randopos = new Vector3(Random.Range(xmin, xmax), Random.Range(ymin, ymax), 0);
            Instantiate(Enemies[Random.Range(0, Enemies.Length)], randopos, Quaternion.identity);
            yield return new WaitForSeconds(SpawnDelay);
        }
    }
}
