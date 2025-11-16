using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KnockbackSystem : MonoBehaviour
{
    Combat combat = Combat.instance;



    private static void HandleOutOfBounds(Unit target)
    {
        Debug.Log($"{target.name} was knocked off the map!");
        // Optional: play falling animation, apply damage, etc.
    }

    private static void HandleWallCollision(Unit target, Tile wallTile)
    {
        Debug.Log($"{target.name} slammed into a wall at {wallTile.tilePos}");
        // Optional: stop knockback, play impact animation, deal damage
    }

    private static void HandleUnitCollision(Unit target, Unit other)
    {
        Debug.Log($"{target.name} collided with {other.name}");
        // Optional: push both, stop movement, apply stagger
    }

    private static void HandleFall(Unit target, Tile fromTile, Tile toTile)
    {
        int fallHeight = (int)(fromTile.tilePos.y - toTile.tilePos.y);
        Debug.Log($"{target.name} fell from height {fromTile.tilePos.y} to {toTile.tilePos.y}");

       // target.FallTo(toTile, fallHeight);
    }
}
