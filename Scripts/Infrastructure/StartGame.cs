using Infrastructure.EventsBus;
using Infrastructure.Level;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

//using GameAnalyticsSDK;   
//using Facebook.Unity;

namespace Infrastructure
{
    public class StartGame : MonoBehaviour
    {
        private IEventBus _eventBus;
        private LevelLoader _levelLoader;

        // public string TenjinKeyAPI = "FTNSVXSVBJYARJM4HPHM8X9PYIPDWO1V";

        [Inject]
        public void Construct(IEventBus eventBus, LevelLoader levelLoader)
        {
            _eventBus = eventBus;
            _levelLoader = levelLoader;
        }

        private void Start()
        {
            //tenjin
            /*
        BaseTenjin instance = Tenjin.getInstance(TenjinKeyAPI);
        instance.Connect();
        */
            //ga
            /*
        GameAnalytics.Initialize();
        */     
            //fb
            /*
        if (!FB.IsInitialized)
        {// Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else   
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }  
        */
            Debug.Log("Start Ga,e");
          
           DontDestroyOnLoad(this);
        }
        /*
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus){}
        else
        {
            BaseTenjin instance = Tenjin.getInstance(TenjinKeyAPI);
            instance.Connect();
        }
    }
       
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
     
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    */
    }
}
