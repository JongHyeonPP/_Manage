

float2 randomVector(float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y * +offset) * 0.4 + 0.5, cos(UV.x * offset) * 0.4 + 0.5);
}

void CustomVoronoi_float(float2 UV, float AngleOffset, float CellDensity, out float DistFromCenter, out float DistFromEdge, out float myDEBUG)
{
    int2 gridPos = floor(UV * CellDensity);
    float2 detailedPos = frac(UV * CellDensity);

    DistFromCenter = 8.0f;
    float2 firstOffset = float2(0, 0);
    int2 firstGrid = int2(0, 0);
    for (int y = -1; y <= 1; ++y)
    {
        for (int x = -1; x <= 1; ++x)
        {
            float2 checkingDir = float2(x, y);
            float2 cellOffset = checkingDir - detailedPos + randomVector(checkingDir + gridPos, AngleOffset);
            float distToPoint = length(cellOffset);

            if (distToPoint < DistFromCenter)
            {
                DistFromCenter = distToPoint;
                firstOffset = cellOffset;
                firstGrid = int2(x, y);
            }
        }
    }

    float2 firstV2_detailed = (firstOffset + detailedPos + gridPos) / CellDensity;
    float2 firstV2_cells = firstOffset + detailedPos - firstGrid;

    DistFromEdge = 8.0f;
    DistFromEdge = (firstV2_cells.x * firstV2_cells.y);

    myDEBUG = length(firstV2_detailed - float2(0.5, 0.5));
}

// - float2(0.5 * CellDensity,0.5 * CellDensity)