using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Threading.Tasks;


public class TicTacToeManager : MonoBehaviour
{
    private const int Size = 3; // 3x3のマス目

    [Header("Cell Colors")]
    [SerializeField] private Color _normalCell = Color.white;
    [SerializeField] private Color _selectedCell = Color.cyan;

    [Header("Cell Sprites")]
    [SerializeField] private Sprite _circle = null;
    [SerializeField] private Sprite _cross = null;

    [Header("JudgementUI")]
    [SerializeField] private GameObject _judgeText;
    [SerializeField] private GameObject _judgeBackSprite;

    [SerializeField] private GameObject _nextText;

    private int _selectedRow; //行数
    private int _selectedColumn; //列数
    private int _aiWaitTime = 500;
    private int _aiMoveCount = 0;

    private bool _isPlayerTurn = true;

    private bool _isPlayerWin = false;
    private bool _isAiWin = false;
    private bool _isDraw = false;
    private bool _isGameFinished
        => _isPlayerWin == true || _isAiWin == true || _isDraw == true;

    private Image[,] _cells;

    private void Start()
    {
        _judgeText.SetActive(false);
        _judgeBackSprite.SetActive(false);
        _nextText.SetActive(false);

        _cells = new Image[Size, Size];

        for (var r = 0; r < _cells.GetLength(0); r++)
        {
            for (var c = 0; c < _cells.GetLength(1); c++)
            {
                var obj = new GameObject($"Cell({r}, {c})");
                obj.transform.parent = transform;
                var cell = obj.AddComponent<Image>();
                _cells[r, c] = cell;
            }
        }
    }

    private void Update()
    {
        if (_isGameFinished)
        {
            ResetGame();
        }
        if (_isPlayerTurn)
        {
            CheckWinCondition();
            MoveCell();
            SelectCell();
        }
        if (!_isPlayerTurn)
        {
            CheckWinCondition();
            if (_aiMoveCount == 0)
            {
                _ = AiMove();
            }
        }
    }


    private void ResetGame()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            for (var r = 0; r < _cells.GetLength(0); r++)
            {
                for (var c = 0; c < _cells.GetLength(1); c++)
                {
                    _cells[r, c].sprite = null;
                }
            }
            _isPlayerWin = false;
            _isAiWin = false;
            _isDraw = false;
            _judgeText.SetActive(false);
            _judgeBackSprite.SetActive(false);
            _nextText.SetActive(false);
        }
    }

    private bool SelectCell()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var cell = _cells[_selectedRow, _selectedColumn];

            if (cell.sprite != null) return false;
            if (_isPlayerTurn)
            {
                cell.sprite = _circle;
                _isPlayerTurn = false;
            }
        }
        return true;
    }

    private async Task AiMove()
    {
        _aiMoveCount++;
        if (_isGameFinished) return;
        await Task.Delay(_aiWaitTime);

        var emptyCells = new System.Collections.Generic.List<(int row, int col)>();

        // 空いているセルをリストに追加
        for (var r = 0; r < _cells.GetLength(0); r++)
        {
            for (var c = 0; c < _cells.GetLength(1); c++)
            {
                if (_cells[r, c].sprite == null)
                {
                    emptyCells.Add((r, c));
                }
            }
        }

        if (emptyCells.Count > 0)
        {
            var winMove = AiBestMove(_cross);
            if (winMove != null)
            {
                _cells[winMove.Value.row, winMove.Value.col].sprite = _cross;
                _isPlayerTurn = true;
                _aiMoveCount = 0;
                return;
            }

            var blockMove = AiBestMove(_circle);
            if (blockMove != null)
            {
                _cells[blockMove.Value.row, blockMove.Value.col].sprite = _cross;
                _isPlayerTurn = true;
                _aiMoveCount = 0;
                return;
            }

            var randomIndex = Random.Range(0, emptyCells.Count);
            var (row, col) = emptyCells[randomIndex];
            _cells[row, col].sprite = _cross;
            _isPlayerTurn = true;
            _aiMoveCount = 0;
            return;
        }
    }

    /// <summary>
    /// AIが最善の手を選ぶための関数
    /// </summary>
    /// <param name="target"></param>
    private (int row, int col)? AiBestMove(Sprite target)
    {
        // 縦と横のライン
        for (var i = 0; i < Size; i++)
        {
            var lineCheckResult = AiCheckLine(target, (i, 0), (i, 1), (i, 2));
            if (lineCheckResult.HasValue)
            {
                return lineCheckResult;
            }
            lineCheckResult = AiCheckLine(target, (0, i), (1, i), (2, i));
            if (lineCheckResult.HasValue)
            {
                return lineCheckResult;
            }
        }
        // 斜めのライン
        var result = AiCheckLine(target, (0, 0), (1, 1), (2, 2));
        if (result.HasValue)
        {
            return result;
        }
        result = AiCheckLine(target, (0, 2), (1, 1), (2, 0));
        if (result.HasValue)
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// AIが特定のラインに対して、勝利またはブロックの手を選ぶための関数
    /// </summary>
    /// <param name="target"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    private (int row, int col)? AiCheckLine
        (Sprite target, (int row, int col) a, (int row, int col) b, (int row, int col) c)
    {
        var cells = new[] { a, b, c };
        int targetCount = 0;
        int emptyCount = 0;
        (int row, int col) emptyCell = (-1, -1);

        foreach (var cell in cells)
        {
            var sprite = _cells[cell.row, cell.col].sprite;

            if (sprite == target)
            {
                targetCount++;
            }
            else if (sprite == null)
            {
                emptyCount++;
                emptyCell = cell;
            }
        }

        if (targetCount == 2 && emptyCount == 1)
        {
            _cells[emptyCell.row, emptyCell.col].sprite = _cross;
            _isPlayerTurn = true;
            _aiMoveCount = 0;
            return emptyCell;
        }
        return null;
    }

    private void MoveCell()
    {
        // cellの移動
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) { _selectedColumn--; }
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) { _selectedColumn++; }
        if (Keyboard.current.upArrowKey.wasPressedThisFrame) { _selectedRow--; }
        if (Keyboard.current.downArrowKey.wasPressedThisFrame) { _selectedRow++; }

        // cellの移動範囲を制限
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
    }

    private void CheckWinCondition()
    {
        // 勝利条件のチェック
        for (var i = 0; i < Size; i++)
        {
            // 横のライン
            if (_cells[i, 0].sprite != null
                && _cells[i, 0].sprite == _cells[i, 1].sprite
                && _cells[i, 1].sprite == _cells[i, 2].sprite)
            {
                SetWinState(_cells[i, 0].sprite);
                return;
            }
            // 縦のライン
            if (_cells[0, i].sprite != null
                && _cells[0, i].sprite == _cells[1, i].sprite
                && _cells[1, i].sprite == _cells[2, i].sprite)
            {
                SetWinState(_cells[0, i].sprite);
                return;
            }
        }
        // 斜めのライン
        if (_cells[0, 0].sprite != null
            && _cells[0, 0].sprite == _cells[1, 1].sprite
            && _cells[1, 1].sprite == _cells[2, 2].sprite)
        {
            SetWinState(_cells[0, 0].sprite);
            return;
        }
        if (_cells[0, 2].sprite != null
            && _cells[0, 2].sprite == _cells[1, 1].sprite
            && _cells[1, 1].sprite == _cells[2, 0].sprite)
        {
            SetWinState(_cells[0, 2].sprite);
            return;
        }
        // 引き分けのチェック
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (_cells[r, c].sprite == null) return;
            }
        }
        _isDraw = true;
        SetWinState(null);
    }

    private void SetWinState(Sprite winningSprite)
    {
        if (winningSprite == _circle)
        {
            _isPlayerWin = true;
            ShowJudgeText("WIN", Color.green);
        }
        else if (winningSprite == _cross)
        {
            _isAiWin = true;
            ShowJudgeText("LOSE", Color.red);
        }
        else
        {
            _isDraw = true;
            ShowJudgeText("DRAW", Color.yellow);
        }
    }

    private void ShowJudgeText(string text, Color color)
    {
        _judgeText.SetActive(true);
        _judgeBackSprite.SetActive(true);
        _nextText.SetActive(true);
        var textComponent = _judgeText.GetComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.color = color;
    }
}