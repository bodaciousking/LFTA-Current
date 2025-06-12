using UnityEngine;

public interface IAbilityEffects 
{
    void Apply(Unit caster, Ability castedAbility, Unit target, Tile tileTarget);    
}

public class Damage : IAbilityEffects
{
    public void Apply(Unit caster, Ability castedAbility, Unit target, Tile tileTarget)
    {
        target.health.x -= castedAbility.damage;
        throw new System.NotImplementedException();
    }
}