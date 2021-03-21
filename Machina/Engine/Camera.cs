using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    // Initial implementation of this camera comes from https://roguesharp.wordpress.com/2014/07/13/tutorial-5-creating-a-2d-camera-with-pan-and-zoom-in-monogame/
    public class Camera
    {
        private GameCanvas resizer;

        public Camera(GameCanvas resizer = null)
        {
            Zoom = 1.0f;

            if (resizer != null)
            {
                this.resizer = resizer;
            }

            ZoomTarget = () => { return this.ViewportCenter; };
        }

        public Vector2 Position
        {
            get; set;
        }

        private float zoom;
        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;
                OnChangeZoom?.Invoke(zoom, value);
            }
        }

        public float Rotation
        {
            get; set;
        }

        public int ViewportWidth
        {
            get => resizer != null ? resizer.ViewportSize.X : 0;
        }
        public int ViewportHeight
        {
            get => resizer != null ? resizer.ViewportSize.Y : 0;
        }

        public Vector2 CanvasTopLeft => resizer != null ? resizer.CanvasRect.Location.ToVector2() : Vector2.Zero;
        public Func<Vector2> ZoomTarget;
        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }

        public Matrix MouseDeltaMatrix =>
            Matrix.CreateScale(NativeScaleFactor, NativeScaleFactor, 1)
            * RotationAndZoomMatrix
            ;

        /// <summary>
        /// The rotation and scale transforms applied to the camera
        /// </summary>
        public Matrix RotationAndZoomMatrix =>
            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
            * Matrix.CreateRotationZ(Rotation)
            ;

        /// <summary>
        /// This is the matrix passed to SpriteBatch to render the scene, if you're using a GameCanvas
        /// this isn't quite enough.
        /// </summary>
        public Matrix GraphicsTransformMatrix =>
            Matrix.CreateTranslation(-(int) Position.X, -(int) Position.Y, 0)
            * Matrix.CreateTranslation(new Vector3(-ZoomTarget(), 0))
            * RotationAndZoomMatrix
            * Matrix.CreateTranslation(new Vector3(ZoomTarget(), 0))
            ;

        public float NativeScaleFactor
        {
            get => resizer != null ? resizer.ScaleFactor : 1.0f;
        }

        /// <summary>
        /// All of the transforms needed to convert screen to world and world to screen.
        /// </summary>
        public Matrix GameCanvasMatrix =>
                 GraphicsTransformMatrix
                * Matrix.CreateScale(NativeScaleFactor, NativeScaleFactor, 1)
                * Matrix.CreateTranslation(new Vector3(CanvasTopLeft, 0))
            ;

        public Action<float, float> OnChangeZoom;

        public void AdjustZoom(float amount)
        {
            Zoom += amount;
            if (Zoom < 0.25f)
            {
                Zoom = 0.25f;
            }
        }

        /// <summary>
        /// CAUTION: You'll almost definitely not want to use this, the SpriteBatch is already doing this transform for you
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
        /// Used to translate screen-based concepts (like mouse position) to the world
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
