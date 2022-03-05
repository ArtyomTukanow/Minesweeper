using System.Collections.Generic;
using Controller.Map;
using Libraries.RSG;
using Tutors.Base;
using View.Map;

namespace Tutors.Start
{
    public class StartTutor1 : ComplexTutor
    {
        public const string TUTOR_KEY = "start";

        public override string TutorKey => TUTOR_KEY;

        public override int StepsCount => 14;
        public override bool NeedSave => false;
        
        public override List<ComplexTutorStep> StepsFactory()
        {
            return new List<ComplexTutorStep>
            {
                new TutorStep1Welcome(this),
                new TutorStep2TapToBegin(this),
                new TutorStep3HowToWin(this),
                new TutorStep4BombsNumber(this),
                new TutorStep5ClosedTile(this),
                new TutorStep6BombsNumber(this),
                new TutorStep7AddFlag(this),
                new TutorStep8AddFlag(this),
                new TutorStep9FreeTile(this),
                new TutorStep10OpenFreeTile(this),
                new TutorStep11(this),
                new TutorStep12FlagByNumber(this),
                new TutorStep13OpenByNumber(this),
                new TutorStep14End(this),
            };
        }

        public MapTutorView Map => MapController.Instance.MapView as MapTutorView;

        protected override IPromise Init()
        {
            return Promise.Resolved();
        }
    }
}