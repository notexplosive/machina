using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machina.Engine
{
    public class SceneLayers
    {
        public readonly Scene debugScene;
        public readonly IFrameStep frameStep;
        public readonly IGameCanvas gameCanvas;
        private readonly List<Scene> sceneList = new List<Scene>();
        public SamplerState SamplerState { get; set; } = SamplerState.PointWrap;

        public InputFrameState CurrentInputFrameState
        {
            get;
            private set;
        }

        private TextInputEventArgs? pendingTextInput;

        public Texture2D RenderToTexture(SpriteBatch spriteBatch)
        {
            var graphicsDevice = MachinaGame.Current.GraphicsDevice;
            var viewportSize = gameCanvas.ViewportSize;
            var renderTarget = new RenderTarget2D(
                                graphicsDevice,
                                viewportSize.X,
                                viewportSize.Y,
                                false,
                                graphicsDevice.PresentationParameters.BackBufferFormat,
                                DepthFormat.Depth24);
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            graphicsDevice.Clear(BackgroundColor);
            DrawOnCanvas(spriteBatch);

            graphicsDevice.SetRenderTarget(null);
            return renderTarget;
        }

        public Color BackgroundColor = Color.SlateBlue;

        public void AddPendingTextInput(object sender, TextInputEventArgs e)
        {
            this.pendingTextInput = e;
        }

        public SceneLayers(bool useDebugScene, IGameCanvas gameCanvas, IFrameStep frameStep)
        {
            this.frameStep = frameStep;
            this.gameCanvas = gameCanvas;

            if (useDebugScene)
            {
                this.debugScene = new Scene(this);
                this.debugScene.SetGameCanvas(new GameCanvas(gameCanvas.WindowSize, ResizeBehavior.FillContent));
            }
        }

        public void Delete()
        {
            while (this.sceneList.Count > 0)
            {
                this.sceneList.RemoveAt(0);
            }
        }

        public Scene AddNewScene()
        {
            var scene = new Scene(this, this.frameStep);
            Add(scene);
            return scene;
        }

        public void RemoveScene(Scene scene)
        {
            this.sceneList.Remove(scene);
        }

        /// <summary>
        /// This is private intentionally, you should only add a scene to the SceneLayers as a new scene via AddNewScene
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
            Scene[] array = new Scene[sceneList.Count + (debugScene != null ? 1 : 0)];
            sceneList.CopyTo(array);
            if (debugScene != null)
            {
                array[sceneList.Count] = debugScene;
            }
            return array;
        }

        public Scene[] AllScenesExceptDebug()
        {
            Scene[] array = new Scene[sceneList.Count];
            sceneList.CopyTo(array);
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

        public void Update(float dt, Matrix mouseTransformMatrix, InputFrameState inputFrameState, bool allowMouseUpdate = true, bool allowKeyboardEvents = true)
        {
            this.CurrentInputFrameState = inputFrameState;
            var scenes = AllScenes();

            var rawMousePos = Vector2.Transform(inputFrameState.mouseFrameState.RawWindowPosition.ToVector2(), mouseTransformMatrix);

            foreach (Scene scene in scenes)
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
                        scene.OnMouseUpdate(rawMousePos, inputFrameState.mouseFrameState.PositionDelta, inputFrameState.mouseFrameState.PositionDelta);
                    }
                }
            }

            this.pendingTextInput = null;

            foreach (Scene scene in scenes)
            {
                if (!scene.frameStep.IsPaused && !scene.IsFrozen)
                {
                    scene.Update(dt);
                }
            }

            HitTestResult.ApproveTopCandidate(scenes);
        }

        public void PreDraw(SpriteBatch spriteBatch)
        {
            var scenes = AllScenes();
            foreach (var scene in scenes)
            {
                scene.FlushBuffers();
                scene.PreDraw(spriteBatch);
            }
        }

        public void DrawOnCanvas(SpriteBatch spriteBatch)
        {
            var scenes = AllScenesExceptDebug();

            foreach (var scene in scenes)
            {
                scene.Draw(spriteBatch);
            }

            if (MachinaGame.DebugLevel > DebugLevel.Passive)
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
    }
}
