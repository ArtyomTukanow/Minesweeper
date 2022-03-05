using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep5ClosedTile : StartTutorStep
    {
        public TutorStep5ClosedTile(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 5;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            wnd?.Hide();
        }
    }
}