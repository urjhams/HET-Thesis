public class EasyRunner : BaseRunner
{
    public override void fillObjectsToPattern()
    {
        base.fillObjectsToPattern();
        fillGameObjectsToPattern(4, 2);
    }

    public override void fillObjectsSprite()
    {
        base.fillObjectsSprite();
        fillObjectsWithSprites(4, 2);
    }
}
