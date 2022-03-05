using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep9FreeTile : StartTutorStep
    {
        public TutorStep9FreeTile(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 9;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            wnd?.Hide();
        }
    }
}