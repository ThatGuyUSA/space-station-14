using Content.Server.Polymorph.Systems;
using Content.Shared.EntityTable;
using Content.Shared.Polymorph;
using Content.Shared.Trigger;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Linq;
using Content.Shared.Mobs.Components;
using Robust.Shared.Map.Components;

namespace Content.Server.Trigger.Systems;

public sealed partial class PolymorphOnTriggerSystem : EntitySystem
{
    [Dependency] private readonly EntityTableSystem _entityTable = null!;
    [Dependency] private readonly PolymorphSystem _polymorph = null!;
    [Dependency] private readonly IPrototypeManager _proto = null!;
    [Dependency] private readonly IRobustRandom _random = null!;
    private readonly List<string> _randomPolyMobList = new();

    /// <summary>
    /// Need to do this so we don't get a collection enumeration error in physics by polymorphing
    /// an entity we're colliding with in case of TriggerOnCollide.
    /// Also makes sure other trigger effects don't activate in nullspace after we have polymorphed.
    /// </summary>
    private Queue<(EntityUid Uid, ProtoId<PolymorphPrototype> Polymorph)> _queuedPolymorphUpdates = new();

    public override void Initialize()
    {
        base.Initialize();
        BuildIndex();

        SubscribeLocalEvent<PolymorphOnTriggerComponent, TriggerEvent>(OnTrigger);

        if (_entityTable.GetSpawns(_entityTable) != null!)
            ent.Comp.Polymorph.Id = _random.Pick(_randomPolyMobList);
    }

    private void BuildIndex()
    {
        _randomPolyMobList.Clear();
        var mapGridCompName = Factory.GetComponentName<MapGridComponent>();
        var mobCompName = Factory.GetComponentName<MobStateComponent>();

        // scrape all proto, blacklist abstract, hidden, and grids though
        foreach (var proto in _proto.EnumeratePrototypes<EntityPrototype>())
        {
            if (proto.Abstract || proto.HideSpawnMenu || proto.Components.ContainsKey(mapGridCompName) || !proto.Components.ContainsKey(mobCompName))
                continue;

            _randomPolyMobList.Add(proto.ID);
        }
    }

    private void OnTrigger(Entity<PolymorphOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;


        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        // if (ent.Comp.PolyTable != default!)
        // {
        //     var enumTable = _entityTable.GetSpawns(ent.Comp.PolyTable);
        //     ent.Comp.Polymorph. = _random.Pick(enumTable.ToList());
        // }

        _queuedPolymorphUpdates.Enqueue((target.Value, ent.Comp.Polymorph));
        args.Handled = true;
    }

    public override void Update(float frametime)
    {
        while (_queuedPolymorphUpdates.TryDequeue(out var data))
        {
            if (TerminatingOrDeleted(data.Uid))
                continue;

            _polymorph.PolymorphEntity(data.Uid, data.Polymorph);
        }
    }
}
