﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PurpleStyrofoam.Rendering;

namespace PurpleStyrofoam.Items.Weapons.Melee.Rapiers
{
    class Lithiel : Rapier
    {
        public override int ID => 004;

        public override string Description => "Although it is considered a failed imitation of the legendary sword, Flight, its power is not to be pitied. Wielders of this powerful rapier have been known to finish fights faster than they started.";

        public Lithiel() : base("Lithiel", 100, RARITY.EPIC, new ItemSprite(PlayerManager.jumpingDPlayerSprite, 50 ,50)) {  }



        public override void OnQAbility()
        {
            // Attacks have no cooldown speed. I.E insanely fast speed. Also ignores defense.
            throw new NotImplementedException();
        }
    }
}
