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

    private LifeCell[,] _cells;

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
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                _cells[r, c] = cell;
            }
        }
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.rightArrowKey.wasPressedThisFrame)
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

    }

    /// <summary>
    /// セルの周囲の生きている数をカウント
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private int CountAliveNeighdors(int row, int col)
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
                if (nc < 0 || nc >= _columns)continue;

                if (_cells[nr,nc].state == Life_CellState.Alive)
                {
                    count++;
                }
            }
        }
        return count;
    }
}
