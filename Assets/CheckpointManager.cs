using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Checkpoint currentCheckpoint;
    private PlayerController player;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if(currentCheckpoint == null)
        {
            currentCheckpoint = checkpoint;
            return;
        }

        if(currentCheckpoint.priority < checkpoint.priority)
        {
            currentCheckpoint = checkpoint;
        }
    }

    public void SpawnAtLastCheckpoint()
    {
        if (currentCheckpoint == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        else
        {
            player.transform.position = currentCheckpoint.GetSpawnPoint();
        }
    }
}
