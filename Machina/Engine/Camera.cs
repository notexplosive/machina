using System;
using Microsoft.Xna.Framework;

namespace Machina.Engine
{
    // Initial implementation of this camera comes from https://roguesharp.wordpress.com/2014/07/13/tutorial-5-creating-a-2d-camera-with-pan-and-zoom-in-monogame/
    public class Camera
    {
        private readonly IGameViewport gameCanvas;

        public Action<float, float> OnChangeZoom;

        private float zoom;

        public Camera(IGameViewport gameCanvas)
        {
            Zoom = 1.0f;
            this.gameCanvas = gameCanvas;
            ZoomTarget = () => { return UnscaledViewportCenter; };
        }

        /// <summary>
        ///     How offset the camera is from its initial position
        /// </summary>
        public Vector2 UnscaledPosition { get; set; }

        public float Zoom
        {
            get => this.zoom;
            set
            {
                this.zoom = value;
                this.OnChangeZoom?.Invoke(this.zoom, value);
            }
        }

        public float Rotation { get; set; }

        [Obsolete("Use UnscaledViewportSize.X instead", false)]
        public int ViewportWidth => UnscaledViewportSize.X;

        [Obsolete("Use UnscaledViewportSize.Y instead", false)]
        public int ViewportHeight => UnscaledViewportSize.Y;

        public Point UnscaledViewportSize => this.gameCanvas.ViewportSize.ToVector2().ToPoint();
        public Vector2 ScaledViewportSize => this.gameCanvas.ViewportSize.ToVector2() / this.zoom;

        public Point ScaledPosition
        {
            get
            {
                var scaledPosVec = ScreenToWorld(CanvasTopLeft);
                scaledPosVec.Round();
                return scaledPosVec.ToPoint();
            }
            set
            {
                var old = ScaledPosition;
                var offset = value - old;
                UnscaledPosition += offset.ToVector2();
            }
        }

        public Vector2 CanvasTopLeft => this.gameCanvas.CanvasRect.Location.ToVector2();

        public Func<Vector2> ZoomTarget { get; set; }

        public Vector2 ViewportCenter => ScaledViewportSize / 2f;

        public Vector2 UnscaledViewportCenter => UnscaledViewportSize.ToVector2() / 2f;

        public Matrix MouseDeltaMatrix =>
            Matrix.CreateScale(NativeScaleFactor, NativeScaleFactor, 1)
            * RotationAndZoomMatrix;

        /// <summary>
        ///     The rotation and scale transforms applied to the camera
        /// </summary>
        public Matrix RotationAndZoomMatrix =>
            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
            * Matrix.CreateRotationZ(Rotation);

        /// <summary>
        ///     This is the matrix passed to SpriteBatch to render the scene, if you're using a GameCanvas
        ///     this isn't quite enough.
        /// </summary>
        public Matrix GraphicsTransformMatrix =>
            Matrix.CreateTranslation(-(int) UnscaledPosition.X, -(int) UnscaledPosition.Y, 0)
            * Matrix.CreateTranslation(new Vector3(-ZoomTarget(), 0))
            * RotationAndZoomMatrix
            * Matrix.CreateTranslation(new Vector3(ZoomTarget(), 0));

        public float NativeScaleFactor => this.gameCanvas.ScaleFactor;

        /// <summary>
        ///     All of the transforms needed to convert screen to world and world to screen.
        /// </summary>
        public Matrix GameCanvasMatrix =>
            GraphicsTransformMatrix
            * Matrix.CreateScale(NativeScaleFactor, NativeScaleFactor, 1)
            * Matrix.CreateTranslation(new Vector3(CanvasTopLeft, 0));

        public void AdjustZoom(float amount)
        {
            Zoom += amount;
            if (Zoom <= 0f)
            {
                Zoom = 0.0001f;
            }
        }

        /// <summary>
        ///     CAUTION: You'll almost definitely not want to use this, the SpriteBatch is already doing this transform for you
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition,
                GameCanvasMatrix
            );
        }

        /// <summary>
        ///     Used to translate screen-based concepts (like mouse position) to the world
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition,
                Matrix.Invert(
                    GameCanvasMatrix
                ));
        }
    }
}