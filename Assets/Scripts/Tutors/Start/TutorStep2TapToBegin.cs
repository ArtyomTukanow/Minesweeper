using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep2TapToBegin : StartTutorStep
    {
        public TutorStep2TapToBegin(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 2;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            wnd?.Hide();
            Tutor.Map.UserMap.CommandSystem.Open(new Vector2Int(0, 1));
        }
    }
}