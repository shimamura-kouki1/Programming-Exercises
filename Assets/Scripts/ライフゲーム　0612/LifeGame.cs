using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LifeGame : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int _rows = 1;
    [SerializeField] private int _columns = 1;

    [SerializeField] private GridLayoutGroup _gridLayoutGroup = null;
    [SerializeField] private LifeCell _cellPrefab = null;

    [SerializeField]
    private float _duration = 1.0F; // セルを更新する時間間隔（秒単位）
    private bool _isPlaying = false; // 時間経過の更新が実行中かどうか

    [SerializeField]
    [Multiline]
    private string _data = "";

    private LifeCell[,] _cells;
    private float _timer = 0.0f;

    void Start()
    {
        _cells = new LifeCell[_rows, _columns];
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        var parent = _gridLayoutGroup.gameObject.transform;
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab,parent,false);
                _cells[r, c] = cell;
            }
        }
        LoadData();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            _isPlaying = !_isPlaying;
            _timer = 0.0f;
        }
        if (_isPlaying)
        {
            _timer += Time.deltaTime;

            if (_timer >= _duration)
            {
                _timer = 0.0f;
                OnNext();
            }
        }

        if (!_isPlaying && keyboard.rightArrowKey.wasPressedThisFrame)
        {
            OnNext();
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var target = eventData.pointerCurrentRaycast.gameObject;
        if (target.TryGetComponent<LifeCell>(out var cell))
        {
            cell.state = cell.state == Life_CellState.Alive ? Life_CellState.Dead : Life_CellState.Alive;
        }
    }

    private void OnNext()
    {
        Life_CellState[,] nextStates = new Life_CellState[_rows, _columns];

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                int aliveCount = CountAliveNeighbors(r, c);

                bool isAlive = _cells[r, c].state == Life_CellState.Alive;

                if (isAlive)
                {
                    nextStates[r, c] = (aliveCount == 2 || aliveCount == 3) ? Life_CellState.Alive : Life_CellState.Dead;
                }
                else
                {
                    nextStates[r, c] = (aliveCount == 3) ? Life_CellState.Alive : Life_CellState.Dead;
                }
            }
        }
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                _cells[r, c].state = nextStates[r, c];
            }
        }
    }

    /// <summary>
    /// セルの周囲の生きている数をカウント
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private int CountAliveNeighbors(int row, int col)
    {
        int count = 0;

        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;

                int nr = row + dr;
                int nc = col + dc;

                if (nr < 0 || nr >= _rows) continue;
                if (nc < 0 || nc >= _columns) continue;

                if (_cells[nr, nc].state == Life_CellState.Alive)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void LoadData()
    {
        string[] lines = _data.Split('\n');

        for (int r = 0; r < _rows; r++)
        {
            // 行が足りなければ何もしない
            if (r >= lines.Length)
                continue;

            for (int c = 0; c < _columns; c++)
            {
                // 列が足りなければ何もしない
                if (c >= lines[r].Length)
                    continue;

                char ch = lines[r][c];

                _cells[r, c].state =
                    (ch == '1') ? Life_CellState.Alive : Life_CellState.Dead;
            }
        }
    }
}
