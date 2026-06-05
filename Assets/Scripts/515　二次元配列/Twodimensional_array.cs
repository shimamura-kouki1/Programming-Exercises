using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Twodimensional_array : MonoBehaviour
{
    [SerializeField] private int _row = 5; // 行数
    [SerializeField] private int _col = 5; // 列数

    private GridLayoutGroup _group;
    private Image[,] _grid; // 2次元配列

    //選択中のセル
    private int _currentRow = 0;
    private int _currentCol = 0;
    private void Start()
    {
        _group = GetComponent<GridLayoutGroup>();
        _group.constraint = GridLayoutGroup.Constraint.FixedColumnCount; // 列数を固定
        _group.constraintCount = _col; // 列数を設定

        _grid = new Image[_row, _col];

        for (var r = 0; r < _row; r++)
        {
            for (var c = 0; c < _col; c++)
            {
                var obj = new GameObject($"Cell({r}, {c})");
                obj.transform.SetParent(transform, false);

                var image = obj.AddComponent<Image>();
                if (r == 0 && c == 0) { image.color = Color.red; }
                else { image.color = Color.white; }

                if (r == 0 && c == 0)
                {
                    image.color = Color.red;
                }
                else
                {
                    image.color = Color.white;
                }

                // 配列に保存
                _grid[r, c] = image;
            }
        }
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) { return; } // 入力デバイスがない場合は処理しない

        if (keyboard.leftArrowKey.wasPressedThisFrame) // 左キーを押した
        {
            Move(-1, 0);
        }
        if (keyboard.rightArrowKey.wasPressedThisFrame) // 右キーを押した
        {
            Move(1, 0);
        }
        if (keyboard.upArrowKey.wasPressedThisFrame) // 上キーを押した
        {
            Move(0, -1);
        }
        if (keyboard.downArrowKey.wasPressedThisFrame) // 下キーを押した
        {
            Move(0, 1);
        }
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            RemoveCurrentCell();
        }
    }

    /// <summary>
    /// セルの移動
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void Move(int x, int y)
    {
        // 現在位置から開始
        int nextCol = _currentCol;
        int nextRow = _currentRow;

        while (true)
        {
            // 1マス進める
            nextCol += x;
            nextRow += y;

            // 範囲外なら移動不可
            if (nextCol < 0 || nextCol >= _col)
            {
                return;
            }

            if (nextRow < 0 || nextRow >= _row)
            {
                return;
            }

            // 有効セルを見つけた
            if (_grid[nextRow, nextCol] != null)
            {
                break;
            }
        }

        // 今のセルを白へ
        _grid[_currentRow, _currentCol].color = Color.white;

        // 更新
        _currentCol = nextCol;
        _currentRow = nextRow;

        // 新しいセルを赤へ
        _grid[_currentRow, _currentCol].color = Color.red;
    }

    /// <summary>
    /// 現在選択されているセルをグリッドから削除し、削除後に最も近い残存セルを新たに選択する。削除セルは透明化され内部配列から除去される
    /// </summary>
    /// <remarks>削除前に座標を保存し、対象セルの色を Color.clear に設定して _grid[行,列] を null
    /// にする。マンハッタン距離（行差の絶対値と列差の絶対値の和）で最も近いセルを探索し、見つかったセルの座標を _currentRow/_currentCol に設定して色を Color.red
    /// にする。全セルが削除済みで該当セルがない場合は選択を変更しない。</remarks>
    private void RemoveCurrentCell()
    {
        var currentCell = _grid[_currentRow, _currentCol];

        if (currentCell == null) return;

        // 削除前の座標保存
        int removedRow = _currentRow;
        int removedCol = _currentCol;

        // 見えなくする
        currentCell.color = Color.clear;

        // 操作対象から外す
        _grid[removedRow, removedCol] = null;

        // 最も近いセルを探す
        float minDistance = float.MaxValue;

        int nextRow = -1;
        int nextCol = -1;

        for (int r = 0; r < _row; r++)
        {
            for (int c = 0; c < _col; c++)
            {
                // 削除済みは無視
                if (_grid[r, c] == null)
                {
                    continue;
                }

                // 距離計算
                float distance =
                    Mathf.Abs(r - removedRow) +
                    Mathf.Abs(c - removedCol);

                // より近いセル発見
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nextRow = r;
                    nextCol = c;
                }
            }
        }

        // 次のセルが存在しない
        // = 全セル削除済み
        if (nextRow == -1 || nextCol == -1)
        {
            return;
        }

        // 選択位置更新
        _currentRow = nextRow;
        _currentCol = nextCol;

        // 新しい選択セルを赤に
        _grid[_currentRow, _currentCol].color = Color.red;
    }
}