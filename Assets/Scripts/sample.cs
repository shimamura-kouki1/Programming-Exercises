using UnityEngine;

public class sample : MonoBehaviour
{
    int[,] _matrix = new int[3, 3];
    void Start()
    {
        int[,] _matrix = new int[3, 3]
        {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9}
        };//これタワーディフェンスゲームのマップのデータとかに使えるかも。1が道、0が壁みたいな感じで。

        //2次元配列
        for (int i = 0; i < _matrix.GetLength(0); i++)
        {
            for (int j = 0; j < _matrix.GetLength(1); j++)
            {
                //一次元配列のインデックスは
                Debug.Log(_matrix[i, j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

//インデックスを変えたいとかならfor文を使えばいいが、時にないならforeach文のほうが簡潔でわかりやすいコードになることもある。

//foreachはGetEnumeratorを持っていれば処理できる。これは次の要素に処理を移すことをしている。