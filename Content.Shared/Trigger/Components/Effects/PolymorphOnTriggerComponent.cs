using Content.Shared.EntityTable;
using Content.Shared.Polymorph;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Trigger.Components.Effects;

/// <summary>
/// Polymorphs the entity when triggered.
/// If TargetUser is true it will transform the user instead.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PolymorphOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// Polymorph settings.
    /// </summary>
    [DataField]
    public ProtoId<PolymorphPrototype> Polymorph;

    /// <summary>
    /// True random polymorph choosing from any MobState, debug or otherwise.
    /// </summary>
    [DataField]
    public bool RandomMobState;

    /// <summary>
    /// A different method of choosing, which picks from a table of prototypes
    /// the target will turn into rather than just one target entity
    /// </summary>
    [DataField]
    public EntityTablePrototype PolyTable = null!;
}
