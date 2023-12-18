using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFrutas : MonoBehaviour
{

    public GameObject[] Frutas;

    private bool canSpawn = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawn)
            StartCoroutine(SpawnCountdownRoutine());
    }

    #region METODOS

    private void SpawnFruta()
    {
        GameObject frutita = Frutas[Random.Range(0, 9)];
        Instantiate(frutita, GenerateSpawnPosition(), frutita.transform.rotation);
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-10, 10);
        float spawnPosZ = Random.Range(-7, 7);
        Vector3 randomPos = new Vector3(spawnPosX, 1, spawnPosZ);
        return randomPos;
    }

    #endregion

    IEnumerator SpawnCountdownRoutine()
    {
      //  canSpawn = false; //PARA QUE EN EL UPDATE NO ENTRE, ESPERE 2 SEGUNDOS Y LUEGO ENTRE
        yield return new WaitForSeconds(1);
      //  canSpawn = true;
        SpawnFruta();
    }
}
