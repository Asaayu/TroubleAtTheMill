using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Enums.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using CoreAPI = Hearthstone_Deck_Tracker.API.Core;

namespace TroubleAtTheMill
{
    internal class FatigueCalculator
    {
        private static Player Player;
        private static Player Opponent;
        private static Entity PlayerHero;
        private static Entity OpponentHero;

        internal static void OnGameStart()
        {
            Player = CoreAPI.Game.Player;
            Opponent = CoreAPI.Game.Opponent;
            PlayerHero = Player.Board.FirstOrDefault(x => x.IsHero);
            OpponentHero = Opponent.Board.FirstOrDefault(x => x.IsHero);

            // Add the two labels to the overlay canvas
            CoreAPI.OverlayCanvas.Children.Add(FatiguePlugin.playerDrawCount);
            CoreAPI.OverlayCanvas.Children.Add(FatiguePlugin.opponentDrawCount);
        }

        internal static void OnGameEnd()
        {
            CoreAPI.OverlayCanvas.Children.Remove(FatiguePlugin.playerDrawCount);
            CoreAPI.OverlayCanvas.Children.Remove(FatiguePlugin.opponentDrawCount);
        }

        internal static void OnGameUpdate()
        {
            // Check if the game is not valid
            if (CoreAPI.Game.CurrentMode != Mode.GAMEPLAY || CoreAPI.Game.IsBattlegroundsMatch)
            {
                OnGameEnd();
                return;
            };

            // Calculate the number of draws remaining for each player
            int playerDrawsRemaining = FatigueCalculator.CalculateTotalDraws(true);
            int opponentDrawsRemaining = FatigueCalculator.CalculateTotalDraws(false);

            // If the labels aren't on the canvas, call the OnGameStart method to set them up
            if (!CoreAPI.OverlayCanvas.Children.Contains(FatiguePlugin.playerDrawCount)) OnGameStart();

            // Update the number of draws remaining on the labels
            FatiguePlugin.playerDrawCount.Content = $"{playerDrawsRemaining} 💀";
            FatiguePlugin.opponentDrawCount.Content = $"{opponentDrawsRemaining} 💀";

            // Update the color of the labels
            FatiguePlugin.playerDrawCount.Foreground = playerDrawsRemaining <= 5 ? Brushes.Red : Brushes.White;
            FatiguePlugin.opponentDrawCount.Foreground = opponentDrawsRemaining <= 5 ? Brushes.Red : Brushes.White;

            // Set position of the labels
            Canvas.SetRight(FatiguePlugin.playerDrawCount, CoreAPI.OverlayCanvas.Width * 0.145);
            Canvas.SetRight(FatiguePlugin.opponentDrawCount, CoreAPI.OverlayCanvas.Width * 0.145);

            Canvas.SetTop(FatiguePlugin.playerDrawCount, CoreAPI.OverlayCanvas.Height * 0.5);
            Canvas.SetTop(FatiguePlugin.opponentDrawCount, CoreAPI.OverlayCanvas.Height * 0.38);
        }

        internal static int CalculateTotalDraws(bool local)
        {
            if (PlayerHero == null || OpponentHero == null) OnGameStart();

            var hero = local ? PlayerHero : OpponentHero;
            if (hero == null) return 0;

            int playerHealth = hero.Health + hero.GetTag(GameTag.ARMOR);
            if (playerHealth <= 0) return 0;

            int playerCardsRemaining = Player.DeckCount;
            int playerFatigue = Player.Fatigue + 1;

            return DrawsUntilFatigue(playerCardsRemaining, playerFatigue, playerHealth);
        }

        private static int DrawsUntilFatigue(int cardsRemaining, int fatigue, int health)
        {
            double sqrtValue = Math.Sqrt((4 * fatigue * fatigue) - 4 * fatigue + 1 + 8 * health);
            int draws = (int)Math.Ceiling((sqrtValue - 2 * fatigue + 1) / 2.0);
            return cardsRemaining + draws;
        }
    }
}
