using Content.Shared.Actions;
using Robust.Shared.Prototypes;

namespace Content.Shared.Magic.Events;

public sealed partial class HomingProjectileSpellEvent : WorldTargetActionEvent
{
    /// <summary>
    /// What entity should be spawned.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId Prototype;

    public void HomingSpellLaunchSequence(EntityUid caster, EntityUid target, EntProtoId proto, float distance, float speed)
    {
        // I'm STEALING this from the admin smite system!

        var spawnCoords = _transformSystem.GetMapCoordinates(caster);
        var projectile = Spawn(proto, spawnCoords);
        // Here we abuse the ChasingWalkComp by making it skip targetting logic and dialling its frequency up
        EnsureComp<ChasingWalkComponent>(rod, out var chasingComp);
        chasingComp.NextChangeVectorTime = TimeSpan.MaxValue; // we just want it to never change
        chasingComp.ChasingEntity = target;
        chasingComp.ImpulseInterval = .1f; // skrrt skrrrrrrt skrrrt
        chasingComp.RotateWithImpulse = true;
        chasingComp.MaxSpeed = speed;
        chasingComp.Speed = speed; // tell me lies, tell me sweet little lies.

        if (TryComp<TimedDespawnComponent>(rod, out var despawn))
            despawn.Lifetime = offset.Length() / speed * 3; // exists thrice as long as it takes to get to you.
    }
}
