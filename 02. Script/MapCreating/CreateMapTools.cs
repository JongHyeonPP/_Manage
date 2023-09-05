using System.Collections.Generic;
using UnityEngine;

internal static class CreateMapTools
{
    internal delegate GameObject InstantiateFunc(GameObject _prefab);

    internal static List<Vector2> GetEventPointRow(this CreateMapEventValues values, List<int> nodeTreeData, int level, Vector2 focusV2)
    {
        List<Vector2> rtnList = new List<Vector2>();
        List<float> ratioAxis = new List<float>();

        // set new List
        int count = 0;
        for (int i = 0; i < nodeTreeData.Count; i++)
        {
            count += nodeTreeData[i];
            nodeTreeData[i] = count;
        }

        count += (nodeTreeData.Count) - 1;

        float coefX = values.eventArea.getLength_X();
        float coefY = values.eventArea.getLength_Y();

        if (true)
        {
            float tempSum = 0f;
            for (int i = 0; i < count + 1; i++)
            {
                tempSum += UnityEngine.Random.Range(0.7f, 1f);
                ratioAxis.Add(tempSum);
            }

            for (int i = 0; i < ratioAxis.Count; i++)
            {
                ratioAxis[i] /= tempSum;
                ratioAxis[i] = -0.5f + ratioAxis[i];
            }
        }
        
        if (true)
        {
            float coeff = 3;
            float coef = Mathf.Min(coeff, count); // 2 3 4 5 5
            if (coef >= coeff) coef = 0;
            else
            {
                coef = 1 - ((coef + 1) / (coeff + 2));
            }

            for (int i = 0; i < count; i++)
            {
                float x = focusV2.x + coefX * ratioAxis[i];
                x = (coef) * focusV2.x + x * (1 - coef);

                float temp = ratioAxis[i] + 0.5f * (ratioAxis[i + 1] - ratioAxis[i]);
                float y = coefY * (level) + values.eventArea.getRandomHeight(temp);
                
                rtnList.Add(new Vector2(x,y) + GetRandomPointInCircle(Vector2.one));
            }
        }

        return rtnList;
    }

    internal static float GetChildSumOfAxis(this CreateMapEventValues values, List<NodeScriptPerLevel> data, Vector2Int focusV2,int index)
    {
        float sum1 = data.getTransformByGridPos(focusV2).position.x;
        float sum2 = 0;
        if (focusV2.x > 0)
        {
            int count = 1;

            Vector2 range = data.getChildRangeByGridPos(new Vector2Int(focusV2.x, index));
            for (int i = (int)range.x; i < range.y+1; i++)
            {
                sum2 += data.getTransformByGridPos(new Vector2Int(focusV2.x + 1, i)).position.x;
                count++;
            }

            sum2 /= count;
        }
        return (sum1 + sum2)/2;
    }

    internal static Vector2 GetRandomPointInCircle(Vector2 coefV2)
    {
        float angle = Random.Range(0.0f, Mathf.PI * 2.0f); // 랜덤한 각도값 지정
        float distance = Random.Range(0, 1f); // 랜덤한 거리값 지정

        Vector2 position = new Vector2(Mathf.Cos(angle) * coefV2.x, Mathf.Sin(angle) * coefV2.y) * distance; // 위치 계산
        return position;
    }

    internal static List<Vector2> GetPointsRandomCurve(Vector2 startV2, Vector2 endV2, float gap, float coef)
    {
        List<Vector2> points = new List<Vector2>();
        Vector2 roadV2 = endV2 - startV2;
        float totalLength = roadV2.magnitude;
        float currentLength = 0;
        Vector2 currentPoint = startV2;

        while (currentLength < totalLength)
        {
            Vector2 direction = (endV2 - currentPoint).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x);
            float gapWithRandomRange = gap * UnityEngine.Random.Range(0.95f, 1.05f);
            float offsetX = gapWithRandomRange * Mathf.Cos(angle);
            float offsetY = gapWithRandomRange * Mathf.Sin(angle);

            Vector2 nextPoint = currentPoint + new Vector2(offsetX, offsetY);

            if ((endV2 - nextPoint).magnitude <= gapWithRandomRange)
            {
                break;
            }

            float randomCoef = coef * UnityEngine.Random.Range(-1f, 1f);
            float perpendicularAngle = angle + Mathf.PI / 2;
            float shiftX = randomCoef * Mathf.Cos(perpendicularAngle);
            float shiftY = randomCoef * Mathf.Sin(perpendicularAngle);

            nextPoint += new Vector2(shiftX, shiftY);

            points.Add(nextPoint);
            currentPoint = nextPoint;
            currentLength += gapWithRandomRange;
        }

        return points;
    }

    internal static List<SpriteRenderer> setBackgroundSprites(this CreateMapBackgroundValues values, Vector2 stdPos, int index)
    {
        //Debug.Log(stdPos);
        List<SpriteRenderer> filledByBG = new();

        Fill_Square();
        return filledByBG;

        void Fill_Square()
        {
            float coefX = values.backgroundArea.getLength_X() / values.countX;
            float coefY = values.backgroundArea.getLength_Y() / values.countY;
            //Vector2 stdV2 = new Vector2(-values.backgroundArea.getLength_X() / 2, -values.backgroundArea.getLength_Y() / 2);
            for (int y = 0; y < values.countY; y++)
            {
                for (int x = 0; x < values.countX; x++)
                {
                    Transform insTrans = Object.Instantiate(values.bgPrefabList[index]).transform;
                    Vector2 tempV2 = new Vector2(coefX * ( x - values.countX/ 2 ), coefY * (y - values.countY / 2))
                         + GetRandomPointInCircle(new Vector2(values.coefX, values.coefY));
                    InstantSprite(insTrans, tempV2);
                }
            }


        }

        void Fill_Moderately()
        {
            float coefX = values.backgroundArea.getLength_X() / values.countX;
            float coefY = values.backgroundArea.getLength_Y() / values.countY;
            Vector2 stdV2 = new Vector2(-values.backgroundArea.getLength_X() / 2, -values.backgroundArea.getLength_Y() / 2);
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Transform insTrans = Object.Instantiate(values.bgPrefabList[index]).transform;
                    Vector2 tempV2 = GetRandomPointInCircle(stdV2 + new Vector2(values.backgroundArea.getLength_X(), values.backgroundArea.getLength_Y()));
                    InstantSprite(insTrans, tempV2);
                }
            }
        }

        void InstantSprite(Transform insTrans, Vector2 tempV2)
        {
            insTrans.parent = values.parentTransform;
            insTrans.localPosition = (Vector3)(stdPos + tempV2);
            insTrans.localPosition = (Vector3)((Vector2)insTrans.localPosition) + Vector3.forward * insTrans.localPosition.y;
            //insTrans.localScale *= UnityEngine.Random.Range(0.8f, 1f);
            filledByBG.Add(insTrans.GetComponent<SpriteRenderer>());
        }
    }

    // debug : temp Start Pos
    internal static List<SpriteRenderer> setEventArea_Root(this CreateMapEventValues values, List<Transform> transformList)
    {
        List<SpriteRenderer> eventAreaSprites = new();

        Quaternion spawnRotation = Quaternion.identity;
        Transform tempObj;

        tempObj = Object.Instantiate(values.eventObject[0]).transform;
        transformList.Add(tempObj);
        tempObj.parent = values.parentTransform;
        //tempObj.localPosition = new Vector3(0, 0, 0);//values.stdPos + Vector2.down* values.eventArea.getLength_Y()/3;
        tempObj.rotation = spawnRotation;

        eventAreaSprites.Add(_GetSpriteRenderer_EventNode(tempObj));

        return eventAreaSprites;

        SpriteRenderer _GetSpriteRenderer_EventNode(Transform instant)
        {
            return instant.GetChild(0).GetComponent<SpriteRenderer>();
        }
    }

    internal static List<SpriteRenderer> setEventArea(this CreateMapEventValues values, List<Vector2> createList, 
        List<Transform> transformList,List<int> eventList,bool isInitState)
    {
        List<SpriteRenderer> eventAreaSprites = new();

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        for (int i = 0; i < createList.Count; i++)
        {
            spawnPosition = createList[i];
            spawnPosition.z = spawnPosition.y;
            spawnRotation = Quaternion.identity;

            Transform tempObj;
            if (eventList != null)
                tempObj = Object.Instantiate(values.eventObject[eventList[i]]).transform;
            else
                tempObj = Object.Instantiate(values.eventObject[0]).transform;
            transformList.Add(tempObj);
            tempObj.parent = values.parentTransform;
            tempObj.name = i + " pos ";
            tempObj.position = spawnPosition;
            tempObj.rotation = spawnRotation;
            eventAreaSprites.Add(_GetSpriteRenderer_EventNode(tempObj));

            createList[i] = tempObj.position;
        }

        return eventAreaSprites;

        SpriteRenderer _GetSpriteRenderer_EventNode(Transform instant)
        {
            return instant.GetChild(0).GetComponent<SpriteRenderer>();
        }
    }

    // V3 data -> int
    internal static int terrainDataToIndex(Vector3 terrainData)
    {
        float MAX = Mathf.Max(terrainData.x, terrainData.y, terrainData.z);
        float[] tmpAry = { terrainData.x, terrainData.y, terrainData.z };
        
        int i = 0;
        for (; i < tmpAry.Length; i++)
        {
            if (tmpAry[i] == MAX)
                break;
        }

        return i;
    }

    internal static void setRoadAreaSetBySet(this CreateRoadValues values, NodeScriptPerLevel nextLevel, NodeScriptPerLevel createdLevel,int childStd)
    {
        for (int index = 0; index < nextLevel.connectingData.Count; index++)
        {
            for (int dest = (int)nextLevel.connectingData[index].x; dest < nextLevel.connectingData[index].y + 1; dest++)
            {
                int source = index + childStd;
                RoadSet temp = setRoadArea(new Vector2Int(source, dest));
                setRoadTerrainData(temp);
                createdLevel.roadSetList.Add(temp);
            }
        }

        return;

        RoadSet setRoadArea(Vector2Int route)
        {
            RoadSet roadAreaSprites = new();
            Vector3 srcPosV3 = nextLevel.transformList[route.x].position;
            Vector3 desPosV3 = createdLevel.transformList[route.y].position;

            List<Vector2> dotPos = GetPointsRandomCurve(srcPosV3, desPosV3, values.gap, values.coef);

            roadAreaSprites.nameV3 = new Vector3Int(createdLevel.level, route.x, route.y);


            Transform DEBUG_trans = null;
            if (values.isDebug_RoadVis)
                DEBUG_trans = DEBUG_init(roadAreaSprites.nameV3 + "");

            DEBUG_trans.position = srcPosV3;
            for (int j = 0; j < dotPos.Count; j++)
            {
                Vector3 spawnPosition = dotPos[j];

                if (Vector2.Distance(srcPosV3, spawnPosition) < values.ignoreRange)
                    continue;
                if (Vector2.Distance(desPosV3, spawnPosition) < values.ignoreRange)
                    continue;

                spawnPosition.z = spawnPosition.y;

                Quaternion spawnRotation = Quaternion.identity;

                Transform tempObj = Object.Instantiate(values.roadObject).transform;
                tempObj.parent = DEBUG_trans;
                tempObj.position = spawnPosition;
                tempObj.rotation = spawnRotation;
                roadAreaSprites.roadSet.Add(tempObj.GetChild(0).GetComponent<SpriteRenderer>());

                //DEBUG
                //tempObj.localScale *= ((float)j / (float)dotPos.Count);
            }

            return roadAreaSprites;

            Transform DEBUG_init(string _name)
            {
                Transform unitRoad = new GameObject().transform;
                unitRoad.name = "Road - " + _name;
                unitRoad.parent = values.parentTransform;
                return unitRoad;
            }
        }

        void setRoadTerrainData(RoadSet _data)
        {
            _data.srcTerrain = nextLevel.nodeTerrainData[_data.nameV3.y];
            _data.desTerrain = createdLevel.nodeTerrainData[_data.nameV3.z];
        }
    }

    internal static void setRoadSpriteByData(this CreateMapBackgroundValues values, NodeScriptPerLevel _createdLevel)
    {
        int std_CUTOFF = 10;
        float std_DISTANCE = 0.8f;
        for (int i = 0; i < _createdLevel.roadSetList.Count; i++)
        {
            RoadSet target = _createdLevel.roadSetList[i];
            
            for (int pointIndex = 0; pointIndex < target.roadSet.Count; pointIndex++)
            {
                if (pointIndex < std_CUTOFF)
                    continue;
                if (target.roadSet.Count - pointIndex < std_CUTOFF)
                    continue;

                Transform prev = target.roadSet[pointIndex - 1].transform;
                Transform curr = target.roadSet[pointIndex].transform;
                Transform next = target.roadSet[pointIndex + 1].transform;

                SpriteRenderer temp = 
                    setBackgroundSprites_Road(curr.position, terrainDataToIndex(target.desTerrain), prev, curr, next);
                if(temp != null)
                    temp.material = values.mapMaterial;

            }
        }

        return;

        // BG obj 관련
        SpriteRenderer setBackgroundSprites_Road(Vector2 stdPos, int index, Transform prev, Transform curr, Transform next)
        {
            return null;
            Vector2 tempV2 = GetRandomPointInCircle(Vector2.one) * 0.4f;

            if (tempV2 != Vector2.zero)
                tempV2 = tempV2.normalized * std_DISTANCE;
            if (Vector3.Distance(prev.position, stdPos + tempV2) < std_DISTANCE) return null;
            if (Vector3.Distance(curr.position, stdPos + tempV2) < std_DISTANCE) return null;
            if (Vector3.Distance(next.position, stdPos + tempV2) < std_DISTANCE) return null;

            Transform insTrans = Object.Instantiate(values.bgPrefabList[index]).transform;
            insTrans.parent = values.parentTransform;
            insTrans.localPosition = stdPos + tempV2;
            insTrans.localScale *= UnityEngine.Random.Range(1f, 1f);
            insTrans.gameObject.layer = 0;


            return insTrans.GetComponent<SpriteRenderer>();
        }
    }


    internal static NodeScriptPerLevel tryAddLevel(this List<NodeScriptPerLevel> data,int level)
    {
        if (data.Count != level)
        {
            Debug.Log(data.Count + " != " + level);
            return data[level];
        }

        NodeScriptPerLevel rtn = new(level);
        data.Add(rtn);
        return rtn;
    }

    internal static Transform getTransformByGridPos(this List<NodeScriptPerLevel> data, Vector2Int focusNodePos)
    {
        if (data[focusNodePos.x].transformList == null)
        { Debug.Log("sad"); return null; }
        //Debug.Log("saddd " + focusNodePos);
        return data[focusNodePos.x].transformList[focusNodePos.y];
    }

    internal static int setConnectedData(this List<Vector2> target, List<int> data)
    {
        if (target == null)
        {
            Debug.Log("sad");
            return 0;
        }

        int curr = 0;
        int temp = 0;
        for (int i = 0; i < data.Count; i++)
        {
            if (i != 0 && i != data.Count - 1 )
            {
                temp++;
            }

            int next = data[i] + temp;
            target.Add(new Vector2(curr, next));
            curr = next;
        }

        if(target.Count == 1)
        {
            target[0] = new Vector2(0, target[0].y - 1);
        }

        return 1;
    }

    internal static Vector2 getChildRangeByGridPos(this List<NodeScriptPerLevel> data, Vector2Int gridNodePos,int std = 0)
    {
        if (gridNodePos.x == 0)
            return new Vector2(0, data[0].transformList.Count);

        return data[gridNodePos.x].connectingData[gridNodePos.y] + Vector2.one * std;
    }
    internal static Vector2 getChildRangeByGridPos_FocusStd(this List<NodeScriptPerLevel> data, Vector2Int gridNodePos)
    {
        int stdValue = 0;
        if (gridNodePos.x - 1 >= 0)
        {
            stdValue = data[gridNodePos.x - 1].childStd;
        }

        if (gridNodePos.x == 0)
            return new Vector2(0, data[1].transformList.Count - 1);


        return data[gridNodePos.x].connectingData[gridNodePos.y- stdValue];
    }


    public static Sprite Capture(this CaptureScreenshotValue value)
    {
        RenderTexture renderTexture = new RenderTexture(value.captureWidth, value.captureHeight, 0);
        value.captureCam.targetTexture = renderTexture;
        value.captureCam.Render();

        Texture2D screenshotTexture = new Texture2D(value.captureWidth, value.captureHeight, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        screenshotTexture.ReadPixels(new Rect(0, 0, value.captureWidth, value.captureHeight), 0, 0);
        screenshotTexture.Apply();

        return Sprite.Create(screenshotTexture, new Rect(0, 0, renderTexture.width, renderTexture.height), Vector2.one*0.5f);
    }

    public static void SetSizeCaptureCam(this CreateMap CreateMapTool, Vector2Int v2)
    {
        CreateMapTool.createMapBackgroundValues.countX = v2.x;
        CreateMapTool.createMapBackgroundValues.countY = v2.y;
    }

    public static void SetNodeMaterialActive(this SpriteRenderer targetNode, Material active)
    {
        targetNode.transform.GetComponent<NodeController>().ActiveDetailed();
    }

}