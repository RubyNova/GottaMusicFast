using System;
using System.IO;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace AccelerometerMusicConverter
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            //Initialize
            //var device = Alc.OpenDevice(null);
            //var context = Alc.CreateContext(device, (int*)null);

            //Alc.MakeContextCurrent(context);
            //var buffer = new AccelerometerMusicBuffer();
            //var version = AL.Get(ALGetString.Version);
            //var vendor = AL.Get(ALGetString.Vendor);
            //var renderer = AL.Get(ALGetString.Renderer);
            //Console.WriteLine(version);
            //Console.WriteLine(vendor);
            //Console.WriteLine(renderer);
            var buffer = new AccelerometerMusicBuffer();
            Console.WriteLine("ready!");
            //Console.ReadKey();

            uint i = 1;

            Console.WriteLine("Enter IP: (enter nothing to skip)");
            string input = Console.ReadLine();
            if (input != String.Empty) Client.ConnectIP = input;

            Thread clientT = new Thread(new ThreadStart(Client.StartClient));
            clientT.Start();

            while (true)
            {
                if(Client.DataBuffer.Count > 0)
                {
                    buffer.PushNewInput(Client.DataBuffer.First());
                    Client.DataBuffer.RemoveAt(0);
                    Thread.Sleep(530);
                }
            }

            //while (true)
            //{
            //    buffer.PushNewInput(new SendData { One = new Accelerometer { XRaw = 440, YRaw = 10 }, Two = new Accelerometer { XRaw = 500, YRaw = 20 } });
            //    Thread.Sleep(530); //BUG: THIS FIXES IT??? WHY???
            //    buffer.PushNewInput(new SendData { One = new Accelerometer { XRaw = 500, YRaw = 10 }, Two = new Accelerometer { XRaw = 550, YRaw = 20 } });
            //    //Console.ReadLine();
            //    Thread.Sleep(530);
            //    buffer.PushNewInput(new SendData { One = new Accelerometer { XRaw = 550, YRaw = 10 }, Two = new Accelerometer { XRaw = 600, YRaw = 20 } });
            //    Thread.Sleep(530);
            //    //Console.ReadLine();
            //    //Console.ReadLine();
            //}
            //Process
            //int buffers;
            //AL.GenBuffers(10, out buffers);
            //AL.GenSource(out uint sourceOne);
            //AL.GenSource(out uint sourceTwo);
            //int frequency = 440;
            //int sampleRate = 44100;
            //int dataCount = 0;
            //int incrementor = 0;
            //do
            //{
            //    if (dataCount != 0)
            //    {
            //        ++incrementor;
            //    }
            //    dataCount = sampleRate / frequency;
            //    dataCount += incrementor;
            //} while (sampleRate % dataCount != 0);



            //WORKS

            //double amplitude = 0.25 * short.MaxValue;
            ////double dt = 2 * Math.PI / sampleRate;
            //short[] sinData = new short[sampleRate];
            //for (int i = 0; i < sinData.Length; i++)
            //{
            //    //sinData[i] = (short)(amplitude * Math.Sin((i + 1) * dt * sampleRate));
            //    sinData[i] = (short)(amplitude * Math.Sin((2 * Math.PI * (i + 1) * frequency) / sampleRate));
            //}
            //AL.BufferData(1, ALFormat.Mono16, sinData, sinData.Length, sampleRate);

            //short[] sinDataTwo = new short[sampleRate];
            //for (int i = 0; i < sinDataTwo.Length; i++)
            //{
            //    //sinData[i] = (short)(amplitude * Math.Sin((i + 1) * dt * sampleRate));
            //    sinDataTwo[i] = (short)(amplitude * Math.Sin((2 * Math.PI * (i + 1) * (frequency - 10) / sampleRate)));
            //}
            //AL.BufferData(2, ALFormat.Mono16, sinDataTwo, sinDataTwo.Length, sampleRate);

            //for (int i = 1; i <= buffers; i++)
            //{
            //    short[] ree = new short[sampleRate];
            //    for (int j = 0; j < ree.Length; j++)
            //    {
            //        //sinData[i] = (short)(amplitude * Math.Sin((i + 1) * dt * sampleRate));
            //        ree[j] = (short)(amplitude * Math.Sin((2 * Math.PI * (j + 1) * (frequency + (100 * i))) / sampleRate));
            //    }
            //    AL.BufferData(i, ALFormat.Mono16, ree, ree.Length, sampleRate);

            //    AL.SourceQueueBuffer((int)sourceOne, i);
            //    var meme = AL.GetError();
            //    if (meme != ALError.NoError)
            //    {
            //        throw new Exception(meme.ToString());
            //    }
            //}



            //var sinDataTwo = new double[sampleRate / 400];
            //for (int i = 0; i < sinDataTwo.Length; i++)
            //{
            //    sinDataTwo[i] = Math.Sin((i + 1) * dt * freq);
            //}
            //AL.BufferData(2, ALFormat.Mono16, sinDataTwo, sinDataTwo.Length, sampleRate);
            //for (int i = 0; i < dataCount; i++)
            //{
            //    AL.BufferData(source, ALFormat.Mono16, new IntPtr((short)(amp * short.MaxValue * Math.Sin(i * dt * freq))), dataCount, sampleFreq);
            //}


            //AL.SourceQueueBuffer((int)sourceOne, 1); //TODO: This doesn't work??? But if I stream them both into 3 it does, despite there being more than 1 audio source
            //AL.SourceQueueBuffer((int)sourceOne, 2);
            ////AL.SourceQueueBuffer((int)sourceTwo, 2);
            ////AL.SourceQueueBuffer(3, 2);
            //var bla = AL.GetError();
            //if (bla != ALError.NoError)
            //{
            //    throw new Exception(bla.ToString());
            //}
            ////AL.Source(sourceOne, ALSourceb.Looping, true);
            //AL.Source(sourceTwo, ALSourceb.Looping, true);

            //AL.SourcePlay(sourceOne);
            ////AL.SourcePlay(sourceTwo);
            //AL.SourceUnqueueBuffer((int)sourceOne);
            //AL.SourcePlay(sourceOne);
            //Console.ReadKey();
            //AL.SourcePlay(sourceTwo);
            //Console.ReadKey();
            //bool isSourceOne = true;
            //while (true)
            //{
            //    Console.ReadKey();

            //    if (isSourceOne)
            //    {
            //        //AL.SourceUnqueueBuffers((int)sourceOne, 1);
            //        //AL.SourceQueueBuffer((int)sourceOne, 2);
            //        AL.SourcePlay(sourceOne);
            //        AL.SourceStop(sourceTwo);
            //    }
            //    else
            //    {
            //        AL.SourcePlay(sourceTwo);
            //        AL.SourceStop(sourceOne);
            //        //AL.SourceUnqueueBuffers((int)sourceOne, 1);
            //        //AL.SourceQueueBuffer((int)sourceOne, 1);
            //    }
            //    isSourceOne = !isSourceOne;
            //    //AL.SourcePlay((int)sourceOne);
            //}


            //int multiplier = 4;


            ///Dispose
            //Console.ReadLine();
        }
    }
}