using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        声音管理工具
            挂载此脚本的对象可作为声音管理对象，负责播放音效、BGM
            声音分类：
                Bgm：持续播放，很少更改，单一播放
                音效：大量短音频资源，可能会重复播放、叠加播放
            因此：
                Bgm用一个组件控制
                音效用 资源管理+组件管理 控制
    */
    public interface IMusicManager
    {
        public float BgmVolume {get;set;}
        public float SoundVolume {get;set;}
        public ResourceManager<AudioClip> ClipManager {get;}

        // 播放Bgm：由于Bgm同一时间唯一，因此只提供播放方法即可
        public void PlayBgm(string name);
        public void StopBgm(bool isPause);
        // 播放音效：支持用回调控制目标音效所在的音频组件
        public void PlaySound(string name,Action<AudioSource> callback=null,GameObject target=null);
        public void StopSound(AudioSource sound);
    }



    public class MusicManager : BehaviourManager<AudioSource> , IMusicManager
    {
        // 资源目录
        string soundDir;                // Resources下音效所在目录
        string bgmDir;                  // Resources下bgm所在目录

        // 音量
        float bgmVolume;                // Bgm音量
        public float BgmVolume 
        {
            get {return bgmVolume;}
            set 
            {
                bgmVolume = value;
                fadeUtil.SetRange(0,bgmVolume);
            }
        }
        float soundVolume;              // 音效音量
        public float SoundVolume
        {
            get {return soundVolume;}
            set {soundVolume = value;}
        }

        AudioSource mBgm;               // 播放Bgm的组件
        AudioSource tempSource;
        FadeUtil fadeUtil;              // 实现淡入淡出效果的工具
        // 存储加载的声音资源 
        ResourceManager<AudioClip> clipManager;
        public ResourceManager<AudioClip> ClipManager
        {
            get {return clipManager;}
        }
        
                         
        
        
        public MusicManager(GameObject gameObject,string soundDir=null,string bgmDir = null) : base(gameObject)
        {
            soundDir = "Audio/Sound/";
            bgmDir = "Audio/BGM/";
            if(soundDir!=null) this.soundDir = soundDir;
            if(bgmDir!=null) this.bgmDir = bgmDir;
            
            bgmVolume = 1f;
            soundVolume = 1f;

            fadeUtil = new FadeUtil(0,bgmVolume,Time.deltaTime);
            clipManager = ResourceSystem.instance.musicManager;

            Debug.Log("MusicManager：构造完毕");
        }
        protected override void initRelativeSystems()
        {
            relativeSystems = new string[]{"ResourceSystem"};
        }


        // 需要在对象的Update中执行
        public void Update()
        {
            if(fadeUtil.State!=FadeSate.Stop)
            {
                // Debug.Log("当前状态："+fadeUtil.State);
                mBgm.volume = fadeUtil.Update();
            }
        }


        public void PlayBgm(string name)
        {
            if(mBgm==null)
            {
                    mBgm = userObject.AddComponent<AudioSource>();
                    mBgm.loop = true;
                    mBgm.volume = 0;
            }
            // 资源库中取得资源
            clipManager.Get(bgmDir+name,GetBgmCallBack);
        }
        // 获取Bgm资源回调处理
        public void GetBgmCallBack(AudioClip bgm)
        {
            if(bgm) Debug.Log("正在处理获取的Bgm："+bgm.name);
            if(!mBgm.isPlaying)     // 当前无播放的Bgm
            {
                fadeUtil.SetState(FadeSate.In,true,null);
                mBgm.clip = bgm;
                mBgm.Play();
            }
            else                    // 当前有正在播放的Bgm
            {
                fadeUtil.SetState(FadeSate.Out,false,()=>{
                    // 之前Bgm淡出后，渐入当前Bgm
                    mBgm.Stop();
                    mBgm.clip = bgm;
                    fadeUtil.SetState(FadeSate.In,true);
                    mBgm.Play();
                });
                
            }
            if(bgm) Debug.Log("MusicManager 当前Bgm："+mBgm.clip.name);
        }

        public void StopBgm(bool isPause)
        {
            if(isPause) mBgm.Pause();
            else mBgm.Stop();
        }


        public void PlaySound(string name, Action<AudioSource> callback = null,GameObject target=null)
        {
            // 获取组件
            RecycleAllAuto( audioS=>audioS.isPlaying );
            Active(out tempSource);
            // 获取资源
            clipManager.Get(soundDir+name,(clip)=>{
                tempSource.clip = clip;
                tempSource.loop = false;
                tempSource.volume = soundVolume;
                tempSource.Play();
                callback?.Invoke(tempSource);
            });
        }

        // 主要针对循环音效
        public void StopSound(AudioSource sound)
        {
            sound.Stop();
            Recycle(sound);
        }
        public void StopSoundAt(int index)
        {
            RecycleAt(index,(c)=>{c.Stop();});
        }

        public override void Clear()
        {
            base.Clear();
        }
    }


}
