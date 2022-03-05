using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep1Welcome : StartTutorStep
    {
        public TutorStep1Welcome(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 1;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            wnd?.Hide();
        }
    }
}