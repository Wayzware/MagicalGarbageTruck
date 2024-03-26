using Game;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace MagicalGarbageTruck;
public sealed partial class GarbageNotificationRemoverSystem : GameSystemBase
{
    private const int GARBAGE_ICON_INDEX = 10; // there is probably a better way to figure out which icon is the garbage one, but I couldn't find one after trying for a bit

    private EntityQuery _iconPrefabQuery;
    private EntityQuery _notificationsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _iconPrefabQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = [
                ComponentType.ReadWrite<NotificationIconDisplayData>(),
            ],
            None = [
                ComponentType.ReadOnly<Deleted>(),
                ComponentType.ReadOnly<Temp>()
            ]
        });

        _notificationsQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All =
            [
                ComponentType.ReadOnly<Icon>(),
                ComponentType.ReadOnly<PrefabRef>()
            ],
            None =
            [
                ComponentType.ReadOnly<Deleted>(),
                ComponentType.ReadOnly<Temp>()
            ]
        });

        Mod.log.Info("Injected GarbageNotificationRemoverSystem!");
        RequireForUpdate(_iconPrefabQuery);
        RequireForUpdate(_notificationsQuery);
    }

    protected override void OnUpdate()
    {
        var icons = _iconPrefabQuery.ToEntityArray(Allocator.Temp);

        Entity garbageIcon = default;
        foreach(var icon in icons)
        {
            var data = EntityManager.GetComponentData<NotificationIconDisplayData>(icon);
            if(data.m_IconIndex == GARBAGE_ICON_INDEX)
            {
                garbageIcon = icon;
                break;
            }
        }

        if(garbageIcon == default)
        {
            Mod.log.Error("No garbage truck problem icon prefab found!");
            Enabled = false;
            return;
        }

        var problems = _notificationsQuery.ToEntityArray(Allocator.Temp);
#if DEBUG
        Mod.log.Info($"garbageIcon: {garbageIcon}, problem length: {problems.Length}");
        var deleted = 0L;
#endif
        foreach(var problem in problems)
        {
            var prefabRef = EntityManager.GetComponentData<PrefabRef>(problem);
            if(prefabRef == garbageIcon)
            {
                EntityManager.AddComponent<Deleted>(problem);
#if DEBUG
                deleted++;
#endif
            }
        }

#if DEBUG
        Mod.log.Info($"Deleted: {deleted}");
#endif
    }
}
