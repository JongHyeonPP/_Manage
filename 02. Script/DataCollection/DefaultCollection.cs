using BattleCollection;
using EnumCollection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DefaultCollection
{
    [Serializable]
    public class TalentClass
    {
        //DB 필드
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public int ableLevel;
        public List<TalentEffect> effects;
        public Sprite sprite;
        public string talentId;
        //인게임
        public int effectLevel;

        public TalentClass(string _talentId, Dictionary<Language, string> _name, int _ableLevel, Dictionary<Language, string> _explain, List<TalentEffect> _effects, Sprite _sprite)
        {
            talentId = _talentId;
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
            string increaseColor = "#0096FF";
            string decreaseColor = "#CF262B";
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
                    float value = effects[index].value[effectLevel];
                    EffectType type = effects[index].type;
                    if (isPercent)
                    {
                        value *= 100;
                    }
                    string replaceStr = value.ToString("0.##");
                    if (isPercent)
                    {
                        replaceStr += "%"; // % 기호를 결과에 포함
                    }
                    string selectedColor;
                    switch (type)
                    {
                        default:
                            selectedColor = increaseColor;
                            break;
                        case EffectType.AttDescend:
                        case EffectType.ResistDescend:
                        case EffectType.SpeedDescend:
                        case EffectType.DefDescend:
                            selectedColor = decreaseColor;
                            break;
                    }
                    string richF = $"<color={selectedColor}><size={fontSize}%><b>";
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
    public class SettingSet
    {
        public Language language;
        public Dictionary<VolumeType, float> volume;
        public Dictionary<VolumeType, bool> onOff;
        public FullScreenMode fullScreenMode;
        public bool isSkillEffectOn;
        public int resolutionIndex;
        public SettingSet(Language _language, Dictionary<VolumeType, float> _volume, Dictionary<VolumeType, bool> _onOff, FullScreenMode _fullScreenMode,bool _isSkillEffectOn, int _resolutionIndex)
        {
            language = _language;
            volume = _volume;
            onOff = _onOff;
            fullScreenMode = _fullScreenMode;
            isSkillEffectOn = _isSkillEffectOn;
            resolutionIndex = _resolutionIndex;
        }
        public SettingSet GetDeepCopySet()
        {
            Dictionary<VolumeType, float> newVolume = new();
            Dictionary<VolumeType, bool> newOnOff = new();
            foreach (VolumeType type in SettingManager.volumeTypes)
            {
                newVolume.Add( type, volume[type]);
                newOnOff.Add(type ,onOff[type]);
            }
            SettingSet newSet = new(language, newVolume, newOnOff, fullScreenMode, isSkillEffectOn, resolutionIndex);
            return newSet;
        }

    }
}