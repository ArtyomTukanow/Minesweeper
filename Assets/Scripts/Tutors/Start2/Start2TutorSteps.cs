using Libraries.RSG;
using Tutors.Base;
using UnityEngine;
using View.Window.Windows;

namespace Tutors.Start2
{
    public class Start2TutorSteps : ComplexTutorStep
    {
        public StartTutor2 Tutor => base.Tutor as StartTutor2;
        
        public Start2TutorSteps(StartTutor2 tutor) : base(tutor)
        {
        }

        private StartTutorWindow wnd;

        public override IPromise Run()
        {
            return Promise.Resolved()
                          .Then(Window1)
                          .Then(Window2)
                          .Then(Window3)
                          .Then(Window4)
                          .Then(Window5)
                          .Then(Window6);
        }

        private IPromise Window1()
        {
            wnd = StartTutorWindow.Of(100);
            Tutor.Map.OnTileClickEvent += OnTileClick;
            return wnd.ClosePromise;
            
            void OnTileClick(Vector2Int pos)
            {
                Tutor.Map.OnTileClickEvent -= OnTileClick;
                wnd?.Hide();
                Tutor?.Map.UserMap.CommandSystem.Open(new Vector2Int(2, 2));
            }
        }

        private IPromise Window2()
        {
            wnd = StartTutorWindow.Of(101);
            Tutor.Map.OnTileClickEvent += OnTileClick;
            return wnd.ClosePromise;
            
            void OnTileClick(Vector2Int pos)
            {
                if (pos == new Vector2Int(4, 3))
                {
                    Tutor.Map.OnTileClickEvent -= OnTileClick;
                    wnd?.Hide();
                    Tutor?.Map.UserMap.CommandSystem.OnClickTile(new Vector2Int(4, 3));
                }
            }
        }

        private IPromise Window3()
        {
            wnd = StartTutorWindow.Of(102);
            Tutor.Map.OnTileClickEvent += OnTileClick;
            return wnd.ClosePromise;
            
            void OnTileClick(Vector2Int pos)
            {
                if (pos == new Vector2Int(4, 5))
                {
                    Tutor.Map.OnTileClickEvent -= OnTileClick;
                    wnd?.Hide();
                    Tutor?.Map.UserMap.CommandSystem.OnClickTile(new Vector2Int(4, 5));
                }
            }
        }

        private IPromise Window4()
        {
            wnd = StartTutorWindow.Of(103);
            Tutor.Map.OnTileClickEvent += OnTileClick;
            return wnd.ClosePromise;
            
            void OnTileClick(Vector2Int pos)
            {
                if (pos == new Vector2Int(4, 8))
                {
                    Tutor.Map.OnTileClickEvent -= OnTileClick;
                    wnd?.Hide();
                    Tutor?.Map.UserMap.CommandSystem.OnClickTile(new Vector2Int(4, 8));
                }
            }
        }

        private IPromise Window5()
        {
            wnd = StartTutorWindow.Of(104);
            Tutor.Map.OnTileClickEvent += OnTileClick;
            return wnd.ClosePromise;
            
            void OnTileClick(Vector2Int pos)
            {
                if (pos == new Vector2Int(7, 5))
                {
                    Tutor.Map.OnTileClickEvent -= OnTileClick;
                    wnd?.Hide();
                    Tutor?.Map.UserMap.CommandSystem.OnClickTile(new Vector2Int(7, 5));
                }
            }
        }

        private IPromise Window6()
        {
            wnd = StartTutorWindow.Of(105);
            Tutor.Map.OnTileClickEvent += OnTileClick;
            return wnd.ClosePromise;
            
            void OnTileClick(Vector2Int pos)
            {
                if (pos == new Vector2Int(8, 6))
                {
                    Tutor.Map.OnTileClickEvent -= OnTileClick;
                    wnd?.Hide();
                    Tutor?.Map.UserMap.CommandSystem.OnClickTile(new Vector2Int(8, 6));
                }
            }
        }

        protected override void Dispose()
        {
            wnd?.Hide();
            base.Dispose();
        }
    }
}