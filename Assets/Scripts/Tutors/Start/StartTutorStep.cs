using Libraries.RSG;
using Tutors.Base;
using UnityEngine;
using View.Window.Windows;

namespace Tutors.Start
{
    public abstract class StartTutorStep : ComplexTutorStep
    {
        protected abstract int STEP { get; }
        
        public StartTutor1 Tutor => base.Tutor as StartTutor1;
        
        public StartTutorStep(StartTutor1 tutor) : base(tutor)
        {
        }

        protected StartTutorWindow wnd;

        public override IPromise Run()
        {
            AddListeners();
            wnd = StartTutorWindow.Of(STEP);
            return wnd.ClosePromise;
        }

        private void AddListeners()
        {
            if(disposed || !Tutor.Map)
                return;
            
            Tutor.Map.OnTileClickEvent += OnTileClick;
            Tutor.Map.OnTileLongClickEvent += OnTileLongClick;
        }

        private void RemoveListeners()
        {
            if (!Tutor.Map)
                return;
            
            Tutor.Map.OnTileClickEvent -= OnTileClick;
            Tutor.Map.OnTileLongClickEvent -= OnTileLongClick;
        }

        protected virtual void OnTileClick(Vector2Int pos)
        {
            
        }

        protected virtual void OnTileLongClick(Vector2Int pos)
        {
            
        }


        protected override void Dispose()
        {
            wnd?.Hide();
            RemoveListeners();
            base.Dispose();
        }
    }
}