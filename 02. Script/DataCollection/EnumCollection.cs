namespace EnumCollection
{
    public enum DataSection : byte
    {
        SoundSetting, Language, Screen
    }
    public enum VolumeType : byte
    {
        All, Sfx, Bgm
    }
    public enum SkillTarget : byte
    {
        Target, Nontarget
    }
    public enum EffectType : byte
    {
        //발동류
        Damage, Heal, Armor, Bleed, AbilityVamp,
        //효과류
        AttAscend, ResistAscend, HealAscend, DebuffAscend, Reduce,
        AttDescend, ResistDescend, SpeedAscend, SpeedDescend,
        Confuse, BleedTransfer, Reflect, Paralyze,
        Enchant, Repeat,
         AbilityAscend,
        ResistByDamage, Vamp, Critical, Revive,
        FameAscend, GoldAscend,
        BuffAscend, RewardAscend,
        ResilienceAscend

    }
    public enum UpgradeEffectType : byte
    {
        AllocateNumberUp, StatusUp, TalentEffectUp, TalentLevelUp, FameUp, GoldUp
    }
    public enum EffectRange : byte
    {
        Dot, Row, Column, Self,
        Behind, Front, TargetsAllies
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
        //Stage 0
        Plains, Forest, Beach, Ruins, ElfCity,
        //Stage1
        MysteriousForest, VineForest, Swamp, WinterForest, IceField,
        //Stage 2
        DesertRuins, Cave, Desert, RedRock, Lava,
        //Store
        Store
    }
    public enum BattleDifficulty : byte
    {
        Normal, Elite, Boss
    }
    public enum Difficulty : byte
    {
        Easy, Noraml, Hard
    }
    public enum Language : byte
    {
        Ko, En
    }
    public enum WeaponType : byte
    {
        Sword, Bow, Magic, Club
    }
    public enum ItemGrade : byte
    {
        Normal, Rare, Unique, None
    }
    public enum LobbyCase : byte
    {
        None, Pub, Guild, Recruit, Depart
    }
    public enum StoreCase : byte
    {
        Cook, Store
    }
    public enum BodyPart : byte
    {
        Arm_L, Arm_R, Head
    }
    public enum Species : byte
    {
        Human, Elf, Devil, Skelton, Orc
    }
    public enum ClothesPart : byte
    {
        Back,
        ClothBody, ClothLeft, ClothRight,
        ArmorBody, ArmorLeft, ArmorRight,
        Helmet,
        FootRight, FootLeft
    }
    public enum ItemType
    {
        Weapon, Skill, Food, Ingredient, All
    }
    public enum IngredientType
    {
        Meat, Fish, Fruit, Vegetable, Special
    }
    public enum ValueBase
    {
        Const, Ability, Armor, HpMax_Enemy, HpMax_Caster,Resist
    }
    public enum StatusType
    {
        Hp, HpMax, Ability, Resist, Speed
    }
    public enum StateInMap
    {
        NeedPhase, NeedEnter,NeedMove, None
    }
    public enum PokerCombination
    {
        NoCard, HighCard, OnePair, TwoPair, ThreeOfAKind,
        Straight, Flush, FullHouse, FourOfAKind, StraightFlush
    }
    public enum ScoreType
    {
        Enemy, Destination, Boss, Food, Total
    }
}