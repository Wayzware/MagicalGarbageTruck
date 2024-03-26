﻿using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace MagicalGarbageTruck;

public class Mod : IMod
{
    public static ILog log = LogManager.GetLogger($"{nameof(MagicalGarbageTruck)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

    public void OnLoad(UpdateSystem updateSystem)
    {
        log.Info(nameof(OnLoad));

        if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            log.Info($"Current mod asset at {asset.path}");

        updateSystem.UpdateAfter<GarbageNotificationRemoverSystem>(SystemUpdatePhase.Deserialize);
        updateSystem.UpdateAfter<MagicalGarbageTruckSystem>(SystemUpdatePhase.Deserialize);
        updateSystem.UpdateAfter<MagicalGarbageTruckSystem>(SystemUpdatePhase.GameSimulation);
    }

    public void OnDispose()
    {
        log.Info(nameof(OnDispose));
    }
}
