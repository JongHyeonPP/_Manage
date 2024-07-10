using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapCollection
{
   public class NodeType
    {
        public List<string> casesStr;
        public Dictionary<Language, string> name;
        public Sprite objectSprite;
        public BackgroundType backgroundType;
        public NodeType(List<string> _casesStr, Dictionary<Language, string> _name, Sprite _objectSprite, BackgroundType _backgroundType)
        {
            casesStr = _casesStr;
            name = _name;
            objectSprite = _objectSprite;
            backgroundType = _backgroundType;
        }
    }
}