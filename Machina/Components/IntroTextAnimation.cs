﻿using System;
using System.Collections.Generic;
using ExTween;
using ExTween.MonoGame;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public class IntroTextAnimation : BaseComponent
    {
        private readonly List<TweenableLetter> allLetters = new List<TweenableLetter>();

        private readonly NoiseBasedRNG random;
        private readonly SequenceTween tween;

        public IntroTextAnimation(Actor actor, Vector2 size) : base(actor)
        {
            var and = new List<TweenableLetter>();
            var notexplosive = new List<TweenableLetter>();
            var quarkimo = new List<TweenableLetter>();
            this.random = new NoiseBasedRNG((uint) new Random().Next());

            foreach (var letter in "NotExplosive")
            {
                notexplosive.Add(new TweenableLetter(letter.ToString()));
            }

            foreach (var letter in "&")
            {
                and.Add(new TweenableLetter(letter.ToString()));
            }

            foreach (var letter in "quarkimo")
            {
                quarkimo.Add(new TweenableLetter(letter.ToString()));
            }

            this.allLetters.AddRange(notexplosive);
            this.allLetters.AddRange(and);
            this.allLetters.AddRange(quarkimo);

            var center = size / 2;

            ArrangeLettersCenter(notexplosive, center + new Vector2(0, -200));
            //ArrangeLettersCenter(and, center);
            //ArrangeLettersCenter(quarkimo, center + new Vector2(0, 200));

            var notexplosiveArranged = new List<LetterData>();
            var notexplosiveArrangedSlightOff = new List<LetterData>();
            var notexplosiveScattered = new List<LetterData>();
            var notexplosiveElevated = new List<LetterData>();

            foreach (var letter in notexplosive)
            {
                var arranged = letter.Data();
                notexplosiveArranged.Add(arranged);

                var elevated = arranged;
                elevated.Position = new Vector2(
                    letter.Position.Value.X,
                    letter.Position.Value.Y - 200 - this.random.NextFloat() * 50);
                
                notexplosiveElevated.Add(elevated);

                var slightlyOff = arranged;
                slightlyOff.Angle += (this.random.NextFloat() - 0.5f) * MathF.PI / 4;
                slightlyOff.Position += new Vector2(0, 10);
                notexplosiveArrangedSlightOff.Add(slightlyOff);

                letter.Position.ForceSetValue(
                        new Vector2(
                            this.random.NextFloat() * center.X,
                            this.random.NextFloat() * center.Y) * 2)
                    ;
                letter.Angle.ForceSetValue(this.random.NextFloat() * MathF.PI * 2);
                var scaleFactor = 5;
                letter.Scale.ForceSetValue(this.random.NextBool()
                    ? this.random.NextFloat() * scaleFactor
                    : this.random.NextFloat() / scaleFactor);

                var scattered = letter.Data();
                notexplosiveScattered.Add(scattered);

                letter.Position.ForceSetValue(
                        // reuse scattered position, I want it to be the same
                        scattered.Position
                        // add offset
                        + new Vector2(0, 1000)
                    )
                    ;
                letter.Angle.ForceSetValue(this.random.NextFloat() * MathF.PI * 2);

                letter.Opacity.ForceSetValue(0f);
            }

            var lateLetterIndex = Math.Abs(this.random.Next()) % (notexplosive.Count - 1);

            this.tween = new SequenceTween()
                    .Add(new DynamicTween(() =>
                    {
                        var result = new MultiplexTween();

                        for (var i = 0; i < notexplosive.Count; i++)
                        {
                            var letter = notexplosive[i];
                            if (i == lateLetterIndex)
                            {
                                // LATE LETTER
                                result.AddChannel(
                                    new SequenceTween()
                                        .Add(new WaitSecondsTween(2))
                                        .Add(letter.Tween(notexplosiveScattered[i], 0.25f,
                                            Ease.QuadFastSlow))
                                        .Add(letter.Tween(notexplosiveElevated[i], 0.25f,
                                            Ease.QuadSlowFast))
                                        .Add(new WaitSecondsTween(0.1f))
                                        .Add(letter.Tween(notexplosiveArrangedSlightOff[i],
                                            0.05f, Ease.QuadSlowFast))
                                        .Add(
                                            new MultiplexTween()
                                                .AddChannel(
                                                    new DynamicTween(() =>
                                                    {
                                                        var result2 = new MultiplexTween();

                                                        for (var j = 0; j < notexplosive.Count; j++)
                                                        {
                                                            var letter2 = notexplosive[j];
                                                            if (j != lateLetterIndex)
                                                            {
                                                                // LATE LETTER IMPACT
                                                                var arranged = notexplosiveArranged[j];
                                                                var tweaked = notexplosiveArranged[j];
                                                                var distance = Math.Abs(lateLetterIndex - j);
                                                                tweaked.Scale = 1 + 0.25f / distance;
                                                                tweaked.Position += new Vector2(0, -40f);
                                                                tweaked.Angle = this.random.NextFloat() - 0.5f;
                                                                result2.AddChannel(
                                                                        new SequenceTween()
                                                                            .Add(new WaitSecondsTween(distance / 40f))
                                                                            .Add(letter2.Tween(tweaked, 0.15f,
                                                                                Ease.QuadFastSlow))
                                                                            .Add(letter2.Tween(arranged, 0.15f,
                                                                                Ease.QuadSlowFast))
                                                                    )
                                                                    ;
                                                            }
                                                        }

                                                        return result2;
                                                    })
                                                )
                                                .AddChannel(letter.Tween(notexplosiveArranged[i], 0.1f,
                                                    Ease.QuadSlowFast))
                                        )
                                );
                            }
                            else
                            {
                                result.AddChannel(
                                    new SequenceTween()
                                        .Add(new WaitSecondsTween(i * 0.01f))
                                        .Add(letter.Tween(notexplosiveScattered[i], 0.5f + this.random.NextFloat() / 2,
                                            Ease.QuadFastSlow))
                                        .Add(letter.Tween(notexplosiveElevated[i], 0.5f + this.random.NextFloat() / 2,
                                            Ease.QuadFastSlow))
                                        .Add(letter.Tween(notexplosiveArrangedSlightOff[i],
                                            0.15f + this.random.NextFloat() / 8, Ease.QuadSlowFast))
                                        .Add(letter.Tween(notexplosiveArranged[i], 0.05f + this.random.NextFloat() / 8,
                                            Ease.QuadSlowFast))
                                );
                            }
                        }

                        return result;
                    }))
                // .Add() and quarkimo down here
                ;
        }

        private void ArrangeLettersCenter(List<TweenableLetter> tweenableLetters, Vector2 centerPos)
        {
            var totalStringWidth = 0f;
            foreach (var letter in tweenableLetters)
            {
                totalStringWidth += letter.Size.X;
            }

            var startPosition = centerPos - new Vector2(totalStringWidth, 0) / 2;
            var letterOffset = Vector2.Zero;

            foreach (var letter in tweenableLetters)
            {
                letter.Position.ForceSetValue(startPosition + letterOffset);
                letterOffset.X += letter.Size.X;
            }
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);

            foreach (var letter in this.allLetters)
            {
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var letter in this.allLetters)
            {
                letter.Draw(spriteBatch);
            }
        }

        private struct LetterData
        {
            public float Opacity { get; set; }
            public float Scale { get; set; }
            public float ScaleX { get; set; }
            public float ScaleY { get; set; }
            public float Angle { get; set; }
            public Vector2 Position { get; set; }
        }

        private class TweenableLetter
        {
            private readonly string content;
            private readonly SpriteFont font;

            public TweenableLetter(string content)
            {
                this.content = content;
                this.font = MachinaClient.Assets.GetSpriteFont("LogoFont");
                Size = this.font.MeasureString(content);
            }

            public TweenableFloat Opacity { get; } = new TweenableFloat(1f);
            public TweenableFloat Scale { get; } = new TweenableFloat(1f);
            public TweenableFloat ScaleX { get; } = new TweenableFloat(1f);
            public TweenableFloat ScaleY { get; } = new TweenableFloat(1f);
            public TweenableFloat Angle { get; } = new TweenableFloat();
            public TweenableVector2 Position { get; } = new TweenableVector2();
            public Vector2 Size { get; }

            public void Draw(SpriteBatch spriteBatch)
            {
                var offset = new Vector2(Size.X, Size.Y) / 2;
                spriteBatch.DrawString(this.font, this.content, Position + offset - new Vector2(0, offset.Y),
                    Color.White.WithMultipliedOpacity(Opacity),
                    Angle, offset, new Vector2(ScaleX, ScaleY) * Scale, SpriteEffects.None, 0f);
            }

            public LetterData Data()
            {
                return new LetterData
                {
                    Opacity = Opacity.Value,
                    Scale = Scale.Value,
                    Angle = Angle.Value,
                    Position = Position.Value,
                    ScaleX = ScaleX.Value,
                    ScaleY = ScaleY.Value,
                };
            }

            public ITween Tween(LetterData data, float duration, Ease.Delegate ease)
            {
                return new MultiplexTween()
                    .AddChannel(new Tween<Vector2>(Position, data.Position,
                        duration, ease))
                    .AddChannel(new Tween<float>(Angle, data.Angle, duration,
                        ease))
                    .AddChannel(new Tween<float>(Scale, data.Scale, duration,
                        ease))
                    .AddChannel(new Tween<float>(Opacity, data.Opacity, duration,
                        ease));
            }
        }
    }
}
