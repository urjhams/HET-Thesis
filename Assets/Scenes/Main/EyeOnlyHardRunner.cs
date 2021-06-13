using UnityEngine;

public class EyeOnlyHardRunner : EyeOnlyBaseRunner
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