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
        public int ableLevel;
        public List<TalentEffect> effects;
        public Sprite sprite;
        public int effectLevel;
        public TalentClass(Dictionary<Language, string> _name, int _ableLevel, Dictionary<Language, string> _explain, List<TalentEffect> _effects, Sprite _sprite)
        {
            effects = _effects;
            name = _name;
            explain = _explain;
            ableLevel = _ableLevel;
            sprite = _sprite;
        }
        public TalentClass SetEffectLevel(int _effectLevel)
        {
            effectLevel = _effectLevel;
            return this;
        }
        public string GetExplain()
        {
            string fontColor = "#0096FF";
            float fontSize = 120f;
            string replacedStr = explain[GameManager.language];
            // ���Խ� ���� ���ڿ� ����, % ���� ���ε� Ȯ��
            string pattern = $@"\{{(\d+)\}}(\%)?";
            Regex regex = new Regex(pattern);

            MatchCollection matches = regex.Matches(replacedStr);

            foreach (Match match in matches)
            {
                int index = int.Parse(match.Groups[1].Value); // {Value_i}���� i ����
                bool isPercent = match.Groups[2].Success; // % ��ȣ ���� ���� Ȯ��
                if (index >= 0 && index < effects.Count)
                {
                    float value = effects[effectLevel].value[index];
                    if (isPercent)
                    {
                        value *= 100;
                    }
                    string replaceStr = value.ToString("0.##");
                    if (isPercent)
                    {
                        replaceStr += "%"; // % ��ȣ�� ����� ����
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