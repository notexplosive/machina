﻿using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class Demo
    {
        public readonly static string MostRecentlySavedDemoPath = "most_recent_demo.json";
        public readonly List<SerializableEntry> records = new List<SerializableEntry>();

        public void Append(SerializableEntry record)
        {
            records.Add(record);
        }

        public string EncodeRecords()
        {
            return JsonConvert.SerializeObject(new EntryList(this.records));
        }

        public static Demo DecodeRecords(string encodedRecords)
        {
            var demo = new Demo();
            var entryList = JsonConvert.DeserializeObject<EntryList>(encodedRecords);

            foreach (var entry in entryList.entries)
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
        public class EntryList
        {
            public SerializableEntry[] entries;
            public EntryList(List<SerializableEntry> entries)
            {
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
                this.demo = new Demo();
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
                FileHelpers.WriteStringToAppData(this.demo.EncodeRecords(), fileName);
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
                IsFinished = false;
            }

            public bool IsFinished
            {
                get;
                private set;
            }

            public InputFrameState[] UpdateAndGetInputFrameStates(float dt)
            {
                this.time += dt;
                var finalIndex = this.demo.GetIndexAtTime(this.currentIndex, this.time);
                var size = finalIndex - this.currentIndex;
                var result = new InputFrameState[size];

                if (this.currentIndex == finalIndex)
                {
                    MachinaGame.Print("Playback finished");
                    IsFinished = true;
                }

                for (int i = this.currentIndex; i < finalIndex; i++)
                {
                    var record = this.demo.records[i];
                    result[i - this.currentIndex] = record.BuildInputFrameState();
                }
                this.currentIndex = finalIndex;

                return result;
            }
        }

        public async static void FromDisk(string demoName, Action<Demo> onComplete)
        {
            var demoJson = await FileHelpers.ReadTextLocalThenAppData(demoName);
            var demo = DecodeRecords(demoJson);
            onComplete?.Invoke(demo);
        }
    }
}
