namespace Core.Engine.IoC
{
    using Autofac;

    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;
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
    using Core.Engine.UserInterface;

    public class EngineModule : CarbonQuickModule
    {
        public EngineModule()
        {
            // Core
            this.For<ICarbonGraphics>().Use<CarbonGraphics>().Singleton();

            // Logic
            this.For<IInputManager>().Use<InputManager>().Singleton();
            this.For<IFirstPersonController>().Use<FirstPersonController>();
            this.For<ITypingController>().Use<TypingController>();
            this.For<IScriptingEngine>().Use<ScriptingEngine>();
            this.For<ISceneManager>().Use<SceneManager>();

            // Rendering
            this.For<IProjectionCamera>().Use<ProjectionCamera>();
            this.For<IOrthographicCamera>().Use<OrthographicCamera>();
            this.For<IFrameManager>().Use<FrameManager>().Singleton();
            this.For<IRenderer>().Use<Renderer>().Singleton();
            this.For<IForwardShader>().Use<ForwardShader>();
            this.For<IGBufferShader>().Use<GBufferShader>();
            this.For<IDeferredLightShader>().Use<DeferredLightShader>();
            this.For<IDebugShader>().Use<DebugShader>();
            this.For<IBlendShader>().Use<BlendShader>();
            this.For<IShadowMapShader>().Use<ShadowMapShader>();
            this.For<IPlainShader>().Use<PlainShader>();

            // Resource
            this.For<IContentManager>().Use<ContentManager>();
            this.For<IResourceManager>().Use<ResourceManager>();

            // Scene
            this.For<ISceneEntityFactory>().Use<SceneEntityFactory>().Singleton();
            this.For<ISceneDebugOverlay>().Use<SceneDebugOverlay>().Singleton();

            // User Interface
            this.For<IUserInterfaceConsole>().Use<UserInterfaceConsole>();
        }
    }
}
