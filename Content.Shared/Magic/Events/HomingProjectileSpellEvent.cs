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

    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

    public void HomingSpellLaunchSequence(EntityUid caster, EntityUid target, EntProtoId proto, float distance, float speed)
    {
        // I'm STEALING this from the admin smite system!

        var spawnCoords = _transformSystem.GetMapCoordinates(caster);
        var projectile = true;
        // Here we abuse the ChasingWalkComp by making it skip targetting logic and dialling its frequency up
    }
}
