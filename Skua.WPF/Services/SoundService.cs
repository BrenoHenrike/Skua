using Skua.Core.Interfaces;
using System;

namespace Skua.WPF.Services;

public class SoundService : ISoundService
{
    public void Beep()
    {
        Console.Beep();
    }

    public void Beep(int frequency, int duration)
    {
        if (frequency < 37)
            frequency = 37;
        else if (frequency > 32767)
            frequency = 32767;

        Console.Beep(frequency, duration);
    }
}
