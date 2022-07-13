using System;
using System.Collections.Generic;
using ExTween;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Machina.Components
{
    public class IntroTextAnimation : BaseComponent
    {
        private readonly List<TweenableLetter> allLetters = new List<TweenableLetter>();
        private readonly TweenableFloat circleRadius = new TweenableFloat();

        private readonly NoiseBasedRNG random;
        private readonly Vector2 screenSize;
        private readonly SequenceTween tween;
        private float totalTime;

        public IntroTextAnimation(Actor actor, Vector2 screenSize) : base(actor)
        {
            this.screenSize = screenSize;
            var and = new List<TweenableLetter>();
            var notexplosive = new List<TweenableLetter>();
            var quarkimo = new List<TweenableLetter>();
            var gmtk = new List<TweenableLetter>();
            this.random = new NoiseBasedRNG((uint) MachinaClient.RandomDirty.Next());

            foreach (var letter in "NotExplosive")
            {
                notexplosive.Add(new TweenableLetter(letter.ToString()));
            }

            foreach (var letter in "&")
            {
                and.Add(new TweenableLetter(letter.ToString()));
            }

            foreach (var letter in "Ryan Yoshikami")
            {
                quarkimo.Add(new TweenableLetter(letter.ToString()));
            }

            foreach (var letter in "GMTK2022")
            {
                gmtk.Add(new TweenableLetter(letter.ToString()));
            }

            this.allLetters.AddRange(notexplosive);
            this.allLetters.AddRange(and);
            this.allLetters.AddRange(quarkimo);
            this.allLetters.AddRange(gmtk);

            var center = screenSize / 2;

            // GetArrangedLetterData(and, center);

            var notexplosiveArranged = GetArrangedLetterData(notexplosive, center + new Vector2(0, -200));
            var notexplosiveScattered = new List<LetterData>();
            var notexplosiveElevated = new List<LetterData>();

            for (var i = 0; i < notexplosive.Count; i++)
            {
                var letter = notexplosive[i];
                var arranged = notexplosiveArranged[i];

                var elevated = arranged;
                elevated.Position = arranged.Position + new Vector2(0, -150 - this.random.NextFloat() * 50);
                notexplosiveElevated.Add(elevated);

                var scattered = arranged;
                scattered.Position = new Vector2(
                    this.random.NextFloat() * center.X,
                    this.random.NextFloat() * center.Y) * 2;
                scattered.Angle = this.random.NextFloat() * MathF.PI * 2;
                scattered.Opacity = 0.5f;

                if (this.random.NextBool())
                {
                    scattered.ScaleX = this.random.NextFloat();
                }
                else
                {
                    scattered.ScaleY = this.random.NextFloat();
                }

                var scaleFactor = 5;
                scattered.Scale = this.random.NextBool()
                    ? this.random.NextFloat() * scaleFactor
                    : this.random.NextFloat() / scaleFactor;
                notexplosiveScattered.Add(scattered);

                letter.ForceSetPosition(scattered.Position + new Vector2(0, 1000));
                letter.Angle.ForceSetValue(this.random.NextFloat() * MathF.PI * 2);
                letter.Opacity.ForceSetValue(0f);
                letter.Scale.ForceSetValue(scattered.Scale);
                letter.ScaleX.ForceSetValue(scattered.ScaleX);
                letter.ScaleY.ForceSetValue(scattered.ScaleY);
            }

            foreach (var letter in gmtk)
            {
                letter.ForceSetPosition(center);
                letter.Opacity.ForceSetValue(0);
            }

            var andKeyframe = LetterData.Default();
            andKeyframe.Position = center;

            foreach (var letter in and)
            {
                letter.ForceSetPosition(andKeyframe.Position + new Vector2(-100, 0));
                letter.Angle.ForceSetValue(-MathF.PI * 2);
                letter.Opacity.ForceSetValue(0);
            }

            foreach (var letter in quarkimo)
            {
                letter.ScaleY.ForceSetValue(0);
                letter.Opacity.ForceSetValue(0);
            }

            var screenLength = Math.Max(screenSize.X, screenSize.Y);

            var lateLetterIndex = Math.Abs(this.random.Next()) % (notexplosive.Count - 1);

            this.tween = new SequenceTween()
                    .Add(new WaitSecondsTween(0.25f))
                    .Add(new Tween<float>(this.circleRadius, screenLength * 0.60f, 0.4f,
                        Ease.SineFastSlow))
                    .Add(new Tween<float>(this.circleRadius, screenLength * 0.55f, 0.1f,
                        Ease.SineFastSlow))
                    .Add(new DynamicTween(() =>
                    {
                        var result = new MultiplexTween();

                        for (var i = 0; i < notexplosive.Count; i++)
                        {
                            var letter = notexplosive[i];
                            var arranged = notexplosiveArranged[i];
                            if (i != lateLetterIndex)
                            {
                                result.AddChannel(
                                    new SequenceTween()
                                        .Add(new WaitSecondsTween(i * 0.02f))
                                        .Add(letter.TweenAllValues(notexplosiveScattered[i],
                                            0.25f + this.random.NextFloat() * 0.75f,
                                            Ease.QuadFastSlow))
                                        .Add(letter.TweenAllValuesSomeLinear(notexplosiveElevated[i],
                                            0.5f + this.random.NextFloat() / 2,
                                            Ease.QuadFastSlow))
                                        .Add(new WaitSecondsTween(0.2f))
                                        .Add(
                                            new DynamicTween(() =>
                                            {
                                                var slightlyOff = arranged;
                                                slightlyOff.Angle += (this.random.NextFloat() - 0.5f) * MathF.PI / 4;
                                                slightlyOff.Position += new Vector2(0, 10);
                                                return letter.TweenAllValues(slightlyOff,
                                                    0.15f + this.random.NextFloat() / 8, Ease.QuadSlowFast);
                                            })
                                        )
                                        .Add(new CallbackTween(() => MachinaClient.SoundEffectPlayer.PlaySound("jar2", baseVolume: 0.75f, this.random.NextFloat() / 2f)))
                                        .Add(
                                            letter.TweenAllValues(arranged, 0.05f + this.random.NextFloat() / 8,
                                                Ease.QuadSlowFast)
                                        )
                                );
                            }
                        }

                        // LATE LETTER
                        var lateLetter = notexplosive[lateLetterIndex];
                        var arrangedLateLetter = notexplosiveArranged[lateLetterIndex];

                        result.AddChannel(
                            new SequenceTween()
                                .Add(new WaitSecondsTween(2))
                                .Add(lateLetter.TweenAllValues(notexplosiveScattered[lateLetterIndex], 0.45f,
                                    Ease.QuadFastSlow))
                                .Add(lateLetter.TweenAllValuesSomeLinear(notexplosiveElevated[lateLetterIndex], 0.25f,
                                    Ease.QuadSlowFast))
                                .Add(new WaitSecondsTween(0.1f))
                                .Add(
                                    new DynamicTween(() =>
                                    {
                                        var target = arrangedLateLetter;
                                        target.Position += new Vector2(0, 50);
                                        return lateLetter.TweenAllValues(target, 0.05f, Ease.QuadSlowFast);
                                    })
                                )
                                .Add(new CallbackTween(() =>
                                {
                                    MachinaClient.SoundEffectPlayer.PlaySound("ouch", 0.5f);
                                }))
                                .Add(
                                    new MultiplexTween()
                                        .AddChannel(
                                            new DynamicTween(() =>
                                            {
                                                var result2 = new MultiplexTween();

                                                for (var i = 0; i < notexplosive.Count; i++)
                                                {
                                                    var letter = notexplosive[i];
                                                    if (i != lateLetterIndex)
                                                    {
                                                        // LATE LETTER IMPACT SHOCKWAVE
                                                        var arranged = notexplosiveArranged[i];
                                                        var tweaked = notexplosiveArranged[i];
                                                        var distance = Math.Abs(lateLetterIndex - i);
                                                        tweaked.Position += new Vector2(0, -80f);
                                                        tweaked.Angle = 0;
                                                        result2.AddChannel(
                                                                new SequenceTween()
                                                                    .Add(new WaitSecondsTween(distance / 40f))
                                                                    .Add(letter.TweenAllValues(tweaked, 0.15f,
                                                                        Ease.QuadFastSlow))
                                                                    .Add(letter.TweenAllValues(arranged, 0.15f,
                                                                        Ease.QuadSlowFast))
                                                            )
                                                            ;
                                                    }
                                                }

                                                return result2;
                                            })
                                        )
                                        // Late letter corrects itself
                                        .AddChannel(lateLetter.TweenAllValues(notexplosiveArranged[lateLetterIndex],
                                            0.1f,
                                            Ease.QuadSlowFast))
                                )
                        );

                        return result;
                    }))
                    .Add(and[0].TweenAllValues(andKeyframe, 0.25f, Ease.QuadFastSlow))
                    .Add(
                        new DynamicTween(() =>
                        {
                            var result = new MultiplexTween();
                            var arrangedWord = GetArrangedLetterData(quarkimo, center + new Vector2(0, 200));
                            var deployedWord = GetArrangedLetterData(quarkimo, center + new Vector2(0, 200));

                            for (var i = 0; i < quarkimo.Count; i++)
                            {
                                deployedWord[i] = arrangedWord[i];
                                deployedWord[i].PositionY += 50;

                                quarkimo[i].ForceSetPosition(arrangedWord[i].Position);
                            }

                            for (var i = 0; i < quarkimo.Count; i++)
                            {
                                var letter = quarkimo[i];
                                result.AddChannel(
                                    new SequenceTween()
                                        .Add(new WaitSecondsTween(i / 10f))
                                        .Add(letter.TweenAllValues(deployedWord[i], 0.1f, Ease.SineFastSlow))
                                        .Add(letter.TweenAllValues(arrangedWord[i], 0.1f, Ease.SineSlowFast))
                                );
                            }

                            return result;
                        }))
                    .Add(new WaitSecondsTween(1))
                    .Add(new MultiplexTween()
                        .AddChannel(
                            new SequenceTween()
                                .Add(new Tween<float>(this.circleRadius, screenLength * 0.6f, 0.1f, Ease.SineSlowFast))
                                .Add(new Tween<float>(this.circleRadius, screenLength * 0.3f, 0.3f, Ease.SineFastSlow))
                        )
                        .AddChannel(
                            new DynamicTween(() =>
                            {
                                var result = new MultiplexTween();
                                var allNames = new List<TweenableLetter>();
                                allNames.AddRange(notexplosive);
                                allNames.AddRange(quarkimo);
                                allNames.AddRange(and);

                                foreach (var letter in allNames)
                                {
                                    var data = letter.Data();
                                    data.Position = center;
                                    data.Opacity = 0f;
                                    result.AddChannel(letter.TweenAllValues(data, 0.4f, Ease.SineFastSlow));
                                }

                                return result;
                            })
                        )
                    )
                    .Add(new DynamicTween(() =>
                    {
                        var result = new MultiplexTween();

                        var arranged = GetArrangedLetterData(gmtk, center);

                        for (var i = 0; i < gmtk.Count; i++)
                        {
                            var letter = gmtk[i];

                            var target = letter.Data();
                            target.Position = arranged[i].Position;
                            target.Opacity = 1f;
                            result.AddChannel(
                                new SequenceTween()
                                    .Add(new WaitSecondsTween((target.Position - arranged[i].Position).Length() / 25))
                                    .Add(letter.TweenAllValues(target, 0.5f, Ease.SineFastSlow))
                            );
                        }

                        return result;
                    }))
                    .Add(new WaitSecondsTween(1.5f))
                    .Add(
                        new MultiplexTween()
                            .AddChannel(new SequenceTween()
                                .Add(new Tween<float>(this.circleRadius, screenLength * 0.35f, 0.1f, Ease.SineSlowFast))
                                .Add(new Tween<float>(this.circleRadius, 0, 0.3f, Ease.SineFastSlow))
                            )
                            .AddChannel(new DynamicTween(() =>
                            {
                                var result = new MultiplexTween();

                                for (var i = 0; i < gmtk.Count; i++)
                                {
                                    var letter = gmtk[i];
                                    var target = letter.Data();
                                    target.ScaleX = 0f;
                                    target.Opacity = 0f;
                                    target.Position = center;
                                    result.AddChannel(
                                        new SequenceTween()
                                            .Add(letter.TweenAllValues(target, 0.25f, Ease.SineFastSlow))
                                    );
                                }

                                return result;
                            }))
                    )
                    .Add(new WaitSecondsTween(0.25f))
                    .Add(new CallbackTween(() =>
                    {
                        // this will trigger the load into the next scene
                        this.actor.Destroy();
                    }))
                ;
        }

        private LetterData[] GetArrangedLetterData(List<TweenableLetter> letters, Vector2 centerPos)
        {
            var result = new LetterData[letters.Count];
            var totalStringWidth = 0f;
            foreach (var letter in letters)
            {
                totalStringWidth += letter.Size.X;
            }

            var startPosition = centerPos - new Vector2(totalStringWidth, 0) / 2;
            var letterOffset = Vector2.Zero;

            for (var i = 0; i < letters.Count; i++)
            {
                letterOffset.X += letters[i].Size.X / 2;
                result[i] = LetterData.Default();
                result[i].Position = startPosition + letterOffset;
                letterOffset.X += letters[i].Size.X / 2;
            }

            return result;
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (state == ButtonState.Pressed && modifiers.None)
            {
                this.actor.Destroy();
            }
        }

        public override void Update(float dt)
        {
            this.totalTime += dt;
            this.tween.Update(dt);

            foreach (var letter in this.allLetters)
            {
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.circleRadius > 0f)
            {
                var richBlack = new Color(0x07, 0x39, 0x3C);
                var ming = new Color(0x2C, 0x66, 0x6E);
                var spaceCadet = new Color(0x11, 0x1D, 0x4A);

                var radius = this.circleRadius + MathF.Sin(this.totalTime / 10f) * this.circleRadius * 0.1f;

                spriteBatch.DrawCircle(new CircleF(this.screenSize / 2, radius * 0.9f), 50, richBlack, 5,
                    Depth.Max - 2);
                spriteBatch.DrawCircle(new CircleF(this.screenSize / 2, radius * 0.8f), 50, ming,
                    radius * 0.8f, Depth.Max - 1);
                spriteBatch.DrawCircle(new CircleF(this.screenSize / 2, radius), 50, spaceCadet,
                    radius, Depth.Max);
            }

            foreach (var letter in this.allLetters)
            {
                letter.Draw(spriteBatch, Runtime.DebugLevel == DebugLevel.Active);
            }
        }

        private struct LetterData
        {
            public float Opacity { get; set; }
            public float Scale { get; set; }
            public float ScaleX { get; set; }
            public float ScaleY { get; set; }
            public float PositionX { get; set; }
            public float PositionY { get; set; }
            public float Angle { get; set; }

            public Vector2 Position
            {
                get => new Vector2(PositionX, PositionY);

                set
                {
                    PositionX = value.X;
                    PositionY = value.Y;
                }
            }

            public static LetterData Default()
            {
                return new LetterData
                {
                    Scale = 1f,
                    ScaleX = 1f,
                    ScaleY = 1f,
                    Opacity = 1f
                };
            }
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
            public TweenableFloat PositionX { get; } = new TweenableFloat();
            public TweenableFloat PositionY { get; } = new TweenableFloat();
            public Vector2 Size { get; }

            public void Draw(SpriteBatch spriteBatch, bool isDebug)
            {
                if (isDebug)
                {
                    spriteBatch.DrawCircle(new CircleF(new Vector2(PositionX, PositionY), 15), 10, Color.Red, 3f);
                }

                var textColor = new Color(0x90, 0xDD, 0xF0);

                var offset = new Vector2(Size.X, Size.Y) / 2;
                spriteBatch.DrawString(this.font, this.content,
                    new Vector2(PositionX, PositionY),
                    textColor.WithMultipliedOpacity(Opacity),
                    Angle, offset, new Vector2(ScaleX, ScaleY) * Scale, SpriteEffects.None, 0f);
            }

            public LetterData Data()
            {
                return new LetterData
                {
                    Opacity = Opacity.Value,
                    Scale = Scale.Value,
                    Angle = Angle.Value,
                    PositionX = PositionX.Value,
                    PositionY = PositionY.Value,
                    ScaleX = ScaleX.Value,
                    ScaleY = ScaleY.Value
                };
            }

            public ITween TweenAllValuesSomeLinear(LetterData data, float duration, Ease.Delegate ease)
            {
                // Position X and Angle are interpolated linearly
                return new MultiplexTween()
                    .AddChannel(new Tween<float>(PositionX, data.PositionX,
                        duration, Ease.Linear))
                    .AddChannel(new Tween<float>(PositionY, data.PositionY,
                        duration, ease))
                    .AddChannel(new Tween<float>(Angle, data.Angle, duration,
                        Ease.Linear))
                    .AddChannel(new Tween<float>(Scale, data.Scale, duration,
                        ease))
                    .AddChannel(new Tween<float>(ScaleX, data.ScaleX, duration,
                        ease))
                    .AddChannel(new Tween<float>(ScaleY, data.ScaleY, duration,
                        ease))
                    .AddChannel(new Tween<float>(Opacity, data.Opacity, duration,
                        ease));
            }

            public ITween TweenAllValues(LetterData data, float duration, Ease.Delegate ease)
            {
                return new MultiplexTween()
                    .AddChannel(new Tween<float>(PositionX, data.PositionX,
                        duration, ease))
                    .AddChannel(new Tween<float>(PositionY, data.PositionY,
                        duration, ease))
                    .AddChannel(new Tween<float>(Angle, data.Angle, duration,
                        ease))
                    .AddChannel(new Tween<float>(Scale, data.Scale, duration,
                        ease))
                    .AddChannel(new Tween<float>(ScaleX, data.ScaleX, duration,
                        ease))
                    .AddChannel(new Tween<float>(ScaleY, data.ScaleY, duration,
                        ease))
                    .AddChannel(new Tween<float>(Opacity, data.Opacity, duration,
                        ease));
            }

            public void ForceSetPosition(Vector2 position)
            {
                PositionX.ForceSetValue(position.X);
                PositionY.ForceSetValue(position.Y);
            }
        }
    }
}
