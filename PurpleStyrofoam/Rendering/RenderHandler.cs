﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleStyrofoam.AiController;
using PurpleStyrofoam.Maps;
using PurpleStyrofoam.Rendering.Menus;

namespace PurpleStyrofoam.Rendering
{
    static class RenderHandler
    {
        public static List<AnimatedSprite> allCharacterSprites { get; private set; }
        public static List<ItemSprite> allItemSprites { get; private set; }
        public static List<Projectile> allProjectiles { get; private set; }
        public static List<Projectile> purgeProjectiles { get; private set; }
        public static BaseMap selectedMap;
        public static GAMESTATE CurrentGameState { get; set; }
        public static void Initialize()
        {
            allCharacterSprites = new List<AnimatedSprite>();
            allItemSprites = new List<ItemSprite>();
            allProjectiles = new List<Projectile>();
            purgeProjectiles = new List<Projectile>();
            ScreenMovement = new Vector2(0, 0);
            CurrentGameState = GAMESTATE.MAINMENU;
        }
        public static void InitiateChange(BaseMap newMap, PlayerController player, int newX = 0, int newY = 0)
        {
            selectedMap = newMap;
            ObjectMapper.MapObjects(selectedMap);
            allCharacterSprites.Add(player);
            //allCharacterSprites = selectedMap.sprites;
            allItemSprites = new List<ItemSprite>();
            if (player.HeldWeapon != null) allItemSprites.Add(player.HeldWeapon.sprite);
            player.X = newX;
            player.Y = newY;
        }
        public static void InitiateChange(BaseMap newMap, PlayerController player, List<AnimatedSprite> newSprites, List<ItemSprite> newItems, int newX = 0, int newY = 0)
        {
            selectedMap = newMap;
            allCharacterSprites = newSprites;
            allItemSprites = newItems;
            player.X = newX;
            player.Y = newY;
        }
        public static void Update()
        {
            switch (CurrentGameState)
            {
                case GAMESTATE.ACTIVE:
                    foreach (AnimatedSprite sprite in RenderHandler.allCharacterSprites)
                    {
                        sprite.Update();
                    }
                    foreach (Projectile item in allProjectiles)
                    {
                        item.Update();
                    }
                    if (purgeProjectiles.Count != 0) DeleteProjectiles();
                    break;
                case GAMESTATE.MAINMENU:
                    break;
                default:
                    throw new NotSupportedException("Game has entered an invalid gamestate: " + CurrentGameState);
            }
        }

        public static void DeleteProjectiles()
        {
            foreach (Projectile proj in purgeProjectiles)
            {
                allProjectiles.Remove(proj);
            }
            purgeProjectiles.Clear();
        }

        public static Vector2 ScreenMovement;
        public static Vector2 ScreenOffset = new Vector2(0, 0);
        public static void Draw(SpriteBatch sp)
        {
            sp.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(ScreenMovement.X, ScreenMovement.Y, 0));
            switch (CurrentGameState)
            {
                case GAMESTATE.ACTIVE:
                    if (selectedMap != null) selectedMap.DrawBackground(sp);
                    if (selectedMap != null) selectedMap.Draw(sp);
                    foreach (AnimatedSprite item in allCharacterSprites)
                    {
                        item.Draw(sp);
                    }
                    foreach (Projectile item in allProjectiles)
                    {
                        item.Draw(sp);
                    }
                    foreach (ItemSprite item in allItemSprites)
                    {
                        item.Draw(sp);
                    }
                    if (selectedMap != null) selectedMap.DrawForeground(sp);
                    if (MenuHandler.ActivePopUps != null) MenuHandler.DrawPopUpMenu(sp);
                    break;
                case GAMESTATE.MAINMENU:
                    if (MenuHandler.ActiveFullScreenMenu != null) MenuHandler.DrawFullScreenMenu(sp);
                    break;
                default:
                    throw new NotSupportedException("Game has entered an invalid gamestate: " + CurrentGameState);
            }
            sp.End();
        }
        public static void Add(ItemSprite input)
        {
            allItemSprites.Add(input);
        }
        public static void Add(AnimatedSprite input)
        {
            allCharacterSprites.Add(input);
        }
        public static float LookAtXY(Vector2 source, int xIn, int yIn)
        {
            double deltaX = xIn - source.X;
            double deltaY = yIn - source.Y;
            return (float) Math.Atan2(deltaY, deltaX);
        }
        public static float LookAtMouse(Vector2 source)
        {
            double deltaX = Mouse.GetState().X - source.X;
            double deltaY = Mouse.GetState().Y - source.Y;
            return (float)Math.Atan2(deltaY, deltaX);
        }
        public static float LookAtSprite(ItemSprite objectIn, AnimatedSprite objectToSee)
        {
            double deltaX = objectToSee.X - objectIn.X;
            double deltaY = objectToSee.Y - objectIn.Y;
            return (float)Math.Atan2(deltaY, deltaX);
        }
        public static float LookAtSprite(ItemSprite objectIn, string characterSpriteName)
        {
            double deltaX = allCharacterSprites.Find(x => x.GetType().Name == characterSpriteName).X - objectIn.X;
            double deltaY = allCharacterSprites.Find(x => x.GetType().Name == characterSpriteName).Y - objectIn.Y;
            return (float)Math.Atan2(deltaY, deltaX);
        }
    }
    enum GAMESTATE
    {
        MAINMENU, ACTIVE
    }
}
