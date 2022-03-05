using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep6BombsNumber : StartTutorStep
    {
        public TutorStep6BombsNumber(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 6;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            wnd?.Hide();
        }
    }
}