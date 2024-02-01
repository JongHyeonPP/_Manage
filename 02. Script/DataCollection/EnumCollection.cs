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
    public enum SkillTarget:byte
    {
        Target, Nontarget 
    }
    public enum EffectType:byte
    {
        //발동류
        Damage, Curse, Heal, Restoration, Armor, Bleed, AbilityVamp,
        //효과류
        AttAscend, ResistAscend, HealAscend, DebuffAscend, Reduce,
        AttDescend,ResistDescend,SpeedAscend ,SpeedDescend,
        Confuse, BleedTransfer, Reflect, Paralyze, Necro,
        BarrierConv, Enchant, Repeat,
        DamageShare, AbilityAscend,
        ResistByDamage, Vamp, Critical, Revive,
        FameAscend, GoldAscend,
        //직업류
        CorpseExplo, ResistAscend_P, BuffAscend, RewardAscend,
        
        AttAscend_Torment, ResilienceAscend
    }
    public enum GuildEffectType : byte
    {
        AllocateNumberUp, AbilityUp, HpUp, ResistUp, TalentLevelUp, TalentNumUp,
        SpeedUp
    }
    public enum EffectRange : byte
    {
        Dot, Row, Column, Self,
        Behind, Front
    }
    public enum EffectPrecon : byte
    {
        Back, Center, Front
    }
    public enum SkillCategori : byte
    {
        Default, Power, Util, Sustain, Enemy
    }
    public enum BattlePatern : byte
    {
        Battle, OnReady
    }
    public enum BackgroundType : byte
    {
        Plains, Ruins, Cave, Desert, Forest, Beach, Lava, IceField, Swamp
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
    public enum WeaponType : byte
    {
        Melee, Bow, Magic, Shield
    }
    public enum WeaponGrade : byte
    {
        Normal, Rare, Unique
    }
}