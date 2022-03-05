using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep10OpenFreeTile : StartTutorStep
    {
        public TutorStep10OpenFreeTile(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 10;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);

            if (pos == new Vector2Int(5, 0))
            {
                wnd?.Hide();
                Tutor.Map.UserMap.CommandSystem.Open(pos);
            }
        }
    }
}