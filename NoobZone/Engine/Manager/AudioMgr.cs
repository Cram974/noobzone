using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Audio;

namespace NoobZone
{
    class AudioMgr
    {
        public static Audio.System m_system = null;

        public static void Init()
        {
            Factory.System_Create(ref m_system);

            
            m_system.init(32, INITFLAGS.NORMAL, (IntPtr)null);
           

            //channelCallback = new CHANNEL_CALLBACK();
            //OnEndSound += new EndMusicEventHandler(SoundManager_OnEndSound);
            //channelCallback = new CHANNEL_CALLBACK(OnEndSound);

        }

        public  static void SetListenerValues()
        {
        }

        public static void Quit()
        {
            m_system.release();
        }
    }

    class Son
    {
        Sound m_sound;
        string m_path;
        Channel m_channel;

        public Son(string filename)
        {
            m_path = filename;
            AudioMgr.m_system.createSound(m_path, MODE.DEFAULT, ref m_sound);
        }

        public void Play()
        {
            m_sound.setMode(MODE.DEFAULT);
            AudioMgr.m_system.playSound(CHANNELINDEX.FREE, m_sound, false, ref m_channel);
        }
        public void PlayLoop()
        {
            m_sound.setMode(MODE.LOOP_NORMAL);
            AudioMgr.m_system.playSound(CHANNELINDEX.FREE, m_sound, false, ref m_channel);
        }
    }
}
