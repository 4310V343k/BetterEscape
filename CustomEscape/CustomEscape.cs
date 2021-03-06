﻿using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using Map = Exiled.Events.Handlers.Map;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace CustomEscape
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CustomEscape : Plugin<Configs>
    {
        public static CustomEscape Singleton;

        private static Harmony _harmony;
        public override string Author { get; } = "Remindme";
        public override string Name { get; } = "Custom Escapes";
        public override string Prefix { get; } = "bEscape";
        public override Version Version { get; } = new Version(3, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 10, 0);
        public override PluginPriority Priority { get; } = PluginPriority.Highest;

        public override void OnEnabled()
        {
            _harmony = new Harmony(Prefix + DateTime.Now.Ticks);

            Singleton = this;

            Config.TryCreateFile();

            _harmony.PatchAll();

            Player.ChangingRole += EventHandlers.OnChangingRole;
            Player.Escaping += EventHandlers.OnEscaping;
            Server.RoundEnded += EventHandlers.OnRoundEnded;
            Map.Generated += EventHandlers.OnGenerated;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.ChangingRole -= EventHandlers.OnChangingRole;
            Player.Escaping -= EventHandlers.OnEscaping;
            Server.RoundEnded -= EventHandlers.OnRoundEnded;
            Map.Generated -= EventHandlers.OnGenerated;

            Singleton = null;

            _harmony.UnpatchAll();
            _harmony = null;

            base.OnDisabled();
        }
    }
}