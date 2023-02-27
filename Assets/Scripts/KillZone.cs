using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    [SerializeField] private int Respawn;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(LoadingScene());
        }
    }

    private IEnumerator LoadingScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(Respawn);
    }
}
