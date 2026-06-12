using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class LightsOut_kawakubo : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 5;
    private List<GameObject> cells = new();

    //周囲のセルの相対座標が入ってる配列
    private Vector2Int[] aroundVector = new Vector2Int[5]
    {
        new (0,1), // 上
        new (-1,0), new (0,0), new (1,0),//左,中央,右
        new (0,-1), // 下
    };

    private int steps = 0;
    private bool isClear = false;
    private void Start()
    {
        List<int> canChengeCellNum = new List<int>();
        string answer = string.Empty;
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < columns; c++)
            {
                cells.Add(new GameObject($"Cell({r}, {c})"));
                cells[r * columns + c].transform.parent = transform;
                cells[r * columns + c].AddComponent<Image>().color = Color.white;
                canChengeCellNum.Add(r * columns + c);
                answer += "■";
            }
            answer += "\n";
        }
        GetComponent<GridLayoutGroup>().constraintCount = columns;

        int cellChenge = Random.Range(2, cells.Count);
        for (int i = 0; i < cellChenge; i++)
        {
            int changeCellNum = Random.Range(0, canChengeCellNum.Count);
            ChangeColor(canChengeCellNum[changeCellNum]);
            answer = Replace(answer, canChengeCellNum[changeCellNum] + canChengeCellNum[changeCellNum] / columns, 1, "●");
            canChengeCellNum.RemoveAt(changeCellNum);
        }
        Debug.Log(answer.Insert(0, "answer:\n"));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClear) { return; }
        var cell = eventData.pointerCurrentRaycast.gameObject;
        var image = cell.GetComponent<Image>();
        int index = cells.FindIndex(c => c == cell);
        if (image != null)
        {
            steps++;
            ChangeColor(index);
        }

        if (IsCleared(Color.white) || IsCleared(color: Color.black))
        {
            Debug.Log($"Clear\ntime:{Time.realtimeSinceStartup},step:{steps}");
            isClear = true;
        }
    }

    private void ChangeColor(int index)
    {
        for (int i = 0; i < aroundVector.Length; i++)
        {
            var aroundIndex = index + (aroundVector[i].y * columns + aroundVector[i].x);
            // 周囲のセルのインデックスが範囲外の場合はスキップ
            if (aroundIndex < 0 || aroundIndex >= cells.Count ||
                aroundIndex < (index / columns + aroundVector[i].y) * columns ||//周囲のセルが現在のセルと同じ行にない場合はスキップ
                aroundIndex >= ((index / columns + aroundVector[i].y + 1) * columns)) { continue; }//

            var aroundImage = cells[aroundIndex].GetComponent<Image>();
            if (aroundImage != null)
            {
                aroundImage.color = aroundImage.color == Color.white ? Color.black : Color.white;
            }
        }
    }

    bool IsCleared(Color color)
    {
        foreach (var cell in cells)
        {
            if (cell.GetComponent<Image>().color != color) { return false; }
        }
        return true;
    }

    string Replace(string str, int start, int length, string newstr)
    {
        return str.Substring(0, start) + newstr + str.Substring(start + length);
    }
}
