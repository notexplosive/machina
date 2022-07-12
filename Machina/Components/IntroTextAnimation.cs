using System;
using System.Collections.Generic;
using ExTween;
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

            // GetArrangedLetterData(and, center);
            // GetArrangedLetterData(quarkimo, center + new Vector2(0, 200));

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
                var scaleFactor = 5;
                scattered.Scale = this.random.NextBool()
                    ? this.random.NextFloat() * scaleFactor
                    : this.random.NextFloat() / scaleFactor;
                notexplosiveScattered.Add(scattered);

                letter.ForceSetPosition(scattered.Position + new Vector2(0, 1000));
                letter.Angle.ForceSetValue(this.random.NextFloat() * MathF.PI * 2);
                letter.Opacity.ForceSetValue(0f);
                letter.Scale.ForceSetValue(scattered.Scale);
            }
            
            var andKeyframe = LetterData.Default();
            andKeyframe.Position = center;

            foreach (var letter in and)
            {
                letter.ForceSetPosition(andKeyframe.Position + new Vector2(-100,0));
                letter.Angle.ForceSetValue(-MathF.PI * 2);
                letter.Opacity.ForceSetValue(0);
            }

            var lateLetterIndex = Math.Abs(this.random.Next()) % (notexplosive.Count - 1);

            this.tween = new SequenceTween()
                    .Add(new WaitSecondsTween(0.25f))
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
                                        .Add(new WaitSecondsTween(i * 0.01f))
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
                            // todo: quarkimo
                            return new CallbackTween(() => { }); // placeholder
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
                result[i] = LetterData.Default();
                result[i].Position = startPosition + letterOffset;
                letterOffset.X += letters[i].Size.X;
            }

            return result;
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

            public void Draw(SpriteBatch spriteBatch)
            {
                var offset = new Vector2(Size.X, Size.Y) / 2;
                spriteBatch.DrawString(this.font, this.content,
                    new Vector2(PositionX, PositionY) + offset - new Vector2(0, offset.Y),
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
