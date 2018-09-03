//////////////////////////////////////////////////
//                                              //
//   See License.txt for Licensing information  //
//                                              //
//////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Helpers;

namespace Rotation
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public class CombatRoutine
    {
        public enum RotationState
        {
            Stopped = 0,
            Running = 1
        }

        public List<Spell> Spells;

        private Thread mainThread;
        private readonly ManualResetEvent pause = new ManualResetEvent(false);
        private int PulseFrequency = 100;
        private readonly Random random;
        public CombatRoutine()
        {
            random = new Random(DateTime.Now.Second);
        }
        public RotationState State { get; private set; } = RotationState.Stopped;
        
        private void MainThreadTick()
        {
            try
            {
                while (true)
                {
                    pause.WaitOne();
                    Pulse();
                    Thread.Sleep(PulseFrequency + random.Next(50));
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, Color.Red);
            }
        }

        public void Load()
        {
            Spells = new List<Spell>();

            PulseFrequency = 100;
            Log.Write("Using Pulse Frequency (ms) = " + PulseFrequency);
            
            mainThread = new Thread(MainThreadTick) {IsBackground = true};
            mainThread.Start();

            Initialize();
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public void LoadFromFile()
        {
            using (StreamReader sr = new StreamReader(Application.StartupPath + "\\rotation.txt")) 
            {
                string line;
                while ((line = sr.ReadLine()) != null) 
                {
                    string[] split = line.Split('|');
                    string r = split[0].Split(',')[0];
                    string g = split[0].Split(',')[1];
                    string b = split[0].Split(',')[2];
                    Color c = Color.FromArgb(1, int.Parse(r), int.Parse(g), int.Parse(b));
                    Keys key = ParseEnum<Keys>(split[1]);
                    string spellName = split[2];
                    string x = split[3].Split(',')[0];
                    string y = split[3].Split(',')[1];
                    POINT point = new POINT(int.Parse(x), int.Parse(y));
                    var loadedSpell = new Spell(c, key, point,spellName);
                    Spells.Add(loadedSpell);
                }
                sr.Close();
            }
        }
    

        internal void Dispose()
        {
            Log.Write("Stopping Pulse() timer...");
            Pause();
            Thread.Sleep(100); // Wait for it to close entirely so that all bitmap reading is done
        }
        
        public void Start()
        {
            try
            {
                if (State == RotationState.Stopped)
                {
                    Log.Write("Starting PixelBuddy...", Color.Green);
                    pause.Set();
                    State = RotationState.Running;
                }
            }
            catch (Exception ex)
            {
                Log.Write("Error Starting Combat Routine", Color.Red);
                Log.Write(ex.Message, Color.Red);
            }
        }

        public void Pause()
        {
            try
            {
                if (State == RotationState.Running)
                {
                    Log.Write("PixelBuddy has stopped.", Color.Red);
                    Stop();
                    pause.Reset();
                    State = RotationState.Stopped;
                }
            }
            catch (Exception ex)
            {
                Log.Write("Error Stopping PixelBuddy", Color.Red);
                Log.Write(ex.Message, Color.Red);
            }
        }
        
        public void Initialize()
        {
            
        }
        public void Stop()
        {
            
        }
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally {
                if (writer != null)
                    writer.Close();
            }
        }

        public void Save()
        {
            using (StreamWriter streamWriter = new StreamWriter(Application.StartupPath + "\\rotation.txt")) 
            {
                streamWriter.AutoFlush = true;
                foreach (var spell in Spells) 
                {
                    streamWriter.WriteLine(spell.Color.R + "," + spell.Color.G + "," + spell.Color.B + "|" + spell.Key + "|" + spell.Name + "|" + spell.p.X + "," + spell.p.Y);
                }
                streamWriter.Close();
            }
        }

        public void Pulse()
        {
            foreach (var spell in Spells)
            {
                if (spell.MustPress())
                {
                    spell.Press();
                }
            }
        }
    }
}