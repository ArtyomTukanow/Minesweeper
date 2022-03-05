using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep7AddFlag : StartTutorStep
    {
        public TutorStep7AddFlag(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 7;

        protected override void OnTileLongClick(Vector2Int pos)
        {
            base.OnTileLongClick(pos);
            if (pos == new Vector2Int(2, 6))
            {
                wnd?.Hide();
                Tutor.Map.UserMap.CommandSystem.SetFlag(pos, true);
            }
        }
    }
}