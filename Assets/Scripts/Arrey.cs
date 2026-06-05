using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Arrey : MonoBehaviour
{
    [SerializeField] private int _count;
    private GameObject[] _cell;
    private int _currentCell;
    private bool[] _deleted;

    void Start()
    {
        _cell = new GameObject[_count];
        _deleted = new bool[_count];
        for (var i = 0; i < _count; i++)//_countは初期化用の変数だから、for文の条件式はi < _cell.Lengthのほうが
                                        //意味としてあっているいいと思う。
        {
            var obj = new GameObject($"Cell{i}");
            obj.transform.SetParent(transform, false);
            _cell[i] = obj;

            var image = obj.AddComponent<Image>();
            if (i == 0) { image.color = Color.red; _currentCell = i; }
            else { image.color = Color.white; }
        }
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) { return; } // 入力デバイスがない場合は処理しない

        if (keyboard.leftArrowKey.wasPressedThisFrame) // 左キーを押した
        {
            if (_currentCell > 0)
            {
                _currentCell--;
                _cell[_currentCell].GetComponent<Image>().color = Color.red;
                _cell[_currentCell + 1].GetComponent<Image>().color = Color.white;
            }
        }

        if (keyboard.rightArrowKey.wasPressedThisFrame) // 右キーを押した
        {
            if (_currentCell < _cell.Length - 1)
            {
                _currentCell++;
                _cell[_currentCell].GetComponent<Image>().color = Color.red;
                _cell[_currentCell - 1].GetComponent<Image>().color = Color.white;//Color.clearこれで透明にできる。
            }
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            _cell[_currentCell].SetActive(false);//これだと意図した動きにならない。
                                                 //透明度を変える方法にするべきかとかんがえている。
        }
    }
}

//outパラメーターは成功したときに値を返すものであり、失敗したときは値を返さない。public bool TryGetValue(string key, out int value)のような形で使われることが多い。成功したときはtrueを返し、失敗したときはfalseを返す。TryGetValueなどのメソッドは、ゲームを止める処理ではなく、失敗したことを知らせることもできるため、ゲームの流れを止めることなくエラー処理ができる。