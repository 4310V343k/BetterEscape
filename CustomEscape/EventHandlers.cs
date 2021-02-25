﻿using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEscape
{
    public class EventHandlers
    {
        public static GameObject EscapePos;
        public void OnGenerated()
        {
            EscapePos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Log.Debug("created a sphere", CustomEscape.singleton.Config.Debug);
            EscapePos.transform.localScale = new Vector3(0, 0, 0); // stop bumping into that shit
            EscapePos.transform.localPosition = new Vector3(170, 984, 26); // somehow it is not triggering properly if this is not set
            Log.Debug($"modified the sphere: {EscapePos.transform.localScale}, {EscapePos.transform.localPosition}", CustomEscape.singleton.Config.Debug);

            SphereCollider collider = EscapePos.AddComponent<SphereCollider>();
            Log.Debug("attached a collider", CustomEscape.singleton.Config.Debug);
            collider.center = new Vector3(170, 984, 26);
            collider.radius = 2f;
            collider.isTrigger = true;
            Log.Debug($"modified the collider: {collider.center}, {collider.radius}, {collider.isTrigger}", CustomEscape.singleton.Config.Debug);

            EscapePos.AddComponent<CustomEscapeComponent>();
            Log.Debug("attached an escape component", CustomEscape.singleton.Config.Debug);
        }
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Object.Destroy(EscapePos.gameObject); // lets hope unity handles the components for us
            Log.Debug("destroyed the sphere", CustomEscape.singleton.Config.Debug);
        }
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.IsEscaped) return;

            if (!CustomEscape.singleton.Config.RoleConversions.TryGetValue(ev.Player.Role, out PrettyCuffedConfig value)) return;

            RoleType role = ev.Player.IsCuffed ? value.CuffedRole : value.UncuffedRole;
            Log.Debug($"changingrole: {ev.Player.Role} to {role}, cuffed: {ev.Player.IsCuffed}", CustomEscape.singleton.Config.Debug);
            ev.NewRole = role;

            // Because SetRole() is called with Player's current role, the items thing is not handled properly and the inventory is changed here, so exiled can change the inventory itself
            ev.Items.Clear();
            ev.Items.AddRange(ev.Player.ReferenceHub.characterClassManager.Classes.SafeGet(ev.NewRole).startItems);
        }
        public void OnEscaping(EscapingEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            /*
			 * Those checks are here and not in OnChangingRole() because
			 * 1. I need the IsAllowed property which is not present in ChangingRole
			 * 2. Other plugins can override the NewRole and it will affect the logic
			 */
            if (ev.NewRole == RoleType.None)
            {
                ev.IsAllowed = false;
                Log.Debug($"but not allowed", CustomEscape.singleton.Config.Debug);
            }
            if (ev.NewRole == RoleType.Spectator)
            {
                Timing.CallDelayed(0.1f, () => ev.Player.Position = Map.GetRandomSpawnPoint(ev.Player.Role));
                Log.Debug($"moved spectator out of the way: {ev.Player.Nickname}", CustomEscape.singleton.Config.Debug);
            }
        }
    }
}
