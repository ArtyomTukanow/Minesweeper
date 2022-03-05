using Libraries.RSG;

namespace Tutors.Base
{
    public abstract class ComplexTutorStep
    {
        public ComplexTutor Tutor { get; }

        /// <summary>
        /// Запускается, только если шаг тутора еще не был пройден
        /// </summary>
        public abstract IPromise Run();


        protected bool disposed = false;
        public void DisposeOnce()
        {
            if(disposed)
                return;
            disposed = true;
            Dispose();
        }

        protected virtual void Dispose()
        {
            
        }

        public ComplexTutorStep(ComplexTutor tutor)
        {
            Tutor = tutor;
        }
        
        // private Dictionary<MapObject, IPromise<TutorHand>> _tutorHands = new Dictionary<MapObject, IPromise<TutorHand>>();
        //
        // protected void AddTutorHandTo(MapObject mapObject)
        // {
        //     if(!mapObject)
        //         return;
        //     
        //     RemoveTutorHandFrom(mapObject);
        //     _tutorHands[mapObject] = TutorHand.CreateHand(mapObject)
        //         .Then(hand => hand.SetOffset(new Vector2(0, 0.5f)));
        // }
        //
        // protected void RemoveTutorHandFrom(MapObject mapObject)
        // {
        //     if(!mapObject)
        //         return;
        //
        //     if (_tutorHands.ContainsKey(mapObject))
        //     {
        //         _tutorHands[mapObject].Then(hand =>
        //         {
        //             if(hand)
        //                 Object.Destroy(hand.gameObject);
        //         });
        //         _tutorHands.Remove(mapObject);
        //     }
        // }
    }
}