﻿#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/champion.cs
// Date:		06/06/2015
// Author:		Robin Kurisu
#endregion

using LeagueSharp;
using System.Collections.Generic;

namespace Activator
{
    public class champion
    {
        public float IncomeDamage;
        public float MinionDamage;
        public Obj_AI_Hero Player;
        public Obj_AI_Base Attacker;

        public bool ForceQSS;
        public bool Immunity;
        public int QSSBuffCount;
        public int QSSHighestBuffTime;

        public List<HitType> HitTypes = new List<HitType>();

        public static List<champion> Heroes = new List<champion>(); 

        public champion(Obj_AI_Hero player, float incdmg)
        {
            Player = player;
            IncomeDamage = incdmg;
        }
    }
}
