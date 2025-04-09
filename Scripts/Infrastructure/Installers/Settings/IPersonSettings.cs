using UnityEngine;

namespace Infrastructure.Installers.Settings
{
    public interface IPersonSettings
    {
        Color Color { get; }
        string Letter { get; }
        string CurrentPlayer { get; }
        int Coins { get; set; }
        int Speed { get; }
        int MaxTimeToAnswer { get; }
    }
}