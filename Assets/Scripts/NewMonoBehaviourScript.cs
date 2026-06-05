using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    int[] score = new int[3];

    //void Start()
    //{
    //    int x = 5;
    //    float y = 5;
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}


    private GameObject[] _cubes;

    private void Start()
    {
        _cubes = new GameObject[5];

        for (int i = 0; i < _cubes.Length; i++)
        {
            _cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _cubes[i].transform.position = new Vector3(i * 2f, 0f, 0f);
        }
    }

    private void Update()
    {
        foreach (GameObject cube in _cubes)
        {
            cube.transform.Rotate(0f, 90f * Time.deltaTime, 0f);
        }
    }
}
