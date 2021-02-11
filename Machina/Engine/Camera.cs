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
        }

        public Vector2 Position
        {
            get; set;
        }

        public float Zoom
        {
            get; set;
        }

        public float Rotation
        {
            get; set;
        }

        public int ViewportWidth
        {
            get => resizer != null ? resizer.WorldSize.X : 0;
        }
        public int ViewportHeight
        {
            get => resizer != null ? resizer.WorldSize.Y : 0;
        }

        public Vector2 CanvasTopLeft => resizer != null ? resizer.CanvasRect.Location.ToVector2() : Vector2.Zero;

        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }

        /// <summary>
        /// I think this should also include a translation that's currently in
        /// away by MouseTracker.Calculate(), then its purpose would become more clear
        /// </summary>
        public Matrix MouseDeltaMatrix =>
            Matrix.CreateScale(NativeScaleFactor, NativeScaleFactor, 1)
            * TransformMatrix
            ;

        /// <summary>
        /// The rotation and scale transforms applied to the camera
        /// </summary>
        public Matrix TransformMatrix =>
            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
            * Matrix.CreateRotationZ(Rotation)
            ;

        /// <summary>
        /// This is the matrix passed to SpriteBatch to render the scene, if you're using a GameCanvas
        /// this isn't quite enough.
        /// </summary>
        public Matrix TranslationMatrix =>

            // Matrix.CreateTranslation(new Vector3(CanvasTopLeft.ToVector2(), 0))
            // * Matrix.CreateScale(NativeScaleFactor)

            Matrix.CreateTranslation(-(int) Position.X, -(int) Position.Y, 0)
            * Matrix.CreateTranslation(new Vector3(-ViewportCenter, 0))
            * TransformMatrix
            * Matrix.CreateTranslation(new Vector3(ViewportCenter, 0))

            ;

        public float NativeScaleFactor
        {
            get => resizer != null ? resizer.ScaleFactor : 1.0f;
        }

        /// <summary>
        /// All of the transforms needed to convert screen to world and world to screen.
        /// </summary>
        public Matrix GameCanvasScaleFactorMatrix =>

            Matrix.Invert(Matrix.CreateTranslation(new Vector3(CanvasTopLeft, 0)))
            * Matrix.Invert(Matrix.CreateScale(NativeScaleFactor, NativeScaleFactor, 1))
            * Matrix.Invert(TranslationMatrix)
            ;

        public void AdjustZoom(float amount)
        {
            Zoom += amount;
            if (Zoom < 0.25f)
            {
                Zoom = 0.25f;
            }
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition,
                    GameCanvasScaleFactorMatrix
                    );
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition,
                Matrix.Invert(
                    GameCanvasScaleFactorMatrix
                    ));
        }
    }
}
