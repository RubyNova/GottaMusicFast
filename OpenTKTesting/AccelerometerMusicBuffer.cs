using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace OpenTKTesting
{
    public unsafe class AccelerometerMusicBuffer
    {
        private List<int> _leftHandData;
        private List<int> _rightHandData;
        private List<int> _buffers;
        private uint _sourceOne;
        private uint _sourceTwo;
        private int _sampleRate;

        public AccelerometerMusicBuffer()
        {
            var device = Alc.OpenDevice(null);
            var context = Alc.CreateContext(device, (int*)null);
            Alc.MakeContextCurrent(context);
            //AL.GenBuffers(10, out uint buffers);
            _buffers = new List<int>();
            AL.GenSource(out _sourceOne);
            AL.GenSource(out _sourceTwo);
            _sampleRate = 44100;
            _leftHandData = new List<int>();
            _rightHandData = new List<int>();
        }

        private int GetNextBuffer()
        {
            AL.GenBuffer(out uint result);
            _buffers.Add((int)result);
            return (int)result;
        }

        public void PushNewInput(SendData data)
        {
            var left = data.One;
            var right = data.Two;

            double leftVol = left.YRaw;
            while (leftVol > 0)
            {
                leftVol /= 10;
            }

            leftVol *= short.MaxValue;

            var leftFrequency = left.XRaw;

            double rightVol = right.YRaw;
            while (rightVol > 0)
            {
                rightVol /= 10;
            }

            rightVol *= short.MaxValue;

            var rightFrequency = right.XRaw;

            UpdateSource(leftVol, (int)leftFrequency, (int)_sourceOne);
            UpdateSource(rightVol, (int)rightFrequency, (int)_sourceTwo);

        }

        private void UpdateSource(double vol, int frequency, int source)
        {
            short[] wave = ResolveSineWave(vol, frequency);
            var buffer = GetNextBuffer();
            AL.BufferData(buffer, ALFormat.Mono16, wave, wave.Length, frequency);
            AL.SourceQueueBuffer(source, buffer);
            if (AL.GetSourceState(source) == ALSourceState.Stopped || AL.GetSourceState(source) == ALSourceState.Paused)
            {
                AL.SourcePlay(source);
            }
            AL.SourceUnqueueBuffer(source);
            AL.DeleteBuffer(_buffers[0]);
            _buffers.RemoveAt(0);
            var err = AL.GetError();
            if (err != ALError.NoError)
            {
                throw new OpenTK.Audio.AudioException($"OpenAL returned the following error in the native code: {err.ToString()}");
            }
            //AL.GetSource(_sourceOne, (ALGetSourcei)0x1016, out int result); gets the number of buffers that have been processed
        }

        private short[] ResolveSineWave(double vol, int frequency)
        {
            short[] sinData = new short[_sampleRate];
            for (int i = 0; i < sinData.Length; i++)
            {
                //sinData[i] = (short)(amplitude * Math.Sin((i + 1) * dt * sampleRate));
                sinData[i] = (short)(vol * Math.Sin((2 * Math.PI * (i + 1) * frequency) / _sampleRate));
            }
            return sinData;
        }
    }
}
