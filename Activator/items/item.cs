using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Activator.Items
{
    public class item
    {
        internal virtual int Id { get; set; }
        internal virtual int Priority { get; set; }
        internal virtual int Duration { get; set; }
        internal virtual bool Craving { get; set; }
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual float Range { get; set; }
        internal virtual MenuType[] Category { get; set; }
        internal virtual MapType[] Maps { get; set; }

        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent { get { return Menu.Parent; } }
        public Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public champion Tar
        {
            get
            {
                return
                    champion.Heroes.Where(
                        hero => hero.Player.IsEnemy && hero.Player.IsValidTarget(Range) &&
                               !hero.Player.IsZombie).OrderBy(x => x.Player.Distance(Game.CursorPos)).FirstOrDefault();
            }
        }

        public static IEnumerable<item> PriorityList()
        {
            var hpi = from ii in spelldata.items
                      where  ii.Craving && LeagueSharp.Common.Items.CanUseItem(ii.Id)
                      orderby ii.Menu.Item("prior" + ii.Name).GetValue<Slider>().Value descending 
                      select ii;

            return hpi;
        }
           
        public void UseItem(bool combo = false)
        {
            Craving = true;

            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (PriorityList().Any())
                {
                    if (Name == PriorityList().First().Name)
                    {
                        if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Duration)
                        {
                            LeagueSharp.Common.Items.UseItem(Id);
                            Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                            Activator.LastUsedDuration = Duration;
                        }
                    }
                }
            }

            Craving = false;
        }

        public void UseItem(Obj_AI_Base target, bool combo = false)
        {
            Craving = true;

            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (PriorityList().Any())
                {
                    if (Name == PriorityList().First().Name)
                    {
                        if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Duration)
                        {
                            LeagueSharp.Common.Items.UseItem(Id, target);
                            Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                            Activator.LastUsedDuration = Duration;
                        }
                    }
                }
            }

            Craving = false;
        }

        public void UseItem(Vector3 pos, bool combo = false)
        {
            Craving = true;

            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (PriorityList().Any() && Name == PriorityList().First().Name)
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Duration)
                    {
                        LeagueSharp.Common.Items.UseItem(Id, pos);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Duration;
                    }
                }
            }

            Craving = false;
        }

        public item CreateMenu(Menu root)
        {
            var usefname = DisplayName ?? Name;

            Menu = new Menu(Name, "m" + Name);
            Menu.AddItem(new MenuItem("use" + Name, "Use " + usefname)).SetValue(true);
            Menu.AddItem(new MenuItem("prior" + Name, DisplayName + " Priority")).SetValue(new Slider(Priority, 1, 7));

            if (Category.Any(t => t == MenuType.EnemyLowHP))
            {
                Menu.AddItem(new MenuItem("EnemyLowHP" + Name + "Pct", "Use on Enemy HP % <="))
                    .SetValue(new Slider(DefaultHP));
            }

            if (Category.Any(t => t == MenuType.SelfLowHP))
                Menu.AddItem(new MenuItem("SelfLowHP" + Name + "Pct", "Use on Hero HP % <="))
                    .SetValue(new Slider(Name == "Botrk" ? 70 : DefaultHP/2));

            if (Category.Any(t => t == MenuType.SelfMuchHP))
                Menu.AddItem(new MenuItem("SelfMuchHP" + Name + "Pct", "Use on Hero Dmg Dealt % >="))
                    .SetValue(new Slider(45));

            if (Category.Any(t => t == MenuType.SelfLowMP))
                Menu.AddItem(new MenuItem("SelfLowMP" + Name + "Pct", "Use on Hero Mana % <="))
                    .SetValue(new Slider(DefaultMP));

            if (Category.Any(t => t == MenuType.SelfCount))
                Menu.AddItem(new MenuItem("SelfCount" + Name, "Use On Enemy Count >="))
                    .SetValue(new Slider(3, 1, 5));

            if (Category.Any(t => t == MenuType.SelfMinMP))
                Menu.AddItem(new MenuItem("SelfMinMP" + Name + "Pct", "Minimum Mana %")).SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.SelfMinHP))
                Menu.AddItem(new MenuItem("SelfMinHP" + Name + "Pct", "Minimum HP %")).SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.Zhonyas))
            {
                Menu.AddItem(new MenuItem("use" + Name + "Norm", "Use on Dangerous (Spells)")).SetValue(false);
                Menu.AddItem(new MenuItem("use" + Name + "Ulti", "Use on Dangerous (Ultimates Only)")).SetValue(true);
            }

            if (Category.Any(t => t == MenuType.Cleanse))
            {
                var ccmenu = new Menu(Name + " Debuffs", Name.ToLower() + "cdeb");
                ccmenu.AddItem(new MenuItem(Name + "cignote", "Ignite")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cexhaust", "Exhaust")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cstun", "Stuns")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "ccharm", "Charms")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "ctaunt", "Taunts")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cfear", "Fears")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cflee", "Flee")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "csnare", "Snares")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "csilence", "Silences")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "csupp", "Supression")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cpolymorph", "Polymorphs")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cblind", "Blinds")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cslow", "Slows")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cpoison", "Poisons")).SetValue(true);
                Menu.AddSubMenu(ccmenu);

                Menu.AddItem(new MenuItem("use" + Name + "Number", "Minimum Spells to Use")).SetValue(new Slider(DefaultHP/5, 1, 5));
                Menu.AddItem(new MenuItem("use" + Name + "Time", "Minumum Durration to Use")).SetValue(new Slider(2, 1, 5)); ;
                Menu.AddItem(new MenuItem("use" + Name + "Od", "Only use on Dangerous")).SetValue(false);
            }



            if (Category.Any(t => t == MenuType.ActiveCheck))
                Menu.AddItem(new MenuItem("mode" + Name, "Mode: "))
                    .SetValue(new StringList(new[] { "Always", "Combo" }, Id == 3222 ? 0 : 1));



            root.AddSubMenu(Menu);
            return this;
        }

        public virtual void OnTick(EventArgs args)
        {

        }
    }
}
