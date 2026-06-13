using UnityEngine;
using UnityEngine.UI;

public class LifeCell : MonoBehaviour
{
    [SerializeField]
    private Image _image = null;//ビュー（見た目）MVCアーキテクチャ

    [SerializeField]
    private Color _aliveColor = Color.black;//生きてるときの色

    [SerializeField]
    private Color _deadColor = Color.white;//死んでるときの色

    [SerializeField]
    private Life_CellState _state = Life_CellState.Dead;

    public Life_CellState state
    {
        get => _state;
        set//セッター
        {
            _state = value;
            OnStateChenge();
        }

    }

    private void OnValidate()//実行してなくても反映することができる　デザイナーが確認しやすいUIなどに使う
    {
        OnStateChenge();
    }

    private void OnStateChenge()
    {
        if (_image == null) return;
        _image.color = (state == Life_CellState.Alive) ? _aliveColor : _deadColor;
    }
}

/// <summary>
/// セルのライフステータス
/// </summary>
public enum Life_CellState
{
    Dead,
    Alive,
}
