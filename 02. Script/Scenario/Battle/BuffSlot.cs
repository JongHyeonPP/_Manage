using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlot : MonoBehaviour
{
    public int num;
    [SerializeField] TMP_Text textBuff;
    [SerializeField] Image imageUp;
    [SerializeField] Image imageDown;
    [SerializeField] TMP_Text textNum;
    public void SetBuffSlot(EffectType _effectType)
    {
        num = 1;
        textNum.text = string.Empty;

        string buffStr = string.Empty;
        int imageType = 0;//0 : None, 1 : imageUp, 2 : imageDown
        Color textColor = Color.white;
        switch (_effectType)
        {
            case EffectType.Bleed:
                buffStr = "Bleed";
                textColor = Color.red;
                break;
            case EffectType.AttAscend:
                buffStr = "Att";
                imageType = 1;
                break;
            case EffectType.DefAscend:
                buffStr = "Def";
                imageType = 1;
                break;
            case EffectType.ResistAscend:
                buffStr = "Res";
                imageType = 1;
                break;
            case EffectType.AttDescend:
                buffStr = "Att";
                imageType = 2;
                break;
            case EffectType.DefDescend:
                buffStr = "Def";
                imageType = 2;
                break;
            case EffectType.ResistDescend:
                buffStr = "Res";
                imageType = 2;
                break;
            case EffectType.SpeedAscend:
                buffStr = "Spd";
                imageType = 1;
                break;
            case EffectType.SpeedDescend:
                buffStr = "Spd";
                imageType = 2;
                break;
            case EffectType.Confuse:
                buffStr = "Confuse";
                textColor = Color.yellow;
                break;
            case EffectType.Paralyze:
                buffStr = "Paralyze";
                textColor = Color.green;
                break;
            case EffectType.Enchant:
                buffStr = "Enchant";
                break;
            case EffectType.Critical:
                buffStr = "Cri";
                imageType = 1;
                break;
            case EffectType.Restore:
                buffStr = "Heal";
                imageType = 1;
                break;
        }
        textBuff.color = textColor;
        textBuff.text = buffStr;
        imageUp.gameObject.SetActive(imageType == 1);
        imageDown.gameObject.SetActive(imageType == 2);
    }
    public bool ChangeBuffSlotOne(bool _isUp)//false = remove
    {
        if (_isUp)
        {
            num++;
            textNum.text = "x" + num;
        }
        else
        {
            num--;
            if (num == 1)
            {
                textNum.text = string.Empty;
            }
            else if (num == 0)
            {
                gameObject.SetActive(false);
                return false;
            }
            else
            {
                textNum.text = "x" + num;
            }
        }
        Debug.Log(num);
        return true;
    }
}
