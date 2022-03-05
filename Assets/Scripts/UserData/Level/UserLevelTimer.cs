using System;
using AllUtils;
using Core;
using UniRx;
using UserData.Rating;
using UserData.TileMap;

namespace UserData.Level
{
    public class UserLevelTimer : IDisposable
    {
        private readonly UserMapClassic userMap;
        
        private IDisposable timerSub;
        private bool isStarted;
        
        private ReactiveProperty<long> timer = new ReactiveProperty<long>();
        public long Time => timer.Value;
        public string StringTime => Time.GetCharTime(2);
        public IObservable<long> TimerSub => timer.ObserveEveryValueChanged(v => v.Value);

        public UserLevelTimer(UserMapClassic userMap)
        {
            this.userMap = userMap;

            timer.Value = UserMapSaver.GetTime();
            
            userMap.OnStateUpdated += OnStateUpdated;
            userMap.CommandSystem.OnRedo += _ => UpdateTimer();
            UpdateTimer();
        }

        public UserRatingPlace Place { get; private set; }

        private void OnStateUpdated(UserMapState state)
        {
            UpdateTimer();

            if (state == UserMapState.Complete)
            {
                Place = new UserRatingPlace(DateTime.Now.Ticks, (int)Time);
                Game.User.Rating.GetRatingByType(userMap.Data.type).AddPlace(Place);
            }
        }

        private void UpdateTimer()
        {
            if(userMap.State == UserMapState.Play && userMap.IsGenerated)
                Start();
            else
                Stop();
        }
        
        public void Start()
        {
            if(isStarted)
                return;
            isStarted = true;
            
            timerSub?.Dispose();
            timerSub = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(OnTimer);
        }

        public void Stop()
        {
            if(!isStarted)
                return;
            isStarted = false;
            
            timerSub?.Dispose();
        }

        private void OnTimer(long l)
        {
            timer.Value += 1;
            UserMapSaver.SaveTime(timer.Value);
        }

        public void Dispose()
        {
            timerSub?.Dispose();
            timer?.Dispose();
        }
    }
}