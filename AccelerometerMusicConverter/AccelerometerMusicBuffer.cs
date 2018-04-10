using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using OpenTK;
using System.Threading.Tasks;

namespace AccelerometerMusicConverter
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
        private uint _sourceThree;
        private uint _sourceFour;
        private bool _firstleftOffset = true;
        private bool _firstRightOffset = true;
        private uint _sourceFive;

        public AccelerometerMusicBuffer()
        {

            _sampleRate = 44100;
            _device = Alc.OpenDevice(null);
            _context = Alc.CreateContext(_device, (int*)null);
            Alc.MakeContextCurrent(_context);
            //AL.GenBuffers(1, out uint buffers);
            _buffers = AL.GenBuffers(8);
            AL.GenBuffer(out uint bassABuffer);
            //_buffers = buffers;
            AL.GenSource(out _sourceOne);
            AL.GenSource(out _sourceTwo);
            AL.GenSource(out _sourceThree);
            AL.GenSource(out _sourceFour);
            AL.GenSource(out _sourceFive);
            var bassWave = ResolveSineWave(0.25 * short.MaxValue, 110);
            AL.BufferData((int)bassABuffer, ALFormat.Mono16, bassWave , bassWave.Length, _sampleRate);
            AL.Source(_sourceFive, ALSourceb.Looping, true);

            AL.SourceQueueBuffer((int)_sourceFive, (int)bassABuffer);
            AL.SourcePlay(_sourceFive);
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
            else if(source == _sourceTwo)
            {
                switch (_firstleftOffset)
                {
                    case true:
                        return _buffers[2];
                    case false:
                        return _buffers[3];
                }
            }
            else if (source == _sourceThree)
            {
                switch (_firstRight)
                {
                    case true:
                        return _buffers[4];
                    case false:
                        return _buffers[5];
                }
            }
            else
            {
                switch (_firstRightOffset)
                {
                    case true:
                        return _buffers[6];
                    case false:
                        return _buffers[7];
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

            var leftFrequencyBase = (uint)Math.Floor(left.XRaw / 10m);
            var leftFrequency = leftFrequencyBase;
            var leftFrequencyOffset = leftFrequencyBase * 1.5;
            double rightVol = right.YRaw;
            while (rightVol >= 1)
            {
                rightVol /= 10;
            }

            rightVol *= short.MaxValue;

            var rightFrequencyBase = (uint)Math.Floor(left.XRaw / 10m);
            var rightFrequency = rightFrequencyBase;
            var rightFrequencyOffset = rightFrequencyBase * 1.5;

            UpdateSource(leftVol, (int)leftFrequency, (int)_sourceOne);
            UpdateSource(leftVol, (int)leftFrequencyOffset, (int)_sourceTwo);
            UpdateSource(rightVol, (int)rightFrequency, (int)_sourceThree);
            UpdateSource(rightVol, (int)rightFrequencyOffset, (int)_sourceFour);
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
                    _firstleftOffset = false;
                }
                else if (removedBuffer == _buffers[3])
                {
                    _firstleftOffset = true;
                }
            }
            else if (sourceId == _sourceThree)
            {
                if (removedBuffer == _buffers[4])
                {
                    _firstRight = false;
                }
                else if (removedBuffer == _buffers[5])
                {
                    _firstRight = true;
                }
            }
            else if (sourceId == _sourceFour)
            {
                if (removedBuffer == _buffers[6])
                {
                    _firstRightOffset = false;
                }
                else if (removedBuffer == _buffers[7])
                {
                    _firstRightOffset = true;
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
