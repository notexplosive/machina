﻿using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Machina.Engine
{
    public class Demo
    {
        public readonly static string MostRecentlySavedDemoPath = "saved.demo";
        public readonly List<SerializableEntry> records = new List<SerializableEntry>();
        private readonly int seedAtStart;

        public Demo(int randomSeed)
        {
            this.seedAtStart = randomSeed;
        }

        public void Append(SerializableEntry record)
        {
            records.Add(record);
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
                demo.Append(new SerializableEntry(entry.time, entry.BuildInputFrameState()));
            }
            return demo;
        }

        private int GetIndexAtTime(int earliestIndex, float latestTime)
        {
            for (int i = earliestIndex; i < records.Count; i++)
            {
                var record = records[i];
                if (record.time > latestTime)
                {
                    return i;
                }
            }

            return records.Count;
        }

        [Serializable]
        public class DemoContent
        {
            public SerializableEntry[] entries;
            public readonly int randomSeed;
            public DemoContent(List<SerializableEntry> entries, int randomSeed)
            {
                this.randomSeed = randomSeed;
                this.entries = entries.ToArray();
            }
        }

        [Serializable]
        public class SerializableEntry
        {
            public float time;
            public int mouseX;
            public int mouseY;
            public int mouseButtonsPressedAsInt;
            public int mouseButtonsReleasedAsInt;
            public int scrollDelta;
            public float mouseDeltaX;
            public float mouseDeltaY;
            public int keyboardModifiersAsInt;
            public Keys[] pressedKeys = Array.Empty<Keys>();
            public Keys[] releasedKeys = Array.Empty<Keys>();

            public SerializableEntry(float time, InputFrameState inputState)
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

            public SerializableEntry()
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
                        new Point(this.mouseX, this.mouseY), new Vector2(this.mouseDeltaX, this.mouseDeltaY), scrollDelta));
            }

            public override string ToString()
            {
                return this.time.ToString();
            }
        }

        public class Recorder
        {
            public readonly string fileName;
            private float totalTime;
            private readonly Demo demo;

            public Recorder(string fileName)
            {
                this.demo = new Demo(MachinaGame.Random.Seed);
                this.totalTime = 0f;
                this.fileName = fileName;
            }

            public void AddEntry(float dt, InputFrameState inputState)
            {
                this.totalTime += dt;
                this.demo.Append(new SerializableEntry(this.totalTime, inputState));
            }

            public void WriteDemoToDisk()
            {
                Directory.CreateDirectory(Path.Join(MachinaGame.Current.appDataPath, "Demos"));
                FileHelpers.WriteStringToAppData(this.demo.EncodeRecords(), Path.Join("Demos", fileName));
            }
        }

        public class Playback
        {
            private readonly Demo demo;
            private float time;
            private int currentIndex;

            public Playback(Demo demo)
            {
                this.demo = demo;
                this.time = 0f;
                this.currentIndex = 0;
                this.demoLength = this.demo.records.Count;
                MachinaGame.Random.Seed = demo.seedAtStart;
            }

            public bool IsFinished => this.currentIndex == demoLength;

            private readonly int demoLength;

            public float Progress => this.currentIndex / (float) demoLength;

            public InputFrameState LatestFrameState
            {
                get;
                private set;
            }

            public InputFrameState UpdateAndGetInputFrameStates(float dt)
            {
                this.time += dt;
                this.currentIndex++;

                if (this.currentIndex < this.demo.records.Count)
                {
                    var record = this.demo.records[this.currentIndex];
                    var result = record.BuildInputFrameState();
                    this.LatestFrameState = result;
                    return result;
                }
                else
                {
                    // Unpress any pressed buttons (this doesn't work because it doesn't work that way)
                    var result =
                        new InputFrameState(
                            new KeyboardFrameState(Array.Empty<Keys>(), LatestFrameState.keyboardFrameState.Pressed, ModifierKeys.NoModifiers)
                            ,
                        new MouseFrameState(
                            new MouseButtonList(),
                            new MouseButtonList(LatestFrameState.mouseFrameState.ButtonsPressedThisFrame.EncodedInt), LatestFrameState.mouseFrameState.RawWindowPosition,
                            Vector2.Zero, 0));
                    this.LatestFrameState = result;
                    return result;
                }
            }
        }

        /// <summary>
        /// This needs to be Sync because we might do some CleanRandom stuff while we're waiting for the demo to load
        /// </summary>
        /// <param name="demoName"></param>
        public static Demo FromDisk_Sync(string demoName)
        {
            var demoJson = FileHelpers.ReadTextAppDataThenLocal(Path.Join("Demos", demoName)).Result;
            return DecodeRecords(demoJson);
        }
    }
}
