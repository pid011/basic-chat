using BasicChat;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Class1 cls = new Class1();
        var a = cls.Test();
        Debug.Log(a);
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
