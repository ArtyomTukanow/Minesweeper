using Libraries.RSG;
using UnityEngine;
using View.Window.Windows;

namespace Tutors.Start
{
    public class TutorStep11 : StartTutorStep
    {
        public TutorStep11(StartTutor1 tutor) : base(tutor)
        {
        }

        protected override int STEP => 11;

        protected override void OnTileClick(Vector2Int pos)
        {
            base.OnTileClick(pos);
            wnd?.Hide();
        }
    }
}