using StructCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCondition : MonoBehaviour
{
    public int characterIndex;
    public Skill[] skill;
    Transform Hp;
    public void OnSkillClicked(int _skillIndex)
    {
        GameManager.battleScenario.OnSkillClicked(characterIndex, _skillIndex);
    }
}
