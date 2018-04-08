using System;
using System.IO;
using OpenTK;
using OpenTK.Audio.OpenAL;


namespace OpenTKTesting
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            //Initialize
            var device = Alc.OpenDevice(null);
            var context = Alc.CreateContext(device, (int*)null);

            Alc.MakeContextCurrent(context);

            var version = AL.Get(ALGetString.Version);
            var vendor = AL.Get(ALGetString.Vendor);
            var renderer = AL.Get(ALGetString.Renderer);
            Console.WriteLine(version);
            Console.WriteLine(vendor);
            Console.WriteLine(renderer);
            Console.ReadKey();

            //Process
            int buffers, sources;
            AL.GenBuffers(2, out buffers);
            AL.GenSources(2, out sources);

            int sampleFreq = 44100;
            double dt = 2 * Math.PI / sampleFreq;
            double amp = 0.5;

            int freq = 440;
            var dataCount = sampleFreq / freq;

            var sinData = new short[dataCount];
            for (int i = 0; i < sinData.Length; ++i)
            {
                sinData[i] = (short)(amp * short.MaxValue * Math.Sin(i * dt * freq));
            }
            AL.BufferData(1, ALFormat.Mono16, sinData, sinData.Length, sampleFreq);
            var sinDataTwo = new short[sampleFreq / 400];
            for (int i = 0; i < sinData.Length; ++i)
            {
                sinDataTwo[i] = (short)(amp * short.MaxValue * Math.Sin(i * dt * freq));
            }
            AL.BufferData(2, ALFormat.Mono16, sinDataTwo, sinDataTwo.Length, sampleFreq);
            //for (int i = 0; i < dataCount; i++)
            //{
            //    AL.BufferData(source, ALFormat.Mono16, new IntPtr((short)(amp * short.MaxValue * Math.Sin(i * dt * freq))), dataCount, sampleFreq);
            //}

            //AL.BufferData(buffers, ALFormat.Mono16, new AccelerometerMusicBuffer[10], 10, 10);


            AL.SourceQueueBuffer(2, 1); //TODO: This doesn't work??? But if I stream them both into 3 it does, despite there being more than 1 audio source
            AL.SourceQueueBuffer(3, 2);
            var bla = AL.GetError();
            if (bla != ALError.NoError)
            {
                throw new Exception(bla.ToString());
            }
            AL.Source(sources, ALSourceb.Looping, true);



            AL.SourcePlay(sources);

            Console.ReadKey();

            int multiplier = 4;


            ///Dispose
            if (context != ContextHandle.Zero)
            {
                Alc.MakeContextCurrent(ContextHandle.Zero);

                Alc.DestroyContext(context);
            }
            context = ContextHandle.Zero;

            if (device != IntPtr.Zero)
            {
                Alc.CloseDevice(device);
            }
            device = IntPtr.Zero;
            //Console.ReadLine();
        }
    }
}