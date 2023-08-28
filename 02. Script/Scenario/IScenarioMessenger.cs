using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScenarioMessenger
{
    public class MapToBattleData : IScenarioMessenger
    {
        public MapArea mapArea;
        public BattleDifficulty battleDifficulty;
        public MapToBattleData(MapArea _mapArea, BattleDifficulty _battleDifficulty)
        {
            mapArea = _mapArea;
            battleDifficulty = _battleDifficulty;
        }
    }
}