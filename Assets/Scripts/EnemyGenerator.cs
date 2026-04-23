using System.Collections;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] GameObject prefabEnemy;
    [SerializeField] float timeBetweenShips = 1f;
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Generate());    
    }

    IEnumerator Generate()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenShips);
            Vector3 position = Vector3.Lerp(top.position, bottom.position, Random.Range(0f, 1f));
            Instantiate(prefabEnemy, position, Quaternion.identity);
        }

    }
}
