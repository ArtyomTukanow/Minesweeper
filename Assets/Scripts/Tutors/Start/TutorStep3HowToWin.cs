using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep3HowToWin : StartTutorStep
    {
        public TutorStep3HowToWin(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 3;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            wnd?.Hide();
        }
    }
}