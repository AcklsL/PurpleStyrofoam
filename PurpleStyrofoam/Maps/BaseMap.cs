﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PurpleStyrofoam.Rendering;
using PurpleStyrofoam.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleStyrofoam.Maps
{
    public abstract class BaseMap
    {
        public abstract MapObject[] BackgroundLayer { get; set; }
        public abstract MapInteractable[] InteractableLayer { get; set; }
        public abstract MapObject[] ActiveLayer { get; set; }
        public abstract MapObject[] ForegroundLayer { get; set; }
        public abstract List<AnimatedSprite> sprites { get; set; }
        public Rectangle maxBounds;

        public BaseMap()
        {
            maxBounds = new Rectangle(0,0,1000,1000);
        }

        public void DrawBackground(SpriteBatch sp)
        {
            foreach (MapObject item in BackgroundLayer)
            {
                item.Draw(sp);
            }
        }
        public void DrawInteractables(SpriteBatch sp)
        {
            foreach (MapObject item in InteractableLayer)
            {
                item.Draw(sp);
            }
        }
        public void Draw(SpriteBatch sp)
        {
            foreach (MapObject item in ActiveLayer)
            {
                item.Draw(sp);
            }
        }
        public void DrawForeground(SpriteBatch sp)
        {
            foreach (MapObject item in ForegroundLayer)
            {
                item.Draw(sp);
            }
        }

        public void DrawMap(SpriteBatch sp, Action<SpriteBatch> insert) 
        {
            DrawBackground(sp);
            DrawInteractables(sp);
            Draw(sp);
            insert(sp);
            DrawForeground(sp);
        }
    }
}
