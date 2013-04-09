using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Contracts.UserInterface;
using Carbon.Engine.Logic;
using Carbon.Engine.Logic.Scripting;
using Carbon.Engine.Rendering;
using Carbon.Engine.Rendering.Camera;
using Carbon.Engine.Rendering.Shaders;
using Carbon.Engine.Resource;
using Carbon.Engine.Scene;

using Ninject.Modules;

namespace Carbon.Engine.Ninject
{
    using Carbon.Engine.UserInterface;

    public class EngineModule : NinjectModule
    {
        public override void Load()
        {
            // Core
            this.Bind<IEngineFactory>().To<EngineFactory>().InSingletonScope();
            this.Bind<ICarbonGraphics>().To<CarbonGraphics>().InSingletonScope();
            this.Bind<IEngineLog>().To<EngineLog>().InSingletonScope();

            // Logic
            this.Bind<IInputManager>().To<InputManager>().InSingletonScope();
            this.Bind<IFirstPersonController>().To<FirstPersonController>();
            this.Bind<IDebugController>().To<DebugController>();
            this.Bind<ITypingController>().To<TypingController>();
            this.Bind<IScriptingEngine>().To<ScriptingEngine>();
            this.Bind<ISceneManager>().To<SceneManager>();

            // Rendering
            this.Bind<IProjectionCamera>().To<ProjectionCamera>();
            this.Bind<IOrthographicCamera>().To<OrthographicCamera>();
            this.Bind<IFrameManager>().To<FrameManager>().InSingletonScope();
            this.Bind<IRenderer>().To<Renderer>().InSingletonScope();
            this.Bind<IDefaultShader>().To<DefaultShader>();
            this.Bind<IGBufferShader>().To<GBufferShader>();
            this.Bind<IDeferredLightShader>().To<DeferredLightShader>();
            this.Bind<IDebugShader>().To<DebugShader>();
            this.Bind<IBlendShader>().To<BlendShader>();

            // Resource
            this.Bind<IContentManager>().To<ContentManager>();
            this.Bind<IResourceManager>().To<ResourceManager>();

            // User Interface
            this.Bind<IUserInterfaceConsole>().To<UserInterfaceConsole>();
        }
    }
}
