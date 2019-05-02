﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UMP.Wrappers;

namespace UMP
{
    public class MediaPlayerAndroid : IPlayer, IPlayerAudio, IPlayerSpu
    {
        private MonoBehaviour _monoObject;
        private WrapperAndroid _wrapper;

        private int _framesCounter;
        private float _frameRate;
        private float _tmpTime;
        private int _tmpFramesCounter;
        private int _tmpVolume;

        private bool _isStarted;
        private bool _isPlaying;
        private bool _isLoad;
        private bool _isReady;
        private bool _isImageReady;
        private bool _isMute;
        private bool _isTextureExist;

        private string _dataSource;
        private object _expandedPlayer;
        private PlayerBufferVideo _videoBuffer;
        private PlayerManagerEvents _eventManager;
        private PlayerOptionsAndroid _options;

        private Texture2D _videoTexture;
        private GameObject[] _videoOutputObjects;

        private IEnumerator _updateVideoTextureEnum;

        #region Constructors
        /// <summary>
        ///  Create instance of MediaPlayerAndroid object with additional arguments
        /// </summary>
        /// <param name="monoObject">MonoBehaviour instanse</param>
        /// <param name="videoOutputObjects">Objects that will be rendering video output</param>
        /// <param name="options">Additional player optons</param>
        public MediaPlayerAndroid(MonoBehaviour monoObject, GameObject[] videoOutputObjects, PlayerOptionsAndroid options)
        {
            _monoObject = monoObject;
            _videoOutputObjects = videoOutputObjects;
            _options = options;
            
            _wrapper = new WrapperAndroid(_options);

            if (_wrapper.NativeIndex < 0)
            {
                Debug.LogError("Don't support video playback on current platform or you use incorrect UMP libraries!");
                throw new Exception();
            }

            _eventManager = new PlayerManagerEvents(_monoObject, this);
            _eventManager.PlayerPlayingListener += OnPlayerPlaying;
            _eventManager.PlayerPausedListener += OnPlayerPaused;
        }
        #endregion

        #region Private methods
        private void UpdateFpsCounter()
        {
            float timeDelta = UnityEngine.Time.time;
            timeDelta = (timeDelta > _tmpTime) ? timeDelta - _tmpTime : 0;
            if (timeDelta >= 1f)
            {
                _frameRate = FramesCounter - _tmpFramesCounter;
                _tmpFramesCounter = FramesCounter;
                _tmpTime = UnityEngine.Time.time;
            }
        }

        private IEnumerator UpdateVideoTexture()
        {
            while (true)
            {
                if (FramesCounter != _framesCounter)
                {
                    _framesCounter = FramesCounter;
                    UpdateFpsCounter();

                    if (_videoBuffer == null)
                    {
                        int width = (int)_options.FixedVideoSize.x;
                        int height = (int)_options.FixedVideoSize.y;

                        if (_options.FixedVideoSize == Vector2.zero)
                        {
                            width = VideoWidth;
                            height = VideoHeight;
                        }

                        _videoBuffer = new PlayerBufferVideo(width, height);
                        _wrapper.NativeSetPixelsBuffer(_videoBuffer.FramePixelsAddr, _videoBuffer.Width, _videoBuffer.Height);
                    }

                    if (!_isTextureExist)
                    {
                        if (_videoTexture != null)
                        {
                            UnityEngine.Object.Destroy(_videoTexture);
                            _videoTexture = null;
                        }

                        _videoTexture = MediaPlayerHelper.GenPluginTexture(_videoBuffer.Width, _videoBuffer.Height);
                        MediaPlayerHelper.ApplyTextureToRenderingObjects(_videoTexture, _videoOutputObjects);
                        _wrapper.NativeSetTexture(_videoTexture.GetNativeTexturePtr());
                        _isTextureExist = true;
                        _isImageReady = false;
                    }

                    _wrapper.PlayerUpdateSurfaceTexture();
                    GL.IssuePluginEvent(_wrapper.NativeGetUnityRenderCallback(), _wrapper.NativeIndex);

                    if (!_isImageReady)
                    {
                        _eventManager.SetEvent(PlayerState.ImageReady, _videoTexture);
                        _isImageReady = true;
                    }
                }

                if (_wrapper.PlayerIsReady() && !_isReady)
                {
                    _isReady = true;

                    if (_isLoad)
                    {
                        _eventManager.ReplaceEvent(PlayerState.Paused, PlayerState.Prepared, new Vector2(VideoWidth, VideoHeight));
                        Pause();
                    }
                    else
                    {
                        _eventManager.SetEvent(PlayerState.Prepared, new Vector2(VideoWidth, VideoHeight));
                        _eventManager.SetEvent(PlayerState.Playing);
                    }
                }

                yield return null;
            }
        }

        private void OnPlayerPlaying()
        {
            _isPlaying = true;
        }

        private void OnPlayerPaused()
        {
            _isPlaying = false;
        }
        #endregion

        public GameObject[] VideoOutputObjects
        {
            set
            {
                _videoOutputObjects = value;
                MediaPlayerHelper.ApplyTextureToRenderingObjects(_videoTexture, _videoOutputObjects);
            }

            get { return _videoOutputObjects; }
        }

        public PlayerManagerEvents EventManager
        {
            get { return _eventManager; }
        }

        public PlayerOptions Options
        {
            get
            {
                return _options;
            }
        }

        public PlayerState State
        {
            get
            {
                return _wrapper.PlayerGetState();
            }
        }

        public object StateValue
        {
            get
            {
                return _wrapper.PlayerGetStateValue();
            }
        }

        public void AddMediaListener(IMediaListener listener)
        {
            if (_eventManager != null)
            {
                _eventManager.PlayerOpeningListener += listener.OnPlayerOpening;
                _eventManager.PlayerBufferingListener += listener.OnPlayerBuffering;
                _eventManager.PlayerImageReadyListener += listener.OnPlayerImageReady;
                _eventManager.PlayerPreparedListener += listener.OnPlayerPrepared;
                _eventManager.PlayerPlayingListener += listener.OnPlayerPlaying;
                _eventManager.PlayerPausedListener += listener.OnPlayerPaused;
                _eventManager.PlayerStoppedListener += listener.OnPlayerStopped;
                _eventManager.PlayerEndReachedListener += listener.OnPlayerEndReached;
                _eventManager.PlayerEncounteredErrorListener += listener.OnPlayerEncounteredError;
            }
        }

        public void RemoveMediaListener(IMediaListener listener)
        {
            if (_eventManager != null)
            {
                _eventManager.PlayerOpeningListener -= listener.OnPlayerOpening;
                _eventManager.PlayerBufferingListener -= listener.OnPlayerBuffering;
                _eventManager.PlayerImageReadyListener -= listener.OnPlayerImageReady;
                _eventManager.PlayerPreparedListener -= listener.OnPlayerPrepared;
                _eventManager.PlayerPlayingListener -= listener.OnPlayerPlaying;
                _eventManager.PlayerPausedListener -= listener.OnPlayerPaused;
                _eventManager.PlayerStoppedListener -= listener.OnPlayerStopped;
                _eventManager.PlayerEndReachedListener -= listener.OnPlayerEndReached;
                _eventManager.PlayerEncounteredErrorListener -= listener.OnPlayerEncounteredError;
            }
        }

        public void Prepare()
        {
            _isLoad = true;
            Play();
        }

        /// <summary>
        /// Play or resume (True if playback started (and was already started), or False on error.
        /// </summary>
        /// <returns></returns>
        public bool Play()
        {
            if (!_isStarted)
            {
                if (_eventManager != null)
                    _eventManager.StartListener();
            }

            if (_updateVideoTextureEnum == null)
            {
                _updateVideoTextureEnum = UpdateVideoTexture();
                _monoObject.StartCoroutine(_updateVideoTextureEnum);
            }

            _isStarted = _wrapper.PlayerPlay();

            if (_isStarted)
            {
                if (_isReady && !_isPlaying)
                    _eventManager.SetEvent(PlayerState.Playing);
            }
            else
            {
                Stop();
            }

            return _isStarted;
        }

        public void Pause()
        {
            if (_videoOutputObjects == null && _updateVideoTextureEnum != null)
            {
                _monoObject.StopCoroutine(_updateVideoTextureEnum);
                _updateVideoTextureEnum = null;
            }

            _wrapper.PlayerPause();
        }

        public void Stop(bool resetTexture)
        {
            if (_isStarted)
            {
                _wrapper.PlayerStop();

                if (_updateVideoTextureEnum != null)
                {
                    _monoObject.StopCoroutine(_updateVideoTextureEnum);
                    _updateVideoTextureEnum = null;
                }

                _framesCounter = 0;
                _frameRate = 0;
                _tmpFramesCounter = 0;
                _tmpTime = 0;

                _isStarted = false;
                _isPlaying = false;
                _isLoad = false;
                _isReady = false;
                _isImageReady = false;
                _isTextureExist = !resetTexture;

                if (resetTexture)
                {
                    if (_videoTexture != null)
                    {
                        UnityEngine.Object.Destroy(_videoTexture);
                        _videoTexture = null;
                    }
                }

                if (_videoBuffer != null)
                {
                    _videoBuffer.ClearFramePixels();
                    _videoBuffer = null;
                }

                if (_eventManager != null)
                    _eventManager.StopListener();
            }
        }

        public void Stop()
        {
            Stop(true);
        }

        public void Release()
        {
            Stop();

            if (_eventManager != null)
            {
                _eventManager.RemoveAllEvents();
                _eventManager = null;
            }

            _wrapper.PlayerRelease();
        }

        public string DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;

                _wrapper.PlayerSetDataSource(MediaPlayerHelper.GetDataSourcePath(_dataSource));
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
        }

        public bool IsReady
        {
            get { return _isReady; }
        }

        public bool AbleToPlay
        {
            get
            {
                return _dataSource != null && !string.IsNullOrEmpty(_dataSource.ToString());
            }
        }

        /// <summary>
        /// Get the current movie length (in ms).
        /// </summary>
        /// <returns></returns>
        public long Length
        {
            get
            {
                return _wrapper.PlayerGetLength();
            }
        }

        /// <summary>
        /// Get the current movie formatted length (hh:mm:ss[:ms]).
        /// </summary>
        /// <param name="detail">True: formatted length will be with [:ms]</param>
        /// <returns></returns>
        public string GetFormattedLength(bool detail)
        {
            var length = TimeSpan.FromMilliseconds(Length);

            var format = detail ? "{0:D2}:{1:D2}:{2:D2}:{3:D3}" : "{0:D2}:{1:D2}:{2:D2}";

            return string.Format(format,
                length.Hours,
                length.Minutes,
                length.Seconds,
                length.Milliseconds);
        }

        public float FrameRate
        {
            get { return _frameRate; }
        }

        public int FramesCounter
        {
            get
            {
                return _wrapper.PlayerVideoFramesCounter();
            }
        }

        public byte[] FramePixels
        {
            get
            {
                _wrapper.NativeUpdatePixelsBuffer();
                return _videoBuffer.FramePixels;
            }
        }

        public long Time
        {
            get
            {
                return _wrapper.PlayerGetTime();
            }
            set
            {
                _wrapper.PlayerSetTime(value);
            }
        }

        public float Position
        {
            get
            {
                return _wrapper.PlayerGetPosition();
            }
            set
            {
                _wrapper.PlayerSetPosition(value);
            }
        }

        public float PlaybackRate
        {
            get
            {
                return _wrapper.PlayerGetRate();
            }
            set
            {
                _wrapper.PlayerSetRate(value);
            }
        }

        public int Volume
        {
            get
            {
                if (_isMute)
                    return _tmpVolume;

                return _wrapper.PlayerGetVolume();
            }
            set
            {
                if (_isMute)
                {
                    _tmpVolume = value;
                    return;
                }

                _wrapper.PlayerSetVolume(value);
            }
        }

        public bool Mute
        {
            get { return _isMute; }
            set
            {
                if (value)
                {
                    _isMute = false;
                    _tmpVolume = Volume;
                    Volume = 0;
                    _isMute = true;
                }
                else
                {
                    _isMute = false;
                    if (_tmpVolume >= 0)
                        Volume = _tmpVolume;
                }
            }
        }

        public int VideoWidth
        {
            get
            {
                var width = _wrapper.PlayerVideoWidth();

                if (_videoBuffer != null && (width <= 0 || _options.FixedVideoSize != Vector2.zero))
                    width = _videoBuffer.Width;

                return width;
            }
        }

        public int VideoHeight
        {
            get
            {
                var height = _wrapper.PlayerVideoHeight();

                if (_videoBuffer != null && (height <= 0 || _options.FixedVideoSize != Vector2.zero))
                    height = _videoBuffer.Height;

                return height;
            }
        }

        public Vector2 VideoSize
        {
            get
            {
                return new Vector2(VideoWidth, VideoHeight);
            }
        }

        /// <summary>
        /// Get available spu tracks.
        /// </summary>
        public MediaTrackInfo[] SpuTracks
        {
            get
            {
                return _wrapper.PlayerSpuGetTracks();
            }
        }

        /// <summary>
        /// Get/Set current spu track.
        /// </summary>
        public MediaTrackInfo SpuTrack
        {
            get
            {
                int currentTrack = _wrapper.PlayerSpuGetTrack();
                return SpuTracks.Where(t => t.Id == currentTrack).SingleOrDefault();
            }
            set
            {
                _wrapper.PlayerSpuSetTrack(value.Id);
            }
        }

        /// <summary>
        /// Get available audio tracks.
        /// </summary>
        public MediaTrackInfo[] AudioTracks
        {
            get
            {
                return _wrapper.PlayerAudioGetTracks();
            }
        }

        /// <summary>
        /// Get/Set current audio track.
        /// </summary>
        public MediaTrackInfo AudioTrack
        {
            get
            {
                int currentTrack = _wrapper.PlayerAudioGetTrack();
                return AudioTracks.Where(t => t.Id == currentTrack).SingleOrDefault();
            }
            set
            {
                _wrapper.PlayerAudioSetTrack(value.Id);
            }
        }

        #region Platform dependent functionality
        public PlayerOptionsAndroid PlatformOptions
        {
            get
            {
                return _options;
            }
        }

        public object ExpandedPlayer
        {
            get
            {
                return _wrapper.PlayerExpanded;
            }
        }

        /// <summary>
        /// Set new video subtitle file
        /// </summary>
        /// <param name="path">Path to the new video subtitle file</param>
        /// <returns></returns>
        public bool SetSubtitleFile(Uri path)
        {
            return _wrapper.PlayerSetSubtitleFile(path);
        }
        #endregion
    }
}