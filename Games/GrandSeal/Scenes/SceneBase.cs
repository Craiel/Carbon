namespace GrandSeal.Scenes
{
    using System.Data;
    using System.Threading;

    using CarbonCore.Processing.Resource;
    using CarbonCore.Utils.Contracts.IoC;

    using Contracts;

    using Core.Engine.Logic.Scripting;
    using Core.Engine.Scene;

    using NLua;

    public abstract class SceneBase : Scene
    {
        // --------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected SceneBase(IFactory factory)
        {
            this.GameState = factory.Resolve<IGrandSealGameState>();
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected IGrandSealGameState GameState { get; private set; }

        protected override bool HasRuntime
        {
            get
            {
                return true;
            }
        }

        protected override void Activate()
        {
            this.GameState.ScriptingEngine.Register(this);

            base.Activate();
        }

        protected override void Deactivate()
        {
            base.Deactivate();

            this.GameState.ScriptingEngine.Unregister(this);
        }

        protected override CarbonScript LoadRuntimeScript(string scriptHash)
        {
            var resource = this.GameState.ResourceManager.Load<ScriptResource>(scriptHash);
            if (resource == null)
            {
                throw new DataException("Runtime was not found: " + scriptHash);
            }

            return new CarbonScript(resource);
        }

        protected override Lua LoadRuntime(CarbonScript script)
        {
            Lua context = this.GameState.ScriptingEngine.GetContext();
            context.DoString(script.Script);
            while (context.IsExecuting)
            {
                // Give the script some time to register
                Thread.Sleep(10);
            }

            return context;
        }
    }
}
