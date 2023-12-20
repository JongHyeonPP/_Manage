namespace EnumCollection
{
    public enum JobType : byte
    {
        None, Tanker, Warrior, Ranger, Crusader, Thief, Witch
    }
    public enum DataSection:byte
    {
        SoundSetting, Language
    }
    public enum EVolume:byte
    {
        All, Sfx, Bgm
    }
    public enum SkillTarget:byte
    {
        Target, Nontarget 
    }
    public enum EffectType:byte
    {
        //발동류
        Damage, Damage_P, Heal, Restoration, Armor, Bleed, AbilityVamp,
        //효과류
        AttAscend, DefAscend, HealAscend, BuffAscend, DebuffAscend, Reduce,
        AttDescend,DefDescend, SpeedDescend,
        Confuse, BleedTransfer, Reflect, Paralyze,  
        BarrierConv, Enchant, Repeat,
        DamageShare, AbilityAscend,
        AbilityByDamage, Vamp, 
        Critical,Revive
    }
    public enum EffectRange : byte
    {
        Dot, Row, Column, Behind, Front, Self
    }
    public enum SkillCategori : byte
    {
        Default, Power, Util, Sustain, Enemy
    }
    public enum BattlePatern : byte
    {
        Battle, OnReady, Pause, Done, 
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
}