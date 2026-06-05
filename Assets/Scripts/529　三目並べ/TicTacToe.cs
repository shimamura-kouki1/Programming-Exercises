using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    [SerializeField]
    private int _waitTime = 2;
    private const int Size = 3;

    private Image[,] _cells;
    private CellState[,] _cellStates;

    [SerializeField]
    private Color _normalCell = Color.white;

    [SerializeField]
    private Color _selectedCell = Color.cyan;

    //現在選択されているセルの行と列
    private int _selectedRow;
    private int _selectedColumn;

    [SerializeField]
    private Sprite _circle = null;

    [SerializeField]
    private Sprite _cross = null;

    private bool _changeTurn = true;
    private bool _AIStop = false;

    private void Start()
    {
        _cellStates = new CellState[Size, Size];
        _cells = new Image[Size, Size];
        for (var r = 0; r < _cells.GetLength(0); r++)
        {
            for (var c = 0; c < _cells.GetLength(1); c++)
            {
                var obj = new GameObject($"Cell({r},{c})");
                obj.transform.parent = transform;
                var cell = obj.AddComponent<Image>();
                _cells[r, c] = cell;
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) { _selectedColumn--; }
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) { _selectedColumn++; }
        if (Keyboard.current.upArrowKey.wasPressedThisFrame) { _selectedRow--; }
        if (Keyboard.current.downArrowKey.wasPressedThisFrame) { _selectedRow++; }

        if (_selectedColumn < 0) { _selectedColumn = 0; }
        if (_selectedColumn >= Size) { _selectedColumn = Size - 1; }
        if (_selectedRow < 0) { _selectedRow = 0; }
        if (_selectedRow >= Size) { _selectedRow = Size - 1; }

        for (var r = 0; r < _cells.GetLength(0); r++)
        {
            for (var c = 0; c < _cells.GetLength(1); c++)
            {
                var cell = _cells[r, c];
                cell.color =
                    (r == _selectedRow && c == _selectedColumn)
                    ? _selectedCell : _normalCell;
            }
        }

        if(!_changeTurn)
        {
            AITurn();
            _changeTurn = true;

            if (CheckWin(CellState.Cross))
            {
                Debug.Log("AIの勝ち！");
                StartCoroutine(RestartGame());
                return;
            }
        }

        ///スペースキーが押されたときの処理
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var cell = _cells[_selectedRow, _selectedColumn];

            if (_cellStates[_selectedRow, _selectedColumn] == CellState.None)
            {
                if (_changeTurn)
                {
                    cell.sprite = _circle;
                    _cellStates[_selectedRow, _selectedColumn] = CellState.Circle;
                    _changeTurn = false;

                    //勝敗判定
                    if (CheckWin(CellState.Circle))
                    {
                        _AIStop = true;
                        Debug.Log("プレイヤーの勝ち！");
                        StartCoroutine(RestartGame());
                    }

                    if (CheckDraw())
                    {
                        Debug.Log("引き分け！");
                        StartCoroutine(RestartGame());
                    }
                }

            }

        }
    }

    /// <summary>
    /// 勝利判定
    /// </summary>
    private bool CheckWin(CellState state)
    {
        for (var r = 0; r < Size; r++)
        {
            if (_cellStates[r, 0] == state && _cellStates[r, 1] == state && _cellStates[r, 2] == state)
            {
                return true;
            }
        }

        for (var c = 0; c < Size; c++)
        {
            if (_cellStates[0, c] == state && _cellStates[1, c] == state && _cellStates[2, c] == state)
            {
                return true;
            }
        }

        if (_cellStates[0, 0] == state && _cellStates[1, 1] == state && _cellStates[2, 2] == state)
        {
            return true;
        }

        if (_cellStates[0, 2] == state && _cellStates[1, 1] == state && _cellStates[2, 0] == state)
        {
            return true;
        }

        return false;
    }

    private bool CheckDraw()
    {
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (_cellStates[r, c] == CellState.None)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(_waitTime);
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                _cellStates[r, c] = CellState.None;
                _cells[r, c].sprite = null;
            }
        }
        _AIStop = false;
    }

    private void AITurn()
    {
        Vector2Int move;

        //勝利できるか
        move = AIFindBestMove(CellState.Cross);
        if (move.x != -1 && move.y != -1)
        {
            PlaceCross(move);
            return;
        }

        //勝利を防ぐ
        move = AIFindBestMove(CellState.Circle);
        if (move.x != -1)
        {
            PlaceCross(move);
            return;
        }

        //真ん中を置けるか
        if(_cellStates[1, 1] == CellState.None)
        {
            PlaceCross(new Vector2Int(1, 1));
            return;
        }

        //角
        var corners = new[]
        {
        new Vector2Int(0,0),
        new Vector2Int(0,2),
        new Vector2Int(2,0),
        new Vector2Int(2,2),
    };

        foreach (var corner in corners)
        {
            if (_cellStates[corner.x, corner.y] == CellState.None)
            {
                PlaceCross(corner);
                return;
            }
        }

        //残り
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (_cellStates[r, c] == CellState.None)
                {
                    PlaceCross(new Vector2Int(r, c));
                    return;
                }
            }
        }
    }

    /// <summary>
    /// べストな手を見つける
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private Vector2Int AIFindBestMove(CellState state)
    {
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (_cellStates[r, c] != CellState.None)
                {
                    continue;
                }
                // 仮置き
                _cellStates[r, c] = state;

                if (CheckWin(state))
                {
                    _cellStates[r, c] = CellState.None;
                    return new Vector2Int(r, c);
                }
                // 戻す
                _cellStates[r, c] = CellState.None;
            }
        }
        return new Vector2Int(-1, -1);
    }

    private void PlaceCross(Vector2Int pos)
    {
        if (_AIStop) return;
        _cellStates[pos.x, pos.y] = CellState.Cross;
        _cells[pos.x, pos.y].sprite = _cross;

        if (CheckWin(CellState.Cross))
        {
            Debug.Log("AIの勝ち！");
            StartCoroutine(RestartGame());
            return;
        }

        if (CheckDraw())
        {
            Debug.Log("引き分け！");
            StartCoroutine(RestartGame());
        }
    }

    private enum CellState
    {
        None,
        Circle,
        Cross
    }
}