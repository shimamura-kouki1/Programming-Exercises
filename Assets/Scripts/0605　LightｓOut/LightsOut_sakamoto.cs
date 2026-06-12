using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Sample3 : MonoBehaviour, IPointerClickHandler
{
    // 行数
    [SerializeField] private int _rows = 5;
    // 列数
    [SerializeField] private int _columns = 5;
    private GridLayoutGroup _layoutGroup;
    private ImageObj2[][] _imageObj;
    private bool _isClear;
    private int _count = 0;
    private float _timer = 0f;
    private void Start()
    {
        _layoutGroup = GetComponent<GridLayoutGroup>();
        _layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _layoutGroup.constraintCount = _columns;
        _imageObj = new ImageObj2[_rows][];

        for (var r = 0; r < _rows; r++)
        {
            _imageObj[r] = new ImageObj2[_columns];
            for (var c = 0; c < _columns; c++)
            {
                var cell = new GameObject($"Cell({r}, {c})");
                cell.transform.parent = transform;
                var image = cell.AddComponent<Image>();
                var imageObj = cell.AddComponent<ImageObj2>();
                imageObj.Initialize(image, r, c);

                int rand = Random.Range(0, 2);
                if (rand == 0)
                    imageObj.SetIsBlack();
                else
                    imageObj.SetIsWhite();

                _imageObj[r][c] = imageObj;
            }
        }
    }

    private void Update()
    {
        if (!_isClear)
            _timer += Time.deltaTime;

        //チート
        var keyboard = Keyboard.current;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    _imageObj[r][c].SetIsWhite();
                }
            }
            if (!_isClear)
            {
                _isClear = ClearCheck();
                if (_isClear)
                    Debug.Log($"Clear! Time: {_timer} Count: {_count}");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject;
        var imageObj = cell.GetComponent<ImageObj2>();

        if (!_isClear)
            _count++;

        FlipCells(imageObj.Row, imageObj.Columns);

        if (!_isClear)
        {
            _isClear = ClearCheck();
            if (_isClear)
                Debug.Log($"Clear! Time: {_timer} Count: {_count}");
        }
    }

    private void Flip(ImageObj2 obj)
    {
        if (obj.IsBlack)
            obj.SetIsWhite();
        else
            obj.SetIsBlack();
    }

    //クリックしたセルの上下左右を反転させる
    private void FlipCells(int row, int column)
    {
        Flip(_imageObj[row][column]);

        if (row > 0)
            Flip(_imageObj[row - 1][column]);

        if (row < _rows - 1)
            Flip(_imageObj[row + 1][column]);

        if (column > 0)
            Flip(_imageObj[row][column - 1]);

        if (column < _columns - 1)
            Flip(_imageObj[row][column + 1]);
    }

    //すべてのセルが白または黒であるかをチェックする
    private bool ClearCheck()
    {
        bool targetColor = _imageObj[0][0].IsBlack;

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                if (_imageObj[r][c].IsBlack != targetColor)
                    return false;
            }
        }
        return true;
    }
}


public class ImageObj2 : MonoBehaviour
{
    public bool IsBlack { get; private set; }
    public int Row { get; private set; }
    public int Columns { get; private set; }

    private Image _image;
    public void Initialize(Image image, int row, int columns)
    {
        _image = image;
        Row = row;
        Columns = columns;
    }

    public void SetIsBlack()
    {
        IsBlack = true;
        _image.color = Color.black;
    }

    public void SetIsWhite()
    {
        IsBlack = false;
        _image.color = Color.white;
    }
}
