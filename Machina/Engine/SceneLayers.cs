using System;
using System.Collections.Generic;
using Machina.Data;
using Machina.Engine.Cartridges;
using Machina.Engine.Debugging.Components;
using Machina.Engine.Debugging.Data;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Engine
{
    public class SceneLayers
    {
        public delegate void DrawAction(SpriteBatch spriteBatch);

        private readonly DebugDock debugDock;
        public readonly Scene debugScene;
        public readonly IGameCanvas gameCanvas;
        public readonly MachinaRuntime runtime;
        private readonly Logger overlayOutputConsole;
        private readonly List<Scene> sceneList = new List<Scene>();

        public Color BackgroundColor = Color.SlateBlue;
        private bool hasDoneFirstDraw;
        private bool hasDoneFirstUpdate;

#pragma warning disable CS0067 // only used in release
        public event Action<Exception> OnError;
#pragma warning restore CS0067

        private TextInputEventArgs? pendingTextInput;

        public SceneLayers(bool useDebugScene, IGameCanvas gameCanvas, MachinaRuntime runtime = null)
        {
            this.gameCanvas = gameCanvas;
            this.runtime = runtime;

            Logger = new StdOutConsoleLogger();

            if (useDebugScene)
            {
                this.debugScene = new Scene(this);
                this.debugScene.SetGameCanvas(new GameCanvas(gameCanvas.WindowSize, ResizeBehavior.FillContent));
                this.overlayOutputConsole = DebugBuilder.BuildOutputConsole(this);
                Logger = this.overlayOutputConsole;

                // DebugBuilder.CreateFramerateCounter(this);
                DebugBuilder.CreateFramestep(this);
                this.debugDock = DebugBuilder.CreateDebugDock(this);
            }
        }

        public ILogger Logger { get; private set; }

        public SamplerState SamplerState { get; set; } = SamplerState.PointWrap;

        public InputFrameState CurrentInputFrameState { get; private set; }

        public Texture2D RenderToTexture(SpriteBatch spriteBatch)
        {
            var graphicsDevice = this.runtime.GraphicsDevice;
            var viewportSize = this.gameCanvas.ViewportSize;
            var renderTarget = new RenderTarget2D(
                graphicsDevice,
                viewportSize.X,
                viewportSize.Y,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };

            graphicsDevice.Clear(this.BackgroundColor);
            DrawOnCanvas(spriteBatch);

            graphicsDevice.SetRenderTarget(null);
            return renderTarget;
        }

        public void AddPendingTextInput(object sender, TextInputEventArgs e)
        {
            this.pendingTextInput = e;
        }

        public void AddDebugApp(App app)
        {
            this.debugDock.AddApp(app);
        }

        public void PushLogger(ILogger newLogger)
        {
            Logger = newLogger;
        }

        /// <summary>
        ///     One day this might behave like a stack but for now it just reverts to the Overlay Console
        /// </summary>
        public void PopLogger()
        {
            Logger = this.overlayOutputConsole;
        }

        public Scene AddNewScene()
        {
            var scene = new Scene(this, this.runtime.GlobalFrameStep);
            Add(scene);
            return scene;
        }

        public void RemoveScene(Scene scene)
        {
            this.sceneList.Remove(scene);
        }

        /// <summary>
        ///     This is private intentionally, you should only add a scene to the SceneLayers as a new scene via AddNewScene
        /// </summary>
        /// <param name="scene"></param>
        private void Add(Scene scene)
        {
            this.sceneList.Add(scene);
        }

        public int IndexOf(Scene scene)
        {
            return this.sceneList.IndexOf(scene);
        }

        public void Set(int i, Scene scene)
        {
            this.sceneList[i] = scene;
        }

        public Scene[] AllScenes()
        {
            var array = new Scene[this.sceneList.Count + (this.debugScene != null ? 1 : 0)];
            this.sceneList.CopyTo(array);
            if (this.debugScene != null)
            {
                array[this.sceneList.Count] = this.debugScene;
            }

            return array;
        }

        public Scene[] AllScenesExceptDebug()
        {
            var array = new Scene[this.sceneList.Count];
            this.sceneList.CopyTo(array);
            return array;
        }

        public void UpdateWithNoInput(float dt)
        {
            Update(dt, Matrix.Identity, InputFrameState.Empty);
        }

        public void Update(float dt, InputFrameState input)
        {
            Update(dt, Matrix.Identity, input);
        }

        public void Update(float dt, Matrix mouseTransformMatrix, InputFrameState inputFrameState,
            bool allowMouseUpdate = true, bool allowKeyboardEvents = true)
        {
#if DEBUG
#else
            try
            {
#endif
            if (!this.hasDoneFirstUpdate)
            {
                DoFirstUpdate();
                this.hasDoneFirstUpdate = true;
            }

            CurrentInputFrameState = inputFrameState;
            var scenes = AllScenes();

            var rawMousePos = Vector2.Transform(inputFrameState.mouseFrameState.RawWindowPosition.ToVector2(),
                mouseTransformMatrix);

            foreach (var scene in scenes)
            {
                scene.FlushBuffers();

                if (!scene.IsFrozen)
                {
                    if (allowKeyboardEvents)
                    {
                        if (this.pendingTextInput.HasValue)
                        {
                            scene.OnTextInput(this.pendingTextInput.Value);
                        }

                        foreach (var key in inputFrameState.keyboardFrameState.Released)
                        {
                            scene.OnKey(key, ButtonState.Released, inputFrameState.keyboardFrameState.Modifiers);
                        }

                        foreach (var key in inputFrameState.keyboardFrameState.Pressed)
                        {
                            scene.OnKey(key, ButtonState.Pressed, inputFrameState.keyboardFrameState.Modifiers);
                        }
                    }

                    if (allowMouseUpdate)
                    {
                        if (inputFrameState.mouseFrameState.ScrollDelta != 0)
                        {
                            scene.OnScroll(inputFrameState.mouseFrameState.ScrollDelta);
                        }

                        // Pressed
                        if (inputFrameState.mouseFrameState.ButtonsPressedThisFrame.left)
                        {
                            scene.OnMouseButton(MouseButton.Left, rawMousePos, ButtonState.Pressed);
                        }

                        if (inputFrameState.mouseFrameState.ButtonsPressedThisFrame.middle)
                        {
                            scene.OnMouseButton(MouseButton.Middle, rawMousePos, ButtonState.Pressed);
                        }

                        if (inputFrameState.mouseFrameState.ButtonsPressedThisFrame.right)
                        {
                            scene.OnMouseButton(MouseButton.Right, rawMousePos, ButtonState.Pressed);
                        }

                        // Released
                        if (inputFrameState.mouseFrameState.ButtonsReleasedThisFrame.left)
                        {
                            scene.OnMouseButton(MouseButton.Left, rawMousePos, ButtonState.Released);
                        }

                        if (inputFrameState.mouseFrameState.ButtonsReleasedThisFrame.middle)
                        {
                            scene.OnMouseButton(MouseButton.Middle, rawMousePos, ButtonState.Released);
                        }

                        if (inputFrameState.mouseFrameState.ButtonsReleasedThisFrame.right)
                        {
                            scene.OnMouseButton(MouseButton.Right, rawMousePos, ButtonState.Released);
                        }

                        // At this point the raw and processed deltas are equal, downstream (Scene and below) they will differ
                        scene.OnMouseUpdate(rawMousePos, inputFrameState.mouseFrameState.PositionDelta,
                            inputFrameState.mouseFrameState.PositionDelta);
                    }
                }
            }

            this.pendingTextInput = null;

            foreach (var scene in scenes)
            {
                if (!scene.frameStep.IsPaused && !scene.IsFrozen)
                {
                    scene.Update(dt);
                }
            }

            HitTestResult.ApproveTopCandidate(scenes);
#if DEBUG
#else
            }
            catch (System.Exception exception)
            {
                MachinaClient.Print("caught exception");
                OnError?.Invoke(exception);
            }
#endif
        }

        public void PreDraw(SpriteBatch spriteBatch)
        {
            if (!this.hasDoneFirstDraw)
            {
                this.hasDoneFirstDraw = true;
                OnFirstPreDraw?.Invoke(spriteBatch);
            }

            var scenes = AllScenes();
            foreach (var scene in scenes)
            {
                scene.FlushBuffers();
                scene.PreDraw(spriteBatch);
            }
        }

        public event DrawAction OnFirstPreDraw;

        private void DoFirstUpdate()
        {
            if (!this.hasDoneFirstUpdate)
            {
                this.hasDoneFirstUpdate = true;
            }
        }

        public void DrawOnCanvas(SpriteBatch spriteBatch)
        {
            var scenes = AllScenesExceptDebug();

            foreach (var scene in scenes)
            {
                scene.Draw(spriteBatch);
            }

            if (this.runtime.DebugLevel > DebugLevel.Passive)
            {
                foreach (var scene in scenes)
                {
                    scene.DebugDraw(spriteBatch);
                }
            }
        }

        public void DrawDebugScene(SpriteBatch spriteBatch)
        {
            this.debugScene?.Draw(spriteBatch);
        }

        public void CloseDebugDock()
        {
            this.debugDock.Close();
        }
    }
}