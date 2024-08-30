using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUiBase : MonoBehaviour
{
    public void ExitBtnClicked()
    {
        SoundManager.SfxPlay("WoodenClick_0");
        gameObject.SetActive(false);
        GameManager.lobbyScenario.SetMediumImage(true);
    }
}
