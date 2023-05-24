using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums.Hearthstone;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Logging;
using CoreAPI = Hearthstone_Deck_Tracker.API.Core;

namespace TroubleAtTheMill
{
    public class FatiguePlugin : IPlugin
    {
        // Font size - % of overlay height
        internal static double infoFontSize = 0.019;

        public string Name => "Trouble at the Mill";
        public string Description => "Displays the number of card draws until each player dies from fatigue.";
        public string ButtonText => "No Settings";
        public string Author => "realchriscasey (v1), Asaayu (v2)";
        public Version Version => new Version(2, 0, 0);
        public MenuItem MenuItem => null;
        public void OnButtonPress() { }

        internal static Label playerDrawCount;
        internal static Label opponentDrawCount;       

        public void OnLoad()
        {
            playerDrawCount = CreateDrawLabel();
            opponentDrawCount = CreateDrawLabel();

            GameEvents.OnGameStart.Add(FatigueCalculator.OnGameStart);
            GameEvents.OnGameEnd.Add(FatigueCalculator.OnGameEnd);
        }

        public void OnUnload()
        {
            FatigueCalculator.OnGameEnd();
        }

        public void OnUpdate()
        {
            FatigueCalculator.OnGameUpdate();
        }

        private static Label CreateDrawLabel()
        {
            return new Label
            {
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 0,
                    Opacity = 1,
                    BlurRadius = 5,
                    RenderingBias = RenderingBias.Quality,
                },
                Padding = new Thickness(5) // Add padding to create space between the text and shadow
            };
        }
    }
}