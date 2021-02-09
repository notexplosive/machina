using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    // Initial implementation of this camera comes from https://roguesharp.wordpress.com/2014/07/13/tutorial-5-creating-a-2d-camera-with-pan-and-zoom-in-monogame/
    public class Camera
    {
        public Camera()
        {
            Zoom = 1.0f;
            NativeScaleFactor = 1.0f;
        }

        public Vector2 Position
        {
            get; set;
        }

        public float Zoom
        {
            get; private set;
        }

        public float Rotation
        {
            get; set;
        }

        public int ViewportWidth
        {
            get; set;
        }
        public int ViewportHeight
        {
            get; set;
        }

        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }

        public Matrix TranslationMatrix
        {
            get
            {
                return
                    Matrix.CreateTranslation(-(int) Position.X, -(int) Position.Y, 0)
                    * Matrix.CreateRotationZ(Rotation)
                    * Matrix.CreateScale(new Vector3(Scale, Scale, 1))
                    * Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
            }
        }

        public float Scale => Zoom * NativeScaleFactor;

        public float NativeScaleFactor
        {
            get;
            set;
        }

        public void AdjustZoom(float amount)
        {
            Zoom += amount;
            if (Zoom < 0.25f)
            {
                Zoom = 0.25f;
            }
        }

        public void MoveCamera(Vector2 cameraMovement, Vector2 cameraMax = new Vector2())
        {
            Vector2 newPosition = Position + cameraMovement;

            if (cameraMax.Length() > 0)
            {
                Position = MapClampedPosition(newPosition, cameraMax);
            }
            else
            {
                Position = newPosition;
            }
        }

        public Rectangle ViewportWorldBoundry()
        {
            Vector2 viewPortCorner = ScreenToWorld(new Vector2(0, 0));
            Vector2 viewPortBottomCorner =
               ScreenToWorld(new Vector2(ViewportWidth, ViewportHeight));

            return new Rectangle((int) viewPortCorner.X,
               (int) viewPortCorner.Y,
               (int) (viewPortBottomCorner.X - viewPortCorner.X),
               (int) (viewPortBottomCorner.Y - viewPortCorner.Y));
        }

        // Center the camera on specific pixel coordinates
        public void CenterOn(Vector2 position)
        {
            Position = position;
        }

        // Clamp the camera so it never leaves the visible area of the map.
        private Vector2 MapClampedPosition(Vector2 position, Vector2 cameraMax)
        {
            return Vector2.Clamp(position,
               new Vector2(ViewportWidth / Zoom / 2, ViewportHeight / Zoom / 2),
               cameraMax);
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition,
                Matrix.Invert(TranslationMatrix));
        }
    }
}
