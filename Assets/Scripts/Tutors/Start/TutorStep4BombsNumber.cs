using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep4BombsNumber : StartTutorStep
    {
        public TutorStep4BombsNumber(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 4;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            wnd?.Hide();
        }
    }
}