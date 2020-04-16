﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PurpleStyrofoam.Rendering.Animations;
using PurpleStyrofoam.Helpers;
using System.Diagnostics;

namespace PurpleStyrofoam.Rendering
{
    public class ItemSprite : GameObject
    {
        public Rectangle ItemRectangle;
        public Animation animate;
        float RotationRate;

        public Action OnStart;
        public Action DuringSwing;
        public Action OnEnd;
        public bool Flipped { 
            get
            {
                return animate.Flipped;
            }
            set
            {
                animate.Flipped = value;
            }
        }
        public bool Visible { get; set; }
        public ItemSprite(string TextureName, int width = 50, int height = 50, int rows = 1, int columns = 1)
        {
            ItemRectangle = new Rectangle(0, 0, width, height);
            animate = new Animation(TextureName, rows, columns);
            Flipped = false;
            RotationRate = 0.0f;
            Visible = false;

            OnStart = DuringSwing = OnEnd = () => { };
        }

        public override void Draw(SpriteBatch sp)
        {
            if (Visible) animate.Draw(sp, ItemRectangle);
        }

        bool StopSwing = false;
        public override void Update()
        {
            if (Visible && !StopSwing)
            {
                animate.Rotate(RotationRate);
                DuringSwing();
                AngleCheck();
            }
            else
            {
                OnEnd();
                Visible = false;
            }
        }

        public override void Load()
        {
            animate.Load();
        }

        Action AngleCheck = () => { };
        public void StartAnimation(float rotateRate)
        {
            if (Visible) return;
            if (animate.Flipped)
            {
                animate.Origin = new Vector2(animate.Texture.Width, animate.Texture.Height);
                RotationRate = -rotateRate;
                animate.Angle = GameMathHelper.PIConstants.PI_45;
                AngleCheck = () => { StopSwing = animate.Angle < -GameMathHelper.PIConstants.PI_45; };
            } else
            {
                animate.Origin = new Vector2(0, animate.Texture.Height);
                RotationRate = rotateRate;
                animate.Angle = -GameMathHelper.PIConstants.PI_45;
                AngleCheck = () => { StopSwing = animate.Angle > GameMathHelper.PIConstants.PI_45; };
            }
            StopSwing = false;
            Visible = true;
            OnStart();
        }
    }
}
