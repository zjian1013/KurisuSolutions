using System;
using System.Linq;
using LeagueSharp.Common;
using LeagueSharp;
using Color = System.Drawing.Color;

namespace KurisuFiora
{
    // KurisuFiora is not ready
    throwError

    internal class KurisuFiora
    {
        private static Menu mainmenu;
        private static int timer;
        private static float incomedamage;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public KurisuFiora()
        {
            Console.WriteLine("KurisuFiora is loading...");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static Spell q, w, e, r;
        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Me.ChampionName != "Fiora")
                return;

            // Init spells
            q = new Spell(SpellSlot.Q, 600f);
            w = new Spell(SpellSlot.W);
            e = new Spell(SpellSlot.E);
            r = new Spell(SpellSlot.R, 350f);

            // Init root menu
            mainmenu = new Menu("KurisuFiora", "kfiora", true);

            var oMenu = new Menu("Fiora: Orbwalker", "forb");
            LxOrbwalker.AddToMenu(oMenu);

            // Init combo menu
            var cMenu = new Menu("Fiora: Combo", "combo");
            cMenu.AddItem(new MenuItem("sep1", "Q - Settings"));
            cMenu.AddItem(new MenuItem("qenable", "Use Q")).SetValue(true);
            cMenu.AddItem(new MenuItem("qrange", "Use Smart Q")).SetValue(true);
            cMenu.AddItem(new MenuItem("qminion", "Use Q on Minion")).SetValue(true);
            cMenu.AddItem(new MenuItem("minrange", "Minimum range to Q")).SetValue(new Slider(100, 50, 500));
            cMenu.AddItem(new MenuItem("sep2", "W - Settings"));
            cMenu.AddItem(new MenuItem("wenable", "Use W")).SetValue(true);
            cMenu.AddItem(new MenuItem("wparry2", "W mode: ")).SetValue(new StringList(new [] {"Always", "Combo"}));
            cMenu.AddItem(new MenuItem("wparry", "W only if enemy dist <= ")).SetValue(new Slider(50, 10, 500));
            cMenu.AddItem(new MenuItem("sep3", "E - Settings"));
            cMenu.AddItem(new MenuItem("eenable", "Use E")).SetValue(true);
            cMenu.AddItem(new MenuItem("ereset", "Wait for AA Reset")).SetValue(true);
            cMenu.AddItem(new MenuItem("sep4", "R - Settings"));
            cMenu.AddItem(new MenuItem("renable", "Use R")).SetValue(true);
            mainmenu.AddSubMenu(cMenu);

            // Init keybind menu
            var kMenu = new Menu("Fiora: Keybinds", "kbind");
            kMenu.AddItem(new MenuItem("usecombo", "Combo Key")).SetValue(new KeyBind(32, KeyBindType.Press));
            mainmenu.AddSubMenu(kMenu);

            // Init draw menu
            var dMenu = new Menu("Fiora: Drawings", "fdraw");
            dMenu.AddItem(new MenuItem("disabledrawing", "Disable drawing")).SetValue(false);
            dMenu.AddItem(new MenuItem("dbounding", "Draw bounding radius")).SetValue(false);
            dMenu.AddItem(new MenuItem("dattackrange", "Draw attack range")).SetValue(true);
            dMenu.AddItem(new MenuItem("dlungerange", "Draw Q range")).SetValue(true);
            mainmenu.AddSubMenu(dMenu);


            var mMenu = new Menu("Fiora: Misc", "fmisc");
            mMenu.AddItem(new MenuItem("igniteon", "Use ignite")).SetValue(true);
            mMenu.AddItem(new MenuItem("itemson", "Use items")).SetValue(true);
            mMenu.AddItem(new MenuItem("packets", "Cast with packets")).SetValue(true);
            mainmenu.AddSubMenu(mMenu);

            // Add menu
            mainmenu.AddToMainMenu();

            // OnGameProcessPacket Event
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;

            // OnProcessSpellCast Event
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            // OnDraw Event
            Drawing.OnDraw += Drawing_OnDraw;

            // Print Chat
            Game.PrintChat("KurisuFiora - Loaded");
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (mainmenu.Item("disabledrawing").GetValue<bool>())
                return;

            if (!Me.IsDead && Me.IsValid)
            {
                if (mainmenu.Item("dbounding").GetValue<bool>())
                    Utility.DrawCircle(Me.Position, Me.BoundingRadius, Color.White);
                if (mainmenu.Item("dattackrange").GetValue<bool>())
                    Utility.DrawCircle(Me.Position, Me.AttackRange, Color.White);
                if (mainmenu.Item("dlungerange").GetValue<bool>())
                    Utility.DrawCircle(Me.Position, q.Range, q.IsReady() ? Color.White : Color.Gray);
                if (mainmenu.Item("ultrange").GetValue<bool>())
                    Utility.DrawCircle(Me.Position, r.Range, r.IsReady() ? Color.White : Color.Gray);
            }
        }

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            var packet = new GamePacket(args.PacketData);
            if (packet.Header == 0x65 && mainmenu.Item("usecombo").GetValue<KeyBind>().Active) 
            {
                packet.Position = 0x10;
                var sourcenetworkid = packet.ReadInteger(1);
                if (sourcenetworkid != Me.NetworkId)
                    return;

                packet.Position = 0x1;
                var damagetype = packet.ReadByte();

                if (damagetype == 0x4 || damagetype == 0x3)
                {
                    if (mainmenu.Item("eenable").GetValue<bool>())
                    {
                        if (e.IsReady())
                        {
                            e.Cast();
                            LxOrbwalker.ResetAutoAttackTimer();
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            // reset after each cast
            incomedamage = 0;

            // check if sender is valid
            if (sender.IsEnemy && sender.Type == Me.Type)
            {
                // get attacker and spellslot
                var attacker = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                var attackerslot = attacker.GetSpellSlot(args.SData.Name);

                if (attackerslot != SpellSlot.Unknown)
                    return;

                var dist = mainmenu.Item("wrange").GetValue<Slider>().Value; 

                incomedamage = (float) attacker.GetAutoAttackDamage(Me);

                if (attacker.Distance(Me.ServerPosition) <= dist)
                {
                    var damagePercent = incomedamage / Me.MaxHealth * 100;
                    if (damagePercent >= 1)
                    {
                        if (w.IsReady())
                            w.Cast();
                    }
                }

            }

            else if (sender.IsMe)
            {
                var senderslot = Me.GetSpellSlot(args.SData.Name);
                if (senderslot == SpellSlot.Q)
                {
                   timer = Environment.TickCount;
                }

                else if (senderslot == SpellSlot.E)
                {
                    Utility.DelayAction.Add(Game.Ping + 70, delegate
                    {
                        if (Items.HasItem(3077) && Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.HasItem(3074) && Items.CanUseItem(3074))
                            Items.UseItem(3074);                  
                    });             
                }
            }
        }

        private static void GapCloseTarget(Obj_AI_Hero target)
        {
            var inst = Me.Spellbook.GetSpell(SpellSlot.Q);
            if (inst.Name == "qtwo") 
                return;

            foreach (
                var obj in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(m => m.IsValidTarget(q.Range) && m.Distance(target.ServerPosition) <= q.Range + 2))
            {
                q.Cast(obj);
            }
        }

        private static void CastCombo(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(900))
            {
                if (target.Distance(Me.ServerPosition) > q.Range * 2)
                {
                    if (target.Health <= ComboDamage(target) ||
                        target.Health <= Me.GetSpellDamage(target, SpellSlot.Q))
                    {
                        GapCloseTarget(target);
                    }
                }

                else if (target.Distance(Me.ServerPosition) <= q.Range)
                {

                    if (target.Distance(Me.ServerPosition) > mainmenu.Item("minrange").GetValue<Slider>().Value)
                    {
                        var inst = Me.Spellbook.GetSpell(SpellSlot.Q);
                        if (inst.Name != "firstQ")
                        {
                            if (timer + 4000 < Environment.TickCount ||
                                target.Health <= Me.GetSpellDamage(target, SpellSlot.Q))
                            {
                                q.Cast(target);
                            }
                        }
                    }
                }

                else if (target.Health <= Me.GetSpellDamage(target, SpellSlot.R))
                {
                    r.Cast(target);
                }
            }
        }

        private static float ComboDamage(Obj_AI_Base target)
        {
            var qq = q.IsReady() ? Me.GetSpellDamage(target, SpellSlot.Q) : 0;
            var ww = w.IsReady() ? Me.GetSpellDamage(target, SpellSlot.W) : 0;
            var rr = r.IsReady() ? Me.GetSpellDamage(target, SpellSlot.R) : 0;

            var tmt = Items.HasItem(3077) && Items.CanUseItem(3077) ? Me.GetItemDamage(target, Damage.DamageItems.Tiamat) : 0;
            var hyd = Items.HasItem(3074) && Items.CanUseItem(3074) ? Me.GetItemDamage(target, Damage.DamageItems.Hydra) : 0;
            var bwc = Items.HasItem(3144) && Items.CanUseItem(3144) ? Me.GetItemDamage(target, Damage.DamageItems.Bilgewater) : 0;
            var brk = Items.HasItem(3153) && Items.CanUseItem(3153) ? Me.GetItemDamage(target, Damage.DamageItems.Botrk) : 0;

            var items = tmt + hyd + bwc + brk;
            var damage = items + qq + ww + rr;

            return (float) damage;
        }
    }
}
