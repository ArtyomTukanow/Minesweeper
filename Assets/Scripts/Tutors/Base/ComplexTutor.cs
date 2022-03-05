using System.Collections.Generic;
using System.Linq;
using Controller.Map;
using Core;
using Libraries.RSG;
using UnityEngine;

namespace Tutors.Base
{
    public abstract class ComplexTutor
    {
        public bool Disposed { get; private set; }
        
        public abstract string TutorKey { get; }
        public abstract int StepsCount { get; }
        public abstract List<ComplexTutorStep> StepsFactory();

        private List<ComplexTutorStep> _steps;

        private bool IsTutorOver => CurrentStep >= StepsCount;
        public virtual bool NeedSave => true;

        private int currentStep;
        private int CurrentStep
        {
            get => NeedSave ? Game.User.Settings.GetInt(TutorKey) : currentStep;
            set
            {
                if (NeedSave)
                    Game.User.Settings.Set(TutorKey, value);
                else
                    currentStep = value;
            }
        }

        private static List<ComplexTutor> runningTutors = new List<ComplexTutor>();

        public static T RunTutor<T>() where T : ComplexTutor, new()
        {
            return runningTutors.OfType<T>().FirstOrDefault() ?? (T)new T().Start();
        }

        private ComplexTutor Start()
        {
            if (IsTutorOver)
                return this;
            
            runningTutors.Add(this);
            Debug.Log("Start tutor: " + GetType().Name);
            InitSteps();

            Promise.Resolved()
                .Then(SetStateWithoutTutor)
                .Then(LogInit)
                .Then(Init)
                .Then(RunAllSteps)
                .Then(OnTutorEnd)
                .Finally(Dispose);

            AddListeners();

            return this;
        }

        private void AddListeners()
        {
            MapController.OnMapDestroyed += OnMapDestroyed;
        }

        private void RemoveListeners()
        {
            MapController.OnMapDestroyed -= OnMapDestroyed;
        }

        private void OnMapDestroyed(MapController obj) => Dispose();
        

        private void LogInit()
        {
            Debug.Log("Init tutor: " + GetType().Name);
        }

        /// <summary>
        /// До запуска тутора может пройти много времени. PreInit 
        /// </summary>
        protected virtual void SetStateWithoutTutor()
        {
            
        }
        
        protected abstract IPromise Init();
        
        private void InitSteps()
        {
            _steps = StepsFactory();
        }

        private IPromise RunAllSteps()
        {
            var promise = Promise.Resolved();
            for(var i = 0; i < StepsCount; i ++)
            {
                var stepId = i;
                promise = promise.Then(() => TryRunStep(stepId));
            }

            return promise;
        }

        protected IPromise TryRunStep(int stepId)
        {
            if (stepId >= StepsCount)
                return Promise.Resolved();
            
            //пропускаем пройденные шаги
            if (CurrentStep > stepId)
            {
                _steps[stepId].DisposeOnce();
                return Promise.Resolved();
            }

            return Promise.Resolved()
                          .Then(() => _steps[stepId].Run())
                          .Then(() => _steps[stepId].DisposeOnce())
                          .Then(() => CurrentStep = stepId + 1);
        }

        protected virtual void OnTutorEnd() { }

        public virtual void Dispose()
        {
            if (Disposed)
                return;
            
            Disposed = true;
            RemoveListeners();
            
            runningTutors.Remove(this);
            Debug.Log("Dispose tutor: " + GetType().Name);
            
            foreach (var step in _steps)
                step.DisposeOnce();
        }
    }
}