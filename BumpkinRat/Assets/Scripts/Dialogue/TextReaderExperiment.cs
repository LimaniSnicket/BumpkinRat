using System.IO;
using UnityEngine;

public class TextReaderExperiment : MonoBehaviour
{
    public string testVariable;

    public string txtPath;

    public string output;
    // Start is called before the first frame update
    void Start()
    {
        string readFile = File.ReadAllText(txtPath);
        output = readFile;
    }

}
