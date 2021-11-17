using System;
using System.Collections.Generic;
using System.IO;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Machina.Engine
{
    public class Demo
    {
        public static readonly string MostRecentlySavedDemoPath = "saved.demo";
        public readonly List<DemoSerializableEntry> records = new List<DemoSerializableEntry>();
        private readonly int seedAtStart;

        public Demo(int randomSeed)
        {
            this.seedAtStart = randomSeed;
        }

        public void Append(DemoSerializableEntry record)
        {
            this.records.Add(record);
        }

        public string EncodeRecords()
        {
            return JsonConvert.SerializeObject(new DemoContent(this.records, this.seedAtStart));
        }

        public static Demo DecodeRecords(string encodedRecords)
        {
            var content = JsonConvert.DeserializeObject<DemoContent>(encodedRecords);
            var demo = new Demo(content.randomSeed);

            foreach (var entry in content.entries)
            {
                demo.Append(new DemoSerializableEntry(entry.time, entry.BuildInputFrameState()));
            }

            return demo;
        }

        private int GetIndexAtTime(int earliestIndex, float latestTime)
        {
            for (var i = earliestIndex; i < this.records.Count; i++)
            {
                var record = this.records[i];
                if (record.time > latestTime)
                {
                    return i;
                }
            }

            return this.records.Count;
        }

        /// <summary>
        ///     This needs to be Sync because we might do some CleanRandom stuff while we're waiting for the demo to load
        /// </summary>
        /// <param name="demoName"></param>
        public static Demo FromDisk_Sync(string demoName)
        {
            var demoJson = FileHelpers.ReadTextAppDataThenLocal(Path.Join("Demos", demoName)).Result;
            return DecodeRecords(demoJson);
        }

        [Serializable]
        public class DemoContent
        {
            public readonly int randomSeed;
            public DemoSerializableEntry[] entries;

            public DemoContent(List<DemoSerializableEntry> entries, int randomSeed)
            {
                this.randomSeed = randomSeed;
                this.entries = entries.ToArray();
            }
        }

        [Serializable]
        public class DemoSerializableEntry
        {
            public int keyboardModifiersAsInt;
            public int mouseButtonsPressedAsInt;
            public int mouseButtonsReleasedAsInt;
            public float mouseDeltaX;
            public float mouseDeltaY;
            public int mouseX;
            public int mouseY;
            public Keys[] pressedKeys = Array.Empty<Keys>();
            public Keys[] releasedKeys = Array.Empty<Keys>();
            public int scrollDelta;
            public float time;

            public DemoSerializableEntry(float time, InputFrameState inputState)
            {
                this.time = time;

                this.mouseX = inputState.mouseFrameState.RawWindowPosition.X;
                this.mouseY = inputState.mouseFrameState.RawWindowPosition.Y;
                this.mouseButtonsPressedAsInt = inputState.mouseFrameState.ButtonsPressedThisFrame.EncodedInt;
                this.mouseButtonsReleasedAsInt = inputState.mouseFrameState.ButtonsReleasedThisFrame.EncodedInt;
                this.scrollDelta = inputState.mouseFrameState.ScrollDelta;
                this.mouseDeltaX = inputState.mouseFrameState.PositionDelta.X;
                this.mouseDeltaY = inputState.mouseFrameState.PositionDelta.Y;
                this.pressedKeys = inputState.keyboardFrameState.Pressed ?? Array.Empty<Keys>();
                this.releasedKeys = inputState.keyboardFrameState.Released ?? Array.Empty<Keys>();
                this.keyboardModifiersAsInt = inputState.keyboardFrameState.Modifiers.EncodedInt;
            }

            public DemoSerializableEntry()
            {
                // DO NOT DELETE!! Used by the Json Decoder
            }

            public InputFrameState BuildInputFrameState()
            {
                return new InputFrameState(
                    new KeyboardFrameState(
                        this.pressedKeys, this.releasedKeys, new ModifierKeys(this.keyboardModifiersAsInt)),
                    new MouseFrameState(
                        new MouseButtonList(this.mouseButtonsPressedAsInt),
                        new MouseButtonList(this.mouseButtonsReleasedAsInt),
                        new Point(this.mouseX, this.mouseY), new Vector2(this.mouseDeltaX, this.mouseDeltaY),
                        this.scrollDelta));
            }

            public override string ToString()
            {
                return this.time.ToString();
            }
        }

        public class Recorder
        {
            private readonly Demo demo;
            public readonly string fileName;
            private float totalTime;

            public Recorder(string fileName)
            {
                this.demo = new Demo(MachinaGame.Current.gameCartridge.Random.Seed);
                this.totalTime = 0f;
                this.fileName = fileName;
            }

            public void AddEntry(float dt, InputFrameState inputState)
            {
                this.totalTime += dt;
                this.demo.Append(new DemoSerializableEntry(this.totalTime, inputState));
            }

            public void WriteDemoToDisk()
            {
                Directory.CreateDirectory(Path.Join(MachinaGame.Current.Runtime.appDataPath, "Demos"));
                FileHelpers.WriteStringToAppData(this.demo.EncodeRecords(), Path.Join("Demos", this.fileName));
            }
        }

        public class Playback
        {
            private readonly Demo demo;
            private readonly int demoLength;
            public readonly int playbackSpeed;
            private int currentIndex;
            private float time;

            public Playback(Demo demo, int playbackSpeed)
            {
                this.demo = demo;
                this.time = 0f;
                this.currentIndex = 0;
                this.demoLength = this.demo.records.Count;
                this.playbackSpeed = playbackSpeed;
                MachinaGame.Current.gameCartridge.Random.Seed = demo.seedAtStart;
            }

            public bool IsFinished => this.currentIndex >= this.demoLength;

            public float Progress => this.currentIndex / (float) this.demoLength;

            public InputFrameState LatestFrameState { get; private set; }

            public InputFrameState UpdateAndGetInputFrameStates(float dt)
            {
                this.time += dt;
                this.currentIndex++;

                if (this.currentIndex < this.demo.records.Count)
                {
                    var record = this.demo.records[this.currentIndex];
                    var result = record.BuildInputFrameState();
                    LatestFrameState = result;
                    return result;
                }
                else
                {
                    // Unpress any pressed buttons (this doesn't work because it doesn't work that way, we need to discover pressed keys some other way)
                    var result =
                        new InputFrameState(
                            new KeyboardFrameState(Array.Empty<Keys>(), LatestFrameState.keyboardFrameState.Pressed,
                                ModifierKeys.NoModifiers)
                            ,
                            new MouseFrameState(
                                new MouseButtonList(),
                                new MouseButtonList(LatestFrameState.mouseFrameState.ButtonsPressedThisFrame
                                    .EncodedInt), LatestFrameState.mouseFrameState.RawWindowPosition,
                                Vector2.Zero, 0));
                    LatestFrameState = result;
                    return result;
                }
            }

            public void PollHumanInput(InputFrameState inputFrameState)
            {
                // InputFrameState is such an inconvenient structure :(
                foreach (var pressedButton in inputFrameState.keyboardFrameState.Pressed)
                {
                    if (pressedButton == Keys.Escape)
                    {
                        SkipToEnd();
                    }
                }
            }

            public void SkipToEnd()
            {
                this.currentIndex = this.demoLength;
            }
        }
    }
}