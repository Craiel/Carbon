namespace GrandSeal.Logic
{
    using System;

    using Contracts;

    using Core.Engine.Logic.Scripting;

    public class GrandSealScriptingProvider : IGrandSealScriptingProvider
    {
        [ScriptingMethod]
        public void SwitchScene(string scene)
        {
            /*SceneKeys key;
            if (Enum.TryParse(scene, out key))
            {
                this.game.SwitchScene(key);
            }*/
        }

        [ScriptingMethod]
        public void ToggleFrameRateLimit()
        {
            throw new NotImplementedException();
            //this.game.LimitFrameRate = !this.game.LimitFrameRate;
        }

        [ScriptingMethod]
        public void SetFrameRate(int value)
        {
            throw new NotImplementedException();
            //this.game.TargetFrameRate = value;
        }

        [ScriptingMethod]
        public void SetGameSpeed(float value)
        {
            throw new NotImplementedException();
            //this.game.GameSpeed = value;
        }

        [ScriptingMethod]
        public void Reload()
        {
            throw new NotImplementedException();
            //this.game.Reload();
        }
    }
}
