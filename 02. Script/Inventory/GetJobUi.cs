using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BattleCollection;
public class GetJobUi : MonoBehaviour
{
    public CharacterHierarchy from;
    public CharacterHierarchy to;
    public TMP_Text toText;
    private void Awake()
    {
        from.animator.enabled = false;
        to.animator.enabled = false;
    }
    public void SetInfo(CharacterData _character)
    {
        from.CopyHierarchySprite(_character.characterHierarchy);
        to.CopyHierarchySprite(_character.characterHierarchy);
        string jobId = GameManager.gameManager.GetJobId(_character.skills);
        to.SetJobSprite(LoadManager.loadManager.jobsDict[jobId]);
        string jobName = LoadManager.loadManager.jobsDict[jobId].name[GameManager.language];
        toText.text = jobName;
    }
    public void SetJob()
    {
        ItemManager.itemManager.SetJobAtSelectedCharacter();
    }
}
