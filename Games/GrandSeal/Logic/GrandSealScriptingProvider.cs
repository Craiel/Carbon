using System;

using Core.Engine.Logic.Scripting;
using GrandSeal.Contracts;

namespace GrandSeal.Logic
{
    public class GrandSealScriptingProvider : ScriptingGameProvider, IGrandSealScriptingProvider
    {
        private readonly IGrandSeal game;
        private readonly IGrandSealGameState gameState;

        public GrandSealScriptingProvider(IGrandSeal game, IGrandSealGameState gameState)
            : base(game)
        {
            this.game = game;
            this.gameState = gameState;
        }

        [ScriptingMethod]
        public void SwitchScene(string scene)
        {
            SceneKeys key;
            if (Enum.TryParse(scene, out key))
            {
                this.game.SwitchScene(key);
            }
        }

        [ScriptingMethod]
        public void ToggleFrameRateLimit()
        {
            this.game.LimitFrameRate = !this.game.LimitFrameRate;
        }

        [ScriptingMethod]
        public void SetFrameRate(int value)
        {
            this.game.TargetFrameRate = value;
        }

        [ScriptingMethod]
        public void SetGameSpeed(float value)
        {
            this.game.GameSpeed = value;
        }

        [ScriptingMethod]
        public void Reload()
        {
            this.game.Reload();
        }
    }
}
