using UnityEngine;

public class HardRunner : BaseRunner
{
    public override void fillObjectsToPattern()
    {
        base.fillObjectsToPattern();
        fillGameObjectsToPattern(8, 4);
    }

    public override void fillObjectsSprite()
    {
        base.fillObjectsSprite();
        fillObjectsWithSprites(8, 4);
    }
}