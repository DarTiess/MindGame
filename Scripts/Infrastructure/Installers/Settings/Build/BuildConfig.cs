using System;
using System.Collections.Generic;

namespace Infrastructure.Installers.Settings.Build
{
    [Serializable]
    public class BuildConfig
    {
        public List<string> State;
      
        public BuildConfig()
        {
            State = new List<string>();
        }
    }
}