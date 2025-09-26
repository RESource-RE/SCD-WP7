// Decompiled with JetBrains decompiler
// Type: Retro_Engine.AudioPlayback
// Assembly: Sonic CD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D35AF46A-1892-4F52-B201-E664C9200079
// Assembly location: C:\Users\koishi\Documents\REProjects\SCD-WP7-REDO\Data\Sonic CD.dll

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Sonic_CD;
using System;
#if !WINDOWS_PHONE
using System.Windows.Media;
#endif

namespace Retro_Engine
{

    public static class AudioPlayback
    {
#if !WINDOWS_PHONE
      private static System.Windows.Media.MediaPlayer wpfMediaPlayer;
      private static string currentWpfTrack;
#endif

      public const int MUSIC_STOPPED = 0;
      public const int MUSIC_PLAYING = 1;
      public const int MUSIC_PAUSED = 2;
      public const int MUSIC_LOADING = 3;
      public const int MUSIC_READY = 4;
      public const int NUM_CHANNELS = 8;
      public static Game gameRef;
      public static int numGlobalSFX = 0;
      public static int numStageSFX = 0;
      public static SoundEffect[] sfxSamples = new SoundEffect[256 /*0x0100*/];
      public static bool[] sfxLoaded = new bool[256 /*0x0100*/];
      public static SoundEffectInstance[] channelInstance = new SoundEffectInstance[8];
      public static int[] channelSfxNum = new int[8];
      public static int nextChannelPos;
      public static bool musicEnabled = true;
      public static int musicVolume = 100;
      public static float musicVolumeSetting = 1f;
      public static float sfxVolumeSetting = 1f;
      public static int musicStatus = 0;
      public static int currentMusicTrack;
      public static MusicTrackInfo[] musicTracks = new MusicTrackInfo[16 /*0x10*/];

      static AudioPlayback()
      {
        for (int index = 0; index < AudioPlayback.musicTracks.Length; ++index)
          AudioPlayback.musicTracks[index] = new MusicTrackInfo();
#if !WINDOWS_PHONE
        wpfMediaPlayer = new System.Windows.Media.MediaPlayer();
        wpfMediaPlayer.MediaEnded += (s, e) => {
          if (AudioPlayback.musicTracks[AudioPlayback.currentMusicTrack].loop)
          {
            wpfMediaPlayer.Position = TimeSpan.Zero;
            wpfMediaPlayer.Play();
          }
        };
#endif
      }

      private static void HeadphonesPauseMusicCheck(object sender, EventArgs eventArgs)
      {
        if (Microsoft.Xna.Framework.Media.MediaPlayer.State != MediaState.Paused || AudioPlayback.musicStatus != 1 || !AudioPlayback.musicTracks[AudioPlayback.currentMusicTrack].loop || StageSystem.stageMode == (byte) 2 || GlobalAppDefinitions.gameMode == (byte) 7)
          return;
        Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
      }

      public static void InitAudioPlayback()
      {
        FileData fData = new FileData();
        char[] fileName = new char[32 /*0x20*/];
        Microsoft.Xna.Framework.Media.MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(AudioPlayback.HeadphonesPauseMusicCheck);
        for (int index = 0; index < 8; ++index)
          AudioPlayback.channelSfxNum[index] = -1;
        if (!FileIO.LoadFile("Data/Game/GameConfig.bin".ToCharArray(), fData))
          return;
        byte num1 = FileIO.ReadByte();
        byte num2;
        for (int index = 0; index < (int) num1; ++index)
          num2 = FileIO.ReadByte();
        byte num3 = FileIO.ReadByte();
        for (int index = 0; index < (int) num3; ++index)
          num2 = FileIO.ReadByte();
        byte num4 = FileIO.ReadByte();
        for (int index = 0; index < (int) num4; ++index)
          num2 = FileIO.ReadByte();
        byte num5 = FileIO.ReadByte();
        for (int index1 = 0; index1 < (int) num5; ++index1)
        {
          byte num6 = FileIO.ReadByte();
          for (int index2 = 0; index2 < (int) num6; ++index2)
            num2 = FileIO.ReadByte();
        }
        for (int index3 = 0; index3 < (int) num5; ++index3)
        {
          byte num7 = FileIO.ReadByte();
          for (int index4 = 0; index4 < (int) num7; ++index4)
            num2 = FileIO.ReadByte();
        }
        byte num8 = FileIO.ReadByte();
        for (int index5 = 0; index5 < (int) num8; ++index5)
        {
          byte num9 = FileIO.ReadByte();
          int index6;
          for (index6 = 0; index6 < (int) num9; ++index6)
          {
            byte num10 = FileIO.ReadByte();
            fileName[index6] = (char) num10;
          }
          fileName[index6] = char.MinValue;
          num2 = FileIO.ReadByte();
          num2 = FileIO.ReadByte();
          num2 = FileIO.ReadByte();
          num2 = FileIO.ReadByte();
        }
        byte num11 = FileIO.ReadByte();
        AudioPlayback.numGlobalSFX = (int) num11;
        for (int sfxNum = 0; sfxNum < (int) num11; ++sfxNum)
        {
          byte num12 = FileIO.ReadByte();
          int index;
          for (index = 0; index < (int) num12; ++index)
          {
            byte num13 = FileIO.ReadByte();
            fileName[index] = (char) num13;
          }
          fileName[index] = char.MinValue;
          FileIO.GetFileInfo(fData);
          FileIO.CloseFile();
          AudioPlayback.LoadSfx(fileName, sfxNum);
          FileIO.SetFileInfo(fData);
        }
      }

      public static void ReleaseAudioPlayback()
      {
      }

      public static void ReleaseGlobalSFX()
      {
      }

      public static void ReleaseStageSFX()
      {
      }

      public static void SetGameVolumes(int bgmVolume, int sfxVolume)
      {
        AudioPlayback.musicVolumeSetting = (float) bgmVolume * 0.01f;
        AudioPlayback.SetMusicVolume(AudioPlayback.musicVolume);
        AudioPlayback.sfxVolumeSetting = (float) sfxVolume * 0.01f;
      }

      public static void StopAllSFX()
      {
        for (int index = 0; index < 8; ++index)
        {
          if (AudioPlayback.channelSfxNum[index] > -1 && !AudioPlayback.channelInstance[index].IsDisposed)
            AudioPlayback.channelInstance[index].Stop();
        }
      }

      public static void PauseSound()
      {
#if WINDOWS_PHONE
        if (!Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
          return;
        Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
        AudioPlayback.musicStatus = 2;
#else
        if (wpfMediaPlayer != null && currentWpfTrack != null)
        {
          wpfMediaPlayer.Pause();
          AudioPlayback.musicStatus = 2;
        }
#endif
      }

      public static void ResumeSound()
      {
#if WINDOWS_PHONE
        if (!Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
          return;
        Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
        AudioPlayback.musicStatus = 1;
#else
        if (wpfMediaPlayer != null && currentWpfTrack != null)
        {
          wpfMediaPlayer.Play();
          AudioPlayback.musicStatus = 1;
        }
#endif
      }

      public static void SetMusicTrack(char[] fileName, int trackNo, byte loopTrack, uint loopPoint)
      {
        char[] charArray = "Music/".ToCharArray();
        int num = FileIO.StringLength(ref fileName);
        for (int index = 0; index < num; ++index)
        {
          if (fileName[index] == '/')
            fileName[index] = '-';
        }
        int index1 = num - 4;
        if (index1 < 0)
          index1 = 0;
        if (fileName.Length > 0)
          fileName[index1] = char.MinValue;
        FileIO.StrCopy(ref AudioPlayback.musicTracks[trackNo].trackName, ref charArray);
        FileIO.StrAdd(ref AudioPlayback.musicTracks[trackNo].trackName, ref fileName);
        AudioPlayback.musicTracks[trackNo].loop = loopTrack == (byte) 1;
        AudioPlayback.musicTracks[trackNo].loopPoint = loopPoint;
      }

      public static void SetMusicVolume(int volume)
      {
        if (volume < 0)
          volume = 0;
        if (volume > 100)
          volume = 100;
        AudioPlayback.musicVolume = volume;
#if WINDOWS_PHONE
        if (!Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
          return;
        Microsoft.Xna.Framework.Media.MediaPlayer.Volume = (float) volume * 0.01f * AudioPlayback.musicVolumeSetting;
#else
        if (wpfMediaPlayer != null && currentWpfTrack != null)
        {
          wpfMediaPlayer.Volume = AudioPlayback.musicVolumeSetting;
        }
#endif
      }

      public static void PlayMusic(int trackNo)
      {
        if (Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl && AudioPlayback.musicTracks[trackNo].trackName[0] != '\0')
        {
          string text = new string(AudioPlayback.musicTracks[trackNo].trackName);
          text = text.Remove(FileIO.StringLength(ref AudioPlayback.musicTracks[trackNo].trackName));
          try
          {
#if WINDOWS_PHONE
            Song song = AudioPlayback.gameRef.Content.Load<Song>(text);
            AudioPlayback.currentMusicTrack = trackNo;
            Microsoft.Xna.Framework.Media.MediaPlayer.Play(song);
            Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = AudioPlayback.musicTracks[trackNo].loop;
            Microsoft.Xna.Framework.Media.MediaPlayer.IsMuted = false;
            Microsoft.Xna.Framework.Media.MediaPlayer.Volume = AudioPlayback.musicVolumeSetting;
            AudioPlayback.musicVolume = 100;
            AudioPlayback.musicStatus = 1;
#else
            string musicPath;
            if (text.StartsWith("Music/"))
            {
              musicPath = System.IO.Path.Combine("Content", text);
            }
            else
            {
              musicPath = System.IO.Path.Combine("Content", "Music", text);
            }
            string foundFile = musicPath + ".wma";
            if (foundFile != null)
            {
              string fullPath = System.IO.Path.GetFullPath(foundFile);
              wpfMediaPlayer.Open(new Uri(fullPath));
              wpfMediaPlayer.Volume = AudioPlayback.musicVolumeSetting;
              wpfMediaPlayer.Play();
              
              currentWpfTrack = text;
              AudioPlayback.currentMusicTrack = trackNo;
              AudioPlayback.musicVolume = 100;
              AudioPlayback.musicStatus = 1;
            }
            else
            {
              AudioPlayback.musicStatus = 0;
            }
#endif
          }
          catch
          {
            AudioPlayback.musicStatus = 0;
          }
        }
      }

      public static void StopMusic()
      {
#if WINDOWS_PHONE
        if (Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
        {
          Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
          Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = false;
          Microsoft.Xna.Framework.Media.MediaPlayer.IsMuted = true;
          AudioPlayback.musicStatus = 0;
        }
#else
        if (wpfMediaPlayer != null && currentWpfTrack != null)
        {
          wpfMediaPlayer.Stop();
          currentWpfTrack = null;
        }
        AudioPlayback.musicStatus = 0;
#endif
      }

      public static void LoadSfx(char[] fileName, int sfxNum)
      {
        FileData fData = new FileData();
        char[] strA = new char[64 /*0x40*/];
        char[] charArray = "Data/SoundFX/".ToCharArray();
        if (sfxNum <= -1 || sfxNum >= 256 /*0x0100*/)
          return;
        FileIO.StrCopy(ref strA, ref charArray);
        FileIO.StrAdd(ref strA, ref fileName);
        if (!FileIO.LoadFile(strA, fData))
          return;
        byte num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        num1 = FileIO.ReadByte();
        uint num2 = 1;
        for (uint index = 0; num2 > 0U && index < 400U; ++index)
        {
          switch (num2)
          {
            case 1:
              if (FileIO.ReadByte() == (byte) 100)
              {
                ++num2;
                break;
              }
              num2 = 1U;
              break;
            case 2:
              if (FileIO.ReadByte() == (byte) 97)
              {
                ++num2;
                break;
              }
              num2 = 1U;
              break;
            case 3:
              if (FileIO.ReadByte() == (byte) 116)
              {
                ++num2;
                break;
              }
              num2 = 1U;
              break;
            case 4:
              num2 = FileIO.ReadByte() != (byte) 97 ? 1U : 0U;
              break;
          }
        }
        uint num3 = (uint) FileIO.ReadByte() + ((uint) FileIO.ReadByte() << 8) + ((uint) FileIO.ReadByte() << 16 /*0x10*/) + ((uint) FileIO.ReadByte() << 24) << 1;
        byte[] buffer = new byte[(int) (num3 + (num3 / 32U /*0x20*/ << 1))];
        if (!BitConverter.IsLittleEndian)
        {
          for (uint index = 0; index < num3; index += 2U)
          {
            buffer[(int) index] = (byte) ((uint) FileIO.ReadByte() - 128U /*0x80*/);
            buffer[(int) (index + 1U)] = (byte) 0;
          }
        }
        else
        {
          for (uint index = 0; index < num3; index += 2U)
          {
            buffer[(int) index] = (byte) 0;
            buffer[(int) (index + 1U)] = (byte) ((uint) FileIO.ReadByte() - 128U /*0x80*/);
          }
        }
        if (AudioPlayback.sfxLoaded[sfxNum])
          AudioPlayback.sfxSamples[sfxNum].Dispose();
        AudioPlayback.sfxSamples[sfxNum] = new SoundEffect(buffer, 44100, AudioChannels.Mono);
        AudioPlayback.sfxLoaded[sfxNum] = true;
        FileIO.CloseFile();
      }

      public static void PlaySfx(int sfxNum, byte sLoop)
      {
        for (int index = 0; index < 8; ++index)
        {
          if (AudioPlayback.channelSfxNum[index] == sfxNum)
          {
            if (!AudioPlayback.channelInstance[index].IsDisposed)
              AudioPlayback.channelInstance[index].Stop();
            AudioPlayback.nextChannelPos = index;
            index = 8;
          }
        }
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos] = AudioPlayback.sfxSamples[sfxNum].CreateInstance();
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos].IsLooped = sLoop == (byte) 1;
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos].Pan = 0.0f;
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos].Volume = AudioPlayback.sfxVolumeSetting;
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos].Play();
        AudioPlayback.channelSfxNum[AudioPlayback.nextChannelPos] = sfxNum;
        ++AudioPlayback.nextChannelPos;
        if (AudioPlayback.nextChannelPos != 8)
          return;
        AudioPlayback.nextChannelPos = 0;
      }

      public static void StopSfx(int sfxNum)
      {
        for (int index = 0; index < 8; ++index)
        {
          if (AudioPlayback.channelSfxNum[index] == sfxNum)
          {
            AudioPlayback.channelSfxNum[index] = -1;
            if (!AudioPlayback.channelInstance[index].IsDisposed)
              AudioPlayback.channelInstance[index].Stop();
          }
        }
      }

      public static void SetSfxAttributes(int sfxNum, int volume, int pan)
      {
        for (int index = 0; index < 8; ++index)
        {
          if (AudioPlayback.channelSfxNum[index] == sfxNum)
          {
            if (!AudioPlayback.channelInstance[index].IsDisposed)
              AudioPlayback.channelInstance[index].Stop();
            AudioPlayback.nextChannelPos = index;
            index = 8;
          }
        }
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos] = AudioPlayback.sfxSamples[sfxNum].CreateInstance();
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos].IsLooped = false;
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos].Pan = (float) pan * 0.01f;
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos].Volume = AudioPlayback.sfxVolumeSetting;
        AudioPlayback.channelInstance[AudioPlayback.nextChannelPos].Play();
        AudioPlayback.channelSfxNum[AudioPlayback.nextChannelPos] = sfxNum;
        ++AudioPlayback.nextChannelPos;
        if (AudioPlayback.nextChannelPos != 8)
          return;
        AudioPlayback.nextChannelPos = 0;
      }

#if !WINDOWS_PHONE
      public static void Dispose()
      {
        if (wpfMediaPlayer != null)
        {
          wpfMediaPlayer.Close();
          wpfMediaPlayer = null;
        }
      }
#endif
    }
}