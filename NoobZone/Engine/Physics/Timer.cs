using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.Sdl;

namespace NoobZone
{
    class Timer
    {
        int m_starttime;
        int m_stopTime;
        bool m_isstopped;

        public Timer()
        {
        }

        public void Start()
        {
            m_starttime = Sdl.SDL_GetTicks();
            m_isstopped = false;
        }
        public void Stop()
        {
            m_stopTime = Sdl.SDL_GetTicks();
            m_isstopped = true;
        }
        public float Elapsed
        {
            get 
            {
                if (m_isstopped)
                    return (m_stopTime - m_starttime) / 1000f;
                else
                    return (Sdl.SDL_GetTicks() - m_starttime) / 1000f;
            }
        }
    }
}
