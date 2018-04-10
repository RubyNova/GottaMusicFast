using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using OpenTK;
using System.Threading.Tasks;

namespace OpenTKTesting
{
    public unsafe class AccelerometerMusicBuffer : IDisposable
    {
        private List<int> _leftHandData;
        private List<int> _rightHandData;
        private int[] _buffers;
        private uint _sourceOne;
        private uint _sourceTwo;
        private int _sampleRate;
        private IntPtr _device;
        private ContextHandle _context;
        private bool _firstleft = true;
        private bool _firstRight = true;

        public AccelerometerMusicBuffer()
        {
            _device = Alc.OpenDevice(null);
            _context = Alc.CreateContext(_device, (int*)null);
            Alc.MakeContextCurrent(_context);
            //AL.GenBuffers(1, out uint buffers);
            _buffers = AL.GenBuffers(4);
            //_buffers = buffers;
            AL.GenSource(out _sourceOne);
            AL.GenSource(out _sourceTwo);

            _sampleRate = 44100;
            _leftHandData = new List<int>();
            _rightHandData = new List<int>();
        }

        private int GetNextBuffer(int source)
        {
            if (source == _sourceOne)
            {
                switch (_firstleft)
                {
                    case true:
                        _firstleft = false;
                        return _buffers[0];
                    case false:
                        _firstleft = true;
                        return _buffers[1];
                }
            }
            else
            {
                switch (_firstRight)
                {
                    case true:
                        return _buffers[2];
                    case false:
                        return _buffers[3];
                }
            }
            return -1;
        }

        public void PushNewInput(SendData data)
        {
            var left = data.One;
            var right = data.Two;

            double leftVol = left.YRaw;
            while (leftVol >= 1)
            {
                leftVol /= 10;
            }

            leftVol *= short.MaxValue;

            var leftFrequency = left.XRaw;

            double rightVol = right.YRaw;
            while (rightVol >= 1)
            {
                rightVol /= 10;
            }

            rightVol *= short.MaxValue;

            var rightFrequency = right.XRaw;

            UpdateSource(leftVol, (int)leftFrequency, (int)_sourceOne);
            UpdateSource(rightVol, (int)rightFrequency, (int)_sourceTwo); //BUG: Only one wave works

        }

        private void UpdateSource(double vol, int frequency, int source)
        {
            ALError err;
            if (frequency % 2 != 0)
            {
                ++frequency;
            }
            short[] wave = ResolveSineWave(vol, frequency);
            int removedBuffer = AL.SourceUnqueueBuffer(source);
            if (removedBuffer != 0)
            {
                UpdateBufferSwitch(removedBuffer, source);
                AL.BufferData(removedBuffer, ALFormat.Mono16, wave, wave.Length, _sampleRate);
                err = AL.GetError();
                //if (err != ALError.NoError && err != ALError.InvalidValue)
                //{
                //    await Task.Delay(200);
                //}
                AL.SourceQueueBuffer(source, removedBuffer);
                //err = AL.GetError();
                //if (err != ALError.NoError && err != ALError.InvalidValue)
                //{
                //    throw new OpenTK.Audio.AudioException($"OpenAL returned the following error in the native code: {err.ToString()}");
                //}
            }
            else
            {
                var buffer = GetNextBuffer(source);
                AL.BufferData(buffer, ALFormat.Mono16, wave, wave.Length, _sampleRate); //BUG: throws here, not sure why
                err = AL.GetError();
                //if (err == ALError.NoError)
                //{
                    AL.SourceQueueBuffer(source, buffer);
                //}
                Console.WriteLine("MEME");
                //err = AL.GetError();
                //if (err != ALError.NoError && err != ALError.InvalidValue)
                //{
                //    throw new OpenTK.Audio.AudioException($"OpenAL returned the following error in the native code: {err.ToString()}");
                //}
            }


            if (AL.GetSourceState(source) != ALSourceState.Playing)
            {
                AL.SourcePlay(source);
            }

            //err = AL.GetError();
            //if (err != ALError.NoError)
            //{
            //    throw new OpenTK.Audio.AudioException($"OpenAL returned the following error in the native code: {err.ToString()}");
            //}
        }

        private void UpdateBufferSwitch(int removedBuffer, int sourceId)
        {
            if (sourceId == _sourceOne)
            {
                if (removedBuffer == _buffers[0])
                {
                    _firstleft = false;
                }
                else if (removedBuffer == _buffers[1])
                {
                    _firstleft = true;
                }
            }
            else if (sourceId == _sourceTwo)
            {
                if (removedBuffer == _buffers[2])
                {
                    _firstleft = false;
                }
                else if (removedBuffer == _buffers[3])
                {
                    _firstleft = true;
                }
            }
        }


        //AL.GetSource(_sourceOne, (ALGetSourcei)0x1016, out int result); gets the number of buffers that have been processed
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

        public void Dispose()
        {
            if (_context != ContextHandle.Zero)
            {
                Alc.MakeContextCurrent(ContextHandle.Zero);

                Alc.DestroyContext(_context);
            }
            _context = ContextHandle.Zero;

            if (_device != IntPtr.Zero)
            {
                Alc.CloseDevice(_device);
            }
            _device = IntPtr.Zero;
        }
    }
}
