using Game;
using Game.Buildings;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace MagicalGarbageTruck;

public sealed partial class MagicalGarbageTruckSystem : GameSystemBase
{
    private EntityQuery _garbageProducerQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _garbageProducerQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All =
            [
                ComponentType.ReadWrite<GarbageProducer>()
            ],
            None =
            [
                ComponentType.ReadOnly<Deleted>(),
                ComponentType.ReadOnly<Temp>()
            ]
        });

        Mod.log.Info("Injected GarbageFixerSystem!");
        RequireForUpdate(_garbageProducerQuery);
    }

    protected override void OnUpdate()
    {
        var garbageProducers = _garbageProducerQuery.ToEntityArray(Allocator.Temp);
        foreach (var entity in garbageProducers)
        {
            var garbage = EntityManager.GetComponentData<GarbageProducer>(entity);
            if (garbage.m_Garbage > 0)
            {
                var collectionRequest = garbage.m_CollectionRequest;

                garbage.m_Garbage = 0;
                garbage.m_Flags = GarbageProducerFlags.None;
                garbage.m_CollectionRequest = default;

                EntityManager.SetComponentData(entity, garbage);

                if (collectionRequest != default)
                {
                    EntityManager.AddComponent<Deleted>(collectionRequest);
                }
            }
        }
    }
}

