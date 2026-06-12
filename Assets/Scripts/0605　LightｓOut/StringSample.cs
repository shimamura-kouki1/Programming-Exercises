using UnityEngine;

public class StringSample : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (char i = 'A'; i <= 'Z'; i++)
        {
            Debug.Log($"{i} = {(int)i}");
        }
    }

    //文字列とは文字の列のこと
    string str = "Hello, World!";
    // Update is called once per frame
    void Update()
    {
        
    }
}
