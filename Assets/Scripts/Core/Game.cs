using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AllUtils;
using Controller;
using Controller.Map;
using IngameDebugConsole;
using Mobile;
using Mobile.Purchase;
using ModelData;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UserData;
using Utils;
using View.HUD;

namespace Core
{
    public class Game : MonoBehaviour
    {
        public const int BASE_THRESHOLD = 10;
        public const int BASE_DPI = 96;
        
        private static Game instance;
        public static User User => Instance.user;
        
        public static Thread MainThread { get; private set; }
        private static Queue<Action> MainThreadActions = new Queue<Action>();
        
        private Vector2 lastScreenSize;
        private Camera mainCam;
        
        private User user;
        
        private HUD hud;
        private UnityIAP iap;
        private MobilePlatform mobile;
        private StaticData staticData;
        
        public static Game Instance => instance;
        
        public static event Action OnResize;
        public ReactiveProperty<bool> AppActive = new ReactiveProperty<bool>(true);

        public static bool IsAppActive => instance != null && instance.AppActive.Value;
        
        public static Camera MainCamera => Instance.mainCam == null ? Instance.mainCam = Camera.main : Instance.mainCam;
        public static HUD Hud => Instance.hud == null ? Instance.hud = FindObjectOfType<HUD>() : Instance.hud;
        public static UnityIAP Iap => Instance.iap;
        public static MobilePlatform Mobile => Instance.mobile;
        public static StaticData Static => Instance.staticData;

        private void Update()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            if (lastScreenSize != screenSize)
            {
                lastScreenSize = screenSize;
                TimerUtils.NextFrame().Then(() => OnResize?.Invoke());
            }
        }
        
        public static Game Create()
        {
            if (instance != null)
                return instance;
            
            MainThread = Thread.CurrentThread;

            var singleton = new GameObject();
            instance = singleton.AddComponent<Game>();
            singleton.name = "Game";

            DontDestroyOnLoad(singleton);

            instance.Init();

            return instance;
        }

        
        private EventSystem _eventSystem;
        public static EventSystem EventSystem
        {
            get
            {
                if (!Instance._eventSystem)
                    Instance._eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
                return Instance._eventSystem;
            }
        }

        private void Init()
        {
            EventSystem.pixelDragThreshold = (int)(BASE_THRESHOLD * Screen.dpi / BASE_DPI);
            
            staticData = new StaticData();
            ParseStatic();
            
            user = new User();
            user.Init();

            gameObject.AddComponent<DebugTool>();
            Hud.HudInit();
            
            iap = gameObject.AddComponent<UnityIAP>();
            iap.Initialize();

            mobile = new MobilePlatform();
            
            MapController.Load();
        }
        
        private void ParseStatic()
        {
            Static.LoadAndParse(StaticData.LOCALES_NAME);
            Static.LoadAndParse(StaticData.PRODUCT_DATA_NAME);
        }

        private void OnApplicationFocus(bool focus)
        {
            AppActive.Value = focus;
        }

        private void OnApplicationPause(bool pause)
        {
            AppActive.Value = !pause;
        }
        
        

        public static bool IsNativeQuit = false;

        public static void Quit()
        {
            Debug.Log("Application quit. IsNativeQuit: " + IsNativeQuit);
			
            if (!IsNativeQuit)
            {
                Application.Quit();
                return;
            }

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#elif UNITY_ANDROID
            AndroidJavaClass ajc = new AndroidJavaClass("com.lancekun.quit_helper.AN_QuitHelper");
            AndroidJavaObject UnityInstance = ajc.CallStatic<AndroidJavaObject>("Instance");
            UnityInstance.Call("AN_Exit");
#else
			Application.Quit();
#endif
        }

        void FixedUpdate()
        {
            lock (MainThreadActions)
            {
                while (MainThreadActions.Any())
                {
                    var act = MainThreadActions.Dequeue();
                    act.Invoke();
                }
            }
        }

        public static void ExecuteOnMainThread(Action action)
        {
            if (Thread.CurrentThread == MainThread)
                action.Invoke();
            else
            {
                lock (MainThreadActions)
                {
                    MainThreadActions.Enqueue(() => action());
                }
            }
        }
    }
}