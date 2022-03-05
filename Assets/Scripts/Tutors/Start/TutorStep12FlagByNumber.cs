using UnityEngine;

namespace Tutors.Start
{
    public class TutorStep12FlagByNumber : StartTutorStep
    {
        public TutorStep12FlagByNumber(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 12;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            if (pos == new Vector2Int(7, 5))
            {
                wnd?.Hide();
                Tutor.Map.UserMap.CommandSystem.OnClickTile(pos);
            }
        }
    }
}