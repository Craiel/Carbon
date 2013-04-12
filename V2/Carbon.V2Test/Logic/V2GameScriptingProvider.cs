using System;

using Carbon.Engine.Logic.Scripting;
using Carbon.V2Test.Contracts;

namespace Carbon.V2Test.Logic
{
    public class V2GameScriptingProvider : ScriptingGameProvider
    {
        private readonly IV2Test game;

        public V2GameScriptingProvider(IV2Test game)
            : base(game)
        {
            this.game = game;
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
