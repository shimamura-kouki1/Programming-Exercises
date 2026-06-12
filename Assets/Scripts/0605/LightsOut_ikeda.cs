using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class Test : MonoBehaviour, IPointerClickHandler
{
    private GameObject[,] _cells;
    private int _turnCount = 0;
    private float _totalTime = 0;
    private bool _IsClear = false;
    [SerializeField] int _rowLength = 5;
    [SerializeField] int _columnLength = 5;
    [SerializeField]
    int _minTurn = 2;
    [SerializeField] Text _timerText;
    [SerializeField] Text _turnText;
    private void Start()
    {
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = _columnLength; //列数を固定
        _cells = new GameObject[_rowLength, _columnLength];
        for (var r = 0; r < _rowLength; r++)
        {
            for (var c = 0; c < _columnLength; c++)
            {
                var cell = new GameObject($"Cell({r}, {c})");
                cell.transform.parent = transform;
                cell.AddComponent<Image>();
                _cells[r, c] = cell;
            }
        }
        ResetGame();
    }

    private void Update()
    {
        if (!_IsClear)
        {
            _totalTime += Time.deltaTime;
            _timerText.text = _totalTime.ToString("F1");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_IsClear)
        {
            var cell = eventData.pointerCurrentRaycast.gameObject;
            var image = cell.GetComponent<Image>();
            GameObject prev = null;
            CellClick(cell);
            _turnCount++;
            if (CheckClear())
            {
                _IsClear = true;
                Debug.Log("クリア");
            }
            _turnText.text = _turnCount.ToString();
        }
        else
        {
            ResetGame();
        }
    }

    private void CellClick(GameObject cell)//セルがクリックされた時に呼ばれる関数
    {
        var image = cell.GetComponent<Image>();
        GameObject prev = null;
        ColorChange(image);
        if (FindCell(cell, 0, 1, out prev))//右方向にセルがあるかチェック
        {
            image = prev.GetComponent<Image>();
            ColorChange(image);
        }
        if (FindCell(cell, 0, -1, out prev))//左方向にセルがあるかチェック
        {
            image = prev.GetComponent<Image>();
            ColorChange(image);
        }
        if (FindCell(cell, 1, 0, out prev))//下方向にセルがあるかチェック
        {
            image = prev.GetComponent<Image>();
            ColorChange(image);
        }
        if (FindCell(cell, -1, 0, out prev))//上方向にセルがあるかチェック
        {
            image = prev.GetComponent<Image>();
            ColorChange(image);
        }
    }
    private bool FindCell(GameObject target, int deltaRow, int deltaColumn, out GameObject result)//クリックしたセルの周囲にセルがあるかチェックする関数
    {
        int targetRow = 0, targetCol = 0;
        for (var r = 0; r < _rowLength; r++)
        {
            for (var c = 0; c < _columnLength; c++)
            {
                if (_cells[r, c] == target)
                {
                    targetRow = r;
                    targetCol = c;
                }
            }
        }
        if (targetRow + deltaRow < _rowLength && targetRow + deltaRow >= 0 && targetCol + deltaColumn < _columnLength && targetCol + deltaColumn >= 0)
        {
            result = _cells[targetRow + deltaRow, targetCol + deltaColumn];
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    private void ColorChange(Image target)//セルの色を反転する関数
    {
        if (target.color == Color.white)
        {
            target.color = Color.black;
        }
        else
        {
            target.color = Color.white;
        }
    }

    private bool CheckClear()//クリアしているかチェックする関数
    {
        foreach (var cell in _cells)
        {
            var image = cell.GetComponent<Image>();
            if (image.color == Color.black)
            {
                return false;
            }
        }
        return true;
    }

    private void ResetGame()//ゲームを初期化するための関数
    {
        _IsClear = false;
        _turnCount = 0;
        _turnText.text = _turnCount.ToString();
        _totalTime = 0;
        _timerText.text = _totalTime.ToString("F1");//初期化処理

        foreach (var cell in _cells)
        {
            var image = cell.GetComponent<Image>();
            image.color = Color.white;
        }
        GameObject[] cheat = new GameObject[_minTurn];
        for (int i = 0; i < _minTurn; i++)
        {
            int choosedRow = Random.Range(0, _rowLength);
            int choosedCol = Random.Range(0, _columnLength);
            var choosedCell = _cells[choosedRow, choosedCol];
            cheat[i] = choosedCell;
            CellClick(choosedCell);
            if (CheckClear())
            {
                i = -1;
                continue;
            }
        }
        foreach (var cell in cheat)
        {
            Debug.Log(cell);
        }
    }
}
