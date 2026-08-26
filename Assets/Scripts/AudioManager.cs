using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is
    public static AudioManager Instance;
    public AudioSource Audiosource;

    public AudioSource BgAudio;

    public AudioSource LoopAudioSource;
    //audio clips

    public AudioClip moveloop;
    public AudioClip attackslash1;
    public AudioClip attackslash2;
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip TakeHit;
    public AudioClip Enemydeath;
    public AudioClip EnemyOnfire;
    public AudioClip BuffoffCooldown;
    public AudioClip invicible;
    public AudioClip Rage;
    public AudioClip Specialattack;
    public AudioClip EnemyHurt1;
    public AudioClip EnemyHurt2;
    public AudioClip EnemyHurt3;
    public AudioClip Crit;
    public AudioClip BuffActivate;
    public AudioClip portal;


    public float cooldown = 0.1f;
    public float lasthurtEnemysfx;
    public float lastCritsfx;
    public float playerhitfx;

    public AudioClip FinalphaseBGM;

   
    void Awake()
    {
        Instance = this;// insted of gettig the comp in every script we can make it a instance and it will be public for everyone
    }

    // Update is called once per frame
   
    public void PlaySFX(AudioClip clip)
    {
       
        Audiosource.PlayOneShot(clip);
    }

    public void PlayLoop(AudioClip clip)
    {
        if(LoopAudioSource.clip == clip && LoopAudioSource.isPlaying)
        {
            return;
        }
        LoopAudioSource.clip = clip;
        LoopAudioSource.loop = true;
        LoopAudioSource.Play();


    }

    public void StopLoop()
    {
        LoopAudioSource.Stop();
        LoopAudioSource.loop = false;
    }

    public void PlayDeathSFX()
    {
        Audiosource.PlayOneShot(Enemydeath, 0.4f); 
    }

    public void BgDuck()
    {
        BgAudio.volume = 0.3f;
    }
    public void BgNormal()
    {
        BgAudio.volume = 0.7f;
    }

    public void ChangeBg()
    {
        BgAudio.clip = FinalphaseBGM;
        BgAudio.Play();

    }

    public void PlayEnemyHurt(AudioClip clip)
    {
        if(Time.time - lasthurtEnemysfx > cooldown)
        {
            Audiosource.PlayOneShot(clip);
            lasthurtEnemysfx = Time.time;
        }
    }


    public void PlayCritSfx(AudioClip clip)
    {
        if (Time.time - lastCritsfx > cooldown)
        {
            Audiosource.PlayOneShot(clip);
            lastCritsfx = Time.time;
        }
    }


    public void PlayPlayerHurtSfx(AudioClip clip)
    {
        if (Time.time - playerhitfx > cooldown)
        {
            Audiosource.PlayOneShot(clip);
            playerhitfx = Time.time;
        }
    }

}
