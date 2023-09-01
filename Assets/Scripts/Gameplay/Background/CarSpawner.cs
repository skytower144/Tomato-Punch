using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] endPoints;
    [SerializeField] private GameObject[] carPrefabs;

    [SerializeField] private bool flip_car;
    private float spawnWait;
    private bool start_spawn_cars;
    private WaitForSeconds _spawnDelay;

    void OnEnable()
    {
        start_spawn_cars = true;
        StartCoroutine(SpawnCar());
        _spawnDelay = new WaitForSeconds(spawnWait);
    }

    private IEnumerator SpawnCar()
    {
        while (start_spawn_cars)
        {
            spawnWait = Random.Range(1.5f, 3f);

            int randCar = Random.Range(0, carPrefabs.Length);
            //int randSpawnPoint = Random.Range(0, spawnPoints.Length);

            GameObject car = Instantiate(carPrefabs[randCar], spawnPoints[0].position, transform.rotation);
            car.transform.SetParent(spawnOrigin);

            car.transform.localPosition = new Vector3(car.transform.localPosition.x, car.transform.localPosition.y, 0);
            car.GetComponent<CarMove>().target = endPoints[0];

            if (flip_car)
                car.transform.Rotate(new Vector3(0, 180, 0));

            yield return _spawnDelay;
        }
    }

}
