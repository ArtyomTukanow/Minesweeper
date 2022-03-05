using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep8AddFlag : StartTutorStep
    {
        public TutorStep8AddFlag(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 8;

        protected override void OnTileLongClick(Vector2Int pos)
        {
            base.OnTileLongClick(pos);

            if (pos == new Vector2Int(5, 1))
            {
                wnd?.Hide();
                Tutor.Map.UserMap.CommandSystem.SetFlag(pos, true);
            }
        }

    }
}