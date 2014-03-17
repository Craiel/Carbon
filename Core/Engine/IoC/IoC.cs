namespace Core.Engine.IoC
{
    using Autofac;

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

    public class EngineModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Core
            builder.RegisterType<EngineFactory>().As<IEngineFactory>().SingleInstance();
            builder.RegisterType<CarbonGraphics>().As<ICarbonGraphics>().SingleInstance();
            builder.RegisterType<EngineLog>().As<IEngineLog>().SingleInstance();

            // Logic
            builder.RegisterType<InputManager>().As<IInputManager>().SingleInstance();
            builder.RegisterType<FirstPersonController>().As<IFirstPersonController>();
            builder.RegisterType<TypingController>().As<ITypingController>();
            builder.RegisterType<ScriptingEngine>().As<IScriptingEngine>();
            builder.RegisterType<SceneManager>().As<ISceneManager>();

            // Rendering
            builder.RegisterType<ProjectionCamera>().As<IProjectionCamera>();
            builder.RegisterType<OrthographicCamera>().As<IOrthographicCamera>();
            builder.RegisterType<FrameManager>().As<IFrameManager>().SingleInstance();
            builder.RegisterType<Renderer>().As<IRenderer>().SingleInstance();
            builder.RegisterType<ForwardShader>().As<IForwardShader>();
            builder.RegisterType<GBufferShader>().As<IGBufferShader>();
            builder.RegisterType<DeferredLightShader>().As<IDeferredLightShader>();
            builder.RegisterType<DebugShader>().As<IDebugShader>();
            builder.RegisterType<BlendShader>().As<IBlendShader>();
            builder.RegisterType<ShadowMapShader>().As<IShadowMapShader>();
            builder.RegisterType<PlainShader>().As<IPlainShader>();

            // Resource
            builder.RegisterType<ContentManager>().As<IContentManager>();
            builder.RegisterType<ResourceManager>().As<IResourceManager>();

            // Scene
            builder.RegisterType<SceneEntityFactory>().As<ISceneEntityFactory>().SingleInstance();
            builder.RegisterType<SceneDebugOverlay>().As<ISceneDebugOverlay>().SingleInstance();

            // User Interface
            builder.RegisterType<UserInterfaceConsole>().As<IUserInterfaceConsole>();
        }
    }
}
