namespace EnumCollection
{
    public enum DataSection:byte
    {
        SoundSetting, Language
    }
    public enum EVolume:byte
    {
        All, Sfx, Bgm
    }
    public enum Job:byte
    {
        None, Witch, Thief, Crusader, Ranger, Warrior, Tanker
    }
    public enum SkillTarget:byte
    {
        Target, Nontarget 
    }
    public enum EffectType:byte
    {
        AttBuff, DefBuff, AttDebuff, DefDebuff,
        AttUp, DefUp, AttDown, DefDown,
        Bleed, BleedTransfer, Reflect, Paralyze, Heal, 
        Damage, BarrierConv, Enchant, Repeat,
        Armor, ArmorAtt, DamageShare, Ability
    }
    public enum EffectRange : byte
    {
        Dot, Cross, Neighbors
    }
    public enum SkillCategori : byte
    {
        Default, Power, Util, Sustain, Enemy
    }
    public enum BattlePatern : byte
    {
        Default, OnReady, Pause, Done, 
    }
    public enum GridPatern:byte
    {
        Deactive, Interactable
    }
    public enum MapArea:byte
    {
        Plains, Ruins, Cave, Desert, Forest
    }
    public enum BattleDifficulty:byte
    {
        Normal, Elite, Boss
    }
    public enum Difficulty:byte
    {
        Easy, Noraml, Hard
    }
    public enum Language:byte
    {
        Ko, En
    }
    public enum T_Type:byte
    {
        None, AttAscend, DefAscend, AttDescend, DefDescend,
        FameAscend, GoldAscend, Critical, HealAscend,
        BuffAscend, DebuffAscend
    }
}