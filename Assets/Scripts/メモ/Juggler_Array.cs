using UnityEngine;

public class Juggler_Array : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //複数の値をまとめるのが配列である。
        int[] line1 = { 1, 2, 3, 4, 5 };
        int[] line2 = { 6, 7, 8, 9, 10 };
        int[] line3 = { 12, 13, 14, 15, 16 };

        //複数の配列をまとめるの配列でいいのではないか？
        int[][] data = new int[3][];//配列の配列を宣言する
        data[0] = line1;
        data[1] = line2;
        data[2] = line3;

        var lineX = data[0]; // 要素として line1 を取り出す
        Debug.Log(lineX[2]); // line1 の 2 番目の要素を取り出す

        // 上記のようなアクセスが面倒なので、一度に[][]を繋げてアクセスできる
        Debug.Log(data[1][2]); // data の 1 番目の要素（line2）の 2 番目の要素を取り出す
        //ジャグ配列は一つ目の要素である配列を取り出して、2つ目の要素を取り出すため、アクセス数が多い
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
