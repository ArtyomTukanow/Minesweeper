using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep13OpenByNumber : StartTutorStep
    {
        public TutorStep13OpenByNumber(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 13;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            if (pos == new Vector2Int(8, 5))
            {
                wnd?.Hide();
                Tutor.Map.UserMap.CommandSystem.OnClickTile(pos);
            }
        }
    }
}