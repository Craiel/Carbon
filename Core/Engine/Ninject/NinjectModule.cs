using Core.Engine.Contracts;
using Core.Engine.Contracts.Logic;
using Core.Engine.Contracts.Rendering;
using Core.Engine.Contracts.Resource;
using Core.Engine.Contracts.Scene;
using Core.Engine.Contracts.UserInterface;
using Core.Engine.Logic;
using Core.Engine.Logic.Scripting;
using Core.Engine.Rendering;
using Core.Engine.Rendering.Camera;
using Core.Engine.Rendering.Shaders;
using Core.Engine.Resource;
using Core.Engine.Scene;

using Ninject.Modules;

namespace Core.Engine.Ninject
{
    using Core.Engine.UserInterface;

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
            //this.Bind<ISceneEntityFactory>().To<SceneEntityFactory>();
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
            this.Bind<IShadowMapShader>().To<ShadowMapShader>();
            this.Bind<IPlainShader>().To<PlainShader>();

            // Resource
            this.Bind<IContentManager>().To<ContentManager>();
            this.Bind<IResourceManager>().To<ResourceManager>();

            // User Interface
            this.Bind<IUserInterfaceConsole>().To<UserInterfaceConsole>();
        }
    }
}
