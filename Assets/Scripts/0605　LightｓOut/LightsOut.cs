using UnityEngine;
using UnityEngine.UI;

public class LightsOut : MonoBehaviour
{
    [SerializeField] private int _col = 5;//列数
    [SerializeField] private int _row = 5;//行数
    private Image[,] _images;//セルのイメージ
    private bool[,] _isBlack;//セルが黒かどうか

    [SerializeField] private int _shuffle = 20;//シャッフル回数

    private int _clickCount = 0;//クリック回数
    private float _startTime;//ゲーム開始時間
    private Vector2Int[] _answer;//正解手順配列

    private void Start()
    {
        _images = new Image[_row, _col];
        _isBlack = new bool[_row, _col];
        _answer = new Vector2Int[_shuffle];

        //========================--セルの生成--========================
        for (var r = 0; r < _row; r++)
        {
            for (var c = 0; c < _col; c++)
            {
                GameObject cell = new GameObject($"Cell({r}, {c})");
                cell.transform.parent = transform;
                Image image = cell.AddComponent<Image>();

                image.color = Color.white;
                _images[r, c] = image;
                _isBlack[r, c] = false;

                //========================--ボタンの生成--========================

                Button button = cell.AddComponent<Button>();

                int row = r;
                int col = c;

                button.onClick.AddListener(() => OnCellClick(row, col));
                // ここで r と c をキャプチャしているため、正しいセルがクリックされたときに正しい座標が渡されるようになります。
            }
        }
        Shuffle();
        Answer();
        _startTime = Time.time;
    }

    /// <summary>
    /// セルがクリックされたときの処理
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void OnCellClick(int row, int col)
    {
        _clickCount++;

        PushCell(row, col);

        if (IsClear())
        {
            float elapsedTime = Time.time - _startTime;
            Debug.Log($"クリア！{_clickCount}回クリックしました。クリア時間: {elapsedTime}秒");
        }
    }

    /// <summary>
    /// セルの状態を切り替える
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void Toggle(int row, int col)
    {
        //範囲外のセルは無視
        if (row < 0 || row >= _row || col < 0 || col >= _col)
            return;
        //Bool値の状態切り替える
        _isBlack[row, col] = !_isBlack[row, col];
        //イメージの色を切り替える
        _images[row, col].color = _isBlack[row, col] ? Color.black : Color.white;
    }

    /// <summary>
    /// ゲームクリアの判定
    /// </summary>
    /// <returns></returns>
    private bool IsClear()
    {
        //セルの状態を基準にして、全てのセルが同じ状態かどうかを判定
        bool target = _isBlack[0, 0];
        for (var r = 0; r < _row; r++)
        {
            for (var c = 0; c < _col; c++)
            {
                if (_isBlack[r, c] != target)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 白黒をランダムにシャッフルする
    /// </summary>
    private void Shuffle()
    {
        for (var r = 0; r <_shuffle ; r++)
        {
            int row = Random.Range(0, _row);
            int col = Random.Range(0, _col);

            PushCell(row, col);

            _answer[r] = new Vector2Int(row, col);//シャッフルの手順を記録
        }
    }

    /// <summary>
    /// チートコード
    /// </summary>
    private void Answer()
    {
        // シャッフルの逆順でクリックすることで、元の状態に戻す。
        for (var i = _answer.Length - 1; i >= 0; i--)
        {
            Debug.Log($"Answer: ({_answer[i].x}, {_answer[i].y})");
        }
    }

    /// <summary>
    /// 押したセルの状態を切り替える範囲乗せる確認
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void PushCell(int row, int col)
    {
        Toggle(row, col);
        Toggle(row - 1, col);
        Toggle(row + 1, col);
        Toggle(row, col - 1);
        Toggle(row, col + 1);
    }
}