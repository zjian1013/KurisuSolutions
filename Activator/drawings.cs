using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using Color = System.Drawing.Color;

namespace Activator
{
    class drawings
    {
        public drawings()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (Activator.Origin.Item("qssdebug").GetValue<bool>())
            {
                foreach (var hero in champion.Heroes)
                {
                    var mpos = Drawing.WorldToScreen(hero.Player.Position);

                    if (!hero.Player.IsDead)
                    {
                        Drawing.DrawText(mpos[0] - 30, mpos[1] + 0, Color.White, "Income Damage: " + hero.IncomeDamage);
                        Drawing.DrawText(mpos[0] - 30, mpos[1] + 15, Color.White, "QSSBuffCount: " + hero.QSSBuffCount);
                        Drawing.DrawText(mpos[0] - 30, mpos[1] + 30, Color.White,
                            "QSSHighestBuffTime: " + hero.QSSHighestBuffTime);
                    }
                }
            }
        }
    }
}
