using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager levelManager;

    public static ILevel ActiveLevel { get; private set; }

    private void Awake()
    {
        if(levelManager == null)
        {
            levelManager = this;
        } else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void SetActiveLevel(ILevel level)
    {
        Debug.Log($"Setting {level.LevelData.LevelName} as Active Level");
        ActiveLevel = level;
    }

}
