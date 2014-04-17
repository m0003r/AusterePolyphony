using System;
using System.Collections.Generic;
using System.Linq;

namespace Compositor.Levels
{
    public class LeapSmoothList : List<LeapOrSmooth>
    {
        public int Reserve { get; private set; }
        public int Uncomp { get; private set; }

        public LeapSmoothList(IEnumerable<LeapOrSmooth> origList) : base(origList)
        {
            Reserve = Uncomp = 0;
        }

        public LeapSmoothList()
        {
            Reserve = Uncomp = 0;
        }

        public void DeleteLast()
        {
            if (Count == 0)
                return;

            if (this.Last().NotesCount == 2)
            {
                RemoveAt(Count - 1);
                return;
            }

            this.Last().Delete();

            Update();
        }

        public void Add(Note lastButOneNote, Note lastNote)
        {
            if (Count == 0)
            {
                Add(new LeapOrSmooth(lastButOneNote, lastNote));
                return;
            }

            var ls = this.Last();
            if (ls.CanAdd(lastNote))
                ls.Add(lastNote);
            else
            {
                ls = new LeapOrSmooth(lastButOneNote, lastNote);
                Add(ls);
            }

            Update();
        }

        internal void Update()
        {
            Reserve = 0;
            Uncomp = 0;

            foreach (var ls in this)
            {
                var deg = ls.Interval.Degrees;

                if (ls.IsSmooth)
                    UpdateUncompIfSmooth(deg);
                else
                    UpdateUncompIfLeap(deg);
            }
        }

        private void UpdateUncompIfLeap(int deg)
        {
            if (Reserve != 0)
                if (CoSign(Reserve, deg))
                    Uncomp += Reserve;
                else
                {
                    if (Math.Abs(Reserve) >= Math.Abs(deg * 2))
                        Reserve += deg * 2;
                    else
                        Reserve = 0;
                }

            Uncomp += deg;
        }

        private void UpdateUncompIfSmooth(int deg)
        {
            if ((Reserve == 0) || (CoSign(Reserve, deg)))
                Reserve += deg;
            else
                if (Math.Abs(Reserve) >= Math.Abs(deg * 2))
                    Reserve += deg * 2;
                else
                    Reserve = deg + Reserve / 2;

            if (Uncomp == 0) return;

            if (CoSign(Uncomp, deg))
                Uncomp += deg / 2;
            else
                if (Math.Abs(Uncomp) <= Math.Abs(deg * 2))
                    Uncomp = 0;
                else
                    Uncomp += deg * 2;
        }

        private static bool CoSign(int a, int b)
        {
            return (Math.Sign(a) == Math.Sign(b));
        }
    }
}