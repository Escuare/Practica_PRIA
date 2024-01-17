using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        if (canSpawn && GameObject.FindGameObjectsWithTag("Fruta").Length < 5)
            StartCoroutine(SpawnCountdownRoutine());
    }

    #region METODOS

    private void SpawnFruta()
    {
        GameObject frutita = Frutas[Random.Range(0, 9)];
        Vector3 randomPos = GenerateSpawnPosition();

        if (PosicionOcupada(randomPos))
        {
            Debug.Log("Posición ocupada");
            SpawnFruta();
        } else
        {
            Instantiate(frutita, randomPos, frutita.transform.rotation);
        }
     
    }

    private Vector3 GenerateSpawnPosition()
    {

        Vector3 randomPos; 
        float spawnPosX = Random.Range(-10, 10);
        float spawnPosZ = Random.Range(-7, 7);
        randomPos = new Vector3(spawnPosX, 1, spawnPosZ);

        return randomPos;
    }

    private bool PosicionOcupada(Vector3 randomSpawnPos)
    {
        float radius = 1.5f;

        Collider[] colliders = Physics.OverlapSphere(randomSpawnPos, radius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Fruta") || collider.CompareTag("Objetivo"))
            {
                return true; //POSICIÓN OCUPADAS
            }
        }

        return false;
    }

    #endregion

    IEnumerator SpawnCountdownRoutine()
    {
        canSpawn = false; //PARA QUE EN EL UPDATE NO ENTRE, ESPERE Y LUEGO ENTRE
        yield return new WaitForSeconds(1);
        canSpawn = true;
        SpawnFruta();
    }
}
