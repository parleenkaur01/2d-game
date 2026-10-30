using UnityEngine;

public class ElevatorAudio : MonoBehaviour
{
    private AudioSource source;
    private AudioSource ambienceSource;
    private AudioClip buttonClip;
    private AudioClip motorClip;
    private AudioClip bellClip;
    private AudioClip doorClip;
    private AudioClip clockClickClip;
    private AudioClip failureClip;
    private AudioClip clueClip;
    private AudioClip tickingClip;
    private AudioClip leftFootstepClip;
    private AudioClip rightFootstepClip;
    private bool nextFootstepIsRight;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0.7f;
        ambienceSource = gameObject.AddComponent<AudioSource>();
        ambienceSource.playOnAwake = false;
        ambienceSource.spatialBlend = 0f;
        ambienceSource.loop = true;
        ambienceSource.volume = 0.16f;

        buttonClip = CreateTone("Elevator Button", 0.12f, 740f, 0.22f);
        motorClip = CreateRumble("Elevator Motor", 4.5f);
        bellClip = CreateBell("Floor Bell");
        doorClip = CreateTone("Sliding Doors", 1.25f, 125f, 0.18f);
        clockClickClip = CreateTone("Clock Click", 0.09f, 940f, 0.19f);
        failureClip = CreateErrorSound();
        clueClip = CreateTone("Inspect Note", 0.1f, 520f, 0.13f);
        tickingClip = CreateTone("Clock Tick", 0.035f, 1200f, 0.1f);
        leftFootstepClip = CreateFootstep("Left Footstep", 88f);
        rightFootstepClip = CreateFootstep("Right Footstep", 104f);
        ambienceSource.clip = CreateTone("Office Electrical Buzz", 2f, 120f, 0.09f);
    }

    public void PlayButton() => source.PlayOneShot(buttonClip);
    public void PlayMotor() => source.PlayOneShot(motorClip, 0.75f);
    public void PlayBell() => source.PlayOneShot(bellClip, 0.9f);
    public void PlayDoors() => source.PlayOneShot(doorClip, 0.65f);
    public void PlayClockClick() => source.PlayOneShot(clockClickClip);
    public void PlayPuzzleFailure() => source.PlayOneShot(failureClip);
    public void PlayPuzzleSuccess() => source.PlayOneShot(bellClip);
    public void PlayClue() => source.PlayOneShot(clueClip);
    public void PlayClockTick() => source.PlayOneShot(tickingClip, 0.4f);
    public void PlayFootstep()
    {
        source.PlayOneShot(nextFootstepIsRight ? rightFootstepClip : leftFootstepClip, 0.55f);
        nextFootstepIsRight = !nextFootstepIsRight;
    }
    public void StartOfficeAmbience() => ambienceSource.Play();
    public void StopOfficeAmbience() => ambienceSource.Stop();

    private AudioClip CreateErrorSound()
    {
        const int sampleRate = 44100;
        const float length = 0.3f;
        const float noteLength = 0.13f;
        int sampleCount = Mathf.CeilToInt(length * sampleRate);
        AudioClip clip = AudioClip.Create("Wrong Answer", sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float localTime = time < noteLength ? time : time - 0.16f;
            if (localTime < 0f || localTime > noteLength) continue;
            float frequency = time < noteLength ? 330f : 220f;
            float envelope = Mathf.Sin(Mathf.PI * localTime / noteLength);
            float phase = 2f * Mathf.PI * frequency * localTime;
            samples[i] = (Mathf.Sin(phase) + 0.2f * Mathf.Sin(phase * 2f)) * envelope * 0.22f;
        }
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateFootstep(string clipName, float baseFrequency)
    {
        const int sampleRate = 44100;
        const float length = 0.12f;
        int sampleCount = Mathf.CeilToInt(length * sampleRate);
        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float impact = Mathf.Exp(-32f * time);
            float body = Mathf.Sin(2f * Mathf.PI * baseFrequency * time);
            float sole = Mathf.Sin(2f * Mathf.PI * (baseFrequency * 2.4f) * time) * 0.35f;
            samples[i] = (body + sole) * impact * 0.24f;
        }

        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateTone(string clipName, float length, float frequency, float volume)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(length * sampleRate);
        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float fade = Mathf.Min(1f, (length - time) * 12f);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * time) * volume * fade;
        }

        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateRumble(string clipName, float length)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(length * sampleRate);
        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float low = Mathf.Sin(2f * Mathf.PI * 58f * time);
            float vibration = Mathf.Sin(2f * Mathf.PI * 117f * time) * 0.35f;
            float envelope = 0.7f + Mathf.Sin(time * 3f) * 0.1f;
            samples[i] = (low + vibration) * 0.08f * envelope;
        }

        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateBell(string clipName)
    {
        const int sampleRate = 44100;
        float length = 1.1f;
        int sampleCount = Mathf.CeilToInt(length * sampleRate);
        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float envelope = Mathf.Exp(-3.8f * time);
            samples[i] = (Mathf.Sin(2f * Mathf.PI * 660f * time) * 0.22f +
                Mathf.Sin(2f * Mathf.PI * 990f * time) * 0.12f) * envelope;
        }

        clip.SetData(samples, 0);
        return clip;
    }
}
