using BattleCollection;
using EnumCollection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DefaultCollection
{
    public class TalentClass
    {
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public int level;
        public List<TalentEffect> effects;
        public Sprite sprite;
        public TalentClass(Dictionary<Language, string> _name, int _level, Dictionary<Language, string> _explain, List<TalentEffect> _effects, Sprite _sprite)
        {
            effects = _effects;
            name = _name;
            explain = _explain;
            level = _level;
            sprite = _sprite;
        }
        public string GetExplain()
        {
            int effectIndex = (int)GameManager.gameManager.upgradeValueDict[UpgradeEffectType.TalentEffectUp];
            string fontColor = "#0096FF";
            float fontSize = 120f;
            string replacedStr = explain[GameManager.language];
            // 정규식 패턴 문자열 생성, % 포함 여부도 확인
            string pattern = $@"\{{(\d+)\}}(\%)?";
            Regex regex = new Regex(pattern);

            MatchCollection matches = regex.Matches(replacedStr);

            foreach (Match match in matches)
            {
                int index = int.Parse(match.Groups[1].Value); // {Value_i}에서 i 추출
                bool isPercent = match.Groups[2].Success; // % 기호 존재 여부 확인
                if (index >= 0 && index < effects.Count)
                {
                    float value = effects[effectIndex].value[index];
                    if (isPercent)
                    {
                        value *= 100;
                    }
                    string replaceStr = value.ToString("0.##");
                    if (isPercent)
                    {
                        replaceStr += "%"; // % 기호를 결과에 포함
                    }

                    string richF = $"<color={fontColor}><size={fontSize}%><b>";
                    string richB = "</b></size></color>";
                    replaceStr = richF + replaceStr + richB;
                    replacedStr = replacedStr.Replace(match.Value, replaceStr);
                }
            }
            replacedStr = replacedStr.Replace("\\n", "\n");
            return replacedStr;
        }
    }
    public class TalentEffect
    {
        public List<float> value;
        public EffectType type;
        public TalentEffect(List<float> _value, EffectType _type)
        {
            value = _value;
            type = _type;
        }
    }
    public class BodyPartClass
    {
        public Sprite head;
        public Sprite armL;
        public Sprite armR;
    }
    public class EyeClass
    {
        public Sprite front;
        public Sprite back;
    }

}