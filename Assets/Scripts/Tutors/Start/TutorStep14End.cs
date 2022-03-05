using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep14End : StartTutorStep
    {
        public TutorStep14End(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 14;

        protected override void OnTileClick(Vector2Int pos)
        {
            wnd?.Hide();
            base.OnTileClick(pos);
        }
    }
}