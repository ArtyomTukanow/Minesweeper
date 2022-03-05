using System.Collections.Generic;
using Controller.Map;
using Libraries.RSG;
using Tutors.Base;
using View.Map;

namespace Tutors.Start2
{
    public class StartTutor2 : ComplexTutor
    {
        public const string TUTOR_KEY = "start_2";

        public override string TutorKey => TUTOR_KEY;

        public override int StepsCount => 1;
        public override bool NeedSave => false;
        
        public override List<ComplexTutorStep> StepsFactory()
        {
            return new List<ComplexTutorStep>
            {
                new Start2TutorSteps(this),
            };
        }

        public MapTutorView Map => MapController.Instance.MapView as MapTutorView;

        protected override IPromise Init()
        {
            return Promise.Resolved();
        }
    }
}